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

namespace PGE.BatchApplication.LoadDifference
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
                        if (string.IsNullOrEmpty(Program.sOracleConnectionstring))
                    {
                        Program._log.Error("Cannot read EDER Connection String path from configuration file");
                        throw new Exception("Connection string should not null,check the config file");
                    }

                    conOraEDER = new OracleConnection();
                    //make the database connection
                    if (string.IsNullOrEmpty(LoadDifference.oraclestr))
                    {
                       LoadDifference.oraclestr= Program.sOracleConnectionstring;
                    }
                    conOraEDER.ConnectionString = LoadDifference.oraclestr;
                    
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
                    Program._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                    throw ex ;
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
        internal void executeparmeterQuery(string updatequery,string replaceGUID, string oldGlobalID,string operatingnumber, string sapequipid,
            string scircuitid ,string swriter, string sshape,int Feat_OID,int iOBJETCID)
        {
            try
            {
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                using (OracleCommand command = new OracleCommand(updatequery, conOraEDER))
                {
                    
                    command.Parameters.Add(":FEAT_REPLACEGUID_OLD", replaceGUID);
                    command.Parameters.Add(":FEAT_GLOBALID", oldGlobalID);
                    command.Parameters.Add(":STATUS", "C");
                    command.Parameters.Add(":FEAT_OPERATINGNO_OLD", operatingnumber);
                    command.Parameters.Add(":FEAT_SAPEQIPID_OLD", sapequipid);
                    command.Parameters.Add(":FEAT_CIRCUITID_OLD", scircuitid);
                    command.Parameters.Add(":FEAT_FIELDS_LIST", swriter);
                    command.Parameters.Add(":FEAT_SHAPE_OLD", sshape);
                    command.Parameters.Add(":Feat_OID", Feat_OID);
                    command.Parameters.Add(":OBJECTID", iOBJETCID);
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

        internal void executeparmeterUpdateCaseQuery(string updatequery,string oldGlobalID,string replaceGUID, string operatingnumber, string sapequipid,
           string scircuitid, string swriter, string sshape, int Feat_OID,int iOBJECTID)
        {
            try
            {
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                using (OracleCommand command = new OracleCommand(updatequery, conOraEDER))
                {

                    command.Parameters.Add(":FEAT_REPLACEGUID_OLD", replaceGUID);
                    command.Parameters.Add(":FEAT_GLOBALID", oldGlobalID);
                    command.Parameters.Add(":STATUS", "C");
                    command.Parameters.Add(":FEAT_OPERATINGNO_OLD", operatingnumber);
                    command.Parameters.Add(":FEAT_SAPEQIPID_OLD", sapequipid);
                    command.Parameters.Add(":FEAT_CIRCUITID_OLD", scircuitid);
                    command.Parameters.Add(":FEAT_FIELDS_LIST", swriter);
                    command.Parameters.Add(":FEAT_SHAPE_OLD", sshape);
                    command.Parameters.Add(":Feat_OID", Feat_OID);
                    command.Parameters.Add(":OBJECTID", iOBJECTID);
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
                    Program._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
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
                    Program._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
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
                    Program._log.Error("Exception encountered while picking the sequence," + ex.Message.ToString() + " at " + ex.StackTrace);

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
                    Program._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

        internal DataRow GetfromGMC(string strfindQuery)
        {
            DataRow dr = null;
            try
            {
                string strconnectionstring = ReadEncryption.GetConnectionStr("INTDATAARCH@EDGMC");
                using (OracleConnection connection = new OracleConnection())
                {
                    DataTable DT = new DataTable();
                    connection.ConnectionString = strconnectionstring;
                    connection.Open();
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.LOAD_GIS_GUID";
                    oracleCommand.CommandText = strfindQuery;
                    oracleCommand.CommandType = CommandType.Text;


                    OracleDataAdapter daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = oracleCommand;
                    daOracle.Fill(DT);

                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            dr = DT.Rows[0];
                        }
                    }
                    if (connection != null)
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        connection.Dispose();
                        


                    }
                    //CloseConnection(conOraEDER, out conOraEDER);
                }
            }
            catch { }
            return dr;
        }
    }

    
}
