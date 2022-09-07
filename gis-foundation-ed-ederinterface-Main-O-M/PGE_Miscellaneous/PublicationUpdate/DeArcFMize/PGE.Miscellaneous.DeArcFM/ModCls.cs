// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Mod Class for Common functions
// TCS V3SF (EDGISREARC-767) 04/20/2021               Created
// </history>
// All rights reserved.
// ========================================================================
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Collections;
using System.Configuration;
using Microsoft.Win32;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Miscellaneous.DeArcFM
{
    class ModCls
    {
        #region Data Members
        private static StreamWriter mtxtStrLogFile;
        internal static string gstrMaster_Path = null;
        internal static string gstrUnderBinPath = null;
        internal static string gdtDateStamp = string.Empty;
        private const string cstSubDirectory = "/Logs/";
        internal static string strLogPath = string.Empty;
        internal static string gstrAssemblyPath = null;
        private const string cstMissingFldNmMsg = "Failed to access the following FieldName: ";
        private static InitLicenseManager m_AOLicenseInitializer = null;

        private static string mSdeConnString = string.Empty;
        internal static IFeatureWorkspace gpFeatWorkspace = null;
        private static IWorkspaceFactory mpWorFatry = null;
        internal static IFeatureClass gmAOI_FC = null;

        //protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(ModCls));
        #endregion

        /// <summary>
        /// Call Process to Initialize License
        /// </summary>
        /// <returns></returns>
        public static bool InitProcess()
        {
            try
            {
                InitMasterPath();
                ModCls.LogMessage("Starting Process");
                ModCls.LogMessage("Initialise license:");
                if (InitializeLicense() == true)
                {
                    ModCls.LogMessage("License Initialised");
                }
                else
                    throw new Exception("Unable to contact license server, please verify license server is available, stopping process.");
                return true;
            }
            catch (Exception ex)
            { 
                ErrorMessage(ex, "InitProcess"); 
                return false; 
            }
        }

        /// <summary>
        /// Set Folder Path for Log File
        /// </summary>
        public static void InitMasterPath()
        {
            try
            {
                string strDLLFileName = Assembly.GetExecutingAssembly().GetModules(false)[0].FullyQualifiedName.ToLower();
                gstrAssemblyPath = strDLLFileName.Remove(strDLLFileName.LastIndexOf(@"\") + 1);
                
                //*Set Folder Path.
                gstrMaster_Path = ModCls.strLogPath;
                gdtDateStamp = System.DateTime.Now.ToString("ddd, MMM dd, yyyy h:mm tt");
                gdtDateStamp = gdtDateStamp.Replace("/", "-");
                gdtDateStamp = gdtDateStamp.Replace(":", "-");
                gdtDateStamp = gdtDateStamp.Replace(",", "-");
                ModCls.gdtDateStamp = ModCls.gdtDateStamp + "_DeArcFM_Session";
            }
            catch (Exception ex)
            { ErrorMessage(ex, "InitMasterPath"); }
        }

        /// <summary>
        /// Write Logs to file
        /// </summary>
        /// <param name="strMaterial"></param>
        /// <param name="blnCreateFile"></param>
        public static void WriteToFile(string strMaterial, bool blnCreateFile)
        {
            try
            {
                string intJobID = gdtDateStamp;
                string strLogsFolderPath = gstrMaster_Path + cstSubDirectory;
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
                string sEx = pException.Message;
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
                string sEx = pException.Message;
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

        /// <summary>
        /// Initialize License
        /// ESRI: Advance
        /// Miner: ArcFM
        /// </summary>
        /// <returns></returns>
        private static bool InitializeLicense()
        {
            ModCls.LogMessage("Initialise license:");
            bool flag = false;
            m_AOLicenseInitializer = new InitLicenseManager();
            try
            {
                //ESRI License Initializer generated code.
                if (m_AOLicenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, (Miner.Interop.mmLicensedProductCode)5))
                {
                    flag = true;
                    ModCls.LogMessage("SUCCESSFULLY GET LICENSE: " + flag);
                }

            }
            catch (Exception Ex)
            { 
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); 
                flag = false; 
            }
            finally
            {
                //_licenseManager.Shutdown();
            }
            return flag;
        }
        
        /// <summary>
        /// Workspace from SDE Connection File
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static bool FileGdbWorkspaceFromPath(string strPath)
        {
            try
            {
                if (string.IsNullOrEmpty(strPath))
                    throw new Exception("Failed to get file SDE-path from config file.");
                
                //**Type of Workspace 
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                
                mpWorFatry = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                gpFeatWorkspace = (IFeatureWorkspace)mpWorFatry.OpenFromFile(strPath, 0);
                
                ModCls.LogMessage("\t Successfully connected to SDE geodatabase");
                
                return true;
            }
            catch (Exception Ex)
            { 
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); 
                ModCls.LogMessage("Failed Path: " + strPath); 
                return false; 
            }
        }

        /// <summary>
        /// Release License
        /// </summary>
        public static void Dispose()
        {
            try 
            { 
                ModCls.m_AOLicenseInitializer.Shutdown(); 
            }
            catch (Exception Exf) 
            {
                ModCls.ErrorMessage(Exf, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
            try
            {
                ModCls.gmAOI_FC = null;
                ModCls.gpFeatWorkspace = null;
                ModCls.mpWorFatry = null;
                ModCls.mSdeConnString = null;
                ModCls.strLogPath = null;
            }
            catch (Exception Exf) 
            {
                ModCls.ErrorMessage(Exf, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }
    }
}
