using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.CWOSLAssignmentTool.classes
{
    class DBhelper
    {
        public static Oracle.DataAccess.Client.OracleConnection conOraEDER = null;
        

        public static void OpenConnection(out OracleConnection objconOra)
        {
            objconOra = null;
            try
            {
                if (conOraEDER == null)
                {
                    // M4JF EDGISREARCH 919
                    string oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["EDER_ConnectionStr"].ConnectionString.ToUpper());

                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                    {
                        //_log.Info("Cannot read EDER Connection String path from configuration file");
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
            }

        }

        public static DataTable GetDataTableByQuery(string strQuery)
        {
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

    }
}
