using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using PGE.BatchApplication.CWOSL.Interfaces;
//using Telvent.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

namespace PGE.BatchApplication.CWOSL
{
    public class Initialize : IValidateInstance
    {
        #region Private static variables
        private static readonly Log4NetLoggerCWOSL _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");

        private static bool _blnErrOccured = false;
        internal static string gdtDateStamp = null;
        private static string gExePath = null;
        private static StreamWriter mtxtStrLogFile;
        private const string cstSubDirectory = "Logs\\";
        internal static string _circuitName = null;
        private bool _isValidating = false;

        #endregion

        #region Encapsulated methods
        //Objectclasses
        public static ITable CWOSLTable { get; set; }
        public static ITable ServicePointTable { get; set; }
        public static IFeatureClass ServiceLocationFC { get; set; }
        public static IFeatureClass TransformerFC { get; set; }
        public static IFeatureClass SecondaryLoadPointFC { get; set; }
        public static IFeatureClass Parcels { get; set; }
        public static IFeatureClass LotLine { get; set; }

        public static IGeometricNetwork GeometricNetwork { get; set; }
        public SDEWorkspaceConnection SDEWorkspaceConnection { get; set; }
        public IWorkspace SDEWorkspace { get; set; }
        public string NetworkName { get; set; }

        //Field names
        public string ServiceLocationGUIDFldName { get; set; }
        public string SecondaryLoadPointGUIDFldName { get; set; }
        public string ServicePointIDFldName { get; set; }

        public string StreetNumberFldName { get; set; }
        public string StreetName1FldName { get; set; }
        public string CityFldName { get; set; }
        public string StateFldName { get; set; }
        public string ZipFldName { get; set; }

        //Object class names
        public string CWOSLTableName { get; set; }
        public string ServicePointTableName { get; set; }
        public string ServiceLocationFCName { get; set; }
        public string SecondaryLoadPointFCName { get; set; }
        public string TransformerFCName { get; set; }

        //Static Field indices
        public static int ServiceLocGUIDFldIdx { get; set; }       //Service Point Service Location GUID index.
        public static int SecondaryLoadPntGUIDFldIdx { get; set; } //Service Point Secondary Load GUID index.
        public static int ServicePointIDFldIdx { get; set; }
        public static int StreetNumberFldIdx { get; set; }
        public static int StreetName1FldIdx { get; set; }
        public static int CityFldIdx { get; set; }
        public static int StateFldIdx { get; set; }
        public static int ZipFldIdx { get; set; }
        public static int CGC12FldIdx { get; set; }
        public static int SLoc_GUIDFldIdx { get; set; }  //Service Loaction GUID index.
        public static int SLoad_GUIDFldIdx { get; set; } //Secondary Load GUID index.
        public static int TransformerInstallationTyp_FldIdx { get; set; }
        public static int TransformerCircuitID_FldIdx { get; set; }
        public static int PhaseDesignation_FldIdx { get; set; }

        public static int SL_STATUS_FldIdx { get; set; }
        public static int SL_SUBTYPECD_FldIdx { get; set; }
        public static int SL_JOBNUMBER_FldIdx { get; set; }
        public static int SL_CircuitID_FldIdx { get; set; }
        public static int SL_PhaseD_FldIdx { get; set; }
        public static int SL_CGC12_FldIdx { get; set; }

        public int NumberOfJobs { get; set; }
        public int LimitEditsInSession { get; set; }
        public string SDEConnectionFile { get; set; }
        public string OleDBConnectionString { get; set; }
        public string SessionName { get; set; }
        public int MaxMinutesToRun { get; set; }
        public int LimitSegUGServices { get; set; }
        public int IgnoreESRIGeocodeServiceError { get; set; } //0 is don't ignore, 1 is Ignore Error.
        public int ESRIGeocodeScore { get; set; } //0 is don't ignore, 1 is Ignore Error.
        public int BufferGeocodeInFeet { get; set; } //distance in feet, used to for Geocoding process.

