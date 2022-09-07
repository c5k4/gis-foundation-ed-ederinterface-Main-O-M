using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;

namespace PLDBBatchProcess
{
    public static class Shared
    {

        private static Hashtable _hshWorkspacePaths = null;
        private static Hashtable _hshWorkspaces = null;
        private static string _workingGDBFolderPath = string.Empty;
        private static string _installDirectory = string.Empty;
        private static string _workingGDBName = string.Empty;
        private static FileInfo _logfileInfo = null;
        private static FileInfo _sqlfileInfo = null;
        private static string _logFolder = string.Empty;
        private static string _logFilename = string.Empty;
        private static string _sqlFilename = string.Empty;
        
        public static IWorkspace GetWorkspaceByName(string name)
        {
            return (IWorkspace)_hshWorkspaces[name];
        }

        //public static void LoadConfigurationSettings()
        //{
        //    try
        //    {
        //        //Load the config document 
        //        string xPath = string.Empty;
        //        XmlDocument xmlDoc = GetConfigXMLDocument();
        //        XmlNode pXMLNode = null;

        //        //Logfolder     
        //        xPath = "//ConfigSettings/AppSettings/Logfolder";
        //        pXMLNode = xmlDoc.SelectSingleNode(xPath);
        //        if (pXMLNode != null)
        //        {
        //            //Append the threadId so if run in a multithreaded 
        //            //situation each thread will have its own logfile 
        //            _logFolder = pXMLNode.InnerText;
        //            _logFilename = _logFolder + "PLDBBatchLogfile" + ".txt";
        //        }

        //        //PoleFilter     
        //        xPath = "//ConfigSettings/AppSettings/PoleFilter";
        //        pXMLNode = xmlDoc.SelectSingleNode(xPath);
        //        if (pXMLNode != null)
        //        {
        //            _poleFilter = pXMLNode.InnerText;
        //        }


        //        //OutputConnectString     
        //        xPath = "//ConfigSettings/AppSettings/OutputConnectString";
        //        pXMLNode = xmlDoc.SelectSingleNode(xPath);
        //        if (pXMLNode != null)
        //        {
        //            _outputConnString = pXMLNode.InnerText;
        //        }

        //        //Workspaces 
        //        _hshWorkspacePaths = new Hashtable();
        //        string workspaceName = string.Empty;
        //        string workspacePath = string.Empty;
        //        XmlNode pSubNode = null;
        //        xPath = "//ConfigSettings/AppSettings/Workspaces";
        //        XmlNode pTopNode = xmlDoc.SelectSingleNode(xPath);
        //        for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
        //        {
        //            pXMLNode = pTopNode.ChildNodes.Item(i);
        //            if (pXMLNode != null)
        //            {
        //                for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
        //                {
        //                    pSubNode = pXMLNode.ChildNodes.Item(j);
        //                    if (pSubNode.LocalName == "Name")
        //                        workspaceName = pSubNode.InnerText.ToLower();
        //                    if (pSubNode.LocalName == "Connection")
        //                        workspacePath = pSubNode.InnerText.ToLower();
        //                }
        //            }

        //            if (workspaceName == "output")
        //            {
        //                int posOfLastSlash = workspacePath.LastIndexOf(@"\");
        //                _workingGDBFolderPath = workspacePath.Substring(
        //                    0, posOfLastSlash);
        //                _workingGDBName = workspacePath.Substring(
        //                    posOfLastSlash + 1, workspacePath.Length - (posOfLastSlash + 1));
        //                _workingGDBName = _workingGDBName.Replace(".gdb", "").ToUpper();
        //            }
        //            if (!_hshWorkspacePaths.ContainsKey(workspaceName))
        //                _hshWorkspacePaths.Add(workspaceName, workspacePath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error loading configuration settings");
        //    }
        //}

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
                configFilename = _installDirectory + "\\" + PLDBBatchConstants.CONFIG_FILENAME;
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

