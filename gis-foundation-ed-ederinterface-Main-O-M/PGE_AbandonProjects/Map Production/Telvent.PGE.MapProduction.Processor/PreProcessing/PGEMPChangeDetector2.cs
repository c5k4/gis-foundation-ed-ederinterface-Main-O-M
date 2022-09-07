using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using Telvent.Delivery.Framework;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Telvent.PGE.MapProduction.PreProcessing
{
    /// <summary>
    /// Implements ChangeDetection for PGE MapProduction
    /// </summary>
    /// <remarks>
    /// This class detects the changes from 2 ways
    ///<list type="bullet">
    /// <item>From MaintenancePlat features recorded in ModifiedMaps table</item>
    /// <item>From MaintenancePlat fatures overlapping with the newwly created WIPCloud features</item>
    /// </list>
    /// </remarks>
    public class PGEMPChangeDetector2 : PGEMPChangeDetector
    {
        #region Private Variables
        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");

        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Constructor gets the Workspace to use for look ing at the Change detection table
        /// </summary>
        /// <param name="workspace">Workspace to which MaintenancePolygon belong to</param>
        /// <param name="gdbDataManager">Instance that holds the information related to MaintenancePlat feature class</param>
        public PGEMPChangeDetector2(IWorkspace workspace, IMPGDBDataManager gdbDataManager)
            : base(workspace, gdbDataManager)
        {
        }
        #endregion Constructor

        #region Public Overridden Methods

        /// <summary>
        /// Retrieves the Changed MaintenancePlat keys in 2 ways.
        /// <list type="bullet">
        /// <item>From MaintenancePlat features recorded in ModifiedMaps table</item>
        /// <item>From MaintenancePlat fatures overlapping with the newwly created WIPCloud features</item>
        /// </list>
        /// </summary>
        /// <returns>Returns the combined list of Changed MaintenancePlat keys in both the ways.</returns>
        public override List<string> GetChangedKey()
        {
            //Get the Unique keys from base class i.e. from MaintenancePlat features recorded in ModifiedMaps table
            List<string> changedKeys = base.GetChangedKey();
            //Get the unique keys retreived from MaintenancePlat feature overlaps with new created WIP Polygons
            List<string> changedKeysFromDateCreatedOrModified = GetChangedKeysFromNewWIPs();
            //Get the unique combination of both the lists
            changedKeys = changedKeys.Union(changedKeysFromDateCreatedOrModified).ToList();
            //return the unique keys
            return changedKeys;
        }

        #endregion Public Overridden Methods

        #region Private Methods

        /// <summary>
        /// Retrieves the Unique Keys (MapNumber:MapOffice) of MaintenacePlat features overlapping with the WIP Polygons created today.
        /// </summary>
        /// <returns>Returns the Unique Keys (MapNumber:MapOffice) of MaintenacePlat features overlapping with the WIP Polygons created today.</returns>
        private List<string> GetChangedKeysFromNewWIPs()
        {
            List<string> changedKeys = new List<string>();
            IGeometryCollection geometryColl = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor featCursor = null;

            try
            {
                //Get the WIP feature class from WIP Geodatabase settings
                IFeatureClass wipFeatureClass = GetWIPPolygonClass();
                if (wipFeatureClass == null) return changedKeys;

                //Get DateCreated field in WIP Feature class
                string dateCreatedFieldName = string.Empty;
                string dateModifiedFieldName = string.Empty;
                if (MapProductionConfigurationHandler.Settings.ContainsKey("WIPDateCreationFieldName") && MapProductionConfigurationHandler.Settings.ContainsKey("WIPDateModifiedFieldName"))
                {
                    dateCreatedFieldName = MapProductionConfigurationHandler.Settings["WIPDateCreationFieldName"];
                    dateModifiedFieldName = MapProductionConfigurationHandler.Settings["WIPDateModifiedFieldName"];
                }
                else
                {
                    _logger.Debug(" Either 'WIPDateCreationFieldName' or 'WIPDateModifiedFieldName' key does not exist in the Map Production Config settings."); return changedKeys;
                }
                _logger.Debug("DateCreated Field Name in '" + wipFeatureClass.AliasName + "' class is '" + dateCreatedFieldName + "'");
                _logger.Debug("DateModified Field Name in '" + wipFeatureClass.AliasName + "' class is '" + dateModifiedFieldName + "'");

                if (string.IsNullOrEmpty(dateCreatedFieldName) || wipFeatureClass.FindField(dateCreatedFieldName) == -1)
                {
                    _logger.Debug(string.Format("Either '{0}' field is invalid or not present in '{1}' feature class.", dateCreatedFieldName, wipFeatureClass.AliasName));
                    return changedKeys;
                }
                if (string.IsNullOrEmpty(dateModifiedFieldName) || wipFeatureClass.FindField(dateModifiedFieldName) == -1)
                {
                    _logger.Debug(string.Format("Either '{0}' field is invalid or not present in '{1}' feature class.", dateModifiedFieldName, wipFeatureClass.AliasName));
                    return changedKeys;
                }

                //Preapre filter to get the WIP features that are created today
                IQueryFilter filter = new QueryFilterClass();
                string dateFormat = "yyyy-MM-dd";
                string dateToday = DateTime.Now.ToString(dateFormat);
                filter.WhereClause = string.Format("{2} >= TO_DATE('{0}','{1}') OR {3} >= TO_DATE('{0}','{1}')", new object[] { dateToday, dateFormat, dateCreatedFieldName, dateModifiedFieldName });
                filter.SubFields = wipFeatureClass.ShapeFieldName;
                //Get the WIP features that are created today
                featCursor = wipFeatureClass.Search(filter, true);

                //Collect the geometry of the WIP features created today
                geometryColl = new GeometryBagClass();
                IFeature wipFeature = null;
                //Collect the WIP geometries of all the created features
                while ((wipFeature = featCursor.NextFeature()) != null)
                {
                    geometryColl.AddGeometry(wipFeature.ShapeCopy);
                }
                //Release cursor
                while (Marshal.ReleaseComObject(featCursor) > 0) { }

                _logger.Debug(string.Format("No. of features with {0} are {1}", filter.WhereClause, geometryColl.GeometryCount));
                if (geometryColl.GeometryCount < 1) return changedKeys;

                //Get the Uninon of these geometries
                topoOperator = new PolygonClass();
                topoOperator.ConstructUnion(geometryColl as IEnumGeometry);

                IGeometry queryGeometry = topoOperator as IGeometry;
                if (queryGeometry.IsEmpty)
                {
                    _logger.Debug("Union of the queried WIP geometries is empty."); return changedKeys;
                }
                //Get the reference to MaintenancePlat feature class
                IFeatureClass maintenancePlatClass = base.GDBDataManager.MapPolygonFeatureClass;

                //prepare filter to get the MaintenancePlat features overlapping with the constructed polygon
                ISpatialFilter spaFilter = new SpatialFilter();
                spaFilter.Geometry = queryGeometry;
                spaFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                spaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spaFilter.GeometryField = maintenancePlatClass.ShapeFieldName;

                //Get the MapNumber and MapOffice field name and indexes
                string mapOfficeFiedName = ModelNameFacade.FieldNameFromModelName(maintenancePlatClass, PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN);
                int mapOfficeFiedIndex = ModelNameFacade.FieldIndexFromModelName(maintenancePlatClass, PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN);
                int mapNumberFiedIndex = base.GDBDataManager.MapNumberFieldIndex;
                string mapNumberFieldName = maintenancePlatClass.Fields.get_Field(base.GDBDataManager.MapNumberFieldIndex).Name;
                if (mapNumberFiedIndex == -1)
                {
                    _logger.Debug("Map Number field is missing or not assigned with model name " + PGEGDBFieldLookUp.PGEGDBFields.MapNumberMN); return changedKeys;
                }
                if (mapOfficeFiedIndex == -1)
                {
                    _logger.Debug("Map Office field is missing or not assigned with model name " + PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN); return changedKeys;
                }

                //Get the MaintenancePlat features overlapping with the constructed polygon
                spaFilter.SubFields = maintenancePlatClass.OIDFieldName + "," + maintenancePlatClass.ShapeFieldName + "," + mapNumberFieldName + "," + mapOfficeFiedName;
                featCursor = maintenancePlatClass.Search(spaFilter, true);

                string mapNumber, mapOffice;
                IFeature maintenanceFeature = null;
                while ((maintenanceFeature = featCursor.NextFeature()) != null)
                {
                    //Get the MapNumber and MapOffice values
                    mapNumber = maintenanceFeature.get_Value(mapNumberFiedIndex) == null ? string.Empty : maintenanceFeature.get_Value(mapNumberFiedIndex).ToString();
                    if (mapNumber == string.Empty)
                    {
                        _logger.Debug("Map Number is null for MaintenancePlat feature with OID = " + maintenanceFeature.OID.ToString()); continue;
                    }
                    mapOffice = maintenanceFeature.get_Value(mapOfficeFiedIndex) == null ? string.Empty : maintenanceFeature.get_Value(mapOfficeFiedIndex).ToString();
                    if (mapOffice == string.Empty)
                    {
                        _logger.Debug("Map Office is null for MaintenancePlat feature with OID = " + maintenanceFeature.OID.ToString()); continue;
                    }

                    //prepare combination of MapNumber:MapOffice and add to the list if does not exist already
                    if (changedKeys.Contains(mapNumber + ":" + mapOffice)) continue;
                    changedKeys.Add(mapNumber + ":" + mapOffice);
                }
                //Release cursor
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
            }
            finally
            {
                //Release the resources
                if (geometryColl != null)
                {
                    while (Marshal.ReleaseComObject(geometryColl) > 0) { }
                }
                if (topoOperator != null)
                {
                    while (Marshal.ReleaseComObject(topoOperator) > 0) { }
                }
                if (featCursor != null)
                {
                    while (Marshal.ReleaseComObject(featCursor) > 0) { }
                }
            }
            return changedKeys;
        }

        /// <summary>
        /// Get the credentials of the databse containing WIPCloud feature class
        /// </summary>
        /// <returns>Returns the credentials of the databse containing WIPCloud feature class</returns>
        private IPropertySet GetWIPPropertySet()
        {
            IPropertySet propSet = new PropertySetClass();
            if (!MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBInstance") || string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBInstance"]))
                throw new NullReferenceException("Instance is required");
            else
                propSet.SetProperty("INSTANCE", MapProductionConfigurationHandler.Settings["WIPGDBInstance"]);

            if (!MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBUserName") || string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBUserName"]))
                throw new NullReferenceException("Username is required");
            else
                propSet.SetProperty("USER", MapProductionConfigurationHandler.Settings["WIPGDBUserName"]);

            if (!MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBPassword") || string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBPassword"]))
                throw new NullReferenceException("Password is required");
            else
                propSet.SetProperty("PASSWORD", MapProductionConfigurationHandler.Settings["WIPGDBPassword"]);

            if (MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBDatabase") && !string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBDatabase"]))
                propSet.SetProperty("DATABASE", MapProductionConfigurationHandler.Settings["WIPGDBDatabase"]);

            if (MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBServer") && !string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBServer"]))
                propSet.SetProperty("SERVER", MapProductionConfigurationHandler.Settings["WIPGDBServer"]);

            if (MapProductionConfigurationHandler.Settings.ContainsKey("WIPGDBVersion") && !string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPGDBVersion"]))
                propSet.SetProperty("VERSION", MapProductionConfigurationHandler.Settings["WIPGDBVersion"].ToUpper());
            else
                propSet.SetProperty("VERSION", "SDE.DEFAULT");
            return propSet;
        }

        /// <summary>
        /// Retrieves the reference to WIPCloud polygon features class
        /// </summary>
        /// <returns></returns>
        private IFeatureClass GetWIPPolygonClass()
        {
            IPropertySet propSet = null;
            IWorkspaceFactory sdeFactory = null;
            IFeatureClass wipPolygonClass = null;
            try
            {
                propSet = GetWIPPropertySet();
                if (propSet == null || propSet.Count < 1)
                {
                    _logger.Error("WIP Geodatbase settings are invalid."); return wipPolygonClass;
                }
                sdeFactory = new SdeWorkspaceFactoryClass();
                IFeatureWorkspace wipWorkspace = sdeFactory.Open(propSet, 0) as IFeatureWorkspace;
                if (wipWorkspace == null)
                {
                    _logger.Error("WIP Geodatbase settings are invalid or the database is invalid.");
                    return wipPolygonClass;
                }
                if (!MapProductionConfigurationHandler.Settings.ContainsKey("WIPClassName") || string.IsNullOrEmpty(MapProductionConfigurationHandler.Settings["WIPClassName"]))
                {
                    _logger.Error("WIP Polygon feature class mentioned in config file is invalid.");
                    return null;
                }
                if (!(wipWorkspace as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureClass, MapProductionConfigurationHandler.Settings["WIPClassName"]))
                {
                    _logger.Error(string.Format("{0} feature class does not exist in WIP Geodatabase.", MapProductionConfigurationHandler.Settings["WIPClassName"]));
                    return null;
                }
                wipPolygonClass = wipWorkspace.OpenFeatureClass(MapProductionConfigurationHandler.Settings["WIPClassName"]);
            }
            finally
            {
                if (propSet != null)
                {
                    while (Marshal.ReleaseComObject(propSet) > 0) { };
                }
                if (sdeFactory != null)
                {
                    while (Marshal.ReleaseComObject(sdeFactory) > 0) { };
                }
            }
            return wipPolygonClass;
        }

        #endregion Private Methods
    }
}
