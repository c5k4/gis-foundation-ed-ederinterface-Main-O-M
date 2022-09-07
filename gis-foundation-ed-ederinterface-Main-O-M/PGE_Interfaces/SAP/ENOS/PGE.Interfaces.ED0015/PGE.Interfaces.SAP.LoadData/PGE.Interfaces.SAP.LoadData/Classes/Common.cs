using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using PGE.Interfaces.SAP.LoadData.Email;
using System.Reflection;
using Miner.Interop.Process;

namespace PGE.Interfaces.SAP.LoadData.Classes
{
    public static class Common
    {
        public static IMMAppInitialize _mmAppInit;
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        public static string GetCorrespondingStage2Tabname(string mainTableName)
        {
            switch (mainTableName)
            {
                case "EDSETT.GENERATIONINFO":
                    return "PGEDATA.PGEDATA_GENERATIONINFO_STAGE";
                case "EDSETT.SM_GENERATION":
                    return "PGEDATA.PGEDATA_SM_GENERATION_STAGE";
                case "EDSETT.SM_PROTECTION":
                    return "PGEDATA.PGEDATA_SM_PROTECTION_STAGE";
                case "EDSETT.SM_RELAY":
                    return "PGEDATA.PGEDATA_SM_RELAY_STAGE";
                case "EDSETT.SM_GENERATOR":
                    return "PGEDATA.PGEDATA_SM_GENERATOR_STAGE";
                case "EDSETT.SM_GEN_EQUIPMENT":
                    return "PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE";
                default:
                    return string.Empty;
            }
        }