        public static void InitializeLogfile()
        {
            try
            {
                //Create a logfile
                if (_logFilename == string.Empty)
                    _logFilename = 
                        ConfigurationManager.AppSettings.Get(
                            PLDBBatchConstants.CONFIG_LOG_FOLDER)  +
                        ConfigurationManager.AppSettings.Get(
                            PLDBBatchConstants.CONFIG_LOG_FILE);

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

        public static void InitializeSQLfile(int fileIndex)
        {
            try
            {
                _sqlFilename = _logFolder + PLDBBatchConstants.PLDB_SQL_FILE_PREFIX +  
                    fileIndex.ToString() + ".sql";
                System.IO.FileStream fs = File.Open(_sqlFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_sqlFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_sqlFilename, "");
                }
                _sqlfileInfo = new FileInfo(_sqlFilename);
            }
            catch
            {
                Debug.Print("Error initializing logfile");
            }
        }

        public static void CleanupSQLfiles()
        {
            try
            {
                string sqlFilename = string.Empty; 
                for (int i = 0; i < 1000; i++)
                {
                    string installDirectory = new FileInfo(
                    Assembly.GetExecutingAssembly().Location).
                    DirectoryName;
                    sqlFilename = installDirectory + "\\" +
                        PLDBBatchConstants.PLDB_SQL_FILE_PREFIX +
                        i.ToString() + ".sql";
                    if (File.Exists(sqlFilename))
                    {
                        try
                        {
                            File.Delete(sqlFilename);
                        }
                        catch
                        {
                            //Will not cause problem - do nothing 
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
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
                _hshWorkspaces.Add("electric", GetWorkspace( 
                    ConfigurationManager.AppSettings.Get(
                        PLDBBatchConstants.CONFIG_EDGIS_SDE)));
                _hshWorkspaces.Add("wip", GetWorkspace(
                    ConfigurationManager.AppSettings.Get(
                        PLDBBatchConstants.CONFIG_WIP_SDE)));

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

        //public static void LaunchSQLBatchFile(int fileIndex, bool waitForExit)
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
        //        Shared.WriteToLogfile("fileIndex: " + fileIndex.ToString());

        //        Process proc = new Process();
        //        string installDirectory = new FileInfo( 
        //            Assembly.GetExecutingAssembly().Location).
        //            DirectoryName;
        //        string batchFilename = installDirectory + "\\" + 
        //            PLDBBatchConstants.BAT_FILENAME;
        //        string[] parts = ConfigurationManager.
        //                ConnectionStrings[PLDBBatchConstants.
        //                CONFIG_PLDB_CONNECTION].
        //                ConnectionString.Split(';');
        //        string dataSource = string.Empty;
        //        string database = string.Empty; 
        //        string user = string.Empty;
        //        string password = string.Empty;
        //        for (int i =0; i < parts.Length; i++)
        //        {
        //            string part = parts[i].Trim();
        //            if (part.StartsWith("Data Source="))
        //            {
        //                dataSource = part.Replace("Data Source=", "").Trim();
        //            }
        //            else if (part.StartsWith("Initial Catalog="))
        //            {
        //                database = part.Replace("Initial Catalog=", "").Trim();
        //            }
        //            else if (part.StartsWith("User id="))
        //            {
        //                user = part.Replace("User id=", "").Trim();
        //            }
        //            else if (part.StartsWith("Password="))
        //            {
        //                password = part.Replace("Password=", "").Trim();
        //            }
        //        }
                
        //        if (File.Exists(batchFilename))
        //        {
        //            string sqlFilename = _logFolder +  
        //                PLDBBatchConstants.PLDB_SQL_FILE_PREFIX + fileIndex.ToString() + ".sql";
        //            File.WriteAllText(batchFilename,
        //                "SQLCMD " + 
        //                "-S " + dataSource + 
        //                " -d " + database + 
        //                " -U " + user + 
        //                " -P " + password + 
        //                " -i " + sqlFilename);

        //            proc.StartInfo.FileName = batchFilename;
        //            proc.StartInfo.RedirectStandardError = false;
        //            proc.StartInfo.RedirectStandardOutput = false;
        //            proc.StartInfo.UseShellExecute = false;                         
        //            proc.Start();
        //            if (waitForExit)
        //            {
        //                proc.WaitForExit();
        //                Shared.WriteToLogfile("fileIndex: " + fileIndex.ToString() + 
        //                    " has completed processing synchronously");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("Batch file not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error populating EDGIS poles");
        //    }
        //}

        public static string GetSQLFromSQLFile(string sqlFile)
        {
            try
            {
                string sql = string.Empty;
                string sqlFilename = string.Empty;
                string installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                sqlFilename = installDirectory + "\\" + sqlFile;

                if (!File.Exists(sqlFilename))
                    throw new Exception("Error locating the PLD_INFO SQL File");
                else
                    sql = File.ReadAllText(sqlFilename);

                return sql;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning sql query from file");
            }
        }

        public static string GetMapDocumentFullFilename()
        {
            try
            {
                string sql = string.Empty;
                string sqlFilename = string.Empty;
                string installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                string mapDocFile = installDirectory + "\\" + PLDBBatchConstants.MAPDOCUMENT_FILENAME;

                if (!File.Exists(mapDocFile))
                    throw new Exception("Error elevation map document does not exist");

                return mapDocFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning sql query from file");
            }
        }

        //public static void NullOutBatchFile()
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

        //        Process proc = new Process();
        //        string installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
        //        string batchFilename = installDirectory + "\\" + PLDBBatchConstants.BAT_FILENAME;

        //        if (File.Exists(batchFilename))
        //            File.WriteAllText(batchFilename, "");
        //        else
        //            throw new Exception("Batch file not found");

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error populating EDGIS poles");
        //    }
        //}

        
        public static void WriteToLogfile(string msg)
        {
            try
            {
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

        public static void WriteToSQLFile(string msg)
        {
            try
             {
                using (StreamWriter sWriter = _sqlfileInfo.AppendText())
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
    }

    public class Pole
    {
        //Property storage variables 
        private string _guid;
        private long _pldbid; 
        private long _sapEquipId;
        string _replaceGUID;
        private double _latitude;
        private double _longitude;
        private double _elevation;        
        private double _horizontalSF;
        private double _overallSF;
        private double _bendingSF;
        private double _verticalSF;
        private string _classString;
        private double _lengthInInches;
        private string _species;
        private string _snowLoadingDistrict;
        private double _xVal;
        private double _yVal; 
        private int _subtypeCd = -1; 
        int _statusIdx;
        string _status; 

        public Pole(
            long pldbid, 
            string guid,
            long sapEquipId,
            string replaceGUID,
            double latitude,
            double longitude, 
            int status)
        {
            _pldbid = pldbid;
            if (guid.IndexOf("{") == -1)
                guid = ("{" + guid + "}").ToUpper(); 
            _guid = guid;
            _sapEquipId = sapEquipId;
            _replaceGUID = replaceGUID;
            _latitude = latitude;
            _longitude = longitude;
            _statusIdx = status; 
        }

        public Pole(
            long pldbid,
            string guid,
            long sapEquipId,
            string replaceGUID,
            double latitude,
            double longitude)
        {
            _pldbid = pldbid;
            if (guid.IndexOf("{") == -1)
                guid = ("{" + guid + "}").ToUpper();
            _guid = guid;
            _sapEquipId = sapEquipId;
            _replaceGUID = replaceGUID;
            _latitude = latitude;
            _longitude = longitude;
        }

        public Pole(
            long pldbid,
            string guid,
            double latitude,
            double longitude, 
            double elevation)
        {
            _pldbid = pldbid;
            if (guid.IndexOf("{") == -1)
                guid = ("{" + guid + "}").ToUpper();
            _guid = guid;
            _latitude = latitude;
            _longitude = longitude;
            _elevation = elevation;
        }

        public Pole(
            long pldbid,
            string status,
            double latitude,
            double longitude,
            double elevation, 
            double horizontalSF, 
            double overallSF, 
            double bendingSF, 
            double verticalSF, 
            string classString, 
            double lengthInInches, 
            string species, 
            long sapEquipId, 
            string globalid, 
            string snowLoadDistrict, 
            double xVal, 
            double yVal)
        {
            _pldbid = pldbid;
            _status = status;
            _latitude = latitude;
            _longitude = longitude;
            _elevation = elevation;
            _horizontalSF = horizontalSF;
            _overallSF = overallSF;
            _bendingSF = bendingSF;
            _verticalSF = verticalSF;
            _classString = classString;
            _lengthInInches = lengthInInches;
            _species = species;
            _sapEquipId = sapEquipId;
            _guid = globalid;
            _snowLoadingDistrict = snowLoadDistrict;
            _xVal = xVal;
            _yVal = yVal; 
        }

        public long PLDBID
        {
            get { return _pldbid; }
        }
        public string GUID
        {
            get { return _guid; }
        }
        public long SapEquipID
        {
            get { return _sapEquipId; }
        }
        public string ReplaceGUID
        {
            get { return _replaceGUID; }
        }
        public double Latitude
        {
            get { return _latitude; }
        }
        public double Longitude
        {
            get { return _longitude; }
        }
        public double Elevation
        {
            get { return _elevation; }
        }
        public long StatusIndex
        {
            get { return _statusIdx; }
        }
        public string Status
        {
            get { return _status; }
        }
        public int Subtype
        {
            get { return _subtypeCd; }
            set { _subtypeCd = value; }
        }
        public double HorizontalSF
        {
            get { return _horizontalSF; }
        }
        public double OverallSF
        {
            get { return _overallSF; }
        }
        public double BendingSF
        {
            get { return _bendingSF; }
        }
        public double VerticalSF
        {
            get { return _verticalSF; }
        }
        public string ClassString
        {
            get { return _classString; }
        }
        public double LengthInInches
        {
            get { return _lengthInInches; }
        }
        public string Species
        {
            get { return _species; }
        }
        public string SnowLoadingDistrict
        {
            get { return _snowLoadingDistrict; }
        }
        public double XValue
        {
            get { return _xVal; }
        }
        public double YValue
        {
            get { return _yVal; }
        }
    }
}

