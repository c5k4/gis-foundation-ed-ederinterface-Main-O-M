using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Linq;
using System.Reflection;
using System.Xml;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    public class UtilityFunctions
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// GetLogfilepath
        /// </summary>
        /// <returns></returns>
        public string GetLogfilepath()
        {
            try
            {
                string appLocalUserPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string logPath = System.IO.Path.Combine(appLocalUserPath, ReadConfigurationValue("PONS_LOG_FILE_RELATIVE_PATH"));//Properties.Resources.PONS_LOG_FILE_RELATIVE_PATH);
                return logPath;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return "";
        }

        /// <summary>
        /// get log4net settings
        /// </summary>
        /// <returns></returns>
        public string Getlog4netConfig()
        {
            //rolling file size 512 KB in bytes, bckup will be till 10 such files  524288

            string Config = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
                                    <configuration> 
                                      <log4net debug=""False""> 
                                        <!-- Define some output appenders --> 
                                        <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender""> 
                                          <file type=""log4net.Util.PatternString"" value=""%property{LogName}"" /> 
                                          <appendToFile value=""true"" /> 
                                          <rollingStyle value=""Composite"" />                               
                                         <datePattern value=""ddMMyyyy"" />
                                          <maxSizeRollBackups value=""30"" /> 
                                          <maximumFileSize value=""1MB"" />                                                                                    
                                        <layout type=""log4net.Layout.PatternLayout"">
                                           <conversionPattern value=""%date [%thread] %level %logger - %message%newline""/>
                                        </layout>
                                        </appender> 
                 
                                   <!-- Setup the root category, add the appenders and set the default level --> 
                                   <root> 
                                   <level value=""ALL""/> 
                                   <appender-ref ref=""RollingLogFileAppender"" /> 
                                   </root> 
                                   </log4net> 
                                    </configuration>";

            return Config;
        }



        public IWorkspace OpenSDEWorkspacefromsdefile(string sdefilenamewithpath)
        {
            IWorkspace pReturnWorkspace = null;
            try
            {
                if (!string.IsNullOrEmpty(sdefilenamewithpath))
                {
                    // Create an SDE workspace factory and open the workspace.
                    Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                    pReturnWorkspace = workspaceFactory.OpenFromFile(sdefilenamewithpath, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return pReturnWorkspace;

        }

        public string GetApplicationConnectionFilePath()
        {
            try
            {
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string sdefilepath = (System.IO.Path.Combine(executingAssemblyPath, ReadConfigurationValue("EDGIS_SDE_CONNECTION_FILE")));//Properties.Resources.EDGIS_SDE_CONNECTION_FILE));

                if (System.IO.File.Exists(sdefilepath))
                {
                    return sdefilepath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return string.Empty;
        }

        public IExtension GetArcMapExtensionByCLSID(string CLSID)
        {
            try
            {
                UID uid = new UIDClass();
                uid.Value = CLSID;
                return PGEGlobal.Application.FindExtensionByCLSID(uid);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return null;
        }

        public string GetConnectedServiceLocationsFromWEBRTrace(string FeatureGlobalID, string FeederID, string FeatureClassID)
        {
            string sReturn = string.Empty;
            try
            {
                //string TableName = ReadConfigurationValue("PGE_FEEDERFEDNETWORK_TRACE");
                string ServiceLocationFCID = ReadConfigurationValue("FCID_SERVICELOCATION");

                //string sQuery = "SELECT TO_FEATURE_GLOBALID FROM " + TableName + " A, " +
                //    "(SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, TO_FEATURE_FCID, TO_FEATURE_EID, TO_FEATURE_OID, FEEDERID, FEEDERFEDBY " +
                //    "FROM " + TableName + " WHERE TO_FEATURE_GLOBALID = '" + FeatureGlobalID + "' AND FEEDERID = '" + FeederID + "' AND TO_FEATURE_FCID = '" + FeatureClassID +
                //    "') B WHERE A.TO_FEATURE_FCID= '" + ServiceLocationFCID + "' AND A.MIN_BRANCH >= B.MIN_BRANCH AND A.MAX_BRANCH <= B.MAX_BRANCH " +
                //    "AND A.TREELEVEL >= B.TREELEVEL AND A.ORDER_NUM <= B.ORDER_NUM AND A.FEEDERID = B.FEEDERID";

                //DataTable dtResults = ExecuteQuery(sQuery, ReadConfigurationValue("WEBR_ConnectionString"));

                DataTable dtParams = new DataTable();
                dtParams.Columns.Add("Param_Name", typeof(string));
                dtParams.Columns.Add("Param_Type", typeof(string));
                dtParams.Columns.Add("Param_Value", typeof(string));
                dtParams.Rows.Add(new object[] { "TO_FEATURE_GLOBALID", "OracleType.VarChar", FeatureGlobalID });
                dtParams.Rows.Add(new object[] { "FEEDERID", "OracleType.VarChar", FeederID });
                dtParams.Rows.Add(new object[] { "TO_FEATURE_FCID_DEVICE", "OracleType.VarChar", FeatureClassID });
                dtParams.Rows.Add(new object[] { "TO_FEATURE_FCID_SERVICELOC", "OracleType.VarChar", ServiceLocationFCID });

                DataTable dtResults = ExecuteSP(ReadConfigurationValue("WEBR_ConnectionString"), ReadConfigurationValue("SP_SLGlobalIds_NetworkTrace"), dtParams, string.Empty);

                foreach (DataRow dataRow in dtResults.Rows)
                    sReturn += Convert.ToString(dataRow[0]) + ",";

                if (sReturn.Contains(","))
                    sReturn = sReturn.TrimEnd(',');
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return sReturn;
        }

        /// <summary>
        /// This method calls a DB Stored procedure with the parameters passed in datatable as argument
        /// </summary>
        /// <param name="connectionString">connectionString</param>
        /// <param name="sSPName">Name of the Stored Procedure</param>
        /// <param name="Params">Datatable containing Parameter names, datatype and input value</param>
        /// <returns></returns>
        public Boolean ExecuteSP(string connectionString, string sSPName, DataTable Params)
        {
            try
            {

                //ConfigurationHelper.ConfigurationHelper configUtil = new ConfigurationHelper.ConfigurationHelper(Logger);

                OracleConnection OraConn = OpenConnection(connectionString);
                OracleTransaction OraTransaction = default(OracleTransaction);

                if (OraConn == null)
                {
                    //Logger.Error("Connection is inaccessible");
                    return false;
                }
                try
                {

                    OraTransaction = OraConn.BeginTransaction();
                }
                catch (Exception)
                {
                    //Logger.Error("Transaction could not be started.");
                    return false;
                }
                OracleCommand OraCmd = default(OracleCommand);
                try
                {
                    OraCmd = new OracleCommand(sSPName, OraConn);
                    OraCmd.CommandType = CommandType.StoredProcedure;
                    OraCmd.Transaction = OraTransaction;

                    OracleParameter param = default(OracleParameter);

                    for (int i = 0; i <= Params.Rows.Count - 1; i++)
                    {
                        try
                        {
                            //ASSIGN PARAMETERS TO BE PASSED
                            param = default(OracleParameter);
                            param.Direction = ParameterDirection.Input;
                            switch (Convert.ToString(Params.Rows[i][1]))
                            {
                                case "String":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.NVarchar2);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToString(Params.Rows[i][2]);
                                        }
                                    }
                                    break;
                                case "DateTime":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.Date);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToDateTime(Params.Rows[i][2]);
                                        }
                                        break;
                                    }
                                case "Integer":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.Int32);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToInt32(Params.Rows[i][2]);
                                        }
                                        break;
                                    }
                            }

                            int iReturnValue = OraCmd.ExecuteNonQuery();
                            if (iReturnValue != 1)
                            {
                                OraTransaction.Rollback();
                                //Logger.Error("Transaction rolled back.");
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Logger.Error(ex.Message);
                            OraTransaction.Rollback();
                            return false;
                        }

                    }
                    OraTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    //Logger.Error(ex.Message);
                    OraTransaction.Rollback();
                    return false;
                }
                finally
                {
                    if (OraConn.State != ConnectionState.Closed)
                        OraConn.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return false;
        }

        public DataTable ExecuteSP(string connectionString, string sSPName, DataTable Params, string VersionName = "")
        {
            OracleConnection OraConn = default(OracleConnection);
            OracleDataAdapter OraDataAdapter = default(OracleDataAdapter);
            DataTable dtReturn = default(DataTable);

            try
            {
                OraConn = OpenConnection(connectionString);

                if (OraConn == null)
                {
                    //Logger.Error("Connection is inaccessible");
                    return dtReturn;
                }
                OracleCommand OraCmd = default(OracleCommand);
                try
                {
                    OraCmd = new OracleCommand(sSPName, OraConn);
                    OraCmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter param = default(OracleParameter);

                    //if (!string.IsNullOrEmpty(VersionName))
                    //{
                    //    param = new OracleParameter();
                    //    param.Direction = ParameterDirection.Input;
                    //    param = OraCmd.Parameters.Add("P_VERSION", OracleDbType.NVarchar2);
                    //    param.Value = VersionName;
                    //}
                    dtReturn = new DataTable();
                    //Add parameters from datatable
                    for (int i = 0; i < Params.Rows.Count; i++)
                    {
                        try
                        {
                            //ASSIGN PARAMETERS TO BE PASSED
                            param = default(OracleParameter);
                            param = new OracleParameter();
                            param.Direction = ParameterDirection.Input;
                            switch (Convert.ToString(Params.Rows[i][1]))
                            {
                                case "OracleType.NVarChar":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.NVarchar2);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToString(Params.Rows[i][2]);
                                        }
                                    }
                                    break;
                                case "OracleType.VarChar":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.NVarchar2);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToString(Params.Rows[i][2]);
                                        }
                                    }
                                    break;
                                case "OracleType.DateTime":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.Date);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToDateTime(Params.Rows[i][2]);
                                        }
                                        break;
                                    }
                                case "OracleType.Number":
                                    {
                                        param = OraCmd.Parameters.Add(Convert.ToString(Params.Rows[i][0]), OracleDbType.Decimal);
                                        if (Params.Rows[i][2] == null)
                                        {
                                            param.Value = DBNull.Value;
                                        }
                                        else
                                        {
                                            param.Value = Convert.ToInt64(Params.Rows[i][2]);
                                        }
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    if (!string.IsNullOrEmpty(VersionName))
                    {
                        param = new OracleParameter();
                        param.Direction = ParameterDirection.Input;
                        param = OraCmd.Parameters.Add("P_VERSION", OracleDbType.NVarchar2);
                        param.Value = VersionName;
                    }
                    param = default(OracleParameter);
                    param = OraCmd.Parameters.Add("p_cursor", OracleDbType.RefCursor);
                    param.Direction = ParameterDirection.Output;

                    param = default(OracleParameter);
                    param = OraCmd.Parameters.Add("p_error", OracleDbType.Varchar2, 100);
                    param.Direction = ParameterDirection.Output;

                    param = default(OracleParameter);
                    param = OraCmd.Parameters.Add("p_success", OracleDbType.Decimal, 100);
                    param.Direction = ParameterDirection.Output;

                    OraDataAdapter = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    ds.Tables.Add("Test");
                    OraDataAdapter.SelectCommand = OraCmd;

                    OraDataAdapter.Fill(ds, "Test");
                    dtReturn = ds.Tables["Test"];

                }
                catch (Exception ex)
                {
                    //Logger.Error(ex.Message);
                    throw ex;
                }
                finally
                {
                    if (OraConn.State != ConnectionState.Closed)
                        OraConn.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return dtReturn;
        }

        public DataTable ExecuteQuery(string SQLQuery, string ConnectionString)
        {
            DataTable dtReturn = default(DataTable);
            OracleConnection OraConn = null;
            try
            {
                OraConn = OpenConnection(ConnectionString);
                if (OraConn == null)
                    return null;
                OracleCommand OraCmd = OraConn.CreateCommand();
                OraCmd.CommandText = SQLQuery;
                OracleDataReader OraReader = OraCmd.ExecuteReader();
                dtReturn = new DataTable();

                for (int iFieldCount = 0; iFieldCount < OraReader.FieldCount; iFieldCount++)
                {
                    dtReturn.Columns.Add(OraReader.GetName(iFieldCount), OraReader.GetFieldType(iFieldCount));
                }

                if (dtReturn.Columns.Count <= 0)
                    return null;

                if (!OraReader.HasRows)
                    return dtReturn;

                object[] values = new object[OraReader.FieldCount];

                while (OraReader.Read())
                {
                    OraReader.GetValues(values);
                    dtReturn.Rows.Add(values);
                }
                //return dtReturn;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (OraConn != null)
                {
                    if (OraConn.State == ConnectionState.Open)
                    {
                        OraConn.Close();
                        //  Logger.Instance.Info("Oracle connection closed.");
                    }
                    OraConn = null;
                }
            }
            return dtReturn;
        }

        public string ReadFieldDomaincodeforValue(IFeatureClass fc, string fieldname, string domainvalue)
        {
            string domcode = "";
            try
            {

                if (fc.Fields.FindField(fieldname) == -1)
                    return domcode;


                IField2 field = fc.Fields.Field[fc.Fields.FindField(fieldname)] as IField2;
                IDomain domain = field.Domain;


                if (domain != null)
                {
                    ICodedValueDomain2 cvDomain = domain as ICodedValueDomain2;
                    if (cvDomain != null)
                    {
                        for (int i = 0; i < cvDomain.CodeCount; i++)
                        {
                            if (cvDomain.get_Name(i).ToUpper().Equals(domainvalue.ToUpper()))
                            {
                                domcode = cvDomain.get_Value(i).ToString();
                                break;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return domcode;
        }

        public OracleConnection OpenConnection(string ConnectionString = "")
        {
            OracleConnection OraConn = default(OracleConnection);

            if (string.IsNullOrEmpty(ConnectionString))
            {
                // Code to get the default connectionstring from the configuration file and open the oracle connection
                // Raise some valid exception
            }

            try
            {
                OraConn = new OracleConnection(ConnectionString);
                OraConn.Open();

                if (OraConn.State != ConnectionState.Open)
                    throw new Exception("Unable to connect to Oracle Database");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            { }
            return OraConn;
        }

        public IWorkspace GetWorkspaceFromSDEFile(string SDEFilePath, string SDEFileName)
        {
            IWorkspace pReturnWorkspace = default(IWorkspace);
            IWorkspaceFactory pSDEWSFactory = default(IWorkspaceFactory);
            try
            {
                if (!(string.IsNullOrEmpty(SDEFilePath) || string.IsNullOrEmpty(SDEFileName)))
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(SDEFilePath, SDEFileName)))
                    {
                        pSDEWSFactory = new SdeWorkspaceFactoryClass();
                        pReturnWorkspace = pSDEWSFactory.OpenFromFile(System.IO.Path.Combine(SDEFilePath, SDEFileName), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            { }

            return pReturnWorkspace;
        }

        /// <summary>
        /// Returns Connection String
        /// </summary>
        /// <param name="ConfigurationKey"></param>
        /// <returns></returns>
        public string ReadConnConfigValue(string ConfigurationKey)
        {
            string ConfigurationValue = string.Empty;
            string connString = string.Empty;
            try
            {
                ConfigurationValue = ReadConfigurationValue(ConfigurationKey);
                if(!string.IsNullOrWhiteSpace(ConfigurationValue))
                {
                    connString = PGE_DBPasswordManagement.ReadEncryption.GetConnectionStr(ConfigurationValue);
                }
                else
                {
                    throw new Exception("Invallid Connection String Found for "+ ConfigurationKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw ex;
            }
            return connString;
        }

        /// <summary>
        /// Returns SDE Path
        /// </summary>
        /// <param name="ConfigurationKey"></param>
        /// <returns></returns>
        public string ReadSDEConfigValue(string ConfigurationKey)
        {
            string ConfigurationValue = string.Empty;
            string connString = string.Empty;
            try
            {
                ConfigurationValue = ReadConfigurationValue(ConfigurationKey);
                if (!string.IsNullOrWhiteSpace(ConfigurationValue))
                {
                    connString = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationValue);
                }
                else
                {
                    throw new Exception("Invallid SDE connection Found for "+ ConfigurationKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw ex;
            }
            return connString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConfigurationKey"></param>
        /// <returns></returns>
        public string ReadConfigurationValue(string ConfigurationKey)
        {
            string ConfigurationValue = string.Empty;
            try
            {
                // SB : Updated following code to avoid cyclic references
                string ConfigFilePath = PGEGlobal.ConfigFilePath; //ReadConfigurationValue("DESKTOP_CONFIG_FILE_WITHPATH");//;Properties.Resources.DESKTOP_CONFIG_FILE_WITHPATH;
                if (!System.IO.File.Exists(ConfigFilePath))
                {
                    //Logger.Error("Application Configuration File not present.");
                    return "NO_FILE";
                }


                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigFilePath);
                    XmlNodeList configfilepathnodelist = doc.DocumentElement.SelectNodes("/config");
                    foreach (XmlNode node in configfilepathnodelist)
                    {
                        ConfigurationValue = node.SelectSingleNode(ConfigurationKey).InnerText;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConfigurationValue;
        }

        internal IList<Customer> GetCustomerListWEBRTrace(IFeature startFeature, IFeature endFeature, string FeederID, string startFCID, string endFCID, bool Exclude)
        {
            IList<Customer> CustomerList = new List<Customer>();

            try
            {
                string SPName = string.Empty;
                string startFeatureGUID = Convert.ToString(startFeature.get_Value(startFeature.Fields.FindField("GLOBALID")));
                string sStartTracedServiceLocationGUIDs = GetConnectedServiceLocationsFromWEBRTrace(startFeatureGUID, FeederID, startFCID);
                IList<string> StartSLGUIDs = new List<string>();
                if (!string.IsNullOrEmpty(sStartTracedServiceLocationGUIDs))
                    StartSLGUIDs = (sStartTracedServiceLocationGUIDs.Split(',')).ToList();

                if (endFeature != null)
                {
                    string endFeatureGUID = Convert.ToString(endFeature.get_Value(endFeature.Fields.FindField("GLOBALID")));
                    string sEndTracedServiceLocationGUIDs = GetConnectedServiceLocationsFromWEBRTrace(endFeatureGUID, FeederID, endFCID);
                    IList<string> EndSLGUIDs = new List<string>();
                    if (!string.IsNullOrEmpty(sEndTracedServiceLocationGUIDs))
                        EndSLGUIDs = (sEndTracedServiceLocationGUIDs.Split(',')).ToList();

                    if (EndSLGUIDs.Count > StartSLGUIDs.Count)
                    {
                        IList<string> SwapList = EndSLGUIDs;
                        EndSLGUIDs = default(IList<string>);
                        EndSLGUIDs = StartSLGUIDs;
                        StartSLGUIDs = default(IList<string>);
                        StartSLGUIDs = SwapList;
                        SwapList = default(IList<string>);
                    }

                    //((List<string>)StartSLGUIDs).RemoveAll(x => EndSLGUIDs.Contains(x));
                    // item1 = item1.Except(item2).ToList();
                    StartSLGUIDs = ((List<string>)StartSLGUIDs).Except(EndSLGUIDs).ToList();

                }

                if (StartSLGUIDs.Count <= 0)
                    return CustomerList;
                //throw new Exception("There are no service locations connected to the start device");

                string ServiceLocationGUIDs = string.Empty;
                foreach (string SLGUID in StartSLGUIDs)
                    ServiceLocationGUIDs += "'" + SLGUID + "',";

                MapUtility objMapUtility = new MapUtility();
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();

                //IFeatureClass pSLFeatureClass = objMapUtility.GetFeatureLayerfromFCName(objUtilityFunctions.ReadConfigurationValue("FC_ServiceLocation")).FeatureClass as IFeatureClass;
                PGEFeatureClass objPGEFeatClass = objMapUtility.GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("FC_ServiceLocation"));
                IFeatureClass pSLFeatureClass = default(IFeatureClass);

                if (objPGEFeatClass.FeatureClass != null)
                {
                    pSLFeatureClass = objPGEFeatClass.FeatureClass;
                }

                if (pSLFeatureClass == null)
                    return CustomerList;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "GLOBALID IN (" + ServiceLocationGUIDs.TrimEnd(',') + ")";
                IFeatureCursor pFeatureCursor = pSLFeatureClass.Search(pQueryFilter, false);
                IFeature pServiceLocation = default(IFeature);
                string RelationshipClassName = ReadConfigurationValue("RC_ServiceLocation_ServicPoint");
                string strServicePointIDs = string.Empty;

                IRelationshipClass m_relationshipClass = null;
                IFeatureWorkspace m_featureWorkspace = null;

                if (pSLFeatureClass != null)
                {
                    m_featureWorkspace = (IFeatureWorkspace)pSLFeatureClass.FeatureDataset.Workspace;
                    if (m_featureWorkspace == null)
                        throw new Exception("Could not load selected feature workspace");

                    m_relationshipClass = m_featureWorkspace.OpenRelationshipClass(RelationshipClassName);
                    if (m_relationshipClass == null)
                        throw new Exception(string.Format("Could not load {0} relationship class.", RelationshipClassName));
                }

                ISet pRelatedObjsSet = null;
                IRow m_RelatedRow = null;

                pServiceLocation = pFeatureCursor.NextFeature();
                while (pServiceLocation != null)
                {
                    pRelatedObjsSet = m_relationshipClass.GetObjectsRelatedToObject(pServiceLocation);
                    m_RelatedRow = default(IRow);
                    if (pRelatedObjsSet.Count > 0)
                    {
                        m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                        while (m_RelatedRow != null)
                        {
                            strServicePointIDs += Convert.ToString(m_RelatedRow.get_Value(m_RelatedRow.Fields.FindField("SERVICEPOINTID"))) + ",";
                            m_RelatedRow = default(IRow);
                            m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                        }
                    }
                    pServiceLocation = null;
                    pServiceLocation = pFeatureCursor.NextFeature();
                }

                if (!string.IsNullOrEmpty(strServicePointIDs))
                {
                    {
                        if (strServicePointIDs.Contains(","))
                            strServicePointIDs = strServicePointIDs.TrimEnd(',');

                        DataTable dtParams = new DataTable();
                        dtParams.Columns.Add("Param_Name", typeof(string));
                        dtParams.Columns.Add("Param_Type", typeof(string));
                        dtParams.Columns.Add("Param_Value", typeof(string));
                        dtParams.Rows.Add(new object[] { "P_INPUTSTRING", "OracleType.VarChar", strServicePointIDs });

                        if (Exclude)
                        {
                            //dtParams.Rows.Add(new object[] { "P_FLAG", "OracleType.NVarChar", "1" });
                            SPName = "SP_CustInfo_ByServicePointIDs_EXCL";
                        }
                        else
                        {
                            //dtParams.Rows.Add(new object[] { "P_FLAG", "OracleType.NVarChar", "0" });
                            SPName = "SP_CustInfo_ByServicePointIDs_INCL";
                        }

                        // M4JF EDGISREARCH - PWD MGMNT CHANGE

                       // DataTable dtCustomerList = ExecuteSP(objUtilityFunctions.ReadConfigurationValue("EDER_ConnectionString"),
                          //  ReadConfigurationValue(SPName), dtParams, "SDE.DEFAULT");

                        DataTable dtCustomerList = ExecuteSP(objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"),
                            ReadConfigurationValue(SPName), dtParams, "SDE.DEFAULT");


                        Customer pCustomer = new Customer();

                        foreach (DataRow drCustomer in dtCustomerList.Rows)
                        {
                            pCustomer = default(Customer);
                            pCustomer = new Customer();
                            pCustomer.ServicePointID = Convert.ToString(drCustomer["ServicePointID"]);
                            pCustomer.MeterNumber = Convert.ToString(drCustomer["MeterNumber"]);
                            pCustomer.CustomerType = Convert.ToString(drCustomer["CustomerType"]);
                            pCustomer.CGC12 = Convert.ToString(drCustomer["CGC12"]);
                            //pCustomer.TransformerOperatingNumber = Convert.ToString(drCustomer["OperatingNumber"]);
                            pCustomer.SSD = Convert.ToString(drCustomer["SourceSideDeviceId"]);

                            try
                            {
                                pCustomer.CustomerName.MailName1 = Convert.ToString(drCustomer["MAILNAME1"]);
                                pCustomer.CustomerName.MailName2 = Convert.ToString(drCustomer["MAILNAME2"]);
                                pCustomer.PhoneNumber.PhoneNumber = Convert.ToString(drCustomer["PHONENUM"]);
                                pCustomer.CustomerAccountNumber = Convert.ToString(drCustomer["AccountID"]);
                            }
                            catch (Exception)
                            { }

                            try
                            {
                                pCustomer.Division = Convert.ToString(drCustomer["Division"]);
                            }
                            catch (Exception)
                            {
                                pCustomer.Division = PGEGlobal.WIZARD_DIVISION;
                            }

                            Address pCustomerAddress = new Address();

                            if (dtCustomerList.Columns.Contains("City"))
                            {
                                pCustomerAddress.City = Convert.ToString(drCustomer["City"]);
                                pCustomerAddress.State = Convert.ToString(drCustomer["State"]);
                                pCustomerAddress.StreetName1 = Convert.ToString(drCustomer["StreetName1"]);
                                pCustomerAddress.StreetName2 = Convert.ToString(drCustomer["StreetName2"]);
                                pCustomerAddress.StreetNumber = Convert.ToString(drCustomer["StreetNumber"]);
                                pCustomerAddress.ZIPCode = Convert.ToString(drCustomer["Zip"]);
                                pCustomer.CustomerAddress = pCustomerAddress;
                            }
                            else
                            {
                                pCustomerAddress.City = "";
                                pCustomerAddress.State = "";
                                pCustomerAddress.StreetName1 = "";
                                pCustomerAddress.StreetName2 = "";
                                pCustomerAddress.StreetNumber = "";
                                pCustomerAddress.ZIPCode = "";
                                pCustomer.CustomerAddress = new Address();
                            }

                            Address pMailAddress = new Address();
                            if (dtCustomerList.Columns.Contains("MailStreetName1"))
                            {
                                pMailAddress.City = Convert.ToString(drCustomer["MailCity"]);
                                pMailAddress.State = Convert.ToString(drCustomer["MailState"]);
                                pMailAddress.StreetName1 = Convert.ToString(drCustomer["MailStreetName1"]);
                                pMailAddress.StreetName2 = Convert.ToString(drCustomer["MailStreetName2"]);
                                pMailAddress.StreetNumber = Convert.ToString(drCustomer["MailStreetNum"]);
                                pMailAddress.ZIPCode = Convert.ToString(drCustomer["MailZipCode"]);
                            }
                            else
                            {
                                pMailAddress.City = "";
                                pMailAddress.State = "";
                                pMailAddress.StreetName1 = "";
                                pMailAddress.StreetName2 = "";
                                pMailAddress.StreetNumber = "";
                                pMailAddress.ZIPCode = "";
                            }
                            pCustomer.MailAddress = pMailAddress;

                            //CustomerList = new List<Customer>();
                            CustomerList.Add(pCustomer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return CustomerList;
        }
    }
}
