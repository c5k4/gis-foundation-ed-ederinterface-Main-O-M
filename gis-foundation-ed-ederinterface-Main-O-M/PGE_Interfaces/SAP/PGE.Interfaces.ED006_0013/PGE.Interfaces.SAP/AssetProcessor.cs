using System;
using System.Collections.Generic;
using System.Linq;
using PGE.Interfaces.SAP.Data;
using System.IO;
using PGE.Interfaces.Integration.Framework;
using System.Reflection;
using Newtonsoft.Json;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// Class used to write SAP data to a database
    /// </summary>
    public class AssetProcessor : IDisposable
    {
        //private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private static Logger _logger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger Log = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.log4net.config");  //ED06_ChangeDetection.log4net.config
        private StreamWriter _streamWriter;
        private DataHelper _dbHelper;
        //instance of GISSAPIntegrator addede for ED06-374
        private GISSAPIntegrator _gisSAPIntegrator;
        private bool _disposed = false;
        private string actionType = string.Empty;
        private string processedRecordID = string.Empty;
        /// <summary>
        /// Default constructor. Initializes a DBInteraction
        /// </summary>
        public AssetProcessor()
        {
            _dbHelper = new DataHelper();


        }

        /// <summary>
        /// Process a SAP row send from data collector and persist in Database.
        /// </summary>
        /// <param name="assetIdAndData">SAP Data row</param>
        public void ProcessDeprecated(KeyValuePair<string, IRowData> assetIdAndData)
        {
            // Query asset id in database                
            var existingAssetRows = _dbHelper.GetRecordsByAssetID(assetIdAndData.Key);
            GISSAP_ASSETSYNCH existingAssetRow;

            ActionType existingActionType = ActionType.NotApplicable;

            IRowData newAssetRow = assetIdAndData.Value;

            ActionType newActionType = ActionType.NotApplicable;
            SAPRowData newSAPRowData = null;

            // If no entry present, insert the sap row in staging table
            if (existingAssetRows.Count == 0)
            {
                InsertRow(newAssetRow);
            }
            else // delete or update
            {
                existingAssetRow = existingAssetRows.First();
                existingActionType = (ActionType)existingAssetRow.ACTIONTYPE[0];

                if (newAssetRow is SAPRowData)
                {
                    newSAPRowData = (SAPRowData)newAssetRow;
                    newActionType = newSAPRowData.ActionType;
                }


                // Database row says insert and sap says update; 
                // end result: update sap attributes in database
                if (existingActionType == ActionType.Insert && newActionType == ActionType.Update)
                {
                    SetRowAttribute(existingAssetRow, newSAPRowData);
                    _dbHelper.UpdateData(existingAssetRow);
                }

                // Database row says insert and sap says delete; 
                // end result: delete row from database
                if (existingActionType == ActionType.Insert && newActionType == ActionType.Delete)
                {
                    _dbHelper.DeleteData(existingAssetRow);
                }

                // Database row says update and sap says delete; 
                // end result: delete row from database
                if (existingActionType == ActionType.Update && newActionType == ActionType.Delete)
                {
                    _dbHelper.DeleteData(existingAssetRow);
                }

                // Database row says update and sap says update; 
                // end result: update sap attributes in database
                if (existingActionType == ActionType.Update && newActionType == ActionType.Update)
                {
                    SetRowAttribute(existingAssetRow, newSAPRowData);
                    _dbHelper.UpdateData(existingAssetRow);
                }

                // All valid cases are handled; if control reaches here then error must be logged
            }
        }


        /// <summary>
        /// Processes data retrieving from Temp table for a perticular session and persists in Database.
        /// </summary>
        /// <param name="sessionName">Session name</param>
        /// <returns>Boolean value</returns>
        public bool GenerateScript(string sessionName)
        {
            // Query asset id in database                
            var assetRows = _dbHelper.GetTempData(sessionName);
            return GenerateScript(assetRows, sessionName);
        }

        /// <summary>
        /// Processes GISSAP_ASSETSYNCH data and persists in Database.
        /// </summary>
        /// <param name="assetRows">GISSAP_ASSETSYNCH data</param>
        /// <returns>Boolean value</returns>
        public bool GenerateScript(IList<GISSAP_ASSETSYNCH> assetRows, string sessionName)
        {
            bool success = true;
            try
            {
                foreach (GISSAP_ASSETSYNCH assetRow in assetRows)
                {
                    // Query asset id in database  
                    var existingAssetRows = _dbHelper.GetRecordsByAssetID(assetRow.ASSETID);
                    GISSAP_ASSETSYNCH existingAssetRow = null;

                    // If no entry present, insert the sap row in staging table
                    if (existingAssetRows.Count > 0)
                    {
                        existingAssetRow = existingAssetRows.First();
                    }

                    string sql = Script(existingAssetRow, assetRow);

                    if (_streamWriter == null)
                    {
                        string scriptsFile = sessionName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".sql";
                        scriptsFile = Path.Combine(_dbHelper.ScriptsLocation, scriptsFile);
                        _streamWriter = new StreamWriter(scriptsFile, true);
                        _streamWriter.AutoFlush = true;
                    }

                    sql.Trim();
                    if (string.IsNullOrEmpty(sql) == false)
                    {
                        _streamWriter.WriteLine(sql);
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
            return success;
        }

        private string Script(GISSAP_ASSETSYNCH existingAssetRow, GISSAP_ASSETSYNCH newAssetRow)
        {
            string sql = string.Empty;

            bool existing = true;
            try
            {
                ActionType newActionType = (ActionType)(char)newAssetRow.ACTIONTYPE[0];

                if (existingAssetRow == null)
                {
                    existing = false;

                    existingAssetRow = new GISSAP_ASSETSYNCH();
                    existingAssetRow.ASSETID = newAssetRow.ASSETID;
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                }

                ActionType existingActionType = (ActionType)(char)existingAssetRow.ACTIONTYPE[0];
                SetRowAttribute(existingAssetRow, newAssetRow);

                string assetID = existingAssetRow.ASSETID;
                string actionTypeString = existingAssetRow.ACTIONTYPE;
                int sapType = existingAssetRow.TYPE;
                DateTime date = existingAssetRow.DATEPROCESSED;
                string dateString = "to_date('" + date.ToString("MM/dd/yyyy HH:mm:ss") + "','mm/dd/yyyy hh24:mi:ss')";
                string sapAttributes = existingAssetRow.SAPATTRIBUTES;

                if (existing == false)
                {
                    sql = "INSERT into " + _dbHelper.GISSAPAssetSyncTableName +
                        "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES)  " +
                        "VALUES ('" + assetID + "','" + actionTypeString + "'," + sapType + "," + dateString + ",'" + sapAttributes + "'";
                }
                else if (existingActionType == ActionType.Insert && newActionType == ActionType.Update
                    || existingActionType == ActionType.Update && newActionType == ActionType.Update)
                {
                    sql = "Update " + _dbHelper.GISSAPAssetSyncTableName +
                        " Set " +
                        " ActionType='" + actionTypeString + "', Type=" + sapType + ", DateProcessed=" + dateString + ", SapAttributes='" + sapAttributes + "'" +
                        " Where AssetID='" + assetID + "'";
                }
                else if (existingActionType == ActionType.Insert && newActionType == ActionType.Delete
                    || existingActionType == ActionType.Update && newActionType == ActionType.Delete)
                {
                    sql = "Delete from " + _dbHelper.GISSAPAssetSyncTableName +
                        " Where AssetID='" + assetID + "'";
                }
                else if (existingActionType == ActionType.Delete && newActionType == ActionType.Delete)
                {
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
            return sql;
        }

        #region Obsolete Nethods
        /// <summary>
        /// Generate scripts as backup measure when asset rows are not properly processed.
        /// </summary>
        /// <param name="assetIdAndData">Asset ID and Row Data</param>
        /// <param name="versionName">Sde Version where the asset is edited</param>
        [ObsoleteAttribute("This method is obsolete.", false)]
        public void GenerateScript(KeyValuePair<string, IRowData> assetIdAndData, string versionName)
        {
            // Query asset id in database
            var existingAssetRows = _dbHelper.GetRecordsByAssetID(assetIdAndData.Key);
            GISSAP_ASSETSYNCH existingAssetRow = null;
            if (existingAssetRows.Count > 0)
            {
                existingAssetRow = existingAssetRows.First();
            }

            SAPRowData newAssetRow = (SAPRowData)assetIdAndData.Value;
            string sql = Script(existingAssetRow, newAssetRow);

            if (_streamWriter == null)
            {
                string scriptsFile = versionName + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".sql";
                scriptsFile = Path.Combine(_dbHelper.ScriptsLocation, scriptsFile);
                _streamWriter = new StreamWriter(scriptsFile, true);
                _streamWriter.AutoFlush = true;
            }

            sql.Trim();
            if (string.IsNullOrEmpty(sql) == false)
            {
                _streamWriter.WriteLine(sql);
            }
        }

        [ObsoleteAttribute("This method is obsolete.", false)]
        private string Script(GISSAP_ASSETSYNCH existingAssetRow, SAPRowData newAssetRow)
        {
            string sql = string.Empty;

            bool existing = true;

            ActionType newActionType = newAssetRow.ActionType;

            if (existingAssetRow == null)
            {
                existing = false;

                existingAssetRow = new GISSAP_ASSETSYNCH();
                existingAssetRow.ASSETID = newAssetRow.AssetID;
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
            }

            ActionType existingActionType = (ActionType)(char)existingAssetRow.ACTIONTYPE[0];
            SetRowAttribute(existingAssetRow, newAssetRow);

            string assetID = existingAssetRow.ASSETID;
            string actionTypeString = existingAssetRow.ACTIONTYPE;
            int sapType = existingAssetRow.TYPE;
            DateTime date = existingAssetRow.DATEPROCESSED;
            string dateString = "to_date('" + date.ToString("MM/dd/yyyy HH:mm:ss") + "','mm/dd/yyyy hh24:mi:ss')";
            string sapAttributes = existingAssetRow.SAPATTRIBUTES;

            if (existing == false)
            {
                sql = "INSERT into " + _dbHelper.GISSAPAssetSyncTableName +
                    "(ASSETID, ACTIONTYPE, TYPE, DATEPROCESSED, SAPATTRIBUTES)  " +
                    "VALUES ('" + assetID + "','" + actionTypeString + "'," + sapType + "," + dateString + ",'" + sapAttributes + "'";
            }
            else if (existingActionType == ActionType.Insert && newActionType == ActionType.Update
                || existingActionType == ActionType.Update && newActionType == ActionType.Update)
            {
                sql = "Update " + _dbHelper.GISSAPAssetSyncTableName +
                    " Set " +
                    " ActionType='" + actionTypeString + "', Type=" + sapType + ", DateProcessed=" + dateString + ", SapAttributes='" + sapAttributes + "'" +
                    " Where AssetID='" + assetID + "'";
            }
            else if (existingActionType == ActionType.Insert && newActionType == ActionType.Delete
                || existingActionType == ActionType.Update && newActionType == ActionType.Delete)
            {
                sql = "Delete from " + _dbHelper.GISSAPAssetSyncTableName +
                    " Where AssetID='" + assetID + "'";
            }
            else if (existingActionType == ActionType.Delete && newActionType == ActionType.Delete)
            {
            }

            return sql;
        }

        /// <summary>
        /// Process a SAP row send from data collector and persist in Database.
        /// </summary>
        /// <param name="assetIdAndData">SAP Data row</param>
        [ObsoleteAttribute("This method is obsolete.", false)]
        public bool Process(KeyValuePair<string, IRowData> assetIdAndData)
        {
            // Query asset id in database                
            var existingAssetRows = _dbHelper.GetRecordsByAssetID(assetIdAndData.Key);

            // If no entry present, insert the sap row in staging table
            if (existingAssetRows.Count == 0)
            {
                return InsertRow(assetIdAndData.Value);
            }
            else // delete or update
            {
                GISSAP_ASSETSYNCH existingAssetRow = existingAssetRows.First();
                return ProcessRow(existingAssetRow, assetIdAndData.Value);
            }
        }

        #endregion 


        /// <summary>
        /// Processes data retrieving from Temp table for a perticular session and persists in Database.
        /// </summary>
        /// <param name="sessionName">Session name</param>
        /// <returns>Boolean value</returns>
        public bool Process(string sessionName)
        {
            //v1t8 dataPersister
            bool success = false;
            try
            {
                // Query asset id in database                
                var assetRows = _dbHelper.GetTempData(sessionName);

                //Delete temp data
                _dbHelper.DeleteTempTableData(sessionName);

                success = Process(assetRows);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }

            return success;
        }

        /// <summary>
        /// Processes GISSAP_ASSETSYNCH data and persists in Database.
        /// </summary>
        /// <param name="assetRows">GISSAP_ASSETSYNCH data</param>
        /// <returns>Boolean value</returns>
        public bool Process(IList<GISSAP_ASSETSYNCH> assetRows)
        {
            bool success = (assetRows.Count == 0);
            string recordID = string.Empty;
            string batchID = string.Empty;
            try
            {
                if (!success)
                {

                    foreach (GISSAP_ASSETSYNCH assetRow in assetRows)
                    {
                        // Query asset id in database  
                        var existingAssetRows = _dbHelper.GetRecordsByAssetID(assetRow.ASSETID);
                        //recordID = _dbHelper.GetBatchID();
                        //batchID = _dbHelper.GetRecordID();
                        // If no entry present, insert the sap row in staging table
                        if (existingAssetRows.Count == 0)
                        {
                            try
                            {
                                success = _dbHelper.InsertData(assetRow);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("Failed to process ObjectID {0} with error {1}: {2}", assetRow.ASSETID, ex.Message, ex.StackTrace));
                                success = false;
                                _dbHelper.LogError(assetRow, ex.Message);
                                continue;
                            }
                        }
                        else // delete or update
                        {
                            try
                            {
                                GISSAP_ASSETSYNCH existingAssetRow = existingAssetRows.First();
                                success = ProcessRow(existingAssetRow, assetRow);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(string.Format("Failed to process ObjectID {0} with error {1}: {2}", assetRow.GUID, ex.Message, ex.StackTrace));
                                _dbHelper.UpdateErrorDescription(assetRow, ex.Message);
                                success = false;
                                continue;
                            }
                        }

                        //  if (!success) break;
                        if (!success) continue;
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        /// <summary>
        /// Processes a SAP rows by data collector/Change Detection and stores in Temp table.
        /// </summary>
        /// <param name="assetIDandRowDataByOutName"></param>
        /// <param name="sessionName"></param>
        /// <returns></returns>
        public bool ProcessTempData(Dictionary<string, Dictionary<string, IRowData>> assetIDandRowDataByOutName, string sessionName, bool deleteTempData)
        {
            //v1t8 need to modify -entry point 3
            bool success = (assetIDandRowDataByOutName.Count == 0);
            string errorDescription = string.Empty;
            // BELOW LINES ARE COMMENTED FOR EDGISREARC PROJECT AS EDO6 TEMP TABLE IS NO MORE REQUIRED - V1T8
            //Delete session data
           // if (deleteTempData) { DeleteTempData(sessionName); }
            try
            {
                if (!success)
                {
                    foreach (KeyValuePair<string, Dictionary<string, IRowData>> classAssetIDandData in assetIDandRowDataByOutName)
                    {

                        string assetClass = classAssetIDandData.Key;
                        foreach (KeyValuePair<string, IRowData> asset in classAssetIDandData.Value)
                        {
                            //try catch added as a fix of duplicate record in stagging table for ED06-374 integration improvement -v1t8
                            try
                            {
                                //Insert Session data into temp stagging table
                                //success = InsertTempRow(asset.Value, sessionName);
                                success = InsertStagingRow(asset.Value);
                                if(success)
                                {
                                    try
                                    {
                                        if ((actionType == "I") && (processedRecordID != string.Empty))
                                            _dbHelper.InsertRelatedRecordIDForED07(processedRecordID);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(MethodInfo.GetCurrentMethod().Name + ":Failed to insert related record Id in ED07 staging table" + asset.Value.AssetID + " in asset synch stagging table for featureclass" + assetClass + ex.StackTrace, ex);
                                        continue;
                                    }
                                }
                                Log.Info("insert record succesful for" + asset.Value.AssetID + " in asset synch stagging table for featureclass" + assetClass);

                            }
                            catch (Exception ex)
                            {

                                Log.Error(MethodInfo.GetCurrentMethod().Name + ":Failed to insert record" + asset.Value.AssetID + " in asset synch stagging table for featureclass" + assetClass + ex.StackTrace, ex);
                                success = false;
                                InsertStagingRowWithError(asset.Value, ex.Message);
                                continue;
                            }
                            //below line commented for ED06-374 integration improvement -v1t8
                            //if (!success) break;
                            //below line added for ED06-374 integration improvement -v1t8
                            if (!success) continue; ;
                        }

                        if (!success) break;
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
            return success;
        }

        public void DeleteTempData(string sessionName)
        {
            _dbHelper.DeleteTempTableData(sessionName);
        }


        /// <summary>
        /// Process SAP row for appropriate action
        /// </summary>
        /// <param name="existingAssetRow">Current assetid row present in database</param>
        /// <param name="newAssetRow">sap row</param>
        [ObsoleteAttribute("This method is obsolete.", false)]
        private bool ProcessRow(GISSAP_ASSETSYNCH existingAssetRow, IRowData newAssetRow)
        {
            ActionType existingActionType = (ActionType)existingAssetRow.ACTIONTYPE[0];

            ActionType newActionType = ActionType.Invalid;
            SAPRowData newSAPRowData = null;
            if (newAssetRow is SAPRowData)
            {
                newSAPRowData = (SAPRowData)newAssetRow;
                newActionType = newSAPRowData.ActionType;
            }

            // Database row says insert and sap says update; 
            // end result: update sap attributes in database
            if (existingActionType == ActionType.Insert && newActionType == ActionType.Update)
            {
                // This makes sure SAPAttributes has I as Action Type, instead of U.
                newSAPRowData.ActionType = ActionType.Insert;
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says insert and sap says delete; 
            // end result: delete row from database
            if (existingActionType == ActionType.Insert && newActionType == ActionType.Delete)
            {
                _dbHelper.DeleteData(existingAssetRow);
                return true;
            }

            // Database row says update and sap says delete; 
            // end result: action as delete and update row from database//xwu 3.25.2013
            if (existingActionType == ActionType.Update && newActionType == ActionType.Delete)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says update and sap says update; 
            // end result: update sap attributes in database
            if (existingActionType == ActionType.Update && newActionType == ActionType.Update)
            {
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says delete and sap says delete; 
            // end result: leave the row in database
            if (existingActionType == ActionType.Delete && newActionType == ActionType.Delete)
            {
                return true;
            }

            // Database row says insert and sap says idle; 
            // end result: action as idle and update sap attributes in database
            if (existingActionType == ActionType.Insert && newActionType == ActionType.Idle)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says update and sap says idle; 
            // end result: action as idle and update sap attributes in database
            if (existingActionType == ActionType.Update && newActionType == ActionType.Idle)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says idle and sap says idle; 
            // end result: action as idle and update sap attributes in database
            if (existingActionType == ActionType.Idle && newActionType == ActionType.Idle)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says idle and sap says update; 
            // end result: action as update and update sap attributes in database
            if (existingActionType == ActionType.Idle && newActionType == ActionType.Update)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }

            // Database row says idle and sap says delete; 
            // end result: action as delete and update sap attributes in database
            if (existingActionType == ActionType.Idle && newActionType == ActionType.Delete)
            {
                existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                SetRowAttribute(existingAssetRow, newSAPRowData);
                _dbHelper.UpdateData(existingAssetRow);
                return true;
            }
            // All valid cases are handled; if control reaches here then error must be logged.
            // But there should not be other cases at this point because ArcFM QA/QC should have made sure 
            // that only good data are submitted for post.

            return false;
        }


        /// <summary>
        /// Processes SAP row for appropriate action
        /// </summary>
        /// <param name="existingAssetRow">Current asset row present in database</param>
        /// <param name="newAssetRow">New asset row present in temp table</param>
        /// <returns>Boolean value</returns>
        private bool ProcessRow(GISSAP_ASSETSYNCH existingAssetRow, GISSAP_ASSETSYNCH newAssetRow)
        {
            ActionType existingActionType = (ActionType)existingAssetRow.ACTIONTYPE[0];
            ActionType newActionType = (ActionType)newAssetRow.ACTIONTYPE[0];
            try
            {
                // Database row says insert and sap says update; 
                // end result: update sap attributes in database

                if (existingActionType == ActionType.Insert && newActionType == ActionType.Update)
                {
                    // This makes sure SAPAttributes has I as Action Type, instead of U.
                    newAssetRow.ACTIONTYPE = ((char)ActionType.Insert).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }



                // Database row says insert and sap says delete; 
                // end result: delete row from database

                if (existingActionType == ActionType.Insert && newActionType == ActionType.Delete)
                {
                    _dbHelper.DeleteData(existingAssetRow);
                    return true;
                }

                // Database row says update and sap says delete; 
                // end result: action as delete and update row from database//xwu 3.25.2013

                if (existingActionType == ActionType.Update && newActionType == ActionType.Delete)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says update and sap says update; 
                // end result: update sap attributes in database
                if (existingActionType == ActionType.Update && newActionType == ActionType.Update)
                {
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says delete and sap says delete; 
                // end result: leave the row in database
                if (existingActionType == ActionType.Delete && newActionType == ActionType.Delete)
                {
                    return true;
                }

                // Database row says insert and sap says idle; 
                // end result: action as idle and update sap attributes in database
                if (existingActionType == ActionType.Insert && newActionType == ActionType.Idle)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says update and sap says idle; 
                // end result: action as idle and update sap attributes in database
                if (existingActionType == ActionType.Update && newActionType == ActionType.Idle)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says idle and sap says idle; 
                // end result: action as idle and update sap attributes in database
                if (existingActionType == ActionType.Idle && newActionType == ActionType.Idle)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says idle and sap says update; 
                // end result: action as update and update sap attributes in database
                if (existingActionType == ActionType.Idle && newActionType == ActionType.Update)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }

                // Database row says idle and sap says delete; 
                // end result: action as delete and update sap attributes in database
                if (existingActionType == ActionType.Idle && newActionType == ActionType.Delete)
                {
                    existingAssetRow.ACTIONTYPE = ((char)newActionType).ToString();
                    SetRowAttribute(existingAssetRow, newAssetRow);
                    _dbHelper.UpdateData(existingAssetRow);
                    return true;
                }
                // All valid cases are handled; if control reaches here then error must be logged.
                // But there should not be other cases at this point because ArcFM QA/QC should have made sure 
                // that only good data are submitted for post.
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }
            return false;
        }

        /// <summary>
        /// This row takes a SAP row data and converts it into a DB row and persists in DB
        /// </summary>
        /// <param name="rowData">SAP row data</param>
        [ObsoleteAttribute("This method is obsolete.", false)]
        private bool InsertRow(IRowData rowData)
        {
            SAPRowData sapData = (SAPRowData)rowData;

            GISSAP_ASSETSYNCH dbRow = new GISSAP_ASSETSYNCH();
            dbRow.ASSETID = sapData.AssetID;
            dbRow.ACTIONTYPE = ((char)sapData.ActionType).ToString();
            //************************************
            dbRow.RECORDTYPE = sapData.SAPType.ToString();            
            dbRow.RECORDID = _dbHelper.GetRecordID(dbRow.RECORDTYPE);
            dbRow.PROCESSSEDFLAG = ProcessFlag.GISProcessed;
            foreach (var pair in sapData.FieldKeyValue)
            {
                if (pair.Key == "IDType")
                {
                    dbRow.ASSETTYPE = pair.Value;
                }
            }

            //*********************************************8




            SetRowAttribute(dbRow, sapData);

            return _dbHelper.InsertData(dbRow);
        }

        /// <summary>
        /// Inserts a GISSAP_ASSETSYNCH data into main table
        /// </summary>
        /// <param name="assetRow">GISSAP_ASSETSYNCH data</param>
        /// <returns>Boolean value</returns>
        private bool InsertRow(GISSAP_ASSETSYNCH assetRow)
        {
            return _dbHelper.InsertData(assetRow);
        }

        /// <summary>
        /// This row takes a SAP row data and converts it into a DB row and persists in Temp table
        /// </summary>
        /// <param name="rowData">SAP row data</param>      
        /// <returns>Boolean value</returns>
        private bool InsertStagingRow(IRowData rowData)
        {
            //v1t8- Need to modify entry point 4
            GISSAP_ASSETSYNCH dbRow = new GISSAP_ASSETSYNCH();
            try
            {
                SAPRowData sapData = (SAPRowData)rowData;
                dbRow.RECORDTYPE = sapData.SAPType.ToString();
                dbRow.ASSETID = sapData.AssetID;

                dbRow.ACTIONTYPE = ((char)sapData.ActionType).ToString();

                

                //BatchID,RecordID,ProcessedTime,Error Description,ProcessedFlag added for for ED06-374 integration improvement - v1t8

                //  dbRow.RECORDID = DataHelper.GetRecordID(dbRow.RECORDTYPE); _dbHelper.GetRecordID()
                dbRow.RECORDID = _dbHelper.GetRecordID(dbRow.RECORDTYPE);
                // dbRow.PROCESSSEDTIME = DateTime.Now.ToString("h:mm:ss tt");
                //if (errorMessage==null || errorMessage==string.Empty)
                //{
                //    dbRow.PROCESSSEDFLAG = ProcessFlag.GISProcessed;
                //}
                //else
                dbRow.PROCESSSEDFLAG = ProcessFlag.GISProcessed;
                //  dbRow.ERRORDESCRIPTION = errorMessage;

                foreach (var pair in sapData.FieldKeyValue)
                {
                    if (pair.Key == "IDType")
                    {
                        dbRow.ASSETTYPE = pair.Value;
                    }
                }

                actionType = dbRow.ACTIONTYPE;
                processedRecordID = dbRow.RECORDID;
                SetRowAttribute(dbRow, sapData);

                
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }

            //commented for for ED06-374 integration improvement - v1t8
            //return _dbHelper.InsertTempData(dbRow, sessionName);

            //Added for for ED06-374 integration improvement - v1t8
            return _dbHelper.InsertData(dbRow);
        }

        /// <summary>
        /// Method to log exception in staging  table- EDGIS Rearch Pproject 2021- v1t8
        /// </summary>
        /// <param name="rowData"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool InsertStagingRowWithError(IRowData rowData, string errorMessage)
        {
            //v1t8- Need to modify entry point 4
            GISSAP_ASSETSYNCH dbRow = new GISSAP_ASSETSYNCH();
            try
            {
                SAPRowData sapData = (SAPRowData)rowData;
                dbRow.RECORDTYPE = sapData.SAPType.ToString();
                dbRow.ASSETID = sapData.AssetID;
                dbRow.ACTIONTYPE = ((char)sapData.ActionType).ToString();
                dbRow.DATEPROCESSED = sapData.DateProcessed;

                //BatchID,RecordID,ProcessedTime,Error Description,ProcessedFlag added for for ED06-374 integration improvement - v1t8

              //  dbRow.RECORDID = DataHelper.GetRecordID(dbRow.RECORDTYPE);
                dbRow.RECORDID = _dbHelper.GetRecordID(dbRow.RECORDTYPE);
                dbRow.PROCESSSEDFLAG = ProcessFlag.GISError;
                if (errorMessage != null || errorMessage != string.Empty)
                    dbRow.ERRORDESCRIPTION = errorMessage;
               
                SetRowAttribute(dbRow, sapData);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Log.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }

            //commented for for ED06-374 integration improvement - v1t8
            //return _dbHelper.InsertTempData(dbRow, sessionName);

            //Added for for ED06-374 integration improvement - v1t8
            return _dbHelper.LogError(dbRow, errorMessage);
        }

        /// <summary>
        /// Given a GISSAP_ASSETSYNCH row and SAPROWDATA will transfer information from SAPRowData to GIS_AssetSYNCH object.
        /// </summary>
        /// <param name="assetDBRow">Row to be persisted in DB - Type GISSAP_AssetSynch</param>
        /// <param name="sapRowData">SAPRowData implementation of IRowData</param>
        /// <returns></returns>
        private GISSAP_ASSETSYNCH SetRowAttribute(GISSAP_ASSETSYNCH assetDBRow, SAPRowData sapRowData)
        {

            assetDBRow.TYPE = (short)sapRowData.SAPType;
            assetDBRow.DATEPROCESSED = sapRowData.DateProcessed;
          //  SortedList<int, string> sortedField = new SortedList<int, string>(sapRowData.FieldValues);
            Dictionary<string, string> sortedField1 = sapRowData.FieldKeyValue;

            //Create the Comma separated value to be stored in the table.
          //  assetDBRow.SAPATTRIBUTES = Sanitize(sortedField);
            assetDBRow.SAPATTRIBUTES = Sanitize1(sortedField1, sapRowData);


            return assetDBRow;
        }

        /// <summary>
        /// Assigns new asset data from Temp table to the GISSAP_ASSETSYNCH row
        /// </summary>
        /// <param name="assetDBRow">Row to be persisted in DB - Type GISSAP_AssetSynch</param>
        /// <param name="newAssetRow">Row from Temp table - Type GISSAP_AssetSynch</param>
        /// <returns></returns>
        private GISSAP_ASSETSYNCH SetRowAttribute(GISSAP_ASSETSYNCH assetDBRow, GISSAP_ASSETSYNCH newAssetRow)
        {
            assetDBRow.TYPE = newAssetRow.TYPE;
            assetDBRow.DATEPROCESSED = newAssetRow.DATEPROCESSED;
            assetDBRow.SAPATTRIBUTES = newAssetRow.SAPATTRIBUTES;
            return assetDBRow;
        }


        /// <summary>
        /// This method sanitizes the data values if they contain '"' or ","
        /// The values are enclosed in double quotes if they contain the above chars.
        /// </summary>
        /// <param name="fieldValues">List of field values</param>
        /// <returns>The sanitized field values concatenated together with commas</returns>
        private string Sanitize(SortedList<int, string> fieldValues)
        {
            //below line commented to include pipe is GIS data  ED06 - 374 integration improvement - v1t8
            //return string.Join(",", fieldValues.Values.Select(v => Csv.Escape(Convert.ToString((object)v))).ToArray());

            return string.Join(",", fieldValues.Values.Select(v => Csv.Escape(Convert.ToString((object)v))).ToArray());

        }
        // (V3SF) Create Json String (26-Jan-2022)
        public class Searlizer
        {
            public string KeyField;
            public string KeyValue;
        }
        private string Sanitize1(Dictionary<string, string> fieldValues, SAPRowData sapRowData)
        {
            //below line commented to include pipe is GIS data  ED06 - 374 integration improvement - v1t8

            //Added below for key value formate for EDGIS Re-Arch project-v1t8
            //            {"KeyField" : "AutoBooster","KeyValue" : "5" },
            string recordType = sapRowData.SAPType.ToString();
            string type = string.Empty;
            string keyValue = string.Join(",", fieldValues.Select(x => "{" + "'KeyField'" + ":" + "'" + x.Key + "'" + "," + "'KeyValue'" + ":" + "'" + x.Value + "'" + "}").ToArray());
            string s = "{" + "'KeyField'" + ":" + "'ActionType'" + "," + "'KeyValue'" + ":" + "'" + ((char)sapRowData.ActionType).ToString() + "'" + "}";
            keyValue = (s +","+  keyValue).Replace('\'', '"');

            keyValue = keyValue.Replace('\'', '"');

            if (recordType == SAPType.FunctionalLocation.ToString())

            { type = "NAV_ED06_FL_KF_KV"; }
            if (recordType == SAPType.StructureEquipment.ToString() || recordType == SAPType.StructureSubEquipment.ToString()) { type = "NAV_ED06_ST_KF_KV"; }

            if (recordType == SAPType.DeviceEquipment.ToString() || recordType == SAPType.DeviceSubEquipment.ToString()) { type = "NAV_ED06_EQ_KF_KV"; }

            //  keyValue = "\"" + type + "\"" + ":[" + "{" + "'KeyField'" + ":" + "ActionType" + "," + "'KeyValue'" + ":" + "'" + ((char)sapRowData.ActionType).ToString() + "'" + "}" + keyValue + "]";
            keyValue = "{" + "\"" + type + "\"" + ":[" + keyValue + "]" + " }";

            //(V3SF) Valid Json Code (26-Jan-2022) [START]
            List<Searlizer> obje = new List<Searlizer>();
            Searlizer searlizer1 = new Searlizer();
            searlizer1.KeyField = "ActionType";
            searlizer1.KeyValue = ((char)sapRowData.ActionType).ToString();
            if (string.IsNullOrWhiteSpace(searlizer1.KeyValue))
                searlizer1.KeyValue = "";
            obje.Add(searlizer1);
            
            foreach (var fv in fieldValues)
            {
                Searlizer searlizer = new Searlizer();
                searlizer.KeyField = fv.Key;
                searlizer.KeyValue = fv.Value;
                if (string.IsNullOrWhiteSpace(searlizer.KeyValue))
                    searlizer.KeyValue = "";
                obje.Add(searlizer);
            }

            string so = JsonConvert.SerializeObject(obje);
            string json = "{\"" + type + "\":" + so + "}";
            return json;
            //(V3SF) Valid Json Code (26-Jan-2022) [END]
            //return keyValue;
        }


        /// <summary>
        /// Start a new transaction. Must commit to persist data if a new transaction starts.
        /// </summary>
        public void BeginTransaction()
        {
            this._dbHelper.BeginTransaction();
        }

        /// <summary>
        /// Commit DB changes. 
        /// </summary>
        public void Commit()
        {
            if (this._dbHelper.Transaction != null)
            {
                this._dbHelper.Transaction.Commit();
            }
        }

        /// <summary>
        /// Rollback DB changes. 
        /// </summary>
        public void Rollback()
        {
            if (this._dbHelper.Transaction != null)
            {
                this._dbHelper.Transaction.Rollback();
            }
        }

        /// <summary>
        /// Release resources used by this object
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
                    if (_dbHelper != null)
                    {
                        _dbHelper.Dispose();
                    }

                    if (_streamWriter != null)
                    {
                        _streamWriter.Close();
                        _streamWriter.Dispose();
                    }
                }

                _streamWriter = null;
                _dbHelper = null;

                this._disposed = true;
            }
        }

    }
}