using System;
using System.Collections; 
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using System.Xml;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Diagnostics;
using System.Threading;

namespace PGE.Interfaces.MapProductionAutomation.MapProd
{
    public enum MapStatusUpdateType
    {
        mapUpdateTypeProcessing = 1,
        mapUpdateTypeFinished = 2,
        mapUpdateTypeError = 3
    }

    class MapProductionHelper
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.MapProduction2.0.log4net.config", "MapProduction2");
        private int _maxAttempts;
        private string _unifiedMapGridSequence = "";
        private OracleConnection _pConnection = null;



        /// <summary>
        /// Initialises new instance of PGEExportHelper
        /// </summary>
        public MapProductionHelper(int maxAttempts, int processId, bool connectToDB)
        {
            _maxAttempts = maxAttempts;            

            //Open the ADO.NET connection 
            if (connectToDB)
            {
                _pConnection = new OracleConnection(Common.Common.ConnectionString);
                OpenConnection();
            }
        }

        private OracleConnection _pConn
        {
            get
            {
                if (_pConnection == null)
                {
                    _pConnection = new OracleConnection(Common.Common.ConnectionString);
                }
                OpenConnection();
                return _pConnection;
            }
            set
            {
                _pConnection = value;
            }
        }

        private void OpenConnection()
        {
            if (_pConnection == null)
            {
                _pConnection = _pConn;
            }
                
            if (_pConnection.State != ConnectionState.Open)
            {
                _pConnection.Open();
                //using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = " + 90, _pConnection))
                //{
                //    cmd.ExecuteNonQuery();
                //    cmd.Dispose();
                //}
            }
        }

        ~MapProductionHelper()
        {
        }

        public enum LogType
        {
            Debug,
            Info,
            Error
        }
        /// <summary>
        /// Logs the message to the underlying streamwriter
        /// </summary>
        /// <param name="log">Message to log</param>
        public static void Log(string log, LogType logType, Exception ex)
        {
            if (logType == LogType.Debug) { _logger.Debug(log); }
            else if (logType == LogType.Error) { _logger.Error(log, ex); }
            else if (logType == LogType.Info) { _logger.Info(log); }
        }

