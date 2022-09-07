// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Database Related operations
// TCS V1T8/V3SF (EDGISREARC-373) 05/10/2021                        Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Configuration;
using PGE_DBPasswordManagement;
using System.Globalization;

namespace PGE.Interfaces.Integration.Gateway
{
    public class DBHelper
    {
        #region Private Variables 
        private static OracleConnection conOraEDER = null;
        public static int objectid;
        #endregion
        #region Public Variable
        public static OracleConnection _oraConnection = null;
        #endregion

        public DBHelper()
        {
            //Load configuration from app config
            ReadConfiguration.LoadConfiguration();
            Common._log.Info("Configuration Loaded");
        }
        #region Private Methods
        /// <summary>
        /// This method is used to open  oracle connection
        /// </summary>
        /// <param name="objconOra"></param>
        /// <param name="oracleConnectionString"></param>
        public static void OpenConnection()
        {

            try
            {
                if (_oraConnection == null)
                {
                    //Read oracle connection string 
                    string oracleConnectionString = ReadConfiguration.oracleConnectionString;
                    //Common._log.Info("Connection String from configuration file" + oracleConnectionString);
                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                    {
                        Common._log.Error("Cannot read EDER Connection String path from configuration file");
                    }

                    //create an instance of OracleConnection
                    _oraConnection = new OracleConnection();
                    Common._log.Info("Created an instance of OracleConnection");
                    //make the database connection
                    _oraConnection.ConnectionString = oracleConnectionString;
                }
                //If databse connection state is not open then open connection
                if (_oraConnection.State != ConnectionState.Open)
                {
                    _oraConnection.Open();
                    Common._log.Info(" OracleConnection opened");

                }

            }
            catch (Exception ex)
            {
                Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
        }

        #region Private Methods
        /// <summary>
        /// This method is used to open  oracle connection
        /// </summary>
        /// <param name="objconOra"></param>
        /// <param name="oracleConnectionString"></param>
        public static void OpenConnectionWIP(string oracleConnectionString)
        {

            try
            {
                if (_oraConnection == null)
                {
                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                    {
                        Common._log.Error("Cannot read EDER Connection String path from configuration file");
                    }
                    //create an instance of OracleConnection
                    _oraConnection = new OracleConnection();
                    Common._log.Info("Created an instance of OracleConnection");
                    //make the database connection                    
                    _oraConnection.ConnectionString = oracleConnectionString;
                }
                //If databse connection state is not open then open connection
                if (_oraConnection.State != ConnectionState.Open)
                {
                    _oraConnection.Open();
                    Common._log.Error(" OracleConnection opened" + oracleConnectionString);

                }

            }
            catch (Exception ex)
            {
                Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// (V3SF)
        /// This method is used to open oracle connection for given Database
        /// </summary>
        /// <param name="objconOra"></param>
        /// <param name="oracleConnectionString"></param>
        public static void OpenConnection(string dbConnection)
        {

            try
            {
                if (_oraConnection == null)
                {
                    // M4JF EDGISREARCH 919
                    // string connstr = dbConnection + "OracleConnectionString";
                    string connstr = dbConnection + "_ConnectionStr";
                    string oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings[connstr].ToString().ToUpper());

                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                    {
                        Common._log.Error("Cannot read " + dbConnection + " Connection String path from configuration file");
                    }

                    _oraConnection = new OracleConnection();
                    //make the database connection
                    _oraConnection.ConnectionString = oracleConnectionString;
                }

                if (_oraConnection.State != ConnectionState.Open)
                {
                    _oraConnection.Open();

                }

            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used to close oracle connection
        /// </summary>
        /// <param name="objconOra"></param>
        /// <param name="objReleaseConn"></param>
        public static void CloseConnection()
        {

            try
            {
                //close the database connection
                if (_oraConnection != null)
                {
                    Common._log.Info(" closing the database Connection ");
                    if (_oraConnection.State == ConnectionState.Open)
                        _oraConnection.Close();
                    _oraConnection.Dispose();
                    _oraConnection = null;
                    Common._log.Info(" OracleConnection closed");
                }
            }

            catch (Exception ex)
            {
                _oraConnection = null;
                Common._log.Error("Unhandled exception encountered while clsoing the database Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }

        }
        #endregion

        #endregion
        #region
        /// <summary>
        /// Method to execute query
        /// </summary>
        /// <param name="strUpdateQry"></param>
        /// <returns></returns>
        public static int ExecuteQuery(string strUpdateQry)
        {
            int intUpdateResult = -1;
            OracleCommand cmdExecuteQuery = null;
            //Open database connection
            OpenConnection();
            Common._log.Info("Database Connection opened");
            try
            {
                //create a new instance of oracle command
                cmdExecuteQuery = new OracleCommand(strUpdateQry, _oraConnection);
                Common._log.Info("created a new instance of oracle command" + strUpdateQry);

                cmdExecuteQuery.CommandTimeout = 0;
                //Exceute oracle command
                intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                Common._log.Info("Command executed successfully" + strUpdateQry);
            }

            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while executing oracle command  with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;

            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return intUpdateResult;
        }




        #region Outbound Functions

        /// <summary>
        /// (V3SF)
        /// Get List of GIS Processed Batch IDs
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="staging_table"></param>
        /// <param name="batchidField"></param>
        /// <param name="processFlagField"></param>
        /// <param name="processFlagValue"></param>
        /// <param name="batchIdList"></param>
        /// <returns></returns>
        /// //V3SF (25 Nov 2021) - Update Re-process Records with T - In Transaction and F - Failed Status
        public static bool GetUnProcessedBatchIdList(string dbConn, string staging_table, string batchidField, string processFlagField, out List<string> batchIdList)
        {
            #region Variable Decleration 
            string strUpdateQry = default;
            bool result = false;
            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;
            batchIdList = default;
            #endregion

            try
            {
                batchIdList = new List<string>();

                //Open Connection 
                OpenConnection(dbConn);

                //Query Bauilder
                //V3SF (25 Nov 2021) - Update Re-process Records with T - In Transaction and F - Failed Status 
                strUpdateQry = "select Distinct(" + batchidField + ") from " + staging_table + " where " + batchidField + " is not Null AND (" + processFlagField + " = '" + ProcessFlag.InTransition +"' OR "+ processFlagField + " = '" + ProcessFlag.Failed + "') order by " + batchidField + " asc";

                Common._log.Info("Executing Query :: " + strUpdateQry);

                cmdExecuteQuery = new OracleCommand(strUpdateQry, _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                oracleDataReader = cmdExecuteQuery.ExecuteReader();

                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        batchIdList.Add(Convert.ToString(oracleDataReader[0]));
                    }
                }
                result = true;
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }

            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Get Values from Stagng Table for given BatchID
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table</param>
        /// <param name="tableName">Staging Table Name</param>
        /// <param name="batchid">Batch ID to search</param>
        /// <param name="columnNames">GIS Column required to send to SAP</param>
        /// <returns></returns>
        public static DataTable GetStagingTableValues(string dbConn, string tableName, string batchid, string columnNames)
        {
            #region Variables
            DataSet DataSet = default;
            DataTable dataTable = default;
            System.Data.Common.DbCommand GetTableCmd = default;
            OracleDataAdapter ODA = default;
            OracleCommand cmd = default;
            string strUpdateQry = default;
            #endregion

            try
            {
                //GetOracleConnection();
                OpenConnection(dbConn);

                GetTableCmd = _oraConnection.CreateCommand();

                DataSet = new DataSet();
                ODA = new OracleDataAdapter();
                //V3SF (25 Nov 2021) - Update Re-process Records with T - In Transaction and F - Failed Status
                //strUpdateQry = "SELECT " + columnNames + " FROM " + tableName + " where BATCHID = '" + batchid + "'";
                strUpdateQry = "SELECT " + columnNames + " FROM " + tableName + " where BATCHID = '" + batchid + "' AND ( " + ReadConfiguration.GISProcessFlagField + " = '" + ProcessFlag.InTransition + "' OR " + ReadConfiguration.GISProcessFlagField + " = '" + ProcessFlag.Failed + "' )";

                Common._log.Info("Executing Query :: " + strUpdateQry);

                //cmd = new OracleCommand("SELECT " + columnNames + " FROM " + tableName + " where BATCHID = '" + batchid + "' AND rownum<100");
                //cmd = new OracleCommand("SELECT " + columnNames + " FROM " + tableName + " where BATCHID = '" + batchid + "'");
                cmd = new OracleCommand(strUpdateQry);
                cmd.Connection = _oraConnection;
                ODA.SelectCommand = cmd;

                ODA.Fill(DataSet, tableName);

                dataTable = DataSet.Tables[tableName];

                Common._log.Info("Executed Query :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered in GetStagingTableValues() " + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                CloseConnection();
            }

            return dataTable;
        }

        /// <summary>
        /// (V3SF)
        /// Update Batch ID and Process Flag Value
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table</param>
        /// <param name="staging_table">Name of Staging Table</param>
        /// <param name="batchIdField">BatchID Field Name in Staging Table</param>
        /// <param name="processFlagField">Process Flag Field Name in Staging Table</param>
        /// <param name="processFlagValue">Processed Flag Value</param>
        /// <param name="batchIDValue">Batch ID Value</param>
        /// <param name="errorField">Error Field Name in Staging Table</param>
        /// <param name="recordidField">Record ID Field Name in Staging Table</param>
        /// <param name="batchSize">Batch Size</param>
        /// <param name="addwhereClause">Where Claues Value</param>
        /// <returns></returns>
        public static bool UpdateBatchIDProcessFlag(string dbConn, string staging_table, string batchIdField, string processFlagField, string processFlagValue, string batchIDValue, string errorField, string recordidField, int batchSize, string addwhereClause)
        {
            #region Variables
            bool result = false;
            StringBuilder strUpdateQry = default;
            OracleCommand cmdExecuteQuery = null;
            #endregion

            try
            {
                //Open Connection
                OpenConnection(dbConn);

                //Query Builder
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("UPDATE " + staging_table + " SET ");
                strUpdateQry.Append(batchIdField + " = '" + batchIDValue + "' ,");
                strUpdateQry.Append(processFlagField + " = '" + processFlagValue);
                strUpdateQry.Append("' WHERE ROWID IN(SELECT ROWID FROM(");
                strUpdateQry.Append("SELECT " + batchIdField + ", " + processFlagField + " FROM ");
                strUpdateQry.Append(staging_table + " WHERE " + batchIdField + " IS NULL AND ");
                strUpdateQry.Append(errorField + " IS NULL AND " + processFlagField + " = '" + ProcessFlag.GISProcessed + "'" + addwhereClause + " ORDER BY " + recordidField + " ASC)");
                strUpdateQry.Append("WHERE ROWNUM <= " + batchSize + " )");

                Common._log.Info("Executing Query :: " + strUpdateQry);

                using (cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection))
                {
                    //Timeout property
                    cmdExecuteQuery.CommandTimeout = 0;

                    //Execute operation
                    cmdExecuteQuery.ExecuteNonQuery();

                    result = true;
                }

                Common._log.Info("Executed Query :: " + strUpdateQry);
            }

            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Update Record ID and Process Flag Value
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table</param>
        /// <param name="staging_table">Name of Staging Table</param>
        /// <param name="recordIDField">BatchID Field Name in Staging Table</param>
        /// <param name="processFlagField">Process Flag Field Name in Staging Table</param>
        /// <param name="processFlagValue">Processed Flag Value</param>
        /// <param name="recordIDValues">Batch ID Value</param>
        /// <param name="errorField">Error Field Name in Staging Table</param>
        /// <param name="recordidField">Record ID Field Name in Staging Table</param>
        /// <param name="batchSize">Batch Size</param>
        /// <param name="addwhereClause">Where Claues Value</param>
        /// <returns></returns>
        public static bool UpdateRecordIDProcessFlag(string dbConn, string staging_table, string recordIDField, string processFlagField, string processFlagValue, string recordIDValues)
        {
            #region Variables
            bool result = false;
            StringBuilder strUpdateQry = default;
            OracleCommand cmdExecuteQuery = null;
            List<string> limitedRecordID = default;
            int count = 0;
            int limit = 700;
            string processRecordIDs = string.Empty;
            #endregion

            try
            {
                //Open Connection
                OpenConnection(dbConn);

                limitedRecordID = new List<string>();

                limitedRecordID = recordIDValues.Split(',').ToList();

                while (limitedRecordID.Count > 0)
                {
                    if (limitedRecordID.Count < limit)
                        count = limitedRecordID.Count;
                    else
                        count = limit;

                    processRecordIDs = string.Join(", ", limitedRecordID.Take(count));

                    //Query Builder
                    strUpdateQry = new StringBuilder();
                    strUpdateQry.Append("UPDATE " + staging_table + " SET ");
                    strUpdateQry.Append(processFlagField + " = '" + processFlagValue + "' , ");
                    strUpdateQry.Append(ReadConfiguration.GISProcessedTime + " = sysdate ");
                    strUpdateQry.Append("WHERE " + recordIDField + " IN ( " + processRecordIDs + ")");

                    Common._log.Info("Executing Query :: " + strUpdateQry);

                    using (cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection))
                    {
                        //Timeout property
                        cmdExecuteQuery.CommandTimeout = 0;

                        //Execute operation
                        cmdExecuteQuery.ExecuteNonQuery();

                        result = true;
                    }

                    limitedRecordID.RemoveRange(0, count);
                }
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }

            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Get GIS Processed Record Count from Staging Table with No Errors
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table </param>
        /// <param name="Staging_Table_Name">Staging Table Name</param>
        /// <param name="gis_errorCol">GIS Staging Table Error Field Name</param>
        /// <param name="gis_batchCol">GIS Staging Table BatchID Field Name</param>
        /// <param name="gis_processCol">GIS Staging Table Process Flag Field Name</param>
        /// <param name="newprocessFlagValue">New Process Flag Value ( GIS Processed )</param>
        /// <returns></returns>
        public static int GetRecordCount(string dbConn, string Staging_Table_Name, string gis_errorCol, string gis_batchCol, string gis_processCol, string newprocessFlagValue)
        {
            #region Variable
            int intUpdateResult = -1;
            string TypeField = default;
            IList<string> TypeValues = default;
            string addWhereClause = default;
            #endregion

            try
            {
                //Initialize
                TypeValues = new List<string>();

                //Main Function
                intUpdateResult = GetRecordCount(dbConn, Staging_Table_Name, gis_errorCol, gis_batchCol, gis_processCol, newprocessFlagValue, TypeField, TypeValues, out addWhereClause);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered in GetRecordCount() " + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            return intUpdateResult;
        }

        /// <summary>
        /// (V3SF)
        /// Get GIS Processed Record Count from Staging Table with No Errors with Type Field
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table</param>
        /// <param name="Staging_Table_Name">Staging Table Name</param>
        /// <param name="gis_errorCol">GIS Staging Table Error Field Name</param>
        /// <param name="gis_batchCol">GIS Staging Table BatchID Field Name</param>
        /// <param name="gis_processCol">GIS Staging Table Process Flag Field Name</param>
        /// <param name="newprocessFlagValue">New Process Flag Value ( GIS Processed )</param>
        /// <param name="TypeField">GIS Staging Table Type Field Name</param>
        /// <param name="TypeValues">GIS Staging Table Type Field Value to Filter result</param>
        /// <param name="addWhereClause">OUT:: Where Clause Value for populating Batch ID</param>
        /// <returns></returns>
        public static int GetRecordCount(string dbConn, string Staging_Table_Name, string gis_errorCol, string gis_batchCol, string gis_processCol, string newprocessFlagValue, string TypeField, IList<string> TypeValues, out string addWhereClause)
        {
            #region Variables
            int intUpdateResult = -1;
            StringBuilder strUpdateQry = default;
            OracleCommand cmdExecuteQuery = null;
            addWhereClause = default;
            string typeValue = default;
            #endregion

            try
            {
                //Open Connection
                OpenConnection(dbConn);

                //Create Where Clause to Edit
                if (!string.IsNullOrWhiteSpace(TypeField) && TypeValues.Count > 0)
                {
                    foreach (string str in TypeValues)
                    {
                        if (string.IsNullOrWhiteSpace(typeValue))
                            typeValue = "'" + str + "'";
                        else
                            typeValue += "," + "'" + str + "'";
                    }

                    addWhereClause = " AND " + TypeField + " IN (" + typeValue + ")";
                }

                //Query Builder
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("select Count(*) from " + Staging_Table_Name + " where " + gis_errorCol + " is Null AND " + gis_batchCol + " is Null AND " + gis_processCol + " = '" + newprocessFlagValue + "'" + addWhereClause);
                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                //Execute Command
                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                intUpdateResult = Convert.ToInt32(cmdExecuteQuery.ExecuteScalar());

                Common._log.Info("Executed Query :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return intUpdateResult;
        }

        /// <summary>
        /// (V3SF)
        /// Get Record Count from Staging Table with given Batch ID
        /// </summary>
        /// <param name="dbConn">Database containing Staging Table</param>
        /// <param name="Staging_Table_Name">Staging Table Name</param>
        /// <param name="gis_errorCol">GIS Staging Table Error Field Name</param>
        /// <param name="gis_batchCol">GIS Staging Table BatchID Field Name</param>
        /// <param name="gis_processCol">GIS Staging Table Process Flag Field Name</param>
        /// <param name="newprocessFlagValue">New Process Flag Value ( GIS Processed )</param>
        /// <param name="TypeField">GIS Staging Table Type Field Name</param>
        /// <param name="TypeValues">GIS Staging Table Type Field Value to Filter result</param>
        /// <param name="addWhereClause">OUT:: Where Clause Value for populating Batch ID</param>
        /// <returns></returns>
        public static int GetBatchRecordCount(string dbConn, string Staging_Table_Name, string gis_batchCol, string batchid)
        {
            #region Variables
            int intUpdateResult = -1;
            StringBuilder strUpdateQry = default;
            OracleCommand cmdExecuteQuery = null;
            #endregion

            try
            {
                //Open Connection
                OpenConnection(dbConn);

                //Query Builder
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("select Count(*) from " + Staging_Table_Name + " where " + gis_batchCol + " = '" + batchid + "'");
                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                //Execute Command
                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                intUpdateResult = Convert.ToInt32(cmdExecuteQuery.ExecuteScalar());
                
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return intUpdateResult;
        }

        /// <summary>
        /// (V3SF)
        /// Get INTERFACE_STAGINGTABLE_CONFIG Values
        /// </summary>
        /// <param name="Interface_Name">IN:: Name of Interface</param>
        /// <param name="staging_table">OUT:: Staging Table Name</param>
        /// <param name="database_name">OUT:: Database Name containg Staging Table</param>
        /// <param name="batch_size">OUT:: Interface record count per Batch</param>
        /// <param name="interface_number">OUT:: Interface Number used in Json String</param>
        /// <returns></returns>
        public static bool GetINTERFACE_STAGINGTABLE_CONFIG(string Interface_Name, out string staging_table, out string database_name, out int batch_size, out string interface_number)
        {
            #region Variable
            //Variables
            bool result = false;
            StringBuilder strUpdateQry = default;

            //Database Objects
            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;

            //Initialization
            staging_table = default;
            database_name = default;
            batch_size = -1;
            interface_number = default;
            #endregion

            try
            {
                //Open Connection
                OpenConnection();

                //Query
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("select " + ReadConfiguration.ISC_staging_table + "," + ReadConfiguration.ISC_database_name + "," + ReadConfiguration.ISC_interface_number + "," + ReadConfiguration.ISC_batch_size + " from " + ReadConfiguration.INTERFACE_STAGINGTABLE_CONFIG + " where " + ReadConfiguration.ISC_interface_name + " = '" + Interface_Name + "'");

                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                oracleDataReader = cmdExecuteQuery.ExecuteReader();

                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        staging_table = Convert.ToString(oracleDataReader[0]);
                        database_name = Convert.ToString(oracleDataReader[1]);
                        interface_number = Convert.ToString(oracleDataReader[2]);
                        batch_size = Convert.ToInt32(oracleDataReader[3]);

                    }
                    result = true;
                    Common._log.Info("Executed Query :: " + strUpdateQry);
                }
                Common._log.Info("Executed Query with no row changes :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Get INTEGRATION_LASTRUN_DATE Values
        /// </summary>
        /// <param name="Interface_Name">IN:: Name of Interface</param>
        /// <param name="last_RunDate">OUT:: Last Date of Run</param>
        /// <returns></returns>
        public static bool GetINTEGRATION_LASTRUN_DATE(string Interface_Name, out DateTime last_RunDate)
        {
            #region Variable
            //Variables
            bool result = false;
            StringBuilder strUpdateQry = default;

            //Database Objects
            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;

            //Initialization
            last_RunDate = default;
            #endregion

            try
            {
                //Open Connection
                OpenConnection();

                //Query
                strUpdateQry = new StringBuilder();
                //select TO_CHAR(LASTRUN_DATE,'YYYYMMDDhh24miss') from PGEDATA.INTEGRATION_LASTRUN_DATE;
                strUpdateQry.Append("select TO_CHAR(" + ReadConfiguration.ILD_LASTRUN_DATE + ",'YYYYMMDDhh24miss') from " + ReadConfiguration.INTEGRATION_LASTRUN_DATE + " where " + ReadConfiguration.ILD_INTERFACE_NAME + " = '" + Interface_Name + "'");

                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                oracleDataReader = cmdExecuteQuery.ExecuteReader();

                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        last_RunDate = DateTime.ParseExact(Convert.ToString(oracleDataReader[0]), "yyyyMMddHHmmss", CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                    }
                    result = true;
                    Common._log.Info("Executed Query :: " + strUpdateQry);
                }
                Common._log.Info("Executed Query with no row change :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Set INTEGRATION_LASTRUN_DATE Values
        /// </summary>
        /// <param name="Interface_Name">IN:: Name of Interface</param>
        /// <param name="last_RunDate">IN:: Date of Run</param>
        /// <returns></returns>
        public static bool SetINTEGRATION_LASTRUN_DATE(string Interface_Name,DateTime last_RunDate)
        {
            #region Variable
            //Variables
            bool result = false;
            StringBuilder strUpdateQry = default;

            //Database Objects
            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;
            #endregion

            try
            {
                //Open Connection
                OpenConnection();

                //Query
                strUpdateQry = new StringBuilder();
                //Update PGEDATA.INTEGRATION_LASTRUN_DATE SET LASTRUN_DATE = TO_DATE('20210804000000', 'YYYYMMDDhh24miss') where interface_name = 'ED11';
                strUpdateQry.Append("Update " + ReadConfiguration.INTEGRATION_LASTRUN_DATE + " SET " + ReadConfiguration.ILD_LASTRUN_DATE + "= TO_DATE('"+ last_RunDate.ToString("yyyyMMddHHmmss") + "', 'YYYYMMDDhh24miss') where " + ReadConfiguration.ILD_INTERFACE_NAME + " = '" + Interface_Name + "'");

                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                cmdExecuteQuery.ExecuteReader();
                result = true;
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Get Field Mapping for Given Intrface Name
        /// </summary>
        /// <param name="Interface_Name">IN:: Interface Name</param>
        /// <param name="columnMappingDict">OUT:: Collection of GIS/SAP Fields</param>
        /// <param name="cSepGISCol">Comma Seperated GIS Column Names</param>
        /// <returns></returns>
        public static bool GetINTEGRATION_FIELD_MAPPING(string Interface_Name, out Dictionary<string, string> columnMappingDict, out string cSepGISCol)
        {

            #region Variable Decleration 
            bool result = false;
            StringBuilder strUpdateQry = default;

            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;

            cSepGISCol = default;
            columnMappingDict = default;
            #endregion

            try
            {
                //OpenConnection Connection
                OpenConnection();

                //Query Builder
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("SELECT " + ReadConfiguration.IFM_SAP_FIELD + "," + ReadConfiguration.IFM_GIS_FIELD + " FROM " + ReadConfiguration.INTEGRATION_FIELD_MAPPING + " where " + ReadConfiguration.IFM_INTERFACE_NAME + " = '" + Interface_Name + "' AND " + ReadConfiguration.IFM_SAP_FIELD + " is not null Order by " + ReadConfiguration.IFM_SAP_SEQUENCE + " asc");

                //Common._log.Info("Query :: " + strUpdateQry);
                Common._log.Info("Executing Query :: " + strUpdateQry);

                //Initialize Variables
                columnMappingDict = new Dictionary<string, string>();
                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                oracleDataReader = cmdExecuteQuery.ExecuteReader();

                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        columnMappingDict.Add(Convert.ToString(oracleDataReader[0]), Convert.ToString(oracleDataReader[1]));
                        if (string.IsNullOrWhiteSpace(cSepGISCol))
                            cSepGISCol = Convert.ToString(oracleDataReader[1]);
                        else
                            cSepGISCol += "," + Convert.ToString(oracleDataReader[1]);
                    }
                }
                result = true;
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// (V3SF)
        /// Get Type Field and Values
        /// </summary>
        /// <param name="Interface_Name">IN:: Interface Name</param>
        /// <param name="TypeTypeValue">OUT:: Dictionary of Type Value to assign BatchID and compare</param>
        /// <param name="TypeField">OUT:: Type Field in Staging Table</param>
        /// <returns></returns>
        public static bool GetTypeFieldValue(string Interface_Name, out Dictionary<string, List<string>> TypeTypeValue, out string TypeField)
        {

            #region Variable Decleration 
            bool result = false;
            string tempstr = string.Empty;
            string[] tempstrarr = default;
            StringBuilder strUpdateQry = default;

            OracleCommand cmdExecuteQuery = null;
            OracleDataReader oracleDataReader = default;

            TypeField = default;
            TypeTypeValue = default;
            #endregion

            try
            {
                //Open Connection
                OpenConnection();

                //Query Builder
                strUpdateQry = new StringBuilder();
                strUpdateQry.Append("SELECT " + ReadConfiguration.IFM_GIS_FIELD + " FROM " + ReadConfiguration.INTEGRATION_FIELD_MAPPING + " where " + ReadConfiguration.IFM_INTERFACE_NAME + " = '" + Interface_Name + "'");

                Common._log.Info("Executing Query :: " + strUpdateQry);

                //Initialize Variables
                TypeTypeValue = new Dictionary<string, List<string>>();
                cmdExecuteQuery = new OracleCommand(Convert.ToString(strUpdateQry), _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                oracleDataReader = cmdExecuteQuery.ExecuteReader();

                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        tempstr = Convert.ToString(oracleDataReader[0]);
                        tempstrarr = tempstr.Split(',');
                        if (tempstrarr.Length == 3)
                        {
                            if (string.IsNullOrWhiteSpace(TypeField) || TypeField == tempstrarr[0])
                                TypeField = tempstrarr[0];
                            else
                                throw new Exception("Multipe Type Fields can't be used");

                            if (!TypeTypeValue.ContainsKey(tempstrarr[1]))
                                TypeTypeValue.Add(tempstrarr[1], new List<string>());
                            if (!TypeTypeValue[tempstrarr[1]].Contains(tempstrarr[2]))
                                TypeTypeValue[tempstrarr[1]].Add(tempstrarr[2]);
                        }
                    }
                }
                result = true;
                Common._log.Info("Executed Query :: " + strUpdateQry);
            }

            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            finally
            {
                cmdExecuteQuery.Dispose();
                CloseConnection();
            }

            return result;
        }

        /// <summary>
        /// (V3SF) Not Used
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable GetStagingDataTable(string tableName)
        {
            DataTable DBTable = default;
            try
            {
                System.Data.Common.DbCommand GetTableCmd = _oraConnection.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;
                DBTable = new DataTable();
                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;

                GetTableCmd.CommandText = "SELECT * FROM " + tableName;
                ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                DBTable.TableName = tableName;

                return DBTable;
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered GetStagingDataTable() " + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }

        }

        #endregion



        /// <summary>
        /// To get oracle connection 
        /// </summary>
        public static void GetOracleConnection()
        {
            Common._log.Info("Making connection with oracle" + ReadConfiguration.oracleConnectionString);
            //make connection with oracle
            // _oraConnection = new OracleConnection("Data Source=EDGEM1D;User Id=EDGIS;password=EDG_ZRz97*gem1d;");
            _oraConnection = new OracleConnection(ReadConfiguration.oracleConnectionString);
            Common._log.Info("Connection done with oracle" + ReadConfiguration.oracleConnectionString);

        }
        /// <summary>
        /// Method to get dataset
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>DataSet</returns>
        public static DataSet GetStagingDataset(string tableName)
        {
            DataSet set = new DataSet();
            try
            {
                //validate the input string
                if (tableName == string.Empty)
                {
                    Common._log.Info("Input string is empty");
                    return null;
                }
                //Open database connection
                OpenConnection();
                Common._log.Info("Database connection opened");
                DataTable oleschema = _oraConnection.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                Common._log.Info("Received the list of tables in connected schema");
                // Create a command
                System.Data.Common.DbCommand GetTableCmd = _oraConnection.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
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
                        Common._log.Info("Found the schema of" + tableName);
                    }
                }

                return set;
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while getting the table schema from database :" + tableName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                CloseConnection();
                throw ex;
            }
            finally
            {
            }

        }


        /// <summary>
        /// To Bulk Load data in database
        /// </summary>
        /// <param name="set"></param>

        public static void BulkLoadData(DataSet set)
        {
            try
            {
                //validate the input string
                if (set == null)
                {
                    Common._log.Info("Input DataSet is null");
                    return;
                }
                //Open database connection            
                OpenConnection();
                Common._log.Info("Database connection opened");
                try
                {
                    using (OracleBulkCopy copy = new OracleBulkCopy(_oraConnection, OracleBulkCopyOptions.UseInternalTransaction))
                    {

                        copy.BulkCopyTimeout = 1000;
                        foreach (DataTable table in set.Tables)
                        {
                            //if (logBulkCopy) { Console.WriteLine(string.Format("Bulk copying {1} rows into table {0}", table.TableName, table.Rows.Count)); }
                            //Set the destination table to be copy
                            copy.DestinationTableName = table.TableName;
                            Common._log.Info("Destination table Name :" + table.TableName);

                            Common._log.Info("OracleBulkCopy Writing data to server");
                            //Write data to server using oracle bulk copy
                            foreach (DataColumn dataColumn in table.Columns)
                            {
                                copy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(Convert.ToString(dataColumn.ColumnName), Convert.ToString(dataColumn.ColumnName)));
                            }
                            copy.WriteToServer(table);


                            Common._log.Info("OracleBulkCopy sucessfully completed");
                        }
                        copy.Close();
                        Common._log.Info("OracleBulkCopy closed");
                        copy.Dispose();
                        Common._log.Info("OracleBulkCopy disposed");
                    }
                }
                catch (Exception ex)
                {
                    Common._log.Error("Exception encountered while Oracle bulk load data :" + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    //DataTable tempDT = new DataTable();
                    DataTable tempDT = new DataTable();
                    DataRow dataRow = default;
                    foreach (DataTable table in set.Tables)
                    {
                        //foreach (DataColumn dataColumn in table.Columns)
                        //{
                        //    tempDT.Columns.Add(new DataColumn(dataColumn.ColumnName));
                        //}
                        tempDT = table.Copy();
                        foreach (DataRow row in table.Rows)
                        {
                            tempDT.Rows.Clear();
                            tempDT.Rows.Add(row.ItemArray);
                            using (OracleBulkCopy copy = new OracleBulkCopy(_oraConnection, OracleBulkCopyOptions.UseInternalTransaction))
                            {

                                copy.BulkCopyTimeout = 1000;
                                //Set the destination table to be copy
                                copy.DestinationTableName = table.TableName;
                                Common._log.Info("Destination table Name :" + table.TableName);

                                Common._log.Info("OracleBulkCopy Writing data to server for single record");
                                //Write data to server using oracle bulk copy
                                foreach (DataColumn dataColumn in tempDT.Columns)
                                {
                                    copy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(Convert.ToString(dataColumn.ColumnName), Convert.ToString(dataColumn.ColumnName)));
                                }
                                try
                                {
                                    copy.WriteToServer(tempDT);
                                }
                                catch(Exception exp)
                                {
                                    Common._log.Error("OracleBulkCopy Writing data to server for single record retry for error :: " + exp.Message + " at " + exp.StackTrace);

                                    try
                                    {
                                        foreach (DataColumn dataColumn in tempDT.Columns)
                                        {
                                            if (dataColumn.ColumnName != ReadConfiguration.recordID && dataColumn.ColumnName != ReadConfiguration.objectId && dataColumn.ColumnName != ReadConfiguration.batchID && dataColumn.ColumnName != ReadConfiguration.creationDate && dataColumn.ColumnName != ReadConfiguration.notificationNo)
                                                try { 
                                                    if(dataColumn.AllowDBNull == true)
                                                    tempDT.Rows[0][dataColumn] = DBNull.Value; 
                                                } 
                                                catch (Exception err)
                                                {
                                                    Common._log.Error("OracleBulkCopy Set Null Value for single record (" + Convert.ToString(tempDT.Rows[0][ReadConfiguration.recordID]) + ") retry Failed for column "+dataColumn.ColumnName+" with error :: " + err.Message + " at " + err.StackTrace);
                                                }

                                            if (dataColumn.ColumnName == ReadConfiguration.flag)
                                                tempDT.Rows[0][dataColumn] = ProcessFlag.Failed;
                                        }

                                        tempDT.Rows[0][ReadConfiguration.error_description] = exp.Message;
                                        copy.WriteToServer(tempDT);
                                    }
                                    catch(Exception exp1)
                                    {
                                        Common._log.Error("OracleBulkCopy Writing data to server for single record ("+ Convert.ToString(tempDT.Rows[0][ReadConfiguration.recordID]) + ") retry Failed with error :: " + exp1.Message + " at " + exp1.StackTrace);
                                    }
                                }

                                Common._log.Info("OracleBulkCopy sucessfully completed");
                                copy.Close();
                                Common._log.Info("OracleBulkCopy closed");
                                copy.Dispose();
                                Common._log.Info("OracleBulkCopy disposed");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while Oracle bulk load data :" + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                CloseConnection();
                throw ex;
            }
            finally
            {

            }
        }

        public static void BulkLoadData(DataTable table, string TableName)
        {
            try
            {
                OpenConnection();

                using (OracleBulkCopy copy = new OracleBulkCopy(_oraConnection))
                {
                    //copy.DestinationTableName = tableName;
                    copy.BulkCopyTimeout = 600;
                    //copy.BatchSize = BatchSize;
                    //copy.Insert(entities);
                    copy.DestinationTableName = TableName;
                    //copif (logBulkCopy) { Console.WriteLine(string.Format("Bulk copying {1} rows into table {0}", table.TableName, table.Rows.Count)); }
                    // copy.DestinationTableName = table.TableName;
                    copy.WriteToServer(table);

                    copy.Close();
                    copy.Dispose();
                }

            }
            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while Oracle bulk load data :" + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Method to get objectID from database
        /// </summary>
        /// <param name="tableName">string</param>
        /// <returns>int</returns>
        public static int GetObjectId(string tableName)
        {
            int objectIdValue = 0;
            OracleCommand cmdSQL = null;

            try
            {
                OpenConnection();
                //Validate the input string
                if (tableName == string.Empty)
                {
                    Common._log.Info("Input string is empty returning to the calling method");
                    return -1;
                }
                // OracleCommand cmdSQL = new OracleCommand("select objectid from edgis.sap_to_gis where rowid in(select max(rowid) from edgis.sap_to_gis) ", _oraConnection);
                //Make the sql string
                string sql = "select max(objectid) from " + tableName;
                //OracleCommand cmdSQL = new OracleCommand("select max(objectid) from edgis.sap_to_gis", _oraConnection);
                //Create and instance of OracleCommand
                cmdSQL = new OracleCommand(sql, _oraConnection);
                Common._log.Info("Created instance of OracleCommand");

                //Execute the oracle command
                OracleDataReader dataReader = cmdSQL.ExecuteReader();
                Common._log.Info("Executed the oracle command");
                if (dataReader.FieldCount > 0)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader[0] == null || dataReader[0] == DBNull.Value)
                        { return objectIdValue; }
                        objectIdValue = Convert.ToInt32(dataReader[0]);

                        Common._log.Info("max Object Id from table :" + tableName + " :" + objectIdValue.ToString());
                        // objectid = objectIdValue++;
                        //   objectIdRead = true;

                    }
                }
            }

            catch (Exception ex)
            {
                Common._log.Error("Exception encountered while selecting max Object Id from table :" + tableName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (cmdSQL != null) cmdSQL.Dispose(); CloseConnection();
            }
            return objectIdValue;
        }

        /// <summary>
        /// Method to check related record ID
        /// </summary>
        /// <param name="relatedRecordID">string</param>
        /// <returns>bool</returns>
        public static bool RelatedRecordIDExist(string relatedRecordID)
        {
            string objectIdValue = string.Empty;
            OracleCommand cmdSQL = null;
            bool objectIdRead = false;
            string sql = string.Empty;

            try
            {
                sql = "Select " + ReadConfiguration.relatedRecordID + " from " + ReadConfiguration.ed07StagingTable + " WHERE " + ReadConfiguration.relatedRecordID + " = '" + relatedRecordID + "'";
                //OracleCommand cmdSQL = new OracleCommand("select objectid from edgis.sap_to_gis where rowid in(select max(rowid) from edgis.sap_to_gis) ", _oraConnection);
                OpenConnection();
                cmdSQL = new OracleCommand(sql, _oraConnection);
                OracleDataReader dataReader = cmdSQL.ExecuteReader();
                if (dataReader.FieldCount > 0)
                {
                    while (dataReader.Read())
                    {
                        objectIdValue = dataReader[0].ToString();
                        if (!string.IsNullOrEmpty(objectIdValue)) ;
                        objectIdRead = true;
                    }
                }

            }

            catch (Exception ex)
            {
                objectIdRead = false;
                //V3SF Exception Handling
                Common._log.Error("Error :: (" + ex.Message + ") at " + ex.StackTrace);
            }

            finally { if (cmdSQL != null) cmdSQL.Dispose(); CloseConnection(); }
            return objectIdRead;


        }

        public static bool UpdateErrorDescription(results row, string error, string tableName)
        {
            bool status = false;
            string sQuery;
            try
            {
                #region commented  coded if needed

                sQuery = "UPDATE " + tableName + " SET " +
                                   ReadConfiguration.error_description + " = '" + error + "' WHERE " + ReadConfiguration.relatedRecordID + " = " + row.RELATEDRECORDID;

                ExecuteQuery(sQuery);

                //   Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                Common._log.Error(ex.Message + "   " + ex.StackTrace);
            }
            return status;
        }

        public static bool UpdateED07Staging(Result row, string tableName)
        {
            bool status = false;
            string error = string.Empty;
            string processedFlag = string.Empty;
            string sQuery;
            try
            {
                #region commented  coded if needed

                if (string.IsNullOrEmpty(row.EquipName.ToString()) || string.IsNullOrEmpty(row.RecordId.ToString()) || string.IsNullOrEmpty(row.SapEquipId.ToString()) || string.IsNullOrEmpty(row.EquipType.ToString()) || string.IsNullOrEmpty(row.GisGuid.ToString()))
                {

                    Common._log.Info("Required field  SapEquipId/EquipName/EquipType/GisGuid is null or empty.Please verify the row");
                    error = "Required field SapEquipId/EquipName/EquipType/GisGuid is null or empty.Please verify the row";
                    processedFlag = ProcessFlag.GISError;
                }

                processedFlag = ProcessFlag.InTransition;
                sQuery = "UPDATE " + tableName + " SET " +
                                   ReadConfiguration.error_description + " = '" + error + "'," + ReadConfiguration.batchID + "='" + row.BatchId + "'," + ReadConfiguration.recordID + "='" + row.RecordId + "'," + ReadConfiguration.sapEquipId + "='" + row.SapEquipId + "'," + ReadConfiguration.sapEquipType + "='" + row.EquipType + "'," + ReadConfiguration.equipmentName + "='" + row.EquipName + "'," + ReadConfiguration.flag + "='" + processedFlag + "' , "+ ReadConfiguration.guid+" = '"+row.GisGuid+"' , " + ReadConfiguration.creationDate + " = sysdate WHERE " + ReadConfiguration.relatedRecordID + " = '" + row.RelatedRecordID + "'";

                ExecuteQuery(sQuery);

                //   Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error(ex.Message + "   " + ex.StackTrace);
                throw ex;
            }
            return status;
        }

        /// <summary>
        /// Mthod to log error
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="error"></param>
        /// <param name="recordID"></param>
        /// <param name="relatedRecordID"></param>

        public static void LogError(string tableName, string error, string recordID)
        {
            bool status = false;
            string sQuery;
            try
            {
                error = error.Replace("'", " ");
                #region commented  coded if needed
                // INSERT INTO table (column1, column2, ... column_n) VALUES (expression1, expression2, ... expression_n);
                //sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate + ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "','" + DateTime.Now + "')";
                sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate + ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "',sysdate)";

                ExecuteQuery(sQuery);

                //   Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error(ex.Message + "   " + ex.StackTrace);
                //throw ex;
            }
            //   return status;
        }

        public static void LogError(string tableName, string error, string recordID,string MFields,string MValues)
        {
            bool status = false;
            string sQuery;
            try
            {
                error = error.Replace("'", " ");
                #region commented  coded if needed
                // INSERT INTO table (column1, column2, ... column_n) VALUES (expression1, expression2, ... expression_n);
                //sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate + ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "','" + DateTime.Now + "')";
                if(string.IsNullOrWhiteSpace(MFields))
                    sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate + ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "',sysdate)";
                else
                {
                    sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate +","+ MFields+ ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "',sysdate,"+ MValues+")";
                }

                ExecuteQuery(sQuery);

                //   Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error(ex.Message + "   " + ex.StackTrace);
                //throw ex;
            }
            //   return status;
        }

        public static void LogError(string tableName, string error, string recordID, int objectID,string batchID)
        {
            bool status = false;
            string sQuery;
            try
            {
                error = error.Replace("'", " ");
                #region commented  coded if needed
                // INSERT INTO table (column1, column2, ... column_n) VALUES (expression1, expression2, ... expression_n);
                sQuery = "INSERT INTO " + tableName + " (" + ReadConfiguration.error_description + "," + ReadConfiguration.recordID + "," + ReadConfiguration.flag + "," + ReadConfiguration.creationDate + "," + ReadConfiguration.objectId +","+ReadConfiguration.batchID+ ") VALUES('" + error + "','" + recordID + "','" + ProcessFlag.GISError + "','" + DateTime.Now + "','" + objectID +"','"+batchID+ "')";

                Common._log.Info("Executing Query :: " + sQuery);

                ExecuteQuery(sQuery);

                Common._log.Info("Executed Query :: " + sQuery);
                //   Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error(ex.Message + "   " + ex.StackTrace);
                //throw ex;
            }
            //   return status;
        }
        #endregion
    }
}