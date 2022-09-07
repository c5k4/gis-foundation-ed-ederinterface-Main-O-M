using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;


//using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
//using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
//using ESRI.ArcGIS.Display;

namespace PoleLoadExtract
{
    public static class Shared
    {

        private static Hashtable _hshWorkspacePaths = null;
        private static Hashtable _hshConductorCodes = null;
        private static Hashtable _hshWorkspaces = null;
        private static Hashtable _hshPolesList = null;
        private static string _workingGDBFolderPath = string.Empty;
        private static string _installDirectory = string.Empty;
        private static string _workingGDBName = string.Empty;
        private static string _mapDocumentPath = string.Empty; 

        private static FileInfo _logfileInfo = null;
        private static FileInfo _spanCSVfileInfo = null;
        private static FileInfo _fullDataCSVfileInfo = null;
        private static FileInfo _staticCSVfileInfo = null;
        private static FileInfo _missingPohCSVfileInfo = null;
        private static string _logFilename = string.Empty;
        private static string _spanCSVFilename = string.Empty;
        private static string _fullDataCSVFilename = string.Empty;
        private static string _missingPohCSVfilename = string.Empty;//written by Tina
        private static string _staticCSVFilename = string.Empty;
        private static string _clipFilter = string.Empty;
        private static string _poleFilter = string.Empty; 
        private static string _outputConnString = string.Empty;

        private static int _goAroundTolerance;
        private static int _goAroundPointCount;
        private static int _goAroundProxPercent;

        private static int _maxTries = 2;        
        private static int _maxSpanLengthLowDensity = 0;
        private static int _maxSpanLengthMediumDensity = 0;
        private static int _maxSpanLengthHighDensity = 0;
        private static double _poleBuffer = 0;
        private static bool _useList = false; 
        private static double _secondaryConductorBuffer = 0;
        private static double _polePrimaryConductorBuffer = 0; 
        private static double _primaryBufferIncrement = 0;
        private static string _clipFC = string.Empty;

        public static int GoAroundTolerance
        {
            get { return _goAroundTolerance; }
        }

        public static int GoAroundPointCount
        {
            get { return _goAroundPointCount; }
        }

        public static int GoAroundProxPercent
        {
            get { return _goAroundProxPercent; }
        }

        public static int MaxSpanLengthLowDensity
        {
            get { return _maxSpanLengthLowDensity; }
        }

        public static int MaxSpanLengthMediumDensity
        {
            get { return _maxSpanLengthMediumDensity; }
        }

        public static int MaxSpanLengthHighDensity
        {
            get { return _maxSpanLengthHighDensity; }
        }

        public static double PoleBuffer
        {
            get { return _poleBuffer; }
        }
        public static bool UseList
        {
            get { return _useList; }
        }


        public static double SecondaryConductorBuffer
        {
            get { return _secondaryConductorBuffer; }
        }

        public static double PolePrimaryConductorBuffer
        {
            get { return _polePrimaryConductorBuffer; }
        }

        public static double PrimaryBufferIncrement 
        {
            get { return _primaryBufferIncrement; }
        }

        public static string PoleFilter
        {
            get { return _poleFilter; }
        }

        public static string MapDocumentPath
        {
            get { return _mapDocumentPath; }
        }

        public static string OutputConnectString
        {
            get { return _outputConnString; }
        }

        //public static double MaxTries
        //{
        //    get { return _maxTries; }
        //}

        public static IWorkspace GetWorkspaceByName(string name)
        {
            return (IWorkspace)_hshWorkspaces[name];
        }

        public static Hashtable PolesList()
        {
            return _hshPolesList;
        }

        public static Hashtable ConductorCodes()
        {
            return _hshConductorCodes;
        }


        /// <summary>
        /// Allows the dividing up of the pole extract processing 
        /// by division, region or other polygon layer as long as the 
        /// layer is available in the ouput workspace 
        /// </summary>
        /// <returns>An IPolygon as a basis for clipping</returns>
        public static IPolygon GetClipPolygon()
        {
            try
            {
                WriteToLogfile("Entering GetClipPolygon");
                if (_clipFC == string.Empty)
                    return null; 
                IPolygon pClipPolygon = null;
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                IFeatureClass pClipFC = pDestFWS.OpenFeatureClass(_clipFC);
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = _clipFilter;
                IFeatureCursor pFCursor = pClipFC.Search(pQF, false);
                IFeature pClipFeature = pFCursor.NextFeature();

                if (pClipFeature != null)
                {
                    pClipPolygon = (IPolygon)pClipFeature.ShapeCopy;
                }
                Marshal.FinalReleaseComObject(pFCursor);
                return pClipPolygon;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in GetClipPolygon: " + ex.Message);
                throw new Exception("Error returning clip polygon");
            }
        }

