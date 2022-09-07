using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data.Common;
using PGE_DBPasswordManagement;

namespace Powerbase_To_GIS
{

    public static class DBHelper
    {     

        /// <summary>
        /// Open Connection, if not opened already
        /// </summary>
        private static OracleConnection OpenConnection(string argStrConnectionString)
        {
            OracleConnection objconOra = null;
            //string strConnString = string.Empty;
            try
            {
                //string[] oracleConnectionInfo = argStrConnectionString.Split(',');
                //strConnString = "Data Source=" + oracleConnectionInfo[0] + ";User Id=" + oracleConnectionInfo[1] + ";password=" + oracleConnectionInfo[2];
                string oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["ConnectionString_pgedata"].ToUpper());
                if (string.IsNullOrEmpty(oracleConnectionString))
                {
                    Common._log.Error("Cannot read EDER Connection String path from configuration file");
                }
                else
                {
                    objconOra = new OracleConnection();
                    objconOra.ConnectionString = oracleConnectionString;
                }

                if (objconOra.State != ConnectionState.Open)
                {
                    objconOra.Open();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + "Exception : " + exp.Message.ToString() + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return objconOra;
        }
                
        /// <summary>
        /// This function closes an open connection.
        /// </summary>
        /// <param name="objconOra"></param>
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
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
            }
        }

