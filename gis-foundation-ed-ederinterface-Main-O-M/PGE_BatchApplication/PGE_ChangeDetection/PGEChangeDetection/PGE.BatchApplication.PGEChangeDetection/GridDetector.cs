using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.ChangeDetection.Exceptions;
using PGE.ChangesManager;
using ESRI.ArcGIS.Geometry;
using PGE.ChangesManagerShared;
using PGE.ChangesManagerShared.Interfaces;

namespace PGE.ChangeDetection
{

    public class GridDetectorWorkspaceInfo
    {
        public IWorkspace WorkspaceGridClass { get; set; }
        public string GridClassName { get; set; }
        public string GridMapNumberFieldName { get; set; }
        public string GridScaleFieldName { get; set; }
    }

    public class ChangeDetectionWorkspaceInfo
    {
        public IWorkspace WorkspaceChangeDetection { get; set; }
        public string VersionParentName { get; set; }
        public string VersionChildName { get; set; }
        public IWorkspace NonVersionedWorkspaceChangeDetection { get; set; }
        public string NonVersionedFeatureClassName { get; set; }
        public string NonVersionedFGDBLocation { get; set; }
    }

    /// <summary>
    /// Detects all changed grids spatially based on data changes.
    /// </summary>
    public class GridDetector
    {

        #region Constants
        /// <summary>
        /// The name of the temporary version to create during processing to ensure that no edits are lost during the application execution.
        /// </summary>
        private const string _VERSION_TEMP_NAME = "Daily_Change_Temp";
        #endregion

        #region Private Members
        private IWorkspace _workspaceChangeDetection;
        private IWorkspace _workspaceGridClass;

        private IVersion2 _versDailyChange = null;
        private IVersion2 _versDailyChangeTemp = null;
        private IVersion2 _versParentDefault = null;
        private IFeatureClass _unifiedGridFC = null;
        private int _gridIdxMapNo = -1;
        private int _gridIdxScale = -1;


        private GridDetectorWorkspaceInfo _gridDetectorWorkspaceInfo;
        private ChangeDetectionWorkspaceInfo _changeDetectionWorkspaceInfo;
        #endregion


        #region Public Members


        /// <summary>
        /// The name of the parent version.
        /// </summary>
        public string VersionParentName;

        /// <summary>
        /// The name of the child version.
        /// </summary>
        public string VersionChildName;

        /// <summary>
        /// The name of the "grid" feature class.
        /// </summary>
        public string GridFeatureClassName;

        /// <summary>
        /// The field name for "Map Number" in the "grid" feature class.
        /// </summary>
        public string GridMapNumberFieldName;

        /// <summary>
        /// The field name for "Scale" in the "grid" feature class.
        /// </summary>
        public string GridScaleFieldName;
        #endregion

        #region Constructor

