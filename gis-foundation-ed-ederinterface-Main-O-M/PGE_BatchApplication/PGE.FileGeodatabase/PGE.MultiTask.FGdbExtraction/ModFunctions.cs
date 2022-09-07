using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Collections;
using System.Configuration;
using Microsoft.Win32;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using log4net;
using log4net.Config;

namespace PGE.MultiTask.FGdbExtraction
{
    class ModFunctions
    {
        private static StreamWriter mtxtStrLogFile;
        internal static string gstrMaster_Path = null;
        internal static string gstrUnderBinPath = null;
        internal static string gdtDateStamp = string.Empty;
        private const string cstSubDirectory = "/Logs/";
        internal static string gstrFgdbPath = string.Empty;
        internal static string gstrAssemblyPath = null;
        private const string cstMissingFldNmMsg = "Failed to access the following FieldName: ";
        private static InitLicenseManager m_AOLicenseInitializer = null;
        //protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog Log = LogManager.GetLogger(typeof(ModFunctions));

        private static string mSdeConnString = string.Empty;
        private static IFeatureWorkspace gpFeatWorkspace = null;
        private static IWorkspaceFactory mpWorFatry = null;
        internal static IFeatureClass gmAOI_FC = null;

        public static bool InitProcess()
        {
            try
            {
                //GurasVar.ErrorOccured = false; // gs remove
                //BasicConfigurator.Configure();
                InitMasterPath();
                ModFunctions.LogMessage("Starting Process");
                ModFunctions.LogMessage("Initialise license:");
                if (InitializeLicense() == true)
                {
                    //if (!connectToSDE())
                    if (!FileGdbWorkspaceFromPath())
                        return false;
                    LoadTables();
                }
                else 
                    throw new Exception("Unable to contact license server, please verify license server is available, stopping process.");
                return true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, "InitProcess"); return false; }
        }
        public static void InitMasterPath()
        {
            try
            {
                ModFunctions.gstrFgdbPath = ConfigurationManager.AppSettings["FGDB_PATH"];
                string strDLLFileName = Assembly.GetExecutingAssembly().GetModules(false)[0].FullyQualifiedName.ToLower();
                gstrAssemblyPath = strDLLFileName.Remove(strDLLFileName.LastIndexOf(@"\")+1);
                //gstrUnderBinPath = strDLLFileName.Substring(0, strDLLFileName.LastIndexOf("bin") + 3);
                //strDLLFileName = strDLLFileName.Substring(0, strDLLFileName.LastIndexOf("bin") - 1);

                //*Set Folder Path.
                //gstrMaster_Path = strDLLFileName;
                gstrMaster_Path = ModFunctions.gstrFgdbPath;
                gdtDateStamp = System.DateTime.Now.ToString("ddd, MMM dd, yyyy h:mm tt");              
                gdtDateStamp = gdtDateStamp.Replace("/", "-");
                gdtDateStamp = gdtDateStamp.Replace(":", "-");
                gdtDateStamp = gdtDateStamp.Replace(",", "-");
                ModFunctions.gdtDateStamp = ModFunctions.gdtDateStamp + "_Session";
            }
            catch (Exception ex)
            { ErrorMessage(ex, "InitMasterPath"); }
        }
      
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
                if (blnCreateFile == true)//
                    mtxtStrLogFile = File.CreateText(strCompletePath);
                else
                    mtxtStrLogFile = File.AppendText(strCompletePath);
                mtxtStrLogFile.WriteLine(strMaterial);
                mtxtStrLogFile.Close();
            }
            catch (Exception pException)
            {
                string sEx = pException.Message;
            }
        }
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
        public static void LogMessage(string strMessage)
        {
            LogMessage(strMessage, false);
        }
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

