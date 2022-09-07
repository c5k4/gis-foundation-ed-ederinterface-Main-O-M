using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using System.Configuration;
using PGE.Common.Delivery.Framework;
using Miner;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.MapProductionAutomation.Common
{
    public static class Common
    {
        #region Public variables

        public static esriLicenseProductCode EsriLicense;
        public static mmLicensedProductCode ArcFMLicense;
        public static List<esriLicenseExtensionCode> EsriExtensions;

        public static string TableTIFUnifiedGrid = "EDGIS.PGE_UNIFIEDGRIDMAP";
        public static string FieldTIFMapDivision = "MAPDIVISION";
        public static string FieldTIFMapDistrict = "MAPDISTRICT";
        public static string FieldTIFLongMin = "LONG_MIN";
        public static string FieldTIFObjectId = "OBJECTID";
        public static string FieldTIFLongMax = "LONG_MAX";
        public static string FieldTIFLatMin = "LAT_MIN";
        public static string FieldTIFLatMax = "LAT_MAX";
        public static string FieldTIFMapNumber = "MAPNUMBER";
        public static string FieldTIFLastModified = "LASTMODIFIEDDATE";

        public static string TableMaintenancePlat = "EDGIS.MaintenancePlat";
        public static string TableChangeDetection = "";
        public static string TableUnifiedMapGrid = "";
        public static string TableServiceArea = "";
        public static string FieldDivision = "";
        public static string FieldDistrict = "";
        public static string FieldRegion = "";

        public static string FieldMapGridNo = "";
        public static string FieldMapGridScale = "";
        public const string FieldUnifiedGridNumber = "";
        public const string FieldHasError = "";

        public static string FieldChangeDetectionMapNumber = "";
        public static string FieldChangeDetectionError = "";
        public static string FieldChangeDetectionDate = "";
        public static string FieldChangeDetectionExportState = "";
        public static string FieldChangeDetectionMapType = "";
        public static string FieldChangeDetectionScale =  "";
        public static string FieldChangeDetectionPriority  = "";
        public static string FieldChangeDetectionMachineName  = "";
        public static string FieldChangeDetectionServiceToProcess  = "";
        public static string FieldChangeDetectionStartDate  = "";
        public static string FieldChangeDetectionEndDate  = "";
        public static string FieldChangeDetectionErrorMsg  = "";
        public static string FieldChangeDetectionOId = "";
        public static string FieldChangeDetectionFailureCount = ""; 

        public const string LogFileDirectory = "Map Production Log Files";
        public const string InputFileDirectory = "Map Production Input Files";
        public static string ExportFileDirectory = "";
        public static string TempFileDirectory = "";
        public static string MxdFolder = "";
        public static int MemoryThreshhold = 800;

        public static int MaxTries = 2;
        public static int MinFileSizePDF = 1100;
        public static int MinFileSizeTIFF = 40000;
        public static int ProcessTimeout = 2;
        public static string ConnectionString = "";
        public static string Password = "";
        public static bool OverWrite = true;
        public static string FileGDBPath = "";
        public static string SDEFilePath = "";
        public static bool EnableMapCache = true;
        public const int Resolution = 96;
        public static int MAXProcessCount = 0;
        public static int SleepTime = 0;
        #endregion

        #region Private static variables

        private static IMMRegistry _reg = new MMRegistry();

        #endregion

        #region Public Static methods

        /// <summary>
        /// Reads the application settings into static vars
        /// </summary>
        public static void ReadAppSettings(string configurationFile)
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configurationFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            try
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), config.AppSettings.Settings["EsriLicense"].Value);
            }
            catch (Exception e)
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), "esriLicenseProductCodeArcInfo");
            }
            try
            {
                List<esriLicenseExtensionCode> extensionsList = new List<esriLicenseExtensionCode>();
                string[] extensions = Regex.Split(config.AppSettings.Settings["EsriExtensions"].Value, ",");
                foreach (string extension in extensions)
                {
                    extensionsList.Add((esriLicenseExtensionCode)Enum.Parse(typeof(esriLicenseExtensionCode), extension));
                }
                EsriExtensions = extensionsList;
            }
            catch (Exception e)
            {
                //No default extensions
                EsriExtensions = new List<esriLicenseExtensionCode>();
            }
            try
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), config.AppSettings.Settings["ArcFMLicense"].Value);
            }
            catch (Exception e)
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), "mmLPArcFM");
            }

            try
            {
                EnableMapCache = Convert.ToBoolean(config.AppSettings.Settings["EnableMapCache"].Value);
            }
            catch
            {
                EnableMapCache = true;
            }

            SleepTime = Convert.ToInt32(config.AppSettings.Settings["SLEEPTIME"].Value);
            TableChangeDetection = config.AppSettings.Settings["ChangeDetectionTable"].Value;
            TableUnifiedMapGrid = config.AppSettings.Settings["UnifiedMapGridFeatureClass"].Value;
            TableServiceArea = config.AppSettings.Settings["ServiceAreaFeatureClass"].Value;

            FieldDivision = config.AppSettings.Settings["DivisionNameField"].Value;
            FieldDistrict = config.AppSettings.Settings["DistrictNameField"].Value;
            FieldRegion = config.AppSettings.Settings["RegionNameField"].Value;
            FieldMapGridNo = config.AppSettings.Settings["UnifiedMapGridNumberField"].Value;
            FieldMapGridScale = config.AppSettings.Settings["UnifiedMapGridScaleField"].Value;
            FieldChangeDetectionMapNumber = config.AppSettings.Settings["ChangeDetectionMapNumberField"].Value;
            FieldChangeDetectionError = config.AppSettings.Settings["ChangeDetectionErrorField"].Value;
            FieldChangeDetectionDate = config.AppSettings.Settings["ChangeDetectionDateField"].Value;
            FieldChangeDetectionExportState = config.AppSettings.Settings["ChangeDetectionStateField"].Value;
            FieldChangeDetectionMapType = config.AppSettings.Settings["ChangeDetectionMapTypeField"].Value;
            FieldChangeDetectionScale = config.AppSettings.Settings["ChangeDetectionScaleField"].Value;
            FieldChangeDetectionPriority = config.AppSettings.Settings["ChangeDetectionPriorityField"].Value;
            FieldChangeDetectionMachineName = config.AppSettings.Settings["ChangeDetectionMachineNameField"].Value;
            FieldChangeDetectionServiceToProcess = config.AppSettings.Settings["ChangeDetectionServiceToProcessField"].Value;
            FieldChangeDetectionStartDate = config.AppSettings.Settings["ChangeDetectionStartDateField"].Value;
            FieldChangeDetectionEndDate = config.AppSettings.Settings["ChangeDetectionEndDateField"].Value;
            FieldChangeDetectionErrorMsg = config.AppSettings.Settings["ChangeDetectionErrorMsgField"].Value;
            FieldChangeDetectionFailureCount = config.AppSettings.Settings["ChangeDetectionFailureCountField"].Value;
            FieldChangeDetectionOId = config.AppSettings.Settings["ChangeDetectionOIdField"].Value;

            ExportFileDirectory = config.AppSettings.Settings["ExportDirectory"].Value;
            TempFileDirectory = config.AppSettings.Settings["TempDirectory"].Value;

            MaxTries = Convert.ToInt32(config.AppSettings.Settings["MaxTries"].Value);
            MinFileSizePDF = Convert.ToInt32(config.AppSettings.Settings["MinFileSizePDF"].Value);
            MinFileSizeTIFF = Convert.ToInt32(config.AppSettings.Settings["MinFileSizeTIFF"].Value);
            MAXProcessCount = Convert.ToInt32(config.AppSettings.Settings["MAXPROCESSCOUNT"].Value);

            // m4jf edgisrearch 919 - get connection string using Password Management tool 
            //string passwordTemp = config.AppSettings.Settings["Password"].Value;
            // Password = EncryptionFacade.Decrypt(passwordTemp);
            // m4jf edgisrearch 919
            //string tempString = config.AppSettings.Settings["ConnectionString"].Value;

            //Append the decripted password in to form the full connectionstring
            // ConnectionString = ""; 
            ConnectionString = ReadEncryption.GetConnectionStr(config.AppSettings.Settings["EDER_ConnectionStr"].Value.ToUpper());
            //string[] connectParams = tempString.Split(';');
            //for (int i = 0; i < connectParams.Length; i++)
            //{
            //    if (connectParams[i] != string.Empty)
            //    {
            //        if (connectParams[i].ToLower().StartsWith("password"))
            //            ConnectionString += "password=" + Password + ";";
            //        else
            //            ConnectionString += connectParams[i] + ";";
            //    }
            //}

            OverWrite = Convert.ToBoolean(config.AppSettings.Settings["Overwrite"].Value);
            MxdFolder = config.AppSettings.Settings["MxdFolder"].Value;  

            try
            {
                MemoryThreshhold = Convert.ToInt32(config.AppSettings.Settings["MemoryThreshhold"].Value);
            }
            catch { }
        }

        /// <summary>
        /// Return the feature class specified from the workspace.  Returns null if feature class does not exist
        /// </summary>
        /// <param name="workspace">IWorkspace to search</param>
        /// <param name="FeatureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static void ResetPassword(string password, string configurationFile) 
        {
            try
            {
                //Open our configuration file
                bool hasUpdate = false; 
                //string assemLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);
                string encPwd = EncryptionFacade.Encrypt(password);
                XmlNodeList settingsList = xmlDoc.SelectNodes("//configuration/appSettings/add");
                for (int i = 0; i < settingsList.Count; i++)
                {
                    XmlNode node = settingsList[i];
                    string settingName = node.Attributes["key"].Value.ToLower();

                    if (settingName == "password")
                    {
                        node.Attributes["value"].InnerText = encPwd;
                        hasUpdate = true;
                        break; 
                    }
                }

                if (hasUpdate)
                    xmlDoc.Save(configurationFile); 

            }
            catch (Exception ex) 
            {
                throw new Exception("Error resetting password: " + ex.Message);
            }
        }

        /// <summary>
        /// Reads the outputformat, iscolor for the passed MXD 
        /// </summary>
        /// <returns></returns>
        public static void PopulateMXDAttributesFromConfigFile(string configurationFile,
            string MxdName, 
            string scale, 
            ref string hasData,
            ref string outputFormat,
            ref string outputName, 
            ref bool IsColor)
        {
            try
            {
                //Open our configuration file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];

                    //Grab the name of the Mxd
                    string curmxdName = node.Attributes["MxdName"].Value.ToLower();
                    string curScale = node.Attributes["Scale"].Value.ToLower();

                    if ((curmxdName == (MxdName).ToLower()) && 
                        (scale == curScale))
                    {
                        hasData = node.Attributes["HasDataFeatureClasses"].Value;
                        outputFormat = node.Attributes["OutputFormat"].Value;
                        outputName = node.Attributes["OutputName"].Value;
                        IsColor = Convert.ToBoolean(node.Attributes["IsColor"].Value);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning attributes from config file: " + ex.Message);
            }
        }

        /// <summary>
        /// When a process is just starting and has not yet exported a map 
        /// it assigns the map_type and scale by what the last digit of the 
        /// processId of the process is 
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="curMapScale"></param>
        /// <param name="curMapMXD"></param>

        public static void PopulateMapScaleAndMapMXDByProcessId(string configurationFile, int processId, ref string curMapScale, ref string curMapType, ref string curMapMXD)
        {
            try
            {
                //Get the last digit of the processId 
                string procIdString = processId.ToString();
                int lastDigit = Convert.ToInt32(procIdString.Substring(procIdString.Length - 1, 1));

                //Open our configuration file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);
                string processStartIndexes = "";
                string[] processStartIndexArr; 
                int processStartIndex = -1;
                string firstMapScale = "";
                string firstMapMXD = "";
                string firstMapType = "";

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];

                    //Grab the name of the Mxd
                    processStartIndexes = node.Attributes["ProcessStartIndex"].Value;
                    processStartIndexArr = processStartIndexes.Split(',');
                    bool foundProcess = false; 

                    if (i == 0)
                    {
                        firstMapScale = node.Attributes["Scale"].Value;
                        firstMapMXD = node.Attributes["MxdName"].Value.ToUpper();
                        firstMapType = node.Attributes["OutputName"].Value.ToUpper();
                    }

                    for (int j = 0; j < processStartIndexArr.Length; j++)
                    {
                        if (Int32.TryParse(processStartIndexArr[j], out processStartIndex)) 
                        {
                            if (processStartIndex == lastDigit)
                            {
                                curMapScale = node.Attributes["Scale"].Value;
                                curMapMXD = node.Attributes["MxdName"].Value.ToUpper();
                                curMapType = node.Attributes["OutputName"].Value.ToUpper();
                                foundProcess = true; 
                                break;
                            }
                        }                        
                    }

                    if (foundProcess)
                        break; 
                }

                //Make sure that a valid mxd and scale are returned even if incorrectly 
                //configured 
                if ((curMapMXD == "") || (curMapScale == ""))
                {
                    curMapMXD = firstMapMXD;
                    curMapScale = firstMapScale;
                    curMapType = firstMapType;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error determining the starting maptype and scale");
            }
        }

        /// <summary>
        /// Return the feature class specified from the workspace.  Returns null if feature class does not exist
        /// </summary>
        /// <param name="workspace">IWorkspace to search</param>
        /// <param name="FeatureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static IFeatureClass getFeatureClass(IWorkspace workspace, string FeatureClassName)
        {
            if (workspace != null)
            {
                IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
                if (featWorkspace != null)
                {
                    if (workspace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
                        FeatureClassName = GetShortDatasetName(FeatureClassName);
                    return featWorkspace.OpenFeatureClass(FeatureClassName);
                }
            }
            return null;
        }

        /// <summary>
        /// Return the table specified from the workspace.  Returns null if table does not exist
        /// </summary>
        /// <param name="workspace">IWorkspace to search</param>
        /// <param name="tableName">Name of the table to find</param>
        /// <returns></returns>
        public static ITable getTable(IWorkspace workspace, string tableName)
        {
            if (workspace != null)
            {
                IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
                if (featWorkspace != null)
                {
                    return featWorkspace.OpenTable(tableName);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the first instance of the IFeatureLayer that matches the request feature class name
        /// </summary>
        /// <param name="map">IMap to search</param>
        /// <param name="featureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static IFeatureLayer getFeatureLayerFromMap(IMap map, string featureClassName)
        {
            IFeatureLayer _returnLayer = null;
            //Get all of the IFeatureLayers
            IFeatureLayer featLayer = null;
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
           
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                 featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    
                    
                    if (ds.BrowseName.ToUpper().Trim() == featureClassName.ToUpper().Trim())
                    {
                        _returnLayer = featLayer;
                        break;
                    }
                }
                layer = enumLayer.Next();
            }
            if (_returnLayer == null)
            {
                for (int i = 0; i < (map as IMap).LayerCount; i++)
                {
                    ILayer ly = (map as IMap).get_Layer(i);
                    string ss = ly.Name;
                    if (ly is IFeatureLayer)
                    {
                        IDataset ds = ly as IDataset;
                        if (ds.BrowseName.ToUpper().Trim() == featureClassName.ToUpper().Trim())
                        {
                            _returnLayer = ly as IFeatureLayer ;
                            break;
                        }
                    }
                }
            }
            return _returnLayer;
        }       

        /// <summary>
        /// Returns a List of IFeatureLayers that match the provided feature class name
        /// </summary>
        /// <param name="map">IMap to search</param>
        /// <param name="featureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static List<IFeatureLayer> getFeatureLayersFromMap(IMap map, string featureClassName)
        {
            List<IFeatureLayer> featLayers = new List<IFeatureLayer>();

            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            string shortFeatureclassName = GetShortDatasetName(featureClassName).ToUpper(); 
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    if ((ds.BrowseName.ToUpper() == featureClassName.ToUpper()) ||
                        (ds.BrowseName.ToUpper() == shortFeatureclassName))
                    {
                        featLayers.Add(featLayer);
                    }
                }
                layer = enumLayer.Next();
            }
            return featLayers;
        }

        public static string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring(
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);
                }
                else
                {
                    shortDatasetName = datasetName;
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        /// <summary>
        /// Opens the specified stored display into the IMap
        /// </summary>
        /// <param name="SDName">Stored Display Name</param>
        /// <param name="workspace">Workspace containing the stored display</param>
        /// <param name="map">Map to load the stored display into</param>
        /// <returns></returns>
        public static bool OpenStoredDisplay(string SDName, IWorkspace workspace, IMap map)
        {
            IMMStoredDisplayManager sdManager = new MMStoredDisplayManagerClass();
            sdManager.Workspace = workspace;

            IMMEnumStoredDisplayName sdNameEnum = sdManager.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem);
            sdNameEnum.Reset();
            IMMStoredDisplayName sdName = sdNameEnum.Next();
            while (sdName != null)
            {
                if (sdName.Name.ToUpper() == SDName.ToUpper())
                {
                    break;
                }
                sdName = sdNameEnum.Next();
            }

            if (sdName == null)
            {
                sdNameEnum = sdManager.GetStoredDisplayNames(mmStoredDisplayType.mmSDTUser);
                sdNameEnum.Reset();
                sdName = sdNameEnum.Next();
                while (sdName != null)
                {
                    if (sdName.Name.ToUpper() == SDName.ToUpper())
                    {
                        break;
                    }
                    sdName = sdNameEnum.Next();
                }
            }

            if (sdName != null)
            {
                IMMStoredDisplay sd = sdManager.GetUnopenedStoredDisplay(sdName);
                sd.Open(map);
                return true;
            }
            else
            {
                throw new Exception("Could not find stored display: " + SDName);
            }
        }

        /// <summary>
        /// Opens a workspace given a SDE Connection file
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string connectionFile)
        {
            try
            {
                // set feature workspace to the SDE connection 
                IWorkspace pWS = null; 
                if (connectionFile.ToLower().EndsWith(".sde"))
                {
                    if (!File.Exists(connectionFile))
                        throw new Exception("The connection file: " + connectionFile + " does not exist on the file system!");
                    SdeWorkspaceFactory sdeWorkspaceFactory = (SdeWorkspaceFactory)new SdeWorkspaceFactory();
                    pWS = sdeWorkspaceFactory.OpenFromFile(connectionFile, 0);
                }
                else if (connectionFile.ToLower().EndsWith(".gdb"))
                {
                    if (!Directory.Exists(connectionFile))
                        throw new Exception("The file geodatabase: " + connectionFile + " does not exist on the file system!");

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("DATABASE", @connectionFile);
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)obj;
                    pWS = workspaceFactory.OpenFromFile(@connectionFile, 0);
                }
                return pWS; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening workspace from connection file"); 
            }
        }

        /// <summary>
        /// Checks if the map grid feature contains any of the features in the specified feature class
        /// </summary>
        /// <param name="mapGridFeature">Map grid feature</param>
        /// <param name="featClass">Feature class to check against</param>
        /// <returns></returns>
        public static bool HasData(IFeature mapGridFeature, IFeatureClass featClass)
        {
            IFeatureCursor featCursor = null;
            IFeature feat = null;
            ISpatialFilter qf = null;
            try
            {
                qf = new SpatialFilterClass();
                qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                qf.Geometry = mapGridFeature.Shape;
                qf.GeometryField = featClass.ShapeFieldName;
                qf.SubFields = featClass.OIDFieldName;
                featCursor = featClass.Search(qf, true);
                feat = featCursor.NextFeature();
                if (feat != null)
                {
                    return true;
                }
                return false;
            }
            finally
            {
                if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            }
        }

        /// <summary>
        /// Returns the value of the requested export state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        //public static int GetExportState(ExportStates state)
        //{
        //    if (state == ExportStates.Processed)
        //    {
        //        return 0;
        //    }
        //    else if (state == ExportStates.Error)
        //    {
        //        return 1;
        //    }
        //    else if (state == ExportStates.InProgress)
        //    {
        //        return 2;
        //    }
        //    else if (state == ExportStates.LocationInformationMissing)
        //    {
        //        return 3;
        //    }
        //    return -1;
        //}

        public enum ExportStates
        {
            ReadyToExport =1,
            InProgress = 2,
            Idle = 3,
            Processed = 7,
            Error = 4,
            LocationInformationMissing= 5,
            RequiredDataMissing = 6
        };

        public enum ExitCodes
        {
            Success,
            Failure,
            ChildFailure,
            InvalidArguments,
            LicenseFailure
        };

        #endregion

    }
}