        public GridDetector(ChangeDetectionWorkspaceInfo changeDetectionWorkspaceInfo, GridDetectorWorkspaceInfo gridDetectorWorkspaceInfo) :
            this(changeDetectionWorkspaceInfo.WorkspaceChangeDetection, gridDetectorWorkspaceInfo.WorkspaceGridClass, 
            changeDetectionWorkspaceInfo.VersionParentName, changeDetectionWorkspaceInfo.VersionChildName, 
            gridDetectorWorkspaceInfo.GridClassName, gridDetectorWorkspaceInfo.GridMapNumberFieldName, gridDetectorWorkspaceInfo.GridScaleFieldName)
        {
            _changeDetectionWorkspaceInfo = changeDetectionWorkspaceInfo;
            _gridDetectorWorkspaceInfo = gridDetectorWorkspaceInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDetector"/> class.
        /// </summary>
        /// <param name="workspaceChangeDetection">The GIS workspace to work with when obtaining grids and changes.</param>
        /// <param name="workspaceGridClass">The GIS workspace to work with for reading Grids.</param>
        /// <param name="versionParentName">The name of the parent version (usually SDE.DEFAULT) to use for change detection.</param>
        /// <param name="versionChildName">The name of the child version to use for change detection. This version will be a day old and as such, the parent version will have the edits.</param>
        /// <param name="gridClassName">The name of the grid feature class used to spatially find affected grid numbers.</param>
        /// <param name="gridMapNumberFieldName">The name of the field within the grid feature class that indicates a specific grid number.</param>
        /// <param name="gridScaleFieldName">The name of the field within the grid feature class that indicates a scale.</param>
        public GridDetector(IWorkspace workspaceChangeDetection, IWorkspace workspaceGridClass, string versionParentName, string versionChildName, string gridClassName, string gridMapNumberFieldName, string gridScaleFieldName)
        {
            //Initialize class-wide variables.
            _workspaceChangeDetection = workspaceChangeDetection;
            _workspaceGridClass = workspaceGridClass;
            VersionParentName = versionParentName;
            VersionChildName = versionChildName;
            GridFeatureClassName = gridClassName;
            GridMapNumberFieldName = gridMapNumberFieldName;
            GridScaleFieldName = gridScaleFieldName;

            //Get our versions
            FindVersions();

            //Find the grid feature class.
            try
            {
                _unifiedGridFC = (_workspaceGridClass as IFeatureWorkspace).OpenFeatureClass(GridFeatureClassName);
                if (_unifiedGridFC == null)
                    throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.GridClass, "Grid feature class is null.");

                _gridIdxMapNo = _unifiedGridFC.FindField(GridMapNumberFieldName);
                if (_gridIdxMapNo < 0)
                    throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.GridClass, "Cannot find map number field name from grid feature class.");

                _gridIdxScale = _unifiedGridFC.FindField(GridScaleFieldName);
                if (_gridIdxScale < 0)
                    throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.GridClass, "Cannot find scale field name from grid feature class.");
            }
            catch (Exception ex)
            {
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.GridClass, "Issue finding grid feature class (" + GridFeatureClassName + "): " + ex.Message, ex);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the differences between the two versions and spatially find the appropriate grids that have been changed.
        /// </summary>
        /// <param name="logger">The logger to use when writing output.</param>
        /// <returns>A collection of distinct grid numbers that have been affected and all of their associated scales.</returns>
        public Dictionary<string, string> GetAffectedGrids(TextWriterTraceListener logger)
        {
            //Use the utility classes to load the version differences into dictionaries.
            Program.WriteStatus("", true);
            Program.WriteStatus("Loading differences via ChangeManager...", true);
            VersionedChangeDetector changeManager = null;// new VersionedChangeReader(_versDailyChange, _versParentDefault, logger);
            //Currently we are passing in all three change types here - this may become configurable later.
            bool changesLoaded = changeManager.LoadDifferences(
                new esriDataChangeType[] { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate,
                    esriDataChangeType.esriDataChangeTypeDelete }
                );
            if (!changesLoaded)
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.ChangeManager, "ChangeManager issue - could not load differences.");

            //TODO: Refactor this whole thing out
            LoadNonVersionedChanges(changeManager);

            Program.WriteStatus("Changes loaded. Counts:", true);
            int insertCount = 0, updateCount = 0, deleteCount = 0;
            foreach (KeyValuePair<string, ChangeTable> p in changeManager.Inserts) { insertCount += p.Value.Count; }
            foreach (KeyValuePair<string, ChangeTable> p in changeManager.Updates) { updateCount += p.Value.Count; }
            foreach (KeyValuePair<string, ChangeTable> p in changeManager.Deletes) { deleteCount += p.Value.Count; }

            Program.WriteStatus("  Inserts = " + insertCount.ToString(), true);
            Program.WriteStatus("  Updates = " + updateCount.ToString(), true);
            Program.WriteStatus("  Deletes = " + deleteCount.ToString(), true);
            Program.WriteStatus("", true);

            //Use the dictionaries to find the affected grids.
            Program.WriteStatus("Grabbing feature geometries to find affected spatial grids.", true);
            Dictionary<string, string> affectedGrids = new Dictionary<string, string>();
            Program.WriteStatus("Inserts", true);
            FindGridsForChangeDictionary(changeManager, esriDataChangeType.esriDataChangeTypeInsert, ref affectedGrids);
            Program.WriteStatus("Updates", true);
            FindGridsForChangeDictionary(changeManager, esriDataChangeType.esriDataChangeTypeUpdate, ref affectedGrids);
            Program.WriteStatus("Deletes", true);
            FindGridsForChangeDictionary(changeManager, esriDataChangeType.esriDataChangeTypeDelete, ref affectedGrids);
            Program.WriteStatus("", true);

