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


namespace PGE.BatchApplication.GDBMAHBatchJobs
{
       
        public   class DBHelper
        {
            
            private static OracleConnection conOraEDER = null;
            

            private   void OpenConnection(out OracleConnection objconOra)
            {
                objconOra = null;
                try
                {
                    if (conOraEDER == null)
                    {
                    
                        //Check whether the connection string present in the config file or not if not then 
                        //show the message to the user.
                        if (string.IsNullOrEmpty(ReadConfigurations.OracleConnString))
                        {
                            Common._log.Error("Cannot read EDER Connection String path from configuration file");
                            throw new Exception("Connection string should not null,check the config file");
                        }

                        conOraEDER = new OracleConnection();
                        //make the database connection
                        conOraEDER.ConnectionString = ReadConfigurations.OracleConnString;
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
                    Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
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

           
            public   DataRow GetSingleDataRowByQuery(string strQuery)
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
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return dr;
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
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }

         
            public int GetSequence(string SequenceName)
            {
                Common CommonFuntions = new Common();
                int value = 0;
                try
                {
                    
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);

                    OracleCommand cmdzs = new OracleCommand("select " + SequenceName + ".nextval FROM dual", conOraEDER);
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
                    Common._log.Error("Exception encountered while picking the sequence," + ex.Message.ToString() + " at " + ex.StackTrace);

                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return value;
            }
            public int UpdateQuery(string strUpdateQry)
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
                    Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
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