        /// <summary>
        /// Disconnects ADO.NET connection to database 
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_pConn != null)
                {
                    if (_pConn.State == ConnectionState.Open)
                        _pConn.Close(); 
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error performing ADO disconnect");
            }
        }

        /// <summary>
        /// Returns the current map grid feature 
        /// </summary>
        /// <param name="pMapGridFC"></param>
        /// <param name="mapNumber"></param>
        /// <param name="mapScale"></param>
        /// <returns></returns>
        public IFeature GetMapGridFeature(
            IFeatureClass pMapGridFC,
            string mapNumber,
            string mapScale)
        {
            try
            {
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause =
                    "MAPNO" + " = " + "'" + mapNumber + "'" + " And " +
                    "SCALE" + " = " + mapScale;
                IFeatureCursor pFCursor = pMapGridFC.Search(pQF, false);
                IFeature pMapGridFeature = pFCursor.NextFeature();
                Marshal.FinalReleaseComObject(pFCursor);
                return pMapGridFeature; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning map grid feature");
            }
        }        

        /// <summary>
        /// Reads the list of maptypes to be exported from the config file  
        /// </summary>
        /// <returns></returns>
        public Hashtable GetMapsFromConfigurationFile(string configurationFile)
        {
            try
            {
                //Open our configuration file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);
                Hashtable hshMXDs = new Hashtable(); 

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");
                string mapType = "";
                string mapTypeAndScale = "";

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];

                    //Grab the name of the Mxd
                    mapType = node.Attributes["OutputName"].Value.ToUpper();
                    string[] scales = Regex.Split(node.Attributes["Scale"].Value.ToLower(), ",");

                    for (int j = 0; j < scales.Count(); j++)
                    {
                        mapTypeAndScale = mapType + "*" + scales[j];
                        if (!hshMXDs.ContainsKey(mapTypeAndScale))
                            hshMXDs.Add(mapTypeAndScale, 0);
                    }
                }
                return hshMXDs; 
            }
            catch (Exception e)
            {
                throw new Exception("Error returning MXDs from config file");
            }
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool MapsStillToProcess(string configurationFile)
        {
            bool mapsStillToProcess = false;
            List<string> sqlStatements = new List<string>();
            //Scenario 4 - try to get any map that is not finished in the current map division  
            string sqlStatement = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.ReadyToExport).ToString() +
                " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
            sqlStatements.Add(sqlStatement);

            //maps that of status Failed but with less than the max attempts
            sqlStatement = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.Error).ToString() + " And " +
                Common.Common.FieldChangeDetectionFailureCount + " < " + _maxAttempts.ToString() +
                " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
            sqlStatements.Add(sqlStatement);
            try
            {
                OpenConnection();
                foreach (string sql in sqlStatements)
                {
                    using (var cmd = new OracleCommand(sql, _pConn))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                mapsStillToProcess = true;
                                break;
                            }
                            dataReader.Close();
                            dataReader.Dispose();
                        }
                        cmd.Dispose();
                    }
                    if (mapsStillToProcess) { break; }
                }
            }
            catch (Exception ex)
            {
                Log("Error determining if maps are left to process: ", LogType.Error, ex);
            }
            finally
            {
                _pConn.Close();
                _pConn.Dispose();
                _pConn = null;
            }
            return mapsStillToProcess;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool GetNextMapToProcess(string configurationFile, int processID, string machineName, ref string MapType, ref string MapScale, 
            ref string MapMxd, ref int FailurCount, ref string mapNumber)
        {
            bool hasNewMapToProcess = false;
            List<string> sqlStatements = new List<string>();
            Dictionary<string,string> mapTypeAndScaleByMxdName = PopulateMapScaleAndMapMXDByProcessId(configurationFile);
            try
            {
                sqlStatements = GetNextMapSelectQueries(configurationFile, processID, MapType, MapScale);
                OpenConnection();
                //tr = _pConn.BeginTransaction();
                //using (var cmd = new OracleCommand("LOCK TABLE " + Common.Common.TableChangeDetection + " IN EXCLUSIVE MODE", _pConn))
                //{
                //    cmd.Transaction = tr;
                //    cmd.ExecuteNonQuery();
                //    cmd.Dispose();
                //}

                mapNumber = "";
                //Iterate over each of our sql statements until we find a map to process
                foreach (string sql in sqlStatements)
                {
                    using (var cmd = new OracleCommand(sql, _pConn))
                    {
                       // cmd.Transaction = tr;
                        cmd.CommandType = CommandType.Text;

                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                MapType = dataReader[Common.Common.FieldChangeDetectionMapType].ToString();
                                MapScale = dataReader[Common.Common.FieldChangeDetectionScale].ToString();
                                string key = MapType + "-" + MapScale;
                                if (mapTypeAndScaleByMxdName.ContainsKey(key.ToUpper())) {MapMxd = mapTypeAndScaleByMxdName[key.ToUpper()];}
                                else {MapMxd = "";}
                                try { FailurCount = Int32.Parse(dataReader[Common.Common.FieldChangeDetectionFailureCount].ToString()); }
                                catch { FailurCount = 0; }
                                mapNumber = dataReader[Common.Common.FieldChangeDetectionMapNumber].ToString();
                                hasNewMapToProcess = true;
                                break;
                            }
                            dataReader.Close();
                            dataReader.Dispose();
                        }
                        cmd.Dispose();
                    }
                    if (!string.IsNullOrEmpty(mapNumber))
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(mapNumber))
                {
                    //We found a new map to process so let's update our table to reflect that we are processing it currently
                    int newStatusInt = (int)Common.Common.ExportStates.InProgress;
                    string sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processID.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " +
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionStartDate + " = SYSDATE Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + MapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + MapType + "'";
                    using (var cmd = new OracleCommand(sql, _pConn))
                    {
                        //cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                //tr.Commit();
            }
            catch (Exception ex)
            {
              
                Log("Error determining next map to process: ", LogType.Error, ex);
            }
            finally
            {
                _pConn.Close();
                _pConn.Dispose();
                _pConn = null;
               
            }
            return hasNewMapToProcess;
        }

        /// <summary>
        /// Obtain the next map queries to obtain.  These are in the order they should be checked.
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public List<string> GetNextMapSelectQueries(string configurationFile,
            int processId,
            string currentMapType,
            string currentMapScale)
        {
            List<string> sqlQueries = new List<string>();

            try
            {
                string sql = "";
                //-------------------------------------------------------------------
                //Scenario 1 - Get the first map as specified in the configuration file
                if (string.IsNullOrEmpty(currentMapType) || string.IsNullOrEmpty(currentMapScale))
                {
                    string defaultMapScale = "";
                    string defaultMapType = "";
                    string defaultMXD = "";
                    Common.Common.PopulateMapScaleAndMapMXDByProcessId(configurationFile,
                        processId, ref defaultMapScale, ref defaultMapType, ref defaultMXD);
                    sql = "Select * From " + Common.Common.TableChangeDetection +
                        " Where " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + defaultMapType + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + defaultMapScale + " And " +
                        Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.ReadyToExport).ToString() +
                        " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
                    sqlQueries.Add(sql);
                    currentMapScale = defaultMapScale;
                }

                
                //-------------------------------------------------------------------
                //Scenario 2 - try to get a map with the same map type and map scale as the current map type and scale. This helps to not
                //have to keep opening new mxd files.
                sql = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                    Common.Common.FieldChangeDetectionScale + " = " + currentMapScale + " And " +
                    Common.Common.FieldChangeDetectionMapType + " = " + "'" + currentMapType + "'" + " And " +
                    Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.ReadyToExport).ToString() +
                    " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
                sqlQueries.Add(sql);

                //-------------------------------------------------------------------
                //Scenario 3 - try to get a map with the same map_type but different scale 
                sql = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                    Common.Common.FieldChangeDetectionMapType + " = " + "'" + currentMapType + "'" + " And " +
                    Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.ReadyToExport).ToString() +
                    " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
                sqlQueries.Add(sql);

                //-------------------------------------------------------------------
                //Scenario 4 - try to get any map that is not finished in the current map division  
                sql = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                    Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.ReadyToExport).ToString() +
                    " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
                sqlQueries.Add(sql);

                //maps that of status Failed but with less than the max attempts
                sql = "Select * from " + Common.Common.TableChangeDetection + " WHERE " +
                    Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.Error).ToString() + " And " +
                    Common.Common.FieldChangeDetectionFailureCount + " < " + _maxAttempts.ToString() +
                    " ORDER BY " + Common.Common.FieldChangeDetectionPriority + " " + "ASC";
                sqlQueries.Add(sql);

                return sqlQueries;
            }
            catch (Exception ex)
            {
                Log("Entering obtaining sql statements for obtaining the next maps", LogType.Error, ex);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Search intelligently for the next map to export. It will 
        /// favor those maps for which the mxd does not have to change 
        /// which will be faster as all the layers in the map will not 
        /// have to be reloaded 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="curMapRow"></param>
        /// <param name="mapOffice"></param>
        /// <param name="curMapType"></param>
        /// <returns></returns>
        public DateTime GetTimeOfLastSucessfulMap(
            int processId,
            DateTime processStartTime)
        {

            try
            {
                Log("Entering GetTimeOfLastSucessfulMap", MapProductionHelper.LogType.Info, null);
                Log(" processId: " + processId.ToString(), MapProductionHelper.LogType.Info, null);
                DateTime timeOfLastSuccess = new DateTime(1900, 1, 1);
                string sql = "";
                const string SUCCESS_DATE = "SUCCESSDATE"; 
                OracleDataReader newMapRow = null;
                
                //Determine for TIFF 
                OracleCommand pCmd = new OracleCommand();
                pCmd.Connection = _pConn;

                sql = "Select MAX(" + Common.Common.FieldChangeDetectionEndDate + ")" + " As " + SUCCESS_DATE +
                    " From " + Common.Common.TableChangeDetection + 
                    " Where " +
                    Common.Common.FieldChangeDetectionMachineName + " = " + "'" + System.Environment.MachineName.ToUpper() + "'" + " And " +
                    Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + " And " +
                    Common.Common.FieldChangeDetectionExportState + " = " + ((int)Common.Common.ExportStates.Processed).ToString();
                //Log(sql);
                pCmd.CommandText = sql; 
                newMapRow = pCmd.ExecuteReader();
                if (newMapRow.Read())
                {
                    if (!newMapRow.IsDBNull(newMapRow.GetOrdinal(SUCCESS_DATE)))
                        timeOfLastSuccess = newMapRow.GetDateTime(newMapRow.GetOrdinal(SUCCESS_DATE));
                    newMapRow.Close(); 
                }

                //If the process has only jut started return the time of last success 
                //as the process start time 
                if (processStartTime > timeOfLastSuccess)
                {
                    Log("Process start time is more recent than the time of last success", MapProductionHelper.LogType.Info, null);
                    timeOfLastSuccess = processStartTime;
                }

                //Return the time of the last successful exported map
                Log("Returning: " + timeOfLastSuccess.ToShortTimeString(), MapProductionHelper.LogType.Info, null);
                return timeOfLastSuccess; 
            }
            catch (Exception ex)
            {
                Log("Entering Error Handler for GetTimeOfLastSucessfulMap", LogType.Error, ex);
                throw ex;
            }
        }

        private Dictionary<string, string> MapTypeAndScaleByMxdName = new Dictionary<string, string>();
        private Dictionary<string, string> PopulateMapScaleAndMapMXDByProcessId(string configurationFile)
        {
            try
            {
                if (MapTypeAndScaleByMxdName.Count > 0) { return MapTypeAndScaleByMxdName; }

                //Open our configuration file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];

                    string mxdName = node.Attributes["MxdName"].Value;
                    string mapType = node.Attributes["OutputName"].Value;
                    string mapScale = node.Attributes["Scale"].Value;
                    string key = mapType + "-" + mapScale;

                    if (!MapTypeAndScaleByMxdName.ContainsKey(key.ToUpper())) { MapTypeAndScaleByMxdName.Add(key.ToUpper(), mxdName); }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error determining map types, scale, and mxd combinations.  Check the configuration file");
            }
            return MapTypeAndScaleByMxdName;
        }

        //private void PopulateMapScaleAndMapMXDByProcessId(int processId, ref string curMapScale, ref string curMapMXD)
        //{
        //    try
        //    {
        //        //Get the last digit of the processId 
        //        string procIdString = processId.ToString();
        //        int lastDigit = Convert.ToInt32(procIdString.Substring(procIdString.Length - 1, 1));                

        //        switch (lastDigit)
        //        {
        //            case (0):
        //                curMapScale = "100";
        //                curMapMXD = "TIFF";
        //                break;
        //            case (1):
        //                curMapScale = "100";
        //                curMapMXD = "ED_M_and_C";
        //                break;
        //            case (2):
        //                curMapScale = "100";
        //                curMapMXD = "ED_M_and_C";
        //                break;
        //            case (3):
        //                curMapScale = "100";
        //                curMapMXD = "TIFF";
        //                break;
        //            case (4):
        //                curMapScale = "100";
        //                curMapMXD = "JointUtility";
        //                break;
        //            case (5):
        //                curMapScale = "100";
        //                curMapMXD = "Streetlight";
        //                break;
        //            case (6):
        //                curMapScale = "500";
        //                curMapMXD = "CircuitMap";
        //                break;
        //            case (7):
        //                curMapScale = "100";
        //                curMapMXD = "CircuitMap";
        //                break;
        //            case (8):
        //                curMapScale = "100";
        //                curMapMXD = "ElectricDistribution";
        //                break;
        //            case (9):
        //                curMapScale = "100";
        //                curMapMXD = "JointUtility";
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Entering Error Handler for PopulateMapScaleAndMapMXDByProcessId: " + ex.Message);
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Updates the ExportStatus to indicate what stage in the 
        /// mapping process the map is at e.g. Processing, Finished, 
        /// Failed etc
        /// When this function returns false it is an indication that 
        /// another process already took this particular map, so it is skipped 
        /// in that case 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="exportState"></param>
        /// <param name="pDBConnection"></param>
        /// <param name="pExportState"></param>
        /// <param name="mapNumber"></param>
        /// <param name="mapScale"></param>
        /// <param name="map_type"></param>
        /// <param name="machineName"></param>
        /// <param name="numFailures"></param>
        /// <param name="errorMsg"></param>
        public bool UpdateMapStatus(
            int processId,
            int initalExportState, 
            Common.Common.ExportStates pExportState,      
            string mapNumber, 
            string mapScale, 
            string map_type, 
            string machineName,
            int numFailures,
            string errorMsg)
        {
            string sql = "";

            try
            {
                Log("Entering UpdateMapStatus", MapProductionHelper.LogType.Debug, null); 
                int recordsUpdated = -1;
                int newStatusInt = -1;
                bool successFlag = true; 
                const int ERROR_FIELD_MAX_LENGTH = 255;
                const string SYS_DATE = "SYSDATE"; 

                //Check the length of the error field 
                if (errorMsg.Length > ERROR_FIELD_MAX_LENGTH)
                    errorMsg = errorMsg.Substring(0, ERROR_FIELD_MAX_LENGTH);
                if (errorMsg.ToLower().Contains("is denied."))
                    errorMsg = "Access to the output file path is denied";
                if (errorMsg.ToLower().Contains("being used by another process"))
                    errorMsg = "File is being used by another process";

                //Build the SQL for the update for each status scenario 
                if (pExportState == Common.Common.ExportStates.InProgress)
                {
                    newStatusInt = (int)Common.Common.ExportStates.InProgress;
                    sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " + 
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionStartDate + " = " + SYS_DATE + 
                        " Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + mapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + map_type + "'" + " And " +
                        Common.Common.FieldChangeDetectionExportState + " = " + initalExportState.ToString(); 
                }
                else if (pExportState == Common.Common.ExportStates.Processed)
                {
                    newStatusInt = (int)Common.Common.ExportStates.Processed;
                    sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " +
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionErrorMsg + " = " + "'" + errorMsg + "'" + ", " +
                        Common.Common.FieldChangeDetectionEndDate + " = " + SYS_DATE + 
                        " Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + mapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + map_type + "'";
                }
                else if (pExportState == Common.Common.ExportStates.RequiredDataMissing)
                {
                    newStatusInt = (int)Common.Common.ExportStates.RequiredDataMissing;
                    sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " +
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionErrorMsg + " = " + "'" + errorMsg + "'" + ", " +
                        Common.Common.FieldChangeDetectionEndDate + " = " + SYS_DATE + ", " +
                        Common.Common.FieldChangeDetectionFailureCount + " = " + numFailures.ToString() +
                        " Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + mapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + map_type + "'";
                }
                else if (pExportState == Common.Common.ExportStates.LocationInformationMissing)
                {
                    newStatusInt = (int)Common.Common.ExportStates.LocationInformationMissing;
                    sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " +
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionErrorMsg + " = " + "'" + errorMsg + "'" + ", " +
                        Common.Common.FieldChangeDetectionEndDate + " = " + SYS_DATE + ", " +
                        Common.Common.FieldChangeDetectionFailureCount + " = " + numFailures.ToString() +
                        " Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + mapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + map_type + "'";
                }
                else if (pExportState == Common.Common.ExportStates.Error)
                {
                    newStatusInt = (int)Common.Common.ExportStates.Error;
                    sql = "UPDATE " + Common.Common.TableChangeDetection + " SET " +
                        Common.Common.FieldChangeDetectionServiceToProcess + " = " + processId.ToString() + ", " +
                        Common.Common.FieldChangeDetectionMachineName + " = " + "'" + machineName + "'" + ", " +
                        Common.Common.FieldChangeDetectionExportState + " = " + newStatusInt.ToString() + ", " +
                        Common.Common.FieldChangeDetectionErrorMsg + " = " + "'" + errorMsg + "'" + ", " +
                        Common.Common.FieldChangeDetectionEndDate + " = " + SYS_DATE + ", " +
                        Common.Common.FieldChangeDetectionFailureCount + " = " + numFailures.ToString() +
                        " Where " +
                        Common.Common.FieldChangeDetectionMapNumber + " = " + "'" + mapNumber + "'" + " And " +
                        Common.Common.FieldChangeDetectionScale + " = " + mapScale + " And " +
                        Common.Common.FieldChangeDetectionMapType + " = " + "'" + map_type + "'";
                }                
                else
                {
                    throw new Exception("Unknown Map Status Update Type");
                }

                //Update the row by calling ExecuteNonQuery ADO wrapper
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;

                pCmd.CommandText = sql;
                recordsUpdated = pCmd.ExecuteNonQuery();
                if (recordsUpdated == 0)
                {
                    if (pExportState == Common.Common.ExportStates.InProgress)
                        Log("UpdateMapStatus failed to set status to InProgress", MapProductionHelper.LogType.Info, null);
                    else
                        Log("UpdateMapStatus failed to set status to something other than InProgress", MapProductionHelper.LogType.Info, null);
                    successFlag = false;
                }

                return successFlag; 
            }
            catch (Exception ex)
            {
                Log("Entering Error Handler for UpdateMapStatus: Sql: " + sql, LogType.Error, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Inserts necessary TIFF information into the EDGIS.PGE_UNIFIEDGRIDMAP table 
        /// </summary>
        /// <param name="pDBConnection"></param>
        /// <param name="mapExtent"></param>
        /// <param name="mapGrid"></param>
        /// <param name="mapNumber"></param>
        /// <param name="division"></param>
        /// <param name="district"></param>
        public void InsertTIFFInformation(
            IEnvelope mapExtent,
            IFeature mapGrid,
            string mapNumber,
            string division,
            string district)
        {
            try
            {
                //Determine our maximum latitude and longitude values for this feature                
                double LongMin;
                double LongMax;
                double LatMin;
                double LatMax;
                const string SYS_DATE = "SYSDATE";
                GetLatLong(mapExtent, mapGrid, out LongMin, out LongMax, out LatMin, out LatMax);

                //Get the current date in an oracle friendly format
                string changeDate = new System.Data.OracleClient.OracleDateTime(DateTime.Now).ToString();
                string sql = "UPDATE " + Common.Common.TableTIFUnifiedGrid + " SET " +
                    Common.Common.FieldTIFMapDivision + " = " + "'" + division + "'" + ", " +
                    Common.Common.FieldTIFMapDistrict + " = " + "'" + district + "'" + ", " +
                    Common.Common.FieldTIFLongMin + " = " + LongMin.ToString() + ", " +
                    Common.Common.FieldTIFLongMax + " = " + LongMax.ToString() + ", " +
                    Common.Common.FieldTIFLatMin + " = " + LatMin.ToString() + ", " +
                    Common.Common.FieldTIFLatMax + " = " + LatMax.ToString() + ", " +
                    Common.Common.FieldTIFMapNumber + " = " + "'" + mapNumber + "'" + ", " +
                    Common.Common.FieldTIFLastModified + " = " + SYS_DATE +
                    " Where " +
                    Common.Common.FieldTIFMapNumber + " = " + "'" + mapNumber + "'";

                OracleTransaction trans = _pConn.BeginTransaction();
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.Transaction = trans;
                pCmd.CommandText = sql;
                int recordsUpdated = pCmd.ExecuteNonQuery();
                trans.Commit();
                trans.Dispose();
                if (recordsUpdated > 0)
                {
                    Log("InsertTIFFInformation - update succeeded!", LogType.Info, null);
                    return;
                }

                //Get the name of the oid sequence 
                if (_unifiedMapGridSequence == string.Empty)
                {
                    int regId = GetSDETableRegistryId(
                        Common.Common.TableTIFUnifiedGrid);
                    _unifiedMapGridSequence = "EDGIS.R" + regId.ToString();
                }

                //Get the next value for OId 
                sql = "SELECT " + _unifiedMapGridSequence + ".NEXTVAL FROM DUAL";
                pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                //pCmd.Transaction = trans;
                pCmd.CommandText = sql;
                object retVal = pCmd.ExecuteScalar();//.ExecuteOracleScalar();
                //long nextOId = Convert.ToInt64(retVal); 

                //If we got to here the update did not work so we need to try 
                //the insert - since the record was not there
                sql = "INSERT INTO " + Common.Common.TableTIFUnifiedGrid +
                    "(" +
                        Common.Common.FieldTIFObjectId + "," +
                        Common.Common.FieldTIFMapDivision + "," +
                        Common.Common.FieldTIFMapDistrict + "," +
                        Common.Common.FieldTIFLongMin + "," +
                        Common.Common.FieldTIFLongMax + "," +
                        Common.Common.FieldTIFLatMin + "," +
                        Common.Common.FieldTIFLatMax + "," +
                        Common.Common.FieldTIFMapNumber + "," +
                        Common.Common.FieldTIFLastModified +
                    ")" +
                    " VALUES " +
                    "(" +
                         retVal.ToString() + "," +
                         "'" + division + "'" + "," +
                         "'" + district + "'" + "," +
                         LongMin + "," +
                         LongMax + "," +
                         LatMin + "," +
                         LatMax + "," +
                         "'" + mapNumber + "'" + "," +
                         SYS_DATE +
                    ")";

                //trans = pDBConnection.BeginTransaction();
                pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                //pCmd.Transaction = trans;
                pCmd.CommandText = sql;
                recordsUpdated = pCmd.ExecuteNonQuery();
                //trans.Commit();
                //trans.Dispose();
                if (recordsUpdated != 0)
                {
                    Log("InsertTIFFInformation - insert succeeded!", LogType.Debug, null);
                    return;
                }
                else
                    throw new Exception("Error updating TIFF information in table: " + Common.Common.TableTIFUnifiedGrid);
            }
            catch (Exception ex)
            {
                //Do not throw an error here
                Log("Entering Error Handler for InsertTIFFInformation", LogType.Error, ex);
                Log("Error updating TIFF information in table: " + Common.Common.TableTIFUnifiedGrid, 
                    LogType.Error, ex);
            }
        }

        /// <summary>
        /// Returns the SDE Reigistry Id of the passed table 
        /// </summary>
        /// <param name="pDBConnection"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        private int GetSDETableRegistryId(string tablename)
        {
            try
            {
                const string REGISTRY_TABLE = "SDE.TABLE_REGISTRY";
                const string REGID_FIELD = "REGISTRATION_ID";
                const string TABLENAME_FIELD = "TABLE_NAME";
                int regId = -1;
                
                //Cannot be full qualified 
                if (tablename.IndexOf('.') != -1)
                    tablename = tablename.Remove(0, (tablename.IndexOf('.') + 1)).ToUpper();
                string sql =
                        "SELECT" + " " + REGID_FIELD +
                        " " + "FROM" + " " +
                        REGISTRY_TABLE +
                        " " + "WHERE" + " " +
                        TABLENAME_FIELD + " = " + "'" + tablename + "'";
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                OracleDataReader reader = pCmd.ExecuteReader();
                if (reader.Read())
                {
                    regId = reader.GetInt32(reader.GetOrdinal(REGID_FIELD));
                }
                return regId; 
            }
            catch
            {
                throw new Exception("Error querying registry Id for table: " + tablename); 
            }
        }

        /// <summary>
        /// Sets up the change detectiongrids table placing one record 
        /// for every map that has to be exported 
        /// </summary>
        /// <param name="pDBConnection"></param>
        public void InsertChangeDetectionMaps(string configurationFile)
        {
            try
            {
                //First get a hashtable of all mapnumbers to process
                Log("Entering InsertChangeDetectionMaps", MapProductionHelper.LogType.Info, null);
                OracleCommand pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                string sql = ""; 
                string mapType = ""; 
                string mapScale = "";
                int recordsAffected = 0; 
                Hashtable hshMaps = new Hashtable(); 

                //Get the list of maps from the configuration file
                hshMaps = GetMapsFromConfigurationFile(configurationFile);
                
                //Get the name of the oid sequence                 
                int regId = GetSDETableRegistryId(
                    Common.Common.TableTIFUnifiedGrid);
                string OIdSequenceName = "EDGIS.R" + regId.ToString();
                pCmd = _pConn.CreateCommand(); 
                                                                                                
                //Get the list of all maps of unique scale to be exported  
                //Do and upsert for each map into PGE_CHANGEDETECTIONGRIDS 
                //int insertCounter = 0; 
                StringBuilder builder;

                //Generate the insert statement syntax
                builder = new StringBuilder();
                builder.Append("INSERT INTO" + " " + Common.Common.TableChangeDetection + " ");
                builder.Append("(");
                builder.Append(     Common.Common.FieldChangeDetectionMapNumber + "," + " ");
                builder.Append(     Common.Common.FieldChangeDetectionOId + "," + " ");                
                builder.Append(     Common.Common.FieldChangeDetectionMapType + "," + " ");
                builder.Append(     Common.Common.FieldChangeDetectionExportState + "," + " ");
                builder.Append(     Common.Common.FieldChangeDetectionScale + " ");
                builder.Append(")");
                builder.Append("("  + " ");
                builder.Append(     "SELECT" + " " + Common.Common.TableChangeDetection + "." + Common.Common.FieldChangeDetectionMapNumber + "," + " ");
                builder.Append(     OIdSequenceName + "." + "NEXTVAL" + "," + " ");
                builder.Append(     "'" + "{0}" + "'" + "," + " ");
                builder.Append(     "'" + ((int)Common.Common.ExportStates.ReadyToExport).ToString() + "'" + "," + " ");
                builder.Append(     "{1}" + " ");
                builder.Append(     "FROM" + " " + Common.Common.TableChangeDetection + " "); 
                builder.Append(     "WHERE" + " "); 
                builder.Append(         Common.Common.FieldChangeDetectionMapType + " " + "IS NULL" + " " + "And" + " ");
                builder.Append(         Common.Common.FieldChangeDetectionScale + " = " + "{1}" + " " + "And" + " ");
                builder.Append(         Common.Common.FieldChangeDetectionMapNumber + " " + "NOT IN");
                builder.Append(         "(");
                builder.Append(             "SELECT" + " " + Common.Common.FieldChangeDetectionMapNumber + " " + "FROM" + " " + Common.Common.TableChangeDetection + " ");
                builder.Append(             "WHERE" + " "); 
                builder.Append(                 Common.Common.FieldChangeDetectionMapType + " = " + "'{0}'" + " " + "And" + " "); 
                builder.Append(                 Common.Common.FieldChangeDetectionScale + " = " + "{1}");  
                builder.Append(         ")");
                builder.Append(")");
                string insertStatement = builder.ToString();
                
                //Generate the update statement syntax
                builder = new StringBuilder();
                builder.Append("UPDATE" + " " + Common.Common.TableChangeDetection + " ");
                builder.Append("SET" + " "); 
                builder.Append(Common.Common.FieldChangeDetectionExportState + " = " + "'" + 
                    ((int)Common.Common.ExportStates.ReadyToExport).ToString() + "'" + "," + " ");
                builder.Append(Common.Common.FieldChangeDetectionFailureCount + " = " + "0" + " ");
                builder.Append("WHERE" + " "); 
                builder.Append(     Common.Common.FieldChangeDetectionMapType + " = " + "'{0}'" + " " + "And" + " ");
                builder.Append(     Common.Common.FieldChangeDetectionScale + " = " + "{1}" + " " + "And" + " ");  
                builder.Append(     Common.Common.FieldChangeDetectionMapNumber + " " + "IN"); 
                builder.Append(         "(");
                builder.Append(             "SELECT" + " " + Common.Common.FieldChangeDetectionMapNumber + " ");
                builder.Append(             "FROM" + " " + Common.Common.TableChangeDetection + " "); 
                builder.Append(             "WHERE" + " ");
                builder.Append(                 "MAP_TYPE IS NULL"); 
                builder.Append(         ")");
                string updateStatement = builder.ToString();

                //Perform the inserts and updates for the maps to be exported                                 
                foreach (string mapTypeScale in hshMaps.Keys)
                {
                    //Loop through each map type scale combination 
                    string[] mapTypeScaleArr = mapTypeScale.Split('*');
                    mapType = mapTypeScaleArr[0];
                    mapScale = mapTypeScaleArr[1];

                    //perform the insert statement
                    Log("Inserting into change detection table for maptype: " + mapType + " scale: " + mapScale, MapProductionHelper.LogType.Info, null);
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = string.Format(insertStatement, mapType, mapScale);
                    recordsAffected = pCmd.ExecuteNonQuery();
                    Log("Inserted: " + recordsAffected.ToString() + " maps", MapProductionHelper.LogType.Info, null);

                    //perform the update statement
                    Log("Updating change detection table for maptype: " + mapType + " scale: " + mapScale, MapProductionHelper.LogType.Info, null);
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = string.Format(updateStatement, mapType, mapScale);
                    recordsAffected = pCmd.ExecuteNonQuery();
                    Log("Updated: " + recordsAffected.ToString() + " maps", MapProductionHelper.LogType.Info, null);
                }

                //Find any maps that have a status of failed or in progress 
                //and reset their status to ready to process
                Log("Setting Failed or In-Progress maps to Ready to Export", MapProductionHelper.LogType.Info, null);
                builder = new StringBuilder();
                builder.Append("UPDATE ");
                builder.Append(Common.Common.TableChangeDetection);
                builder.Append(" SET ");
                builder.Append(Common.Common.FieldChangeDetectionExportState + " = " + 
                    ((int)Common.Common.ExportStates.ReadyToExport).ToString());
                builder.Append(" WHERE ");
                builder.Append(Common.Common.FieldChangeDetectionExportState);
                builder.Append(" IN ");
                builder.Append("(");
                builder.Append(     ((int)Common.Common.ExportStates.Error).ToString() + ",");
                builder.Append(     ((int)Common.Common.ExportStates.InProgress).ToString());
                builder.Append(")");
                pCmd.Connection = _pConn;
                pCmd.CommandText = builder.ToString();
                recordsAffected = pCmd.ExecuteNonQuery();
                Log(recordsAffected.ToString() +
                    " maps switched from status Failed or In Progress to Ready to Process", MapProductionHelper.LogType.Info, null);

                //Update all maps of status Processed, LocationInformationMissing or   
                //RequiredDataMissing to a status of Idle so that 
                //we can distinguish between maps processed in the most recent run 
                //and those processed in previous runs 
                Log("Setting Success, LocationInformationMissing, RequiredDataMissing to Idle", MapProductionHelper.LogType.Info, null);
                builder = new StringBuilder();
                builder.Append("UPDATE ");
                builder.Append(Common.Common.TableChangeDetection);
                builder.Append(" SET ");
                builder.Append(Common.Common.FieldChangeDetectionExportState + " = " +
                    ((int)Common.Common.ExportStates.Idle).ToString());
                builder.Append(" WHERE ");
                builder.Append(Common.Common.FieldChangeDetectionExportState);
                builder.Append(" IN ");
                builder.Append("(");
                builder.Append(((int)Common.Common.ExportStates.Processed).ToString() + ",");
                builder.Append(((int)Common.Common.ExportStates.LocationInformationMissing).ToString() + ","); 
                builder.Append(((int)Common.Common.ExportStates.RequiredDataMissing).ToString());
                builder.Append(")");
                pCmd.Connection = _pConn;
                pCmd.CommandText = builder.ToString();
                recordsAffected = pCmd.ExecuteNonQuery();
                Log(recordsAffected.ToString() +
                    " maps switched from status Success, LocationInformationMissing, RequiredDataMissing to Idle", MapProductionHelper.LogType.Info, null);

                //Delete existing change detection rows 
                Log("Deleting maps where map_type is null", MapProductionHelper.LogType.Info, null);
                sql = "DELETE" + " " + "FROM" + " " + Common.Common.TableChangeDetection +
                    " " + "WHERE" + " " + Common.Common.FieldChangeDetectionMapType + " IS NULL";
                pCmd = _pConn.CreateCommand();
                pCmd.Connection = _pConn;
                pCmd.CommandText = sql;
                recordsAffected = pCmd.ExecuteNonQuery();

                Log("Leaving InsertChangeDetectionMaps", MapProductionHelper.LogType.Info, null); 
            }
            catch (Exception ex)
            {
                //Do not throw an error here 
                throw new Exception("Error inserting change detection maps: " + ex.StackTrace); 
            }
        }        
        
        private void GetLatLong(IEnvelope mapExtent, IFeature mapGrid, out double LongMin, out double LongMax, out double LatMin, out double LatMax)
        {
            //Determine the max,min values for the features envelope
            IEnvelope featEnvelope = mapExtent;
            double xMin = featEnvelope.XMin;
            double xMax = featEnvelope.XMax;
            double yMin = featEnvelope.YMin;
            double yMax = featEnvelope.YMax;

            LongMin = xMin;
            LongMax = xMax;
            LatMin = yMin;
            LatMax = yMax;
        }
    }
}
