using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Common;
using System.Data;
using System.Text.RegularExpressions;

using Oracle.DataAccess.Client;

namespace PGE.Interface.Integration.DMS.Manager
{
    /// <summary>
    /// Class for abstracting the reading and writing of process control records. The main purpose of the control table is to
    /// determine when all of the processes have finished
    /// </summary>
    public class ControlTable
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private OracleConnection _oracleConnection;
        private string _connectionString = "";
        /// <summary>
        /// Initialize the Control Table. Creates the OracleConnection from config
        /// </summary>
        public ControlTable(string connectionString)
        {
            _connectionString = connectionString;
            _oracleConnection = new OracleConnection(connectionString);
        }

        private OracleConnection _OracleConnection
        {
            get
            {
                if (_oracleConnection == null) { _oracleConnection = new OracleConnection(_connectionString); }
                return _oracleConnection;
            }
            set
            {
                _oracleConnection = value;
            }
        }

        private void OpenConnection()
        {
            if (_OracleConnection.State != ConnectionState.Open) { _OracleConnection.Open(); }
            using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = " + (Configuration.getIntSetting("BulkCopyTimeout",60) * 3), _OracleConnection))
            {
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }
        
        /// <summary>
        /// Get the control record for a particular process
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public string GetBatchID()
        {
            string batchID = "";
            string sql = "SELECT distinct(BATCHID) FROM DMSSTAGING.PGE_DMS_TO_PROCESS";
            try
            {
                using (OracleCommand cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["BATCHID"];
                            if (value != null && !(value is DBNull))
                            {
                                batchID = value.ToString();
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
            }
            return batchID;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool AreProcessesFinished()
        {
            bool AreProcessesFinished = true;
            string sql = "SELECT CIRCUITSTATUS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS is null or CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress);
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            AreProcessesFinished = false;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return AreProcessesFinished;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool CircuitsStillToProcess()
        {
            bool circuitsStillToProcess = false;
            string sql = "SELECT CIRCUITSTATUS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS IS NULL or CIRCUITSTATUS = " + ((int)CircuitStatus.Retry);
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            circuitsStillToProcess = true;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return circuitsStillToProcess;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public Dictionary<string, List<string>> CircuitsByStatusAndServerName(string serverName, CircuitStatus status)
        {
            Dictionary<string, List<string>> circuits = new Dictionary<string, List<string>>();
            string sql = "SELECT CIRCUITIDS,PROCESSORID FROM DMSSTAGING.PGE_DMS_TO_PROCESS where SERVERNAME = '" + serverName + "' and CIRCUITSTATUS = " + ((int)status);
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            if (!circuits.ContainsKey(dataReader["PROCESSORID"].ToString()))
                            {
                                circuits.Add(dataReader["PROCESSORID"].ToString(), new List<string>());
                            }
                            circuits[dataReader["PROCESSORID"].ToString()].Add(dataReader["CIRCUITIDS"].ToString());
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return circuits;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public void CircuitsByStatusAndServerName(string serverName, CircuitStatus status, ref int totalProcessed, ref int totalFeaturesProcessed)
        {
            string sql = "SELECT FEATURESPROCESSED FROM DMSSTAGING.PGE_DMS_TO_PROCESS where SERVERNAME = '" + serverName + "' and CIRCUITSTATUS = " + ((int)status);
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["FEATURESPROCESSED"];
                            if (value != null && !(value is DBNull))
                            {
                                totalFeaturesProcessed += Int32.Parse(value.ToString());
                            }
                            totalProcessed++;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return;
        }

        /// <summary>
        /// Get the OID of the electric stitch point related to the subelectric stitch point
        /// </summary>
        /// <param name="substationOID">ObjectID of the substation</param>
        /// <returns>Returns the ObjectID of the related electric stitch point</returns>
        public int GetRelatedElectricStitchPoint(int substationOID)
        {
            int relatedElecStitchPointOID = -1;
            string sql = "SELECT ELECOID FROM DMSSTAGING.PGE_SUBSTITCH_ELECSTITCH_MAP where SUBOID = " + substationOID;
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["ELECOID"];
                            if (value != null && !(value is DBNull))
                            {
                                relatedElecStitchPointOID = Int32.Parse(value.ToString());
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error determining related electric stitch point. SQL: " + sql, ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return relatedElecStitchPointOID;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public int CircuitCountByStatus(CircuitStatus status, bool checkSubstation, bool findNull)
        {
            int count = 0;
            string sql = "SELECT CIRCUITSTATUS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS = " + ((int)status);
            if (findNull) { sql = "SELECT CIRCUITSTATUS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS is null"; }
            if (checkSubstation) { sql += " and SUBSTATIONORCIRCUIT = 'S'"; }
            else { sql += " and SUBSTATIONORCIRCUIT = 'C'"; }
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            count++;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return count;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public int CircuitCount()
        {
            int count = 0;
            string sql = "SELECT CIRCUITSTATUS FROM DMSSTAGING.PGE_DMS_TO_PROCESS";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            count++;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return count;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public string GetNextCircuitToProcess(string serverName, string processID, bool getSubstation, bool getRetry)
        {
            OracleTransaction tr = null;
            string circuitToProcess = "";
            string sql = "SELECT CIRCUITIDS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where SUBSTATIONORCIRCUIT = 'S'";
            if (!getSubstation) { sql = "SELECT CIRCUITIDS FROM DMSSTAGING.PGE_DMS_TO_PROCESS where SUBSTATIONORCIRCUIT = 'C'"; }
            if (!getRetry) { sql += " and CIRCUITSTATUS is null"; }
            else { sql += " and CIRCUITSTATUS = " + ((int)CircuitStatus.Retry); }
            try
            {
                OpenConnection();
                tr = _OracleConnection.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE DMSSTAGING.PGE_DMS_TO_PROCESS IN EXCLUSIVE MODE", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                string circuit = "";
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["CIRCUITIDS"];
                            if (value != null && !(value is DBNull))
                            {
                                circuit = value.ToString();
                                break;
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }

                if (!string.IsNullOrEmpty(circuit))
                {
                    using (var cmd = new OracleCommand("UPDATE PGE_DMS_TO_PROCESS SET SERVERNAME = '" + serverName + "',PROCESSORID = '" + processID + "',CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress) + " where" +
                        " CIRCUITIDS = '" + circuit + "'", _OracleConnection))
                    {
                        cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    circuitToProcess = circuit;
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
                if (tr != null) { tr.Dispose(); }
            }
            return circuitToProcess;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool CanRunExport(string serverName)
        {
            bool canRunExport = true;
            OracleTransaction tr = null;
            string sql = "SELECT EXPORTEDDATA FROM DMSSTAGING.PGE_DMS_PROCESSES_RUNNING";
            try
            {
                OpenConnection();
                tr = _OracleConnection.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE DMSSTAGING.PGE_DMS_PROCESSES_RUNNING IN EXCLUSIVE MODE", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["EXPORTEDDATA"];
                            if (value != null && !(value is DBNull))
                            {
                                if (!string.IsNullOrEmpty(value.ToString()))
                                {
                                    canRunExport = false;
                                }
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
                string exportedString = "Yes";
                if (!canRunExport) { exportedString = "No"; }
                using (var cmd = new OracleCommand("UPDATE DMSSTAGING.PGE_DMS_PROCESSES_RUNNING SET EXPORTEDDATA = '" + exportedString + "' where SERVERNAME = '" + serverName + "'", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
                if (tr != null) { tr.Dispose(); }
            }
            return canRunExport;
        }

        /// <summary>
        /// Inserts the circuits that will be processed by the children processes as well as the necessary relationships defined
        /// between substation and electric stitch points
        /// </summary>
        /// <param name="substations">List of substation IDs to insert</param>
        /// <param name="circuits">List of electric circuits to insert</param>
        /// <param name="SubStitchToElecStitchRelationships">Substation OID to Electric OID map</param>
        /// <param name="batchID">ID of batch process</param>
        /// <returns></returns>
        public bool InsertCircuits(List<string> substations, List<string> circuits, 
            Dictionary<int,int> SubStitchToElecStitchRelationships, ref string batchID)
        {
            // #10 
            OracleTransaction tr = null;
            bool alreadyinserted = false;
            string sql = "SELECT PROCESSORID FROM DMSSTAGING.PGE_DMS_TO_PROCESS";
            try
            {
                OpenConnection();
                tr = _OracleConnection.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE DMSSTAGING.PGE_DMS_TO_PROCESS IN EXCLUSIVE MODE", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            alreadyinserted = true;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
                if (!alreadyinserted)
                {
                    batchID = Guid.NewGuid().ToString();
                    //Now let's list all of our substation IDs and circuit IDs into our PGE_DMS_TO_PROCESS table
                    foreach (string circuit in substations)
                    {
                        InsertCircuitsForProcessing("S", circuit, batchID, tr);
                    }

                    foreach (string circuit in circuits)
                    {
                        InsertCircuitsForProcessing("C", circuit, batchID, tr);
                    }

                    //Now insert our list of defined relationships between substich points and elecstitchpoints
                    foreach(KeyValuePair<int, int> kvp in SubStitchToElecStitchRelationships)
                    {
                        InsertRelationshipsForProcessing(kvp.Key, kvp.Value, tr);
                    }
                }
                else
                {
                    batchID = GetBatchID();
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _log4.Error("Error inserting circuit records.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
                if (tr != null) { tr.Dispose(); }
            }
            return alreadyinserted;
        }

        /// <summary>
        /// Insert a new process record. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void InsertProcessIDNotification(int maximumProcessors, string serverName)
        {
            //3
            int rowsChanged = 0;
            OracleTransaction tr = null;
            string insertsql = "INSERT into DMSSTAGING.PGE_DMS_PROCESSES_RUNNING" +
                " (AVAILABLEPROCESSORS,SERVERNAME,CIRCUITSPROCESSED) " +
                "VALUES (" + maximumProcessors + ", '" + serverName + "'" + ",0)";
            try
            {
                OpenConnection();
                tr = _OracleConnection.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE DMSSTAGING.PGE_DMS_PROCESSES_RUNNING IN EXCLUSIVE MODE", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                bool clearStagingTable = false;
                using (var cmd = new OracleCommand("Select SERVERNAME from DMSSTAGING.PGE_DMS_PROCESSES_RUNNING", _OracleConnection))
                {
                    cmd.Transaction = tr;
                    cmd.CommandType = CommandType.Text;
                    bool recordsExist = false;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["SERVERNAME"];
                            if (value != null && !(value is DBNull))
                            {
                                if (value.ToString() == serverName)
                                {
                                    clearStagingTable = true;
                                }
                            }
                            recordsExist = true;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    if (!recordsExist)
                    {
                        clearStagingTable = true;
                    }
                }

                if (clearStagingTable)
                {
                    using (var cmd = new OracleCommand("DMSSTAGING.CLEANUPDMS", _OracleConnection))
                    {
                        cmd.Transaction = tr;
                        cmd.CommandType = CommandType.StoredProcedure;
                        rowsChanged = (int)cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    /*Changes for ENOS to SAP migration - DMS ..*/
                    _log4.Info("Executing procedure : DMSSTAGING.CLEANUPDMS");
                }

                using (var cmd = new OracleCommand(insertsql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _log4.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
                if (tr != null) { tr.Dispose(); }
            }
        }

        
        private static Dictionary<int, string> ClassIDToClassNameMap = new Dictionary<int, string>();
        /// <summary>
        /// Insert a new process record. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public string GetObjectClassName(int objectClassID)
        {
            if (ClassIDToClassNameMap.ContainsKey(objectClassID)) { return ClassIDToClassNameMap[objectClassID]; }

            string objectClassName = "";
            string sql = "select PHYSICALNAME from sde.gdb_items where objectid = " + objectClassID;
            if (!String.IsNullOrEmpty(Configuration.getSetting("GeoMart_Exception")))
                sql += Configuration.getSetting("GeoMart_Exception");
            try
            {
                OpenConnection();

                /*Changes for ENOS to SAP migration - DMS ..Start */
                //_log4.Info(sql);
                /*Changes for ENOS to SAP migration - DMS ..End */

                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["PHYSICALNAME"];
                            if (value != null && !(value is DBNull))
                            {
                                objectClassName = value.ToString();
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                objectClassName = objectClassID.ToString();
                _log4.Error("Error obtaining object class name.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            ClassIDToClassNameMap.Add(objectClassID, objectClassName);
            return objectClassName;
        }

        /// <summary>
        /// Obtains the object class ID from the physical name provided
        /// </summary>
        ///<param name="className">Name of class including schema owner (i.e. EDGIS.TRANSFORMER)</param>
        ///<returns>Object class ID of specified class</returns>
        public int GetObjectClassID(string className)
        {
            foreach (KeyValuePair<int,string> kvp in ClassIDToClassNameMap)
            {
                if (kvp.Value == className)
                {
                    return kvp.Key;
                }
            }

            int objectID = -1;
            string sql = "select OBJECTID from sde.gdb_items where PHYSICALNAME = '" + className.ToUpper() + "'";
            if (!String.IsNullOrEmpty(Configuration.getSetting("GeoMart_Exception")))
                sql += Configuration.getSetting("GeoMart_Exception");
            try
            {
                OpenConnection();

                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["OBJECTID"];
                            if (value != null && !(value is DBNull))
                            {
                                objectID = Int32.Parse(value.ToString());
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
                if (objectID > 0)
                {
                    ClassIDToClassNameMap.Add(objectID, className.ToUpper());
                }
                else
                {
                    _log4.Info("Unable to find specified object class ID for physical name: " + className.ToUpper());
                    _log4.Info(sql);
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error obtaining object class ID for : " + className + " SQL: " + sql, ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            
            return objectID;
        }


        public static List<string> SettingsErrors = new List<string>();

        /// <summary>
        /// Determines the value associated with the provided global ID from the table with the specified column name from
        /// the PGEData schema. Uses the PGEDATA.PGEDATA_SM_TABLE_LOOKUP table for domain mappings
        /// </summary>
        /// <param name="globalID">Global ID of feature</param>
        /// <param name="tableName">PGEData table name</param>
        /// <param name="columnName">PGEData table column name</param>
        /// <param name="deviceName">PGEData data device name from PGEDATA_SM_TABLE_LOOKUP (if applicable)</param>
        /// <returns>Returns the string value from the PGEData table (domain description if applicable)</returns>
        public string GetSettingsValue(string globalID, string tableName,
            string columnName, string deviceName)
        {
            bool foundCodedValue = false;
            bool foundDomainDescription = false;
            string returnValue = "";
            string selectCodeSQL = "";
            string selectDescriptionSQL = "";


            string sql = "select " + columnName + " from " + tableName + " where GLOBAL_ID = '" + globalID + "'";
            selectCodeSQL = sql;
            try
            {
                OpenConnection();
                //First need to determine the controller type from the correct table.
                using (OracleCommand cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader[columnName];
                            if (value != null && !(value is DBNull))
                            {
                                returnValue = value.ToString();                                
                            }
                            foundCodedValue = true;
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }

                if (!string.IsNullOrEmpty(deviceName))
                {
                    //Now that we have the controller type we have to look up the description for that controller type
                    sql = "select DESCRIPTION from PGEDATA.PGEDATA_SM_TABLE_LOOKUP where UPPER(FIELD_NAME) = '" + columnName.ToUpper() + "' AND UPPER(DEVICE_NAME) = '" +
                        deviceName.ToUpper() + "' AND UPPER(CODE) = '" + returnValue.ToUpper() + "'";
                    selectDescriptionSQL = sql;
                    using (OracleCommand cmd = new OracleCommand(sql, _OracleConnection))
                    {
                        cmd.CommandType = CommandType.Text;

                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                object value = dataReader["DESCRIPTION"];
                                if (value != null && !(value is DBNull))
                                {
                                    returnValue = value.ToString();
                                    foundDomainDescription = true;
                                }
                            }
                            dataReader.Close();
                            dataReader.Dispose();
                        }
                        cmd.Dispose();
                    }
                }
                else
                {
                    foundDomainDescription = true;
                }
            }
            catch (Exception ex)
            {
                SettingsErrors.Add("Error determining controller type from PGDData schema: " + sql + ": " + ex.Message);
                //_log4.Error("Error determining controller type from PGDData schema: " + sql, ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }

            if (!foundCodedValue) { SettingsErrors.Add("Unable to determine value from PGEData schema: " + selectCodeSQL); }
            if (!string.IsNullOrEmpty(returnValue) && !foundDomainDescription)
            { SettingsErrors.Add("Unable to determine domain description from PGEData schema: " + selectDescriptionSQL); }

            return returnValue;
        }

        /// <summary>
        /// Returns the control_type field from the associated PGEData table for a given subtype and globalID.
        /// </summary>
        /// <param name="subtypeCode"></param>
        /// <param name="globalID"></param>
        /// <returns></returns>
        public string GetControllerType(string subtypeCode, string globalID)
        {
            bool foundController = false;
            bool foundControllerDomainDescription = false;
            string controllerType = "";
            string tableName = "";
            string columnName = "CONTROL_TYPE";
            string deviceName = "";
            string selectControllerType = "";
            string selectControllerTypeDescription = "";
            
            switch (subtypeCode)
            {
                case "1":
                    tableName = "PGEDATA.PGEDATA_SM_CAPACITOR_EAD"; 
                    deviceName = "SM_CAPACITOR";                    
                    break;
                case "2":
                    tableName = "PGEDATA.PGEDATA_SM_REGULATOR_EAD"; 
                    deviceName = "SM_REGULATOR";
                    break;
                case "3":
                    tableName = "PGEDATA.PGEDATA_SM_RECLOSER_EAD"; 
                    deviceName = "SM_RECLOSER";
                    break;
                case "4":
                    tableName = "PGEDATA.PGEDATA_SM_INTERRUPTER_EAD"; 
                    deviceName = "SM_INTERRUPTER";
                    break;
                case "5":
                    tableName = "PGEDATA.PGEDATA_SM_SECTIONALIZER_EAD"; 
                    deviceName = "SM_SECTIONALIZER";
                    break;
                case "6":
                    tableName = "PGEDATA.PGEDATA_SM_SWITCH_EAD"; 
                    columnName = "CONTROL_UNIT_TYPE";
                    deviceName = "SM_SWITCH";
                    break;
            }

            string sql = "select " + columnName + " from " + tableName + " where GLOBAL_ID = '" + globalID + "'";
            selectControllerType = sql;
            try
            {
                OpenConnection();
                //First need to determine the controller type from the correct table.
                using (OracleCommand cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader[columnName];
                            if (value != null && !(value is DBNull))
                            {
                                controllerType = value.ToString();
                                foundController = true;
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }

                //Now that we have the controller type we have to look up the description for that controller type
                sql = "select DESCRIPTION from PGEDATA.PGEDATA_SM_TABLE_LOOKUP where UPPER(FIELD_NAME) = '" + columnName.ToUpper() + "' AND UPPER(DEVICE_NAME) = '" + 
                    deviceName.ToUpper() + "' AND UPPER(CODE) = '" + controllerType.ToUpper() + "'";
                selectControllerTypeDescription = sql;
                using (OracleCommand cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object value = dataReader["DESCRIPTION"];
                            if (value != null && !(value is DBNull))
                            {
                                controllerType = value.ToString();
                                foundControllerDomainDescription = true;
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                SettingsErrors.Add("Error determining controller type from PGDData schema: " + sql + ": " + ex.Message);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }

            if (!foundController) { SettingsErrors.Add("Unable to determine controller type for controller: " + selectControllerType); }
            if (!string.IsNullOrEmpty(controllerType) && !foundControllerDomainDescription)
            { SettingsErrors.Add("Unable to determine controller type domain description: " + selectControllerTypeDescription); }

            return controllerType;
        }

        /// <summary>
        /// Insert a new control record. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void InsertCircuitsForProcessing(string substationOrCircuit, string circuits, string batchID, OracleTransaction tr)
        {
            int rowsChanged = 0;
            string insertsql = "INSERT into DMSSTAGING.PGE_DMS_TO_PROCESS" +
                " (CIRCUITIDS,SUBSTATIONORCIRCUIT,BATCHID)  " +
                "VALUES ('" + circuits + "','" + substationOrCircuit + "','" + batchID + "')";
            try
            {
                //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (var cmd = new OracleCommand(insertsql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                
            }
            catch (Exception ex)
            {
                _log4.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Inserts a new record into the substitch to elecstitch relationship table
        /// </summary>
        /// <param name="SubStitchOID">ObjectID of sub stitch point</param>
        /// <param name="ElecStitchOID">ObjectID of elec stitch point</param>
        /// <param name="tr">Transaction to be included with</param>
        public void InsertRelationshipsForProcessing(int SubStitchOID, int ElecStitchOID, OracleTransaction tr)
        {
            int rowsChanged = 0;
            string insertsql = "INSERT into DMSSTAGING.PGE_SUBSTITCH_ELECSTITCH_MAP" +
                " (SUBOID,ELECOID)  " +
                "VALUES ('" + SubStitchOID + "','" + ElecStitchOID + "')";
            try
            {
                //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (var cmd = new OracleCommand(insertsql, _OracleConnection))
                {
                    cmd.Transaction = tr;
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

            }
            catch (Exception ex)
            {
                _log4.Error("Error inserting record into the control table. SQL: " + insertsql, ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Update circuits to begin processing. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void UpdateCircuitsForProcessing(string processorID)
        {
            int rowsChanged = 0;
            string updateSql = "Update DMSSTAGING.PGE_DMS_TO_PROCESS set (PROCESSORID) = ('" + processorID + "') where " +
                "PROCESSORID is null and CIRCUITSTATUS is null AND CIRCUITIDS in (select min(CIRCUITIDS) from DMSSTAGING.PGE_DMS_TO_PROCESS where " +
                "PROCESSORID is null and CIRCUITSTATUS is null)";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(updateSql, _OracleConnection))
                {
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
        }

        /// <summary>
        /// Update circuits to begin processing. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void UpdateOverallServerStatus(string serverName, int circuitsProcessed, int totalFeaturesProcessed)
        {
            int rowsChanged = 0;
            string updateSql = "Update DMSSTAGING.PGE_DMS_PROCESSES_RUNNING set CIRCUITSPROCESSED = " + circuitsProcessed + ",FEATURESPROCESSED = " + totalFeaturesProcessed +
                "where SERVERNAME = '" + serverName + "'";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(updateSql, _OracleConnection))
                {
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
        }

        /// <summary>
        /// Update circuits to begin processing. The process ID should be unique
        /// </summary>
        /// <param name="substationOrCircuit">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void UpdateCircuitsAsFinished(string circuitIDs, double timeToProcessInMinutes, CircuitStatus status, string error)
        {
            int featuresProcessed = CADOPS.TotalFeaturesProcessed;
            int rowsChanged = 0;
            string updateSql = "Update DMSSTAGING.PGE_DMS_TO_PROCESS set CIRCUITSTATUS = " + ((int)status) + ",MINUTESTOPROCESS = " + timeToProcessInMinutes + ",ERROR = '" +
                error + "',FEATURESPROCESSED = " + featuresProcessed + " where CIRCUITIDS = '" + circuitIDs + "'";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(updateSql, _OracleConnection))
                {
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
        }

        /// <summary>
        /// This method will return the list of other servers that are currently running ED050 with work they are currently working on
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetOtherMachineProcessesCurrentlyRunning()
        {
            Dictionary<string, int> processesCurrentlyRunning = new Dictionary<string, int>();
            string sql = "SELECT ProcessorID FROM DMSSTAGING.PGE_DMS_TO_PROCESS where CIRCUITSTATUS is null and PROCESSORID is not null";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, _OracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        string processID = "";
                        while (dataReader.Read())
                        {
                            object value = dataReader["PROCESSORID"];
                            if (value != null && !(value is DBNull))
                            {
                                processID = value.ToString();
                                if (!processesCurrentlyRunning.ContainsKey(processID))
                                {
                                    processesCurrentlyRunning.Add(processID, 0);
                                }
                                processesCurrentlyRunning[processID] = processesCurrentlyRunning[processID] + 1;
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error reading process record from the control table.", ex);
            }
            finally
            {
                _OracleConnection.Close();
                _OracleConnection.Dispose();
                _OracleConnection = null;
            }
            return processesCurrentlyRunning;
        }

        /// <summary>
        /// Run the CLEANUPDMS Oracle stored procedure
        /// </summary>
        public void ClearStagingTable()
        {
            PGE.Interface.Integration.DMS.Common.Oracle.ExecuteProcedure(Configuration.CadopsConnection, "DMSSTAGING.CLEANUPDMS");
        }
        /// <summary>
        /// Run the REMOVE_DUPLICATES Oracle stored procedure
        /// </summary>
        public void RemoveDuplicates()
        {
            PGE.Interface.Integration.DMS.Common.Oracle.ExecuteProcedure(Configuration.CadopsConnection, "DMSSTAGING.REMOVE_DUPLICATES");
        }
        /// <summary>
        /// Run the UPDATE_SCHEM_XY Oracle stored procedure
        /// </summary>
        public void UpdateSchemXY()
        {
            //This is no longer necessary as we are directly connecting to the schematics database and obtaining the xy data and pathing information.
            //PGE.Interface.Integration.DMS.Common.Oracle.ExecuteProcedure(Configuration.CadopsConnection, "DMSSTAGING.UPDATE_SCHEM_XY");
        }
    }

    public enum CircuitStatus
    {
        InProgress,
        Finished,
        Retry,
        Error
    }
}
