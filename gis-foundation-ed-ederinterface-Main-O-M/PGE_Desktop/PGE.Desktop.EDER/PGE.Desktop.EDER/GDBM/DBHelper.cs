// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Common PL SQL Functions
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;
using System.IO;
using System.Threading;

namespace PGE.Desktop.EDER.GDBM
{
       
        public   class DBHelper
        {
            
            private static OracleConnection conOraEDER = null;
            

            private   void OpenConnection(out OracleConnection objconOra)
            {
                objconOra = null;
                try
                {

                     string OracleConnString = CommonFunctions.gConnectionstring;
                    if(string.IsNullOrEmpty(OracleConnString))
                    {
                        CommonFunctions.gConnectionstring = ReadEncryption.GetConnectionStr("EDGIS@EDER");
                        OracleConnString = CommonFunctions.gConnectionstring;
                }
                   
                    if (string.IsNullOrEmpty(OracleConnString))
                    {
                        throw new Exception("Connection string should not null,For Exception check the log file");
                    }
                    if (conOraEDER == null)
                    {
                        conOraEDER = new OracleConnection();
                        //make the database connection
                        conOraEDER.ConnectionString = OracleConnString;
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
                    throw ex;
                }
            }

            private   void CloseConnection(OracleConnection objconOra, out OracleConnection objReleaseConn)
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
                }

            }
    /// <summary>
    /// Bulk Copy data
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
        internal bool BulkCopyDataFromDataTable(DataTable dt, string tableName)
        {
            bool retval = false;
            CommonFunctions CommonFuntions = new CommonFunctions();
            try
            {
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                {
                    bulkCopy.BulkCopyOptions = OracleBulkCopyOptions.UseInternalTransaction;
                    bulkCopy.BulkCopyTimeout = 60000;
                    bulkCopy.DestinationTableName = tableName;
                   
                    try
                    {
                        bulkCopy.WriteToServer(dt);
                        retval = true;
                    }
                    catch
                    {
                        int waitTime = 0;
                        waitTime = 30000;
                        Thread.Sleep(waitTime);
                        //CommonFuntions.WriteLine_Info("Trying one more time, Unprocessed Table update started ," + DateTime.Now);
                        bulkCopy.WriteToServer(dt);
                        retval = true;

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                CloseConnection(conOraEDER, out conOraEDER);

            }
            return retval;
        }

        public DataRow GetSingleDataRowByQuery(string strQuery)
            {
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
                    throw ex;
               }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return dr;
            }
        public void UpdateStatusWithPostedTime(string sVersioName)
        {
            try
            {
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);

                var oracleCommand = new OracleCommand(
                      "update " + SchemaInfo.General.pAHInfoTableName + " set STATUS=:STATUS,CAPTURE_DATE=:CAPTURE_DATE where VERSIONNAME=:VERSIONNAME and status ='N'",
                      conOraEDER);
                oracleCommand.Parameters.Add(new OracleParameter(":STATUS", "P"));
                oracleCommand.Parameters.Add(new OracleParameter(":CAPTURE_DATE", System.DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss")));
                oracleCommand.Parameters.Add(new OracleParameter(":VERSIONNAME", sVersioName));

                oracleCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection(conOraEDER, out conOraEDER);

            }

        }

        internal void executeparmeterQuery(string updatequery, string GLOBALID, int OLD_OID ,string featureclassname,
            string versionname, string sessionname,  string saction, string susername,long objID)
        {
            try
            {
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                using (OracleCommand command = new OracleCommand(updatequery, conOraEDER))
                {
                    command.Parameters.Add(":GLOBALID", GLOBALID);
                    command.Parameters.Add(":OLD_OID", OLD_OID);
                    command.Parameters.Add(":featureclassname", featureclassname.ToUpper());
                    command.Parameters.Add(":versionname", versionname);
                    command.Parameters.Add(":sessionname", sessionname);
                    command.Parameters.Add(":status", "N");
                    command.Parameters.Add(":saction", saction);
                    command.Parameters.Add(":susername", susername);
                    command.Parameters.Add(":OBJECTID", objID);

                    command.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               
                CloseConnection(conOraEDER, out conOraEDER);

            }
        }
        public    DataTable GetDataTable(string strQuery)
            {
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
                throw ex;
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }

         
          public int UpdateQuery(string strUpdateQry)
            {

                
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
                throw ex;

                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

            
        }

    
}
