// ========================================================================
// Copyright © 2021 PGE.
// <history> 
// Oracle Database Connections.(EDGISREARCH - 378)
// TCS M4JF 04/07/2021 Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Data;
using Oracle.DataAccess.Client;


namespace PGE.BatchApplication.OldSessionNotification
{

    public   class DBHelper
        {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static OracleConnection conOraEDER = null;

        /// <summary>
        /// Open Oracle connection .
        /// (M4JF) EDGISREARCH - 378
        /// </summary>
        /// <param name="objconOra"></param>
        private static void OpenConnection(out OracleConnection objconOra)
            {
                objconOra = null;
                try
                {
                    if (conOraEDER == null)
                    {
                    //string[] oracleConnectionInfo = ReadConfigurations.OracleConnString_EDER.Split(',');

                    // string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
                    string oracleConnectionString = ReadConfigurations.OracleConnString_EDER;
                        //Check whether the connection string present in the config file or not if not then 
                        //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                        {
                            Common._log.Error("Cannot read EDER Connection String path from configuration file");
                            
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
                    Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                }
            }

        /// <summary>
        /// Close Oracle connection .
        /// (M4JF) EDGISREARCH - 378
        /// </summary>
        /// <param name="objconOra"></param>
        /// <param name="objReleaseConn"></param>

        private static void CloseConnection(OracleConnection objconOra, out OracleConnection objReleaseConn)
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
                    _log.Error(ex.Message);
                    objconOra = null;
                }

            }
       
        /// <summary>
        /// Get datatable from query .
        /// (M4JF) EDGISREARCH - 378
        /// </summary>
        /// <param name="strQuery">Query to get datatable.</param>
        /// <returns></returns>
        public static  DataTable GetDataTable(string strQuery)
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
   
           
    }

    
}
