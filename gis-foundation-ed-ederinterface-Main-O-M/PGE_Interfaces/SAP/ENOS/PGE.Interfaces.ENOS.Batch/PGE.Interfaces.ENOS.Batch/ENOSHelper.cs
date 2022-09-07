using System;
using System.Collections; 
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection; 
using System.Runtime.InteropServices; 

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem; 
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.Geodatabase;
//using Miner.ComCategories;

namespace PGE.Interface.ENOS.Batch
{
    class ENOSHelper
    {

        private OracleConnection _pConn = null;
        private IWorkspace _pWS = null;
        private string _logfileName = "";
        private StreamWriter _logfileWriter = null;
        private ENOSConfig _pConfig; 

        public ENOSHelper(int processIdx, ENOSConfig pConfig)
        {
            //1. Create the folder with the date if necessary
            _pConfig = pConfig;
            string logFilename = _pConfig.LogfileName; 
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\"));
            string logFileDirectory = System.IO.Path.Combine(path, "ENOS Logfiles");
            string day = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            if (day.Length == 1) { day = "0" + day; }
            if (month.Length == 1) { month = "0" + month; }
            string year = DateTime.Today.Year.ToString();
            logFileDirectory = System.IO.Path.Combine(logFileDirectory, year + month + day);
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }

            //2. Set the logfile name 
            _logfileName = logFileDirectory + "\\" + logFilename + "_" + processIdx.ToString() + "_Log" + ".txt";
            System.IO.FileStream fs = File.Open(_logfileName, FileMode.OpenOrCreate);
            fs.Close();

            //3. Set the stream writer 
            int num = 0;
            bool createLogfile = true;
            try
            {
                _logfileWriter = new StreamWriter(_logfileName, true);
                createLogfile = false;
            }
            catch { }

            while (createLogfile)
            {
                if (!File.Exists(_logfileName))
                {
                    try
                    {
                        _logfileWriter = new StreamWriter(_logfileName, true);
                        createLogfile = false;
                    }
                    catch { }
                }
                num++;
            }
        }

        /// <summary>
        /// Logs the message to the underlying streamwriter
        /// </summary>
        /// <param name="log">Message to log</param>
        public void Log(string log)
        {
            //Console.WriteLine(DateTime.Now.ToString() + ": " + log);
            _logfileWriter.WriteLine(DateTime.Now.ToString() + ": " + log);
            _logfileWriter.Flush();
        }

        /// <summary>
        /// Creates the edit version used to store ENOS Generation updates 
        /// </summary>
        /// <param name="connectionfile"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        public bool CreateEditVersion(int processIdx)
        {
            try
            {
                //Connect the workspace  
                if (_pWS == null)
                {
                    _pWS = GetWorkspace(
                        ENOSCommon.DEFAULT_VERSION,
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.EditUser,
                        _pConfig.EditUserPassword);
                }
                IVersionedWorkspace pVWS = (IVersionedWorkspace)_pWS;
                IVersion pVersion = null;
                string editVersionName = _pConfig.ReconcileVersion + processIdx.ToString();

                if (!VersionExists(pVWS, editVersionName))
                {
                    Log("Version does not exist - so creating the version: " + editVersionName); 
                    pVersion = pVWS.DefaultVersion.CreateVersion(editVersionName);
                    pVersion.Access = esriVersionAccess.esriVersionAccessPublic; 
                }

                if (VersionExists(pVWS, editVersionName))
                {
                    _pWS = GetWorkspace(
                        _pConfig.EditUser + "." + editVersionName,
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.EditUser,
                        _pConfig.EditUserPassword);
                    return true;
                }
                else
                    return false; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                return false; 
            }
        }