        public int MaxParcelCustomerPoints { get; set; } //Count of CustomerPoints on a parcel.
        public int MaxParcelArea { get; set; } //Area of Parcel, in feet.

        public static DateTime AppStartDateTime { get; set; }
        //ME Q1 : 2020        
        public string EDERConnectionSDEUser { get; set; }
        //1368195 START
        //public string NoOfFeature { get; set; }
        //public string RunTime { get; set; }
        public string HFTDDomainName { get; set; }
        public string Division { get; set; }
        public string County { get; set; }
        public string SecLoadPoint { get; set; }
        public string LocalOfficeID { get; set; }
        public string Transformer { get; set; }
        public string SketchPreFeature { get; set; }
        public string FULLPROCESS { get; set; }
        public string CIRCUITID { get; set; }
        public string MAILFROM { get; set; }
        public string MAILTO { get; set; }
        public string MAILCC { get; set; }
        public string MAILSUBJECT { get; set; }
        public string SMTPSERVER { get; set; }
        public string QAQCMess { get; set; }
        public string SessionExist { get; set; }
        //1368195 END
        #endregion

        #region Initialize functions
        internal bool InitializeVariables()
        {
            _logger.Debug("");
            try
            {
                SDEWorkspace = SDEWorkspaceConnection.Workspace;
                if (SDEWorkspace == null)
                    throw new Exception("Failed to get ED SDE workspace");
                if (SDEWorkspaceConnection.WorkspaceLandbase == null)
                    throw new Exception("Failed to get Landbase SDE workspace");

                _logger.Debug("Opening CWOSL table...");
                ITable table = null;
                if (!InitializeTable(ref table, CWOSLTableName, SDEWorkspace as IFeatureWorkspace)) return false;
                CWOSLTable = table;

                _logger.Debug("Opening ServicePoint table...");
                table = null;
                if (!InitializeTable(ref table, ServicePointTableName, SDEWorkspace as IFeatureWorkspace)) return false;
                ServicePointTable = table;

                _logger.Debug("Opening Transformer FC...");
                IFeatureClass featCls = null;
                if (!InitializeFeatCls(ref featCls, TransformerFCName, SDEWorkspace as IFeatureWorkspace)) return false;
                TransformerFC = featCls;

                _logger.Debug("Opening ServiceLocation FC...");
                featCls = null;
                if (!InitializeFeatCls(ref featCls, ServiceLocationFCName, SDEWorkspace as IFeatureWorkspace)) return false;
                ServiceLocationFC = featCls;

                _logger.Debug("Opening SecondaryLoadPoint FC...");
                featCls = null;
                if (!InitializeFeatCls(ref featCls, SecondaryLoadPointFCName, SDEWorkspace as IFeatureWorkspace)) return false;
                SecondaryLoadPointFC = featCls;

                IFeatureDataset featureDataset = ServiceLocationFC.FeatureDataset;
                _logger.Debug("Opening FeatureDataset [ " + featureDataset + " ] GeometricNetwork [ " + NetworkName + " ]");
                INetworkCollection networkCollection = (INetworkCollection)featureDataset;
                GeometricNetwork = networkCollection.get_GeometricNetworkByName(NetworkName);
                if (GeometricNetwork == null)
                    throw new Exception("Failed to get following geometric network: " + NetworkName);

                //Initialise field indices
                int fldIndex = -1;
                HelperCls.GetFldIdx(ServicePointTable, ServiceLocationGUIDFldName, out fldIndex);
                ServiceLocGUIDFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, SecondaryLoadPointGUIDFldName, out fldIndex);
                SecondaryLoadPntGUIDFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, ServicePointIDFldName, out fldIndex);
                ServicePointIDFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, StreetNumberFldName, out fldIndex);
                StreetNumberFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, StreetName1FldName, out fldIndex);
                StreetName1FldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, CityFldName, out fldIndex);
                CityFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, StateFldName, out fldIndex);
                StateFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, ZipFldName, out fldIndex);
                ZipFldIdx = fldIndex;

                HelperCls.GetFldIdx(ServicePointTable, HelperCls.CGC12_FldName, out fldIndex);
                CGC12FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.GlobalID_FldName, out fldIndex);
                SLoc_GUIDFldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)SecondaryLoadPointFC, HelperCls.GlobalID_FldName, out fldIndex);
                SLoad_GUIDFldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)TransformerFC, HelperCls.TransformerType_FldName, out fldIndex);
                TransformerInstallationTyp_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)TransformerFC, HelperCls.CircuitID_FldName, out fldIndex);
                TransformerCircuitID_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)TransformerFC, HelperCls.PHASEDESIGNATION_FldName, out fldIndex);
                PhaseDesignation_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.STATUS_FldName, out fldIndex);
                SL_STATUS_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.SubTypeCD_FldName, out fldIndex);
                SL_SUBTYPECD_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.INSTALLJOBNUMBER_FldName, out fldIndex);
                SL_JOBNUMBER_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.CircuitID_FldName, out fldIndex);
                SL_CircuitID_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.PHASEDESIGNATION_FldName, out fldIndex);
                SL_PhaseD_FldIdx = fldIndex;

                HelperCls.GetFldIdx((ITable)ServiceLocationFC, HelperCls.CGC12_FldName, out fldIndex);
                SL_CGC12_FldIdx = fldIndex;

                //Initialize Parcel FC
                _logger.Debug("Opening Landbase Parcels FC...");
                table = null;
                if (!InitializeFeatCls(ref featCls, HelperCls.FC_Landbase_ParcelsName, SDEWorkspaceConnection.WorkspaceLandbase as IFeatureWorkspace)) return false;
                Parcels = featCls;

                _logger.Debug("Opening Landbase Lotlines FC...");
                table = null;
                if (!InitializeFeatCls(ref featCls, HelperCls.FC_Landbase_LotLinesName, SDEWorkspaceConnection.WorkspaceLandbase as IFeatureWorkspace)) return false;
                LotLine = featCls;

                //if(!ValidProperties())
                //    throw new Exception("One of the Property value in null/empty");
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            return (!ErrorOccured);
        }
        private bool ValidProperties()
        {
            //Check the if the propeties of this class are valid or not. Return true if value, else false.
            bool validProperty = true;
            try
            {
                Type t = this.GetType();
                var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(this, null);
                    if (prop.Name != "SDEWorkspaceConnection")
                    {
                        if (value == null || (string)value == "")
                        {
                            _logger.Error("Null/empty value in property: " + prop.Name);
                            validProperty = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); validProperty = false; }
            return validProperty;
        }


        internal bool InitializeFeatCls(ref IFeatureClass objectClass, string objectClassName, IFeatureWorkspace featureWorkspace)
        {
            try
            {
                if (string.IsNullOrEmpty(objectClassName))
                    throw new Exception("Class name is empty.");
                //IFeatureWorkspace featureWorkspace  = SDEWorkspaceConnection.Workspace as IFeatureWorkspace;
                objectClass = featureWorkspace.OpenFeatureClass(objectClassName);
                if (objectClass == null)
                    throw new Exception("Failed to access feature classs with the following name: " + objectClassName);
                return true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }
        internal bool InitializeTable(ref ITable table, string tableName, IFeatureWorkspace featureWorkspace)
        {
            try
            {
                if (string.IsNullOrEmpty(tableName))
                    throw new Exception("Table name is empty.");
                //IFeatureWorkspace featureWorkspace = SDEWorkspaceConnection.Workspace as IFeatureWorkspace;
                table = featureWorkspace.OpenTable(tableName);
                if (table == null)
                    throw new Exception("Failed to access feature classs with the following name: " + tableName);
                return true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IFeatureWorkspace GetWorkspace(string connectionFile, out IWorkspace pWS)
        {
            pWS = null;
            IFeatureWorkspace pFWS = null;

            try
            {
                // set feature workspace to the SDE connection                
                if (connectionFile.ToLower().EndsWith(".sde"))
                {
                    //if (!File.Exists(connectionFile))
                    //    WriteToLogfile("The connection file: " + connectionFile + " does not exist on the file system!");
                    SdeWorkspaceFactory sdeWorkspaceFactory = (SdeWorkspaceFactory)new SdeWorkspaceFactory();
                    pWS = sdeWorkspaceFactory.OpenFromFile(connectionFile, 0);
                }
                else if (connectionFile.ToLower().EndsWith(".gdb"))
                {
                    //if (!Directory.Exists(connectionFile))
                    //    WriteToLogfile("The file geodatabase: " + connectionFile + " does not exist on the file system!");

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
                    propertySet.SetProperty("DATABASE", @connectionFile);
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)obj;
                    pWS = workspaceFactory.OpenFromFile(@connectionFile, 0);
                }
                pFWS = (IFeatureWorkspace)pWS;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            return pFWS;
        }
        #endregion

        #region Internal functions
        internal string GetValue(IRow pRow, int intFldIndex)
        {
            return GetValue(pRow, intFldIndex, null);
        }

        internal string GetValue(IRow pRow, int intFldIndex, string strFieldName)
        {
            try
            {
                if (intFldIndex == -1)
                {
                    intFldIndex = pRow.Fields.FindField(strFieldName);
                }
                if (intFldIndex == -1)
                    throw new Exception("Missing field name: " + strFieldName);

                if (pRow.get_Value(intFldIndex) != System.DBNull.Value)
                {
                    object obj = pRow.get_Value(intFldIndex);
                    string strValue = null;
                    if (obj != null)
                    {
                        strValue = pRow.get_Value(intFldIndex).ToString();
                    }
                    if (!string.IsNullOrEmpty(strValue))
                        return strValue;
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Name: " + strFieldName);
                return null;
            }
        }

        internal void Dispose()
        {
            try
            {
                //Initialize._AOLicenseInitializer.Shutdown(); 
            }
            catch (Exception Exf) { }
            try
            {
                SDEWorkspace = null;
                GeometricNetwork = null;
            }
            catch (Exception Exf) { }
        }

        internal void ErrorMessage(Exception ex, string strFunctionName)
        {
            ErrorOccured = true;
            _logger.Error(ex.Message, ex);
        }

        internal bool ErrorOccured
        {
            get { return Initialize._blnErrOccured; }
            set { Initialize._blnErrOccured = value; }
        }

        internal void InitLogFilePath()
        {
            try
            {
                string exeFileName = Assembly.GetExecutingAssembly().GetModules(false)[0].FullyQualifiedName.ToLower();
                gExePath = exeFileName.Remove(exeFileName.LastIndexOf(@"\") + 1);
                //*Set Folder Path.
                gdtDateStamp = DateTime.Now.ToString("_yyyy_MMM_dd_ddd_hh_mm_tt");
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        internal void LogMessage(string strMessage)
        {
            LogMessage(strMessage, false);
        }
        internal void LogMessage(string strMessage, bool blnNewLine)
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
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }
        internal void WriteToFile(string strMaterial, bool blnCreateFile)
        {
            try
            {
                //string intJobID = _circuitName + "_" + gdtDateStamp;
                //string strLogsFolderPath = System.IO.Path.Combine(OutputFilePath, cstSubDirectory);
                //string strSuffix = "_Log";
                //string strFileName = intJobID + strSuffix + ".txt";
                //string strCompletePath = strLogsFolderPath + strFileName;
                //if (Directory.Exists(strLogsFolderPath) == false)
                //    Directory.CreateDirectory(strLogsFolderPath);
                //if (blnCreateFile == true)
                //    mtxtStrLogFile = File.CreateText(strCompletePath);
                //else
                //    mtxtStrLogFile = File.AppendText(strCompletePath);
                //mtxtStrLogFile.WriteLine(strMaterial);
                //mtxtStrLogFile.Close();
            }
            catch (Exception ex) { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        #endregion

        void IValidateInstance.Validate()
        {
            _logger.Info("Validating...");
            _isValidating = true;
            InitializeVariables();
        }
    }
}
