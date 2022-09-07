using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;

namespace Utility
{
    public class Robc
    {
        private static readonly string ECTPSSD_SP_NAME = "GENERATE_ECTPSSD_REPORT";
        private static readonly string EEP_SP_NAME = "GENERATE_EEP_REPORT";

        public static int LoadEctpStagingTables()
        {
            string errorMsg, errorCode = string.Empty;
            int retValue = 0;
            try
            {
                logMessage(string.Format("Start of Populating the ECTP reports staging tables for ROBC App."));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.ROBC_ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = ECTPSSD_SP_NAME;
                        cmd.Parameters.Add("ErrorMsg", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
                        cmd.Parameters.Add("ErrorCode", OracleDbType.Varchar2, ParameterDirection.Output).Size = 200;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        errorMsg = cmd.Parameters["ErrorMsg"].Value.ToString();
                        errorCode = cmd.Parameters["ErrorCode"].Value.ToString();
                        if ((!string.IsNullOrEmpty(errorCode) && errorCode != "null") || (!string.IsNullOrEmpty(errorMsg) && errorMsg != "null"))
                        {
                            logMessage(string.Format("Error occured during the process for populating ECTP reports' staging tables  :::: Process - {0} and Error Message::{1} and Error Code::{2}", ECTPSSD_SP_NAME, errorMsg, errorCode));
                            retValue = 1;
                        }
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the ECTP reports' staging table populating Process."));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the process for ECTP reports staging tables populating"), ex);
                retValue = 1;
            }
            return retValue;
        }

        public static int LoadEepStagingTables()
        {
            string errorMsg, errorCode = string.Empty;
            int retValue = 0;
            try
            {
                logMessage(string.Format("Start of Populating the EEP reports staging tables for ROBC App."));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.ROBC_ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {

                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = EEP_SP_NAME;
                        cmd.Parameters.Add("ErrorMsg", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
                        cmd.Parameters.Add("ErrorCode", OracleDbType.Varchar2, ParameterDirection.Output).Size = 200;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        errorMsg = cmd.Parameters["ErrorMsg"].Value.ToString();
                        errorCode = cmd.Parameters["ErrorCode"].Value.ToString();
                        if ((!string.IsNullOrEmpty(errorCode) && errorCode != "null") || (!string.IsNullOrEmpty(errorMsg) && errorMsg != "null"))
                        {
                            logMessage(string.Format("Error occured during the process for populating EEP reports' staging tables  :::: Process - {0} and Error Message::{1} and Error Code::{2}", EEP_SP_NAME, errorMsg, errorCode));
                            retValue = 1;
                        }
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the EEP reports' staging table populating Process."));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the process for EEP reports staging tables populating"), ex);
                retValue = 1;
            }
            return retValue;
        }

        private static void logMessage(string message)
        {
            Common.WriteLog(message, Constants.ROBC_ApplicationLogFile);
        }

        private static void logMessage(string message, Exception ex)
        {
            Common.WriteLog(message, Constants.ROBC_ApplicationLogFile);
            Common.WriteLog(ex.ToString(), Constants.ROBC_ApplicationLogFile);
        }

       
    }
}