        public static string GetFieldValue(IFeature pFeature, int intFldIndex)
        {
            return GetFieldValue(pFeature, null, false, intFldIndex);
        }
        public static string GetFieldValue(IFeature pFeature, int intFldIndex, bool blnReturn_Zero_IfNotFound)
        {
            return GetFieldValue(pFeature, null, blnReturn_Zero_IfNotFound, intFldIndex);
        }
        public static string GetFieldValue(IFeature pFeature, string strFieldName, bool blnReturn_Zero_IfNotFound, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                    intFldIndex = pFeature.Fields.FindField(strFieldName);
                if (intFldIndex == -1)
                    throw new Exception(cstMissingFldNmMsg + strFieldName + ". FeatureClassName: " + pFeature.Class.AliasName);

                if (pFeature.get_Value(intFldIndex) != System.DBNull.Value)
                {
                    string strValue = pFeature.get_Value(intFldIndex).ToString();
                    if (strValue != "")
                        return strValue;
                }
                if (blnReturn_Zero_IfNotFound == true)
                    return "0";
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "GetFieldValue. Field Name: " + strFieldName); return null;
            }
        }
        public static string GetFieldValue(IRow pRow, int intFldIndex)
        {
            return GetFieldValue(pRow, null, false, intFldIndex);
        }
        public static string GetFieldValue(IRow pRow, int intFldIndex, bool blnReturn_Zero_IfNotFound)
        {
            return GetFieldValue(pRow, null, blnReturn_Zero_IfNotFound, intFldIndex);
        }
        public static string GetFieldValue(IRow pRow, string strFieldName, bool blnReturn_Zero_IfNotFound, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                {
                    //if (string.IsNullOrEmpty(strFieldName))
                    //    throw new Exception("Null field name passed.");
                    intFldIndex = pRow.Fields.FindField(strFieldName);
                }
                if (intFldIndex == -1)
                    throw new Exception(cstMissingFldNmMsg + strFieldName);

                if (pRow.get_Value(intFldIndex) != System.DBNull.Value)
                {
                    string strValue = pRow.get_Value(intFldIndex).ToString();
                    if (strValue != "")
                        return strValue;
                }
                if (blnReturn_Zero_IfNotFound == true)
                    return "0";
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "GetFieldValue. Field Name: " + strFieldName); return null;
            }
        }

        private static bool InitializeLicense()
        {
            ModFunctions.LogMessage("Initialise license:");
            bool flag = false;
            m_AOLicenseInitializer = new InitLicenseManager();
            try
            {
                //ESRI License Initializer generated code.
                if (m_AOLicenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, (Miner.Interop.mmLicensedProductCode)5))
                {
                    flag = true;
                    ModFunctions.LogMessage("SUCCESSFULLY GET LICENSE: " + flag);
                }

            }
            catch (Exception Ex)
            { ModFunctions.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); flag = false; }
            finally
            {
                //_licenseManager.Shutdown();
            }
            return flag;
        }
        private static bool connectToSDE()
        {
            ModFunctions.LogMessage("Connect to SDE.");
            try
            {
                if (string.IsNullOrEmpty(mSdeConnString))
                    throw new Exception("Cannot read sde file path from configuration file");

                ModFunctions.LogMessage("Try to connect to SDE: " + mSdeConnString);
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                mpWorFatry = Activator.CreateInstance(t) as IWorkspaceFactory;
                gpFeatWorkspace = (IFeatureWorkspace)mpWorFatry.OpenFromFile(mSdeConnString, 0);
                ModFunctions.LogMessage("Successfully connected to database");
                return true;
            }
            catch (Exception Ex)
            { ModFunctions.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }

        }
                
        public static bool FileGdbWorkspaceFromPath()
        {
            string strPath =null;
            try
            {
                //strPath = @"C:\Source\Offline Data\version_10_2\m1q.gdb";
                strPath = ConfigurationManager.AppSettings["FGDB_CONNECTION"];
                if (string.IsNullOrEmpty(strPath))
                    throw new Exception("Failed to get file geodatabase-path from config file.");
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                mpWorFatry = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                gpFeatWorkspace = (IFeatureWorkspace)mpWorFatry.OpenFromFile(strPath, 0);
                ModFunctions.LogMessage("\t Successfully connected to master file geodatabase");
                return true;
            }
            catch (Exception Ex)
            { ModFunctions.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); ModFunctions.LogMessage("Master Path: " + strPath); return false; }
        }
        private static void LoadTables()
        {
            ModFunctions.LogMessage("Load tables.");
            string strPlatMapNm = ConfigurationManager.AppSettings["AOI_FC"];
            gmAOI_FC = gpFeatWorkspace.OpenFeatureClass(strPlatMapNm);
        }
        public static void Dispose()
        {
            try { ModFunctions.m_AOLicenseInitializer.Shutdown(); } catch (Exception Exf) { }
            try
            {  
                ModFunctions.gmAOI_FC = null;
                ModFunctions.gpFeatWorkspace = null;
                ModFunctions.mpWorFatry = null;
                ModFunctions.mSdeConnString = null;
                ModFunctions.gstrFgdbPath = null;
            }
            catch (Exception Exf) { } 
        }


        
    }
}
