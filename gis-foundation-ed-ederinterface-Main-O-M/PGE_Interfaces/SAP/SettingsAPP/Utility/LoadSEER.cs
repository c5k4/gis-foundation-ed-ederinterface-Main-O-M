using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;

namespace Utility
{
    class LoadSEER
    {
        private static readonly string FEEDER_LOAD_SP_NAME ="GENERATE_FEEDER_LOAD";
        private static readonly string TRANSFORMER_LOAD_SP_NAME = "GENERATE_TRANSFORMER_LOAD";
        
        public static int LoadFeederStagingTables()
        {
            string errorMsg, errorCode = string.Empty;
            int retValue = 0;
            try
            {
                logMessage(string.Format("Start of Populating the LoadSEER reports staging tables for Feeder"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.LOADSEER_ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = FEEDER_LOAD_SP_NAME;
                        cmd.Parameters.Add("ErrorMsg", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
                        cmd.Parameters.Add("ErrorCode", OracleDbType.Varchar2, ParameterDirection.Output).Size = 200;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        errorMsg = cmd.Parameters["ErrorMsg"].Value.ToString();
                        errorCode = cmd.Parameters["ErrorCode"].Value.ToString();
                        if ((!string.IsNullOrEmpty(errorCode) && errorCode != "null") || (!string.IsNullOrEmpty(errorMsg) && errorMsg != "null"))
                        {
                            logMessage(string.Format("Error occured during the process for Feeder LoadSEER reports staging tables populating  :::: Process - {0} and Error Message::{1} and Error Code::{2}", FEEDER_LOAD_SP_NAME , errorMsg,errorCode));
                            retValue = 1;
                        }
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the Feeder LoadSEER Load Process"));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the process for Feeder LoadSEER reports staging tables populating"), ex);
                retValue = 1;
            }
            return retValue;
        }

        public static int LoadTransformerStagingTables()
        {
            string errorMsg, errorCode = string.Empty;
            int retValue = 0;
            try
            {
                logMessage(string.Format("Start of Populating the LoadSEER reports staging tables for Transformer"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.LOADSEER_ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = TRANSFORMER_LOAD_SP_NAME;
                        cmd.Parameters.Add("ErrorMsg", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
                        cmd.Parameters.Add("ErrorCode", OracleDbType.Varchar2, ParameterDirection.Output).Size = 200;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        errorMsg = cmd.Parameters["ErrorMsg"].Value.ToString();
                        errorCode = cmd.Parameters["ErrorCode"].Value.ToString();
                        if ((!string.IsNullOrEmpty(errorCode) && errorCode != "null") || (!string.IsNullOrEmpty(errorMsg) && errorMsg != "null"))
                        {
                            logMessage(string.Format("Error occured during the process for Feeder LoadSEER reports staging tables populating  :::: Process - {0} and Error Message::{1} and Error Code::{2}",TRANSFORMER_LOAD_SP_NAME, errorMsg, errorCode));
                            retValue = 1;
                        }
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the Transformer LoadSEER Load Process"));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the process for Transformer LoadSEER reports staging tables populating"), ex);
                retValue = 1;
            }
            return retValue;
        }
        private static void logMessage(string message)
        {
            Common.WriteLog(message, Constants.LOADSEER_ApplicationLogFile);
        }

        private static void logMessage(string message, Exception ex)
        {
            Common.WriteLog(message, Constants.LOADSEER_ApplicationLogFile);
            Common.WriteLog(ex.ToString(), Constants.LOADSEER_ApplicationLogFile);
        }

    }
}
