// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Interface Execution Summary API
// TCS V1T8/V3SF (EDGISREARC-389) 01/09/2021 (DD/MM/YYYY)           Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PGE.BatchApplication.IntExecutionSummary
{
    /// <summary>
    /// IntExecutionSummary API
    /// </summary>
    public static class ExecutionSummary
    {
        /// <summary>
        /// Populate Interface Execution Summary in IntExecutionSummary Table
        /// </summary>
        /// <param name="args"></param>
        public static void PopulateIntExecutionSummary(string[] args)
        {
            MainProcess mainProcess = new MainProcess();
            mainProcess.RunIntExecutionSummary(args);
        }

        /// <summary>
        /// Populate Interface Execution Summary in IntExecutionSummary Table
        /// </summary>
        /// <param name="Interface_Name">Name of Interface</param>
        /// <param name="Interface_TYPE">Inbound/Outbound wrt EDGIS/LBGIS</param>
        /// <param name="Interface_SYSTEM">Other side System</param>
        /// <param name="START_TIME">Start Time - When process starts Format:: 'mm/dd/yyyy hh24:mi:ss' </param>
        /// <param name="END_TIME">End Time - When process finished Format:: 'mm/dd/yyyy hh24:mi:ss'</param>
        /// <param name="STATUS">D-Done, E-Error (Status of completion)</param>
        /// <param name="REMARKS">Error Description in case of Error or processed record count</param>
        /// <param name="PROCESS_TIME">[Optional] Process Time - Time Process Starts like (Usually used for storing Last Run Date) Format:: 'mm/dd/yyyy hh24:mi:ss' </param>
        public static void PopulateIntExecutionSummary(string Interface_Name, string Interface_TYPE, string Interface_SYSTEM, string START_TIME, string END_TIME, string STATUS, string REMARKS, [Optional] string PROCESS_TIME)
        {
            MainProcess mainProcess = new MainProcess();
            mainProcess.RunIntExecutionSummary(Interface_Name, Interface_TYPE, Interface_SYSTEM, START_TIME, END_TIME, STATUS, REMARKS, PROCESS_TIME);
        }

        /// <summary>
        /// Populate Interface Execution Summary in IntExecutionSummary Table
        /// </summary>
        /// <param name="Interface_Name">Name of Interface</param>
        /// <param name="Interface_TYPE">Inbound/Outbound wrt EDGIS/LBGIS</param>
        /// <param name="Interface_SYSTEM">Other side System</param>
        /// <param name="START_TIME">Start Time - When process starts Format:: Type DateTime </param>
        /// <param name="END_TIME">End Time - When process finished Format:: Type DateTime </param>
        /// <param name="STATUS">D-Done, E-Error (Status of completion)</param>
        /// <param name="REMARKS">Error Description in case of Error or processed record count</param>
        /// <param name="PROCESS_TIME">[Optional] Process Time - Time Process Starts like (Usually used for storing Last Run Date) Format:: Type DateTime </param>
        public static void PopulateIntExecutionSummary(string Interface_Name, string Interface_TYPE, string Interface_SYSTEM, DateTime START_TIME, DateTime END_TIME, string STATUS, string REMARKS, [Optional] DateTime PROCESS_TIME)
        {
            MainProcess mainProcess = new MainProcess();
            mainProcess.RunIntExecutionSummary(Interface_Name, Interface_TYPE, Interface_SYSTEM, START_TIME.ToString("MM/dd/yyyy HH:mm:ss"), END_TIME.ToString("MM/dd/yyyy HH:mm:ss"), STATUS, REMARKS, PROCESS_TIME.ToString("MM/dd/yyyy HH:mm:ss"));
        }

        /// <summary>
        /// Populate Interface Execution Summary in IntExecutionSummary Table
        /// </summary>
        /// <param name="Interface_Name">Name of Interface</param>
        /// <param name="Interface_TYPE">Inbound/Outbound wrt EDGIS/LBGIS</param>
        /// <param name="Interface_SYSTEM">Other side System</param>
        /// <param name="START_TIME">Start Time - When process starts Format:: 'mm/dd/yyyy hh24:mi:ss' </param>
        /// <param name="END_TIME">End Time - When process finished Format:: 'mm/dd/yyyy hh24:mi:ss'</param>
        /// <param name="STATUS">D-Done, E-Error (Status of completion)</param>
        /// <param name="REMARKS">Error Description in case of Error or processed record count</param>
        /// <param name="PROCESS_TIME">[Optional] Process Time - Time Process Starts like (Usually used for storing Last Run Date) Format:: 'mm/dd/yyyy hh24:mi:ss' </param>
        public static void PopulateIntExecutionSummary(string Interface_Name, interface_type Interface_TYPE, string Interface_SYSTEM, string START_TIME, string END_TIME, status STATUS, string REMARKS, [Optional] string PROCESS_TIME)
        {
            MainProcess mainProcess = new MainProcess();
            mainProcess.RunIntExecutionSummary(Interface_Name, Convert.ToString(Interface_TYPE), Interface_SYSTEM, START_TIME, END_TIME, Convert.ToString(STATUS), REMARKS, PROCESS_TIME);
        }

        /// <summary>
        /// Populate Interface Execution Summary in IntExecutionSummary Table
        /// </summary>
        /// <param name="Interface_Name">Name of Interface</param>
        /// <param name="Interface_TYPE">Inbound/Outbound wrt EDGIS/LBGIS</param>
        /// <param name="Interface_SYSTEM">Other side System</param>
        /// <param name="START_TIME">Start Time - When process starts Format:: Type DateTime </param>
        /// <param name="END_TIME">End Time - When process finished Format:: Type DateTime </param>
        /// <param name="STATUS">D-Done, E-Error (Status of completion)</param>
        /// <param name="REMARKS">Error Description in case of Error or processed record count</param>
        /// <param name="PROCESS_TIME">[Optional] Process Time - Time Process Starts like (Usually used for storing Last Run Date) Format:: Type DateTime </param>
        public static void PopulateIntExecutionSummary(string Interface_Name, interface_type Interface_TYPE, string Interface_SYSTEM, DateTime START_TIME, DateTime END_TIME, status STATUS, string REMARKS, [Optional] DateTime PROCESS_TIME)
        {
            MainProcess mainProcess = new MainProcess();
            mainProcess.RunIntExecutionSummary(Interface_Name, Convert.ToString(Interface_TYPE), Interface_SYSTEM, START_TIME.ToString("MM/dd/yyyy HH:mm:ss"), END_TIME.ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToString(STATUS), REMARKS, PROCESS_TIME.ToString("MM/dd/yyyy HH:mm:ss"));
        }


        /// <summary>
        /// Returns last Sucessful Run date of Process for given Interface_Name
        /// Set Value of T as DateTime/string according to your requirement
        /// </summary>
        /// <typeparam name="T">DateTime/string</typeparam>
        /// <param name="Interface_Name">Name of Interface</param>
        /// <param name="WhereClause">[Optional] additional clause to query</param>
        /// <returns>DateTime/string depending on type of T</returns>
        public static T LastProcessExecutionDate<T>(string Interface_Name, [Optional] string WhereClause)
        {
            T returnVariable;
            MainProcess mainProcess = new MainProcess();
            returnVariable = (T)Convert.ChangeType(mainProcess.LastRunDate(Interface_Name, WhereClause), typeof(T));
            Console.WriteLine("Last Run Date for Interface (" + Interface_Name + ") :: " + returnVariable);
            return returnVariable;
        }
     
    }

    /// <summary>
    /// ExecutionSummary enum for Interface_TYPE wrt EDGIS/LBGIS
    /// </summary>
    public enum interface_type
    {
        /// <summary>
        /// Internal - Interface used within GIS
        /// </summary>
        Internal,
        /// <summary>
        /// Outbound - Outbound wrt EDGIS/LBGIS 
        /// </summary>
        Outbound,
        /// <summary>
        /// Inbound - Inbound wrt EDGIS/LBGIS
        /// </summary>
        Inbound
    }

    /// <summary>
    /// ExecutionSummary enum for STATUS of completion
    /// </summary>
    public enum status
    {
        /// <summary>
        /// D-Done
        /// </summary>
        D,
        /// <summary>
        /// E-Error 
        /// </summary>
        E
    }

}
