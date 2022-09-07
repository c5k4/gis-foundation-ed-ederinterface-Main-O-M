using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using log4net;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Updates the Map Number of the Device when Distribution Map feature is updated
    /// </summary>
    [ComVisible(true)]
    [Guid("8767F359-7D5B-461F-BC14-52F0EFB45B61")]
    [ProgId("PGE.Desktop.EDER.UpdateDistributionMapNumberAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UpdateDistributionMapNumberAU : BaseSpecialAU
    {
        #region Private Variables

        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Error message to be displayed for a missing field
        /// </summary>
        private string _missingField = "Field not found with {0} model name in {1}";
        /// <summary>
        /// Map Number Field index in Distribution object class
        /// </summary>
        private int _mapNumberIx = -1;
        /// <summary>
        /// Map Type Field index in Distribution object class
        /// </summary>
        private int _mapTypeIx = -1;
        /// <summary>
        /// Local Office Field index in Distribution object class
        /// </summary>
        private int _localOfficeIx = -1;
        /// <summary>
        /// Handles the Step Progress Bar in current application
        /// </summary>
        private StepProgressBar _stepProgressBar = null;
        /// <summary>
        /// Default Map Number value of the Device feature
        /// </summary>
        private const string NOMAPVALUE = "NOMAP";
        /// <summary>
        /// Field index of Subtype field of Distribution Map feature class
        /// </summary>
        private string _distMapSubtypeFldName = null;
        /// <summary>
        /// Subtype Code of the applicable Subtype. it is Electric Plat with code =1
        /// </summary>
        private int _subtypeApplicable = 1;
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes new instance of <see cref="UpdateDistributionMapNumberAU"/> class
        /// </summary>
        public UpdateDistributionMapNumberAU()
            : base("PGE Update Distribution Map Number AU") { }

        #endregion Constructor

        #region Overridden BaseSpecialAU Methods

        /// <summary>
        /// Returns true if the EditEvent is either FeatureCreate / FeatureUpdate / FeatureDelete and the ObjectClass is assigned with required model name
        /// </summary>
        /// <param name="objectClass">Object Class to validate against for the availability of the Autoupdater</param>
        /// <param name="eEvent">AU Event mode to be validated against</param>
        /// <returns>Returns true if the EditEvent is either FeatureCreate / FeatureUpdate / FeatureDelete and the ObjectClass is assigned with required model name; false, otherwise</returns>
        protected override bool InternalEnabled(ESRI.ArcGIS.Geodatabase.IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            bool enabled = false;
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureDelete)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DistributionMap);
            }
            return enabled;
        }

        /// <summary>
        /// Returns true if AU is fired in ArcMap mode
        /// </summary>
        /// <param name="eAUMode">Autoupdater execution mode</param>
        /// <returns>Returns true if AU is fired in ArcMap mode; false, otherwise</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode == mmAutoUpdaterMode.mmAUMArcMap);
        }

        /// <summary>
        /// Executed when an AU fired and validates the MapNumber of Distribution Map
        /// </summary>
        /// <param name="obj">Distribution object</param>
        /// <param name="eAUMode">AU firing mode</param>
        /// <param name="eEvent">AU event mode</param>
        /// <remarks>
        /// Updates the Map number of underlying device features falling in the map features by considering the updated/new/deleted map feature
        /// </remarks>
        protected override void InternalExecute(ESRI.ArcGIS.Geodatabase.IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                //validate for the DistributionMap model names again
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DistributionMap) == false) return;

                /*Validate whether the Subtype of the Map is 'Electric Plat' and process only
                 * if the Map feature is 'Electric Plat'
                */
                if (obj.HasSubtypeCode(_subtypeApplicable) == false) return;

                //Get the subtype field name
                _distMapSubtypeFldName = (obj.Class as ISubtypes).SubtypeFieldName;

                //Get the field indexes and validate
                _mapNumberIx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.MapNumber);
                if (_mapNumberIx == -1)
                {
                    _logger.Debug(string.Format(_missingField, SchemaInfo.Electric.FieldModelNames.MapNumber, obj.Class.AliasName)); return;
                }

                _mapTypeIx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.MapType);
                if (_mapTypeIx == -1)
                {
                    _logger.Debug(string.Format(_missingField, SchemaInfo.Electric.FieldModelNames.MapType, obj.Class.AliasName)); return;
                }

                _localOfficeIx = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                if (_localOfficeIx == -1)
                {
                    _logger.Debug(string.Format(_missingField, SchemaInfo.Electric.FieldModelNames.LocalOfficeID, obj.Class.AliasName)); return;
                }

                

                //Get  MapNumber value
                string mapNumber = Convert.ToString(obj.get_Value(_mapNumberIx));

                //initialize the statusbar
                _stepProgressBar = new StepProgressBar(base.Application);
                _stepProgressBar.Show();

                //If the event is update then check whether a particular set of fields are updated
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    UpdateDevicesOnMapUpdateEvent(obj as IFeature);
                }
                else if (eEvent == mmEditEvent.mmEventFeatureCreate)
                {
                    UpdateDevicesOnMapCreateEvent(obj as IFeature);
                }
                else if (eEvent == mmEditEvent.mmEventFeatureDelete)
                {
                    UpdateDevicesOnMapDeleteEvent(obj as IFeature, mapNumber);
                }

                //Display the completion status
                _stepProgressBar.Reset(0, 0, 0, "Map Number updation completed.");
            }
            finally
            {
                if (_stepProgressBar != null) { _stepProgressBar.Hide(); }
            }

        }

        #endregion Overridden BaseSpecialAU Methods

        #region Private Methods

        /// <summary>
        /// Updates the Map Number of devices underlying the updated map feature
        /// </summary>
        /// <param name="mapFeature">Reference of the updated map feature</param>
        private void UpdateDevicesOnMapUpdateEvent(IFeature mapFeature)
        {
            //Validate if any of the fields in question are updated
            IRowChanges changes = mapFeature as IRowChanges;
            bool mapNumberChanged = changes.get_ValueChanged(_mapNumberIx);
            bool mapTypeChanged = changes.get_ValueChanged(_mapTypeIx);
            bool localOfficeChanged = changes.get_ValueChanged(_localOfficeIx);
            bool shapeChanged = GeometryFacade.ShapeChanged(mapFeature);

            //Exit if none of the required fields are updated
            if (shapeChanged || mapNumberChanged || mapTypeChanged || localOfficeChanged) { }
            else return;

            //Validate whether only MapNumber is changed
            bool onlyMapNumberChanged = false;
            if (mapNumberChanged)
            {
                if (mapTypeChanged || localOfficeChanged || shapeChanged) onlyMapNumberChanged = false;
                else onlyMapNumberChanged = true;
            }

            //Validate whether only Shape is changed
            bool onlyShapeChanged = false;
            if (shapeChanged)
            {
                if (mapTypeChanged || localOfficeChanged || onlyMapNumberChanged) onlyShapeChanged = false;
                else onlyShapeChanged = true;
            }

            if (onlyMapNumberChanged == true)
            {
                //Get the old and new map numbers
                string oldMapNumber = Convert.ToString(changes.get_OriginalValue(_mapNumberIx));
                string mapNumber = Convert.ToString(mapFeature.get_Value(_mapNumberIx));

                //If OldMapNumber is empty then calculate the MapNumber of devices from scratch
                if (string.IsNullOrEmpty(oldMapNumber))
                {
                    //Need to process each feature by considering Geometry, MapType, LocalOffice
                    ProcessDevicesInArea(mapFeature, string.Empty, mapFeature.ShapeCopy, null, EditType.Other);
                }
                else
                {
                    //Update the Devices having old map number with new map number
                    UpdateMapNoOfDevices(mapFeature, oldMapNumber, mapNumber, null);
                }
            }
            else
            {

                IGeometry affectedMapArea = null;
                IGeometry overlappingRegion = null;
                IGeometry nonOverlappingRegion = null;

                if (onlyShapeChanged)
                {
                    //Get the affected geometry because of the updated map feature
                    affectedMapArea = GetAffectedMapArea(mapFeature);
                    //Get the overlapping region from other map features
                    overlappingRegion = GetOverlappingRegion(mapFeature, affectedMapArea, string.Empty);
                    //Get the non-overlapping region of the affected map feature
                    nonOverlappingRegion = GetNonOverlappingRegion(affectedMapArea, overlappingRegion);
                }
                else
                {
                    if (shapeChanged)
                    {
                        //Get the effected geometry as union of Old and new shape as the entire area need to processed
                        //do to change in MapType/LocalOffice/MapNumber along with Shape
                        IGeometry originalShape = (mapFeature as IFeatureChanges).OriginalShape;
                        affectedMapArea = (mapFeature.ShapeCopy as ITopologicalOperator).Union(originalShape);
                    }
                    else affectedMapArea = mapFeature.ShapeCopy;//Get the map shape as affected area

                    //Get the non-overlapping region of the affected map feature
                    overlappingRegion = GetOverlappingRegion(mapFeature, affectedMapArea, string.Empty);
                    //Get the overlapping region from other map features
                    nonOverlappingRegion = GetNonOverlappingRegion(affectedMapArea, overlappingRegion);
                    //Get the common-area of the Map Feature before and after shape changed
                    IGeometry intersection = IntersectionGeometry(mapFeature);
                    if (intersection != null)
                    {
                        if (overlappingRegion == null || overlappingRegion.IsEmpty) overlappingRegion = intersection;
                        else
                        {
                            overlappingRegion = (overlappingRegion as ITopologicalOperator).Union(intersection);
                        }
                    }
                }

                if (!shapeChanged && !mapNumberChanged) nonOverlappingRegion = null;

                //Determine edit type of the map feature and process the non-overlapping and overlapping area
                //for the map number updation
                EditType editType = (shapeChanged ? EditType.ShapeUpdated : EditType.Other);
                ProcessDevicesInArea(mapFeature, string.Empty, overlappingRegion, nonOverlappingRegion, editType);

            }
        }

        /// <summary>
        /// Updates the Map Number of devices underlying the created map feature
        /// </summary>
        /// <param name="mapFeature">Reference of the created map feature</param>
        private void UpdateDevicesOnMapCreateEvent(IFeature mapFeature)
        {
            try
            {
                //Get the overlapping area from existing map features
                IGeometry overlappingGeometry = GetOverlappingRegion(mapFeature, mapFeature.ShapeCopy, string.Empty);
                //Get the non-overlapping area of new map feature
                IGeometry nonOverlappingGeometry = GetNonOverlappingRegion(mapFeature.Shape, overlappingGeometry);
                //Process the non-overlapping and overlapping area for the map number updation
                ProcessDevicesInArea(mapFeature, string.Empty, overlappingGeometry, nonOverlappingGeometry, EditType.Created);
            }
            catch (Exception ex)
            {
                _logger.Error("Error" + ex);
            }
        }

        /// <summary>
        /// Updates the Map Number of devices underlying the deleted map feature
        /// </summary>
        /// <param name="mapFeature">Reference of the created map feature</param>
        /// <param name="oldMapNumber">Map Number of the map feature being deleted</param>
        /// <remarks>
        /// If any abandoned feature is found by allowing the current delete operation, the edit task is cancelled.
        /// </remarks>
        private void UpdateDevicesOnMapDeleteEvent(IFeature mapFeature, string oldMapNumber)
        {
            try
            {
                //Get the overlapping area from existing map features
                IGeometry overlappingGeometry = GetOverlappingRegion(mapFeature, mapFeature.ShapeCopy, string.Empty);
                //Get the non-overlapping area of deleted map feature
                IGeometry nonOverlappingGeometry = GetNonOverlappingRegion(mapFeature.Shape, overlappingGeometry);
                //Process the non-overlapping and overlapping area for the map number updation
                ProcessDevicesInArea(mapFeature, oldMapNumber, overlappingGeometry, nonOverlappingGeometry, EditType.Deleted);
            }
            catch (COMException comEx)
            {
                if (comEx.ErrorCode == (int)mmErrorCodes.MM_E_CANCELEDIT) throw;
                else _logger.Error("Error" + comEx);
            }
            catch (Exception ex)
            {
                _logger.Error("Error" + ex);
            }
        }

        /// <summary>
        /// Updates the devices having mapNumber=<paramref name="oldMapNumber"/> with mapNumber=<paramref name="newMapNumber"/> 
        /// </summary>
        /// <param name="mapFeature">Reference to the map feature</param>
        /// <param name="oldMapNumber">Old Map Number of map feature</param>
        /// <param name="newMapNumber">New Map Number of map feature</param>
        /// <param name="affectedArea">Affected area because of map feature updation</param>
        /// <remarks>
        /// Retrieves the device features having mapNumber=<paramref name="oldMapNumber"/> and falls in <paramref name="affectedArea"/> area are replaced with new map number
        /// </remarks>
        private void UpdateMapNoOfDevices(IFeature mapFeature, string oldMapNumber, string newMapNumber, IGeometry affectedArea)
        {
            try
            {
                //Validate map feature
                if (mapFeature == null) return;

                //Get the workspace
                IWorkspace wSpace = (mapFeature.Class as IDataset).Workspace;
                //If affectedArea=null then get the shape of the feature
                if (affectedArea == null) { affectedArea = mapFeature.ShapeCopy; }

                //Get the device classes with DistUpdate model name
                IEnumFeatureClass featureClassColl = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(wSpace, SchemaInfo.Electric.ClassModelNames.DistMapUpdate);
                featureClassColl.Reset();

                IFeatureClass deviceClass = null;
                int deviceMapNoIx = -1;

                //Loop through each device class to update the mapNumber of its features
                while ((deviceClass = featureClassColl.Next()) != null)
                {
                    //Get the Map Number field of Device
                    deviceMapNoIx = ModelNameFacade.FieldIndexFromModelName(deviceClass, SchemaInfo.Electric.FieldModelNames.ElecMapNumber);
                    if (deviceMapNoIx == -1)
                    {
                        _logger.Debug(string.Format(_missingField, SchemaInfo.Electric.FieldModelNames.ElecMapNumber, deviceClass.AliasName));
                        continue;
                    }

                    //Prepare where clause to get the features with old Map No
                    string whereClause = string.Empty;
                    if (!string.IsNullOrEmpty(oldMapNumber))
                    {
                        whereClause = deviceClass.Fields.get_Field(deviceMapNoIx).Name + "='" + oldMapNumber + "'";
                    }
                    //Get the feature count to be updated
                    int featureCount = GetFeatureCount(affectedArea, whereClause, deviceClass);

                    if (featureCount > 0)
                    {
                        //Reset progress bar credentials                    
                        _stepProgressBar.Reset(0, featureCount, 1, "Updating Map Number of " + deviceClass.AliasName + " features...");

                        //Get the Device features having the MapNumber as OldMapNumber
                        IFeatureCursor deviceCursor = GetCursor(affectedArea, esriSpatialRelEnum.esriSpatialRelContains, whereClause, deviceClass, true);

                        IFeature deviceFeature = null;
                        //Update each device with new Map no
                        while ((deviceFeature = deviceCursor.NextFeature()) != null)
                        {
                            //Replace oldMapNo with newMapNo
                            deviceFeature.set_Value(deviceMapNoIx, newMapNumber);
                            deviceCursor.UpdateFeature(deviceFeature);
                            //Increment the step of progress bar
                            _stepProgressBar.Step();
                        }

                        //Flush and release the cursor
                        deviceCursor.Flush();
                        if (deviceCursor != null) { Marshal.ReleaseComObject(deviceCursor); }
                    }
                }

                // Release the object
                if (featureClassColl != null) { Marshal.ReleaseComObject(featureClassColl); }
            }
            catch (Exception ex)
            {
                _logger.Error("Error" + ex);
            }
        }

        /// <summary>
        /// Prepares the cursor on a Feature Class using the filter
        /// </summary>
        /// <param name="featureGeometry">Shape of the Filtering geometry</param>
        /// <param name="spatialRelation">Spatial relation to be checked</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="deviceClass">Class to execute the filter</param>
        /// <param name="updateCursor">Whether to return Update cursor or Search Cursor</param>
        /// <returns>Returns the cursor prepared using the given credentials</returns>
        private IFeatureCursor GetCursor(IGeometry featureGeometry, esriSpatialRelEnum spatialRelation, string whereClause, IFeatureClass deviceClass, bool updateCursor)
        {
            IFeatureCursor cursor = null;

            //Prepare spatial filter with the given credentials
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = featureGeometry;
            spatialFilter.GeometryField = deviceClass.ShapeFieldName;
            spatialFilter.SpatialRel = spatialRelation;
            spatialFilter.WhereClause = whereClause;

            //Prepares either Update/Search cursor
            if (updateCursor)
            {
                cursor = deviceClass.Update(spatialFilter, false);
            }
            else
            {
                cursor = deviceClass.Search(spatialFilter, true);
            }
            //Returns the cursor
            return cursor;
        }

        /// <summary>
        /// Returns the number of features in a class satisfying the given filter
        /// </summary>
        /// <param name="featureGeometry">Querying Geometry to look for features falling inside</param>
        /// <param name="whereClause">Where clause</param>
        /// <param name="deviceClass">Class to fire filter</param>
        /// <returns>Returns the number of features in a class satisfying the given filter</returns>
        private int GetFeatureCount(IGeometry featureGeometry, string whereClause, IFeatureClass deviceClass)
        {
            int featureCount = 0;

            //Prepare spatial filter with the given credentials
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = featureGeometry;
            spatialFilter.GeometryField = deviceClass.ShapeFieldName;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            spatialFilter.WhereClause = whereClause;
            //Get the effected feature count
            featureCount = deviceClass.FeatureCount(spatialFilter);

            return featureCount;
        }

        /// <summary>
        /// Gets the affected area of the map because of the shape updation of the feature
        /// </summary>
        /// <param name="updatedMapFeature">Reference of the updated map feature</param>
        /// <returns>Returns the affected area</returns>
        /// <remarks>
        /// If a feature is moved from one location to another, this method returns SymmetricDifference of Old Shape and new Shape i.e. the area that is part of old shape but not part of new shape and also vice-versa.
        /// Basically, ignores the common area of old and new shape and returns the other affected which needs to be condiered. This is 
        /// </remarks>
        private IGeometry GetAffectedMapArea(IFeature updatedMapFeature)
        {
            IGeometry affectedArea = null;
            if (updatedMapFeature == null) return affectedArea;

            //Get the old and new geometries
            IGeometry oldGeometry = (updatedMapFeature as IFeatureChanges).OriginalShape;
            IGeometry newGeometry = updatedMapFeature.ShapeCopy;

            //Get the affected area of the Map because of change in shape
            ITopologicalOperator topoOperator = newGeometry as ITopologicalOperator;
            affectedArea = topoOperator.SymmetricDifference(oldGeometry);

            //Returns the affected area
            return affectedArea;
        }

        /// <summary>
        /// Updates the MapNumber of each device feature either by calculating from Scracth or populating with new map number or erasing the existing map number
        /// </summary>
        /// <param name="mapFeature">Reference of the map feature</param>
        /// <param name="oldMapNumber">Old Map Number of the map feature</param>
        /// <param name="affectedArea">Effected area (overlapping area) of map feature. Map number is calculated for all features falling in this area</param>
        /// <param name="mapNumberUpdatedArea">Non-overlapping area of the map features.Either the Map Number is populated or the old Map Number is erased for the device features falling in this area</param>
        /// <param name="editType">Indicates the type of edit operation</param>
        private void ProcessDevicesInArea(IFeature mapFeature, string oldMapNumber, IGeometry affectedArea, IGeometry mapNumberUpdatedArea, EditType editType)
        {
            //Get the workspace
            IWorkspace wSpace = (mapFeature.Class as IDataset).Workspace;

            string whereClause = string.Empty;

            //Validates the regions/areas to be processed
            bool shouldProcessMapNumberUpdatedArea = (mapNumberUpdatedArea != null && !mapNumberUpdatedArea.IsEmpty);
            bool shouldProcessFromScratch = (affectedArea != null && !affectedArea.IsEmpty);
            //Checks whether map no changed
            string mapNumber = Convert.ToString(mapFeature.get_Value(_mapNumberIx));


            //Create an object to calculate the Map Number of device from scratch,
            //and pass deleted feature OID if the operation is deleted.
            PopulateDistMapNo populateDistMapNoObj = new PopulateDistMapNo();
            if (editType == EditType.Deleted) populateDistMapNoObj.DeletedMapFeatureOID = mapFeature.OID;

            //Get the classes with DistUpdate model name
            IEnumFeatureClass featureClassColl = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(wSpace, SchemaInfo.Electric.ClassModelNames.DistMapUpdate);
            featureClassColl.Reset();

            IFeatureClass deviceClass = null;
            IFeatureCursor cursor = null;
            IFeature deviceFeature = null;
            //New Overlapping Region means the area where the device features are populated with new mapp number
            IGeometry newNonOverlappingRegion = null;
            //New Overlapping Region means the area where existing map number is erased from device features
            IGeometry oldNonOverlappingRegion = null;

            //Loop through each device and process
            while ((deviceClass = featureClassColl.Next()) != null)
            {
                //Get the MapNumber filed index of Device
                int deviceMapNoIx = ModelNameFacade.FieldIndexFromModelName(deviceClass, SchemaInfo.Electric.FieldModelNames.ElecMapNumber);
                if (deviceMapNoIx == -1)
                {
                    _logger.Debug(string.Format(_missingField, SchemaInfo.Electric.FieldModelNames.ElecMapNumber, deviceClass.AliasName));
                    continue;
                }


                //Prepare where clause to update only features with OldMapNumber if provided
                if (oldMapNumber != string.Empty)
                {
                    whereClause = deviceClass.Fields.get_Field(deviceMapNoIx).Name + "='" + oldMapNumber + "'";
                }
                else whereClause = string.Empty;

                //Get the effected feature count
                int featureCount = 0;

                if (shouldProcessFromScratch)
                {
                    //Get the feature count that need to be processed from scratch
                    featureCount = GetFeatureCount(affectedArea, whereClause, deviceClass);
                }

                //Get the newNonOverlappingRegion  and oldNonOverlappingRegion based on the Edit operation type
                if (shouldProcessMapNumberUpdatedArea)
                {
                    switch (editType)
                    {
                        case EditType.ShapeUpdated:
                            newNonOverlappingRegion = GetNewNonOverlappingARea(mapFeature, mapNumberUpdatedArea);//Updated with new map number
                            oldNonOverlappingRegion = GetOldNonOverlappingArea(mapFeature, mapNumberUpdatedArea);//erase the existing map number
                            break;
                        case EditType.Created:
                            newNonOverlappingRegion = mapNumberUpdatedArea;//Updated with new map number
                            oldNonOverlappingRegion = null;//Nothing to erase
                            break;
                        case EditType.Deleted:
                            newNonOverlappingRegion = null;//Nothing to update
                            oldNonOverlappingRegion = mapNumberUpdatedArea;//erase the existing map number
                            break;
                    }

                    //Get the feature count in New Non-Overlapping region
                    if (newNonOverlappingRegion != null && !newNonOverlappingRegion.IsEmpty)
                    {
                        featureCount += GetFeatureCount(newNonOverlappingRegion, whereClause, deviceClass);
                    }
                    //Get the feature count in New Non-Overlapping region
                    if (oldNonOverlappingRegion != null && !oldNonOverlappingRegion.IsEmpty)
                    {
                        //Abort the edit operation if abandoned features exist and the operation is delete
                        int devicesInOldNORegion = GetFeatureCount(oldNonOverlappingRegion, whereClause, deviceClass);
                        if (devicesInOldNORegion > 0 && editType == EditType.Deleted)
                        {
                            string errorMessage = "Orphaned device features found. Hence the Distribution Map Polygon can not be deleted.";
                            throw new COMException(errorMessage, (int)mmErrorCodes.MM_E_CANCELEDIT);
                        }
                        else featureCount += GetFeatureCount(oldNonOverlappingRegion, whereClause, deviceClass);
                    }
                }

                //Process only if the total effected feature count >0
                if (featureCount > 0)
                {
                    //Reset progress bar credentials                    
                    _stepProgressBar.Reset(0, featureCount, 1, "Updating Map Number of " + deviceClass.AliasName + " features...");

                    //Check if device feature to be processed form scratch i.e. in the overlapping region
                    if (shouldProcessFromScratch)
                    {
                        //Pass the device field index to 'populateDistMapNoObj' class
                        populateDistMapNoObj.MapNoDeviceFieldIndex = deviceMapNoIx;
                        populateDistMapNoObj.LocalOfficeDeviceFieldIndex = ModelNameFacade.FieldIndexFromModelName(deviceClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                        if (populateDistMapNoObj.LocalOfficeDeviceFieldIndex != -1)
                        {
                            populateDistMapNoObj.LocalOfficeDeviceField = deviceClass.Fields.get_Field(populateDistMapNoObj.LocalOfficeDeviceFieldIndex);
                        }
                        else populateDistMapNoObj.LocalOfficeDeviceField = null;
                        populateDistMapNoObj.MapOfficeDeviceFieldIndex = ModelNameFacade.FieldIndexFromModelName(deviceClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);

                        //Get the cursor with the given where clause and fall under the given geometry
                        cursor = GetCursor(affectedArea, esriSpatialRelEnum.esriSpatialRelContains, whereClause, deviceClass, true);
                        while ((deviceFeature = cursor.NextFeature()) != null)
                        {
                            //Calculate & Update the Map number
                            populateDistMapNoObj.ExecuteUpdateMapNoFromDistMap(deviceFeature);
                            cursor.UpdateFeature(deviceFeature);
                            //Step the progress bar by one position
                            _stepProgressBar.Step();
                        }

                        //Flush and release the cursor
                        cursor.Flush();
                        Marshal.ReleaseComObject(cursor);
                    }

                    //Check if device feature to be in the non-overlapping region
                    if (shouldProcessMapNumberUpdatedArea)
                    {
                        //Process the features the need to be updated with new map number
                        if (newNonOverlappingRegion != null && !newNonOverlappingRegion.IsEmpty)
                        {
                            //Get the cursor with the given where clause and fall under the given geometry
                            cursor = GetCursor(newNonOverlappingRegion, esriSpatialRelEnum.esriSpatialRelContains, whereClause, deviceClass, true);
                            while ((deviceFeature = cursor.NextFeature()) != null)
                            {
                                //Calculate & Update the Map number
                                deviceFeature.set_Value(deviceMapNoIx, mapNumber);
                                cursor.UpdateFeature(deviceFeature);
                                //Step the progress bar by one position
                                _stepProgressBar.Step();
                            }

                            //Flush and release the cursor
                            cursor.Flush();
                            Marshal.ReleaseComObject(cursor);
                        }

                        //Process the features whose existing map number is to erased
                        if (oldNonOverlappingRegion != null && !oldNonOverlappingRegion.IsEmpty)
                        {
                            //Get the cursor with the given where clause and fall under the given geometry
                            cursor = GetCursor(oldNonOverlappingRegion, esriSpatialRelEnum.esriSpatialRelContains, whereClause, deviceClass, true);
                            while ((deviceFeature = cursor.NextFeature()) != null)
                            {
                                //Calculate & Update the Map number
                                deviceFeature.set_Value(deviceMapNoIx, NOMAPVALUE);
                                cursor.UpdateFeature(deviceFeature);
                                //Step the progress bar by one position
                                _stepProgressBar.Step();
                            }

                            //Flush and release the cursor
                            cursor.Flush();
                            Marshal.ReleaseComObject(cursor);
                        }
                    }
                }
            }

            // Release the object
            if (featureClassColl != null) { Marshal.ReleaseComObject(featureClassColl); }
        }

        /// <summary>
        /// Returns the overlapping region of <paramref name="featureGeometry"/> with existing map features other than <paramref name="mapFeature"/>
        /// </summary>
        /// <param name="mapFeature">Map Feature to be excluded</param>
        /// <param name="featureGeometry">Affected area</param>
        /// <param name="whereClause">Where clause of the map features to get the overlapping area</param>
        /// <returns>Returns the overlapping region of <paramref name="featureGeometry"/> with existing map features other than <paramref name="mapFeature"/></returns>
        private IGeometry GetOverlappingRegion(IFeature mapFeature, IGeometry featureGeometry, string whereClause)
        {
            IGeometry overlappingRegion = null;

            if (featureGeometry == null) { featureGeometry = mapFeature.ShapeCopy; }

            //Restrict the current feature also
            if (string.IsNullOrEmpty(whereClause)) whereClause = mapFeature.Class.OIDFieldName + "<>" + mapFeature.OID.ToString();
            else whereClause += " AND " + mapFeature.Class.OIDFieldName + "<>" + mapFeature.OID.ToString();

            whereClause = _distMapSubtypeFldName + " = " + _subtypeApplicable + " AND " + whereClause;

            IFeatureCursor cursor = GetCursor(featureGeometry, esriSpatialRelEnum.esriSpatialRelIntersects, whereClause, mapFeature.Class as IFeatureClass, false);
            //Get the Union of all the geometries

            IFeature mapFeatureResult = null;
            IGeometryCollection geometryColl = new GeometryBagClass();
            while ((mapFeatureResult = cursor.NextFeature()) != null)
            {
                geometryColl.AddGeometry(mapFeatureResult.ShapeCopy);
            }
            IGeometry unionGeometry = new PolygonClass();
            if (geometryColl.GeometryCount > 0)
            {
                ITopologicalOperator topoOp = unionGeometry as ITopologicalOperator;
                topoOp.ConstructUnion(geometryColl as IEnumGeometry);
                if (unionGeometry.IsEmpty) return overlappingRegion;
                overlappingRegion = topoOp.Intersect(featureGeometry, esriGeometryDimension.esriGeometry2Dimension);
            }

            return overlappingRegion;
        }

        /// <summary>
        /// Returns the Non-Overlapping region given the affected area and overlapping region
        /// </summary>
        /// <param name="mapAffectedGeometry">Affected area</param>
        /// <param name="overlappingRegion">Overlapping region</param>
        /// <returns>Non-Overlappingregion = mapAffectedGeometry.Difference(overlappingRegion)</returns>
        private IGeometry GetNonOverlappingRegion(IGeometry mapAffectedGeometry, IGeometry overlappingRegion)
        {
            IGeometry nonOverlappingRegion = null;

            if (overlappingRegion == null || overlappingRegion.IsEmpty)
            {
                nonOverlappingRegion = mapAffectedGeometry;
            }
            else
            {
                ITopologicalOperator topoOp = mapAffectedGeometry as ITopologicalOperator;
                nonOverlappingRegion = topoOp.Difference(overlappingRegion);
            }
            return nonOverlappingRegion;
        }

        /// <summary>
        /// Returns the New-Non-Overlapping-Region for given Overlapping Region and updated Map feature
        /// </summary>
        /// <param name="mapFeature">Reference to the updated Map feature</param>
        /// <param name="nonOverlappingRegion">Non-Overlapping region of the map feature</param>
        /// <returns>New-Non-Overlapping-Region=nonOverlappingRegion.Difference(New shape of mapFeature)</returns>
        private IGeometry GetNewNonOverlappingARea(IFeature mapFeature, IGeometry nonOverlappingRegion)
        {
            IGeometry oldGeometry = (mapFeature as IFeatureChanges).OriginalShape;
            IGeometry oldNonOverlappingRegion = (nonOverlappingRegion as ITopologicalOperator).Difference(oldGeometry);
            return oldNonOverlappingRegion;
        }

        /// <summary>
        /// Returns the Old-Non-Overlapping-Region for given Overlapping Region and updated Map feature
        /// </summary>
        /// <param name="mapFeature">Reference to the updated Map feature</param>
        /// <param name="nonOverlappingRegion">Non-Overlapping region of the map feature</param>
        /// <returns>Old-Non-Overlapping-Region=nonOverlappingRegion.Difference(Old shape of mapFeature)</returns>
        private IGeometry GetOldNonOverlappingArea(IFeature mapFeature, IGeometry nonOverlappingRegion)
        {
            IGeometry newNonOverlappingRegion = (nonOverlappingRegion as ITopologicalOperator).Difference(mapFeature.ShapeCopy);
            return newNonOverlappingRegion;
        }

        /// <summary>
        /// Common Area of the Old and New Shape of the map feature
        /// </summary>
        /// <param name="mapFeature">Reference to the updated map feature</param>
        /// <returns>Returns the intersection geometry/area of the shape updated map feature </returns>
        private IGeometry IntersectionGeometry(IFeature mapFeature)
        {
            if (!GeometryFacade.ShapeChanged(mapFeature)) return null;
            IGeometry oldGeometry = (mapFeature as IFeatureChanges).OriginalShape;
            return (mapFeature.ShapeCopy as ITopologicalOperator).Intersect(oldGeometry, esriGeometryDimension.esriGeometry2Dimension);
        }

        #endregion Private Methods
    }

    #region Enum

    /// <summary>
    /// Holds the different types of edit operations carried on a feature
    /// </summary>
    internal enum EditType
    {
        //Indicates Shape updation
        ShapeUpdated,
        //Indicates Feature Deletion
        Deleted,
        //Indicates Feature Creation
        Created,
        //Any other edit
        Other
    }

    #endregion Enum
}