        /// <summary>
        /// Execute a query, Get Datatable, extract the Datarow and return
        /// </summary>
        public static DataRow GetSingleDataRowByQuery(string argstrConnectionstring, string strQuery)
        {
            OracleConnection conOraEDER = null;
            OracleCommand cmdExecuteSQL = null;
            OracleDataAdapter daOracle = null;
            DataRow drGetData = null;       
            DataTable dtGetDatTable = new DataTable();           
            try
            {
                using (conOraEDER = OpenConnection(argstrConnectionstring))
                {
                    using (cmdExecuteSQL = new OracleCommand())
                    {
                        using (daOracle = new OracleDataAdapter())
                        {
                            cmdExecuteSQL.CommandType = CommandType.Text;
                            cmdExecuteSQL.CommandTimeout = 0;
                            cmdExecuteSQL.CommandText = strQuery;
                            cmdExecuteSQL.Connection = conOraEDER;

                            daOracle.SelectCommand = cmdExecuteSQL;
                            daOracle.Fill(dtGetDatTable);
                            if (dtGetDatTable != null)
                            {
                                if (dtGetDatTable.Rows.Count > 0)
                                {
                                    drGetData = dtGetDatTable.Rows[0];
                                    Common._log.Info("Successfully executed query  : " + strQuery);
                                }
                            }
                        }
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
                CloseConnection(conOraEDER);
            }
            return drGetData;
        }

        /// <summary>
        /// Fill Data Table based on Connection Strign and SQL Query
        /// </summary>
        public static DataTable GetDataTable(string argstrConnectionstring, string strQuery)
        {
            DataTable dtGetDatTable = null;
            DataSet dsData = null;
            OracleCommand cmdExecuteSQL = null;
            OracleDataAdapter daOracle = null;
            OracleConnection conOraEDER = null;
            try
            {
                using (conOraEDER = OpenConnection(argstrConnectionstring))
                {
                    using (cmdExecuteSQL = new OracleCommand())
                    {
                        using (dsData = new DataSet())
                        {
                             cmdExecuteSQL.CommandType = CommandType.Text;
                            cmdExecuteSQL.CommandTimeout = 0;
                            cmdExecuteSQL.CommandText = strQuery;
                            cmdExecuteSQL.Connection = conOraEDER;

                            daOracle = new OracleDataAdapter();
                            daOracle.SelectCommand = cmdExecuteSQL;
                            daOracle.Fill(dsData);
                            dtGetDatTable = dsData.Tables[0];

                            Common._log.Info("Successfully executed query  : " + strQuery);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                CloseConnection(conOraEDER);
            }
            return dtGetDatTable;
        }

        /// <summary>
        /// Execute Given Query on given database 
        /// </summary>
        public static int ExecuteQueryOnDatabase(string argstrConnectionstring, string strQuery)
        {
            OracleConnection conOraEDER = null;
            OracleCommand cmdExecuteQuery = null;
            int icountResults = 0;
            try
            {
                using (conOraEDER = OpenConnection(argstrConnectionstring))
                {
                    using (cmdExecuteQuery = new OracleCommand(strQuery, conOraEDER))
                    {
                        cmdExecuteQuery.CommandTimeout = 500;
                        icountResults = cmdExecuteQuery.ExecuteNonQuery();
                        Common._log.Info("Successfully executed query  : " + strQuery);
                        Common._log.Info("Record count updated  : " + icountResults);
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception encountered while update Table with QueryString" + strQuery, exp);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                CloseConnection(conOraEDER);
            }
            return icountResults;
        }

        /// <summary>
        /// This function stores the rejected records in the database table
        /// </summary>
        /// <param name="argdtRecords"></param>
        /// <returns></returns>
        public static bool ProcessRejectedRecords(DataTable argdtRecords)
        {
            bool bProcess = false;
            int ibatchID = 0;
            string strFeatureClass = null;
            string strGLOBALID = null;
            string strOperatingNumber = null;
            Int64 ipb_rlid = 0;
            string strAttributeName = null;
            string strAttributeValue = null;
            string strDateTime = null;
            string strComments = null;
            string sqlQueryToInsert = null;
            int maxBatchID = 0;
            OracleConnection conOraEDER = null;
            OracleCommand cmdExecuteQuery = null;
            int icountResults = 0;
            try
            {
                string sqlQuery = "select NVL(MAX(" + ReadConfigurations.col_BATCHID + "),0) as " + ReadConfigurations.col_MAXBATCHID + " from " + ReadConfigurations.GetValue(ReadConfigurations.TB_PB_GIS_REJECTED_RECORDS);
                string connstringpgedata = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata);
                System.Data.DataRow preturnRow = DBHelper.GetSingleDataRowByQuery(connstringpgedata, sqlQuery);
                if (preturnRow != null)
                {
                    maxBatchID = Convert.ToInt32(preturnRow[ReadConfigurations.col_MAXBATCHID]);
                    maxBatchID++;

                    Common._log.Info("Successfully executed query  : " + sqlQuery);
                }

                MainClass._ibatchID = maxBatchID;

                using (conOraEDER = OpenConnection(connstringpgedata))
                {
                    foreach (DataRow pRow in argdtRecords.Rows)
                    {
                        try
                        {
                            //Get Max batchID from database table
                            ibatchID = maxBatchID;
                            strFeatureClass = Convert.ToString(pRow[ReadConfigurations.col_FEATURECLASS]);
                            strGLOBALID = Convert.ToString(pRow[ReadConfigurations.col_GLOBALID]);
                            strOperatingNumber = Convert.ToString(pRow[ReadConfigurations.col_OPERATINGNUMBER]); 
                            ipb_rlid = Convert.ToInt64(pRow[ReadConfigurations.col_PB_RLID]);                                                         
                            strAttributeName = Convert.ToString(pRow[ReadConfigurations.col_ATTRIBUTENAME]);
                            strAttributeValue = Convert.ToString(pRow[ReadConfigurations.col_ATTRIBUTEVALUE]);
                            strComments = Convert.ToString(pRow[ReadConfigurations.col_COMMENTS]);
                            strDateTime = DateTime.Now.ToString();

                            sqlQueryToInsert = "INSERT INTO " + ReadConfigurations.GetValue(ReadConfigurations.TB_PB_GIS_REJECTED_RECORDS) + " values(" + ibatchID + ",'" + strFeatureClass + "','" + strGLOBALID + "','" + strOperatingNumber + "'," +ipb_rlid+",'"+ strAttributeName + "','" + strAttributeValue + "','" + strComments + "',sysdate)";

                            using (cmdExecuteQuery = new OracleCommand(sqlQueryToInsert, conOraEDER))
                            {
                                cmdExecuteQuery.CommandTimeout = 500;
                                icountResults = cmdExecuteQuery.ExecuteNonQuery();
                                Common._log.Info("Successfully executed query  : " + sqlQueryToInsert);
                            }                           
                        }
                        catch (System.Exception exp)
                        {
                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                            Common._log.Error("Error executing query (may be): " + sqlQueryToInsert);
                            ErrorCodeException ece = new ErrorCodeException(exp);
                            Environment.ExitCode = ece.CodeNumber;
                        }
                    }                  
                }
                
                bProcess = true;
            }
            catch (System.Exception exp)
            {
                bProcess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bProcess;
        }

        /// <summary>
        /// This function updates the status for all the records 
        /// </summary>
        /// <param name="argdtRecords"></param>
        /// <param name="connstringpgedata"></param>
        /// <returns></returns>
        public static bool UpdateStatusForRecords(DataTable argdtRecords, string connstringpgedata)
        {
            bool bProcess = false;         
            string strGLOBALID = null;
            string sqlQueryToUpdate = null;
            string strStatus = null;
            string strComments = null;
            OracleConnection conOraEDER = null;
            OracleCommand cmdExecuteQuery = null;
            int icountResults = 0;
            try
            {
                using (conOraEDER = OpenConnection(connstringpgedata))
                {
                    foreach (DataRow pRow in argdtRecords.Rows)
                    {
                        try
                        {                          
                            strGLOBALID = Convert.ToString(pRow[ReadConfigurations.col_GLOBALID]);
                            strStatus = Convert.ToString(pRow[ReadConfigurations.col_RECORD_STATUS]);
                            strComments = Convert.ToString(pRow[ReadConfigurations.col_COMMENTS]);

                            sqlQueryToUpdate = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage) + " SET " + ReadConfigurations.col_RECORD_STATUS + "=" + "'" + strStatus + "'," + ReadConfigurations.col_COMMENTS + "='" + strComments + "' WHERE " + ReadConfigurations.col_GLOBALID + "=" + "'" + strGLOBALID + "'";

                            using (cmdExecuteQuery = new OracleCommand(sqlQueryToUpdate, conOraEDER))
                            {
                                cmdExecuteQuery.CommandTimeout = 500;
                                icountResults = cmdExecuteQuery.ExecuteNonQuery();
                                Common._log.Info("Successfully executed query  : " + sqlQueryToUpdate);
                            }
                        }
                        catch (System.Exception exp)
                        {
                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                            Common._log.Error("Error executing query (may be): " + sqlQueryToUpdate);
                            ErrorCodeException ece = new ErrorCodeException(exp);
                            Environment.ExitCode = ece.CodeNumber;
                        }
                    }
                }

                bProcess = true;
            }
            catch (System.Exception exp)
            {
                bProcess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bProcess;
        }      
    }
}