        /// <summary>
        /// Return a bool to indicate whether the passed version exists 
        /// </summary>
        /// <param name="pVWS"></param>
        /// <param name="versionName"></param>
        /// <returns></returns>
        private bool VersionExists(IVersionedWorkspace pVWS, string versionName)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod()); 

                bool versionExists = false; 
                IVersion pVersion = pVWS.FindVersion(versionName);
                if (pVersion != null)
                    versionExists = true;
                return versionExists; 
            }
            catch
            {
                return false; 
            }
        }

        /// <summary>
        /// Deletes all versions edited through the application  
        /// </summary>
        /// <returns></returns>
        public void DeleteVersions()
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());

                IWorkspace pWS = null;
                IVersion pVersion = null; 

                for (int j = 0; j < _pConfig.ChildProcesses.Length; j++)
                {
                    ChildProcess pChildProc = _pConfig.ChildProcesses[j];
                    pWS= GetWorkspace( 
                        _pConfig.ReconcileVersion + pChildProc.ProcessIndex.ToString(), 
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.EditUser,
                        _pConfig.EditUserPassword); 
                    if (pWS != null) 
                    {                        
                        pVersion = (IVersion)pWS;
                        Log("Deleting " + pVersion.VersionInfo.VersionName);
                        try
                        {
                            pVersion.Delete();
                            Log("Deleted");
                        }
                        catch (Exception ex)
                        {
                            Log("Error deleting version: " + ex.Message);
                        }
                    }
                }

                //Parent version 
                if (_pWS != null)
                {
                    Marshal.FinalReleaseComObject(_pWS);
                    _pWS = null; 
                }
                pWS = GetWorkspace(
                        _pConfig.ReconcileVersion + ENOSCommon.PARENT_VERISON_INDEX.ToString(),
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.EditUser,
                        _pConfig.EditUserPassword);
                if (pWS != null)
                {
                    pVersion = (IVersion)pWS;
                    Log("Deleting " + pVersion.VersionInfo.VersionName);
                    try
                    {
                        pVersion.Delete();
                        Log("Deleted");
                    }
                    catch (Exception ex)
                    {
                        Log("Error deleting version: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error deleting the edit version");
            }
        }

        /// <summary>
        /// Archives records from the staging table (ENOS_Stage) into the 
        /// archive table (ENOS_Archive) 
        /// </summary>
        /// <returns></returns>
        public void Archive()
        {
            try
            {
                string connectionString = BuildOracleConnectString(
                    _pConfig.EditUser,
                    _pConfig.EditUserPassword,
                    _pConfig.Service);
                if (_pConn == null)
                    _pConn = new OracleConnection(connectionString);
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();

                //Run an INSERT INTO SELECT statement for the archive 
                string fieldList =
                    ENOSCommon.ET_ENOS_REF_ID_FLD + "," + 
                    ENOSCommon.ET_ENOS_STATUS_FLD + "," + 
                    ENOSCommon.ET_EQUIPMENT_ID_FLD + "," +
                    ENOSCommon.ET_EQUIPMENT_TYPE_FLD + "," + 
                    ENOSCommon.ET_GENERATION_STATUS_FLD + "," + 
                    ENOSCommon.ET_INVERTERID_FLD + "," +
                    ENOSCommon.ET_MANUFACTURER_FLD + "," + 
                    ENOSCommon.ET_MODEL_FLD + "," + 
                    ENOSCommon.ET_POWER_SOURCE_FLD + "," +
                    ENOSCommon.ET_QUANTITY_FLD + "," + 
                    ENOSCommon.ET_RATING_FLD + "," + 
                    ENOSCommon.ET_SERVICE_POINT_ID_FLD + "," + 
                    ENOSCommon.ET_STATUS_FLD;

                string sql =
                    "INSERT INTO " +
                    ENOSCommon.ENOS_ARCHIVE_TBL + " " +
                    "(" + fieldList + ")" + " " +
                    "SELECT" + " " + fieldList + " " + 
                    "FROM" + " " + ENOSCommon.ENOS_STAGE_TBL;
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                int recordsUpdated = pCmd.ExecuteNonQuery();
                
                //Run an UPDATE statement to set the ARCHIVE_DATE field 
                sql = "UPDATE " + ENOSCommon.ENOS_ARCHIVE_TBL + " SET " +
                    ENOSCommon.ET_ARCHIVE_DATE_FLD + " = " + ENOSCommon.ORACLE_SYSDATE +
                    " Where " +
                    ENOSCommon.ET_ARCHIVE_DATE_FLD + " IS NULL";

                pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                recordsUpdated = pCmd.ExecuteNonQuery();
            }
            catch (Exception ex) 
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error arhiving ENOS records"); 
            }
        }

        /// <summary>
        /// Concatenates connection details to form ADO.NET connection string
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private string BuildOracleConnectString(string user, string password, string service)
        {
            try
            {
                ENOSEncryption pEncrypt = new ENOSEncryption(); 
                string connectString = "user id="; 
                connectString += user; 
                connectString += ";password=";
                connectString += pEncrypt.Decrypt(password);
                connectString += ";data source="; 
                connectString += service;
                return connectString; 
            }
            catch (Exception ex) 
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error building Oracle ADO.NET Connect String");  
            }
        }

        /// <summary>
        /// This function stops the ENOS_Archive table from growing 
        /// uncontrollably by deleting all archived records that are 
        /// older than the configured MaxArchiveAge 
        /// </summary>
        public void DeleteOldArchiveRecords()
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());                

                //Setup oracle connection 
                string connectionString = BuildOracleConnectString(
                    _pConfig.EditUser,
                    _pConfig.EditUserPassword,
                    _pConfig.Service);
                if (_pConn == null)
                    _pConn = new OracleConnection(connectionString);
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();

                //Delete all records older than configured: MaxArchiveAge (months)                                
                string sql =
                    "DELETE FROM " +
                    ENOSCommon.ENOS_ARCHIVE_TBL + " " +
                    "WHERE" + " " +
                    ENOSCommon.ET_ARCHIVE_DATE_FLD + " " + "<=" + " " + "ADD_MONTHS(TRUNC(SYSDATE), " + 
                    "-" + _pConfig.MaxArchiveAge.ToString() + ")";
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                int recordsUpdated = pCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error deleting old ENOS archive records");
            }
        }

        /// <summary>
        /// This function deletes all records from the ENOS_STAGE
        /// table 
        /// </summary>
        public void DeleteAllENOSStageRecords()
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());

                //Check the config flag is set to delete the staging 
                if (!_pConfig.DeleteENOSStage)
                {
                    Log("Config setting DeleteENOSStage is set to false - returning");
                    return;
                }

                //Setup oracle connection 
                string connectionString = BuildOracleConnectString(
                    _pConfig.EditUser,
                    _pConfig.EditUserPassword,
                    _pConfig.Service);
                if (_pConn == null)
                    _pConn = new OracleConnection(connectionString);
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();

                //Delete all records from ENOS_STAGE                                
                string sql = "DELETE FROM " + ENOSCommon.ENOS_STAGE_TBL;
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                int recordsUpdated = pCmd.ExecuteNonQuery();
                Log("Records deleted: " + recordsUpdated.ToString()); 

                Log("Leaving " + MethodBase.GetCurrentMethod()); 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error deleting records from " + ENOSCommon.ENOS_STAGE_TBL);
            }
        }

        /// <summary>
        /// Adds an error to the ENOS_ERROR table for all equipment 
        /// records for a servicepoint 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="service"></param>
        /// <param name="errorMsg"></param>
        public void AddENOSError(
            ref Hashtable hshENOSEquipErrorList, 
            List<GenEquipmentItem> pEquipItems, 
            string localOffice, 
            string errorMsg)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   pEquipItems.Count " + pEquipItems.Count.ToString());
                Log("   errorMsg " + errorMsg);

                //If no equipment items then no need to do anything 
                if (pEquipItems.Count == 0)
                    return; 

                //Get oracle connection 
                if (_pConn == null)
                    throw new Exception("No ADO.NET connection available");
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();                             

                //Insert the ENOS Error records that are NOT in the   
                //ENOSEquipErrorList 
                int recordsUpdated = -1; 
                OracleCommand pCmd = null; 
                string fieldList = "";
                string sql = ""; 
                string addEquip = "";
                string allEquip = ""; 
                int addCount = 0;
                int allCount = 0; 
                foreach (GenEquipmentItem pEquipItem in pEquipItems)
                {
                    if (allEquip == "")
                        allEquip += pEquipItem.EquipmentId.ToString();
                    else
                        allEquip += "," + pEquipItem.EquipmentId.ToString();
                    allCount++; 

                    if (!hshENOSEquipErrorList.ContainsKey(pEquipItem.EquipmentId))
                    {
                        if (addEquip == "")
                            addEquip += pEquipItem.EquipmentId.ToString(); 
                        else
                            addEquip += "," + pEquipItem.EquipmentId.ToString();
                        addCount++; 
                    }
                }

                //Do not run the INSERT unless there are some records to add 
                if (addCount != 0)
                {
                    fieldList =
                        ENOSCommon.ET_ENOS_REF_ID_FLD + "," + 
                        ENOSCommon.ET_ENOS_STATUS_FLD + "," + 
                        ENOSCommon.ET_EQUIPMENT_ID_FLD + "," +
                        ENOSCommon.ET_EQUIPMENT_TYPE_FLD + "," +
                        ENOSCommon.ET_GENERATION_STATUS_FLD + "," +
                        ENOSCommon.ET_INVERTERID_FLD + "," +
                        ENOSCommon.ET_MANUFACTURER_FLD + "," + 
                        ENOSCommon.ET_MODEL_FLD + "," + 
                        ENOSCommon.ET_POWER_SOURCE_FLD + "," +
                        ENOSCommon.ET_QUANTITY_FLD + "," + 
                        ENOSCommon.ET_RATING_FLD + "," + 
                        ENOSCommon.ET_SERVICE_POINT_ID_FLD + "," + 
                        ENOSCommon.ET_STATUS_FLD;

                    //Run an INSERT INTO SELECT statement for the error  
                    sql = "INSERT INTO " +
                        ENOSCommon.ENOS_ERROR_TBL + " " +
                        "(" +
                            fieldList + "," +
                                ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," +
                                ENOSCommon.ET_LOCAL_OFFICE_FLD + "," +
                                ENOSCommon.ET_ERROR_DATE_FLD + 
                        ")" + " " +
                        "SELECT" + " " +
                            fieldList + "," +
                                "'" + errorMsg + "'" + " AS " + ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," +
                                "'" + localOffice + "'" + " AS " + ENOSCommon.ET_LOCAL_OFFICE_FLD + "," +
                                ENOSCommon.ORACLE_SYSDATE + " AS " + ENOSCommon.ET_ERROR_DATE_FLD + " " +
                        "FROM" + " " +
                        ENOSCommon.ENOS_STAGE_TBL + " " +
                        "WHERE" + " " +
                        ENOSCommon.ET_EQUIPMENT_ID_FLD + " IN(" + addEquip + ")";

                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sql;
                    Log("Executing insert only");
                    recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated != addCount)
                    {
                        Log("Error inserting into ENOS_Error - initial insert, recordsUpdated: " + recordsUpdated.ToString());
                    }
                }

                //Lastly update the ENOSErrorList 
                foreach (GenEquipmentItem pEquipItem in pEquipItems)
                {
                    if (!hshENOSEquipErrorList.ContainsKey(pEquipItem.EquipmentId))
                        hshENOSEquipErrorList.Add(pEquipItem.EquipmentId, 0); 
                }
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error arhiving ENOS records");
            }
        }

        /// <summary>
        /// Adds an error to the ENOS_ERROR table for all equipment 
        /// records for a servicepoint 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="service"></param>
        /// <param name="errorMsg"></param>
        public void AddENOSError(
            ref Hashtable hshENOSEquipErrorList,
            Int64 equipId,
            string localOffice,
            string errorMsg)
        {
            try
            {
                //Copying records from ENOS_STAGE to ENOS_ERROR with appropriate description / date 
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   equipId " + equipId.ToString());
                Log("   errorMsg " + errorMsg);

                //Get oracle connection 
                if (_pConn == null)
                    throw new Exception("No ADO.NET connection available");
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();

                //Insert the ENOS Error records that are NOT in the   
                //ENOSEquipErrorList 
                OracleCommand pCmd = null; 
                string addEquip = "";
                string allEquip = equipId.ToString();
                string fieldList = "";
                string sql = ""; 
                int addCount = 0;
                int recordsUpdated = -1; 

                if (!hshENOSEquipErrorList.ContainsKey(equipId))
                {
                    addCount = 1;
                    addEquip = equipId.ToString();
                    fieldList =
                        ENOSCommon.ET_ENOS_REF_ID_FLD + "," +
                        ENOSCommon.ET_ENOS_STATUS_FLD + "," +
                        ENOSCommon.ET_EQUIPMENT_ID_FLD + "," +
                        ENOSCommon.ET_EQUIPMENT_TYPE_FLD + "," +
                        ENOSCommon.ET_GENERATION_STATUS_FLD + "," +
                        ENOSCommon.ET_INVERTERID_FLD + "," +
                        ENOSCommon.ET_MANUFACTURER_FLD + "," +
                        ENOSCommon.ET_MODEL_FLD + "," +
                        ENOSCommon.ET_POWER_SOURCE_FLD + "," +
                        ENOSCommon.ET_QUANTITY_FLD + "," +
                        ENOSCommon.ET_RATING_FLD + "," +
                        ENOSCommon.ET_SERVICE_POINT_ID_FLD + "," +
                        ENOSCommon.ET_STATUS_FLD; 
                    
                    
                    //Run an INSERT INTO SELECT statement for the error  
                    sql = "INSERT INTO " +
                        ENOSCommon.ENOS_ERROR_TBL + " " +
                        "(" + 
                            fieldList + "," + 
                                ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," + 
                                ENOSCommon.ET_LOCAL_OFFICE_FLD + "," + 
                                ENOSCommon.ET_ERROR_DATE_FLD + 
                        ")" + " " +
                        "SELECT" + " " + 
                            fieldList + "," + 
                                "'" + errorMsg + "'" + " AS " + ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," +
                                "'" + localOffice + "'" + " AS " + ENOSCommon.ET_LOCAL_OFFICE_FLD + "," +
                                ENOSCommon.ORACLE_SYSDATE + " AS " + ENOSCommon.ET_ERROR_DATE_FLD + " " + 
                        "FROM" + " " +
                        ENOSCommon.ENOS_STAGE_TBL + " " +
                        "WHERE" + " " +
                        ENOSCommon.ET_EQUIPMENT_ID_FLD + " IN(" + addEquip + ")";

                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sql;
                    Log("Executing insert only");
                    recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated != addCount)
                    {
                        Log("Error inserting into ENOS_Error - initial insert, recordsUpdated: " + recordsUpdated.ToString());
                    }
                }

                //Lastly update the ENOSErrorList
                if (!hshENOSEquipErrorList.ContainsKey(equipId))
                    hshENOSEquipErrorList.Add(equipId, 0);
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error adding ENOS Error record");
            }
        }

        /// <summary>
        /// Adds an error to the ENOS_ERROR table for all equipment 
        /// records for a servicepoint 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="service"></param>
        /// <param name="errorMsg"></param>
        public Hashtable GetENOSErrorList()
        {
            try
            {
                //Get oracle connection
                string connectionString = BuildOracleConnectString(
                    _pConfig.EditUser,
                    _pConfig.EditUserPassword,
                    _pConfig.Service);  
                if (_pConn == null)
                    _pConn = new OracleConnection(connectionString);
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();

                //Run an INSERT INTO SELECT statement for the error 
                Hashtable hshENOSErrorList = new Hashtable(); 
                string sql = "SELECT" + " " + ENOSCommon.ET_EQUIPMENT_ID_FLD + " " + 
                    "FROM" + " " + 
                    ENOSCommon.ENOS_ERROR_TBL; 
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                Int64 equipId = -1; 
                OracleDataReader reader = pCmd.ExecuteReader();
                while (reader.Read())
                {
                    equipId = reader.GetInt64(reader.GetOrdinal(
                                    ENOSCommon.ET_EQUIPMENT_ID_FLD));
                    if (!hshENOSErrorList.ContainsKey(equipId))
                        hshENOSErrorList.Add(equipId, 0);                     
                }
                if (reader != null)
                    reader.Close();

                //Return the hashtable 
                return hshENOSErrorList; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error arhiving ENOS records");
            }
        }

        /// <summary>
        /// Returns a hashtable of all ServicePoints that have 
        /// Generation Equipment records. The hasthable will be 
        /// keyed with ServicePointGUID and values will be OIds 
        /// </summary>
        /// <returns></returns>
        public Hashtable GetServicePointsWithGeneration()
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());

                //Get the sde connection must be looking at version 
                //SDE.DEFAULT 
                IWorkspace pWS = GetWorkspace(
                ENOSCommon.DEFAULT_VERSION,
                _pConfig.ServicePrefix + _pConfig.Service,
                _pConfig.EditUser,
                _pConfig.EditUserPassword);
                
                Hashtable hshInverters = new Hashtable();
                Hashtable hshGenerationFields = new Hashtable();
                IVersion pVersion = (IVersion)pWS;
                Log("Calling RefreshVersion");
                pVersion.RefreshVersion(); 
                IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;
                Log("Operating on version: " + pVersion.VersionInfo.VersionName);
                if (!pVersion.VersionInfo.VersionName.Contains("DEFAULT"))
                    Log("Versioning error: " + pVersion.VersionInfo.VersionName); 

                //Open required tables 
                ITable pGenerationTable =
                    pFWS.OpenTable(ENOSCommon.GENERATION_TBL);

                //Search for all generation spids which have generation  
                IQueryFilter pQF = new QueryFilterClass();
                pQF.SubFields = ENOSCommon.SERVICEPOINTGUID_FLD;
                pQF.WhereClause =
                    ENOSCommon.SERVICEPOINTGUID_FLD + " is not null" + " AND " +
                    ENOSCommon.GENTYPE_FLD + " " + "<>" + " " + ((int)EquipmentType.equipTypeInverter).ToString(); 
                Log("Query on generation table returns: " + pGenerationTable.RowCount(pQF).ToString() + 
                    " servicepoint rows"); 
                
                Hashtable hshSPsWithGeneration = new Hashtable();
                ICursor pCursor = pGenerationTable.Search(pQF, false);
                int fldIdx = -1; 
                IRow pRow = pCursor.NextRow();
                string spId = "";

                while (pRow != null)
                {
                    if (fldIdx == -1)
                        fldIdx = pRow.Fields.FindField(ENOSCommon.SERVICEPOINTGUID_FLD);
                    spId = pRow.get_Value(fldIdx).ToString();
                    if (!hshSPsWithGeneration.ContainsKey(spId))
                        hshSPsWithGeneration.Add(spId, 0);
                    pRow = pCursor.NextRow();
                }
                Marshal.FinalReleaseComObject(pCursor);

                //Release workspace 
                Marshal.FinalReleaseComObject(pWS);

                //Return the hashtable of SPIDs
                Log("Found " + hshSPsWithGeneration.Count.ToString() + " servicepoints with generation");
                Log("Leaving " + MethodBase.GetCurrentMethod());
                return hshSPsWithGeneration; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning the SPIDs with Generation");
            }
        }

        /// <summary>
        /// Sets the GenType field on the ServiceLocation feature to indicate 
        /// the ServiceLocation has one or more servicepoints with Generation 
        /// (this can be used to display servicelocations with generation differently 
        /// from servicelocations which have no servicepoints that have generation)  
        /// </summary>
        public void UpdateServiceLocation(Hashtable hshSPsWithGeneration)
        {
            IWorkspaceEdit2 pWSE = null;
            IMMAutoUpdater autoupdater = null;
            mmAutoUpdaterMode oldMode = Miner.Interop.mmAutoUpdaterMode.mmAUMArcMap;

            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   hshSPsWithGeneration.Count " + hshSPsWithGeneration.Count.ToString());

                //Get the sde connection to the target database 
                CreateEditVersion(ENOSCommon.PARENT_VERISON_INDEX);
                _pWS = GetWorkspace(
                    _pConfig.EditUser + "." + _pConfig.ReconcileVersion + ENOSCommon.PARENT_VERISON_INDEX.ToString(),
                    _pConfig.ServicePrefix + _pConfig.Service,
                    _pConfig.EditUser,
                    _pConfig.EditUserPassword);

                IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;
                pWSE = (IWorkspaceEdit2)_pWS;

                //Turn off AUs 
                Log("Turning off AUs");
                object objAutoUpdater = null;
                objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
                autoupdater = (IMMAutoUpdater)objAutoUpdater;
                oldMode = autoupdater.AutoUpdaterMode;
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                                
                //1. Get a list of all servicelocation features with generation
                Log("Step 1. Get a list of all servicelocation features with generation");

                //Start edit session / operation 
                pWSE.StartEditing(false);
                pWSE.StartEditOperation(); 

                //Open required tables 
                IFeatureClass pServiceLocationFC =
                    pFWS.OpenFeatureClass(ENOSCommon.SERVICELOCATION_FC);
                ITable pServicePointTable =
                    pFWS.OpenTable(ENOSCommon.SERVICEPOINT_TBL);

                //Search for all servicepoints with generation and return 
                //the servicelocationguid 
                Hashtable hshSLsWithGeneration = new Hashtable();
                ICursor pCursor = null;
                IRow pRow = null;
                string spId = "";
                string slId = "";
                int globalIdFldIdx = -1;
                int slGUIDFldIdx = -1;
                string[] commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(
                    hshSPsWithGeneration, 1000, true);
                IQueryFilter pQF = new QueryFilterClass();
                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    //could change this to use an IN() clause and break into 
                    //batches of 1000 
                    Log("Getting servicelocations with generation: " + 
                        i.ToString() + " of: " + commaSeparatedKeys.Length.ToString()); 
                    pQF.SubFields = "globalid, servicelocationguid";
                    pQF.WhereClause = "globalid" + " IN(" + commaSeparatedKeys[i] + ")";
                    pCursor = pServicePointTable.Search(pQF, false);
                    pRow = pCursor.NextRow(); 
                    
                    while (pRow != null)
                    {
                        if (globalIdFldIdx == -1)
                            globalIdFldIdx = pRow.Fields.FindField("globalid");
                        if (slGUIDFldIdx == -1)
                            slGUIDFldIdx = pRow.Fields.FindField("servicelocationguid");
                        spId = pRow.get_Value(globalIdFldIdx).ToString();
                        slId = pRow.get_Value(slGUIDFldIdx).ToString();
                        if (!hshSLsWithGeneration.ContainsKey(slId))
                            hshSLsWithGeneration.Add(slId, 0);
                        pRow = pCursor.NextRow();
                    }
                    Marshal.FinalReleaseComObject(pCursor);
                }
                Log("hshSLsWithGeneration.Count " + hshSLsWithGeneration.Count.ToString());
                                
                //2. Find all ServiceLocation features which have a GenType 
                //which indicates it HAS generation and verify the slId is 
                //in our SLsWithGeneration, otherwise reset GenType to None
                Log("Step 2. Updating ServiceLocation Gentype to None where no generation is found");
                globalIdFldIdx = -1;
                int genCategoryFldIdx = -1; 
                pQF = new QueryFilterClass(); 
                pQF.WhereClause = "gencategory" + " = " + ((int)GenType.genTypeDC).ToString();
                pQF.SubFields = "globalid" + "," + "gencategory";
                IFeatureCursor pFCursor = pServiceLocationFC.Update(pQF, false);
                IFeature pFeature = pFCursor.NextFeature();

                while (pFeature != null)
                {
                    if (globalIdFldIdx == -1)
                        globalIdFldIdx = pFeature.Fields.FindField("globalid");
                    if (genCategoryFldIdx == -1)
                        genCategoryFldIdx = pFeature.Fields.FindField("gencategory");

                    slId = pFeature.get_Value(globalIdFldIdx).ToString();
                    if (hshSLsWithGeneration.ContainsKey(slId))
                    {
                        //We can remove these since they already have correct 
                        //GenCategory  
                        hshSLsWithGeneration.Remove(slId); 
                    }
                    else 
                    {
                        //Set the gentype to None  
                        pFeature.set_Value(genCategoryFldIdx, ((int)GenType.genTypeNone));
                        pFCursor.UpdateFeature(pFeature); 
                        //pFeature.Store(); 
                    }
                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);
                
                //3. Find all ServiceLocation features which have a GenType 
                //which indicates it HAS NO generation and set this to GenType 
                //DC where the spid is in the list from step 1. 
                
                //could change this to use an IN() clause and break into 
                //batches of 1000
                Log("Step 3. Updating remaining SLs where generation is found");
                Log("hshSLsWithGeneration.Count " + hshSLsWithGeneration.Count.ToString()); 
                genCategoryFldIdx = -1; 
                pQF = new QueryFilterClass(); 
                commaSeparatedKeys = null; 
                commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(
                    hshSLsWithGeneration, 1000, true);
                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    Log(i.ToString() + " of: " + commaSeparatedKeys.Length.ToString());
                    pQF.WhereClause = "globalid" + " IN(" + commaSeparatedKeys[i] + ")";
                    pFCursor = pServiceLocationFC.Update(pQF, false);
                    pFeature = pFCursor.NextFeature();

                    while (pFeature != null)
                    {
                        //Set the gentype to DC
                        if (genCategoryFldIdx == -1)
                            genCategoryFldIdx = pFeature.Fields.FindField("gencategory");
                        pFeature.set_Value(genCategoryFldIdx, ((int)GenType.genTypeDC));
                        pFCursor.UpdateFeature(pFeature); 
                        //pFeature.Store();

                        pFeature = pFCursor.NextFeature(); 
                    }
                    Marshal.FinalReleaseComObject(pCursor);
                }
               
                //Stop edit session / operation 
                pWSE.StopEditOperation();
                pWSE.StopEditing(true);
                Log("Leaving " + MethodBase.GetCurrentMethod());
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);

                if (pWSE.IsInEditOperation)
                    pWSE.AbortEditOperation();
                if (pWSE.IsBeingEdited())
                    pWSE.StopEditing(false);

                Log("Edits aborted");
                throw new Exception("Error updating ServiceLocation");  
            }
            finally
            {
                //Switch AUs back again
                Log("Turning AUs back to original state");
                if (autoupdater != null)
                    autoupdater.AutoUpdaterMode = oldMode;
            }
        }

        /// <summary>
        /// Returns a lit of comma separated keys to allow use in a SQL 
        /// IN() clause 
        /// </summary>
        /// <param name="hshKeys"></param>
        /// <param name="batchSize"></param>
        /// <param name="addApostrophe"></param>
        /// <returns></returns>
        private string[] GetArrayOfCommaSeparatedKeys(Hashtable hshKeys, int batchSize, bool addApostrophe)
        {
            try
            {
                Hashtable hshCommaSeparatedKeys = new Hashtable(); 
                int counter = 0;
                StringBuilder batchLine = new StringBuilder();  

                foreach (object key in hshKeys.Keys)
                {
                    if (counter == 0)
                    {
                        if (addApostrophe) 
                            batchLine.Append("'" + key.ToString() + "'"); 
                        else
                            batchLine.Append(key.ToString());
                    }
                    else
                    {
                        if (addApostrophe)
                            batchLine.Append("," + "'" + key.ToString() + "'");
                        else
                            batchLine.Append("," + key.ToString());
                    }

                    counter++;
                    if (counter == batchSize)
                    {
                        hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);
                        batchLine = new StringBuilder();
                        counter = 0; 
                    }
                }

                //Add what is left over 
                if (batchLine.ToString().Length != 0)
                    hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);

                //Convert this to an array 
                counter = 0; 
                string[] commaSepKeys = new string[hshCommaSeparatedKeys.Count];
                foreach (string line in hshCommaSeparatedKeys.Keys)
                {
                    commaSepKeys[counter] = line;
                    counter++; 
                }

                //return array 
                return commaSepKeys; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning array of comma separated keys");
            }
        }

        /// <summary>
        /// The idea is that equipment records whould have been loaded 
        /// into the EDGIS.Generation table, or they should appear in the 
        /// ENOS_Error table. Where an equipment record has been loaded 
        /// successfully it should not appear in the ENOS_Error table. 
        /// This routine ensoures that all equipment records that appear 
        /// in the Generation table are removed from the ENOS_Error table 
        /// </summary>
        /// <param name="pConfig"></param>
        public void UpdateENOSError(ENOSConfig pConfig)
        {
            try
            {                
                Log("Entering " + MethodBase.GetCurrentMethod());

                //Get the sde connection to the target database
                _pWS = GetWorkspace(
                        ENOSCommon.DEFAULT_VERSION,
                        pConfig.ServicePrefix + pConfig.Service,
                        pConfig.EditUser,
                        pConfig.EditUserPassword);
                IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;

                //Open required tables 
                ITable pGenerationTable =
                    pFWS.OpenTable(ENOSCommon.GENERATION_TBL);
                
                //1. Get a equip records in Generation table 
                Hashtable hshGenerationEquipIds = GetEquipmentIdList(pGenerationTable);
                //2. Get a equip records in ENOS_Error table 
                Hashtable hshENOSErrorEquipIds = GetENOSErrorList(); 

                //3. Find a list of all equipment items that are in the 
                //ENOS_ERROR table that are also in the Generation 
                //table, these records must be delete from ENOS_ERROR 
                Hashtable hshENOSErrorDeleteList = new Hashtable();
                foreach (Int64 equipId in hshENOSErrorEquipIds.Keys)
                {
                    if (hshGenerationEquipIds.ContainsKey(equipId))  
                        hshENOSErrorDeleteList.Add(equipId, 0); 
                }
                Log("Found: " + hshENOSErrorDeleteList.Count.ToString() +
                    " records to be deleted from ENOS_Error");
                if (hshENOSErrorDeleteList.Count == 0)
                    return; 

                //Get oracle connection 
                if (_pConn == null)
                    throw new Exception("No ADO.NET connection available");
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();                
                string[] commaSeparatedkeyList = GetArrayOfCommaSeparatedKeys(
                    hshENOSErrorDeleteList, 1000, false);
                string sql = "";
                int recordsUpdated = -1; 
                OracleCommand pCmd = null;

                for (int i = 0; i < commaSeparatedkeyList.Length; i++)
                {
                    sql = "DELETE" + " " +
                    "FROM" + " " + ENOSCommon.ENOS_ERROR_TBL + " " + 
                    "WHERE" + " " +
                    ENOSCommon.ET_EQUIPMENT_ID_FLD + " IN(" + commaSeparatedkeyList[i] + ")";

                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sql;
                    recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 0)
                    {
                        Log("Error deleting records from ENOS_Error");
                    }                    
                }
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error updating ENOS_Error table");  
            }
        }
        
        /// <summary>
        /// Reconcile and posts the passed version 
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <param name="reconcileVersionName"></param>
        /// <param name="targetVersionName"></param>
        /// <param name="acquireLock"></param>
        /// <param name="abortIfConflicts"></param>
        /// <param name="childWins"></param>
        /// <param name="columnLevel"></param>
        /// <param name="postAfterReconcile"></param>
        /// <returns></returns>
        public bool ReconcileAndPost(
            string postUser,
            string postUserPassword, 
            string servicePrefix, 
            string service, 
            string reconcileVersionName, 
            string targetVersionName, 
            bool acquireLock, 
            bool abortIfConflicts, 
            bool childWins, 
            bool columnLevel, 
            bool postAfterReconcile)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod()); 

                //Connect the workspace  
                IWorkspace pWS = GetWorkspace( 
                    reconcileVersionName, 
                    servicePrefix + service, 
                    postUser, 
                    postUserPassword);
                IVersionedWorkspace pVWS = (IVersionedWorkspace)pWS;
                IVersion pVersion = null;

                if (VersionExists(pVWS, reconcileVersionName))
                {
                    pVersion = pVWS.FindVersion(reconcileVersionName);
                    IVersionEdit4 pVersionEdit = (IVersionEdit4)pVersion;
                    bool hasConflicts = pVersionEdit.Reconcile4( 
                        targetVersionName, 
                        acquireLock, 
                        abortIfConflicts, 
                        childWins, 
                        columnLevel);
                    if (hasConflicts)
                    {
                        //WriteToLogfile("Conflicts encountered"); 
                    }
                    pVersionEdit.Post(targetVersionName);
                    return true; 
                }
                else
                    return false;

                
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error encountered during reconcile and post"); 
            }
        }

        /// <summary>
        /// Just reconciles and posts the parent version to SDE.DEFAULT
        /// </summary>
        public void ReconcileAndPostParentVersion()
        {
            try
            {
                IWorkspace pWS = GetWorkspace(
                        _pConfig.EditUser + "." + _pConfig.ReconcileVersion + ENOSCommon.PARENT_VERISON_INDEX.ToString(),
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.PostUser,
                        _pConfig.PostUserPassword);

                //Reconcile and Post 
                if (pWS != null)
                {
                    ReconcileAndPostVersion(
                        (IVersion)pWS,
                        _pConfig.ReconcileAcquireLock,
                        _pConfig.ReconcileAbortIfConflicts,
                        _pConfig.ReconcileChildWins,
                        _pConfig.ReconcileColumnLevel);
                    Log(_pConfig.ReconcileVersion + ENOSCommon.PARENT_VERISON_INDEX.ToString() + " succeeded");
                }
                else
                {
                    throw new Exception("Unable to post parent version as the version was not found"); 
                }
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error encountered during reconcile and post");
            }
        }

        /// <summary>
        /// Reconciles and posts all versions created by child edits 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="pathToLogfileDirectory"></param>
        public void ReconcileAndPostAllVersions(Hashtable hshProcesses)
        {
            //IMMAutoUpdater autoupdater = null;
            //mmAutoUpdaterMode oldMode = Miner.Interop.mmAutoUpdaterMode.mmAUMArcMap;

            try
            {
                Log("Entering ReconcileAndPostAllVersions");

                //Turn off AUs 
                //Log("Turning off AUs");
                //object objAutoUpdater = null;
                //objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
                //autoupdater = (IMMAutoUpdater)objAutoUpdater;
                //oldMode = mmAutoUpdaterMode.mmAUMArcMap;
                //autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                //Reconcile and Post 
                Log("Calling ReconcileAndPost for all versions");

                for (int j = 0; j < _pConfig.ChildProcesses.Length; j++)
                {
                    //Get the child process 
                    ChildProcess pChildProc = _pConfig.ChildProcesses[j];

                    //Get the featureworkspace
                    Log("=============================================");
                    Log("Reconciling and posting version: " + _pConfig.ReconcileVersion + pChildProc.ProcessIndex.ToString());

                    Log("Getting the featureworkspace");
                    IWorkspace pWS = GetWorkspace(
                        _pConfig.EditUser + "." + _pConfig.ReconcileVersion + pChildProc.ProcessIndex.ToString(),
                        _pConfig.ServicePrefix + _pConfig.Service,
                        _pConfig.PostUser,
                        _pConfig.PostUserPassword); 

                    //Reconcile and Post 
                    if (pWS != null)
                    {
                        ReconcileAndPostVersion(
                            (IVersion)pWS,
                            _pConfig.ReconcileAcquireLock,
                            _pConfig.ReconcileAbortIfConflicts,
                            _pConfig.ReconcileChildWins,
                            _pConfig.ReconcileColumnLevel);
                        Log(_pConfig.ReconcileVersion + pChildProc.ProcessIndex.ToString() + " succeeded");
                    }
                }

                Log("Leaving ReconcileAndPostAllVersions");
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
            }
            finally
            {
                //Switch AUs back again
                //Log("Turning AUs back to original state");
                //if (autoupdater != null)
                //    autoupdater.AutoUpdaterMode = oldMode;
            }
        }

        /// <summary>
        /// Reconciles and posts a version to SDE.DEFAULT  
        /// </summary>
        /// <param name="version"></param>
        /// <param name="acquireLock"></param>
        /// <param name="abortIfConflicts"></param>
        /// <param name="childWins"></param>
        /// <param name="columnLevel"></param>
        public void ReconcileAndPostVersion(IVersion version, bool acquireLock, bool abortIfConflicts, bool childWins, bool columnLevel)
        {
            try
            {
                Log("Entering ReconcileAndPost");
                Log(" acquireLock: " + acquireLock.ToString());
                Log(" abortIfConflicts: " + abortIfConflicts.ToString());
                Log(" childWins: " + childWins.ToString());
                Log(" columnLevel: " + columnLevel.ToString());
                Log(" version: " + version.VersionInfo.VersionName);

                // setup the versioning variables to prepare for the reconcile and post
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)version;
                IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

                //reconcile against the default version    
                if (version.HasParent() != false)
                {
                    Log("starting edit session and edit operation");
                    workspaceEdit.StartEditing(false);
                    workspaceEdit.StartEditOperation();
                    Log("Performing Reconcile");
                    Boolean conflictsDetected = versionEdit.Reconcile4(
                        version.VersionInfo.Parent.VersionName,
                        acquireLock,
                        abortIfConflicts,
                        childWins,
                        columnLevel);
                    Log("Reconciled");

                    //no conflicts detected so post can be performed-
                    Log("conflictsDetected: " + conflictsDetected.ToString());
                    Log("versionEdit.CanPost(): " + versionEdit.CanPost().ToString());

                    if (conflictsDetected != true && versionEdit.CanPost())
                    {
                        Log("Performing Post");
                        versionEdit.Post(version.VersionInfo.Parent.VersionName);
                        Log("Posted");
                    }

                    // Commit the edit operation and edit session.
                    Log("stopping edit session and edit operation - saving edits");
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(true);
                    Log("stopped edit session and edit operation");
                }

                Log("Leaving ReconcileAndPostVersion");
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Simply resets the data, deleting all of the Generation 
        /// records and setting the GenCategory on the ServiceLocation 
        /// to 0 (no generation)
        /// </summary>
        public void ResetData()
        {
            IMMAutoUpdater autoupdater = null;
            mmAutoUpdaterMode oldMode = Miner.Interop.mmAutoUpdaterMode.mmAUMArcMap;

            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());

                //Get the SDE.DEFAULT workspace 
                _pWS = GetWorkspace(
                ENOSCommon.DEFAULT_VERSION,
                _pConfig.ServicePrefix + _pConfig.Service,
                _pConfig.PostUser,
                _pConfig.PostUserPassword);
                Log("Got the SDE.DEFAULT workspace");

                //Turn off AUs 
                object objAutoUpdater = null;
                objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
                autoupdater = (IMMAutoUpdater)objAutoUpdater;
                oldMode = autoupdater.AutoUpdaterMode;
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                Log("Switched AUs off");

                //**************************** 
                string sql = "UPDATE " + ENOSCommon.SERVICELOCATION_FC + " SET " +
                    "GENCATEGORY" + " = " + ((int)GenType.genTypeNone).ToString();
                Log("Running ExecuteSQL: " + sql);
                _pWS.ExecuteSQL(sql);
                Log("Done");
                sql = "DELETE" + " " + "FROM" + " " + ENOSCommon.GENERATION_TBL;
                Log("Running ExecuteSQL: " + sql); 
                _pWS.ExecuteSQL(sql);
                Log("Done"); 
                //****************************
                                                
                ////Start editing 
                //IWorkspaceEdit pWSE = (IWorkspaceEdit)_pWS;
                //IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS; 
                //pWSE.StartEditing(false);
                //pWSE.StartEditOperation();
                //Log("Started editing");
                
                ////IFeatureClass pServiceLocationFC =
                ////    pFWS.OpenFeatureClass(ENOSCommon.SERVICELOCATION_FC);
                //ITable pGenerationTable =
                //    pFWS.OpenTable(ENOSCommon.GENERATION_TBL);
                //Log("opened tables/featureclasses");

                ////int genCatFldIdx = -1;
                ////IQueryFilter pQF = new QueryFilterClass();
                ////pQF.SubFields = "gencategory";
                ////IFeatureCursor pFCursor = pServiceLocationFC.Update(pQF, false);
                ////IFeature pFeature = pFCursor.NextFeature();
                ////long counter = 0;

                ////while (pFeature != null)
                ////{
                ////    counter++;
                ////    if (genCatFldIdx == -1)
                ////        genCatFldIdx = pFeature.Fields.FindField("gencategory");

                ////    //Set the gentype to None  
                ////    pFeature.set_Value(genCatFldIdx, ((int)GenType.genTypeNone));
                ////    pFCursor.UpdateFeature(pFeature);

                ////    pFeature = pFCursor.NextFeature();
                ////}
                ////Marshal.FinalReleaseComObject(pFCursor);
                ////Log("updated the gencategory");



                



                //// Create a query filter defining which fields will be updated
                //// (the subfields) and how to constrain which rows are updated
                //// (the where clause).


                //// Create a feature buffer containing the values to be updated.
                ////IQueryFilter pQF = new QueryFilterClass();
                ////pQF.SubFields = "gencategory";
                ////IFeatureBuffer featureBuffer = pServiceLocationFC.CreateFeatureBuffer();
                ////featureBuffer.set_Value(pServiceLocationFC.Fields.FindField("gencategory"), 0);
                ////ITable table = (ITable)pServiceLocationFC;
                ////IRowBuffer rowBuffer = (IRowBuffer)featureBuffer;
                
                ////table.UpdateSearchedRows(pQF, rowBuffer);
                ////Marshal.FinalReleaseComObject(featureBuffer); 



                ////******************************

                               
                ////Delete all the generation records 
                ////ICursor pUpdateCursor = pGenerationTable.Update(null, false);
                ////IRow pRow = null;
                ////while ((pRow = pUpdateCursor.NextRow()) != null)
                ////{
                ////    pUpdateCursor.DeleteRow();
                ////}
                ////Marshal.FinalReleaseComObject(pUpdateCursor);
                ////Log("deleted generation records"); 

                //pGenerationTable.DeleteSearchedRows(null);

                ////Stop Editing 
                //pWSE.StopEditOperation();
                //pWSE.StopEditing(true);
                //Log("Stopped editing");

            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw ex;
            }
            finally
            {
                //Switch AUs back again
                Log("Turning AUs back to original state");
                if (autoupdater != null)
                    autoupdater.AutoUpdaterMode = oldMode;
            }
        }

        /// <summary>
        /// The main procedure for the child process. This procedure will 
        /// update the generation table for servicepoints assigned to this 
        /// child process (according to last digit of SPID) and create a 
        /// relationship to the servicepoint 
        /// </summary>
        /// <param name="childIdx"></param>
        /// <param name="childSPIDDigits"></param>
        public void ProcessChild(int childIdx, string childLastDigits)
        {

            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   childIdx: " + childIdx.ToString());
                Log("   childLastDigits " + childLastDigits);

                //check the child index 
                if ((childIdx < 0) || (childIdx > 9))
                    throw new Exception("Invalid child index"); 
 
                //Open the ADO.NET connection to the database 
                string connectionString = BuildOracleConnectString( 
                    _pConfig.EditUser, 
                    _pConfig.EditUserPassword, 
                    _pConfig.Service); 
                if (_pConn == null)
                    _pConn = new OracleConnection(connectionString);
                if (_pConn.State != ConnectionState.Open)
                    _pConn.Open();
                
                //Get all the generation data for this child process 
                Hashtable hshGeneration = LoadGeneration(childLastDigits);

                //Sets flag on SP to indicate if SP / Tx is found in the GIS 
                UpdateServicePoints(ref hshGeneration, childIdx);
 
                //Sets the localoffice for the servicepoint 
                UpdateSPLocalOffice(ref hshGeneration); 

                //Update the generation in EDGIS 
                UpdateGeneration(hshGeneration); 
                
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);                 
                throw new Exception("Error processing child with index: " + childIdx); 
            }
        }

        /// <summary>
        /// Releases resources like database connections, file handles etc 
        /// </summary>

        public void ReleaseResources()
        {
            try
            {
                if (_pConn != null)
                {
                    if (_pConn.State == ConnectionState.Open)
                        _pConn.Close();
                }

                if (_logfileWriter != null)
                {
                    _logfileWriter.Close();
                }

                if (_pWS != null)
                    _pWS = null; 
                
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
            }
        }



        //private IRow GetServicePointRow(string spid, ITable pServicePointTable)
        //{
        //    try
        //    {
        //        IQueryFilter pQF = new QueryFilterClass();
        //        pQF.WhereClause = "servicepointid" + " = " + "'" + spid.ToString() + "'";
        //        ICursor pCursor = pServicePointTable.Search(pQF, false);
        //        IRow pRow = pCursor.NextRow();
        //        Marshal.FinalReleaseComObject(pCursor);
        //        return pRow; 
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
        //            ex.Message); 
        //        throw new Exception("Error returing the servicepoint row"); 
        //    }
        //}

        //private IRow GetTransformerRow(IRow pServicePointRow, IRelationshipClass pTransformerServicePointRC)
        //{
        //    try
        //    {
        //        if (pServicePointRow == null)
        //            return null; 
        //        IRow pRow = null; 
        //        ISet pSet = pTransformerServicePointRC.GetObjectsRelatedToObject((IObject)pServicePointRow);
        //        object pObj = pSet.Next();
        //        if (pObj != null)
        //            pRow = (IRow)pObj;
        //        Marshal.FinalReleaseComObject(pSet); 
        //        return pRow;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
        //            ex.Message); 
        //        throw new Exception("Error returing the servicepoint row");
        //    }
        //}

        private IRow GetGenerationRow(IRow pServicePointRow, IRelationshipClass pServicePointGenerationRC)
        {
            try
            {
                IRow pRow = null;
                ISet pSet = pServicePointGenerationRC.GetObjectsRelatedToObject((IObject)pServicePointRow);
                object pObj = pSet.Next();
                if (pObj != null)
                    pRow = (IRow)pObj;
                Marshal.FinalReleaseComObject(pSet);
                return pRow;
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message); 
                throw new Exception("Error returing the servicepoint row");
            }
        }
        /// <summary>
        /// Return the workspace with the passed version 
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="instance"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public IWorkspace GetWorkspace(string versionName, string instance, string user, string pwd)
        {
            IWorkspace pWS = null;

            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());

                ENOSEncryption pEncrypt = new ENOSEncryption(); 
                IPropertySet pPropset = new PropertySetClass();
                pPropset.SetProperty("INSTANCE", instance);
                pPropset.SetProperty("AUTHENTICATION_MODE", "DBMS");
                pPropset.SetProperty("USER", user);
                pPropset.SetProperty("PASSWORD", pEncrypt.Decrypt(pwd));
                pPropset.SetProperty("VERSION", ENOSCommon.DEFAULT_VERSION);

                IWorkspaceFactory pWSF = new SdeWorkspaceFactoryClass();
                pWS = pWSF.Open(pPropset, 0);

                if (pWS is IVersionedWorkspace)
                {
                    IVersion pEditVersion = GetVersion(
                        (IVersionedWorkspace)pWS,
                        versionName);
                    pWS = (IWorkspace)pEditVersion;
                }
                else
                    throw new Exception("Error opening target version");

                //WriteToLogfile("Leaving GetWorkspace");
                return pWS;
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message); 
                throw new Exception(string.Format("Error opening SDE workspace.\r\n{0}", ex.Message));
            }
        }

        private IVersion GetVersion(IVersionedWorkspace pVWS, string versionName)
        {
            try
            {
                //WriteToLogfile("Entering GetVersion");
                //WriteToLogfile(" versionName: " + versionName);

                IVersion pVersion = null;

                try
                {
                    pVersion = pVWS.FindVersion(versionName);
                    //WriteToLogfile("Verion already exists");
                    return pVersion;
                }
                catch
                {
                    //WriteToLogfile("Verion needs to be created");
                    pVersion = null;
                }

                //Version does not exist so create it 
                if (pVersion == null)
                {
                    pVersion = pVWS.DefaultVersion.CreateVersion(versionName);
                    pVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                }

                //WriteToLogfile("Leaving GetVersion");
                return pVersion;
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message); 
                throw new Exception(string.Format("Error in GetVersion.\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Loads the generation data from ENOS_Staging and ENOS_Error 
        /// into memory read for updating EDGIS 
        /// </summary>
        /// <param name="childLastDigits"></param>
        /// <returns></returns>
        private Hashtable LoadGeneration(string childLastDigits)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   childLastDigits " + childLastDigits);

                //First get a list of all SPIDs that will need to be 
                //processed by this child, from ENOS_STAGE and ENOS_ERROR
                string[] lastDigitsArr = childLastDigits.Split(',');
                Hashtable hshLastDigits = new Hashtable();
                string digit = "";
                for (int i = 0; i < lastDigitsArr.Length; i++)
                {
                    digit = lastDigitsArr[i].ToString().Trim();
                    if (!hshLastDigits.ContainsKey(Convert.ToInt32(digit)))
                        hshLastDigits.Add(Convert.ToInt32(digit), 0);
                }

                string targetTable = "";
                Hashtable hshGeneration = new Hashtable();
                string spId = "";
                string fuelType = ""; 
                Int64 equipId = -1;
                Int64 inverterId = -1;
                Int32 lastDigit = -1;
                Int64 enosRefId = -1; 
                string manufacturer = ""; 
                string model = "";
                string refId = ""; 
                double rating = 0; 
                int quantity = -1;
                bool isValidEquipType = true; 
                string equipTypeValue = "";
                ServicePoint pServicePoint;
                string sql = ""; 
                EquipmentType equipType = EquipmentType.equipTypePV;
                SourceType sourceType = SourceType.sourceTypeENOS_Stage; 
                object fieldVal; 

                for (int i = 1; i < 3; i++)
                {
                    if (i == 1)
                    {
                        targetTable = ENOSCommon.ENOS_STAGE_TBL;
                        sourceType = SourceType.sourceTypeENOS_Stage;
                    }
                    else
                    {
                        targetTable = ENOSCommon.ENOS_ERROR_TBL;
                        sourceType = SourceType.sourceTypeENOS_Error;
                    }

                    OracleCommand pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    sql = "Select * From " + targetTable;
                    pCmd.CommandText = sql;                    

                    //Execute the reader 
                    OracleDataReader servicePointGenReader = pCmd.ExecuteReader();
                    if (servicePointGenReader.HasRows)
                    {
                        while (servicePointGenReader.Read())
                        {
                            //Check the servicepointid needs to be processed by 
                            //checking the last digit 
                            
                            //Get the ENOSRefId 
                            refId = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                    ENOSCommon.ET_ENOS_REF_ID_FLD)).ToString();

                            if (Int32.TryParse(refId.Substring((refId.Length - 1), 1), out lastDigit))
                            {                               

                                //lastDigit = Convert.ToInt32(spId.Substring(spId.Length - 1));
                                if (hshLastDigits.ContainsKey(lastDigit))
                                {
                                    //Get teh enosrefid as a long 
                                    enosRefId = Convert.ToInt64(refId);

                                    //Get the spid 
                                    spId = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                        ENOSCommon.ET_SERVICE_POINT_ID_FLD)).ToString();

                                    //Check that the ServicePoint object exists 
                                    if (hshGeneration.ContainsKey(spId))
                                        pServicePoint = (ServicePoint)hshGeneration[spId];
                                    else
                                        pServicePoint = new ServicePoint(spId, enosRefId);

                                    equipTypeValue = servicePointGenReader.GetString(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_EQUIPMENT_TYPE_FLD)).Trim().ToLower();
                                    fieldVal = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_EQUIPMENT_ID_FLD));
                                    equipId = Convert.ToInt64(fieldVal);
                                    fieldVal = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_INVERTERID_FLD));
                                    if (fieldVal != DBNull.Value)
                                        inverterId = Convert.ToInt64(fieldVal);
                                    else
                                        inverterId = 0;

                                    fuelType = ""; 
                                    fieldVal = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_POWER_SOURCE_FLD));
                                    if (fieldVal != DBNull.Value)
                                        fuelType = fieldVal.ToString();
                                    manufacturer = "";
                                    fieldVal = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_MANUFACTURER_FLD));
                                    if (fieldVal != DBNull.Value)
                                        manufacturer = fieldVal.ToString();
                                    model = ""; 
                                    fieldVal = servicePointGenReader.GetValue(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_MODEL_FLD));                                     
                                    if (fieldVal != DBNull.Value)
                                        model = fieldVal.ToString();
                                    rating = servicePointGenReader.GetDouble(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_RATING_FLD));
                                    quantity = servicePointGenReader.GetInt32(servicePointGenReader.GetOrdinal(
                                            ENOSCommon.ET_QUANTITY_FLD));

                                    isValidEquipType = true; 
                                    switch (equipTypeValue)
                                    {
                                        case "photovoltaic":
                                            equipType = EquipmentType.equipTypePV;
                                            break;
                                        case "photovoltaic with inverter":
                                            equipType = EquipmentType.equipTypePVWithInverter;
                                            break;
                                        case "wind turbine":
                                            equipType = EquipmentType.equipTypeWindTurbine;
                                            break;
                                        case "wind turbine with inverter":
                                            equipType = EquipmentType.equipTypeWindTurbineWithInverter;
                                            break;
                                        case "inverter":
                                            equipType = EquipmentType.equipTypeInverter;
                                            break;
                                        default:
                                            isValidEquipType = false; 
                                            System.Diagnostics.Debug.Print("Unknown equipment type");
                                            Log("Error unknown equipment type: " + equipTypeValue);
                                            break;
                                    }


                                    //Create the GenEquipmentItem and add it to the ServicePoint  
                                    if (isValidEquipType)
                                    {                                        
                                        GenEquipmentItem pEquipItem = new GenEquipmentItem(
                                            equipId, inverterId, equipType, sourceType,
                                            manufacturer, fuelType, model, rating, quantity);
                                        if (!pServicePoint.HasEquipmentItem(equipId))
                                            pServicePoint.EquipmentList.Add(pEquipItem);
                                        else
                                            Log("Equip item repeated: " + equipId); 
                                    }

                                    //Update the hashtable 
                                    if (!hshGeneration.ContainsKey(spId))
                                        hshGeneration.Add(spId, pServicePoint);
                                    else
                                        hshGeneration[spId] = pServicePoint;
                                }
                            }
                            else
                            {
                                Log("Error processing record from: " + targetTable +
                                    " enos_ref_id does not contain all numeric format: " + spId);  
                            }
                        }
                    }
                }
                
                return hshGeneration; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message); 
                throw new Exception("Error returning generation data for servicepoints"); 
            }
            finally
            {
                //release ADO.NET connection to DB 
            }
        }

        /// <summary>
        /// This routine updates the ServicePoint objects to set the 
        /// HasSPInGIS and TxExistsInGIS flags where matching servicepoints 
        /// and transformers are found in GIS 
        /// </summary>
        public void UpdateServicePoints(ref Hashtable hshGeneration, int processIdx)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   hshGeneration.Count: " + hshGeneration.Count.ToString());

                //Get the sde connection to the target database 
                Log("Setting up arcobjects connections to geodatabase");
                CreateEditVersion(processIdx);
                Log("Version is: " + ((IVersion)_pWS).VersionInfo.VersionName);

                //Open required tables 
                Log("Opening required tables"); 
                IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;                 
                ITable pServicePointTable =
                    pFWS.OpenTable(ENOSCommon.SERVICEPOINT_TBL);
                ITable pTransformerTable =
                    pFWS.OpenTable(ENOSCommon.TRANSFORMER_FC);
                
                //For every ServicePoint object set the properties: 
                //ServicePointGUID 
                //TransformerGUID 
                Log("Looking for servicepoints in GIS via SPID");
                int spidMatchCount = 0;
                int spidFldIdx = -1;
                int txIdFldIdx = -1;
                int globalIdFldIdx = -1; 
                string spid = ""; 
                string tranformerGUID = "";
                Hashtable hshTransformerGUIDs = new Hashtable();
                ICursor pCursor = null;
                IRow pRow = null;
                string[] commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(hshGeneration, 1000, true);
                IQueryFilter pQF = new QueryFilterClass();
                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    pQF.SubFields = "servicepointid, transformerguid, globalid";
                    pQF.WhereClause = "servicepointid" + " IN(" + commaSeparatedKeys[i] + ")";
                    pCursor = pServicePointTable.Search(pQF, false);
                    pRow = pCursor.NextRow();

                    while (pRow != null)
                    {
                        spidMatchCount++; 
                        if (spidFldIdx == -1)
                            spidFldIdx = pRow.Fields.FindField("servicepointid");
                        if (txIdFldIdx == -1)
                            txIdFldIdx = pRow.Fields.FindField("transformerguid");
                        if (globalIdFldIdx == -1)
                            globalIdFldIdx = pRow.Fields.FindField("globalid");


                        spid = pRow.get_Value(spidFldIdx).ToString();
                        tranformerGUID = string.Empty;

                        if (pRow.get_Value(txIdFldIdx) != DBNull.Value)
                        {
                            tranformerGUID = pRow.get_Value(txIdFldIdx).ToString();
                            if (!hshTransformerGUIDs.ContainsKey(tranformerGUID))
                                hshTransformerGUIDs.Add(tranformerGUID, false);
                        }
                        else
                            Debug.Print("transformerguid is null");

                        ServicePoint pSP = (ServicePoint)hshGeneration[spid];
                        pSP.ServicePointGUID = pRow.get_Value(globalIdFldIdx).ToString();
                        pSP.TransformerGUID = tranformerGUID; 
                        
                        //Update the hashtable of SPs 
                        hshGeneration[spid] = pSP;
                        pRow = pCursor.NextRow();
                    }
                    Marshal.FinalReleaseComObject(pCursor);
                }
                Log("SPID match count: " + spidMatchCount.ToString());
                              
                //Do a secondary matching effort on the uniqueSPID 
                Log("Looking for servicepoints in GIS via UniqueSPID");
                int uniquespidFldIdx = -1;
                int uniqueSPIDMatchCount = 0; 
                txIdFldIdx = -1;
                globalIdFldIdx = -1; 
                Hashtable hshUnmatchedServicePoints = new Hashtable();
                foreach (string servicePointId in hshGeneration.Keys)
                {
                    ServicePoint pSP = (ServicePoint)hshGeneration[servicePointId];
                    if (pSP.ServicePointGUID == string.Empty)
                        hshUnmatchedServicePoints.Add(servicePointId, 0); 
                }
                commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(hshUnmatchedServicePoints, 1000, true);
                pQF = new QueryFilterClass();
                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    //could change this to use an IN() clause and break into 
                    //batches of 1000 
                    pQF.SubFields = "uniquespid, transformerguid, globalid";
                    pQF.WhereClause = "uniquespid" + " IN(" + commaSeparatedKeys[i] + ")";
                    pCursor = pServicePointTable.Search(pQF, false);
                    pRow = pCursor.NextRow();

                    while (pRow != null)
                    {
                        uniqueSPIDMatchCount++; 
                        if (uniquespidFldIdx == -1)
                            uniquespidFldIdx = pRow.Fields.FindField("uniquespid");
                        if (txIdFldIdx == -1)
                            txIdFldIdx = pRow.Fields.FindField("transformerguid");
                        if (globalIdFldIdx == -1)
                            globalIdFldIdx = pRow.Fields.FindField("globalid");

                        spid = pRow.get_Value(uniquespidFldIdx).ToString();
                        tranformerGUID = string.Empty;

                        if (pRow.get_Value(txIdFldIdx) != DBNull.Value)
                        {
                            tranformerGUID = pRow.get_Value(txIdFldIdx).ToString();
                            if (!hshTransformerGUIDs.ContainsKey(tranformerGUID))
                                hshTransformerGUIDs.Add(tranformerGUID, false);
                        }
                        else
                        {
                            Debug.Print("transformerguid is null");
                        }

                        ServicePoint pSP = (ServicePoint)hshGeneration[spid];
                        pSP.ServicePointGUID = pRow.get_Value(globalIdFldIdx).ToString();
                        pSP.TransformerGUID = tranformerGUID; 

                        //Update the hashtable of SPs 
                        hshGeneration[spid] = pSP;
                        pRow = pCursor.NextRow();
                    }
                    Marshal.FinalReleaseComObject(pCursor);
                }
                Log("UniqueSPID match count: " + uniqueSPIDMatchCount.ToString());
                                                
                //Verify that all the Transformers referenced actually exist in 
                //Transformer featureclass 
                Log("Looking for transformers for ServicePoints in GIS");
                commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(hshTransformerGUIDs, 1000, true);
                string globalId = string.Empty; 
                pQF = new QueryFilterClass();
                globalIdFldIdx = -1; 

                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    //could change this to use an IN() clause and break into 
                    //batches of 1000 
                    pQF.SubFields = "globalid";
                    pQF.WhereClause = "globalid" + " IN(" + commaSeparatedKeys[i] + ")";
                    pCursor = pTransformerTable.Search(pQF, false);
                    pRow = pCursor.NextRow();

                    while (pRow != null)
                    {
                        if (globalIdFldIdx == -1)
                            globalIdFldIdx = pRow.Fields.FindField("globalid");

                        globalId = pRow.get_Value(globalIdFldIdx).ToString();
                        hshTransformerGUIDs[globalId] = true;
                        pRow = pCursor.NextRow();
                    }
                    Marshal.FinalReleaseComObject(pCursor);
                }

                //Set the TransformerGUID property to an empty string 
                //if the transformer was not found in Transformer FC 
                Hashtable hshTemp = new Hashtable(); 
                foreach (string servicePointId in hshGeneration.Keys)
                {
                    hshTemp.Add(servicePointId, 0);
                }
                foreach (string servicePointId in hshTemp.Keys)
                {
                    //Get the transformerguid
                    ServicePoint pSP = (ServicePoint)hshGeneration[servicePointId];
                    if (pSP.TransformerGUID != string.Empty)
                    {
                        if (Convert.ToBoolean(hshTransformerGUIDs[pSP.TransformerGUID]) == false)
                        {
                            pSP.TransformerGUID = string.Empty;
                            hshGeneration[servicePointId] = pSP; 
                        }
                    }
                }

                Log("Leaving " + MethodBase.GetCurrentMethod());
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning generation data for servicepoints");
            } 
        }

        /// <summary>
        /// This routine updates the ServicePoint objects to set the 
        /// LocalOffice code
        /// </summary>
        public void UpdateSPLocalOffice(ref Hashtable hshGeneration)
        {
            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   hshGeneration.Count: " + hshGeneration.Count.ToString());
               
                //Query the METER table for LocalOffice for all 
                //of the servicepoints 
                string sql = string.Empty;
                string spid = string.Empty;
                string localOffice = string.Empty;
                object fieldVal = DBNull.Value;
                OracleCommand pCmd = _pConn.CreateCommand(); 
                string[] commaSeparatedKeys = GetArrayOfCommaSeparatedKeys(hshGeneration, 1000, true);
                IQueryFilter pQF = new QueryFilterClass();
                for (int i = 0; i < commaSeparatedKeys.Length; i++)
                {
                    sql = "SELECT" + " " +
                    "service_point_id" + ", " +
                    "local_office" + " " +
                    "FROM" + " " +
                    ENOSCommon.CEDSA_METER_TBL + " " +
                    "WHERE" + " " +
                    "service_point_id" + " IN(" + commaSeparatedKeys[i] + ")";
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sql;
                    OracleDataReader reader = pCmd.ExecuteReader();
                    while (reader.Read())
                    {

                        spid = string.Empty;
                        localOffice = string.Empty;
                        fieldVal = DBNull.Value;

                        fieldVal = reader.GetValue(reader.GetOrdinal("service_point_id"));
                        if (fieldVal != DBNull.Value)
                            spid = fieldVal.ToString();
                        fieldVal = reader.GetValue(reader.GetOrdinal("local_office"));
                        if (fieldVal != DBNull.Value)
                            localOffice = fieldVal.ToString();

                        if (spid != string.Empty)
                        {
                            ServicePoint pSP = (ServicePoint)hshGeneration[spid];
                            pSP.LocalOffice = localOffice;
                            hshGeneration[spid] = pSP;
                        }
                    }
                    if (reader != null)
                        reader.Close();
                }
                Log("Leaving " + MethodBase.GetCurrentMethod());
            }
            catch (Exception ex)
            {
                //No need to throw an error as not having the LocalOffice is not 
                //a show stopper 
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
            }
        }

        /// <summary>
        /// Updates the EDGIS Generation table with new Generation data 
        /// from ENOS 
        /// </summary>
        public void UpdateGeneration(Hashtable hshGeneration)
        {
            IWorkspaceEdit2 pWSE = null;

            try
            {
                Log("Entering " + MethodBase.GetCurrentMethod());
                Log("   hshGeneration.Count: " + hshGeneration.Count.ToString());

                IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;
                pWSE = (IWorkspaceEdit2)_pWS;
                Hashtable hshInverters = new Hashtable();
                Hashtable hshGenerationFields = new Hashtable();
                Hashtable hshENOSErrorList = GetENOSErrorList();

                //Open required tables 
                Log("Opening required tables");
                ITable pServicePointTable =
                    pFWS.OpenTable(ENOSCommon.SERVICEPOINT_TBL);
                ITable pGenerationTable =
                    pFWS.OpenTable(ENOSCommon.GENERATION_TBL);

                //Open required relationship classes 
                Log("Opening required relationship classes");
                bool hasInverter = false;
                bool addEquipRecord = false;

                //cache field indexes for performance 
                hshGenerationFields.Add(ENOSCommon.SERVICEPOINTID_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.SERVICEPOINTID_FLD));
                hshGenerationFields.Add(ENOSCommon.ENOS_REF_ID_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.ENOS_REF_ID_FLD));
                hshGenerationFields.Add(ENOSCommon.GENTYPE_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.GENTYPE_FLD));
                hshGenerationFields.Add(ENOSCommon.NOTES_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.NOTES_FLD));
                hshGenerationFields.Add(ENOSCommon.ENOS_EQP_ID_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.ENOS_EQP_ID_FLD));
                hshGenerationFields.Add(ENOSCommon.INV_INVERTERID_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.INV_INVERTERID_FLD));
                hshGenerationFields.Add(ENOSCommon.MANF_CD_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.MANF_CD_FLD));
                hshGenerationFields.Add(ENOSCommon.MODEL_CD_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.MODEL_CD_FLD));
                hshGenerationFields.Add(ENOSCommon.DC_RATING_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.DC_RATING_FLD));
                hshGenerationFields.Add(ENOSCommon.QUANTITY_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.QUANTITY_FLD));
                hshGenerationFields.Add(ENOSCommon.SERVICEPOINTGUID_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.SERVICEPOINTGUID_FLD));
                hshGenerationFields.Add(ENOSCommon.POWER_SOURCE_CD_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.POWER_SOURCE_CD_FLD));
                hshGenerationFields.Add(ENOSCommon.STATUS_CD_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.STATUS_CD_FLD));
                hshGenerationFields.Add(ENOSCommon.KW_OUT_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.KW_OUT_FLD));
                hshGenerationFields.Add(ENOSCommon.NP_KVA_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.NP_KVA_FLD));
                hshGenerationFields.Add(ENOSCommon.LASTUSER_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.LASTUSER_FLD));
                hshGenerationFields.Add(ENOSCommon.DATECREATED_FLD, 
                    pGenerationTable.Fields.FindField(ENOSCommon.DATECREATED_FLD));

                //Loop through servicepoints executing business logic 
                int spCounter = 0;
                int newRowCount = 0;
                int totalInsertCount = 0; 
                int spTotal = hshGeneration.Count;
                bool isInverter = false; 

                //Start editing 
                pWSE.StartEditing(false);
                pWSE.StartEditOperation();

                //Get a list of all equipids so as to be able to make sure none are 
                //getting duplicated 
                Hashtable hshEquipIds = GetEquipmentIdList(pGenerationTable);
                ICursor pInsertCursor = pGenerationTable.Insert(true);
                IRowBuffer pInsertRB = pGenerationTable.CreateRowBuffer();

                foreach (string spid in hshGeneration.Keys)
                {
                    ServicePoint pServicePoint = (ServicePoint)hshGeneration[spid];

                    //1. Check if the service point exists in the Geodatabase
                    //If no - Add records to the ENOS_ERROR table and go to the next SP
                    if (pServicePoint.ServicePointGUID == "")
                    {
                        //All equipment for this servicepoint sourced from ENOS_STAGE is in error
                        AddENOSError(
                            ref hshENOSErrorList, 
                            pServicePoint.EquipmentList,
                            pServicePoint.LocalOffice, 
                            "Missing ServicePoint in GIS for ServicePoint: " + spid);
                        spCounter++;
                        continue;
                    }
                    Log("ServicePoint: " + spid + " found");

                    //2. Look for a connected transformer 
                    if (pServicePoint.TransformerGUID == string.Empty)
                    {
                        //All equipment for this servicepoint sourced from ENOS_STAGE is in error
                        Log("Missing Transformer in GIS for ServicePoint: " + spid);
                        AddENOSError( 
                            ref hshENOSErrorList, 
                            pServicePoint.EquipmentList,
                            pServicePoint.LocalOffice, 
                            "No connected transformer found in GIS");
                        spCounter++;
                        continue;
                    }
                    Log("Related transformer found");

                    //Get a list of all the inverters for the SP
                    hshInverters.Clear();
                    foreach (GenEquipmentItem pEquipItem in pServicePoint.EquipmentList)
                    {
                        if (pEquipItem.EquipmentType == EquipmentType.equipTypeInverter)
                        {
                            if (!hshInverters.ContainsKey(pEquipItem.EquipmentId))
                                hshInverters.Add(pEquipItem.EquipmentId, 0);
                        }
                    }

                    //Add the equipment records 
                    foreach (GenEquipmentItem pEquipItem in pServicePoint.EquipmentList)
                    {
                        //Check for the inverter - different rules for different equip types 
                        hasInverter = true;
                        if ((pEquipItem.EquipmentType != EquipmentType.equipTypeInverter) &&
                            (pEquipItem.EquipmentType != EquipmentType.equipTypePVWithInverter) &&
                            (pEquipItem.EquipmentType != EquipmentType.equipTypeWindTurbineWithInverter) &&
                            (!hshInverters.ContainsKey(pEquipItem.InverterId)))
                        {
                            hasInverter = false;
                        }

                        if ((pEquipItem.EquipmentType == EquipmentType.equipTypePVWithInverter) ||
                            (pEquipItem.EquipmentType == EquipmentType.equipTypeWindTurbineWithInverter))
                            Log("With Inverter Scenario - EquipId: " + pEquipItem.EquipmentId.ToString());

                        if (hasInverter)
                        {
                            //We have an inverter (or it is an inverter) so add the equipment record 

                            //First check it does not already exist 
                            if (hshEquipIds.ContainsKey(pEquipItem.EquipmentId))
                                Log("Equipment Item with ID: " + pEquipItem.EquipmentId.ToString() +
                                    " already exists");
                            else
                            {
                                //Execute logic twice because if it a PVWithInverter or 
                                //WindTurbineWithInverter we need to create an inverter as well
                                for (int i = 0; i < 2; i++)
                                {
                                    //Do this twice because  
                                    addEquipRecord = false;
                                    if (i == 0)
                                    {
                                        addEquipRecord = true;
                                    }
                                    else if (i == 1)
                                    {
                                        if ((pEquipItem.EquipmentType == EquipmentType.equipTypePVWithInverter) ||
                                            (pEquipItem.EquipmentType == EquipmentType.equipTypeWindTurbineWithInverter))
                                        {
                                            addEquipRecord = true;
                                        }
                                    }

                                    if (addEquipRecord)
                                    {
                                        //Determine if we are processing an inverter 
                                        isInverter = false;
                                        if (i == 0)
                                        {
                                            if (pEquipItem.EquipmentType == EquipmentType.equipTypeInverter)
                                                isInverter = true;
                                        }                                            
                                        else if (i == 1) 
                                            isInverter = true;

                                        //Add equip records 
                                        //pGenerationRow = pGenerationTable.CreateRow();                                        
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.SERVICEPOINTID_FLD], spid);

                                        if (i == 0)
                                            pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.GENTYPE_FLD], (int)pEquipItem.EquipmentType);
                                        else if (i == 1)
                                            pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.GENTYPE_FLD], (int)EquipmentType.equipTypeInverter);

                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.NOTES_FLD], pServicePoint.ENOSRefId);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.ENOS_REF_ID_FLD], pServicePoint.ENOSRefId);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.ENOS_EQP_ID_FLD], pEquipItem.EquipmentId);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.INV_INVERTERID_FLD], pEquipItem.InverterId);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.MANF_CD_FLD], HandleApostrophe(pEquipItem.Manufacturer));
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.MODEL_CD_FLD], HandleApostrophe(pEquipItem.Model));
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.QUANTITY_FLD], pEquipItem.Quantity);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.DC_RATING_FLD], pEquipItem.CalculateDCRating());
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.SERVICEPOINTGUID_FLD], pServicePoint.ServicePointGUID);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.POWER_SOURCE_CD_FLD], pEquipItem.FuelType);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.STATUS_CD_FLD], ENOSCommon.STATUS_CD_VALUE);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.LASTUSER_FLD], _pConfig.EditUser);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.DATECREATED_FLD], DateTime.Now);
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.KW_OUT_FLD], pEquipItem.CalculatekWOut(isInverter));
                                        pInsertRB.set_Value((int)hshGenerationFields[ENOSCommon.NP_KVA_FLD], pEquipItem.CalculateNP_KVA(isInverter));

                                        try
                                        {
                                            pInsertCursor.InsertRow(pInsertRB);
                                            newRowCount++;
                                            totalInsertCount++; 
                                        }
                                        catch
                                        {
                                            Log("Failed to insert rowbuffer on spid: " + spid);
                                            throw new Exception("Unable to insert row");
                                        }

                                        //Flush every record 
                                        if (newRowCount == 5000)
                                        {
                                            Log("Written: " + newRowCount.ToString() + " equipment records");
                                            try
                                            { pInsertCursor.Flush(); }
                                            catch { throw new Exception("Unable to flush row buffer"); }
                                            newRowCount = 0;
                                        }

                                        //Update the equip id hashtable 
                                        if (!hshEquipIds.ContainsKey(pEquipItem.EquipmentId))
                                            hshEquipIds.Add(pEquipItem.EquipmentId, 0);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Log("Equipment with Id: " + pEquipItem.EquipmentId +
                                " has not got an inverter");
                            AddENOSError(
                                ref hshENOSErrorList, 
                                pEquipItem.EquipmentId,
                                pServicePoint.LocalOffice,
                                "Equipment item does not have an inverter");
                        }
                    }

                    spCounter++;
                    Log("Processed " + spCounter.ToString() + " servicepoints of: " + spTotal.ToString());
                }

                //Flush if necessary 
                if (newRowCount != 0)
                    pInsertCursor.Flush();
                
                //Stop editing and save edits 
                pWSE.StopEditOperation();
                pWSE.StopEditing(true);
                Log("StopEditing succeeded");
                Log("Total generation equip records added: " + totalInsertCount.ToString());
                Log("Leaving " + MethodBase.GetCurrentMethod());

                //Release resources 
                Marshal.FinalReleaseComObject(pInsertCursor);
                Marshal.FinalReleaseComObject(pInsertRB);
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);

                if (pWSE.IsInEditOperation)
                    pWSE.AbortEditOperation();
                if (pWSE.IsBeingEdited())
                    pWSE.StopEditing(false);

                Log("Edits aborted");
                throw new Exception("Error updating Generation");
            }
        }
        /// <summary>
        /// Oracle queries do not like Apostrophes in 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        private string HandleApostrophe(string inputString)
        {
            try
            {
                string outputString = inputString.Clone().ToString();
                if (outputString.IndexOf("'") != -1)
                    outputString = outputString.Replace("'", "''");
                return outputString; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error converting string to Oracle friendly format"); 
            }
        }

        /// <summary>
        /// Returns existing list of all Generation equipment so 
        /// as to avoid duplication of equipment records 
        /// </summary>
        /// <param name="pGenerationTable"></param>
        /// <returns></returns>
        private Hashtable GetEquipmentIdList(ITable pGenerationTable)
        {
            try
            {
                Hashtable hshEquipIds = new Hashtable();
                IQueryFilter pQF = null;
                ICursor pCursor = pGenerationTable.Search(pQF, false);
                IRow pRow = pCursor.NextRow();
                Int64 equipId = -1;
                int equipIdFieldIdx =  pGenerationTable.Fields.FindField("ENOS_EQP_ID"); 
                if (equipIdFieldIdx == -1) 
                    throw new Exception("Error ENOS_EQP_ID field not found!"); 

                while (pRow != null)
                {
                    equipId = Convert.ToInt64(pRow.get_Value(equipIdFieldIdx));
                    if (!hshEquipIds.ContainsKey(equipId))
                        hshEquipIds.Add(equipId, 0); 

                    pRow = pCursor.NextRow(); 
                }
                Marshal.FinalReleaseComObject(pCursor); 
                return hshEquipIds; 
            }
            catch (Exception ex)
            {
                Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error loading equipment list"); 
            }
        }
    }
}