        public static void LoadConfigurationSettings(string processSuffix)
        {
            try
            {
                //Load the config document 
                string xPath = string.Empty;
                XmlDocument xmlDoc = GetConfigXMLDocument();
                XmlNode pXMLNode = null;

                //MapDocumentPath
                _mapDocumentPath = GetMapDocumentPath();

                //Logfile     
                xPath = "//ConfigSettings/AppSettings/Logfile";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                string logfilePrefix = "PoleLoadLog";
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    string windowsProcessId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
                    _logFilename = pXMLNode.InnerText + logfilePrefix + "_" + processSuffix + ".txt";
                }

                //SpanCSVfile     
                xPath = "//ConfigSettings/AppSettings/SpanCSVfile";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                string spanCSVfilePrefix = "SPAN_ANGLE";
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _spanCSVFilename = pXMLNode.InnerText + spanCSVfilePrefix + "_" + processSuffix + ".csv";
                }

                //FullDataCSVfile     
                xPath = "//ConfigSettings/AppSettings/FullDataCSVfile";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                string fullDataCSVfilePrefix = "FULL_DATA";
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _fullDataCSVFilename = pXMLNode.InnerText + fullDataCSVfilePrefix + "_" + processSuffix + ".csv";
                }

                //StaticCSVfile     
                xPath = "//ConfigSettings/AppSettings/StaticCSVfile";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                string staticCSVfilePrefix = "STATIC";
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _staticCSVFilename = pXMLNode.InnerText + staticCSVfilePrefix + "_" + processSuffix + ".csv";
                }

                //GoAroundTolerance 
                xPath = "//ConfigSettings/AppSettings/GoAroundTolerance";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _goAroundTolerance = Convert.ToInt32(pXMLNode.InnerText);
                }

                //GoAroundMinimumPointCount
                xPath = "//ConfigSettings/AppSettings/GoAroundPointCount";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _goAroundPointCount = Convert.ToInt32(pXMLNode.InnerText);
                }

                //GoAroundProxPercentFromClosestPoint 
                xPath = "//ConfigSettings/AppSettings/GoAroundProxPercentFromClosestPoint";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _goAroundProxPercent = Convert.ToInt32(pXMLNode.InnerText);
                }

                //PoleFilter   
                xPath = "//ConfigSettings/AppSettings/PoleFilter";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _poleFilter = pXMLNode.InnerText;
                }

                //MaxSpanLengthLowDensity      
                xPath = "//ConfigSettings/AppSettings/MaxSpanLengthLowDensity";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _maxSpanLengthLowDensity = Convert.ToInt32(pXMLNode.InnerText);
                }

                //MaxSpanLengthMediumDensity     
                xPath = "//ConfigSettings/AppSettings/MaxSpanLengthMediumDensity";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _maxSpanLengthMediumDensity = Convert.ToInt32(pXMLNode.InnerText);
                }

                //MaxSpanLengthHighDensity     
                xPath = "//ConfigSettings/AppSettings/MaxSpanLengthHighDensity";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _maxSpanLengthHighDensity = Convert.ToInt32(pXMLNode.InnerText);
                }

                //PoleBuffer     
                xPath = "//ConfigSettings/AppSettings/PoleBuffer";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _poleBuffer = double.Parse(pXMLNode.InnerText.Trim(),
                        System.Globalization.NumberFormatInfo.InvariantInfo);
                }

                //SecondaryConductorBuffer     
                xPath = "//ConfigSettings/AppSettings/SecondaryConductorBuffer";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    _secondaryConductorBuffer = double.Parse(pXMLNode.InnerText.Trim(),
                        System.Globalization.NumberFormatInfo.InvariantInfo);
                }

                //PolePrimaryConductorBuffer
                xPath = "//ConfigSettings/AppSettings/PolePrimaryConductorBuffer";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    _polePrimaryConductorBuffer = double.Parse(pXMLNode.InnerText.Trim(),
                        System.Globalization.NumberFormatInfo.InvariantInfo);
                }

                //PrimaryBufferIncrement     
                xPath = "//ConfigSettings/AppSettings/PrimaryBufferIncrement";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _primaryBufferIncrement = double.Parse(pXMLNode.InnerText.Trim(),
                        System.Globalization.NumberFormatInfo.InvariantInfo);
                }

                //ClipFC     
                xPath = "//ConfigSettings/AppSettings/ClipFC";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _clipFC = pXMLNode.InnerText;
                }

                //ClipFilter     
                xPath = "//ConfigSettings/AppSettings/ClipFilter";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _clipFilter = pXMLNode.InnerText;
                }

                //OutputConnectString    
                xPath = "//ConfigSettings/AppSettings/OutputConnectString";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    _outputConnString = pXMLNode.InnerText;
                }

                //Max tries for each featureclass  
                xPath = "//ConfigSettings/AppSettings/MaxTries";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                    _maxTries = Convert.ToInt32(pXMLNode.InnerText);

                //UseList   
                xPath = "//ConfigSettings/AppSettings/UseList";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                    _useList = Convert.ToBoolean(pXMLNode.InnerText);

                //ConductorCodes 
                xPath = "//ConfigSettings/AppSettings/ConductorCodes";
                XmlNode pTopNode = pTopNode = xmlDoc.SelectSingleNode(xPath);
                PoleLoad pl = new PoleLoad();
                _hshConductorCodes = pl.LoadConductorCodes(pTopNode);
                
                //Workspaces 
                _hshWorkspacePaths = new Hashtable();
                string workspaceName = string.Empty;
                string workspacePath = string.Empty;
                XmlNode pSubNode = null;
                xPath = "//ConfigSettings/AppSettings/Workspaces";
                pTopNode = xmlDoc.SelectSingleNode(xPath);
                for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
                {
                    pXMLNode = pTopNode.ChildNodes.Item(i);
                    if (pXMLNode != null)
                    {
                        for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
                        {
                            pSubNode = pXMLNode.ChildNodes.Item(j);
                            if (pSubNode.LocalName == "Name")
                                workspaceName = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "Connection")
                                workspacePath = pSubNode.InnerText.ToLower();
                        }
                    }

                    if (workspaceName == "output")
                    {
                        int posOfLastSlash = workspacePath.LastIndexOf(@"\");
                        _workingGDBFolderPath = workspacePath.Substring(
                            0, posOfLastSlash);
                        _workingGDBName = workspacePath.Substring(
                            posOfLastSlash + 1, workspacePath.Length - (posOfLastSlash + 1));
                        _workingGDBName = _workingGDBName.Replace(".gdb", "").ToUpper();
                    }
                    if (!_hshWorkspacePaths.ContainsKey(workspaceName))
                        _hshWorkspacePaths.Add(workspaceName, workspacePath);
                }

                //Poles list 
                string poleSapEquipId = string.Empty;
                int sapEquipIDInt = 0; 
                xPath = "//ConfigSettings/AppSettings/Poles";
                pTopNode = xmlDoc.SelectSingleNode(xPath);
                for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
                {
                    if (_hshPolesList == null)
                        _hshPolesList = new Hashtable();

                    pSubNode = pTopNode.ChildNodes[i];
                    poleSapEquipId = pSubNode.InnerText.ToLower();
                    if (Int32.TryParse(poleSapEquipId, out sapEquipIDInt))
                    {
                        if (!_hshPolesList.ContainsKey(sapEquipIDInt))
                            _hshPolesList.Add(sapEquipIDInt, 0);
                    }                
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading configuration settings");
            }
        }

        private static string GetQueryFilterText(string xmlQFText)
        {
            try
            {
                string qfText = xmlQFText;
                if (xmlQFText.IndexOf("[CDATA[") == 0)
                {
                    int lengthOfQF = xmlQFText.Length;
                    qfText = xmlQFText.Substring(7, lengthOfQF - 9);
                }
                return qfText;
            }
            catch (Exception ex)
            {
                return xmlQFText;
            }
        }

        private static XmlDocument GetConfigXMLDocument()
        {
            try
            {
                //Open up the xml configuration file  
                string configFilename = "";
                _installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                configFilename = _installDirectory + "\\" + PoleLoadConstants.CONFIG_FILENAME;
                if (!File.Exists(configFilename))
                {
                    throw new Exception("Unable to find config file: " + configFilename);
                }

                //Get the xml config document 
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilename);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading configuration file");
            }
        }

        private static string GetMapDocumentPath()
        {
            try
            {
                //Open up the xml configuration file  
                string mapFilename = "";
                _installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                mapFilename = _installDirectory + "\\" + PoleLoadConstants.MAP_FILENAME;
                if (!File.Exists(mapFilename))
                {
                    throw new Exception("Unable to find map document file: " + mapFilename);
                }
                return mapFilename;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading map document filename");
            }
        }


        public static void InitializeLogfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_logFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_logFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_logFilename, "");
                }
                _logfileInfo = new FileInfo(_logFilename);
            }
            catch
            {
                Debug.Print("Error initializing logfile");
            }
        }

        public static void InitializeSpanCSVfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_spanCSVFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_spanCSVFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_spanCSVFilename, "");
                }
                _spanCSVfileInfo = new FileInfo(_spanCSVFilename);
            }
            catch
            {
                Debug.Print("Error initializing csvfile");
            }
        }

        public static void InitializeFullDataCSVfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_fullDataCSVFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_fullDataCSVFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_fullDataCSVFilename, "");
                }
                _fullDataCSVfileInfo = new FileInfo(_fullDataCSVFilename);
            }
            catch
            {
                Debug.Print("Error initializing csvfile");
            }
        }


        //public void AddENOSError(
        //    ref Hashtable hshENOSEquipErrorList,
        //    Int64 equipId,
        //    string localOffice,
        //    string errorMsg)
        //{
        //    try
        //    {
        //        //Copying records from ENOS_STAGE to ENOS_ERROR with appropriate description / date 
        //        Log("Entering " + MethodBase.GetCurrentMethod());
        //        Log("   equipId " + equipId.ToString());
        //        Log("   errorMsg " + errorMsg);

        //        //Get oracle connection 
        //        if (_pConn == null)
        //            throw new Exception("No ADO.NET connection available");
        //        if (_pConn.State != ConnectionState.Open)
        //            _pConn.Open();

        //        //Insert the ENOS Error records that are NOT in the   
        //        //ENOSEquipErrorList 
        //        OracleCommand pCmd = null;
        //        string addEquip = "";
        //        string allEquip = equipId.ToString();
        //        string fieldList = "";
        //        string sql = "";
        //        int addCount = 0;
        //        int recordsUpdated = -1;

        //        if (!hshENOSEquipErrorList.ContainsKey(equipId))
        //        {
        //            addCount = 1;
        //            addEquip = equipId.ToString();
        //            fieldList =
        //                ENOSCommon.ET_ENOS_REF_ID_FLD + "," +
        //                ENOSCommon.ET_ENOS_STATUS_FLD + "," +
        //                ENOSCommon.ET_EQUIPMENT_ID_FLD + "," +
        //                ENOSCommon.ET_EQUIPMENT_TYPE_FLD + "," +
        //                ENOSCommon.ET_GENERATION_STATUS_FLD + "," +
        //                ENOSCommon.ET_INVERTERID_FLD + "," +
        //                ENOSCommon.ET_MANUFACTURER_FLD + "," +
        //                ENOSCommon.ET_MODEL_FLD + "," +
        //                ENOSCommon.ET_POWER_SOURCE_FLD + "," +
        //                ENOSCommon.ET_QUANTITY_FLD + "," +
        //                ENOSCommon.ET_RATING_FLD + "," +
        //                ENOSCommon.ET_SERVICE_POINT_ID_FLD + "," +
        //                ENOSCommon.ET_STATUS_FLD;


        //            //Run an INSERT INTO SELECT statement for the error  
        //            sql = "INSERT INTO " +
        //                ENOSCommon.ENOS_ERROR_TBL + " " +
        //                "(" +
        //                    fieldList + "," +
        //                        ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," +
        //                        ENOSCommon.ET_LOCAL_OFFICE_FLD + "," +
        //                        ENOSCommon.ET_ERROR_DATE_FLD +
        //                ")" + " " +
        //                "SELECT" + " " +
        //                    fieldList + "," +
        //                        "'" + errorMsg + "'" + " AS " + ENOSCommon.ET_ERROR_DESCRIPTION_FLD + "," +
        //                        "'" + localOffice + "'" + " AS " + ENOSCommon.ET_LOCAL_OFFICE_FLD + "," +
        //                        ENOSCommon.ORACLE_SYSDATE + " AS " + ENOSCommon.ET_ERROR_DATE_FLD + " " +
        //                "FROM" + " " +
        //                ENOSCommon.ENOS_STAGE_TBL + " " +
        //                "WHERE" + " " +
        //                ENOSCommon.ET_EQUIPMENT_ID_FLD + " IN(" + addEquip + ")";

        //            pCmd = _pConn.CreateCommand();
        //            pCmd.Connection = _pConn;
        //            pCmd.CommandText = sql;
        //            Log("Executing insert only");
        //            recordsUpdated = pCmd.ExecuteNonQuery();
        //            if (recordsUpdated != addCount)
        //            {
        //                Log("Error inserting into ENOS_Error - initial insert, recordsUpdated: " + recordsUpdated.ToString());
        //            }
        //        }

        //        //Lastly update the ENOSErrorList
        //        if (!hshENOSEquipErrorList.ContainsKey(equipId))
        //            hshENOSEquipErrorList.Add(equipId, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
        //            ex.Message);
        //        throw new Exception("Error adding ENOS Error record");
        //    }
        //}

        public static void InitializeStaticCSVfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_staticCSVFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_staticCSVFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_staticCSVFilename, "");
                }
                _staticCSVfileInfo = new FileInfo(_staticCSVFilename);
            }
            catch
            {
                Debug.Print("Error initializing csvfile");
            }
        }


        public static void InitializeMissingPohCSVfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_missingPohCSVfilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_missingPohCSVfilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_missingPohCSVfilename, "");
                }
                _missingPohCSVfileInfo = new FileInfo(_missingPohCSVfilename);
            }
            catch
            {
                Debug.Print("Error initializing csvfile");
            }
        }


        /// <summary>
        /// Loads all the workspaces referenced in the application 
        /// </summary>
        /// <param name="mapGeo"></param>
        public static void LoadWorkspaces()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                _hshWorkspaces = new Hashtable();
                //ExtractUtils pExtractUtils = new ExtractUtils();
                foreach (string workspace in _hshWorkspacePaths.Keys)
                {
                    WriteToLogfile("connecting to workspace: " + workspace);
                    _hshWorkspaces.Add(workspace, GetWorkspace(_hshWorkspacePaths[workspace].ToString()));
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in LoadWorkspaces: " + ex.Message);
                throw new Exception("Error loading workspaces"); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFeatureWorkspace GetWorkspace(string connectionFile)
        {
            IWorkspace pWS = null;
            IFeatureWorkspace pFWS = null;

            try
            {
                // set feature workspace to the SDE connection                
                if (connectionFile.ToLower().EndsWith(".sde"))
                {
                    if (!File.Exists(connectionFile))
                        WriteToLogfile("The connection file: " + connectionFile + " does not exist on the file system!");
                    SdeWorkspaceFactory sdeWorkspaceFactory = (SdeWorkspaceFactory)new SdeWorkspaceFactory();
                    pWS = sdeWorkspaceFactory.OpenFromFile(connectionFile, 0);
                }
                else if (connectionFile.ToLower().EndsWith(".gdb"))
                {
                    if (!Directory.Exists(connectionFile))
                        WriteToLogfile("The file geodatabase: " + connectionFile + " does not exist on the file system!");

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("DATABASE", @connectionFile);
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)obj;
                    pWS = workspaceFactory.OpenFromFile(@connectionFile, 0);
                }

                pFWS = (IFeatureWorkspace)pWS;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error opening SDE workspace.\r\n{0}", ex.Message));
            }
            return pFWS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteToSpanCSV(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _spanCSVfileInfo.AppendText())
                {
                    sWriter.WriteLine(msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        public static void WriteStaticDataToCSV(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _staticCSVfileInfo.AppendText())
                {
                    sWriter.WriteLine(msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        public static void WriteMissingPohDataToCSV(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _missingPohCSVfileInfo.AppendText())
                {
                    sWriter.WriteLine(msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }


        public static void WriteToFullDataCSV(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _fullDataCSVfileInfo.AppendText())
                {
                    sWriter.WriteLine(msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        public static void WriteToLogfile(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _logfileInfo.AppendText())
                {
                    sWriter.WriteLine(DateTime.Now.ToLongTimeString() + " " + msg);
                    //sWriter.WriteLine(msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

    }
}
