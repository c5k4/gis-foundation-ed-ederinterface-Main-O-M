using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using System.Data;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;

namespace PGE.BatchApplication.SSD_Initialization
{
    public class OracleConnectionControl
    {
        private string OracleConnectionString = "";
        private static Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "CachedTracing.log4net.config");

        public OracleConnectionControl(string commaSeparatedOracleConnection)
        {
            OracleConnectionString = commaSeparatedOracleConnection;
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
                    string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
                    string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
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
            string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);

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
                using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE.BatchApplication.SSD_Initialization IN EXCLUSIVE MODE", OracleConn))
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
        /// Insert a new control record. The process ID should be unique
        /// </summary>
        /// <param name="CIRCUITTYPE">"S" or "C"</param>
        /// <param name="circuits">Comma separated circuitIDs or substation names</param>
        public void InsertCircuitsForProcessing(CircuitType circuitType, string circuitID, OracleTransaction tr)
        {
            int rowsChanged = 0;
            string insertsql = "INSERT into EDGIS.PGE.BatchApplication.SSD_Initialization" +
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
            string sql = "UPDATE EDGIS.PGE.BatchApplication.SSD_Initialization SET error = '" + error + "',CircuitStatus = " + (int)circuitStatus + " where FEEDERID = '" + feederID + "'";
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE.BatchApplication.SSD_Initialization IN EXCLUSIVE MODE", OracleConn))
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
                _logger.Error("Error updating circuit status in EDGIS.PGE.BatchApplication.SSD_Initialization table.", ex);
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
            string sql = "SELECT CIRCUITSTATUS FROM EDGIS.PGE.BatchApplication.SSD_Initialization where FEEDERID = '" + feederID + "'";
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                try
                {
                    using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE.BatchApplication.SSD_Initialization IN EXCLUSIVE MODE", OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error locking table EDGIS.PGE.BatchApplication.SSD_Initialization.", ex);
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
                    _logger.Error("Error determining the current status of circuit from EDGIS.PGE.BatchApplication.SSD_Initialization. SQL: " + sql, ex);
                    throw ex;
                }

                try
                {
                    if (!isInProgress)
                    {
                        sql = "UPDATE EDGIS.PGE.BatchApplication.SSD_Initialization SET CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress) + " where" +
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
                    _logger.Error("Error updating EDGIS.PGE.BatchApplication.SSD_Initialization with circuits to process. SQL: " + sql, ex);
                    throw ex;
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error determining if circuit can be processed from EDGIS.PGE.BatchApplication.SSD_Initialization.", ex);
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
            string sql = "SELECT FEEDERID FROM EDGIS.PGE.BatchApplication.SSD_Initialization where CIRCUITSTATUS IS NULL AND CIRCUITTYPE = " + (int)circuitType;
            try
            {
                OpenConnection();
                tr = OracleConn.BeginTransaction();
                try
                {
                    using (var cmd = new OracleCommand("LOCK TABLE EDGIS.PGE.BatchApplication.SSD_Initialization IN EXCLUSIVE MODE", OracleConn))
                    {
                        cmd.Transaction = tr;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error locking table EDGIS.PGE.BatchApplication.SSD_Initialization.", ex);
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
                    _logger.Error("Error obtaining list of circuits from EDGIS.PGE.BatchApplication.SSD_Initialization. SQL: " + sql, ex);
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
                        sql = "UPDATE EDGIS.PGE.BatchApplication.SSD_Initialization SET CIRCUITSTATUS = " + ((int)CircuitStatus.InProgress) + " where" +
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
                    _logger.Error("Error updating EDGIS.PGE.BatchApplication.SSD_Initialization with circuits to process. SQL: " + sql, ex);
                    throw ex;
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                if (tr != null) { tr.Rollback(); }
                _logger.Error("Error reading next circuit to process record from EDGIS.PGE.BatchApplication.SSD_Initialization.", ex);
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

        public List<string> GetUltimateSourceCircuitsLeft()
        {
            List<string> circuitsNotFinished = new List<string>();
            string selectCmd = "";
            try
            {
                OpenConnection();
                selectCmd = "select FeederID from EDGIS.PGE.BatchApplication.SSD_Initialization where CIRCUITSTATUS <> " + (int)CircuitStatus.Finished + " AND CIRCUITTYPE = " + (int)CircuitType.UltimateSource;
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
                selectCmd = "select FeederID from EDGIS.PGE.BatchApplication.SSD_Initialization where CIRCUITSTATUS <> " + (int)CircuitStatus.Finished + " AND CIRCUITTYPE = " + (int)CircuitType.Substation;
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
                string selectCmd = "select FeederID from EDGIS.PGE.BatchApplication.SSD_Initialization where CIRCUITSTATUS = " + (int)CircuitStatus.Finished;
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
            string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
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

        public void GetFCIDListFromNetwork(string schemaName, int defaultJunctionFCID, ref Dictionary<int,int> junctionEIDToFCIDList,
            ref Dictionary<int, int> edgeEIDToFCIDList, ref Dictionary<int, int> junctionEIDToOIDList,
            ref Dictionary<int, int> edgeEIDToOIDList)
        {
            try
            {
                OpenConnection();
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        string networkTable = schemaName + "." + "N_" + i + "_DESC";
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
                            string selectJunctionsCommand = "select userclassid,EID,USERID from " + networkTable + " WHERE ELEMENTTYPE = 1";
                            using (var cmd = new OracleCommand(selectJunctionsCommand, OracleConn))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int eid = Int32.Parse(reader[1].ToString());
                                        int classID = Int32.Parse(reader[0].ToString());
                                        int OID = Int32.Parse(reader[2].ToString());
                                        junctionEIDToFCIDList.Add(eid, classID);
                                        junctionEIDToOIDList.Add(eid, OID);
                                    }
                                }
                                cmd.Dispose();
                            }

                            string selectEdgesCommand = "select userclassid,EID,USERID from " + networkTable + " WHERE ELEMENTTYPE = 2";
                            using (var cmd = new OracleCommand(selectEdgesCommand, OracleConn))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (OracleDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int eid = Int32.Parse(reader[1].ToString());
                                        int classID = Int32.Parse(reader[0].ToString());
                                        int OID = Int32.Parse(reader[2].ToString());
                                        edgeEIDToFCIDList.Add(eid, classID);
                                        edgeEIDToOIDList.Add(eid, OID);
                                    }
                                }
                                cmd.Dispose();
                            }
                        }
                    }
                    catch { }
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
            string sql = "select OBJECTID from sde.gdb_items where PHYSICALNAME = '" + className.ToUpper() + "'";
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
            string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);

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
                        DataTable DBTable = new DataTable();
                        GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"]; ;
                        ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                        DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                        set.Tables.Add(DBTable);
                        DBTable = null;
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

