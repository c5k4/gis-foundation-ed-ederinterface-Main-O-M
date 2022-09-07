using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using System.Data;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PGE_Cached_Tracing
{
    public class OracleConnectionControl
    {
        private string OracleConnectionString = "";
        private static Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "CachedTracing.log4net.config");

        public OracleConnectionControl(string commaSeparatedOracleConnection)
        {
            // m4jf edgisrearch 919 
            // OracleConnectionString = commaSeparatedOracleConnection;
            OracleConnectionString = ReadEncryption.GetConnectionStr(commaSeparatedOracleConnection.ToUpper());
        }

        private string CreateOracleConnectionString(string server, string sid, string user, string pass)
        {
            string connection = connection = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));", server, sid);
            connection += String.Format("User Id={0};Password={1};", user, pass);
            return connection;
        }

        private OracleConnection _oracleConnection;
        private OracleConnection OracleConn
        {
            get
            {
                if (_oracleConnection == null) 
                {
                    // m4jf edgisrearch 919 - get connection string using Passwordmanagement tool.
                    //string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
                    //string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
                    string connection = OracleConnectionString;

                     _oracleConnection = new OracleConnection(connection); 
                }
                return _oracleConnection;
            }
            set
            {
                _oracleConnection = value;
            }
        }

        private void OpenConnection()
        {
            try
            {
                if (OracleConn.State != ConnectionState.Open) { OracleConn.Open(); }
                using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = 60", OracleConn))
                {
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to connect to database: " + OracleConnectionString + " Error: " + ex.Message);
                throw ex;
            }
        }

        public void BulkLoadData(DataSet set)
        {
            // m4jf edgisrearch 919 - get connection string using Passwordmanagement tool.
            // string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            // string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
            string connection = OracleConnectionString;

            try
            {
                OpenConnection();
                using (var copy = new OracleBulkCopy(OracleConn))
                {
                    //copy.DestinationTableName = tableName;
                    //copy.BulkCopyTimeout = DefaultTimeoutInSeconds;
                    //copy.BatchSize = BatchSize;
                    //copy.Insert(entities);
                    foreach (DataTable table in set.Tables)
                    {
                        //_log.Debug("Loading Table " + table.TableName);
                        copy.DestinationTableName = table.TableName;
                        copy.WriteToServer(table);
                    }
                    copy.Close();
                    copy.Dispose();
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Loading Data.  " + System.Environment.NewLine + ex.ToString());
                throw ex;
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
        }


        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool InsertCircuits(List<string> circuits, CircuitType circuitType)
        {
            OracleTransaction tr = null;
            bool alreadyinserted = false;
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE_CachedTrace_ToProcess IN EXCLUSIVE MODE", OracleConn))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                //Now let's list all of our substation IDs and circuit IDs into our PGE_DMS_TO_PROCESS table
                foreach (string circuit in circuits)
                {
                    InsertCircuitsForProcessing(circuitType, circuit, tr);
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error inserting circuit records.", ex);
            }
            finally
            {
                OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
                if (tr != null) { tr.Dispose(); }
            }
            return alreadyinserted;
        }


        /// <summary>
        /// Insert into the PGE_FEEDERFEDNETWORK_MAP the list of circuits based on mapping.
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public void InsertCircuitMap(Dictionary<string, List<string>> dictCircuitMapList)
        {
            OracleTransaction tr = null;            
            int rowsChanged = 0;
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE_FEEDERFEDNETWORK_MAP IN EXCLUSIVE MODE", OracleConn))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                //Now let's list all of our substation IDs and circuit IDs into our PGE_DMS_TO_PROCESS table
                foreach (string strSourceCircuitID in dictCircuitMapList.Keys)
                {
                    List<string> listInsertStatements = new List<string>();
// desc PGE_FEEDERFEDNETWORK_MAP
// Name                 Type
// -----------------  -------------
// FROM_CIRCUITID      NVARCHAR2(9)
// TO_CIRCUITID        NVARCHAR2(9)
                    listInsertStatements.Add("INSERT into EDGIS.PGE_FEEDERFEDNETWORK_MAP" +
                        " (FROM_CIRCUITID,TO_CIRCUITID)  " +
                        "VALUES (null,'" + strSourceCircuitID + "')");
                    foreach (string strChildCircuitID in dictCircuitMapList[strSourceCircuitID])
                    {
                        listInsertStatements.Add("INSERT into EDGIS.PGE_FEEDERFEDNETWORK_MAP" +
                        " (FROM_CIRCUITID,TO_CIRCUITID)  " +
                        "VALUES ('" + strSourceCircuitID + "','" + strChildCircuitID + "')");
                    }
                    foreach (string insertsql in listInsertStatements)
                    {
                        try
                        {
                            _logger.Debug("Oracle Insert to Execute:--- " + insertsql + " --- "); 
                            //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                            OpenConnection();
                            using (var cmd = new OracleCommand(insertsql, OracleConn))
                            {
                                cmd.Transaction = tr;
                                rowsChanged = (int)cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }
                            _logger.Debug("Oracle Insert Executed:--- " + insertsql + " --- which updated " + rowsChanged.ToString() + " rows"); 

                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error inserting record into the EDGIS.PGE_FEEDERFEDNETWORK_MAP table.", ex);
                        }
                        finally
                        {
                        }
                    }
                }
                tr.Commit();
                try
                {
                    string strDeleteSQL = "delete from EDGIS.PGE_FEEDERFEDNETWORK_MAP fm where fm.FROM_CIRCUITID is null and fm.TO_CIRCUITID in (select pfm.TO_CIRCUITID from EDGIS.PGE_FEEDERFEDNETWORK_MAP pfm where pfm.FROM_CIRCUITID is not null group by pfm.TO_CIRCUITID)";
                    _logger.Debug("Oracle Delete to Execute:--- " + strDeleteSQL + " --- "); 
                    //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (var cmd = new OracleCommand(strDeleteSQL, OracleConn))
                    {
                        cmd.Transaction = tr;
                        rowsChanged = (int)cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    _logger.Debug("Oracle Delete Invalid Entries Executed:--- " + strDeleteSQL + " --- which deleted " + rowsChanged.ToString() + " rows");

                }
                catch (Exception ex)
                {
                    _logger.Error("Error inserting record into the EDGIS.PGE_FEEDERFEDNETWORK_MAP table.", ex);
                }
                finally
                {
                }
                try
                {
                    string strDeleteSQL = "delete from edgis.PGE_FEEDERFEDNETWORK_MAP sp1 where rowid not in ( select max(rowid) from edgis.PGE_FEEDERFEDNETWORK_MAP sp2 where NVL(sp1.TO_CIRCUITID,'NONE')=NVL(sp2.TO_CIRCUITID,'NONE') and NVL(sp1.FROM_CIRCUITID,'NONE') =NVL(sp2.FROM_CIRCUITID,'NONE'))";
                    _logger.Debug("Oracle Delete to Execute:--- " + strDeleteSQL + " --- ");
                    //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (var cmd = new OracleCommand(strDeleteSQL, OracleConn))
                    {
                        cmd.Transaction = tr;
                        rowsChanged = (int)cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    _logger.Debug("Oracle Delete Duplicate Entries Executed:--- " + strDeleteSQL + " --- which deleted " + rowsChanged.ToString() + " rows");

                }
                catch (Exception ex)
                {
                    _logger.Error("Error inserting record into the EDGIS.PGE_FEEDERFEDNETWORK_MAP table.", ex);
                }
                finally
                {
                }
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error inserting circuit map records.", ex);
            }
            finally
            {
                OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
                if (tr != null) { tr.Dispose(); }
            }
        }


        /// <summary>
        /// Insert a new control record. The process ID should be unique
        /// </summary>
        /// <param name="CIRCUITTYPE">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void InsertCircuitsForProcessing(CircuitType circuitType, string circuitID, OracleTransaction tr)
        {
            int rowsChanged = 0;
            string insertsql = "INSERT into EDGIS.PGE_CachedTrace_ToProcess" +
                " (FEEDERID,CIRCUITTYPE)  " +
                "VALUES ('" + circuitID + "'," + (int)circuitType + ")";
            try
            {
                //_OracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                OpenConnection();
                using (var cmd = new OracleCommand(insertsql, OracleConn))
                {
                    cmd.Transaction = tr;
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error inserting record into the control table.", ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public void UpdateCircuitStatus(string feederID, string error, CircuitStatus circuitStatus)
        {
            OracleTransaction tr = null;
            string sql = "UPDATE EDGIS.PGE_CachedTrace_ToProcess SET error = '" + error + "',CircuitStatus = " + (int)circuitStatus + " where FEEDERID = '" + feederID + "'";
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE_CachedTrace_ToProcess IN EXCLUSIVE MODE", OracleConn))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                using (var cmd = new OracleCommand(sql, OracleConn))
                {
                    cmd.Transaction = tr;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating circuit status in EDGIS.PGE_CACHEDTRACE_TOPROCESS table.", ex);
                if (tr != null) { tr.Rollback(); }
                throw ex;
            }
            finally
            {
                OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
                if (tr != null) { tr.Dispose(); }
            }
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public bool CanProcessCircuit(string feederID)
        {
            bool isInProgress = true;
            OracleTransaction tr = null;
            string sql = "SELECT CIRCUITSTATUS FROM EDGIS.PGE_CachedTrace_ToProcess where FEEDERID = '" + feederID + "'";
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                try
                {
                    using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE_CachedTrace_ToProcess IN EXCLUSIVE MODE", OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error locking table EDGIS.PGE_CachedTrace_ToProcess.", ex);
                    throw ex;
                }

                try
                {
                    using (var cmd = new OracleCommand(sql, OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.CommandType = CommandType.Text;

                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                object value = dataReader["CIRCUITSTATUS"];
                                if (value != null && !(value is DBNull))
                                {
                                    if (String.IsNullOrEmpty(value.ToString()))
                                    {
                                        isInProgress = false;
                                    }
                                }
                                else
                                {
                                    isInProgress = false;
                                }
                                break;
                            }
                            dataReader.Close();
                            dataReader.Dispose();
                        }
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error determining the current status of circuit from EDGIS.PGE_CachedTrace_ToProcess. SQL: " + sql, ex);
                    throw ex;
                }

                try
                {
                    if (!isInProgress)
                    {
                        sql = "UPDATE EDGIS.PGE_CachedTrace_ToProcess SET CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress) + " where" +
                            " FEEDERID = '" + feederID + "'";
                        using (var cmd = new OracleCommand(sql, OracleConn))
                        {
                            cmd.Transaction = tr;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error updating EDGIS.PGE_CachedTrace_ToProcess with circuits to process. SQL: " + sql, ex);
                    throw ex;
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error determining if circuit can be processed from EDGIS.PGE_CachedTrace_ToProcess.", ex);
                throw ex;
            }
            finally
            {
                OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
                if (tr != null) { tr.Dispose(); }
            }
            return !isInProgress;
        }

        /// <summary>
        /// Get the circuits to process for this processID
        /// </summary>
        /// <param name="processID">The ID of the process</param>
        /// <returns>A control record with details about the process</returns>
        public List<string> GetNextCircuitToProcess(CircuitType circuitType, int count)
        {
            List<string> circuits = new List<string>();
            OracleTransaction tr = null;
            string sql = "SELECT FEEDERID FROM EDGIS.PGE_CachedTrace_ToProcess where CIRCUITSTATUS IS NULL AND CIRCUITTYPE = " + (int)circuitType;
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                try
                {
                    using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE_CachedTrace_ToProcess IN EXCLUSIVE MODE", OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error locking table EDGIS.PGE_CachedTrace_ToProcess.", ex);
                    throw ex;
                }

                try
                {
                    using (var cmd = new OracleCommand(sql, OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.CommandType = CommandType.Text;

                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                object value = dataReader["FEEDERID"];
                                if (value != null && !(value is DBNull))
                                {
                                    string circuit = value.ToString();
                                    circuits.Add(circuit);
                                    if (circuits.Count > count) { break; }
                                }
                            }
                            dataReader.Close();
                            dataReader.Dispose();
                        }
                        cmd.Dispose();
                    }
                }
                catch(Exception ex)
                {
                    _logger.Error("Error obtaining list of circuits from EDGIS.PGE_CachedTrace_ToProcess. SQL: " + sql, ex);
                    throw ex;
                }

                try
                {
                    if (circuits.Count > 0)
                    {
                        string inList = "";
                        for (int i = 0; i < circuits.Count; i++)
                        {
                            if (i == circuits.Count - 1)
                            {
                                inList += "'" + circuits[i] + "'";
                            }
                            else
                            {
                                inList += "'" + circuits[i] + "',";
                            }
                        }
                        sql = "UPDATE EDGIS.PGE_CachedTrace_ToProcess SET CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress) + " where" +
                            " FEEDERID in (" + inList + ")";
                        using (var cmd = new OracleCommand(sql, OracleConn))
                        {
                            cmd.Transaction = tr;
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error updating EDGIS.PGE_CachedTrace_ToProcess with circuits to process. SQL: " + sql, ex);
                    throw ex;
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error reading next circuit to process record from EDGIS.PGE_CachedTrace_ToProcess.", ex);
                throw ex;
            }
            finally
            {
                OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
                if (tr != null) { tr.Dispose(); }
            }
            return circuits;
        }

        public Dictionary<int, List<int>> GetConduitOIDs(ref Dictionary<int, List<int>> conduitOIDs, string tableName, string conduitOIDField, string UGOIDField, string whereInClause)
        {
            string selectCmd = "";
            try
            {
                OpenConnection();
                selectCmd = "select " + conduitOIDField + "," + UGOIDField + " from " + tableName + " where " + UGOIDField + " in (" + whereInClause + ")";
                using (var cmd = new OracleCommand(selectCmd, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int conduitOID = Int32.Parse(reader[0].ToString());
                        int UGOID = Int32.Parse(reader[1].ToString());
                        try { conduitOIDs.Add(UGOID, new List<int>()); }
                        catch { }
                        conduitOIDs[UGOID].Add(conduitOID);
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error obtaining conduit object IDs from: " + tableName, ex);
                throw ex;
            }
            finally
            {
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
            }

            return conduitOIDs;
        }

        public List<string> GetUltimateSourceCircuitsLeft()
        {
            List<string> circuitsNotFinished = new List<string>();
            string selectCmd = "";
            try
            {
                OpenConnection();
                selectCmd = "select FeederID from EDGIS.PGE_CachedTrace_ToProcess where CIRCUITSTATUS <> " + (int)CircuitStatus.Finished + " AND CIRCUITTYPE = " + (int)CircuitType.UltimateSource;
                using (var cmd = new OracleCommand(selectCmd, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        circuitsNotFinished.Add(reader.GetString(0));
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Determining ultimate source feeders left to process: ", ex);
                throw ex;
            }
            finally
            {
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
            }

            return circuitsNotFinished;
        }

        public List<string> GetSubstationCircuitsLeft()
        {
            List<string> circuitsNotFinished = new List<string>();
            string selectCmd = "";
            try
            {
                OpenConnection();
                selectCmd = "select FeederID from EDGIS.PGE_CachedTrace_ToProcess where CIRCUITSTATUS <> " + (int)CircuitStatus.Finished + " AND CIRCUITTYPE = " + (int)CircuitType.Substation;
                using (var cmd = new OracleCommand(selectCmd, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        circuitsNotFinished.Add(reader.GetString(0));
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Determining substation feeders left to process: ", ex);
                throw ex;
            }
            finally
            {
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
            }

            return circuitsNotFinished;
        }

        public List<string> GetCircuitsTraced()
        {
            List<string> circuitsProcessed = new List<string>();
            try
            {
                OpenConnection();
                string selectCmd = "select FeederID from EDGIS.PGE_CachedTrace_ToProcess where CIRCUITSTATUS = " + (int)CircuitStatus.Finished;
                using (var cmd = new OracleCommand(selectCmd, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    //_logger.Debug("Determining feeders processed: " + selectCmd);
                    //Console.WriteLine("Determining feeders processed: " + selectCmd);
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        circuitsProcessed.Add(reader.GetString(0));
                    }

                    //_logger.Debug("Finished Determining feeders processed: " + selectCmd);
                    //Console.WriteLine("Finished Determining feeders processed: " + selectCmd);
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Determining feeders processed: ", ex);
                Console.WriteLine("Error Determining feeders processed: ", ex);
                throw ex;
            }
            finally
            {
                if (OracleConn.State == ConnectionState.Open)
                    OracleConn.Close();
                OracleConn.Dispose();
                OracleConn = null;
            }

            return circuitsProcessed;
        }

        public void ExecuteProcedure(string procedure, List<OracleParameter> oracleParameters)
        {
            OracleConnection oraConn = null;
            int rowsChanged = 0;
            // m4jf edgisrearch 919 - get connection string using Passwordmanagement tool.
            // string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            //string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
            string connection = OracleConnectionString;


            try
            {
                oraConn = new OracleConnection(connection);
                oraConn.Open();
                using (var cmd = new OracleCommand(procedure, oraConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (OracleParameter param in oracleParameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    _logger.Debug("Executing Stored Procedure: " + procedure);
                    Console.WriteLine(DateTime.Now + ": Executing Stored Procedure: " + procedure);
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    _logger.Debug("Finished Executing Stored Procedure: " + procedure);
                    Console.WriteLine(DateTime.Now + ": Finished Executing Stored Procedure: " + procedure);
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing stored procedure: " + procedure, ex);
                Console.WriteLine("Error executing stored procedure: " + procedure, ex);
                throw ex;
            }
            finally
            {
                if (oraConn.State == ConnectionState.Open)
                    oraConn.Close();
                oraConn.Dispose();
                oraConn = null;
            }
        }

        public void ExecuteSql(string sqlCommand)
        {
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sqlCommand, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
        }

        public int RowCount(string selectCommand)
        {
            int rowsReturned = 0;
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(selectCommand, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;

                    rowsReturned = Int32.Parse(cmd.ExecuteScalar().ToString());

                    cmd.Dispose();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
            return rowsReturned;
        }


        private static int SchematicsID = -1;
        private static string SchematicsOwner = "";
        /// <summary>
        /// This will return a dictionary of the ED class names mapped to the schematics class names
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetSchematicsFeatureClassNameMap()
        {
            Dictionary<string, string> EDFeatClassNameToSchemFeatClassName = new Dictionary<string, string>();

            
            //First we need to determine the owner and the current schematics ID for our tables by querying
            //the sde.sch_dataset
            string sql = "select OWNER,ID from sde.sch_dataset ORDER BY ID";
            try
            {
                OpenConnection();
                using (var cmd = new OracleCommand(sql, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object SchemOwnerObject = dataReader["OWNER"];
                            object SchemIDObject = dataReader["ID"];
                            if ((SchemIDObject != null && !(SchemIDObject is DBNull)) &&
                                (SchemOwnerObject != null && !(SchemOwnerObject is DBNull)))
                            {
                                SchematicsID = Int32.Parse(SchemIDObject.ToString());
                                SchematicsOwner = SchemOwnerObject.ToString();
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }

                //Now we can query the ELTClass table for the feature class name mapping
                sql = "select UPPER(CREATIONNAME) CREATIONNAME,UPPER(NAME) NAME from " + SchematicsOwner + ".SCH" + SchematicsID + "_ELTCLASS";
                using (var cmd = new OracleCommand(sql, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object CreationNameObject = dataReader["CREATIONNAME"];
                            object NameObject = dataReader["NAME"];
                            if ((CreationNameObject != null && !(CreationNameObject is DBNull)) &&
                                (NameObject != null && !(NameObject is DBNull)))
                            {
                                EDFeatClassNameToSchemFeatClassName.Add(SchematicsOwner + "." + NameObject.ToString(), SchematicsOwner + ".SCH" + SchematicsID + "E_" + CreationNameObject.ToString());
                               
                            }
                            
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error obtaining schematics Feature Class name mapping. SQL : " + sql, ex);
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }

            return EDFeatClassNameToSchemFeatClassName;
        }

        //Network table name dictionary. The key is the default junction FCID
        private static Dictionary<int, string> NetworkTableToNames = new Dictionary<int, string>();

        /// <summary>
        /// Populates the provided dictionaries with the EdgeEID to FCID and Edge EID to OID lists for the provided
        /// where in clauses of OIDs. Also populates the junction EID equivalents.
        /// </summary>
        /// <param name="schemaName">Schema name of the network</param>
        /// <param name="defaultJunctionFCID">Default junction FCID of the network</param>
        /// <param name="junctionEIDToFCIDList">Junction EID to FCID dictionary to populate</param>
        /// <param name="edgeEIDToFCIDList">Edge EID to FCID dictionary to populate</param>
        /// <param name="junctionEIDToOIDList">Junction EID to OID dictionary to populate</param>
        /// <param name="edgeEIDToOIDList">Edge EID to OID dictionary to populate</param>
        /// <param name="whereInClauses">The list of where in clauses of EIDs to obtain by feature type (1=Junction,2=Edge)</param>
        public void GetFCIDListFromNetwork(string schemaName, int defaultJunctionFCID, ref Dictionary<int,int> junctionEIDToFCIDList,
            ref Dictionary<int, int> edgeEIDToFCIDList, ref Dictionary<int, int> junctionEIDToOIDList,
            ref Dictionary<int, int> edgeEIDToOIDList, Dictionary<string, List<string>> whereInClauses)
        {
            try
            {
                OpenConnection();

                string networkTable = "";
                if (!NetworkTableToNames.ContainsKey(defaultJunctionFCID))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            networkTable = schemaName + "." + "ZZ_MV_N_" + i + "_DESC";
                            string checkNetworkTableSql = "select count(*) from " + networkTable + " where userclassid = " + defaultJunctionFCID;
                            int count = 0;
                            using (var cmd = new OracleCommand(checkNetworkTableSql, OracleConn))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        count = Int32.Parse(reader[0].ToString());
                                    }
                                }
                                cmd.Dispose();
                            }

                            if (count > 0)
                            {
                                NetworkTableToNames.Add(defaultJunctionFCID, networkTable);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if (NetworkTableToNames.ContainsKey(defaultJunctionFCID))
                {
                    networkTable = NetworkTableToNames[defaultJunctionFCID];
                }
                else
                {
                    throw new Exception("Unable to find the network table for network with default junction FCID " + defaultJunctionFCID);
                }

                foreach (KeyValuePair<string, List<string>> kvp in whereInClauses)
                {
                    string featureType = kvp.Key;
                    bool isJunction = (featureType == "1");
                    foreach (string whereInClause in kvp.Value)
                    {
                        string selectJunctionsCommand = "select userclassid,EID,USERID from " + networkTable + " WHERE ELEMENTTYPE = " + featureType + " and EID in (" + whereInClause + ")";
                        using (var cmd = new OracleCommand(selectJunctionsCommand, OracleConn))
                        {
                            cmd.CommandType = CommandType.Text;
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        int eid = Int32.Parse(reader[1].ToString());
                                        int classID = Int32.Parse(reader[0].ToString());
                                        int OID = Int32.Parse(reader[2].ToString());
                                        if (isJunction)
                                        {
                                            junctionEIDToFCIDList.Add(eid, classID);
                                            junctionEIDToOIDList.Add(eid, OID);
                                        }
                                        else
                                        {
                                            edgeEIDToFCIDList.Add(eid, classID);
                                            edgeEIDToOIDList.Add(eid, OID);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //Keep processing if we encounter a bad row
                                    }
                                }
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Failed obtaining FCID list: " + OracleConnectionString + " Error: " + e.Message);
                throw e;
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
        }

        /// <summary>
        /// This method will return a dictionary of OID to EIDs for the network table that contains the specified conduit FCID.
        /// </summary>
        /// <param name="schemaName">Schema of the network</param>
        /// <param name="defaultJunctionFCID">Default Junction FCID of the network</param>
        /// <param name="conduitFCID">Conduit FCID</param>
        /// <param name="whereInClauses">List of Where Clauses</param>
        /// <returns></returns>
        public void GetConduitEIDsFromNetwork(ref Dictionary<int,int> OIDToEIDConduitMapping, string schemaName, int defaultJunctionFCID, int conduitFCID, List<string> whereInClauses)
        {
            try
            {
                OpenConnection();

                string networkTable = "";
                if (!NetworkTableToNames.ContainsKey(defaultJunctionFCID))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            networkTable = schemaName + "." + "N_" + i + "_DESC";
                            string checkNetworkTableSql = "select count(*) from " + networkTable + " where userclassid = " + defaultJunctionFCID;
                            int count = 0;
                            using (var cmd = new OracleCommand(checkNetworkTableSql, OracleConn))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        count = Int32.Parse(reader[0].ToString());
                                    }
                                }
                                cmd.Dispose();
                            }

                            if (count > 0)
                            {
                                NetworkTableToNames.Add(defaultJunctionFCID, networkTable);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                if (NetworkTableToNames.ContainsKey(defaultJunctionFCID))
                {
                    networkTable = NetworkTableToNames[defaultJunctionFCID];
                }
                else
                {
                    throw new Exception("Unable to find the network table for network with default junction FCID " + defaultJunctionFCID);
                }

                foreach (string whereInClause in whereInClauses)
                {
                    string selectJunctionsCommand = "select USERID,EID from " + networkTable + " WHERE USERCLASSID = " + conduitFCID + " and USERID in (" + whereInClause + ")";
                    using (var cmd = new OracleCommand(selectJunctionsCommand, OracleConn))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    int eid = Int32.Parse(reader[1].ToString());
                                    int OID = Int32.Parse(reader[0].ToString());
                                    if (!OIDToEIDConduitMapping.ContainsKey(OID)) { OIDToEIDConduitMapping.Add(OID, eid); }
                                }
                                catch (Exception ex)
                                {
                                    //Keep processing if we encounter a bad row
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Failed obtaining OID to EID conduit mapping: " + OracleConnectionString + " Error: " + e.Message);
                throw e;
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
        }

        private static Dictionary<int, string> ClassIDToClassNameMap = new Dictionary<int, string>();
        /// <summary>
        /// Obtains the object class ID from the physical name provided
        /// </summary>
        ///<param name="className">Name of class including schema owner (i.e. EDGIS.TRANSFORMER)</param>
        ///<returns>Object class ID of specified class</returns>
        public int GetObjectClassID(string className)
        {
            foreach (KeyValuePair<int, string> kvp in ClassIDToClassNameMap)
            {
                if (kvp.Value == className)
                {
                    return kvp.Key;
                }
            }

            int objectID = -1;
            string sql = "select OBJECTID from sde.gdb_items where PHYSICALNAME = '" + className.ToUpper() + "' and Upper(Name) = '" + className.ToUpper() + "'";
            try
            {
                OpenConnection();

                using (var cmd = new OracleCommand(sql, OracleConn))
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
                    _logger.Info("Unable to find specified object class ID for physical name: " + className.ToUpper());
                    _logger.Info(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error obtaining object class ID for : " + className + " SQL: " + sql, ex);
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }

            return objectID;
        }

        public Dictionary<int, string> GetClassIDToPhysicalNameMap(string traceTableName)
        {
            Dictionary<int, string> classIDToPhysicalName = new Dictionary<int, string>();
            try
            {                
                string selectCommand = "select objectid,physicalname from SDE.GDB_ITEMS where objectid in (select distinct(to_feature_classid) from " + traceTableName + ")";
                OpenConnection();
                using (var cmd = new OracleCommand(selectCommand, OracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int classID = reader.GetInt32(0);
                            string physicalName = reader.GetString(1);
                            classIDToPhysicalName.Add(classID, physicalName);
                        }
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
            return classIDToPhysicalName;
        }

        public OracleDataReader SelectRows(string selectStatement)
        {
            try
            {
                OpenConnection();
                var cmd = new OracleCommand(selectStatement, OracleConn);
                cmd.CommandType = CommandType.Text;
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
            return null;
        }

        public DataSet GetDataset(string tableName)
        {
            // m4jf edgisrearch 919 - get connection string using Passwordmanagement tool.
            // string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            //string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
            string connection = OracleConnectionString;
            try
            {
                OpenConnection();
                DataTable oleschema = OracleConn.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                System.Data.Common.DbCommand GetTableCmd = OracleConn.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
                DataSet set = new DataSet();
                foreach (DataRow row in oleschema.Rows)
                {
                    if (row["TABLE_NAME"].Equals(tableName))
                    {
                        try
                        {
                            DataTable DBTable = new DataTable();
                            GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"]; ;
                            ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                            DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                            set.Tables.Add(DBTable);
                            DBTable = null;
                        }
                        catch { }
                    }
                }

                return set;
            }
            catch (Exception ex)
            {
                _logger.Error("Error Getting Schema.  " + System.Environment.NewLine + ex.ToString());
                throw ex;
            }
            finally
            {
                if (OracleConn != null)
                {
                    OracleConn.Close();
                    OracleConn.Dispose();
                    OracleConn = null;
                }
            }
            return null;
        }
    }


    public enum CircuitStatus
    {
        InProgress,
        Finished,
        Retry,
        Error
    }

    public enum CircuitType
    {
        UltimateSource,
        Substation,
        Distribution
    }

}

