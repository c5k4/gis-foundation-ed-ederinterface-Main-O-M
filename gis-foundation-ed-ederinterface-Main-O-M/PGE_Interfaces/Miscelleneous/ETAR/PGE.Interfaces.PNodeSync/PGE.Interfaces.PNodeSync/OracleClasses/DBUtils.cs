using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using System.Threading.Tasks;
using Oracle.DataAccess.Client;
//using System.Data.OracleClient;
using System.Text.RegularExpressions;


namespace PGE.Interface.PNodeSync.OracleClasses
{
    class DBUtils
    {
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Feb-2019, ETAR Project - Moving FNM data from MDR to GIS
        /// Class that reads connection properties from config file.
        /// </summary>
        public static OracleConnection GetDBConnection(bool gisCon, bool mdrCon)
        {
            //string host = null;
            //int port = 0;
            //string sid = null;
            string serviceName = null;
            string user = null;
            string password = null;

            if (gisCon)
            {
                //host = ConfigSettings.GISOraclehost;
                //port = ConfigSettings.GISPort;
                //sid = ConfigSettings.GISSid;
                serviceName = ConfigSettings.GISService;
                user = ConfigSettings.GISUserName;
                password = ConfigSettings.GISPass;
                // m4jf edgisrearch 919
             //   password = Encryption.Decrypt(password);
            }
            else if (mdrCon)
            {
                //host = ConfigSettings.MDROraclehost;
                //port = ConfigSettings.GISPort;
                //sid = ConfigSettings.MDRSid;
                serviceName = ConfigSettings.MDRService;
                user = ConfigSettings.MDRUserName;
                password = ConfigSettings.MDRPass;
                // M4JF EDGISREARCH 919
              //  password = Encryption.Decrypt(password);
            }

            return DBOracleUtils.GetDBConnection(serviceName, user, password);
        }

        /// <summary>
        /// Execute the passed SQL.
        /// </summary>
        /// <param name="sql">SQL statement.</param>
        /// <param name="con">The connection the sql will be executed against.</param>
        internal static string ExecuteSql(OracleConnection con, string sql, bool writetoLlog = true)
        {
            //try
            //{
            //    if (writetoLlog)
            //        _logger.Info("Executing SQL: " + sql);
            //    OracleTransaction oraTransaction = con.BeginTransaction();
            //    OracleCommand command = new OracleCommand(sql, con);
            //    command.ExecuteNonQuery();
            //    oraTransaction.Commit();
            //}
            //catch (Exception Ex)
            //{ _logger.Error(Ex.Message + ". SQL: " + sql); }
            //return null;

            OracleTransaction oraTransaction = con.BeginTransaction();
            try
            {
                if (writetoLlog)
                    _logger.Info("Executing SQL: " + sql);
                //OracleTransaction oraTransaction = con.BeginTransaction();
                OracleCommand command = new OracleCommand(sql, con);
                command.ExecuteNonQuery();
                oraTransaction.Commit();
            }
            catch (Exception Ex)
            {
                try
                {
                    oraTransaction.Rollback();
                }
                catch (Exception rollBack)
                {
                    _logger.Error(rollBack.Message + " - rollback error.");
                }
                _logger.Error(Ex.Message + ". SQL: " + sql);
                throw Ex;
            }
            return null;
        }

        /// <summary>
        /// Execute the passed stored procedure.
        /// </summary>
        /// <param name="procedureName">procedure name.</param>
        /// <param name="oraConn">The connection procedure will be executed against.</param>
        internal static void ExecuteStoredProcedure(OracleConnection con, string procedureName)
        {
            //try
            //{
            //    _logger.Info("Executing stored procedure:  " + procedureName);
            //    // Create a Command object to call Get_Emp_No function.
            //    OracleCommand cmd = new OracleCommand(procedureName, con);

            //    // CommandType is StoredProcedure
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    // Call procedure.
            //    cmd.ExecuteNonQuery();
            //}
            //catch (Exception Ex)
            //{ _logger.Error(Ex.Message); }

            try
            {
                ExecuteProcedure(con, procedureName, null);
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing stored procedure " + procedureName + ". Message: " + ex.Message);
                Console.WriteLine("Error executing stored procedure " + procedureName + ". Message: " + ex.Message);
                throw ex;
            }
        }

        internal static void ExecuteProcedure(OracleConnection oraConn, string procedure, List<OracleParameter> oracleParameters =  null)
        {
            //OracleConnection oraConn = null;
            int rowsChanged = 0;
            //string[] connSepeartedList = Regex.Split(OracleConnectionString, ",");
            //string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);
            try
            {
                //oraConn = new OracleConnection(connection);
                //oraConn.Open();
                using (var cmd = new OracleCommand(procedure, oraConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (oracleParameters != null)
                    {
                        foreach (OracleParameter param in oracleParameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    _logger.Info("Executing Stored Procedure: " + procedure);
                    Console.WriteLine(DateTime.Now + ": Executing Stored Procedure: " + procedure);
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    _logger.Info("-rows effected " + rowsChanged.ToString());

                    _logger.Debug("Finished Executing Stored Procedure: " + procedure);
                    Console.WriteLine(DateTime.Now + ": Finished Executing Stored Procedure: " + procedure);
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing stored procedure: " + procedure, ex);
                Console.WriteLine("Error executing stored procedure: " + procedure, ex);
                throw ex;
            }
        }

    }

}
