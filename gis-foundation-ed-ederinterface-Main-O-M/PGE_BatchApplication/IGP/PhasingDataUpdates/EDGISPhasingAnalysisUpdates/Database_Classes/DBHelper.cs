using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data.Common;
using System.Threading;
using PGE.BatchApplication.IGPPhaseUpdate.Utility_Classes;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
       
        public  class DBHelper
        {
            
            
            public  OracleConnection conOraEDER = null;

           
           
         
            private  void OpenConnection(out OracleConnection objconOra)
            {
                Common CommonFuntions = new Common();
                objconOra = null;
                try
                {
                    if (conOraEDER == null)
                    {
                    // M4JF EDGISREARCH 919
                    // string[] oracleConnectionInfo = ReadConfigurations.OracleConnString.Split(',');

                    // Changes for IGP Issue : Database Error changing pooling = true
                    //string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
                    // string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=true";
                    string oracleConnectionString = ReadConfigurations.OracleConnString;
                        //Check whether the connection string present in the config file or not if not then 
                        //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                        {
                            CommonFuntions.WriteLine_Error("Cannot read EDER Connection String path from configuration file");
                            
                        }

                        conOraEDER = new OracleConnection();
                        //make the database connection
                        conOraEDER.ConnectionString = oracleConnectionString;
                    }
                    else
                    {
                        objconOra = conOraEDER;
                    }
                    if (objconOra.State != ConnectionState.Open)
                    {
                        objconOra.Open();

                    }

                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);

                }
            }

            // Changes for IGP Issue : Database Error 
            // Function to return conncetion
            public static OracleConnection GetOracleConnection()
            {
                Common CommonFuntions = new Common();
                OracleConnection objconOra = null;
                string[] oracleConnectionInfo = null;
                string oracleConnectionString = null;
                try
                {
                // M4JF EDGISREARCH 919
                // oracleConnectionInfo = ReadConfigurations.OracleConnString.Split(',');
                //oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=true";
                oracleConnectionString = ReadConfigurations.OracleConnString;
                    //string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                if (string.IsNullOrEmpty(oracleConnectionString))
                    {
                        CommonFuntions.WriteLine_Error("Cannot read EDER Connection String path from configuration file");
                    }

                    objconOra = new OracleConnection();
                    objconOra.ConnectionString = oracleConnectionString;
                    objconOra.Open();
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while closing the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                return objconOra;
            }

            private  void CloseConnection(OracleConnection objconOra, out OracleConnection objReleaseConn)
            {
                objReleaseConn = null;
                Common CommonFuntions = new Common();
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

                catch (Exception ex)
                {

                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                
                    objconOra = null;
                }

            }

            // Changes for IGP Issue : Database Error 
            public static void CloseConnection(OracleConnection objconOra)
            {
                Common CommonFuntions = new Common();
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
                    CommonFuntions.WriteLine_Error(exp.Message+" at "+exp.StackTrace);
                }
            }

            public  int UpdateQuery(string strUpdateQry)
            {
                
                Common CommonFuntions = new Common();
                OpenConnection(out conOraEDER);

                int intUpdateResult = -1;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 0;
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

            public  bool ExecuteStoredProcedureCommand(string procedure_name, List<OracleParameter> parameters)
            {
                Common CommonFuntions = new Common();
                OracleCommand cmdExecuteSP = new OracleCommand();
                try
                {
                    //CommonFuntions.WriteLine_Info("Executing Procedure " + procedure_name);
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                    // connection.Open();
                    DataTable dt = new DataTable();

                    cmdExecuteSP.Connection = conOraEDER;                     
                    cmdExecuteSP.CommandText = procedure_name;
                    cmdExecuteSP.CommandType = System.Data.CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (OracleParameter param in parameters)
                        {
                            cmdExecuteSP.Parameters.Add(param);
                        }
                    }
                    int value = cmdExecuteSP.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(cmdExecuteSP);
                    try
                    {
                        da.Fill(dt);
                    }
                    catch 
                    {
                        da.Fill(dt);
                    }
                    //CommonFuntions.WriteLine_Info("Executed Procedure " + procedure_name);
                    return true;


                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error(ex.Message.ToString() + " at " + ex.StackTrace);

                    return false;
                }
                finally
                {
                    cmdExecuteSP.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
            }

            public  bool ExecuteStoredProcedureCommand_TRUnit(string procedure_name, List<OracleParameter> parameters)
            {
                int value = -1;
                Common CommonFuntions = new Common();
                OracleCommand cmdExecuteSP = new OracleCommand();
                try
                {
                    //CommonFuntions.WriteLine_Info("Executing Procedure " + procedure_name);
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                    // connection.Open();
                    DataTable dt = new DataTable();

                    cmdExecuteSP.Connection = conOraEDER;
                    cmdExecuteSP.CommandText = procedure_name;
                    cmdExecuteSP.CommandType = System.Data.CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (OracleParameter param in parameters)
                        {
                            cmdExecuteSP.Parameters.Add(param);
                        }
                    }
                    try
                    {
                          value = cmdExecuteSP.ExecuteNonQuery();
                    }
                    catch
                    {
                          value = cmdExecuteSP.ExecuteNonQuery();
                    }
                   
                    //OracleDataAdapter da = new OracleDataAdapter(cmdExecuteSP);
                    //da.Fill(dt);
                    //CommonFuntions.WriteLine_Info("Executed Procedure " + procedure_name);
                    return true;


                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error(ex.Message.ToString() + " at " + ex.StackTrace);

                    return false;
                }
                finally
                {
                    cmdExecuteSP.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
            }

            public  DataTable GetDataTableByQuery(string strQuery)
            {
                Common CommonFuntions = new Common();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(DT);

                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while getting the data from query with querystring " + strQuery + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }

            public DataTable GetDataTableByQuery(ref OracleConnection argConOraEDER,string strQuery)
            {
                Common CommonFuntions = new Common();
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    if (argConOraEDER.State != ConnectionState.Open)
                    {
                        CloseConnection(argConOraEDER);
                        argConOraEDER = GetOracleConnection();
                    }

                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = argConOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(DT);
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while getting the data from query with querystring " + strQuery + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    if (cmdExecuteSQL != null)
                    {
                        cmdExecuteSQL.Dispose();
                        cmdExecuteSQL = null;
                    }

                    if (daOracle != null)
                    {
                        daOracle.Dispose();
                        daOracle = null;
                    }   
                }
                return DT;
            }

            internal  OracleDataReader GetDataReaderByQuery(string strQuery)
            {
                Common CommonFuntions = new Common();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                OracleDataReader DReader = null;
                OracleCommand cmdExecuteSQL = new OracleCommand();

                try
                {

                    cmdExecuteSQL.Connection = conOraEDER;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.CommandType = CommandType.Text;

                    DReader = cmdExecuteSQL.ExecuteReader();
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while getting the data from query with querystring " + strQuery + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    // CloseConnection(conOraEDER, out conOraEDER);

                }
                return DReader;
            }

            internal  bool BulkCopyDataFromDataTable(DataTable dt, string tableName)
            {
                bool retval = false;
                Common CommonFuntions = new Common();
                try
                {
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 30000;
                        foreach (DataColumn column in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                        }
                        try
                        {                          
                            bulkCopy.WriteToServer(dt);
                            retval = true;
                        }
                        catch 
                        {
                            int waitTime = 0;
                            waitTime = Convert.ToInt32(ConfigurationManager.AppSettings["WAIT_TIME"]);
                            Thread.Sleep(waitTime);
                            CommonFuntions.WriteLine_Info("Trying one more time, Unprocessed Table update started ," + DateTime.Now);
                            string strquery = "Delete from " + ReadConfigurations.UnprocessedTableName + " where BatchID='" + MainClass.Batch_Number + "'";
                            int iresult = UpdateQuery(strquery);
                            if (iresult != -1)
                            {
                                bulkCopy.WriteToServer(dt);
                                retval = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while BulkCopy the Data in Table " + tableName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    ErrorCodeException ece = new ErrorCodeException(ex);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return retval;
            }

            internal  object UpdateQueryWithResult(string strUpdateQry)
            {
                Common CommonFuntions = new Common();
                OpenConnection(out conOraEDER);
                object oid = null;
                int intUpdateResult = 0;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 0;
                    cmdExecuteQuery.Parameters.Add(new OracleParameter("myObjectId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                    oid = cmdExecuteQuery.Parameters["myObjectId"].Value;
                }

                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return oid;
            }

            internal  object ExecuteScalerQuery(string strUpdateQry)
            {
                OpenConnection(out conOraEDER);
                Common CommonFuntions = new Common();
                object exist = null;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand();
                    cmdExecuteQuery.Connection = conOraEDER;
                    cmdExecuteQuery.CommandText = strUpdateQry;
                    cmdExecuteQuery.CommandType = CommandType.Text;
                    exist = cmdExecuteQuery.ExecuteScalar();
                }

                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return exist;
            }

            public DataRow GetSingleDataRowByQuery(string strConnString, string strQuery)
            {
                DataRow drGetData = null;
                Common CommonFuntions = new Common();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(strConnString, out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable dtGetDatTable = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

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
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " |  Stack Trace | " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return drGetData;
            }

            public DataRow GetSingleDataRowByQuery(string strQuery)
            {
                Common CommonFuntions = new Common();
                DataRow dr = null;
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable() ;
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(DT);
                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            dr = DT.Rows[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return dr;
            }


            /// <summary>
            /// Fill Data Table based on Connection Strign and SQL Query
            /// </summary>
            public DataTable GetDataTableFromConnectionstring(string strConnString, string strQuery)
            {
                DataTable dtGetDatTable = null;
                DataSet dsData = null;
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                Common CommonFuntions = new Common();
                OracleConnection pOraConnection_Extra = null;
                try
                {
                    strQuery = strQuery.Trim();
                    ////Open Connection
                    //if (pOraConnection_Extra == null || pOraConnection_Extra.State == ConnectionState.Closed)
                    //{
                        //pOraConnection_Extra = new OracleConnection();
                        //pOraConnection_Extra.ConnectionString = strConnString;

                        OpenConnection(strConnString, out pOraConnection_Extra);
                    //}
                    if (pOraConnection_Extra.State == ConnectionState.Open)
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
                            cmdExecuteSQL.Connection = pOraConnection_Extra;

                            daOracle = new OracleDataAdapter();
                            daOracle.SelectCommand = cmdExecuteSQL;
                            daOracle.Fill(dsData);
                            dtGetDatTable = dsData.Tables[0];
                        }
                        catch (Exception exp)
                        {
                            CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + exp.Message + " |  Stack Trace | " + exp.StackTrace);
                            CommonFuntions.WriteLine_Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace);
                        }
                        finally
                        {
                            cmdExecuteSQL.Dispose();
                            daOracle.Dispose();
                            CloseConnection(pOraConnection_Extra, out pOraConnection_Extra);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                return dtGetDatTable;
            }

            public DataTable GetDataTable(string strQuery)
            {
                Common CommonFuntions = new Common();
                strQuery = strQuery.Trim();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(dsData);
                    DT = dsData.Tables[0];
                    
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                    ErrorCodeException ece = new ErrorCodeException(ex);
                    Environment.ExitCode = ece.CodeNumber;
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }

            public DataTable GetDataTable_QAQC(string strQuery)
            {
                Common CommonFuntions = new Common();
                strQuery = strQuery.Trim();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(dsData);
                    DT = dsData.Tables[0];

                }
                catch (Exception ex)
                {
                    //CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }


            public  void BulkCopyDataFromDataTableToDatabaseTable(DataTable dt, string tableName)
            {
                Common CommonFuntions = new Common();
                try
                {
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                  
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 30000;
                        foreach (DataColumn column in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                        }
                        try
                        {
                            bulkCopy.WriteToServer(dt);
                        }
                        catch
                        {
                            bulkCopy.WriteToServer(dt);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while BulkCopy the Data in Table " + tableName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    CloseConnection(conOraEDER,out conOraEDER);
                }
            }

            public  int GetSequence( string SequenceName)
            {
                Common CommonFuntions = new Common();
                int value = 0;
                try
                {

                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);

                    OracleCommand cmdzs = new OracleCommand("select "  +  SequenceName + ".nextval FROM dual" , conOraEDER);
                    cmdzs.CommandType = CommandType.Text;
                    var value_SP = cmdzs.ExecuteScalar();
                    if (value_SP.ToString() == "")
                    { value = 0; }
                    else
                    {
                        value = Convert.ToInt32(value_SP);
                    }

                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while picking the sequence," + ex.Message.ToString() + " at " + ex.StackTrace);
                    
                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }

                return value;
            }

            public void runValidations(string StoredProceduceName, string sBATCHID)
            {
                Common CommonFuntions = new Common();
                try
                {

                    OracleCommand objCmd = new OracleCommand();
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);

                    objCmd.Connection = conOraEDER;
                    objCmd.CommandText = StoredProceduceName;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter pParam = new OracleParameter("sBatchNumber", OracleDbType.Varchar2);
                    pParam.Direction = ParameterDirection.Input;
                    pParam.Size = 100;
                    pParam.Value = sBATCHID;

                    objCmd.Parameters.Add(pParam);

                    objCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while validating the data through Stored Procedure " + StoredProceduceName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    throw ex;

                }
                finally
                {
                    CloseConnection(conOraEDER,out conOraEDER);
                }
            }
         
           
           public  void BulkCopyDataFromDataTableJSON(DataTable dt, string tableName)
            {
                Common CommonFuntions = new Common();
                try
                {
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);

                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 3000;
                        foreach (DataColumn column in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                        }
                        bulkCopy.WriteToServer(dt);
                    }
                }

                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while BulkCopy the Data in Table " + tableName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    throw ex;
                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }
            }

            /// <summary>
            /// Execute Update Query for single updates
            /// </summary>
            public int UpdateQuery(string strConnString, string strUpdateQry)
            {
                Common CommonFuntions = new Common();
                OpenConnection(strConnString, out conOraEDER);

                int intUpdateResult = 0;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 500;
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    //CommonFuntions._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, exp);
                    CommonFuntions.WriteLine_Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

            private void OpenConnection(string strConnString1, out OracleConnection objconOra)
            {
                objconOra = null;
                string strConnString = string.Empty;
                Common CommonFuntions = new Common();
                try
                {
                //string[] oracleConnectionInfo = strConnString1.Split(',');
                //strConnString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
                strConnString = strConnString1;
                if (objconOra == null)
                    {

                        if (string.IsNullOrEmpty(strConnString))
                        {
                            CommonFuntions.WriteLine_Error("Cannot read EDER Connection String path from configuration file");

                        }
                        else
                        {
                            objconOra = new OracleConnection();
                            objconOra.ConnectionString = strConnString;
                        }
                    }
                    if (objconOra.State != ConnectionState.Open)
                    {
                        objconOra.Open();
                        //Common._log.Info("Fetching connection string.");
                        //Common._log.Info("Connection string to fetch data : " + strConnString);  
                    }
                }
                catch (Exception exp)
                {
                    CommonFuntions.WriteLine_Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace);
                    CommonFuntions.WriteLine_Error("Unhandled exception encountered while Open the Oracle Connection : --" + "Exception : " + exp.Message.ToString() + " |  Stack Trace | " + exp.StackTrace);
                }
            }

            internal bool BulkCopytoDMSTable(DataTable dt, string outputTableName)
            {
                bool retval = false;
                Common CommonFuntions = new Common();
                try
                {

                    // checking whether the table selected from the dataset exists in the database or not
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                     
                    // copying the data from datatable to database table
                    using (OracleBulkCopy bulkcopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkcopy.DestinationTableName = outputTableName;
                        bulkcopy.WriteToServer(dt);
                    }

                        retval = true;
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Exception encountered while BulkCopy the Data in Table " + outputTableName +  ex.Message.ToString());
                }
                finally
                {
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return retval;
            }

            internal DataTable GetDataTablefromSP(string procedure_name, List<OracleParameter> parameters)
            {
                DataTable dt = new DataTable();             
                Common CommonFuntions = new Common();
                OracleCommand cmdExecuteSP = new OracleCommand();
                try
                {
                    //CommonFuntions.WriteLine_Info("Executing Procedure " + procedure_name);
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                    // connection.Open();
                    cmdExecuteSP.Connection = conOraEDER;
                    cmdExecuteSP.CommandText = procedure_name;
                    cmdExecuteSP.CommandType = System.Data.CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (OracleParameter param in parameters)
                        {
                            cmdExecuteSP.Parameters.Add(param);
                        }
                    }
                    int value = cmdExecuteSP.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(cmdExecuteSP);
                    try
                    {
                        da.Fill(dt);
                    }
                    catch
                    {
                        da.Fill(dt);
                    }
                    //CommonFuntions.WriteLine_Info("Executed Procedure " + procedure_name);
                   


                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error(ex.Message.ToString() + " at " + ex.StackTrace);

                   
                }
                finally
                {
                    cmdExecuteSP.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return dt;
            }
        }

    
}
