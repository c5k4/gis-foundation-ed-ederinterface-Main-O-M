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

namespace PGE.Common.ChangesManager
{
    public class FGDBManager : IExtractGDBManager
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private SDEWorkspaceConnection SDEWorkspace { get; set; }
        private IWorkspace _fgdbWorkspace;
        private IName _fgdbName;

        private string _featureClassName;
        private string _fgdbBaseLocation;
        private string _fgdbEditsLocation;
        private const string FGDB_SUFFIX_EDITS = "_edits";

        public string EditsLocation
        {
            get
            {
                return _fgdbEditsLocation;
            }
        }

        public string BaseLocation
        {
            get
            {
                return _fgdbBaseLocation;
            }
        }

        private string GetFGDBFeatureClassName()
        {
            if (_featureClassName.Contains("."))
            {
                return _featureClassName.Substring(_featureClassName.LastIndexOf(".") + 1);
            }

            return _featureClassName;
        }

        public void KillConnections()
        {
            if (_fgdbName != null) Marshal.FinalReleaseComObject(_fgdbName);
            if (_fgdbWorkspace != null) Marshal.FinalReleaseComObject(_fgdbWorkspace);
            
            _fgdbName = null;
            _fgdbWorkspace = null;
        }

        public IFeatureClass OpenBaseFeatureClass()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " on [ " + _fgdbBaseLocation + " ]" );

            IFeatureWorkspace featureWorkspace = FileGdbWorkspaceFromPath(_fgdbBaseLocation) as IFeatureWorkspace;
            return featureWorkspace.OpenFeatureClass(GetFGDBFeatureClassName());
        }
        public IFeatureClass OpenEditsFeatureClass()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " on [ " + _fgdbEditsLocation + " ]");

            IFeatureWorkspace featureWorkspace = FileGdbWorkspaceFromPath(_fgdbEditsLocation) as IFeatureWorkspace;
            return featureWorkspace.OpenFeatureClass(GetFGDBFeatureClassName());
        }

        public FGDBManager(SDEWorkspaceConnection sdeWorkspaceConnection, string featureClassName, string fgdbLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info(String.Format("Source GDB [ {0} ] FeatureClass [ {1} ] File GDB [ {2} ]",
                sdeWorkspaceConnection.WorkspaceConnectionFile, featureClassName, fgdbLocation));

            try
            {
                SDEWorkspace = sdeWorkspaceConnection; 
                _featureClassName = featureClassName;
                _fgdbBaseLocation = fgdbLocation;
                _fgdbEditsLocation = fgdbLocation.Replace(".gdb", FGDB_SUFFIX_EDITS + ".gdb");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw ex;
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
                "esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);

            // Create a file geodatabase.
            IWorkspaceName workspaceName = workspaceFactory.Create(Path.GetDirectoryName(_fgdbBaseLocation),
                Path.GetFileNameWithoutExtension(location),
                null, 0);

            // Open the geodatabase using the name object.
            _fgdbName = (IName)workspaceName;

        }

        public void ExportToBaseGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ExportToFGDB(_fgdbBaseLocation);
        }

        public void ExportToEditsGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ExportToFGDB(_fgdbEditsLocation);
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
                throw new ErrorCodeException(ErrorCode.FGDBBaseMissing, "FGDB Can't be Opened at [ " + path + " ]", ex);
            }

            return workspace;
        }

        private void CreateAndOpenFGDB(string fgdbLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for [ " + fgdbLocation + " ]");

            if (!Directory.Exists(fgdbLocation))
            {
                CreateGDB(fgdbLocation);
                _fgdbWorkspace = (IWorkspace)_fgdbName.Open();
            }
            else
            {
                throw new ErrorCodeException(ErrorCode.FGDBEditsExist, "FGDB Already exists at location [ {0} ]" + fgdbLocation);
            }

        }

        private void ExportToFGDB(string targetFGDBLocation)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for [ " + targetFGDBLocation + " ]");

            CreateAndOpenFGDB(targetFGDBLocation);

            IFeatureClass featureClass = ((IFeatureWorkspace)SDEWorkspace.Workspace).OpenFeatureClass(_featureClassName);

            // Create workspace name objects.
            IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass();
            IWorkspaceName targetWorkspaceName = new WorkspaceNameClass();
            IName targetName = (IName)targetWorkspaceName;
            sourceWorkspaceName = ((IDatasetName)((IDataset)featureClass).FullName).WorkspaceName;

            targetWorkspaceName.PathName = targetFGDBLocation;
            targetWorkspaceName.WorkspaceFactoryProgID =
                "esriDataSourcesGDB.FileGDBWorkspaceFactory";

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
            geoDBDataTransfer.Transfer(enumNameMapping, targetName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(enumNameMapping);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(geoDBDataTransfer);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(targetWorkspaceName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_fgdbName);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_fgdbWorkspace);
            _fgdbWorkspace = null;
            _fgdbName = null;
            featureClass = null;
            SDEWorkspace.KillConnection();
        }

        public void Dispose()
        {
        }
        public void RolloverGDBs()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Rollover from [ " + _fgdbEditsLocation + " ] to [ " + _fgdbBaseLocation + " ]" );

            KillConnections();
            Directory.Delete(_fgdbBaseLocation, true);
            Directory.Move(_fgdbEditsLocation, _fgdbBaseLocation);
        }


    }
}
