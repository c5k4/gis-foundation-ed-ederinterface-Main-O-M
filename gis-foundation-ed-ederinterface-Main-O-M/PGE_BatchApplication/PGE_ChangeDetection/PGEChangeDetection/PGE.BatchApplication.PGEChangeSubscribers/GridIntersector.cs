using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.ChangesManagerShared;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.ChangesManagerShared.Utilities;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.ChangeDetectionAPI;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace PGE.BatchApplication.ChangeSubscribers
{

    public class GridIntersector
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        public Action<IFeature> ProcessFoundFeature { get; set; }

        public SDEWorkspaceConnection SDEWorkspace { get; private set; }
        private IFeatureClass _unifiedGridFC = null;
        private IFeatureClass UnifiedGridFC
        {
            get
            {
                if (_unifiedGridFC == null)
                {
                    _unifiedGridFC = (SDEWorkspace.Workspace as IFeatureWorkspace).OpenFeatureClass(GridFeatureClassName);
                    if (_unifiedGridFC == null)
                        throw new ErrorCodeException(ErrorCode.GridClass, "Grid feature class is null.");

                    _gridIdxMapNo = _unifiedGridFC.FindField(GridMapNumberFieldName);
                    if (_gridIdxMapNo < 0)
                        throw new ErrorCodeException(ErrorCode.GridClass, "Cannot find map number field name from grid feature class.");

                    _gridIdxScale = _unifiedGridFC.FindField(GridScaleFieldName);
                    if (_gridIdxScale < 0)
                        throw new ErrorCodeException(ErrorCode.GridClass, "Cannot find scale field name from grid feature class.");
                }
                return _unifiedGridFC;
            }
        }
        public string GridMapNumberFieldName
        {
            get;
            private set;
        }

        public string GridScaleFieldName
        {
            get;
            private set;
        }

        public string GridFeatureClassName
        {
            get;
            private set;
        }

        private int _gridIdxMapNo = -1;
        private int _gridIdxScale = -1;

        public GridIntersector(SDEWorkspaceConnection sdeWorkspaceConnection, string gridMapNumberFieldName, string gridScaleFieldName, string gridFeatureClassName)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info(String.Format("Grid GDB [ {0} ] MapNumberField [ {1} ] ScaleField [ {2} ] FeatureClass [ {3} ]",
                sdeWorkspaceConnection.WorkspaceConnectionFile, gridMapNumberFieldName, gridScaleFieldName, gridFeatureClassName));

            SDEWorkspace = sdeWorkspaceConnection; ;
            GridMapNumberFieldName = gridMapNumberFieldName;
            GridScaleFieldName = gridScaleFieldName;
            GridFeatureClassName = gridFeatureClassName;
        }

        public void InitializeWorkspace()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IWorkspace workspace = SDEWorkspace.Workspace;
            IFeatureClass featureClass = UnifiedGridFC;
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_unifiedGridFC != null) Marshal.FinalReleaseComObject(_unifiedGridFC);
            _unifiedGridFC = null;

            Marshal.ReleaseComObject(SDEWorkspace.Workspace);
            SDEWorkspace = null;
        }

        public Dictionary<string, string> GetIntersectingGrids(ITable table, IList<ChangeRow> changeRows)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string oidsCSV = string.Join(",", changeRows.Select(c => c.OID.ToString()).ToArray());

            return GetIntersectingGrids(table, oidsCSV);
        }

        public Dictionary<string, string> CDGetIntersectingGrids(ITable table, IList<ChangeRow> changeRows, IDictionary<string, ChangedFeatures> changedFeatures)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string oidsCSV = string.Join(",", changeRows.Select(c => c.OID.ToString()).ToArray());
            if(!changeRows.All(c => c.DifferenceType == esriDataChangeType.esriDataChangeTypeDelete))
                return GetIntersectingGrids(table, oidsCSV);
            else
                return CDGetIntersectingGrids(table, oidsCSV,changedFeatures);
        }

        public Dictionary<string, string> GetIntersectingGrids(ITable table, string changeRowOIDsCSV)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " changeRowOIDsCSV [ " + changeRowOIDsCSV + " ]");

            Dictionary<string, string> intersectingGrids = new Dictionary<string, string>();
            if (!(table is IFeatureClass)) return intersectingGrids;

            //Create new geometry bag for use in our spatial query filters.  This should hopefully drastically
            //improve performance.
            IGeometryBag geometryBag = BuildGeometryBag(table, changeRowOIDsCSV);

            //Build a spatial index for performance improvement
            ISpatialIndex spatialIndex = (ISpatialIndex)geometryBag;
            spatialIndex.AllowIndexing = true;
            spatialIndex.Invalidate();

            if (!geometryBag.IsEmpty)
            {
                intersectingGrids = GetIntersectingGridsFromGeometry(geometryBag);
            }

            if (geometryBag != null) Marshal.FinalReleaseComObject(geometryBag);

            return intersectingGrids;
        }

        public Dictionary<string, string> CDGetIntersectingGrids(ITable table, string changeRowOIDsCSV, IDictionary<string, ChangedFeatures> changedFeatures)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " changeRowOIDsCSV [ " + changeRowOIDsCSV + " ]");

            Dictionary<string, string> intersectingGrids = new Dictionary<string, string>();
            if (!(table is IFeatureClass)) return intersectingGrids;

            //Create new geometry bag for use in our spatial query filters.  This should hopefully drastically
            //improve performance.


            IGeometryBag geometryBag = CDBuildGeometryBag(table, changeRowOIDsCSV,changedFeatures);

            //Build a spatial index for performance improvement
            ISpatialIndex spatialIndex = (ISpatialIndex)geometryBag;
            spatialIndex.AllowIndexing = true;
            spatialIndex.Invalidate();

            if (!geometryBag.IsEmpty)
            {
                intersectingGrids = GetIntersectingGridsFromGeometry(geometryBag);
            }

            if (geometryBag != null) Marshal.FinalReleaseComObject(geometryBag);

            return intersectingGrids;
        }

        public Dictionary<string, string> GetIntersectingGridsFromGeometry(IGeometry geometry)
        {
            Dictionary<string, string> intersectingGrids = new Dictionary<string, string>();

            //Use spatial filter to detemine ALL intersected grids.
            ISpatialFilter sf = new SpatialFilter();
            sf.Geometry = geometry;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            sf.SubFields = GridMapNumberFieldName + "," + GridScaleFieldName;
            IFeatureCursor featCurs = (UnifiedGridFC as IFeatureClass).Search(sf, false);

            //Due to differing grid sizes there will very likely be numerous results - loop through and retrieve all of them.
            //bool foundGrid = false;
            for (IFeature gridFeat = featCurs.NextFeature(); gridFeat != null; gridFeat = featCurs.NextFeature())
            {
                string gridNum = gridFeat.get_Value(_gridIdxMapNo).ToString();
                string scale = gridFeat.get_Value(_gridIdxScale).ToString();
                _logger.Debug("  Found a mapNo: " + gridNum + " (mapNoInfo " + scale + ")");

                if (ProcessFoundFeature != null)
                {
                    ProcessFoundFeature(gridFeat);
                }

                if (!intersectingGrids.ContainsKey(gridNum))
                {
                    intersectingGrids.Add(gridNum, scale);
                }
            }

            if (featCurs != null) { while (Marshal.ReleaseComObject(featCurs) > 0) { };}
            if (sf != null) { while (Marshal.ReleaseComObject(sf) > 0) { };}

            return intersectingGrids;
        }

        private IGeometryBag BuildGeometryBag(ITable table, string changeRowOIDsCSV)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;
            IFeatureCursor changedFeaturesCursor = null;
            IGeometryBag geometryBag = null;

            try
            {
                geometryBag = new GeometryBagClass();
                IGeometryCollection geometryCollection = (IGeometryCollection)geometryBag;
                IGeoDataset geoDataset = (IGeoDataset)table;
                ISpatialReference spatialReference = geoDataset.SpatialReference;
                geometryBag.SpatialReference = spatialReference;

                qf = new QueryFilterClass();
                qf.SubFields = (table as IFeatureClass).ShapeFieldName;
                qf.WhereClause = table.OIDFieldName + " in (" + changeRowOIDsCSV + ")";
                changedFeaturesCursor = (table as IFeatureClass).Search(qf, false);
                IFeature changedFeature = null;
                object missingType = Type.Missing;
                while ((changedFeature = changedFeaturesCursor.NextFeature()) != null)
                {
                    if (changedFeature.Shape != null)
                    {
                        geometryCollection.AddGeometry(changedFeature.Shape, ref missingType, ref missingType);
                    }
                }
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { };}
                if (changedFeaturesCursor != null) { while (Marshal.ReleaseComObject(changedFeaturesCursor) > 0) { };}

            }
            return geometryBag;
        }

        private IGeometryBag CDBuildGeometryBag(ITable table, string changeRowOIDsCSV, IDictionary<string, ChangedFeatures> changedFeatures)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;
            IFeatureCursor changedFeaturesCursor = null;
            IGeometryBag geometryBag = null;
            string[] changeRowOIDRows = default;
            string FCName = string.Empty;
            IGeometry geometry = default;

            try
            {
                geometryBag = new GeometryBagClass();
                IGeometryCollection geometryCollection = (IGeometryCollection)geometryBag;
                IGeoDataset geoDataset = (IGeoDataset)table;
                ISpatialReference spatialReference = geoDataset.SpatialReference;
                geometryBag.SpatialReference = spatialReference;

                if (string.IsNullOrWhiteSpace(changeRowOIDsCSV))
                    return geometryBag;

                changeRowOIDRows = changeRowOIDsCSV.Split(',');

                if(table!=null)
                    FCName = ((IDataset)table).BrowseName.ToUpper();

                object missingType = Type.Missing;

                foreach (string oid in changeRowOIDRows)
                {
                    if(changedFeatures.ContainsKey(FCName))
                    {
                        if(changedFeatures[FCName].Action.Delete.Any(c => c.OID == Convert.ToInt32(oid)))
                        {
                            geometry = changedFeatures[FCName].Action.Delete.First(c => c.OID == Convert.ToInt32(oid)).geometry_Old;
                            if(geometry!=null)
                                geometryCollection.AddGeometry(geometry, ref missingType, ref missingType);
                        }
                    }
                }

                //qf = new QueryFilterClass();
                //qf.SubFields = (table as IFeatureClass).ShapeFieldName;
                //qf.WhereClause = table.OIDFieldName + " in (" + changeRowOIDsCSV + ")";
                //changedFeaturesCursor = (table as IFeatureClass).Search(qf, false);
                //IFeature changedFeature = null;
                //object missingType = Type.Missing;
                //while ((changedFeature = changedFeaturesCursor.NextFeature()) != null)
                //{
                //    if (changedFeature.Shape != null)
                //    {
                //        geometryCollection.AddGeometry(changedFeature.Shape, ref missingType, ref missingType);
                //    }
                //}
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { }; }
                if (changedFeaturesCursor != null) { while (Marshal.ReleaseComObject(changedFeaturesCursor) > 0) { }; }

            }
            return geometryBag;
        }

    }
}
