using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using PGE.Common.ChangesManager.Utilities;
using PGE.Common.ChangesManagerShared;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;

namespace PGE.Common.ChangesManager
{
    public class MDBManager : PGE.Common.ChangesManager.IExtractGDBManager
    {

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private SDEWorkspaceConnection SDEWorkspace { get; set; }
        private IWorkspace _mdbWorkspace;
        private IName _mdbName;

        private string _featureClassName;
        private string _mdbBaseLocation;
        private string _mdbEditsLocation;
        private const string MGDB_SUFFIX_EDITS = "_edits";

        public static IWorkspace AccessWorkspaceFromPath(String path)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for path [ " + path + " ]");

            IWorkspace workspace = null;

            try
            {
                Type factoryType = Type.GetTypeFromProgID(
                    "esriDataSourcesGDB.AccessWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                workspace = workspaceFactory.OpenFromFile(path, 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ErrorCodeException(ErrorCode.MDBBaseMissing, "Access Can't be Opened at [ " + path + " ]", ex);
            }

            return workspace;
        }

        public static IWorkspace CreateAccessWorkspace(String path, string mdbName)
        {
            // Instantiate an Access workspace factory and create a personal geodatabase.
            // The Create method returns a workspace name object.
            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.AccessWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);
            IWorkspaceName workspaceName = workspaceFactory.Create(path, mdbName, null,
                0);