            Program.WriteStatus(affectedGrids.Count + " affected grids found.", true);
            affectedGrids = affectedGrids.OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Value);
            foreach (KeyValuePair<string, string> grid in affectedGrids)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("  Grid " + grid.Key);
                sb.Append(" (" + grid.Value + ")");

                Program.WriteStatus(sb.ToString(), true);
            }

            changeManager.Dispose();
            changeManager = null;

            return affectedGrids;
        }

        /// <summary>
        /// Free up any allocated resources or references
        /// </summary>
        public void Dispose()
        {
            while (Marshal.ReleaseComObject(_workspaceChangeDetection) > 0) { };
            while (Marshal.ReleaseComObject(_workspaceGridClass) > 0) { };
            while (Marshal.ReleaseComObject(_versDailyChange) > 0) { };
            while (Marshal.ReleaseComObject(_versDailyChangeTemp) > 0) { };
            while (Marshal.ReleaseComObject(_versParentDefault) > 0) { };
            while (Marshal.ReleaseComObject(_unifiedGridFC) > 0) { };

            _workspaceChangeDetection = null;
            _workspaceGridClass = null;
            _versDailyChange = null;
            _versDailyChangeTemp = null;
            _versParentDefault = null;
            _unifiedGridFC = null;
        }

        /// <summary>
        /// Intended to be run after all processing is complete. Rolls the versions forward by deleting the child version
        /// and renaming the temporary version to that of the child version.
        /// </summary>
        static public void RollVersions(IWorkspace workspaceChangeDetection, string versionDailyChangeName)
        {
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)workspaceChangeDetection;
                IVersion2 versDailyChange = (IVersion2)vWorkspace.FindVersion(versionDailyChangeName);
                Program.WriteStatus("Rolling Versions:", true);

                //Delete daily change version.
                string versionName = versDailyChange.VersionName;
                if (versionName.LastIndexOf('.') >= 0) versionName = versionName.Substring(versionName.LastIndexOf('.') + 1);
                versDailyChange.Delete();
                versDailyChange = null;
                Program.WriteStatus("  Deleted version (" + versionName + ").", true);

                //Rename the temporary version.
                IVersion2 versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(_VERSION_TEMP_NAME);
                versDailyChangeTemp.VersionName = versionName;
                Program.WriteStatus("  Renamed version " + _VERSION_TEMP_NAME + " to " + versionName + ".", true);
            }
            catch (Exception ex)
            {
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.VersionReset, "Issue rolling versions forward: " + ex.Message, ex);
            }
        }
        #endregion

        #region Private Methods

        private void LoadNonVersionedChanges(VersionedChangeDetector changeManager)
        {
            FGDBManager fgdbManager = new FGDBManager(_changeDetectionWorkspaceInfo.NonVersionedWorkspaceChangeDetection, 
                _changeDetectionWorkspaceInfo.NonVersionedFeatureClassName, 
                _changeDetectionWorkspaceInfo.NonVersionedFGDBLocation);
            FeatureCompareChangeDetector featureCompareChanges = new FeatureCompareChangeDetector(fgdbManager.OpenBaseFeatureClass(), 
                fgdbManager.OpenEditsFeatureClass());

            featureCompareChanges.LoadDifferences(
                new esriDataChangeType[] { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate,
                    esriDataChangeType.esriDataChangeTypeDelete });

            if (featureCompareChanges.Inserts.First().Value.Count > 0)
                changeManager.Inserts.Add(featureCompareChanges.Inserts);
            if (featureCompareChanges.Updates.First().Value.Count > 0)
                changeManager.Updates.Add(featureCompareChanges.Updates);
            if (featureCompareChanges.Deletes.First().Value.Count > 0)
                changeManager.Deletes.Add(featureCompareChanges.Deletes);

        }

        /// <summary>
        /// Use the class-level variables to load the versions into the class.
        /// </summary>
        private void FindVersions()
        {
            IVersionedWorkspace vWorkspace = (IVersionedWorkspace)_workspaceChangeDetection;

            Program.WriteStatus("Preparing versions:", true);
            try
            {
                //Find required versions (excluding the temp version, as it requires more logic).
                Program.WriteStatus("  Finding parent version (" + VersionParentName + ")...", true);
                _versParentDefault = (IVersion2)vWorkspace.FindVersion(VersionParentName);
                Program.WriteStatus("  Finding child version (" + VersionChildName + ")...", true);
                _versDailyChange = (IVersion2)vWorkspace.FindVersion(VersionChildName);
            }
            catch (Exception ex)
            {
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.VersionRetrieval, "Issue finding version: " + ex.Message, ex);
            }
            Program.WriteStatus("  Versions found.", true);

            //Validate the versions to make sure that the parent/child relationship is correct.
            if (!VersionIsChildOf(_versDailyChange, _versParentDefault))
            {
                string errorMsg = "The versions configured to run did not have a proper parent/child relationship.";
                if (VersionIsChildOf(_versParentDefault, _versDailyChange))
                    errorMsg += " Check to ensure that the version names are not in the wrong order.";

                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.VersionRetrieval, errorMsg);
            }

            //Try to find the temp version.
            Program.WriteStatus("  Looking for temporary version (" + _VERSION_TEMP_NAME + ")...", true);
            try { _versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(_VERSION_TEMP_NAME); }
            catch { }

            //Delete existing temp version if it exists.
            if (_versDailyChangeTemp != null)
            {
                _versDailyChangeTemp.Delete();
                Program.WriteStatus("    Deleted old temporary version (" + _VERSION_TEMP_NAME + ").", true);
            }
            //Get a fresh temp version so it reflects today's data and set it to protected so GDBM doesn't try to reconcile it
            _versParentDefault.CreateVersion(_VERSION_TEMP_NAME);
            Program.WriteStatus("    Created new temporary version (" + _VERSION_TEMP_NAME + ").", true);
            _versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(_VERSION_TEMP_NAME);
            _versDailyChangeTemp.Access = esriVersionAccess.esriVersionAccessProtected;
        }

        /// <summary>
        /// Determines whether or not the version is a child of the specified potential parent version.
        /// </summary>
        /// <param name="version">The version to compare with a potential parent version.</param>
        /// <param name="potentialParent">The potential parent version.</param>
        /// <returns><c>true</c> if the version is a child of the parent, otherwise <c>false</c>.</returns>
        private bool VersionIsChildOf(IVersion version, IVersion potentialParent)
        {
            if (!version.HasParent())
                return false;

            return version.VersionInfo.Parent.VersionName == potentialParent.VersionName;
        }

        /// <summary>
        /// Takes a dictionary of changes from the ChangeManager object and spatially extrapolates the relevant
        /// grid numbers.
        /// </summary>
        /// <param name="changeManager">The change manager containing the relevant change dictionaries.</param>
        /// <param name="changeType">The change type representing the desired dictionary to process.</param>
        /// <param name="affectedGrids">A dictionary containing grid numbers and each associated scale. Will be appended to by this method.</param>
        private void FindGridsForChangeDictionary(VersionedChangeDetector changeManager, esriDataChangeType changeType, 
            ref Dictionary<string, string> affectedGrids)
        {
            //Obtain the change dictionary based on the specified change type.
            string changeTypeInfo = "";
            ChangeDictionary changeDictionary = null;
            switch (changeType)
            {
                case esriDataChangeType.esriDataChangeTypeInsert: changeDictionary = changeManager.Inserts; changeTypeInfo = "inserts"; break;
                case esriDataChangeType.esriDataChangeTypeUpdate: changeDictionary = changeManager.Updates; changeTypeInfo = "updates"; break;
                case esriDataChangeType.esriDataChangeTypeDelete: changeDictionary = changeManager.Deletes; changeTypeInfo = "deletes"; break;
                default: return;
            }

            foreach (KeyValuePair<string, ChangeTable> change in changeDictionary)
            {
                //Retrieve the table from the object.
                //Currently not using ChangeRow.GetIRow() as the table object is instantiated every time using that method.
                //  We'll grab the table once on this level and use it for every change row.
                ITable changeTable = changeDictionary.AsTable(change.Key);
                if (!(changeTable is IFeatureClass))
                {
                    while (Marshal.ReleaseComObject(changeTable) > 0) { };
                    continue;
                }

                Program.WriteStatus("  Detecting " + changeTypeInfo + " for " + (changeTable as IDataset).Name, true);
                Program.WriteStatus("    " + change.Value.Count + " " + changeTypeInfo + " to process", true);

                //List for our oids
                List<int> oids = new List<int>();

                int counter = 0;
                //Loop through all change rows in this class.
                foreach (ChangeRow changeRow in change.Value)
                {
                    //string featureIdentifier = (changeTable as IDataset).Name + " #" + changeRow.OID;
                    try
                    {
                        counter++;
                        oids.Add(changeRow.OID);

                        //Query every 999 features or if this is our last changeRow
                        if ((oids.Count % 999 == 0) || change.Value.Count == counter)
                        {
                            //Create new geometry bag for use in our spatial query filters.  This should hopefully drastically
                            //improve performance.
                            IGeometryBag geometryBag = new GeometryBagClass();
                            IGeometryCollection geometryCollection = (IGeometryCollection)geometryBag;
                            IGeoDataset geoDataset = (IGeoDataset)changeTable;
                            ISpatialReference spatialReference = geoDataset.SpatialReference;
                            geometryBag.SpatialReference = spatialReference;

                            //Build our geometry bag for processing in the spatial filter
                            int[] oidsToProcess = oids.ToArray();
                            StringBuilder builder = new StringBuilder();
                            foreach(int oid in oidsToProcess)
                            {
                                builder.Append(oid + ", ");
                            }

                            string oidInList = builder.ToString().Substring(0, builder.Length - 2);

                            IQueryFilter qf = new QueryFilterClass();
                            qf.SubFields = (changeTable as IFeatureClass).ShapeFieldName;
                            qf.WhereClause = changeTable.OIDFieldName + " in (" + oidInList + ")"; 
                            IFeatureCursor changedRowsCursor = (changeTable as IFeatureClass).Search(qf, false);
                            IFeature changedRow = null;
                            object missingType = Type.Missing;
                            while ((changedRow = changedRowsCursor.NextFeature()) != null)
                            {
                                if (changedRow.Shape != null)
                                {
                                    geometryCollection.AddGeometry(changedRow.Shape, ref missingType, ref missingType);
                                }
                            }

                            //Build a spatial index for performance improvement
                            ISpatialIndex spatialIndex = (ISpatialIndex)geometryBag;
                            spatialIndex.AllowIndexing = true;
                            spatialIndex.Invalidate();

                            //Use spatial filter to detemine ALL intersected grids.
                            ISpatialFilter sf = new SpatialFilter();
                            sf.Geometry = geometryBag;
                            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            sf.SubFields = GridMapNumberFieldName + "," + GridScaleFieldName;
                            IFeatureCursor featCurs = (_unifiedGridFC as IFeatureClass).Search(sf, false);

                            //Due to differing grid sizes there will very likely be numerous results - loop through and retrieve all of them.
                            //bool foundGrid = false;
                            for (IFeature gridFeat = featCurs.NextFeature(); gridFeat != null; gridFeat = featCurs.NextFeature())
                            {
                                string gridNum = String.Copy(gridFeat.get_Value(_gridIdxMapNo).ToString());
                                string scale = String.Copy(gridFeat.get_Value(_gridIdxScale).ToString());
                                //Program.WriteStatus("  Found a grid: " + gridNum + " (scale " + scale + ")", false);

                                if (!affectedGrids.ContainsKey(gridNum))
                                    affectedGrids.Add(gridNum, scale);
                            }

                            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { };}
                            if (changedRowsCursor != null) { while (Marshal.ReleaseComObject(changedRowsCursor) > 0) { };}
                            if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { };}
                            if (geometryCollection != null) { while (Marshal.ReleaseComObject(geometryCollection) > 0) { };}
                            if (featCurs != null) { while (Marshal.ReleaseComObject(featCurs) > 0) { };}
                            if (sf != null) { while (Marshal.ReleaseComObject(sf) > 0) { };}

                            oids.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Continue if caught, but document the error.
                        Program.WriteWarning(ex, false);
                        continue;
                    }
                }
                while (Marshal.ReleaseComObject(changeTable) > 0) { };
            }
        }
        #endregion
    }
}
