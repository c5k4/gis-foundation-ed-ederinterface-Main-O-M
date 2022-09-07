using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Records maps that should be plotted by the Mass Map Production Tool overnight based on the features changed.
    /// On modification of a feature will run only when the feature's shape has changed or for Annotation featureclass if hte shape has changed or the TextString field value has changed.
    /// </summary>
    [ComVisible(true)]
    [Guid("1ADB60F5-56EE-4068-B694-0BBDF01769C8")]
    [ProgId("PGE.Desktop.EDER.RecordChangedMaps")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class RecordChangedMaps : BaseSpecialAU
    {
        #region Private members
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Class ModelName for the Change Table
        /// </summary>
        private const string ChangeTableMN = "PGE_MODIFIEDMAPS";
        /// <summary>
        /// Class Modelname for the Distribution Map Featureclass
        /// </summary>
        private const string DistMapMN = "PGE_DISTRIBUTIONMAP";
        /// <summary>
        /// Field Modelname for the Map Office field on the Distribution Map featureclass
        /// </summary>
        private const string MapOfficeMN = "PGE_MAPOFFICE";
        /// <summary>
        /// Field Modelname for the Map Type field on the Distribution Map featureclass
        /// </summary>
        private const string MapTypeMN = "PGE_MAPTYPE";
        /// <summary>
        /// Field Modelname for the LocalOffice field on the Distribution Map featureclass
        /// </summary>
        private const string LocalOfficeMN = "PGE_LOCALOFFICE";
        /// <summary>
        /// Field Modelname for the LocalOffice field on the Feature being edited
        /// </summary>
        private const string FeatureLocalOfficeMN = "PGE_LOCALOFFICE";
        /// <summary>
        /// Field Model name for the Map Number field in the Distribution Map featureclass
        /// </summary>
        private const string MapNumberMN = "PGE_DISTRIBUTIONMAPNUMBER";
        /// Version Name field in Changes table
        /// </summary>
        private const string _versionNameField = "Version";
        /// <summary>
        /// Value Name field in Changes table
        /// </summary>
        private const string _fieldValueField = "MODIFIEDMAPS_VALUE";
        /// <summary>
        /// Modified Date field in Changes table
        /// </summary>
        private const string _modefiedDateField = "ModifiedDate";
        /// <summary>
        /// Table Namr storing the changes
        /// </summary>
        private const string _tableModelName = "PGE_MODIFIEDMAPS";
        /// <summary>
        /// Distribution Map Number field Index on the Maintenance Plat Featureclass
        /// </summary>
        private string _distMapNumberFieldName = string.Empty;
        /// <summary>
        /// Map Office Field Index on hte Maintenance Plat featureclass
        /// </summary>
        private string _mapOfficeFieldName = string.Empty;
        /// <summary>
        /// Map Type Field Index on hte Maintenance Plat featureclass
        /// </summary>
        private string _mapTypeFieldName = string.Empty;
        /// <summary>
        /// Local Office Field Index on hte Maintenance Plat featureclass
        /// </summary>
        private string _localOfficeFieldName = string.Empty;
        /// <summary>
        /// Field modelname that should be assigned on field that trigger a change for MapProduction if it changed
        /// </summary>
        private const string _triggerMN = "PGE_TRIGGERMAPCHANGE";
        /// <summary>
        /// Separator to use between MapOffice and Distribution Map Number before persisting to the database.
        /// </summary>
        private const string Connector = ":";
        /// <summary>
        /// The PGE_ModifiedMaps Objectclass. The objectclass that has the PGE_MODIFIEDMAPS modelname assigned
        /// </summary>
        IObjectClass _changeTable = null;
        /// <summary>
        /// Distribution Map featureclass. The featureclass that has the PGE_DISTRIBUTIONMAP modelname assigned
        /// </summary>
        IFeatureClass _distMapFC = null;
        #endregion

        #region CTOR
        /// <summary>
        /// Creates an Instance of the AU. Called by the ArcFM autoupdater framework
        /// </summary>
        public RecordChangedMaps()
            : base("PGE Record Changed Map Number")
        {
        }
        #endregion

        #region Base AU Overrides
        /// <summary>
        /// Core logic implementaiton.
        /// Checks if the objectclass with modelname PGE_MODIFIEDMAPS and featureclass with modelname PGE_DISTRIBUTIONMAP exists in the edit database
        /// Checks if the feature is added new or edited. If edited checks if the feature's shape is edited. For Annotation features in addiiton to shape checks if hte TextString field is edited.
        /// if the above conditions are true will search for the DistributionMap features that intersects the feature that is edited/added/deleted
        /// Gets the MapOffice and MapNumber for each map polygon found and records the MapNumber:MapOffice value pair separated by comma to the Value field along with the version name and date modified.
        /// </summary>
        /// <param name="obj">Feature being edited</param>
        /// <param name="eAUMode">Autoupdater Mode</param>
        /// <param name="eEvent">Event - Create/Delete/Update etc.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eAUMode != mmAutoUpdaterMode.mmAUMArcMap)
                return;
            if (ShouldFire(obj, eEvent))
            {
                SetUpDatabaseTables(obj);
                if (_changeTable != null && _distMapFC != null)
                {
                    if (string.IsNullOrEmpty(_mapOfficeFieldName) || string.IsNullOrEmpty(_distMapNumberFieldName))
                    {
                        _logger.Error("MapOffice/MapNumber fields are not configured on he MaintenancePlat featureclass");
                        return;
                    }
                    //object localOffice = obj.GetFieldValue(null, true, FeatureLocalOfficeMN);
                    //localOffice = (localOffice != null && localOffice != DBNull.Value) ? localOffice : string.Empty;
                    //List<string> persistValue = GetPersistValue((IFeature)obj, localOffice.ToString());
                    List<string> persistValue = GetPersistValue((IFeature)obj);
                    //Check if the MapNumber field of teh feature has changed if the feature has MapNumber field 
                    //If MapNumber field is not there check if the feature shape has changed and get another list from the old shape and add it to the mix.
                    persistValue.AddRange(GetOtherMapNumbers(obj));
                    
                    PersistChange(persistValue.Distinct().ToList(), obj);
                }
                else
                {
                    _logger.Error("Maintenace Plat featureclass or the Change tables are configured with modelnames");
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Checks for the conditions and returns true if all the conditions satifies for firing the AU
        /// Returns true if hte following conditions are met.
        /// Checks if the feature is added new or edited. If edited checks if the feature's shape is edited. In addiiton to shape changes checks if any field with modelname PGE_TRIGGERMAPCHANGE field has changed.
        /// else returns false
        /// </summary>
        /// <param name="obj">Object that is being edited</param>
        /// <param name="eEvent">Edit Event</param>
        /// <returns>boolean</returns>
        private bool ShouldFire(IObject obj, mmEditEvent eEvent)
        {
            bool retVal = false;
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureDelete) return true;
            if (obj is IFeature)
            {
                //Any change to annotation should trigger a change.
                if (obj is IAnnotationFeature)
                {
                    return true;
                }
                IFeatureChanges featureChanges = (IFeatureChanges)obj;
                if (featureChanges.ShapeChanged)
                {
                    retVal = true;
                }
                else
                {
                    //Only fields that affects the symbology move the feature status etc should trigger change
                    List<int> fieldIdxToCheck = ModelNameFacade.FieldIndicesFromModelName(obj.Class, _triggerMN);
                    if (fieldIdxToCheck.Count == 0) return true; 
                    IRowChanges rowChanges = (IRowChanges)obj;
                    foreach (int idx in fieldIdxToCheck)
                    {
                        if (rowChanges.get_ValueChanged(idx))
                        {
                            retVal = true;
                            break;
                        }
                    }
                }
            }
            return retVal;
        }
        /// <summary>
        /// Sets up the tables required for recording edited maps.
        /// Featureclass with Modelname PGE_DISTRIBUTIONMAP and Objectclass with PGE_MODIFIEDMAPS are searched for and set up on a variable
        /// </summary>
        /// <param name="obj">Object that is edited</param>
        private void SetUpDatabaseTables(IObject obj)
        {
            if (obj != null)
            {
                IWorkspace wkspc = ((IDataset)obj.Class).Workspace;
                if (wkspc is IFeatureWorkspace)
                {
                    if (_changeTable == null)
                    {
                        _changeTable = ModelNameFacade.ObjectClassByModelName(wkspc, ChangeTableMN);
                    }
                    if (_distMapFC == null)
                    {
                        _distMapFC = ModelNameFacade.FeatureClassByModelName(wkspc, DistMapMN);
                        if (_distMapFC != null)
                        {
                            _mapOfficeFieldName = ModelNameFacade.FieldNameFromModelName(_distMapFC, MapOfficeMN);
                            _distMapNumberFieldName = ModelNameFacade.FieldNameFromModelName(_distMapFC, MapNumberMN);
                            _localOfficeFieldName = ModelNameFacade.FieldNameFromModelName(_distMapFC, LocalOfficeMN);
                            _mapTypeFieldName = ModelNameFacade.FieldNameFromModelName(_distMapFC, MapTypeMN);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs a spatial query and gets the value of the MapNumber:MapOffice for each of the map polygon that intersects the feature being edited.
        /// The method first gets a list of all features that falls underneath the point/line
        /// Creates a dictionary of MapScale and MapNumber:MapOffice and LocalOffice
        /// then checks if the localoffice of the feature matches any of hte map polygon's map office
        /// if a MapPolygons' localoffice matches then everything in that scale that overlaps are tagged as changed.
        /// If no local office is found to match then only the lowest scale maps on which the feature falls is considered changed.
        /// </summary>
        /// <param name="feature">Feature that is edited</param>
        /// <param name="localoffice">LocalOfficeID value of the feature</param>
        /// <param name="queryingGeometry">The geometry used for the spatil query. If null is passed or omitted then feature's current geometry is used</param>
        /// <returns>Returns the list of the Map Numbers to be recorded for printing</returns>
        private List<string> GetPersistValueWithLocalOffice(IFeature feature, string localoffice = "", IGeometry queryingGeometry = null)
        {
            List<string> retVal = new List<string>();
            if (!string.IsNullOrEmpty(_mapOfficeFieldName) && !string.IsNullOrEmpty(_mapTypeFieldName) && !string.IsNullOrEmpty(_localOfficeFieldName) && !string.IsNullOrEmpty(_distMapNumberFieldName))
            {
                ISpatialFilter spFilter = new SpatialFilterClass();
                spFilter.Geometry = (queryingGeometry != null ? queryingGeometry : feature.Shape as IGeometry);
                spFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor featCursor = _distMapFC.Search(spFilter, false);
                try
                {
                    IFeature mapFeature = null;
                    object mapOfficeobj = null;
                    object mapNumberobj = null;
                    object mapTypeobj = null;
                    object localOfficeobj = null;
                    //Dictionary of <MapType,<MapNumber:MapOffice,LocalOffice>>
                    SortedDictionary<int, Dictionary<string, List<string>>> mapTypeDict = new SortedDictionary<int, Dictionary<string, List<string>>>();
                    Dictionary<string, List<string>> mapsDict = null;
                    List<string> localOfficeList = null;
                    int defaultMapTypeValue = 9999;
                    int mapType;
                    while ((mapFeature = featCursor.NextFeature()) != null)
                    {
                        mapOfficeobj = mapFeature.GetFieldValue(_mapOfficeFieldName, true);
                        mapOfficeobj = (mapOfficeobj == null || mapOfficeobj == DBNull.Value) ? string.Empty : mapOfficeobj;
                        mapNumberobj = mapFeature.GetFieldValue(_distMapNumberFieldName, true);
                        mapNumberobj = (mapNumberobj == null || mapNumberobj == DBNull.Value) ? string.Empty : mapNumberobj;
                        mapTypeobj = mapFeature.GetFieldValue(_mapTypeFieldName, false);
                        mapType = (mapTypeobj == null || mapTypeobj == DBNull.Value) ? defaultMapTypeValue : Convert.ToInt32(mapTypeobj);
                        if (mapType == 0) mapType = defaultMapTypeValue;
                        localOfficeobj = mapFeature.GetFieldValue(_localOfficeFieldName, true);
                        localOfficeobj = (localOfficeobj == null || localOfficeobj == DBNull.Value) ? string.Empty : localOfficeobj;
                        if (!string.IsNullOrEmpty(mapOfficeobj.ToString()) && !string.IsNullOrEmpty(mapNumberobj.ToString()))
                        {
                            if (!mapTypeDict.ContainsKey(mapType))
                            {
                                mapsDict = new Dictionary<string, List<string>>();
                                localOfficeList = new List<string>();
                                localOfficeList.Add(localOfficeobj.ToString());
                                mapsDict.Add(mapNumberobj.ToString() + Connector + mapOfficeobj.ToString(), localOfficeList);
                                mapTypeDict.Add(mapType, mapsDict);
                            }
                            else
                            {
                                mapsDict = mapTypeDict[mapType];
                                if (mapsDict.ContainsKey(mapNumberobj.ToString() + Connector + mapOfficeobj.ToString()))
                                {
                                    localOfficeList = mapsDict[mapNumberobj.ToString() + Connector + mapOfficeobj.ToString()];
                                    localOfficeList.Add(localOfficeobj.ToString());
                                    mapsDict[mapNumberobj.ToString() + Connector + mapOfficeobj.ToString()] = localOfficeList;
                                    mapTypeDict[mapType] = mapsDict;
                                }
                                else
                                {
                                    localOfficeList = new List<string>();
                                    localOfficeList.Add(localOfficeobj.ToString());
                                    mapsDict.Add(mapNumberobj.ToString() + Connector + mapOfficeobj.ToString(), localOfficeList);
                                    mapTypeDict[mapType] = mapsDict;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(localoffice))
                    {
                        bool found = false;
                        //Check if there are more than one maptype found
                        //if only one found then return all the maps
                        if (mapTypeDict.Count == 1)
                        {
                            found = true;
                            retVal.AddRange(mapTypeDict.ElementAt(0).Value.Keys.ToList<string>());
                        }
                        //Loop thru' each and get the map numbers that has the lowest scale and the same local office
                        else
                        {
                            foreach (KeyValuePair<int, Dictionary<string, List<string>>> kp in mapTypeDict)
                            {
                                //If teh localoffice is not found in a lower scale print everything until you find a scale with LocalOffice.
                                //If the localoffice is not found then print everything.
                                foreach (KeyValuePair<string, List<string>> kp1 in kp.Value)
                                {
                                    if (kp1.Value.Contains(localoffice))
                                    {
                                        found = true;
                                        //Changed as per Robert's email. If a polygon is found with the same localoffice just plot that polygon.
                                        //retVal.Add(kp1.Key);
                                        retVal.AddRange(kp.Value.Keys);
                                    }

                                    if (found) break;
                                }
                                if (found) break;
                                //if (kp.Value.Values. Contains(localoffice))
                                //{
                                //    found = true;
                                //}
                                //retVal.AddRange(kp.Value.Values); 
                                //if (found) break;
                            }
                            if (!found)
                            {
                                localoffice = string.Empty;
                            }
                        }

                    }
                    if (string.IsNullOrEmpty(localoffice) && mapTypeDict.Count > 0)
                    {
                        int cnt = 0;
                        //Populate only the lowest and go ahead
                        foreach (Dictionary<string, List<string>> mapnumbers in mapTypeDict.Values)
                        {
                            if (cnt == 0)
                                retVal.AddRange(mapnumbers.Keys);
                            else
                                break;
                            cnt++;
                            //foreach (List<string> mapNumberMapOffice in mapnumbers.Values)
                            //{
                            //    retVal.AddRange(mapNumberMapOffice);
                            //}
                        }
                    }
                }
                finally
                {
                    if (featCursor != null)
                    {
                        while (Marshal.ReleaseComObject(featCursor) > 0) { }
                    }
                }
            }
            return retVal.Distinct().ToList();
        }
        /// <summary>
        /// Persists the change to the PGE_MODIFIEDMAP Tables.
        /// If hte map already exists for the version it is not persisted
        /// </summary>
        /// <param name="changes">Changed MapNumber:MapOffice data</param>
        /// <param name="obj">Feature that is edited</param>
        private void PersistChange(List<string> changes, IObject obj)
        {
            IWorkspace workspace = ((IDataset)obj.Class).Workspace;
            string versionName = string.Empty;
            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                versionName = ((IVersion)((IDataset)obj.Class).Workspace).VersionName;
            }
            if (changes.Count == 0)
            {
                _logger.Debug("No map changes found:" + versionName);
                return;
            }
            IRow rowToPersist = null;
            try
            {
                //rowToPersist = ChangeTable.CreateRow();
                rowToPersist = GetChangeDetectionRow(versionName);
                _logger.Debug(string.Format("Created a new row with OID:{0} on table :{1}", rowToPersist.OID, ((IDataset)_changeTable).Name));
                _logger.Debug("setting version Name");
                rowToPersist.set_Value(rowToPersist.Fields.FindField(_versionNameField), versionName);
                _logger.Debug("Setting field Value");
                int valueFldIx = rowToPersist.Fields.FindField(_fieldValueField);
                object fieldValue = rowToPersist.get_Value(valueFldIx);
                string existingValue = (fieldValue == null || fieldValue == System.DBNull.Value) ? string.Empty : fieldValue.ToString();
                List<string> mergedData = GetMergedData(existingValue, changes);
                if (mergedData.Count != 0)
                {
                    //if (existingValue != string.Empty) existingValue = existingValue + ",";
                    //rowToPersist.set_Value(valueFldIx, existingValue + string.Join(",", changes.ToArray()));
                    rowToPersist.set_Value(valueFldIx, string.Join(",", mergedData.ToArray()));
                }
                _logger.Debug("Setting date Value");
                rowToPersist.set_Value(rowToPersist.Fields.FindField(_modefiedDateField), DateTime.Today.ToShortDateString());
                rowToPersist.Store();
                //Clear the changes
                changes.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed persisting change to table with modelname: " + _tableModelName, ex);
            }
            finally
            {
                if (rowToPersist != null)
                {
                    while (Marshal.ReleaseComObject(rowToPersist) > 0) { }
                }
            }
        }
        /// <summary>
        /// Gets a ChangeDetection Row to be persisted to the PGE_MODIFIEDMAPS table.
        /// Queries for a record with versioname same as the one passed if a record is found that record is returned else a new Record is created and sent back.
        /// </summary>
        /// <param name="versionName">Name of the current edit version</param>
        /// <returns>IRow to be persisted to the PGE_MODIFIEDMAPS table</returns>
        private IRow GetChangeDetectionRow(string versionName)
        {
            ICursor cursor = null;
            IRow retVal = null;
            try
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = _versionNameField + "='" + versionName + "'";
                cursor = ((ITable)_changeTable).Search(qf, false);
                retVal = cursor.NextRow();
                if (retVal == null)
                {
                    retVal = ((ITable)_changeTable).CreateRow();
                }
                //release the cursor
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message, ex);
            }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                }
            }
            return retVal;
        }
        /// <summary>
        /// Gets string in the form MapNumber:MapOffice,MapNumber:MapOffice format and a list of string in MapNumber:MapOffice format and sends a unique value to be stored in the PGE_MODIFIEDMAPS table
        /// </summary>
        /// <param name="exisitngData">Data from the row that found</param>
        /// <param name="searchedData">Data obtained by performing a sptial search on the Distribution Map Polygon</param>
        /// <returns></returns>
        private List<string> GetMergedData(string exisitngData, List<string> searchedData)
        {
            List<string> retVal = new List<string>();
            if (!string.IsNullOrEmpty(exisitngData))
            {
                retVal = new List<string>(exisitngData.Split(",".ToCharArray()));
                foreach (string mo in searchedData)
                {
                    if (!retVal.Contains(mo))
                    {
                        retVal.Add(mo);
                    }
                }
            }
            else
            {
                return searchedData;
            }
            return retVal;
        }

        /// <summary>
        /// Get the Map Number of Map Polygons to be printed on the basis of its previous shape when a feature is moved.
        /// </summary>
        /// <param name="obj">Reference of feature moved</param>
        /// <param name="localOffice">LocalOfficeID value of the feature</param>
        /// <returns>Returns the list of the Map Numbers to be recorded for printing</returns>
        private List<string> GetOtherMapNumbers(IObject obj, string localOffice = "")
        {
            IFeatureChanges featureChanges = obj as IFeatureChanges;
            if (featureChanges.ShapeChanged && featureChanges.OriginalShape != null)
            {
                //Get the Map Numbers to be recorded from Old shape
                //return GetPersistValue(obj as IFeature, localOffice, featureChanges.OriginalShape);
                return GetPersistValue(obj as IFeature, featureChanges.OriginalShape);
            }
            else return new List<string>();
        }
        /// <summary>
        /// Gets a list of MapNumber:MapOffice combination for each polygon found.
        /// If any of the field value is null or empty will not add any data.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="queryingGeometry"></param>
        /// <returns></returns>
        private List<string> GetPersistValue(IFeature feature, IGeometry queryingGeometry = null)
        {
            List<string> retVal = new List<string>();

            //if shape and querying geometry are null, return retval; otherwise the spatial query returns all instersecting map polygons.
            if ((feature.ShapeCopy as IGeometry == null) && (queryingGeometry == null)) return retVal;

            if (!string.IsNullOrEmpty(_mapOfficeFieldName) && !string.IsNullOrEmpty(_mapTypeFieldName) && !string.IsNullOrEmpty(_localOfficeFieldName) && !string.IsNullOrEmpty(_distMapNumberFieldName))
            {
                ISpatialFilter spFilter = new SpatialFilterClass();
                spFilter.Geometry = (queryingGeometry != null ? queryingGeometry : feature.Shape as IGeometry);
                spFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor featCursor = _distMapFC.Search(spFilter, false);
                try
                {
                    IFeature mapFeature = null;
                    object mapOfficeobj = null;
                    object mapNumberobj = null;
                    while ((mapFeature = featCursor.NextFeature()) != null)
                    {
                        mapOfficeobj = mapFeature.GetFieldValue(_mapOfficeFieldName, true);
                        mapOfficeobj = (mapOfficeobj == null || mapOfficeobj == DBNull.Value) ? string.Empty : mapOfficeobj;
                        _logger.Debug("Mapoffice found:" + mapOfficeobj.ToString());
                        mapNumberobj = mapFeature.GetFieldValue(_distMapNumberFieldName, true);
                        mapNumberobj = (mapNumberobj == null || mapNumberobj == DBNull.Value) ? string.Empty : mapNumberobj;
                        _logger.Debug("Mapnumber found:" + mapNumberobj.ToString());
                        if(!string.IsNullOrEmpty(mapNumberobj.ToString()) && !string.IsNullOrEmpty(mapOfficeobj.ToString()))
                        {
                            retVal.Add(mapNumberobj.ToString()+":"+mapOfficeobj.ToString());
                        }
                    }
                }
                finally
                {
                    if (featCursor != null)
                    {
                        while (Marshal.ReleaseComObject(featCursor) > 0) { }
                    }
                }
            }
            return retVal.Distinct().ToList();
        }
        #endregion
    }
}
