// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Collection of Common methods used in program
// TCS V3SF (EDGISREARC-389) 04/28/2021               Created
// </history>
// All rights reserved.
// ========================================================================
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PGE_DBPasswordManagement;
using System.Text.RegularExpressions;

namespace PGE.BatchApplication.IntExecutionSummary
{
    /// <summary>
    /// Common Fumctions 
    /// </summary>
    class Common
    {
        #region Private Members
        private static Configuration pConfiguration = default;
        internal static string gdtDateStamp = string.Empty;
        internal static string baseFolderPath = null;
        private const string cstSubDirectory = "/Logs/";
        public static int OracleTimeout = 360;
        public static string delimiter = string.Empty;
        private static StreamWriter mtxtStrLogFile;
        public static DateTime DefaultDate = default;
        #endregion

        /// <summary>
        /// Set Folder Path for Log File and read config values
        /// </summary>
        public static void Initialize()
        {
            StringBuilder logPath = default;
            ExeConfigurationFileMap objExeConfigMap = default;
            try
            {
                //Set Config Path
                objExeConfigMap = new ExeConfigurationFileMap();
                if (File.Exists(@"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool\PGE.BatchApplication.IntExecutionSummary.exe.config"))
                    objExeConfigMap.ExeConfigFilename = @"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool\PGE.BatchApplication.IntExecutionSummary.exe.config";
                else if (File.Exists(@"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool\PGE.BatchApplication.IntExecutionSummary.exe.config"))
                    objExeConfigMap.ExeConfigFilename = @"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool\PGE.BatchApplication.IntExecutionSummary.exe.config";

                //Console.WriteLine("objExeConfigMap.ExeConfigFilename : "+ objExeConfigMap.ExeConfigFilename);

                pConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objExeConfigMap, ConfigurationUserLevel.None);
                if (!pConfiguration.HasFile)
                {
                    throw new Exception("Config File not found. Exiting");
                }

                //Set Log Path
                try
                {
                    logPath = new StringBuilder(pConfiguration.AppSettings.Settings["LOG_Path"].Value);
                }
                catch (NullReferenceException ex)
                {
                    if (Directory.Exists(@"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool"))
                        logPath = new StringBuilder(@"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool");
                    else if (Directory.Exists(@"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool"))
                        logPath = new StringBuilder(@"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\PGE.IntExecutionSummaryTool");
                    else
                        throw new Exception("Log Path not set/Invalid installation Directory");

                }
                //logPath = new StringBuilder(String.IsNullOrWhiteSpace(Convert.ToString(pConfiguration.AppSettings.Settings["LOG_Path"].Value)) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : pConfiguration.AppSettings.Settings["LOG_Path"].Value); 
                baseFolderPath = Convert.ToString(logPath);

                //Console.WriteLine("logPath : " + logPath);
                //Get date time for log file name
                gdtDateStamp = System.DateTime.Now.ToString("ddd, MMM dd, yyyy h:mm tt");
                gdtDateStamp = gdtDateStamp.Replace("/", "-");
                gdtDateStamp = gdtDateStamp.Replace(":", "-");
                gdtDateStamp = gdtDateStamp.Replace(",", "-");
                gdtDateStamp = gdtDateStamp + "_IntExecutionSummary";

                //Initialize string delimiter
                delimiter = pConfiguration.AppSettings.Settings["delimiter"].Value;

                DefaultDate = Convert.ToDateTime(pConfiguration.AppSettings.Settings["Default_LastRun"].Value);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "InitMasterPath");
            }
        }

        /// <summary>
        /// Create Database Connection
        /// </summary>
        /// <returns></returns>
        public static OracleConnection GetDBConnection()
        {

            // m4jf edgisrearch 919 - get oracle connection string using Password Management tool
            string oracleConnectionString = ReadEncryption.GetConnectionStr(pConfiguration.AppSettings.Settings["EDGMC_ConnectionStr"].Value.ToUpper());

            OracleConnection oraConn = new OracleConnection();

            if ((oraConn != null) && (oraConn.State != ConnectionState.Open))
            {
                try
                {
                    oraConn = new OracleConnection(oracleConnectionString);
                    Common.LogMessage("Connecting Database [" + pConfiguration.AppSettings.Settings["EDGMC_ConnectionStr"].Value + "]...");
                    oraConn.Open();
                    Common.LogMessage("Database [" + pConfiguration.AppSettings.Settings["EDGMC_ConnectionStr"].Value + "] connection successful...");
                    using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = " + Common.OracleTimeout, oraConn))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Common.LogMessage("Error: Connecting Database [" + pConfiguration.AppSettings.Settings["EDGMC_ConnectionStr"].Value + "] -- ");
                    Common.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    throw;
                }
            }

            return oraConn;
        }

        /// <summary>
        /// Write Logs to File
        /// </summary>
        /// <param name="strMaterial"></param>
        /// <param name="blnCreateFile"></param>
        public static void WriteToFile(string strMaterial, bool blnCreateFile)
        {
            try
            {
                string intJobID = gdtDateStamp;
                string strLogsFolderPath = baseFolderPath + cstSubDirectory;
                string strSuffix = "_Log";
                string strFileName = intJobID + strSuffix + ".txt";
                string strCompletePath = strLogsFolderPath + strFileName;
                if (Directory.Exists(strLogsFolderPath) == false)
                    Directory.CreateDirectory(strLogsFolderPath);
                if (blnCreateFile == true)
                    mtxtStrLogFile = File.CreateText(strCompletePath);
                else
                    mtxtStrLogFile = File.AppendText(strCompletePath);
                mtxtStrLogFile.WriteLine(strMaterial);
                mtxtStrLogFile.Close();
                Console.WriteLine(strMaterial);
            }
            catch (Exception pException)
            {
                Console.WriteLine(pException.Message);
            }
        }

        /// <summary>
        /// Manages Error Logs to Log File
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strFunctionName"></param>
        public static void ErrorMessage(Exception ex, string strFunctionName)
        {
            try
            {
                //ModEvents.ErrorOccured = true;                 
                //ModEvents.AddErrorToList(ex, strFunctionName); 
                WriteToFile("------------------------------------------------------------------------------------------------", false);
                WriteToFile("[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")", false);
                WriteToFile("------------------------------------------------------------------------------------------------", false);
            }
            catch (Exception pException)
            {
                Console.WriteLine(pException.Message);
            }
        }

        /// <summary>
        /// Manages Message Logs to Log File
        /// </summary>
        /// <param name="strMessage"></param>
        public static void LogMessage(string strMessage)
        {
            LogMessage(strMessage, false);
        }

        /// <summary>
        /// Manages Message Logs to Log File
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="blnNewLine"></param>
        public static void LogMessage(string strMessage, bool blnNewLine)
        {
            try
            {
                //mpLog.Info(strMessage);
            }
            catch (Exception ex)
            { ErrorMessage(ex, "LogMessage"); }

            try
            {
                string strNewLine = "";
                if (blnNewLine == true)
                    strNewLine = "\r\n";

                WriteToFile(strNewLine + "[" + DateTime.Now + "] " + strMessage, false);
            }
            catch (Exception ex)
            { ErrorMessage(ex, "LogMessage"); }
        }

    }
}
