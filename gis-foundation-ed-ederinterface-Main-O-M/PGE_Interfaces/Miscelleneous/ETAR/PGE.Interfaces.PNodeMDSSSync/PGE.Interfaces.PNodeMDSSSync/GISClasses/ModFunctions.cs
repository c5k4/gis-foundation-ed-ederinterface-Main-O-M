using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Collections;
using System.Configuration;
using Microsoft.Win32;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

using PGE.Interface.PNodeMDSSSync.PNodeSyncProcess;

namespace PGE.Interface.PNodeMDSSSync
{
    class ModFunctions
    {
        private static InitLicenseManager m_AOLicenseInitializer = null;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string mSdeConnString = string.Empty;        
        internal static IFeatureWorkspace gFeatWorkspace = null;
        private static IWorkspaceFactory mpWorFatry = null;
        private static bool mblnError = false;        

        public static bool InitProcess()
        {
            return InitProcess(true);
        }
        public static bool InitProcess(bool blnGISObjts)
        {
            try
            { 
                ModFunctions.LogMessage("Starting process");
                if(!blnGISObjts) return true;
                if (InitializeLicense() == true)
                {
                    if (!connectToSDE())
                        return false;
                }
                else
                    throw new Exception("Unable to contact license server, please verify license server is available, stopping process.");                    
                return true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, "InitProcess"); return false; }
        }
        
        public static void ErrorMessage(Exception ex, string strFunctionName)
        {
            try
            {
                ErrorOccured = true;
                InitializeProcess.ErrorMessage(ex, strFunctionName);
                //return;
                //WriteToFile("------------------------------------------------------------------------------------------------", false);
                //WriteToFile("[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")", false);
                //WriteToFile("------------------------------------------------------------------------------------------------",false);
                
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
                InitializeProcess.LogMessage(strMessage);
            }
            catch (Exception ex)
            { ErrorMessage(ex, "LogMessage"); }
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
            return flag;
        }
        internal static bool connectToSDE()
        {
            ModFunctions.LogMessage("Connect to SDE.");
            try
            {
                //mSdeConnString = @"C:\Users\t1mq\AppData\Roaming\ESRI\Desktop10.2\ArcCatalog\Test_SSSub1T_as_EDGIS.sde";
                //mSdeConnString = ConfigurationManager.AppSettings["SDECon"];
                if (string.IsNullOrEmpty(mSdeConnString))
                    throw new Exception("Cannot read sde file path from configuration file");

                ModFunctions.LogMessage("Try to connect to SDE: " + mSdeConnString);
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                mpWorFatry = Activator.CreateInstance(t) as IWorkspaceFactory;
                gFeatWorkspace = (IFeatureWorkspace)mpWorFatry.OpenFromFile(mSdeConnString, 0);
                ModFunctions.LogMessage("Successfully connected to database");
                return true;
            }
            catch (Exception Ex)
            { ModFunctions.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }

        }
        
        public static string GetFieldValue(IRow pRow, string strFieldName, bool blnReturn_Zero_IfNotFound, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                    intFldIndex = pRow.Fields.FindField(strFieldName);
                if (intFldIndex == -1)
                    throw new Exception("Failed to access the following FieldName: " + strFieldName);

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
                _logger.Error(ex.Message); 
                //_logger.Error(ex, "GetFieldValue. Field Name: " + strFieldName); return null;
                throw ex;
                return null;
            }
            
        }
        public static string GetFieldValue(IRow pRow, int intFldIndex)
        {
            return GetFieldValue(pRow, null, false, intFldIndex);
        }   

        public static Int32 GetFldIdx(IRow pRow, string strFieldName)
        {
            int intFldIndex = -1;
            try
            {
                if (intFldIndex == -1)
                {
                    //if (string.IsNullOrEmpty(strFieldName))
                    //    throw new Exception("Null field name passed.");
                    intFldIndex = pRow.Fields.FindField(strFieldName);

                }
                if (intFldIndex == -1)
                    throw new Exception("Missing field name: " + strFieldName);
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "GetFieldValue. Field Name: " + strFieldName);
            }
            return intFldIndex;
        }

        public static IFeatureCursor SpatialQuery(IFeatureClass featureClassIN, IGeometry searchGeo, esriSpatialRelEnum spatialRelation, string whereClause)
        {
            try
            {
                string strShpFld;
                IFeatureCursor featCur;
                IQueryFilter queryFilter;

                //Set the search geometry and shapefieldname.
                ISpatialFilter spatialFilter = new SpatialFilter();
                spatialFilter.Geometry = searchGeo;
                strShpFld = featureClassIN.ShapeFieldName;
                spatialFilter.GeometryField = strShpFld;

                spatialFilter.SpatialRel = spatialRelation;
                //If description != "" Then pSpatialFilter.SpatialRelDescription = description;
                if (!string.IsNullOrEmpty(whereClause)) spatialFilter.WhereClause = whereClause;

                //pSpatialFilter.SearchOrder = esriSearchOrderSpatial;
                queryFilter = spatialFilter;
                featCur = featureClassIN.Search(queryFilter, false);
                return (IFeatureCursor)featCur;
            }
            catch (Exception ex)
            { ModFunctions.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return null; }
        }

        public static void Dispose()
        {
            try { ModFunctions.m_AOLicenseInitializer.Shutdown(); }
            catch (Exception Exf) { }
            try
            {
                ModFunctions.gFeatWorkspace = null;
                ModFunctions.mpWorFatry = null;
                ModFunctions.mSdeConnString = null;
            }
            catch (Exception Exf) { }
        }
        public static bool ErrorOccured
        {
            get { return ModFunctions.mblnError; }
            set { ModFunctions.mblnError = value; }
        }
        public static string SdeConnString
        {
            get { return ModFunctions.mSdeConnString; }
            set { ModFunctions.mSdeConnString = value; }
        }
        

    }
}
