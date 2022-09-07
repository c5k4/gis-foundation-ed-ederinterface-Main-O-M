using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework;
using PGE_DBPasswordManagement;

using Oracle.DataAccess.Client;
using PGE.Common.Delivery.Systems.Configuration;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// This class facilitates interaction with the Database
    /// </summary>
    public class DataHelper : IDisposable
    {
        public static bool DisableSapRWNotificationWrites = false;
        private static string functionalLocation = string.Empty;
        private static string supportStructure = string.Empty;
        private static string device = string.Empty;
        private static string notApplicable = string.Empty;
        private static string separator = string.Empty;
        private static string interfaceName = string.Empty;
        private static  PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger; //= new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.log4net.config");

        private const string _configFileName = "PGE.Interfaces.SAP.dll.config";
        private Configuration _config;
        private string _connectionstring;
        private string _EDGISconnectionstring;
        private string _gisSAPAssetSyncTableName;
        private string _assetSyncTempTableName;
        private string ED07User;
        private string lastUserField;
        private string ed07TableName = string.Empty;
        private OracleConnection _oracleConnection;
        private OracleTransaction _oraTran;
        private bool _disposed = false;
        private int _recordid = 1;
        private int _batchID = 1;
       
        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public DataHelper(bool useconn = true)
        {
            try
            {

                Console.WriteLine("DBHelper Constructor registry start");
              //  SystemRegistry sysRegistry = new SystemRegistry("PGESAP");
                //string PGEFolder = sysRegistry.Directory;
                //string installationFolder = sysRegistry.GetSetting<string>("SAPDirectory", PGEFolder);
                Console.WriteLine("SystemRegistry end ");
                Console.WriteLine("DBhelper starting");
               // string installationFolder = sysRegistry.Directory;
                Console.WriteLine("test");
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;

                Console.WriteLine(assemblyLocation);
                //installationFolder = Path.GetDirectoryName(assemblyLocation);
                //string configPath = Path.Combine(installationFolder, "Config");
                //string configPath = Path.Combine(installationFolder, "SAP Asset Synch");  //line backup

                // ME Q3 change : Config file to be read from installation directory not from system registry
                //string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SAP Asset Synch";
                // string configPath = System.IO.Path.GetDirectoryName();
                
                string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Console.WriteLine(configPath);

                _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.ED06.Batch.log4net.config");
                _logger.Debug("Configuration Path found:" + configPath);
                string configFile = Path.Combine(configPath, _configFileName);
                Console.WriteLine(configFile);
                _logger.Debug("Opening Configuration file:" + configFile);

                Console.WriteLine(configFile);
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap(); //Path to your config file
                fileMap.ExeConfigFilename = configFile;
                _config = null;
                _config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                // m4jf edgisrearch 919


                //_logger.Debug("Connectionstring is null:" + (_config.AppSettings.Settings["connectionString"] == null).ToString());
                //_connectionstring = _config.AppSettings.Settings["connectionString"].Value;
                string connection = _config.AppSettings.Settings["EDER_ConnectionStr"].Value;
                // m4jf edgisrearch 919
                _logger.Debug("Connectionstring is null:" + (connection == null).ToString());
                _connectionstring = ReadEncryption.GetConnectionStr(connection.ToUpper()) ;
                _EDGISconnectionstring = ReadEncryption.GetConnectionStr(_config.AppSettings.Settings["EDGIS_ConnectionStr"].Value.ToUpper());
                //  Console.WriteLine(_connectionstring);
                //_logger.Debug("connection:" + _connectionstring);
                _logger.Debug("connection:" + connection);


                _logger.Debug("gisSapAssetSynchTableName:" + _config.AppSettings.Settings["gisSapAssetSynchTableName"].Value);
                _gisSAPAssetSyncTableName = _config.AppSettings.Settings["gisSapAssetSynchTableName"].Value;
                _assetSyncTempTableName = _config.AppSettings.Settings["gisSapAssetSynchTempTableName"].Value;
                _logger.Debug("scriptsLocation:" + _config.AppSettings.Settings["scriptsLocation"].Value);
                ScriptsLocation = _config.AppSettings.Settings["scriptsLocation"].Value;

                //Below code added for EDGIS Rearch Project to validate last user is not ed07 user-v1t8
                ED07User = _config.AppSettings.Settings["ED07_User"].Value;
                _logger.Debug("ED07_User:" + _config.AppSettings.Settings["ED07_User"].Value);

                lastUserField = _config.AppSettings.Settings["LASTUSERFEILD"].Value;
                _logger.Debug("LASTUSERFEILD:" + _config.AppSettings.Settings["LASTUSERFEILD"].Value);

                ed07TableName = _config.AppSettings.Settings["ED07TableName"].Value;
                _logger.Debug("ED07 Table Name:" + _config.AppSettings.Settings["ED07TableName"].Value);

                functionalLocation = _config.AppSettings.Settings["FunctionalLocation"].Value;
                _logger.Debug("functionalLocation:" + _config.AppSettings.Settings["FunctionalLocation"].Value);

                supportStructure = _config.AppSettings.Settings["SupportStructure"].Value;
                _logger.Debug("SupportStructure:" + _config.AppSettings.Settings["SupportStructure"].Value);

                device = _config.AppSettings.Settings["Device"].Value;
                _logger.Debug("Device:" + _config.AppSettings.Settings["Device"].Value);

                notApplicable = _config.AppSettings.Settings["NotApplicable"].Value;
                _logger.Debug("NotApplicable:" + _config.AppSettings.Settings["NotApplicable"].Value);

                separator = _config.AppSettings.Settings["Separator"].Value;
                _logger.Debug("Separator for recordID:" + _config.AppSettings.Settings["Separator"].Value);

                interfaceName = _config.AppSettings.Settings["InterfaceName"].Value;
                _logger.Debug("InterfaceName:" + _config.AppSettings.Settings["InterfaceName"].Value);

                //_connectionstring = "Data Source=EDGIST1D;User ID=edgis;Password=edgis!T1Di";
                //_gisSAPAssetSyncTableName = "EDGIS.PGE_GISSAP_AssetSynch";
                //_assetSyncTempTableName = "EDGIS.PGE_GISSAP_AssetSynch_TEST";
                //ScriptsLocation = "";

                if (useconn)
                {
                    _connectionstring = _connectionstring + " Connection Timeout=600; Max Pool Size=150;";
                    _oracleConnection = new OracleConnection(_connectionstring);
                    _oracleConnection.Open();
                }

            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {


                Console.WriteLine(ex.Message +"dbhelper");
                _logger.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;

            }

        }

        public void ResetSequence()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_EDGISconnectionstring))
                {
                    con.Open();
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Alter sequence EDGIS.ED06_RECORDID_SEQ restart start with 1";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Reset Sequence Sucessful");
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Console.WriteLine("Reset Sequence failed with Error: "+ex.Message);
                _logger.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }
        }

        public void CloseConnection()
        {
            if (this._oracleConnection != null)
            {
                if (this._oracleConnection.State == ConnectionState.Open)
                {
                    this._oracleConnection.Close();
                    this._oracleConnection.Dispose();
                }
                this._oracleConnection = null;
            }
        }

        ~DataHelper()
        {
            try
            {
                if (this._oracleConnection != null)
                {
                    if (this._oracleConnection.State == ConnectionState.Open)
                    {
                        this._oracleConnection.Close();
                        this._oracleConnection.Dispose();
                    }
                    this._oracleConnection = null;
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                GISSAPIntegrator._logger.Error(ex.GetType() + " || " + ex.Message + " || \n" + ex.StackTrace);
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
        /// The name of the database table where the data will be stored.
        /// </summary>
        public string GISSAPAssetSyncTableName
        {
            get
            {
                return _gisSAPAssetSyncTableName;
            }
        }

        /// <summary>
        /// The location where SQL scripts from failure in processing asset rows are stored.
        /// </summary>
        public string ScriptsLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this._oraTran = this._oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// This method executes a SQL statement against the DB and returns a list of results
        /// </summary>
        /// <param name="sql">The SQL string to execute</param>
        /// <returns>List of rows from the database</returns>
        private IList<GISSAP_ASSETSYNCH> GetData(string sql)
        {
            _logger.Debug(sql);
            IList<GISSAP_ASSETSYNCH> returnRows = new List<GISSAP_ASSETSYNCH>();

            int i = 0;
            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;
                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (i++ % 1000 == 0) _logger.Debug("Read [ " + i + " ] rows");
                        try
                        {
                            var data = new GISSAP_ASSETSYNCH();
                            data.ASSETID = dataReader.GetString(0);
                            data.ACTIONTYPE = dataReader.GetString(1);
                            data.TYPE = dataReader.GetInt16(2);
                            data.DATEPROCESSED = dataReader.GetDateTime(3);
                            data.SAPATTRIBUTES = dataReader.GetString(4);
                            returnRows.Add(data);
                        }
                        catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                        catch (Exception e)
                        {
                            _logger.Error("Error getting data from database.", e);
                        }
                    }
                }
                return returnRows;
            }
        }

        /// <param name="actionType"></param>
        /// <summary>
        /// This method constructs the actual sql that is executed
        /// </summary>
        /// <param name="sapType"></param>
        /// <returns></returns>
        public IList<GISSAP_ASSETSYNCH> GetRecords(short sapType, char actionType)
        {
            //dbms_lob.substr(sapattributes,32000,1)
            string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, dbms_lob.substr(sapattributes,32000,1) SAPATTRIBUTES FROM " + _gisSAPAssetSyncTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL) AND ACTIONTYPE = '" + actionType + "' AND TYPE = " + sapType;
            //string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES FROM " + _gisSAPAssetSyncTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL) AND ACTIONTYPE = '" + actionType + "' AND TYPE = " + sapType;

            return GetData(sql);
        }
        /// <summary>
        /// Get all records with a specific assetID
        /// </summary>
        /// <param name="assetID">The assetID to search for</param>
        /// <returns>List of rows from the database with that assetID</returns>
        public IList<GISSAP_ASSETSYNCH> GetRecordsByAssetID(string assetID)
        {
            string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, dbms_lob.substr(sapattributes,32000,1) SAPATTRIBUTES FROM " + _gisSAPAssetSyncTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL AND PROCESSEDFLAG <> 'E') AND ASSETID = '" + assetID + "'";
            //string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES FROM " + _gisSAPAssetSyncTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL) AND ASSETID = '" + assetID + "'";
            return GetData(sql);
        }

        #region Below methods for  EDGIS Rearch ED06 Interface Improvemment -v1t8

        /// <summary>
        /// Inserts a row in table -v1t8
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertData(GISSAP_ASSETSYNCH data)
        {
            string insertsql = "";
            try
            {
                _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
                int rowsChanged = 0;

                #region Code commented for ED06-374 integration improvement - v1t8
                //insertsql = "INSERT into " + _gisSAPAssetSyncTableName +
                //    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES)  " +
                //    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :sapattributes)";

                //code modified for EDGIS ReArch
                // added for ED06-374 integration improvement - v1t8   ,PROCESSEDTIME 

                //insertsql = "INSERT into " + _gisSAPAssetSyncTableName +
                //    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES,RECORDID,PROCESSEDFLAG,ASSETTYPE) " +
                //    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :sapattributes, :recordID || LPAD(EDGIS.ED06_RECORDID_SEQ.NEXTVAL,7,0), :processedFlag, :assetType)";
                #endregion 

                insertsql = "INSERT into " + _gisSAPAssetSyncTableName +
    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES,RECORDID,PROCESSEDFLAG,ASSETTYPE) " +
    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :sapattributes, :recordID , :processedFlag, :assetType)";
                _logger.Debug(insertsql);

                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                using (var cmd = new OracleCommand(insertsql, _oracleConnection))
                {
                    cmd.Parameters.Add(new OracleParameter("assetid", data.ASSETID));
                    cmd.Parameters.Add(new OracleParameter("actiontype", data.ACTIONTYPE));
                    cmd.Parameters.Add(new OracleParameter("type", data.TYPE));
                    string dateString = data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss");
                    cmd.Parameters.Add(new OracleParameter("dateprocessed", dateString));
                    cmd.Parameters.Add(new OracleParameter("sapattributes", data.SAPATTRIBUTES));
                    cmd.Parameters.Add(new OracleParameter("recordID", data.RECORDID));
                    int s = Convert.ToInt32(data.ASSETTYPE);

                    cmd.Parameters.Add(new OracleParameter("processedFlag", data.PROCESSSEDFLAG));
                    cmd.Parameters.Add(new OracleParameter("assetType", s));
                    //   cmd.Parameters.Add(new OracleParameter("processedTime", data.PROCESSSEDTIME));
                    //  cmd.Parameters.Add(new OracleParameter("errordescription", data.ERRORDESCRIPTION));
                    rowsChanged = (int)cmd.ExecuteNonQuery();

                    _logger.Info("Successfully Inserted GIS record to send SAP in staging table :" + "RecordID :" + data.RECORDID + "GUID :" + data.ASSETID);
                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }

                return true;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Launch();
                }
                _logger.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw new Exception(string.Format("Failed inserting data for asset {0}. SQL Executed {1}. Error encountered: {2}", data.ASSETID, insertsql, ex.Message));
            }
        }

        /// <summary>
        /// Methood to get last objectID from databse for ED07 Interface -v1t8
        /// </summary>
        /// <returns>int</returns>
        public int GetObjectId()
        {
            int objectIdValue = 0;
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            try
            {               
                OracleCommand cmdSQL = new OracleCommand("select max(objectid) from edgis.sap_to_gis", _oracleConnection);
                OracleDataReader dataReader = cmdSQL.ExecuteReader();
                if (dataReader.FieldCount > 0)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader[0] == null || dataReader[0] == DBNull.Value)
                        { return objectIdValue; }
                        objectIdValue = Convert.ToInt32(dataReader[0]);
                        // objectid = objectIdValue++;
                       

                    }
                }
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }

            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                 _logger.Error("Exception occure while fetching the last ObjectID from ED07 staging table" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
            return objectIdValue;


        }

        /// <summary>
        /// Get RecordID sequence from databse -v1t8
        /// </summary>
        /// <returns></returns>
        private string GetSeq()
        {
            string seqValue = "";
            bool objectIdRead = false;
            try
            {
                _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
                OracleCommand cmdSQL = new OracleCommand("select To_CHAR( LPAD(EDGIS.ED06_RECORDID_SEQ.NEXTVAL,7,0)) FROM dual", _oracleConnection);
                OracleDataReader dataReader = cmdSQL.ExecuteReader();
                if (dataReader.FieldCount > 0)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader[0] == null || dataReader[0] == DBNull.Value)
                        { return seqValue; }
                        //seqValue = Convert.ToInt32(dataReader[0]);
                        seqValue = dataReader[0].ToString();
                        // objectid = objectIdValue++;
                        objectIdRead = true;

                    }
                }
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }

            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while fecting the record seq from EDGIS.ED06_RECORDID_SEQ" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                objectIdRead = false;
            }

            return seqValue;


        }

        /// <summary>
        /// Method to insert related record id in SAP_TO_GIS staging table for ED07 interface -v1t8
        /// </summary>
        /// <param name="recordID"></param>
        public void InsertRelatedRecordIDForED07(string recordID)
        {
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            string insertsql = "";
            int objectID = -1;
            try
            {
                int rowsChanged = 0;
                objectID = GetObjectId();
                objectID++;
                insertsql = "INSERT into " + ed07TableName + "(RELATEDRECORDID,OBJECTID) " + "VALUES (:recordID, :objectid )";
                _logger.Debug(insertsql);

                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                using (var cmd = new OracleCommand(insertsql, _oracleConnection))
                {
                    cmd.Parameters.Add(new OracleParameter("recordID", recordID));
                    cmd.Parameters.Add(new OracleParameter("objectid", objectID));
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                }

                _logger.Info("Sucessfully executed" + MethodInfo.GetCurrentMethod().Name);
                //  return true;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {               
                _logger.Error("Exception occure while inserting related recordID in ED07 staging table" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);              
            }
            //  return 
        }



        /// <summary>
        /// This method is used to insert data with error in assetSynch table to log -EDGIS Rearch-by v1t8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        public bool LogError(GISSAP_ASSETSYNCH data, string error)
        {
            string insertsql = "";
            try
            {
                _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
                _logger.Info("Proccessing the record with exception");

                int rowsChanged = 0;
                //code modified for EDGIS ReArch
                insertsql = "INSERT into " + _gisSAPAssetSyncTableName +
                    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED,SAPATTRIBUTES,RECORDID, PROCESSEDFLAG, ERRORDESCRIPTION)  " +
                    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :recordID, :processedFlag, :errordescription)";
                _logger.Debug(insertsql);
                _logger.Info("Insert sql :" + insertsql);
                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                using (var cmd = new OracleCommand(insertsql, _oracleConnection))
                {
                    cmd.Parameters.Add(new OracleParameter("assetid", data.ASSETID));
                    cmd.Parameters.Add(new OracleParameter("actiontype", data.ACTIONTYPE));
                    cmd.Parameters.Add(new OracleParameter("type", data.TYPE));
                    string dateString = data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss");
                    cmd.Parameters.Add(new OracleParameter("dateprocessed", dateString));
                    cmd.Parameters.Add(new OracleParameter("sapattributes", data.SAPATTRIBUTES));
                    cmd.Parameters.Add(new OracleParameter("recordID", data.RECORDID));
                    cmd.Parameters.Add(new OracleParameter("processedFlag", data.PROCESSSEDFLAG));
                    //cmd.Parameters.Add(new OracleParameter("processedTime", data.PROCESSSEDTIME));
                    cmd.Parameters.Add(new OracleParameter("errordescription", error.Replace("'", "''")));
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    _logger.Info("Successfully Inserted GIS record in staging table with exception:" + "RecordID :" + data.RECORDID + "GUID :" + data.ASSETID);

                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }
                return true;
            }

            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                _logger.Error ("Failed to Inserted GIS record in staging table with execption :" + "RecordID :" + data.RECORDID + "GUID :" + data.ASSETID + ex.Message );
                return false;

            }
        }

        /// <summary>
        /// Method to log the error in database if edit made in GIS not valid for SAP
        /// </summary>
        /// <param name="globalid"></param>
        /// <param name="error"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public void LogError(string globalid, string error, string recordID, string actionType)
        {
            string insertsql = "";
            try
            {
                _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
                int rowsChanged = 0;
                _logger.Info("Proccessing the record not valid for SAP");
                //code modified for EDGIS ReArch
                insertsql = "INSERT into " + _gisSAPAssetSyncTableName +
                    "(ASSETID,DATEPROCESSED,RECORDID, PROCESSEDFLAG,ACTIONTYPE,SAPATTRIBUTES,TYPE,ERRORDESCRIPTION)  " +
                    "VALUES (:assetid, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :recordID, :processedFlag, :actiontype, :sapattributes, :type, :errordescription)";
                _logger.Debug(insertsql);

                _logger.Info("Insert sql :" + insertsql );
                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                using (var cmd = new OracleCommand(insertsql, _oracleConnection))
                {
                    cmd.Parameters.Add(new OracleParameter("assetid", globalid));
                    DateTime DATEPROCESSED = DateTime.Now;
                    string dateString = DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss");
                    cmd.Parameters.Add(new OracleParameter("dateprocessed", dateString));
                    cmd.Parameters.Add(new OracleParameter("recordID", recordID));
                    cmd.Parameters.Add(new OracleParameter("processedFlag", ProcessFlag.GISError));
                    cmd.Parameters.Add(new OracleParameter("actiontype", actionType));
                    cmd.Parameters.Add(new OracleParameter("sapattributes", notApplicable));
                    int x = (int)SAPType.NotApplicable;
                    cmd.Parameters.Add(new OracleParameter("type", x));
                    cmd.Parameters.Add(new OracleParameter("errordescription", error));
                    rowsChanged = (int)cmd.ExecuteNonQuery();

                    _logger.Info("Successfully Inserted GIS record in staging table with error not valid for SAP:" + "RecordID :"+recordID + "GUID :" + globalid);
                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }
               
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error ("Failed to Insert GIS record in staging table with error not valid for SAP:" + "RecordID :" + recordID + "GUID :" + globalid  + ex.Message );               
            }
        }


        /// <summary>
        /// This method is used to get unique RecordID
        /// </summary>
        /// <returns></returns>
        public string GetRecordID(string recordType)
        {
            //Below is the foramt require for recordID ED06-374 integration improvement -v1t8
            //ED.F.06.20201228.110401.1234567 for Functional location
            //ED.E.06.20201228.110401.1234567 for Equipment
            //ED.S.06.20201228.110401.1234567 for Structure
            string recordID = string.Empty; string type = string.Empty;
            //if (recordType == "FunctionalLocation")
            //{ type = "F"; }
            //if (recordType == "StructureEquipment" || recordType == "StructureSubEquipment") { type = "S"; }

            //if (recordType == "DeviceEquipment" || recordType == "DeviceSubEquipment") { type = "E"; }

            if (recordType == SAPType.FunctionalLocation.ToString())
            { type = functionalLocation; }
            if (recordType == SAPType.StructureEquipment.ToString() || recordType == SAPType.StructureSubEquipment.ToString()) { type = supportStructure; }

            if (recordType == SAPType.DeviceEquipment.ToString() || recordType == SAPType.DeviceSubEquipment.ToString()) { type = device; }
            if (recordType == string.Empty) type = notApplicable;
            //   recordID = "ED" + "." + type + "." + "06" + "." + DateTime.Now.ToString("yyyyMMdd") + "." + DateTime.Now.ToString("hhmmss") + "." ;
            recordID = interfaceName.Substring(0, 2) + separator + type + separator + interfaceName.Substring(2, 2) + separator + DateTime.Now.ToString("yyyyMMdd") + separator + DateTime.Now.ToString("hhmmss") + separator;

            recordID = recordID + GetSeq();

            return recordID;
        }


        /// <summary>
        /// To Check last user /Modified user-v1t8
        /// </summary>
        /// <param name="row"></param>
        /// <returns>string</returns>
        public string GetLastUserValue(IRow row)
        {
            int fieldIndex = -1;
            ITable tbl = row.Table;
            fieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, lastUserField);
            if (fieldIndex == -1)
            {
                return "";
            }
            return row.get_Value((int)fieldIndex).ToString();
        }


        /// <summary>
        /// This method is used to validate if Last modified user is ED07 user -v1t8
        /// </summary>
        /// <param name="row"></param>
        /// <returns>bool</returns>
        public bool ValidateLastUser(IRow row)
        {
            string modifieduser = GetLastUserValue(row);

            if (modifieduser != "" || modifieduser != string.Empty)
            {
                if (modifieduser == ED07User) { return true; }
            }
            return false;
        }

        /// <summary>
        /// This method is used to validate if Last modified user is ED07 user -v1t8
        /// </summary>
        /// <param name="row"></param>
        /// <returns>bool</returns>
        public bool ValidateLastUser(DeleteFeat row)
        {
            string modifieduser = string.Empty; //GetLastUserValue(row);

            //dynamic activeRow = row;

            if (row.fields_Old.ContainsKey(lastUserField.ToUpper()))
                modifieduser = row.fields_Old[lastUserField.ToUpper()];
            else
                modifieduser = string.Empty;



            if (modifieduser != "" || modifieduser != string.Empty)
            {
                if (modifieduser == ED07User) { return true; }
            }
            return false;
        }

        #endregion 

        /// <summary>
        /// To update the error description in assetSynch table
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateErrorDescription(GISSAP_ASSETSYNCH data, string error)
        {
            string updateSql = string.Empty;
            try
            {
                int rowsChanged = 0;
                string dateString = "to_date('" + data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss") + "','mm/dd/yyyy hh24:mi:ss')";
                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                updateSql = string.Format("Update {0} Set ActionType='{5}', Type={1},DateProcessed={2},SapAttributes='{3}', ERRORDESCRIPTION='{6}' Where AssetID='{4}'",
                    _gisSAPAssetSyncTableName,
                    data.TYPE,
                    dateString,
                    data.SAPATTRIBUTES.Replace("'", "''"),
                    data.ASSETID,
                    data.ACTIONTYPE,
                    error.Replace("'", "''"));
                _logger.Debug(updateSql);


                using (var cmd = new OracleCommand(updateSql, _oracleConnection))
                {

                    rowsChanged = (int)cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {

                throw new Exception(string.Format("Failed updating data for asset {0}. SQL Executed {1}. Error encountered: {2}", data.ASSETID, updateSql, ex.Message));
            }
        }

        /// <summary>
        /// Updates a row in table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateData(GISSAP_ASSETSYNCH data)
        {
            string updateSql = string.Empty;
            try
            {
                int rowsChanged = 0;

                //string updateSql = "UPDATE " + _gisSAPAssetSyncTableName +
                //    " SET TYPE = :type, DATEPROCESSED = to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), SAPATTRIBUTES = :sapattributes " +
                //    "WHERE ASSETID = :assetid";

                string dateString = "to_date('" + data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss") + "','mm/dd/yyyy hh24:mi:ss')";
                //SAPAttributes must have single quoted characters escaped to be able to insert it via sql
                updateSql = string.Format("Update {0} Set ActionType='{5}', Type={1},DateProcessed={2},SapAttributes='{3}' Where AssetID='{4}'",
                    _gisSAPAssetSyncTableName,
                    data.TYPE,
                    dateString,
                    data.SAPATTRIBUTES.Replace("'", "''"),
                    data.ASSETID,
                    data.ACTIONTYPE);
                _logger.Debug(updateSql);

                using (var cmd = new OracleCommand(updateSql, _oracleConnection))
                {
                    //cmd.Parameters.Add(new OracleParameter("assetid", "'" + data.ASSETID + "'"));
                    //cmd.Parameters.Add(new OracleParameter("type", data.TYPE));
                    //string date = data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss");
                    //cmd.Parameters.Add(new OracleParameter("dateprocessed", date));
                    //OracleParameter dateParam = new OracleParameter("dateprocessed", OracleDbType.Date, OracleDate(DateTime.Now), ParameterDirection.Input);
                    //cmd.Parameters.Add(dateParam);
                    //cmd.Parameters.Add(new OracleParameter("sapattributes", "'" + data.SAPATTRIBUTES + "'"));


                    rowsChanged = (int)cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Launch();
#endif
                throw new Exception(string.Format("Failed updating data for asset {0}. SQL Executed {1}. Error encountered: {2}", data.ASSETID, updateSql, ex.Message));
            }
        }

        /// <summary>
        /// deletes a row from table
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool DeleteData(GISSAP_ASSETSYNCH data)
        {

            int rowsChanged = 0;
            string deletesql = "DELETE from " + _gisSAPAssetSyncTableName + " " +
                    "WHERE ASSETID = :assetid";
            _logger.Debug(deletesql);

            using (var cmd = new OracleCommand(deletesql, _oracleConnection))
            {

                cmd.Parameters.Add(new OracleParameter("assetid", data.ASSETID));
                rowsChanged = (int)cmd.ExecuteNonQuery();
            }

            return true;
        }

        /// <summary>
        /// Deletes all data from the staging table after data are written to csv files.
        /// </summary>
        public int DeleteData()
        {
            string deleteSql = "DELETE from " + _gisSAPAssetSyncTableName + " where (ActionType != '" + (char)ActionType.Invalid + "' and ActionType != '" + (char)ActionType.NotApplicable + "')";
            _logger.Debug(deleteSql);

            using (var deleteCommand = new OracleCommand(deleteSql, _oracleConnection))
            {
                int num = deleteCommand.ExecuteNonQuery();
                return num;
            }
        }

        /// <summary>
        /// Cleans session data from the TEMP staging table.
        /// </summary>
        /// <param name="sessionName">Session name</param>
        /// <returns>Number of records affected</returns>
        public int DeleteTempTableData(string sessionName)
        {
            //string deleteSql = "DELETE from " + _assetSyncTempTableName + " where SESSIONNAME= '" + sessionName + "'";
            string deleteSql = "DELETE from " + _assetSyncTempTableName;   //## ME Q3 RELEASE : Removed where clause for SessionName
            _logger.Debug(deleteSql);
            _logger.Debug("Deleting records from table : " + _assetSyncTempTableName);
            using (var deleteCommand = new OracleCommand(deleteSql, _oracleConnection))
            {
                int num = deleteCommand.ExecuteNonQuery();
                return num;
            }
        }

        /// <summary>
        /// Updates a row in the TEMP staging table.
        /// </summary>
        /// <param name="data">GISSAP_ASSETSYNCH object</param>
        /// <param name="sessionName">Session name</param>
        /// <returns>Boolean value</returns>
        public bool UpdateTempData(GISSAP_ASSETSYNCH data, string sessionName)
        {
            int rowsChanged = 0;

            string updateSql = string.Empty;
            string dateString = "to_date('" + data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss") + "','mm/dd/yyyy hh24:mi:ss')";
            updateSql = string.Format("Update {0} Set ActionType='{5}', Type={1},DateProcessed={2},SapAttributes='{3}' Where AssetID='{4}' And SESSIONNAME='{6}'",
                _gisSAPAssetSyncTableName,
                data.TYPE,
                dateString,
                data.SAPATTRIBUTES,
                data.ASSETID,
                data.ACTIONTYPE,
                sessionName);

            _logger.Debug(updateSql);

            using (var cmd = new OracleCommand(updateSql, _oracleConnection))
            {
                //cmd.Transaction = this.Transaction;
                rowsChanged = (int)cmd.ExecuteNonQuery();
            }

            return true;
        }

        /// <summary>
        /// Inserts a row in the TEMP staging table.
        /// </summary>
        /// <param name="data">ISSAP_ASSETSYNCH object</param>
        /// <param name="sessionName">Session name</param>
        /// <returns>Boolean value</returns>
        public bool InsertTempData(GISSAP_ASSETSYNCH data, string sessionName)
        {
            int rowsChanged = 0;

            //v1t8 need to modify for new stagging table schema point 5
            //original insert sql used by action handler process
            //string insertsql = "INSERT into " + _assetSyncTempTableName +
            //    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES, SESSIONNAME)  " +
            //    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :sapattributes, :sessionName)";

            // ## ME Q3-19 Release ## modified insert sql (Insert data in Main table insted of temp table) for batch process migration from action handler 
            // Fix : 03-Nov-20 : Checking existing asset id in asset synch table, if already exist then donot insert again
            var existingAssetRows = this.GetRecordsByAssetID(data.ASSETID);
            if (existingAssetRows.Count == 0)
            {
                string insertsql = "INSERT into " + _assetSyncTempTableName +
                    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES)  " +
                    "VALUES (:assetid, :actiontype, :type, to_date(:dateprocessed,'mm/dd/yyyy hh24:mi:ss'), :sapattributes)";

                //_logger.Debug(insertsql);

                try
                {
                    using (var cmd = new OracleCommand(insertsql, _oracleConnection))
                    {
                        //cmd.Transaction = this.Transaction;
                        cmd.Parameters.Add(new OracleParameter("assetid", data.ASSETID));
                        cmd.Parameters.Add(new OracleParameter("actiontype", data.ACTIONTYPE));
                        cmd.Parameters.Add(new OracleParameter("type", data.TYPE));
                        string dateString = data.DATEPROCESSED.ToString("MM/dd/yyyy HH:mm:ss");
                        cmd.Parameters.Add(new OracleParameter("dateprocessed", dateString));
                        cmd.Parameters.Add(new OracleParameter("sapattributes", data.SAPATTRIBUTES));
                        // cmd.Parameters.Add(new OracleParameter("sessionName", sessionName));     // ## ME Q3-19 Release ## commented for batch process migration from action handler 
                        insertsql = "INSERT into " + _assetSyncTempTableName +
                    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES)  " +
                    "VALUES (" + data.ASSETID + "," + data.ACTIONTYPE + "," + data.TYPE + "," + dateString + "," + data.SAPATTRIBUTES + ")";
                        _logger.Debug("Data Insertion start using SQL : " + insertsql);
                        rowsChanged = (int)cmd.ExecuteNonQuery();
                        _logger.Debug("Data Insertion successful for assetid : " + data.ASSETID);
                    }
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                catch (Exception ex)
                {
                    _logger.Debug("Error while inserting data in table " + _assetSyncTempTableName + " for assetid : " + data.ASSETID + " using insert SQL : " + insertsql + " exception found " + ex.ToString());
                }
            }
            else
            {
                _logger.Debug("Record is already exist in table " + _assetSyncTempTableName + " for assetid : " + data.ASSETID + " hence not inserting again.");
            }
            return true;
        }

        /// <param name="actionType"></param>
        /// <summary>
        /// This method constructs the actual sql that is executed
        /// </summary>
        /// <param name="sapType"></param>
        /// <returns></returns>
        public IList<GISSAP_ASSETSYNCH> GetTempData(string sessionName)
        {
            string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, dbms_lob.substr(sapattributes,32000,1) SAPATTRIBUTES FROM " + _assetSyncTempTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL) AND SESSIONNAME = '" + sessionName + "'";
            //string sql = "select ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES FROM " + _assetSyncTempTableName + " WHERE (ASSETID IS NOT NULL AND ACTIONTYPE IS NOT NULL AND TYPE IS NOT NULL AND DATEPROCESSED IS NOT NULL AND SAPATTRIBUTES IS NOT NULL) AND SESSIONNAME = '" + sessionName + "'";
            return GetData(sql);
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

                    if (this._oracleConnection != null)
                    {
                        this._oracleConnection.Close();
                        this._oracleConnection.Dispose();
                    }
                }

                this._oraTran = null;
                this._oracleConnection = null;
                this._disposed = true;
            }
        }
    }
}

