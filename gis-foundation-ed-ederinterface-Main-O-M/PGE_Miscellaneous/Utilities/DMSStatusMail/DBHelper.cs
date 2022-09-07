using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
using PGE_DBPasswordManagement;
namespace DMSStatusMail.DBHelper
{
       
        public static class cls_DBHelper
        {
            public static OracleConnection conOraEDER = null;
        //    public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
            public static bool CheckConnection()
            {
                try
                {
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                        
                    }
                    else
                    {
                        CloseConnection(conOraEDER, out conOraEDER);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            
            public static void OpenConnection(out OracleConnection objconOra)
            {
                objconOra = null;
                try
                {
                    if (conOraEDER == null)
                    {
                    // string[] oracleConnectionInfo = ConfigurationManager.AppSettings["ConnectionString"].Split(',');
                    string oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["ConnectionString"]);
                    //string[] oracleConnectionInfo = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["ConnectionString"]).Split(';');
                   // string oracleConnectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3] ;
                   // string connection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].ToUpper()) + "Pooling = true; Min Pool Size = 6;";
                   

                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                        {
                           // _log.Info("Cannot read EDER Connection String path from configuration file");
                            throw new Exception();
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
                  //  _log.Error("Exception encountered while Open the Oracle Connection", ex);
                    throw ex;
                }
            }

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

                catch (Exception ex)
                {
                    objconOra = null;
                    throw;
                }

            }
            public static int UpdateQuery(string strUpdateQry)
            {

                OpenConnection(out conOraEDER);

                int intUpdateResult = 0;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 0;
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    throw;
                    //_log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, ex);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

            public static bool ExecuteStoredProcedureCommand(string procedure_name, List<OracleParameter> parameters)
            {
                OracleCommand cmdExecuteSP = new OracleCommand();
                try
                {
                    //_log.Info("Executing Procedure " + procedure_name);
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
                    da.Fill(dt);
                  //  _log.Info("Executed Procedure " + procedure_name);
                    return true;


                }
                catch (Exception ex)
                {
                    //_log.Error(ex.Message);
                    
                    return false;
                }
                finally
                {
                    cmdExecuteSP.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
            }
            public static bool ExecuteStoredProcedureWithoutParameter(string procedure_name)
            {
                try
                {

                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }

                    OracleCommand cmd = new OracleCommand(procedure_name, conOraEDER);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    OracleParameter STATUS = new OracleParameter("@STATUS", OracleDbType.Varchar2);
                    STATUS.Direction = ParameterDirection.Output;
                    STATUS.Size = 20;
                    cmd.Parameters.Add(STATUS);
                    int value = cmd.ExecuteNonQuery();
                    string status = cmd.Parameters["@STATUS"].Value.ToString();
                    if (status == "SUCCESS")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    throw;
                    //_log.Error("Error in Function call_procedure: " + ex.Message);
                    return false;
                }

            }
            public static DataTable GetDataTableByStoredProcedure(string strStored_Proc_Name, OracleParameter[] prmInOutCollection)
            {
                DataTable DT = new DataTable();
                OracleCommand cmdSelect = null;
                OracleDataAdapter daOracle = null;
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);

                try
                {
                    cmdSelect = new OracleCommand();
                    cmdSelect.CommandType = CommandType.StoredProcedure;
                    cmdSelect.CommandTimeout = 0;
                    cmdSelect.CommandText = strStored_Proc_Name.ToUpper();
                    cmdSelect.Connection = conOraEDER;
                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdSelect;
                    if (prmInOutCollection != null)
                    {
                        foreach (OracleParameter prmInOut in prmInOutCollection)
                        {
                            cmdSelect.Parameters.Add(prmInOut);
                        }
                    }
                    daOracle.Fill(DT);

                }

                catch (Exception ex)
                {
                   // _log.Error("Exception encountered while get dataset from stored procedure", ex);
                }
                finally
                {
                    daOracle.Dispose();
                    cmdSelect.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return DT;
            }

            public static DataTable GetDataTableByQuery(string strQuery)
            {
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

                }
                catch (Exception ex)
                {
                    //_log.Error("Exception encountered while getting the data from query with querystring " + strQuery, ex);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }


            internal static OracleDataReader GetDataReaderByQuery(string strQuery)
            {
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
                  //  _log.Error("Exception encountered while getting the data from query with querystring " + strQuery, ex);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                   // CloseConnection(conOraEDER, out conOraEDER);

                }
                return DReader;
            }

            internal static void BulkCopyDataFromDataTable(DataTable dt, string tableName)
            {

                try
                {
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 300;
                        foreach (DataColumn column in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                        }
                        bulkCopy.WriteToServer(dt);
                    }
                }
                catch (Exception ex)
                {
                   // _log.Error("Exception encountered while BulkCopy the Data in Table " + tableName, ex);
                    //Common.SendMail(string.Format(ConfigurationManager.AppSettings["MAIL_BODY_DATA_LOAD_FAILED"], ex.Message), ConfigurationManager.AppSettings["MAIL_SUBJ_LOADFAIL"].ToString());
                    Environment.Exit(0);
                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }
            }

            internal static object UpdateQueryWithResult(string strUpdateQry)
            {
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
                    //_log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, ex);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return oid;
            }

            internal static object ExecuteScalerQuery(string strUpdateQry)
            {
                OpenConnection(out conOraEDER);

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
                    throw;
                    // _log.Error("Exception encountered while update Table with QueryString" + strUpdateQry, ex);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return exist;
            }
            internal static object UpdateSpacialQueryWithResult(OracleCommand cmdSQL)
            {
                OpenConnection(out conOraEDER);
                object oid = null;
                int iSuccess = 0;
                
                try
                {
                    cmdSQL.Connection = conOraEDER;
                    iSuccess = cmdSQL.ExecuteNonQuery();
                    oid = cmdSQL.Parameters["myOId"].Value;
                }

                catch (Exception ex)
                {
                    //_log.Error("Exception encountered while update Table" +ex);
                }
                finally
                {
                    
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return oid;
            }

            public static void ExecuteSpacialQuery(string query)
            {
                OpenConnection(out conOraEDER);
               
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand();
                    cmdExecuteQuery.Connection = conOraEDER;
                    cmdExecuteQuery.CommandText = query;
                    cmdExecuteQuery.CommandType = CommandType.Text;
                    cmdExecuteQuery.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    throw;
                    //_log.Error("Exception encountered while update Table" +ex);
                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);
                }
               
            }

        }

    
}