            // Cast the workspace name object to the IName interface and open the workspace.
            IName name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)name.Open();
            return workspace;
        }

        public string EditsLocation
        {
            get
            {
                return _mdbEditsLocation;
            }
        }

        public string BaseLocation
        {
            get
            {
                return _mdbBaseLocation;
            }
        }

        private string GetFeatureClassName()
        {
            if (_featureClassName.Contains("."))
            {
                return _featureClassName.Substring(_featureClassName.LastIndexOf(".") + 1);
            }

            return _featureClassName;
        }

        public void KillConnections()
        {
            if (_mdbName != null) Marshal.FinalReleaseComObject(_mdbName);
            if (_mdbWorkspace != null) Marshal.FinalReleaseComObject(_mdbWorkspace);

            _mdbName = null;
            _mdbWorkspace = null;
        }

        public IFeatureClass OpenBaseFeatureClass()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " on [ " + _mdbBaseLocation + " ]");

            IFeatureWorkspace featureWorkspace = AccessWorkspaceFromPath(_mdbBaseLocation) as IFeatureWorkspace;
            return featureWorkspace.OpenFeatureClass(GetFeatureClassName());
        }
        public IFeatureClass OpenEditsFeatureClass()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " on [ " + _mdbEditsLocation + " ]");

            IFeatureWorkspace featureWorkspace = AccessWorkspaceFromPath(_mdbEditsLocation) as IFeatureWorkspace;
            return featureWorkspace.OpenFeatureClass(GetFeatureClassName());
        }

        public MDBManager(SDEWorkspaceConnection sdeWorkspaceConnection, string featureClassName, string mdbLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info(String.Format("Source GDB [ {0} ] FeatureClass [ {1} ] File GDB [ {2} ]",
                sdeWorkspaceConnection.WorkspaceConnectionFile, featureClassName, mdbLocation));

            try
            {
                SDEWorkspace = sdeWorkspaceConnection; 
                _featureClassName = featureClassName;
                _mdbBaseLocation = mdbLocation;
                _mdbEditsLocation = mdbLocation.Replace(".mdb", MGDB_SUFFIX_EDITS + ".mdb");
                CleanupLDBs();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw ex;
            }

        }

        private void CleanupLDBs()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                if (File.Exists(_mdbBaseLocation.ToLower().Replace(".mdb", ".ldb")))
                {
                    File.Delete(_mdbBaseLocation.ToLower().Replace(".mdb", ".ldb"));
                }
                if (File.Exists(_mdbEditsLocation.ToLower().Replace(".mdb", ".ldb")))
                {
                    File.Delete(_mdbEditsLocation.ToLower().Replace(".mdb", ".ldb"));
                }
                if (!File.Exists(_mdbBaseLocation) && File.Exists(_mdbEditsLocation))
                {
                    _logger.Debug("Moving Edits to Base (recovering from previous lock)");

                    File.Move(_mdbEditsLocation, _mdbBaseLocation);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        static public bool TableSchemasAreEquivalent(ITable table1, ITable table2)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " table1 [ " + ((IDataset)table1).Name + " ] table2 [ " + ((IDataset)table1).Name + " ]");

            Geoprocessor GP = new Geoprocessor();
            ESRI.ArcGIS.DataManagementTools.TableCompare tableCompareTool = new
                ESRI.ArcGIS.DataManagementTools.TableCompare(table1, table2, "OBJECTID");

            tableCompareTool.compare_type = "SCHEMA_ONLY";

            IGeoProcessorResult pResult = (IGeoProcessorResult)GP.Execute(tableCompareTool, null);
            for (int i = 0; i < pResult.MessageCount; i++)
            {
                _logger.Debug(pResult.GetMessage(i));
            }

            return Convert.ToBoolean(pResult.ReturnValue);
        }

        public void CreateGDB(string location)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for location [ " + location + " ]");

            // Create a file geodatabase workspace factory.
            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.AccessWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);

            // Create a file geodatabase.
            IWorkspaceName workspaceName = workspaceFactory.Create(Path.GetDirectoryName(_mdbBaseLocation),
                Path.GetFileNameWithoutExtension(location),
                null, 0);

            // Open the geodatabase using the name object.
            _mdbName = (IName)workspaceName;

        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_mdbName != null) Marshal.FinalReleaseComObject(_mdbName);
            if (_mdbWorkspace != null) Marshal.FinalReleaseComObject(_mdbWorkspace);
            if (SDEWorkspace != null)
            {
                SDEWorkspace.KillConnection();
                SDEWorkspace = null;
            }
            _mdbName = null;
            _mdbWorkspace = null;

        }

        public void ExportToBaseGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ExportToMDB(_mdbBaseLocation);
        }

        public void ExportToEditsGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ExportToMDB(_mdbEditsLocation);
        }

        public static IWorkspace FileGdbWorkspaceFromPath(String path)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for path [ " + path + " ]");

            IWorkspace workspace = null;

            try
            {
                Type factoryType = Type.GetTypeFromProgID(
                    "esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                workspace = workspaceFactory.OpenFromFile(path, 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ErrorCodeException(ErrorCode.MDBBaseMissing, "MDB Can't be Opened at [ " + path + " ]", ex);
            }

            return workspace;
        }

        private void CreateAndOpenMDB(string mdbLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for [ " + mdbLocation + " ]");

            if (!File.Exists(mdbLocation))
            {
                CreateGDB(mdbLocation);
                _mdbWorkspace = (IWorkspace)_mdbName.Open();
            }
            else
            {
               System.IO.FileInfo FileInfo = new System.IO.FileInfo(mdbLocation);
               string  strfilename = Path.GetFileName(mdbLocation);
               string strnewfilename = strfilename.Split('.')[0] + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".mdb";
               string strfilepath = Path.GetDirectoryName(mdbLocation);
               //Copy the file with new name having date time stamp
               string  strnewpath = Path.Combine(strfilepath, strnewfilename);
               FileInfo.CopyTo(strnewpath, true);              
               //Deleting the old file 
               FileInfo.Delete();
               CreateGDB(mdbLocation);
               _mdbWorkspace = (IWorkspace)_mdbName.Open();

                //throw new ErrorCodeException(ErrorCode.MDBEditsExist, "MDB Already exists at location [ {0} ]" + mdbLocation);
            }

        }

        public bool CopyByGPTool()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                ProcessStartInfo start = default;
                string sourceConn = string.Empty;
                string destConn = string.Empty;
                string pythonexe = string.Empty;
                string pythonfile = string.Empty;
                string arguments = string.Empty;
                string standardError = string.Empty;
                List<string> argumentsList = new List<string>();
                List<string> pythonexeList = new List<string>();

                #region Python Path
                pythonexeList.Add(@"D:\python27\ArcGIS10.8\python.exe");
                pythonexeList.Add(@"c:\python27\arcgisx6410.1\python.exe");
                pythonexeList.Add(@"d:\arcgis10.0\python.exe");
                pythonexeList.Add(@"c:\arcgis10.0\python.exe");
                pythonexeList.Add(@"d:\python26\arcgis10.0\python.exe");
                pythonexeList.Add(@"c:\python26\arcgis10.0\python.exe");
                pythonexeList.Add(@"d:\python27\arcgis10.1\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.1\python.exe");
                pythonexeList.Add(@"d:\python27\arcgis10.2\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.2\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.8\python.exe");
                #endregion

                foreach (string str in pythonexeList)
                {
                    if (File.Exists(str))
                    {
                        pythonexe = str;
                    }
                }

                if (string.IsNullOrWhiteSpace(pythonexe))
                    throw new Exception("Valid Python Path not found");

                //Python File Name
                string dir = Assembly.GetExecutingAssembly().Location.Replace("PGE.Common.ChangesManager.dll", "");
                pythonfile = System.IO.Path.Combine(dir, "Python Files", "ExportToMDB.py");
                pythonfile = "\"" + pythonfile + "\"";

                start = new ProcessStartInfo();
                start.FileName = pythonexe;
                start.Arguments = string.Format("{0}", pythonfile);
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                    standardError = process.StandardError.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(standardError))
                {
                    throw new Exception("GP Tool Copy Failed with Error:" + standardError);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ErrorCodeException(ErrorCode.MDBBaseMissing, "Failed to Copy Data in MDB File", ex);
            }

        }

        private void ExportToMDB(string targetMDBLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for [ " + targetMDBLocation + " ]");

            CreateAndOpenMDB(targetMDBLocation);

            IFeatureClass featureClass = ((IFeatureWorkspace)SDEWorkspace.Workspace).OpenFeatureClass(_featureClassName);

            // Create workspace name objects.
            IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass();
            IWorkspaceName targetWorkspaceName = new WorkspaceNameClass();
            IName targetName = (IName)targetWorkspaceName;
            sourceWorkspaceName = ((IDatasetName)((IDataset)featureClass).FullName).WorkspaceName;

            targetWorkspaceName.PathName = targetMDBLocation;
            targetWorkspaceName.WorkspaceFactoryProgID =
                "esriDataSourcesGDB.AccessWorkspaceFactory";

            // Create a name object for the source feature class.
            IFeatureClassName featureClassName = new FeatureClassNameClass();

            // Set the featureClassName properties.
            IDatasetName sourceDatasetName = (IDatasetName)featureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = _featureClassName;
            IName sourceName = (IName)sourceDatasetName;

            // Create an enumerator for source datasets.
            IEnumName sourceEnumName = new NamesEnumeratorClass();
            IEnumNameEdit sourceEnumNameEdit = (IEnumNameEdit)sourceEnumName;

            // Add the name object for the source class to the enumerator.
            sourceEnumNameEdit.Add(sourceName);

            // Create a GeoDBDataTransfer object and a null name mapping enumerator.
            IGeoDBDataTransfer geoDBDataTransfer = new GeoDBDataTransferClass();
            IEnumNameMapping enumNameMapping = null;

            // Use the data transfer object to create a name mapping enumerator.
            Boolean conflictsFound = geoDBDataTransfer.GenerateNameMapping(sourceEnumName,
                targetName, out enumNameMapping);
            enumNameMapping.Reset();

            // Check for conflicts.
            if (conflictsFound)
            {
                // Iterate through each name mapping.
                INameMapping nameMapping = null;
                while ((nameMapping = enumNameMapping.Next()) != null)
                {
                    // Resolve the mapping's conflict (if there is one).
                    if (nameMapping.NameConflicts)
                    {
                        nameMapping.TargetName = nameMapping.GetSuggestedName(targetName);
                    }

                    // See if the mapping's children have conflicts.
                    IEnumNameMapping childEnumNameMapping = nameMapping.Children;
                    if (childEnumNameMapping != null)
                    {
                        childEnumNameMapping.Reset();

                        // Iterate through each child mapping.
                        INameMapping childNameMapping = null;
                        while ((childNameMapping = childEnumNameMapping.Next()) != null)
                        {
                            if (childNameMapping.NameConflicts)
                            {
                                childNameMapping.TargetName = childNameMapping.GetSuggestedName
                                    (targetName);
                            }
                        }
                    }
                }
            }

            // Start the transfer.
            bool isSuccess = false;
            _logger.Debug("Triggering python to export data to MDB");
            isSuccess = CopyByGPTool();
            
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(featureClass);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(enumNameMapping);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(featureClassName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(geoDBDataTransfer);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(sourceEnumName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(sourceWorkspaceName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(targetWorkspaceName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_mdbName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_mdbWorkspace);
            _mdbWorkspace = null;
            _mdbName = null;
            featureClass = null;
            SDEWorkspace.KillConnection();
        }

        public void RolloverGDBs()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Rollover from [ " + _mdbEditsLocation + " ] to [ " + _mdbBaseLocation + " ]");

            KillConnections();
            File.Delete(_mdbBaseLocation);
            File.Move(_mdbEditsLocation, _mdbBaseLocation);
        }


    }
}
