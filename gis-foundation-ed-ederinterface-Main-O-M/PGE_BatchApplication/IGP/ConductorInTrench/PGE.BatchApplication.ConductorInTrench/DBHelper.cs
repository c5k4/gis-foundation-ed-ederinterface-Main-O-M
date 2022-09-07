using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data.Common;
using PGE.BatchApplication.ConductorInTrench;


namespace PGE.BatchApplication.ConductorInTrench
{
       
        public static class DBHelper
        {            
            public static OracleConnection _conOraEDER = null;

            /// <summary>
            /// Open Connection, if not opened already
            /// </summary>
            private static void OpenConnection(string strConnString1,out OracleConnection objconOra)
            {
                objconOra = null;
                string strConnString = string.Empty;
                try
                {
                    string[] oracleConnectionInfo = strConnString1.Split(',');
                    strConnString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];             
                    if (_conOraEDER == null)
                    {
                      
                        if (string.IsNullOrEmpty(strConnString))
                        {
                            Common._log.Error("Cannot read EDER Connection String path from configuration file");
                        }
                        else
                        {
                            _conOraEDER = new OracleConnection();
                            _conOraEDER.ConnectionString = strConnString;
                        }
                    }           
                    if (_conOraEDER.State != ConnectionState.Open)
                    {
                        _conOraEDER.Open();                       
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + "Exception : " + exp.Message.ToString() +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
            }

            /// <summary>
            /// Close Database Connection
            /// </summary>
            public static void CloseConnection(OracleConnection objconOra, out OracleConnection objReleaseConn)
            {
                objReleaseConn = null;
                try
                {
                    //close the database connection
                    if (objconOra != null)
                    {
                        if (objconOra.State == ConnectionState.Open)
                            objconOra.Close();
                        objconOra.Dispose();
                        objconOra = null;
                        objReleaseConn = objconOra;
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    objconOra = null;
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }

            }

            /// <summary>
            /// Execute a query, Get Datatable, extract the Datarow and return
            /// </summary>
            public static DataRow GetSingleDataRowByQuery(string strConnString, string strQuery)
            {
                DataRow drGetData = null;
                //Open Connection
                if (_conOraEDER == null)
                    OpenConnection(strConnString,out _conOraEDER);
                DataSet dsData = new DataSet();
                DataTable dtGetDatTable = new DataTable() ;
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = _conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(dtGetDatTable);
                    if (dtGetDatTable != null)
                    {
                        if (dtGetDatTable.Rows.Count > 0)
                        {
                            drGetData = dtGetDatTable.Rows[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " |  Stack Trace | " + ex.StackTrace);
                    ErrorCodeException ece = new ErrorCodeException(ex);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    if (cmdExecuteSQL != null)
                        cmdExecuteSQL.Dispose();

                    if (daOracle != null)
                        daOracle.Dispose();

                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
                return drGetData;
            }

            /// <summary>
            /// Fill Data Table based on Connection Strign and SQL Query
            /// </summary>
            public static  DataTable GetDataTable(string strConnString,string strQuery)
            {
                DataTable dtGetDatTable = null;
                DataSet dsData = null;
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    strQuery = strQuery.Trim();                  
                    //Open Connection
                    if (_conOraEDER == null || _conOraEDER.State == ConnectionState.Closed)
                    {
                        OpenConnection(strConnString, out _conOraEDER);
                    }
                    if (_conOraEDER.State == ConnectionState.Open)
                    {
                        dsData = new DataSet();
                        cmdExecuteSQL = null;
                        daOracle = null;
                        try
                        {
                            cmdExecuteSQL = new OracleCommand();
                            cmdExecuteSQL.CommandType = CommandType.Text;
                            cmdExecuteSQL.CommandTimeout = 0;
                            cmdExecuteSQL.CommandText = strQuery;
                            cmdExecuteSQL.Connection = _conOraEDER;

                            daOracle = new OracleDataAdapter();
                            daOracle.SelectCommand = cmdExecuteSQL;
                            daOracle.Fill(dsData);
                            dtGetDatTable = dsData.Tables[0];
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + exp.Message+" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                            Common._log.Info("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                            ErrorCodeException ece = new ErrorCodeException(exp);
                            Environment.ExitCode = ece.CodeNumber;
                        }
                        finally
                        {
                            cmdExecuteSQL.Dispose();
                            daOracle.Dispose();
                            CloseConnection(_conOraEDER, out _conOraEDER);
                        }
                    }
                }
                catch (Exception exp)
                {                    
                  Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                  ErrorCodeException ece = new ErrorCodeException(exp);
                  Environment.ExitCode = ece.CodeNumber;
                }               
                return dtGetDatTable;
            }
            
            /// <summary>
            /// Execute Update Query for single updates
            /// </summary>
            public static int UpdateQuery(string strConnString,string strUpdateQry)
            {
                OpenConnection( strConnString,out _conOraEDER);

                int intUpdateResult = 0;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, _conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 500;
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, exp);
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
                return intUpdateResult;
            }

            /// <summary>
            /// Execute Update Query for Datarow Array
            /// </summary>
            public static int UpdateQuery(string strConnString, DataRow[] drUGConductors)
            {
                string strsql = string.Empty;
                OracleCommand cmdExecuteQuery = null;
                int intUpdateResult = 0;
                string processed_on = null;
                try
                {
                    OpenConnection(strConnString, out _conOraEDER);

                    for (int i = 0; i < drUGConductors.Length; i++)
                    {
                        try
                        {
                            string globalid_1 = drUGConductors[i][ReadConfigurations.col_GLOBALID].ToString();
                            processed_on = DateTime.Now.ToString("dd-MMM-y").ToUpper();
                            //Removing status='New' clause from here
                            strsql = "Update " + ConfigurationManager.AppSettings["tableNameToSaveFinalData"] + " SET " + ReadConfigurations.col_STATUS + " = 'Completed'," + ReadConfigurations.col_PROCESSED_ON + "='" + processed_on + "' WHERE " + ReadConfigurations.col_GLOBALID + "='" + globalid_1 + "'";
                            cmdExecuteQuery = new OracleCommand(strsql, _conOraEDER);
                            cmdExecuteQuery.CommandTimeout = 500;
                            intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();

                            Common._log.Info("Query: " + strsql + " and result count :" + intUpdateResult);
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Exception encountered while update Table with QueryString. SQLQuery: " + strsql + ". Exception:" + exp.Message + " | Stack Trace |" + exp.StackTrace);
                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                            ErrorCodeException ece = new ErrorCodeException(exp);
                            Environment.ExitCode = ece.CodeNumber;
                        }
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    if (cmdExecuteQuery != null)
                        cmdExecuteQuery.Dispose();
                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
                return intUpdateResult;
            }

            /// <summary>
            /// Execute Update Query for Datarow Array
            /// </summary>
            public static int UpdateQuery(string strConnString, DataRow[] drUGConductors,string strTableName)
            {
                string strQuery = string.Empty;
                OracleCommand cmdExecuteQuery = null;
                int intUpdateResult = 0;
                DateTime dtTime;
                string globalid_1;
                string processed_on;
                try
                {
                    OpenConnection(strConnString, out _conOraEDER);

                    for (int i = 0; i < drUGConductors.Length; i++)
                    {
                        try
                        {                            
                            processed_on = "";
                            globalid_1 = drUGConductors[i][ReadConfigurations.col_GLOBALID].ToString();
                            processed_on = DateTime.Now.ToString("dd-MMM-y").ToUpper();
                            strQuery = "Update " + strTableName + " SET " + ReadConfigurations.col_STATUS + " = 'Completed'," + ReadConfigurations.col_PROCESSED_ON + "='" + processed_on + "' WHERE " + ReadConfigurations.col_GLOBALID + "='" + globalid_1 + "' AND " + ReadConfigurations.col_STATUS + "='New'";
                            cmdExecuteQuery = new OracleCommand(strQuery, _conOraEDER);
                            cmdExecuteQuery.CommandTimeout = 500;
                            intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();

                            Common._log.Info("Updated Table: " + strTableName + " with query: " + strQuery + " and result count :" + intUpdateResult);
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Exception encountered while update Table with QueryString. SQLQuery: " + strQuery + ". Exception:" + exp.Message + " | Stack Trace |" + exp.StackTrace);
                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                            ErrorCodeException ece = new ErrorCodeException(exp);
                            Environment.ExitCode = ece.CodeNumber;
                        }
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                }
                finally
                {
                    if (cmdExecuteQuery != null)
                    cmdExecuteQuery.Dispose();
                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
                return intUpdateResult;
            }

            /// <summary>
            /// Execute multiple Update Queries
            /// </summary>
            public static int UpdateMultipleQueries(string strConnString, List<string> arglstQueries)
            {
                int intUpdateResult = 0;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    OpenConnection(strConnString, out _conOraEDER);

                    foreach (string strUpdateQry in arglstQueries)
                    {
                        try
                        {
                            cmdExecuteQuery = new OracleCommand(strUpdateQry, _conOraEDER);
                            cmdExecuteQuery.CommandTimeout = 500;
                            intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, exp);
                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                        }                       
                    }
                }
                catch (Exception exp)
                {                    
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
                return intUpdateResult;
            }

            /// <summary>
            /// This function will run given query on given table
            /// </summary>
            /// <param name="tableName"></param>
            public static void RunGivenQueryOnTable(string tableName, string argQuery)
            {
                OracleConnection objconOra = null;
                OracleCommand cmd = null;
                try
                {
                // string[] oracleConnectionInfo = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata) .Split(',');
                // string oracleConnectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
                    string oracleConnectionString = ReadConfigurations.ConnectionString_pgedata;
                   objconOra = new OracleConnection();
                    objconOra.ConnectionString = oracleConnectionString;
                    objconOra.Open();
                    var sql = argQuery;
                    cmd = new OracleCommand(sql, objconOra);
                    cmd.ExecuteNonQuery();

                    Common._log.Info("Running query " + argQuery + " on table " + tableName + " is successful.");
                }
                catch (Exception exp)
                {                   
                    Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    if (cmd != null)
                        cmd.Dispose();
                    CloseConnection(objconOra);
                }
            }

            public static void CloseConnection(OracleConnection objconOra)
            {
                try
                {
                    //close the database connection
                    if (objconOra != null)
                    {
                        if (objconOra.State == ConnectionState.Open)
                            objconOra.Close();
                        objconOra.Dispose();
                        objconOra = null;
                    }
                }
                catch (Exception exp)
                {
                    objconOra = null;
                    Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                }
            }

            public static void InsertQuery(string strConnString, string globalID, string objectID, int filled_manual, int filled_auto, string division, string recepient)
            {
                string strsql = string.Empty;
                OracleCommand cmdExecuteQuery = null;
                int intInsertResult = 0;
                string processed_date = null;
                string processed_type = string.Empty;
                try
                {
                    OpenConnection(strConnString, out _conOraEDER);
                    processed_date = DateTime.Now.ToString("dd-MMM-y").ToUpper();
                    processed_type = "N";
                    strsql = "Insert into " +  ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation) + " values('" + globalID + "'," + objectID + "," + filled_manual + "," + filled_auto + "," + "'" + processed_date + "','" + processed_type + "'," + division + ",'"+recepient+"')";
                    cmdExecuteQuery = new OracleCommand(strsql, _conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 500;
                    intInsertResult = cmdExecuteQuery.ExecuteNonQuery();
                    
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    if (cmdExecuteQuery != null)
                        cmdExecuteQuery.Dispose();
                    //CloseConnection(conOraEDER, out conOraEDER);
                }
            }

            public static void UpdateConflictInformationTable(string date_process, string present_date)
            {
                string strsql = string.Empty;
                OracleCommand cmdExecuteQuery = null;
                string strConnString = ReadConfigurations.ConnectionString_pgedata ;
                try
                {
                    string whereClause = date_process;

                    OpenConnection(strConnString, out _conOraEDER);
                    strsql = "Update " + ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation) + " set PROCESSED = 'P'" + whereClause;

                    cmdExecuteQuery = new OracleCommand(strsql, _conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 500;
                    cmdExecuteQuery.ExecuteNonQuery();                   
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    ErrorCodeException ece = new ErrorCodeException(exp);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    if (cmdExecuteQuery != null)
                        cmdExecuteQuery.Dispose();
                    CloseConnection(_conOraEDER, out _conOraEDER);
                }
            }
        }
    
}
