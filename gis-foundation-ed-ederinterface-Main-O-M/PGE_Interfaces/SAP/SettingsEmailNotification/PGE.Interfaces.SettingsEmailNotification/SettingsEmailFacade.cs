using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using Diagnostics = PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;


namespace PGE.Interfaces.SettingsEmailNotification
{
    public class SettingsEmailFacade : IDisposable
    {
        private string _settingsConnectionString;
        private OracleConnection _settingsOracleConnection;
        //private OracleConnection _sdeOracleConnection;
        private OracleTransaction _oraTran;
        private bool _disposed = false;
        private const string VIEW_SUFFIX = "ZZ_MV_";

        private const string DIV_KEY = "DIVISION";
        private const string DIST_KEY = "DISTRICT";
        private const string LO_KEY = "LOCALOFFICE";

        private esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private LicenseInitializer _licenceManager = new LicenseInitializer();

        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SettingsEmailNotification.log4net.config");
        
        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public SettingsEmailFacade()
        {
            try
            {

                // M4JF EDGISREARCH 919
               // _settingsConnectionString = ConfigurationManager.AppSettings["SettingsConnectionString"];
                _settingsConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"].ToUpper());
                _settingsOracleConnection = new OracleConnection(_settingsConnectionString);

                _settingsOracleConnection.Open();
            }
            catch (Exception ex)
            {
                _logger.Error("SettingsEmailFacade() - " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Database transaction. 
        /// </summary>
        public OracleTransaction Transaction
        {
            get
            {
                return this._oraTran;
            }
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this._oraTran = this._settingsOracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
            using (var cmd = new OracleCommand("LOCK TABLE FEATURES_TO_NOTIFY IN EXCLUSIVE MODE", _settingsOracleConnection))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }


        /// <summary>
        /// Sends emails to recepients
        /// </summary>
        /// <returns>boolean</returns>
        public bool SendNotification()
        {
            List<Email> emailLists = new List<Email>();
            bool success = false;
            if (CheckLicense(_licenceManager))
            {
                try
                {
                    success = PopulateLocalOfficeIDsAndObjectIDs();

                    string emailOption = ConfigurationManager.AppSettings["EmailOption"];
                    IBuildEmail emails;

                    if (string.Compare(emailOption, "EmailByDivDistLO") == 0)
                        emails = new EmailByDivDistLO(_settingsOracleConnection);
                    else
                        emails = new EmailByRecipient(_settingsOracleConnection);

                    if (success)
                        emailLists = emails.BuildEmails();

                    //send emails to recepients
                    if (emailLists.Count > 0)
                    {
                        foreach (Email email in emailLists)
                        {
                            success = email.SendMail();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("SendNotification() - " + ex.Message + Environment.NewLine + ex.StackTrace);
                    success = false;
                    throw ex;
                }

                _licenceManager.ShutdownApplication();
            }
            else
            {
                throw new ApplicationException("ESRI ArcGIS license not available");
            }

            return success;
        }

        /// <summary>
        /// Populates LocalOfficeIDs and ObjectIDs of features in settings table
        /// </summary>
        /// <returns>boolean</returns>
        private bool PopulateLocalOfficeIDsAndObjectIDs()
        {
            bool success = true;
            int iUpdCnt = -1;
            IWorkspace ws = Common.OpenWorkspace();
            IWorkspace wsSub = Common.OpenSubWorkspace();
            string featureClassName = string.Empty,
                   sRegion = string.Empty;
            try
            {
                Dictionary<string, string> pDiv_Reg = new Dictionary<string, string>();
                using (OracleCommand oDiv_Reg = new OracleCommand("SELECT DIVISIONNAME, REGIONNAME FROM GIS_DIV_REGION", _settingsOracleConnection))
                {
                    oDiv_Reg.CommandType = CommandType.Text;
                    using (OracleDataReader pODReader = oDiv_Reg.ExecuteReader())
                    {
                        while (pODReader.Read())
                        {
                            if (!pDiv_Reg.ContainsKey(Convert.ToString(pODReader["DIVISIONNAME"])))
                                pDiv_Reg.Add(Convert.ToString(pODReader["DIVISIONNAME"]), Convert.ToString(pODReader["REGIONNAME"]));
                            //pODReader.NextResult();
                        }
                    }
                }


                using (OracleCommand settingsCmd = new OracleCommand("SELECT GLOBAL_ID, FEATURE_CLASS_NAME FROM FEATURES_TO_NOTIFY", _settingsOracleConnection))
                {
                    settingsCmd.CommandType = CommandType.Text;

                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    {
                        while (settingsDataReader.Read())
                        {
                            string globalID = settingsDataReader["GLOBAL_ID"] as string;
                            featureClassName = settingsDataReader["FEATURE_CLASS_NAME"] as string;

                            if (!ConfigurationManager.AppSettings.AllKeys.Contains(featureClassName))
                                continue;
                            IFeatureClass featClass = null;
                            //extract values from GIS
                            if(featureClassName.ToUpper().Contains("SUB"))
                                featClass = (wsSub as IFeatureWorkspace).OpenFeatureClass(featureClassName);
                            else
                                featClass = (ws as IFeatureWorkspace).OpenFeatureClass(featureClassName);
                            string fieldString = ConfigurationManager.AppSettings[featureClassName];
                            string[] fields = Regex.Split(fieldString, ",");
                            for (int i = 0; i <= fields.Length - 1; i++)
                            {
                                fields[i] = fields[i].Trim().ToUpper();
                            }
                            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
                            StringBuilder sb = new StringBuilder();
                            IQueryFilter qf = new QueryFilterClass();
                            qf.SubFields = fieldString.Replace("REGION","DIVISION");
                            qf.WhereClause = string.Format("GLOBALID = '{0}'", globalID);

                            IFeatureCursor cursor = featClass.Search(qf, true);
                            try
                            {
                                IFeature feat;
                                while ((feat = cursor.NextFeature()) != null)
                                {
                                    for (int i = 0; i <= fields.Length - 1; i++)
                                    {
                                        if (fields[i] == "REGION")
                                            fields[i] = "DIVISION";
                                        //    fieldValues.Add(fields[i], feat.get_Value(featClass.FindField("DIVISION")));
                                        //else
                                        fieldValues.Add(fields[i], feat.get_Value(featClass.FindField(fields[i])));
                                        IField2 fld2 = feat.Fields.Field[featClass.FindField(fields[i])] as IField2;
                                        ICodedValueDomain cvDomain = fld2.Domain as ICodedValueDomain;
                                        List<string> domainValues = new List<string>();
                                        Dictionary<string, string> domainV = new Dictionary<string, string>();
                                        if (cvDomain != null)
                                        {
                                            for (int j = 0; j <= cvDomain.CodeCount - 1; j++)
                                            {
                                                if (cvDomain.get_Value(j).ToString().Trim() == fieldValues[fields[i]].ToString().Trim())
                                                {
                                                    fieldValues[fields[i]] = cvDomain.get_Name(j).ToString();
                                                }
                                            }
                                        }

                                        if (fields[i] != "LOCALOFFICEID")
                                        {
                                            string value = fieldValues[fields[i]].ToString();
                                            if (fields[i] == "DIVISION")
                                            {
                                                if (pDiv_Reg.ContainsKey(value))
                                                {
                                                    value = pDiv_Reg[value];
                                                }
                                                fields[i] = "REGION";
                                            }
                                            if (i == fields.Length - 1)
                                            {
                                                sb.Append(string.Format("{0}: {1} ", fields[i], value));
                                            }
                                            else
                                            {
                                                sb.Append(string.Format("{0}: {1} | ", fields[i], value));
                                            }
                                        }
                                    }
                                }
                            }
                            catch { throw; }
                            finally
                            {
                                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0);}
                                cursor = null;
                            }

                            
                            if (fieldValues.ContainsKey("OBJECTID") && fieldValues.ContainsKey("LOCALOFFICEID"))
                            {
                                string objID = fieldValues["OBJECTID"].ToString();
                                string localOfficeID = fieldValues["LOCALOFFICEID"].ToString();
                                sRegion = fieldValues["DIVISION"].ToString();
                                if (pDiv_Reg.ContainsKey(sRegion))
                                {
                                    sRegion = pDiv_Reg[sRegion];
                                }
                                using (OracleCommand cmd = new OracleCommand())
                                {
                                    cmd.Connection = _settingsOracleConnection;
                                    //cmd.CommandText = string.Format("UPDATE FEATURES_TO_NOTIFY SET OBJECTID = {0}, LOCALOFFICEID = '{1}', ATTRIBUTEINFO = '{2}' WHERE GLOBAL_ID = '{3}'",
                                    // objID, localOfficeID, sb.ToString(), globalID);
                                    cmd.CommandText = string.Format("UPDATE FEATURES_TO_NOTIFY SET OBJECTID = {0}, LOCALOFFICEID = '{1}', ATTRIBUTEINFO = '{2}', REGION = '{3}' WHERE GLOBAL_ID = '{4}'",
                                      objID, localOfficeID, sb.ToString(), sRegion, globalID);
                                    iUpdCnt = cmd.ExecuteNonQuery();
                                    
                                }
                            }

                            if (iUpdCnt != -1 && sRegion != null && (!string.IsNullOrEmpty(sRegion)) && sRegion != "")
                            {
                                //Populate Recepient column
                                PopulateRecepientList(featureClassName, sRegion, _settingsOracleConnection);
                            }
                        }

                    } //settingsDataReader
                } //settingsCmd

                #region Commented old recepient lst population
                /*
                //populate recepient list in FEATURES_TO_NOTIFY table
                using (OracleCommand cmd = new OracleCommand())
                {
                   StringBuilder updatesql = new StringBuilder();
                   updatesql.Append("update FEATURES_TO_NOTIFY ftn set ftn.RECIPIENT = (select distinct EMAILDISTRIBUTIONNAME from "); 
                   updatesql.Append("(WITH EmailDistName AS ( SELECT e.EMAILDISTRIBUTIONNAME, s.LOCALOFFICEID, s.DISTRICT, s.DIVISION, ROW_NUMBER() OVER (PARTITION BY s.DIVISION ORDER BY e.EMAILDISTRIBUTIONNAME ASC) AS rn ");
                   updatesql.Append("FROM SETTINGSWORKINGASSIGNMENT s inner join EMAILDISTRIBUTIONLIST e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME) ");
                   updatesql.Append("SELECT EMAILDISTRIBUTIONNAME, LOCALOFFICEID, DISTRICT, DIVISION FROM EmailDistName WHERE rn = 1) x ");
                   updatesql.Append("where upper(ftn.LOCALOFFICEID) = upper(x.LOCALOFFICEID) and upper(ftn.DIVISION) = upper(x.DIVISION) and upper(ftn.DISTRICT) = upper(x.DISTRICT))");

                   cmd.Connection = _settingsOracleConnection;
                   cmd.CommandText = updatesql.ToString();
                   int count = cmd.ExecuteNonQuery();

                   //In case there is no location id in SETTINGSWORKINGASSIGNMENT check if there is any recepient list for District-Division
                   updatesql = new StringBuilder();
                   updatesql.Append("update FEATURES_TO_NOTIFY ftn set ftn.RECIPIENT = (select distinct EMAILDISTRIBUTIONNAME from "); 
                   updatesql.Append("(WITH EmailDistName AS ( SELECT e.EMAILDISTRIBUTIONNAME, s.LOCALOFFICEID, s.DISTRICT, s.DIVISION, ROW_NUMBER() OVER (PARTITION BY s.DIVISION ORDER BY e.EMAILDISTRIBUTIONNAME ASC) AS rn ");
                   updatesql.Append("FROM SETTINGSWORKINGASSIGNMENT s inner join EMAILDISTRIBUTIONLIST e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME) ");
                   updatesql.Append("SELECT EMAILDISTRIBUTIONNAME, LOCALOFFICEID, DISTRICT, DIVISION FROM EmailDistName WHERE rn = 1) x ");
                   updatesql.Append("where x.LOCALOFFICEID is null and upper(ftn.DIVISION) = upper(x.DIVISION) and  upper(ftn.DISTRICT) = upper(x.DISTRICT)) where ftn.RECIPIENT is NULL");

                   cmd.CommandText = updatesql.ToString();
                   count = cmd.ExecuteNonQuery();

                   //No local office id and district in SETTINGSWORKINGASSIGNMENT check if there is any recepient list for Division
                   updatesql = new StringBuilder();
                   updatesql.Append("update FEATURES_TO_NOTIFY ftn set ftn.RECIPIENT = (select distinct EMAILDISTRIBUTIONNAME from ");
                   updatesql.Append("(WITH EmailDistName AS ( SELECT e.EMAILDISTRIBUTIONNAME, s.LOCALOFFICEID, s.DISTRICT, s.DIVISION, ROW_NUMBER() OVER (PARTITION BY s.DIVISION ORDER BY e.EMAILDISTRIBUTIONNAME ASC) AS rn ");
                   updatesql.Append("FROM SETTINGSWORKINGASSIGNMENT s inner join EMAILDISTRIBUTIONLIST e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME) ");
                   updatesql.Append("SELECT EMAILDISTRIBUTIONNAME, LOCALOFFICEID, DISTRICT, DIVISION FROM EmailDistName WHERE rn = 1) x ");
                   updatesql.Append("where x.LOCALOFFICEID is null and x.DISTRICT is null and upper(ftn.DIVISION) = upper(x.DIVISION)) where ftn.RECIPIENT is NULL");

                   cmd.CommandText = updatesql.ToString();
                   count = cmd.ExecuteNonQuery();
                }
                 * */
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error("PopulateLocalOfficeIDsAndObjectIDs() - " + ex.Message);
                success = false;
                throw ex;
            }

            return success;
        }

        public string PopulateRegion(string sFeatureClassName, string sDivision, OracleConnection _settingsOracleConnection)
        {
            string sRegion = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error("PopulateRegion() - " + ex.Message);
                sRegion = null;
                throw ex;
            }
            return sRegion;
        }

        public bool PopulateRecepientList(string sFeatureClassName, string sRegion, OracleConnection _settingsOracleConnection)
        {
            bool success = true;
            string strCmd = string.Empty;
            StringBuilder sbRecepientList = new StringBuilder();
            try
            {
                strCmd = "SELECT CAPACITY_GRP_ENOTIFY, RELIABILITY_GRP_ENOTIFY, DISTGEN_GRP_ENOTIFY, OPERATING_GRP_ENOTIFY, NETWORKENG_GRP_ENOTIFY FROM NOTIFY_EMAIL_EQUIP where FEATURE_CLASS_NAME = '" + sFeatureClassName + "'";
                using (OracleCommand settingsCmd = new OracleCommand(strCmd, _settingsOracleConnection))
                {
                    settingsCmd.CommandType = CommandType.Text;

                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    {
                        while (settingsDataReader.Read())
                        {
                            string sCapacity = settingsDataReader["CAPACITY_GRP_ENOTIFY"] as string;
                            string sReliability = settingsDataReader["RELIABILITY_GRP_ENOTIFY"] as string;
                            string sDistGen = settingsDataReader["DISTGEN_GRP_ENOTIFY"] as string;
                            string sOperating = settingsDataReader["OPERATING_GRP_ENOTIFY"] as string;
                            string sNetworking = settingsDataReader["NETWORKENG_GRP_ENOTIFY"] as string;
                            string sWorkgroup = string.Empty;
                            List<string> liWorkgrp = new List<string>();
                            if (sCapacity == "Y")
                            {
                                liWorkgrp.Add("Capacity");
                            }
                            if (sReliability == "Y")
                            {
                                liWorkgrp.Add("Reliability");
                            }
                            if (sDistGen == "Y")
                            {
                                liWorkgrp.Add("Distributed Generation");
                            }
                            if (sOperating == "Y")
                            {
                                liWorkgrp.Add("Operations");
                            }
                            if (sNetworking == "Y")
                            {
                                liWorkgrp.Add("Network Eng. Group");
                            }
                            string sWhereClause = string.Empty;
                            for (int iWorkgrpCnt = 0; iWorkgrpCnt < liWorkgrp.Count; iWorkgrpCnt++)
                            {
                                sWhereClause += "WORKGROUP = '" + liWorkgrp[iWorkgrpCnt] + "' OR ";
                            }
                            sWhereClause = sWhereClause.Remove(sWhereClause.Length - 3);
                            string sCmd = "SELECT DISTRIBUTIONLIST FROM EMAILADDRESSESLIST WHERE REGION = '" + sRegion + "' AND (" + sWhereClause + ")";
                            using (OracleCommand setCmd = new OracleCommand(sCmd, _settingsOracleConnection))
                            {
                                setCmd.CommandType = CommandType.Text;

                                using (OracleDataReader setDataReader = setCmd.ExecuteReader())
                                {
                                    while (setDataReader.Read())
                                    {
                                        string sRecepient = setDataReader["DISTRIBUTIONLIST"] as string;
                                        sbRecepientList.Append(sRecepient);
                                        sbRecepientList.Append(", ");
                                    }
                                    sbRecepientList = sbRecepientList.Remove(sbRecepientList.Length - 2, 2);
                                }
                            }
                        }
                    }
                }
                int count = -1;
                string sSql = "update FEATURES_TO_NOTIFY ftn set ftn.RECIPIENT = '" + sbRecepientList.ToString() + "' where ftn.FEATURE_CLASS_NAME = '" + sFeatureClassName + "' AND REGION = '" + sRegion + "'" ;
                using (OracleCommand cmd = new OracleCommand(sSql, _settingsOracleConnection))
                {                    
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PopulateRecepientList() - " + ex.Message);
                success = false;
                throw ex;
            }
            return success;
        }

        /// <summary>
        /// Moves sent email records to history table
        /// </summary>
        /// <returns></returns>
        public bool MoveToHistoryTable()
        {
            bool success = true;
            string insertSql = "insert into FEATURES_NOTIFICATION_HISTORY(GLOBAL_ID, FEATURE_CLASS_NAME, LOCALOFFICEID, DISTRICT, DIVISION, RECIPIENT, OBJECTID, ATTRIBUTEINFO, REGION) select GLOBAL_ID, FEATURE_CLASS_NAME, LOCALOFFICEID, DISTRICT, DIVISION, RECIPIENT, OBJECTID, ATTRIBUTEINFO, REGION from FEATURES_TO_NOTIFY where REGION is not NULL and RECIPIENT is not NULL"; //add region
            string deleteSql = "delete from FEATURES_TO_NOTIFY where RECIPIENT is not NULL and REGION is not NULL";//modify for region
            //Added REGION to the queries
            //string insertSql = "insert into FEATURES_NOTIFICATION_HISTORY(GLOBAL_ID, FEATURE_CLASS_NAME, LOCALOFFICEID, DISTRICT, DIVISION, RECIPIENT, OBJECTID, ATTRIBUTEINFO) select GLOBAL_ID, FEATURE_CLASS_NAME, LOCALOFFICEID, DISTRICT, DIVISION, RECIPIENT, OBJECTID, ATTRIBUTEINFO from FEATURES_TO_NOTIFY where LOCALOFFICEID is not NULL and RECIPIENT is not NULL";
            //string deleteSql = "delete from FEATURES_TO_NOTIFY where LOCALOFFICEID is not null and RECIPIENT is not NULL";
            using (OracleCommand cmd = new OracleCommand())
            {
                try
                {
                    cmd.Connection = _settingsOracleConnection;
                    cmd.CommandText = insertSql;
                    cmd.CommandType = CommandType.Text;
                    int recNum= cmd.ExecuteNonQuery();

                    cmd.CommandText = deleteSql;
                    recNum = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("MoveToHistoryTable() - " + ex.Message);
                    success = false;
                    throw ex;
                }
            }

            return success;
        }


        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                _logger.Error("Failed to initialize license : " + ex.Message);
            }

            return isOk;
        }

        /// <summary>
        /// Disposes oracle connection object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing)
                {
                    // managed resources
                    if (this._oraTran != null)
                    {
                        this._oraTran.Dispose();
                    }

                    if (this._settingsOracleConnection != null)
                    {
                        this._settingsOracleConnection.Close();
                        this._settingsOracleConnection.Dispose();
                    }

                }

                this._oraTran = null;
                this._settingsOracleConnection = null;
                this._disposed = true;
            }
        }
    }
}