        public static string GetCorrespondingStage2TabnameFromObjType(Type objType)
        {
            if (objType == typeof(SmGeneration))
            {
                return "PGEDATA.PGEDATA_SM_GENERATION_STAGE";
            }else if (objType == typeof(SmProtection))
            {
                return "PGEDATA.PGEDATA_SM_PROTECTION_STAGE";
            }
            else if (objType == typeof(SmRelay))
            {
                return "PGEDATA.PGEDATA_SM_RELAY_STAGE";
            }
            else if (objType == typeof(SmGenerator))
            {
                return "PGEDATA.PGEDATA_SM_GENERATOR_STAGE";
            }
            else if (objType == typeof(SmGenEquipment))
            {
                return "PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE";
            }
            else if (objType == typeof(GenerationInfo))
            {
                return "PGEDATA.PGEDATA_GENERATIONINFO_STAGE";
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetCorrespondingMainTabname(string stage2TableName)
        {
            switch (stage2TableName)
            {
                case "PGEDATA.PGEDATA_GENERATIONINFO_STAGE":
                    return "EDSETT.GENERATIONINFO";
                case "PGEDATA.PGEDATA_SM_GENERATION_STAGE":
                    return "EDSETT.SM_GENERATION";
                case "PGEDATA.PGEDATA_SM_PROTECTION_STAGE":
                    return "EDSETT.SM_PROTECTION";
                case "PGEDATA.PGEDATA_SM_RELAY_STAGE":
                    return "EDSETT.SM_RELAY";
                case "PGEDATA.PGEDATA_SM_GENERATOR_STAGE":
                    return "EDSETT.SM_GENERATOR";
                case "PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE":
                    return "EDSETT.SM_GEN_EQUIPMENT";
                default:
                    return string.Empty;
            }
        }

        internal static string GetCorrespondingUpdateQuery(string corrStage2TabName)
        {
            switch(corrStage2TabName)
            {
                case "PGEDATA.PGEDATA_GENERATIONINFO_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_GENERATIONINFO;
                case "PGEDATA.PGEDATA_SM_GENERATION_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_SM_GENERATION;
                case "PGEDATA.PGEDATA_SM_PROTECTION_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_SM_PROTECTION;
                case "PGEDATA.PGEDATA_SM_RELAY_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_SM_RELAY;
                case "PGEDATA.PGEDATA_SM_GENERATOR_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_SM_GENERATOR;
                case "PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE":
                    return ReadConfigurations.UPDATE_FIELD_IN_STAGE2_SM_GEN_EQUIP;
                default:
                    return string.Empty;
            }
        }

        private static AoInitializeClass m_AoInit = null;

        //private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {
                //Cache product codes by enum int so can be sorted without custom sorter
                List<int> m_requestedProducts = new List<int>();
                foreach (esriLicenseProductCode code in new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced })
                {
                    int requestCodeNum = Convert.ToInt32(code);
                    if (!m_requestedProducts.Contains(requestCodeNum))
                    {
                        m_requestedProducts.Add(requestCodeNum);
                    }
                }

                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                m_AoInit = new AoInitializeClass();
                esriLicenseProductCode currentProduct = new esriLicenseProductCode();
                foreach (int prodNumber in m_requestedProducts)
                {
                    esriLicenseProductCode prod = (esriLicenseProductCode)Enum.ToObject(typeof(esriLicenseProductCode), prodNumber);
                    esriLicenseStatus status = m_AoInit.IsProductCodeAvailable(prod);
                    if (status == esriLicenseStatus.esriLicenseAvailable)
                    {
                        status = m_AoInit.Initialize(prod);
                        if (status == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                            status == esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            currentProduct = m_AoInit.InitializedProduct();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
               _log.Error("Unable to checkout ESRI license" + Ex.Message.ToString());
               throw Ex;
              
            }
        }

        /// <summary>
        /// Initializes the license necessary to read/edit ArcFM components
        /// </summary>
        /// <returns></returns>
        public static void InitializeArcFMLicense()
        {
            try
            {
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                  _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (mmLS == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                }

            }
            catch (Exception Ex)
            {
                _log.Error("Unable to checkout ARCFM license" + Ex.Message.ToString());
                throw Ex;

            }
        }

        /// <summary>
        /// Closes the license object. The user will not be able to read/edit ArcGIS
        /// or ArcFM components after a call to this method
        /// </summary>
        /// <returns></returns>
        public static void ReleaseLicense()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()            
            try
            {
                // Write if condition here 
               if(m_AoInit !=null) m_AoInit.Shutdown();
                if(_mmAppInit!=null)_mmAppInit.Shutdown();
            }
            catch (Exception exp)
            {
                // m4jf edgisrearch - 416 ed15 improvements
                _log.Error(exp.Message);
            }
        }


        public static DataTable GetDataTableWithSchema()
        {
            DataTable dtRecordsToUpdateInStageTable = null;
            try
            {
                dtRecordsToUpdateInStageTable = new DataTable();
                dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain), typeof(String));
                dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDStage), typeof(String));
                dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_SPGuidStage), typeof(String));
                dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_Action), typeof(String));
            }
            catch (Exception exp)
            {
                // throw;
                // m4jf edgisrearch - 416 ed15 improvements
                _log.Error(exp.Message);

            }
            return dtRecordsToUpdateInStageTable;
        }

        public static DataTable GetDataTableWithSchemaToUpdateServiceLocation()
        {
            DataTable dtUpdateServiceLocation = null;
            try
            {
                dtUpdateServiceLocation = new DataTable();
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_Action), typeof(String));
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_SERVICELOCATIONGUID), typeof(String));
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_GENCATEGORY), typeof(int));
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_LABELTEXT), typeof(String));
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_VERSIONNAME), typeof(String));
                //DERMS Change start 04/20/2021
                dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_SYMBOLNUMBER), typeof(int));
                //DERMS Change end 04/20/2021
            }
            catch (Exception exp)
            {
                //throw;
                // m4jf edgisrearch - 416 ed15 improvements
                _log.Error(exp.Message);
            }
            return dtUpdateServiceLocation;
        }

        public static IWorkspace OpenSdeWorkspace(string sdeConnection)
        {
            ///Opening workspace and return feature work space. The connection is from the configuration file
            try
            {
               // sdeConnection = ConfigurationManager.AppSettings["EDWorkSpaceConnString"];
               
                if (string.IsNullOrEmpty(sdeConnection))
                {
                    _log.Info("Cannot read sde file path from configuration file");
                    return null;
                }

                _log.Info("Try to connect to SDE: " + sdeConnection);
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory wsf = Activator.CreateInstance(t) as IWorkspaceFactory;
                IWorkspace featureWorkspace =wsf.OpenFromFile(sdeConnection, 0);
                _log.Info("Successfully connected to database");

                return featureWorkspace;
            }
            catch (Exception Ex)
            {
                _log.Error("Error Fail to connect to workspace: ", Ex);
                return null;
            }

        }
        public static bool CheckVersionExistFromSessionName(IWorkspace _wSpace, IMMPxApplication pxApplication)
        {

            bool SessionExist = false;

            IFeatureWorkspace fWSpace = (IFeatureWorkspace)_wSpace;
            IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
            try
            {
                int iSessionID = GetExistingSessionId(pxApplication);
                if (iSessionID != -1)
                {
                    string sessionName = ConfigurationManager.AppSettings["VersionNamePrefix"] + iSessionID.ToString();
                    IEnumVersionInfo vinfo = (IEnumVersionInfo)vWSpace.Versions;
                    IVersionInfo pversioninfo = vinfo.Next();
                    while (pversioninfo != null)
                    {
                        string vname = pversioninfo.VersionName.ToString();

                        if (vname.Contains(sessionName))
                        {
                           //Return false if SAP->EDER Process completed
                             string strQuery = "select * from  " + ReadConfigurations.SAPToEDERSTAGTableName + " where "+ReadConfigurations.ColName_STAGE_STATUS+"='COMPLETED' and  "+ ReadConfigurations.ColName_PROCESS_NAME+"='SAP_TO_EDER'";
                             DataTable DT = cls_DBHelper_For_EDER.GetDataTableByQuery(strQuery);
                            if (DT != null)
                            {
                                if (DT.Rows.Count > 0)
                                {
                                    _log.Info("Version " + vname + " already exist...Deleting Version...");
                                    SessionExist = false;
                                    //Delete the version
                                    vWSpace.FindVersion(vname).Delete();
                                    //SAPDailyInterfaceProcess.SAPDailyInterfaceVersion.Delete();
                                    _log.Info("Version " + vname + " Deleted");
                                    
                                }
                                else
                                {
                                    strQuery = "select * from  " + ReadConfigurations.SAPToEDERSTAGTableName + " where " + ReadConfigurations.ColName_STAGE_STATUS + "='IN_PROGRESS' and  " + ReadConfigurations.ColName_PROCESS_NAME + "='SAP_TO_EDER'";
                                    DataTable DT1 = cls_DBHelper_For_EDER.GetDataTableByQuery(strQuery);
                                    if (DT1 != null)
                                    {
                                        if (DT1.Rows.Count > 0)
                                        {
                                            _log.Info("Version " + vname + " already exisit...Deleting Version...");
                                            SessionExist = false;
                                            //Delete the version
                                            vWSpace.FindVersion(vname).Delete();
                                            //SAPDailyInterfaceProcess.SAPDailyInterfaceVersion.Delete();
                                            _log.Info("Version " + vname + " Deleted");
                                        }
                                        else
                                        {
                                            SessionExist = true;
                                            Common.SendMail(string.Format(ConfigurationManager.AppSettings["MAIL_BODY_VERSIONEXIST"], sessionName), ConfigurationManager.AppSettings["MAIL_SUBJ_POSTFAIL"].ToString());
                                            _log.Info("SAPToGIS Version Exist OR Version Failed on Previous Day");
                                            break;
                                        }
                                    }
                                    
                                }
                            }
                           
                        }
                        pversioninfo = vinfo.Next();
                    }
                }

            }
            catch (Exception EX)
            {

                _log.Error("Exception encountered while checking the existing version", EX);

            }
            return SessionExist;

        }

        public static int GetExistingSessionId(IMMPxApplication pxApplication)
        {
            _log.Debug(MethodBase.GetCurrentMethod().Name);
            int sessionId = -1;
            try
            {
                string sapdailyinterfaceversionname = ReadConfigurations.GetValue(ReadConfigurations.SAPDailyInterfaceVersionName);
                string sql = "SELECT max(SESSION_ID) from PROCESS.MM_SESSION where hidden=0 and " +
                            " upper(Session_Name) like upper('%" + sapdailyinterfaceversionname + "%')";
                object recordsaffected = null;
                ADODB.Recordset recordset = pxApplication.Connection.Execute(sql, out recordsaffected);

                if (!recordset.EOF)
                {
                    object sessionObj = recordset.GetRows();
                    if (sessionObj != DBNull.Value)
                    {
                        System.Array array = sessionObj as System.Array;
                        if (array.GetValue(0, 0) != DBNull.Value)
                        {
                            sessionId = Convert.ToInt32(array.GetValue(0, 0));
                            _log.Info("Session found with SESSION_ID " + sessionId);
                        }
                    }
                }
            }

            catch (Exception EX)
            {

                _log.Error("Exception encountered while getting the sessionid", EX);

            }

            return sessionId;
        }


        public static void SendMail(string strbodyText)
        {
            try
            {
                string strquery = "SELECT * FROM PGEDATA.APPLICATIONCONFIG where BATCH_NAME='ED15' ";

                DataTable DT = cls_DBHelper_For_EDER.GetDataTableByQuery(strquery);
                if (DT != null)
                {
                    if (DT.Rows.Count > 0)
                    {
                        DataRow[] Row_lanIDs = DT.Select("CONFIG_KEY='" + ConfigurationManager.AppSettings["LANIDS"] + "'");
                        foreach (DataRow rw in Row_lanIDs)
                        {
                            string[] LanIDsList = rw["CONFIG_VALUE"].ToString().Split(';');
                            string lanID = string.Empty;
                            string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                            string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                            string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT"];
                            string bodyText = strbodyText;
                            // string lanIDs = ConfigurationManager.AppSettings["LANIDS"];


                            for (int i = 0; i < LanIDsList.Length; i++)
                            {
                                lanID = LanIDsList[i];
                                EmailService.Send(mail =>
                                {
                                    mail.From = fromAddress;
                                    mail.FromDisplayName = fromDisplayName;
                                    mail.To = lanID + ";";
                                    mail.Subject = subject;
                                    mail.BodyText = bodyText;

                                });
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _log.Error("Error in Sending Mail: ", ex);
            }
        }
        /// <summary>
        /// Function added to Customize Subject in case of Failure Mails--- Post Release Fixes
        /// </summary>
        /// <param name="strbodyText"></param>
        /// <param name="mail_subject"></param>
        public static void SendMail(string strbodyText, string mail_subject)
        {
            try
            {
                string strquery = "SELECT * FROM PGEDATA.APPLICATIONCONFIG where BATCH_NAME='ED15' ";

                DataTable DT = cls_DBHelper_For_EDER.GetDataTableByQuery(strquery);
                if (DT != null)
                {
                    if (DT.Rows.Count > 0)
                    {
                        DataRow[] Row_lanIDs = DT.Select("CONFIG_KEY='" + ConfigurationManager.AppSettings["LANIDS"] + "'");
                        foreach (DataRow rw in Row_lanIDs)
                        {
                            List<string> LanIDsList = rw["CONFIG_VALUE"].ToString().Split(';').ToList();
                            string lanID = string.Empty;
                            string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                            string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                            string subject = mail_subject;
                            string bodyText = strbodyText;
                            // string lanIDs = ConfigurationManager.AppSettings["LANIDS"];
                            if (mail_subject == "SAP to GIS daily interface ( ED15 ) failed – Error in loading the input files")
                            {
                                List<string> LanIDsList_loadfail = ConfigurationManager.AppSettings["LANIDS_LOADFAIL"].ToString().Split(';').ToList();

                                LanIDsList.AddRange(LanIDsList_loadfail);
                            }
                            else if (mail_subject == "SAP to GIS daily interface ( ED15 ) failed – Automated version posting failed")
                            {
                                List<string> LanIDsList_postfail = ConfigurationManager.AppSettings["LANIDS_POSTFAIL"].ToString().Split(';').ToList();

                                LanIDsList.AddRange(LanIDsList_postfail);
                            }
                            else if (mail_subject == "ED 15 POST JOB FAILED")
                            {
                                List<string> LanIDsList_postfail = ConfigurationManager.AppSettings["LANIDS_POSTFAIL"].ToString().Split(';').ToList();

                                LanIDsList.AddRange(LanIDsList_postfail);
                            }
                            
                            for (int i = 0; i < LanIDsList.Count; i++)
                            {
                                lanID = LanIDsList[i];
                                EmailService.Send(mail =>
                                {
                                    mail.From = fromAddress;
                                    mail.FromDisplayName = fromDisplayName;
                                    mail.To = lanID + ";";
                                    mail.Subject = subject;
                                    mail.BodyText = bodyText;

                                });
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _log.Error("Error in Sending Mail: ", ex);
            }
        }
       
    }

    

    public static class MyExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object myObj)
        {
            return myObj.GetType()
                .GetProperties()
                .Select(pi => new { Name = pi.Name, Value = pi.GetValue(myObj, null) })
                .Union(
                    myObj.GetType()
                    .GetFields()
                    .Select(fi => new { Name = fi.Name, Value = fi.GetValue(myObj) })
                 )
                .ToDictionary(ks => ks.Name, vs => vs.Value);
        }
    }



}
