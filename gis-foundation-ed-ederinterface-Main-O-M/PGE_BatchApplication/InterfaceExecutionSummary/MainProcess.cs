// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Interface Execution Summary Main Program
// TCS V1T8/V3SF (EDGISREARC-389) 01/09/2021 (DD/MM/YYYY)           Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.IntExecutionSummary
{
    class MainProcess
    {
        public MainProcess()
        {
            //Initialize values from App Config
            Common.Initialize();
        }

        public void RunIntExecutionSummary(string[] args)
        {
            #region Data Members
            string Interface_Name = default;
            string Interface_TYPE = default;
            string Interface_SYSTEM = default;
            string START_TIME = default;
            string END_TIME = default;
            string STATUS = default;
            string REMARKS = default;
            string PROCESS_TIME = default;

            List<string> Arguments = default;
            #endregion

            try
            {

                Common.LogMessage("Interface Execution Summary Started");

                if (args == null || args.Length == 0)
                {
                    throw new Exception("No arguments passed");
                }

                Common.LogMessage("Interface Execution Summary Started Arguments Passed : " + args[0]);

                Arguments = new List<string>();
                Arguments = Regex.Split(args[0], Common.delimiter).ToList();

                if (!(Arguments.Count == 7 || Arguments.Count == 8))
                {
                    throw new Exception("Incorrect number of arguments passed");
                }

                //Check to ensure nono of the fields are blank
                if (Arguments[0] != null)
                    Interface_Name = Arguments[0];
                else
                    throw new Exception("Interface_Name cannot be left blank");
                if (Arguments[1] != null)
                    Interface_TYPE = Arguments[1];
                else
                    throw new Exception("Interface_TYPE cannot be left blank");
                if (Arguments[2] != null)
                    Interface_SYSTEM = Arguments[2];
                else
                    throw new Exception("Interface_SYSTEM cannot be left blank");
                if (Arguments[3] != null)
                    START_TIME = Arguments[3];
                else
                    throw new Exception("START_TIME cannot be left blank");
                if (Arguments[4] != null)
                    END_TIME = Arguments[4];
                else
                    throw new Exception("END_TIME cannot be left blank");
                if (Arguments[5] != null)
                    STATUS = Arguments[5];
                else
                    throw new Exception("STATUS cannot be left blank");
                if (Arguments[6] != null)
                    REMARKS = Arguments[6];
                else
                    throw new Exception("REMARKS cannot be left blank");
                if (Arguments.Count == 8)
                    PROCESS_TIME = Arguments[7];
                else
                    PROCESS_TIME = "";

                //Add Entry in Int Exe Summary Table
                RunIntExecutionSummary(Interface_Name, Interface_TYPE, Interface_SYSTEM, START_TIME, END_TIME, STATUS, REMARKS, PROCESS_TIME);

            }
            catch (Exception ex)
            {
                Common.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new Exception(ex.Message + " at ( " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " ) " + ex.StackTrace);
            }
        }

        public void RunIntExecutionSummary(string Interface_Name, string Interface_TYPE, string Interface_SYSTEM, string START_TIME, string END_TIME, string STATUS, string REMARKS, [Optional] string PROCESS_TIME)
        {
            #region Data Members
            StringBuilder commandText = default;
            OracleConnection oraConn = default;
            OracleCommand command = default;
            #endregion

            try
            {

                Common.LogMessage("Interface Execution Summary Started");

                Common.LogMessage("Interface Execution Summary Started Arguments Passed : " + Interface_Name + ";" + Interface_TYPE + ";" + Interface_SYSTEM + ";" + START_TIME + ";" + END_TIME + ";" + STATUS + ";" + REMARKS + ";" + PROCESS_TIME);

                //Check to ensure nono of the fields are blank
                if (string.IsNullOrWhiteSpace(Interface_Name))
                    throw new Exception("Interface_Name cannot be left blank");
                if (string.IsNullOrWhiteSpace(Interface_TYPE))
                    throw new Exception("Interface_TYPE cannot be left blank");
                if (string.IsNullOrWhiteSpace(Interface_SYSTEM))
                    throw new Exception("Interface_SYSTEM cannot be left blank");
                if (string.IsNullOrWhiteSpace(START_TIME))
                    throw new Exception("START_TIME cannot be left blank");
                if (string.IsNullOrWhiteSpace(END_TIME))
                    throw new Exception("END_TIME cannot be left blank");
                if (string.IsNullOrWhiteSpace(STATUS))
                    throw new Exception("STATUS cannot be left blank");
                if (string.IsNullOrWhiteSpace(REMARKS))
                    throw new Exception("REMARKS cannot be left blank");
                if (string.IsNullOrWhiteSpace(PROCESS_TIME))
                    PROCESS_TIME = "";

                //Building Query
                commandText = new StringBuilder();
                commandText.Append("insert into INTDATAARCH.PGE_INTEXECUTIONSUMMARY ");
                commandText.Append("(Interface_Name,Interface_TYPE,Interface_SYSTEM,START_TIME,END_TIME,STATUS,REMARKS,PROCESS_TIME) ");
                commandText.Append("values(:Interface_Name,:Interface_TYPE,:Interface_SYSTEM,TO_DATE(:START_TIME,'mm/dd/yyyy hh24:mi:ss'),TO_DATE(:END_TIME,'mm/dd/yyyy hh24:mi:ss'),:STATUS,:REMARKS,TO_DATE(:PROCESS_TIME,'mm/dd/yyyy hh24:mi:ss'))");

                //Connect Oracle Database
                using (oraConn = Common.GetDBConnection())
                {
                    using (command = new OracleCommand(Convert.ToString(commandText), oraConn))
                    {
                        command.Parameters.Add(new OracleParameter("Interface_Name", Interface_Name));
                        command.Parameters.Add(new OracleParameter("Interface_TYPE", Interface_TYPE));
                        command.Parameters.Add(new OracleParameter("Interface_SYSTEM", Interface_SYSTEM));
                        command.Parameters.Add(new OracleParameter("START_TIME", START_TIME));
                        command.Parameters.Add(new OracleParameter("END_TIME", END_TIME));
                        command.Parameters.Add(new OracleParameter("STATUS", STATUS));
                        command.Parameters.Add(new OracleParameter("REMARKS", REMARKS));
                        command.Parameters.Add(new OracleParameter("PROCESS_TIME", PROCESS_TIME));

                        try
                        {
                            Common.LogMessage("Command Executing: " + command.CommandText);
                            command.ExecuteNonQuery();
                            Common.LogMessage("Command Executed: " + command.CommandText);
                        }
                        catch (Exception ex)
                        {
                            Common.LogMessage("Command Execution Failed");
                            throw ex;
                        }
                        finally
                        {
                            if (oraConn != null)
                            {
                                if (command.Connection.State == System.Data.ConnectionState.Open)
                                    command.Connection.Close();
                            }
                        }
                    }
                }

                Common.LogMessage("Interface Execution Summary Sucessfully Updated for " + Interface_Name);
            }
            catch (Exception ex)
            {
                Common.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new Exception(ex.Message + " at ( " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " ) " + ex.StackTrace);
            }
        }

        public string LastRunDate(string Interface_Name, [Optional] string WhereClause)
        {
            #region Data Members
            StringBuilder commandText = default;
            OracleConnection oraConn = default;
            OracleCommand command = default;
            OracleDataReader oracleDataReader = default;
            string lastDate = default;
            #endregion

            try
            {
                Common.LogMessage("Reading Last Run Date from Interface Execution Summary Started");

                commandText = new StringBuilder();
                //commandText.Append("Select MAX(TO_CHAR(PROCESS_TIME,'mm/dd/yyyy hh24:mi:ss')) from INTDATAARCH.PGE_INTEXECUTIONSUMMARY where INTERFACE_NAME = :INTERFACE_NAME and STATUS = 'D'");
                commandText.Append("select * from (SELECT TO_CHAR(PROCESS_TIME, 'mm/dd/yyyy hh24:mi:ss') FROM INTDATAARCH.PGE_INTEXECUTIONSUMMARY WHERE INTERFACE_NAME = :INTERFACE_NAME and PROCESS_TIME is not NULL and STATUS = 'D' order by PROCESS_TIME desc) where rownum = 1");

                if (!string.IsNullOrWhiteSpace(WhereClause))
                {
                    commandText.Clear();
                    commandText.Append("Select * from (SELECT TO_CHAR(PROCESS_TIME, 'mm/dd/yyyy hh24:mi:ss') FROM INTDATAARCH.PGE_INTEXECUTIONSUMMARY WHERE INTERFACE_NAME = :INTERFACE_NAME and PROCESS_TIME is not NULL and STATUS = 'D' AND "+ WhereClause + " order by PROCESS_TIME desc) where rownum = 1");
                    //commandText.Append(" AND :WhereClause");
                }

                //Connect Oracle Database
                using (oraConn = Common.GetDBConnection())
                {
                    using (command = new OracleCommand(Convert.ToString(commandText), oraConn))
                    {
                        command.Parameters.Add(new OracleParameter("Interface_Name", Interface_Name));
                        //if (!string.IsNullOrWhiteSpace(WhereClause))
                        //{
                        //    command.Parameters.Add(new OracleParameter("WhereClause", WhereClause));
                        //}
                        try
                        {
                            Common.LogMessage("Command Executing: " + command.CommandText);
                            oracleDataReader = command.ExecuteReader();
                            if (oracleDataReader.HasRows)
                            {
                                while (oracleDataReader.Read())
                                {
                                    lastDate = (Convert.ToString(oracleDataReader[0]));
                                }
                            }
                            Common.LogMessage("Command Executed: " + command.CommandText);
                        }
                        catch (Exception ex)
                        {
                            Common.LogMessage("Command Execution Failed");
                            throw ex;
                        }
                        finally
                        {
                            if (oraConn != null)
                            {
                                if (command.Connection.State == System.Data.ConnectionState.Open)
                                    command.Connection.Close();
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(lastDate))
                {
                    lastDate = Common.DefaultDate.ToString();
                    Common.LogMessage("No last Run Date found,default date provided for " + Interface_Name);
                }
                else
                    Common.LogMessage("Interface Execution Summary Sucessfully captured LastRunDate for " + Interface_Name);
            }
            catch (Exception ex)
            {
                Common.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                throw new Exception(ex.Message + " at ( " + System.Reflection.MethodInfo.GetCurrentMethod().Name + " ) " + ex.StackTrace);
            }

            return lastDate;
        }

    }
}
