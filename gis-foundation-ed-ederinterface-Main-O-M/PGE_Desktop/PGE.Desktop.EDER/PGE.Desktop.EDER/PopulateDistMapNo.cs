using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.EDER.AutoUpdaters.Special;

namespace PGE.Desktop.EDER
{
    //Helper Class for Distribution Map No Autoupdater.
    internal class PopulateDistMapNo
    {
        #region Private Variables
        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Default map number value
        /// </summary>
        public const string NOMAP_MAPNUMBER = "NOMAP";
        /// <summary>
        /// Value to populate when duplicate map numbers are found
        /// </summary>
        public const string DUPMAP_MAPNUMBER = "DUPMAP";
        /// <summary>
        /// Value to populate when wrong Local Office is found
        /// </summary>
        public const string WRONGLO_MAPNUMBER = "WRONG LO";
        /// <summary>
        /// Reference to the MaintenancePlat feature class
        /// </summary>
        private IFeatureClass _pDistMapClass = null;
        /// <summary>
        /// Map Number field index in Maintenance Plat Class
        /// </summary>
        private int _mapNoFieldIdx = 0;
        /// <summary>
        /// Map Type field index
        /// </summary>
        private int _mapTypeFieldIdx = 0;

        /// <summary>
        /// Map Number field index of device feature class
        /// </summary>
        private int _mapNoElecFieldIdx = 0;
        /// <summary>
        /// Local Office Field index of the Device feature class
        /// </summary>
        private int _localOfficeDeviceFieldIdx = 0;
        /// <summary>
        /// Reference to Local Office Field of the Device feature class
        /// </summary>
        private IField _localOfficeDeviceField = null;
        /// <summary>
        /// Indicates whether final polygon is found from which the MapNumber in to be invoked.
        /// </summary>
        private bool _isPolygonExist = false;
        /// <summary>
        /// List of Map features containing the device feature
        /// </summary>
        private List<IFeature> _lstSelectedFeaOnMap = new List<IFeature>();
        /// <summary>
        /// Local Office Field index of the Device feature class
        /// </summary>
        private IField _localOfficePolyField = null;
        /// <summary>
        /// Local Office Field index of the MapPolygon feature class
        /// </summary>
        private int _localOfficePolyFieldIdx = 0;
        /// <summary>
        /// Field index of Subtype field of Distribution Map feature class
        /// </summary>
        private string _distMapSubtypeFldName = null;
        /// <summary>
        /// Subtype Code of the applicable Subtype. it is Electric Plat with code =1
        /// </summary>
        private int _subtypeApplicable = 1;
        /// <summary>
        /// Map Office Field index of the Device feature class
        /// </summary>
        private int _mapOfficeDeviceFieldIdx = -1;
        /// <summary>
        /// Map Office Field index of the MapPolygon feature class
        /// </summary>
        private int _mapOfficeFldIx = -1;


        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes the new instance of <see cref="PopulateDistMapNo"/>
        /// </summary>
        public PopulateDistMapNo()
        {
            DeletedMapFeatureOID = -1;
        }

        #endregion

        #region Property

        /// <summary>
        /// Set or Get DistributionMapClass
        /// </summary>
        public IFeatureClass DistributionMapClass
        {
            set { _pDistMapClass = value; }
            get { return _pDistMapClass; }
        }

        /// <summary>
        /// Set or Get MapNo Field Index of Distribution Map Polygon
        /// </summary>
        public int MapNoFieldIndex
        {
            set { _mapNoFieldIdx = value; }
            get { return _mapNoFieldIdx; }
        }

        /// <summary>
        /// Set or Get MapType Field Index of Distribution Map Polygon
        /// </summary>
        public int MapTypeFieldIndex
        {
            set { _mapTypeFieldIdx = value; }
            get { return _mapTypeFieldIdx; }
        }

        /// <summary>
        /// Set or Get MapNo Field Index of Device Object
        /// </summary>
        public int MapNoDeviceFieldIndex
        {
            set { _mapNoElecFieldIdx = value; }
            get { return _mapNoElecFieldIdx; }
        }

        /// <summary>
        /// Set or Get Map Office Field Index of Device Object
        /// </summary>
        public int MapOfficeDeviceFieldIndex
        {
            set { _mapOfficeDeviceFieldIdx = value; }
            get { return _mapOfficeDeviceFieldIdx; }
        }
        /// <summary>
        /// Set or Get LocalOffice Field Index of Device Object
        /// </summary>
        public int LocalOfficeDeviceFieldIndex
        {
            set { _localOfficeDeviceFieldIdx = value; }
            get { return _localOfficeDeviceFieldIdx; }
        }

        /// <summary>
        /// The 'local office device' field definition.
        /// </summary>
        public IField LocalOfficeDeviceField
        {
            set { _localOfficeDeviceField = value; }
            get { return _localOfficeDeviceField; }
        }

        public int DeletedMapFeatureOID { get; set; }

        #endregion Property

        #region Public Methods
        /// <summary>
        /// Execute populate Distribution MapNo to object Created or Updated
        /// </summary>
        /// <param name="pObject">The object to populate Distribution MapNo</param>
        public void ExecuteUpdateMapNoFromDistMap(IObject pObject)
        {
            IDataset pDset = pObject.Class as IDataset;
            IWorkspace pWorkspace = pDset.Workspace;
            
            //Checks DistributionMap ObjectClass is assigned with ModelName
            _pDistMapClass = ModelNameFacade.FeatureClassByModelName(pWorkspace, SchemaInfo.Electric.ClassModelNames.DistributionMap);
            if (_pDistMapClass == null)
            {
                _logger.Debug(SchemaInfo.Electric.ClassModelNames.DistributionMap + " ModelName is not assigned to DistributionMap objectclass, exiting.");
                return;
            }

            //Get the Subtype field name
            _distMapSubtypeFldName = (_pDistMapClass as ISubtypes).SubtypeFieldName;

            _mapTypeFieldIdx = ModelNameFacade.FieldIndexFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.MapType);
            if (_mapTypeFieldIdx == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.MapType + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            //Checks DistributionMap MapType field is assigned with ModelName
            IField pField = ModelNameFacade.FieldFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.MapType);
            if (pField == null)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.MapType + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            _localOfficePolyFieldIdx = ModelNameFacade.FieldIndexFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
            if (_mapTypeFieldIdx == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.LocalOfficeID + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            //Checks DistributionMap LocalOffice field is assigned with ModelName
            _localOfficePolyField = ModelNameFacade.FieldFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
            if (pField == null)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.LocalOfficeID + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            //checks DistributionMap object class field assigned with ModelName.
            _mapNoFieldIdx = ModelNameFacade.FieldIndexFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.MapNumber);
            if (_mapNoFieldIdx == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.MapNumber + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            //checks DistributionMap object class field assigned with ModelName.
            _mapOfficeFldIx = ModelNameFacade.FieldIndexFromModelName(_pDistMapClass as IObjectClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);
            if (_mapOfficeFldIx == -1)
            {
                _logger.Debug(SchemaInfo.General.FieldModelNames.MapOfficeMN + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return;
            }

            IFeature deviceObject = pObject as IFeature;
            string localOfficeValue = pObject.get_Value(_localOfficeDeviceFieldIdx).Convert<string>(string.Empty);
            if (string.IsNullOrEmpty(localOfficeValue))
            {
                if (PopulateDistMapNoAU.AllowExecutionOutsideOfArcMap)
                {
                    _logger.Debug(string.Format("Local Office ID value is null on the '{0}' with feature OID = '{1}'", pObject.Class.AliasName, pObject.OID));
                    UpdateDeviceFeature(deviceObject, WRONGLO_MAPNUMBER, null);
                    return;
                }
                else
                {
                    string message = string.Format("LocalOfficeID field value can not be null on the '{0}' feature OID = {1}.", pObject.Class.AliasName, pObject.OID);
                    throw new COMException(message, (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
            }

            //Apply Spatial filter using input object on DistributionMap ObjectClass
            ISpatialFilter pSFilter = new SpatialFilterClass();
            pSFilter.Geometry = deviceObject.Shape;
            pSFilter.GeometryField = (deviceObject.Class as IFeatureClass).ShapeFieldName;
            pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
            pSFilter.WhereClause = _distMapSubtypeFldName + "=" + _subtypeApplicable;
            if (DeletedMapFeatureOID != -1)
            {
                pSFilter.WhereClause += " AND " + _pDistMapClass.OIDFieldName + "<>" + DeletedMapFeatureOID.ToString(); //"";//"[" + pField.Name + "] = (select min( [" + pField.Name + "]) from " + dSet.Name + ")";
            }
            IFeatureCursor pFeaCursor = _pDistMapClass.Search(pSFilter, false);

            if (pFeaCursor != null)
            {
                //Get the Dist Map Feature List from cursor
                List<DistributionMapFeature> distMapFeatureList = PrepareListFromCursor(pFeaCursor);
                bool wrongLO = false;
                //Get the Map Features with LocalOfficeID as that of the device feature and with least scale
                distMapFeatureList = GetFilteredMapFeatureList(distMapFeatureList, localOfficeValue.ToUpper(), out wrongLO);

                //This lolgic was introduced as part of a DA meeting 
                //Business wanted to populate the MapNumber with WrongLO value if the following condition is met.
                //With LocalOffice filter there are not polygon features found but without LocalOffice filter there is atleast 1 MapPolygon found.
                //If no polygon is found even after removing the "LocalOffice" filter then populate NOMAP Value.

                if (wrongLO)//If wrong LocalOffice is found
                {
                    UpdateDeviceFeature(deviceObject, WRONGLO_MAPNUMBER, null);
                }
                //If no map feature found, then populate "NOMAP" into the device
                else if (distMapFeatureList.Count == 0)
                {
                    UpdateDeviceFeature(deviceObject, NOMAP_MAPNUMBER, null);
                }
                else if (distMapFeatureList.Count == 1)
                {//Populate the MapNo from the only MapPolygon found
                    UpdateDeviceFeature(deviceObject, distMapFeatureList[0].MapNumber, distMapFeatureList[0].MapOffice);
                }
                else
                {
                    //Throw error if multiple polygons are found
                    //throw new COMException("Unable to find the Distribution Map Polygon to inherit the Map Number.", (int)mmErrorCodes.MM_E_CANCELEDIT);
                    //The above logic was changed by Robert on 05/17/2013 to update the mapnumber as "DUP MAP" and the MapOffice from teh first map feature. 
                    //Per Robert both map polygon fatures should have the same MapOffice
                    UpdateDeviceFeature(deviceObject, DUPMAP_MAPNUMBER, distMapFeatureList[0].MapOffice);
                }

                ////Separate Map Polygon based on minimum MapNo
                //List<IFeature> lstFeature = UpdateListWithMinimumMapType(deviceObject, pFeaCursor, out _lstSelectedFeaOnMap);
                //Marshal.ReleaseComObject(pFeaCursor);

                ////Populate Disribution MapNO to input object
                //UpdateMapNoFromDistributionMap(pObject, lstFeature, _lstSelectedFeaOnMap, true);
            }
            else
            {
                _logger.Debug("Search result for DistributionMap is EMPTY, exiting.");
                return;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Find Polygon in with Minimum Maptype No.
        /// </summary>
        /// <param name="deviceObject">The Object being created or updated.</param>
        /// <param name="pFeaCursor">Collection of Distribution Map polygon.</param>
        /// <param name="lstSelectedFea">List of polygons selected on Map.</param>
        /// <returns>List of polygons with minimum MapType</returns>
        [Obsolete("Not in use")]
        private List<IFeature> UpdateListWithMinimumMapType(IFeature deviceObject, IFeatureCursor pFeaCursor, out List<IFeature> lstSelectedFea)
        {
            List<IFeature> lstFeature = new List<IFeature>();
            lstSelectedFea = new List<IFeature>();
            int minVal = 0;
            object mapTypeVal = null;
            int cnt = 1;
            bool isNumeric = false;
            int outNumeric = 0;
            IFeature tmpFea = null;
            IFeature pFeature = pFeaCursor.NextFeature();
            while (pFeature != null)
            {
                tmpFea = pFeature;
                lstSelectedFea.Add(tmpFea);

                mapTypeVal = pFeature.get_Value(_mapTypeFieldIdx);

                if (!string.IsNullOrEmpty(mapTypeVal.ToString()))
                {
                    isNumeric = int.TryParse(mapTypeVal.ToString(), out outNumeric);

                    if (isNumeric)
                    {
                        if (cnt == 1)
                        {
                            minVal = outNumeric;
                        }
                        else
                        {
                            if (outNumeric <= minVal)
                            {
                                minVal = outNumeric;

                            }
                        }
                        cnt = cnt + 1;
                    }
                }

                pFeature = pFeaCursor.NextFeature();
            }




            for (int i = 0; i < lstSelectedFea.Count; i++)
            {
                pFeature = lstSelectedFea[i];
                if (lstSelectedFea.Count == 1)
                {
                    lstFeature.Add(lstSelectedFea[i]);
                }
                else
                {
                    mapTypeVal = pFeature.get_Value(_mapTypeFieldIdx);

                    if (mapTypeVal.ToString() == minVal.ToString())
                    {
                        lstFeature.Add(pFeature);
                    }
                }

            }

            if (lstSelectedFea.Count > 0 && lstFeature.Count == 0 || lstFeature.Count > 0)
                _isPolygonExist = true;
            else
                _isPolygonExist = false;

            return lstFeature;
        }

        /// <summary>
        /// Populate Disribution MapNO for input object
        /// </summary>
        /// <param name="pObject">The Object being created or updated.</param>
        /// <param name="lstFeature">Collection of Distribution Map polygon with minimum MapType</param>
        /// <param name="lstSelFeature">List of Distribution polygons selected on Map.</param>
        /// <param name="isUpdateOnMultiplePoly">Allows search on Multiple Distribution polygons if true</param>
        [Obsolete("Not in use")]
        private void UpdateMapNoFromDistributionMap(IObject pObject, List<IFeature> lstFeature, List<IFeature> lstSelFeature, bool isUpdateOnMultiplePoly)//, List<IFeature> lstFeatures, List<IFeature> lstSelFeatures
        {
            //Checks Distribution map polygon, if identified apply Map No to input object Else "NOMAP"
            object mapNo = NOMAP_MAPNUMBER;
            object localOfficeValue = null;
            IFeature pFeature = null;
            IFeatureCursor localOfficeDistMapCursor = null;
            if (lstFeature.Count > 0)
            {
                //Checks feature count for 1
                if (lstFeature.Count == 1)//&& lstSelFeature.Count == 1 -- Commented by Naidu
                {
                    pFeature = lstFeature[0];
                    mapNo = pFeature.get_Value(_mapNoFieldIdx);

                    if (!string.IsNullOrEmpty(mapNo.ToString()))
                    {
                        pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                    }
                    else
                    {
                        mapNo = NOMAP_MAPNUMBER;
                        pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                    }
                }
                else
                {
                    if (isUpdateOnMultiplePoly)
                    {
                        //Check for Device is popolated with localOffice without null
                        localOfficeValue = pObject.get_Value(_localOfficeDeviceFieldIdx);
                        if (!string.IsNullOrEmpty(localOfficeValue.ToString()))
                        {
                            //Apply Spatial filter using LocalOffice on multiple MapType polygons with same Map No
                            IFeature deviceFea = pObject as IFeature;
                            ISpatialFilter pSFilter = new SpatialFilterClass();
                            pSFilter.Geometry = deviceFea.Shape as IGeometry;
                            pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                            pSFilter.WhereClause = _distMapSubtypeFldName + "=" + _subtypeApplicable + " AND " + _localOfficePolyField.Name + " = '" + pObject.get_Value(_localOfficeDeviceFieldIdx).ToString() + "'";
                            if (DeletedMapFeatureOID != -1)
                            {
                                pSFilter.WhereClause += " AND " + _pDistMapClass.OIDFieldName + "<>" + DeletedMapFeatureOID.ToString();
                            }
                            localOfficeDistMapCursor = _pDistMapClass.Search(pSFilter, false);

                            if (localOfficeDistMapCursor != null)
                            {
                                List<IFeature> lstSelTmpFeature = new List<IFeature>();

                                //Create list with polygon using minimum Map no
                                List<IFeature> lstMinFeature = UpdateListWithMinimumMapType(deviceFea, localOfficeDistMapCursor, out lstSelTmpFeature);

                                //Recursive method called to update status of Map Number
                                UpdateMapNoFromDistributionMap(pObject, lstMinFeature, lstSelTmpFeature, false);
                                Marshal.ReleaseComObject(localOfficeDistMapCursor);
                                return;
                            }
                        }
                        else
                        {
                            //Update Device with 'NOMAP' if LocalOffice value is null or Empty
                            mapNo = NOMAP_MAPNUMBER;
                            pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                            _logger.Debug("LocalOffice value is Empty, exiting.");
                            return;
                        }
                    }
                    else
                    {
                        if (lstFeature.Count == 1)
                        {
                            pFeature = lstFeature[0];
                            mapNo = pFeature.get_Value(_mapNoFieldIdx);

                            if (!string.IsNullOrEmpty(mapNo.ToString()))
                            {
                                pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                            }
                            else
                            {
                                mapNo = NOMAP_MAPNUMBER;
                                pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                            }
                        }
                        else
                        {
                            //Update Device with 'NOMAP' if identified a multiple polygons with same LocalOffice
                            mapNo = NOMAP_MAPNUMBER;
                            pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                            _logger.Debug("Multiple Distribution Map Polygon identified with same LocalOffice, exiting.");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (_isPolygonExist)
                {
                    //Unable to identify a multiple polygons with minimum map number
                    mapNo = NOMAP_MAPNUMBER;
                    pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                    _logger.Debug("Not found Polygon with minimum Map Number, exiting.");
                }
                else
                {
                    //Update Device with 'NOMAP' if Distribution map polygon not found
                    pObject.set_Value(_mapNoElecFieldIdx, mapNo);
                    _logger.Debug("Not found Distribution Map Polygon, exiting.");
                }
                return;
            }
        }

        /// <summary>
        /// Prepares Map Polygon List from the cusror
        /// </summary>
        /// <param name="mapCursor">Cursor to retrieve the Map Polygon features</param>
        /// <returns>Returns the list of map polygon features</returns>
        private List<DistributionMapFeature> PrepareListFromCursor(IFeatureCursor mapCursor)
        {
            List<DistributionMapFeature> mapFeatureList = new List<DistributionMapFeature>();

            if (mapCursor == null) return mapFeatureList;
            IFeature mapFeature = null;

            DistributionMapFeature distMapFeature = null;
            //Loop through all the Map Polygon features
            while ((mapFeature = mapCursor.NextFeature()) != null)
            {
                //Create Distribution Map feature and store MapNumber, LocalOffice and MapType values of the Map Feature
                distMapFeature = new DistributionMapFeature();
                distMapFeature.MapNumber = mapFeature.get_Value(_mapNoFieldIdx).Convert<string>(string.Empty);
                distMapFeature.LocalOffice = mapFeature.get_Value(_localOfficePolyFieldIdx).Convert<string>(string.Empty).ToUpper();
                distMapFeature.MapType = mapFeature.get_Value(_mapTypeFieldIdx).Convert<int>(0);
                //If the MapType=0, then it is "Other" map type ( 0 = Domain code and "Other" = Domain Description).
                //This should be treated with least priority if any MapPolygon exists scale whose MapType is not "Other"
                if (distMapFeature.MapType == 0) distMapFeature.MapType = 9999;//This is set just for sorting purpose
                distMapFeature.MapOffice = mapFeature.get_Value(_mapOfficeFldIx).Convert<string>(string.Empty);

                //Add the Distribution Map feature to the list
                mapFeatureList.Add(distMapFeature);
            }

            return mapFeatureList;
        }

        /// <summary>
        /// Returns the filtered list
        /// <list type="bullet">
        /// <item>First, for the Features having the LocalOfficeID as that of <paramref name="localOfficeID"/></item>
        /// <item>If multiple features present in the list, then for the features with least scale i.e. Map Type</item>
        /// </list>
        /// </summary>
        /// <param name="mapFeatureList">Map Polygon feature list to filter</param>
        /// <param name="localOfficeID">LocalOfficeID to filter</param>
        /// <returns>Returns the filtered Map Polygon feature list.</returns>
        private List<DistributionMapFeature> GetFilteredMapFeatureList(List<DistributionMapFeature> mapFeatureList, string localOfficeID, out bool wrongLocalOffice)
        {
            wrongLocalOffice = false;
            List<DistributionMapFeature> filteredMapFeatureList = new List<DistributionMapFeature>();

            //Return the list if it containt either none or one map feature 
            if (mapFeatureList.Count == 0) return mapFeatureList;

            //Get the Map polygon features with LocalOfficeID as that of the device feature
            filteredMapFeatureList = mapFeatureList.Where(mapFeature => string.Compare(mapFeature.LocalOffice, localOfficeID) == 0).ToList<DistributionMapFeature>();

            if (filteredMapFeatureList.Count == 0 && mapFeatureList.Count != 0)
            {
                wrongLocalOffice = true; return filteredMapFeatureList;
            }

            //Return the list if it containt either none or one map feature
            if (filteredMapFeatureList.Count < 2) return filteredMapFeatureList;

            //Get the Map polygon features with least scale
            int leastMapScale = filteredMapFeatureList.Min(mapFeature => mapFeature.MapType);
            filteredMapFeatureList = filteredMapFeatureList.Where(mapFeature => mapFeature.MapType == leastMapScale).ToList<DistributionMapFeature>();
            //Return the list if it containt either none or one map feature
            return filteredMapFeatureList;
        }

        /// <summary>
        /// Updates Map Number and Map Office into the Device feature
        /// </summary>
        /// <param name="deviceFeature">Device feature to update</param>
        /// <param name="mapNo">Map Number to be updated</param>
        /// <param name="mapOffice">Map Office to be updated</param>
        private void UpdateDeviceFeature(IFeature deviceFeature, string mapNo, string mapOffice)
        {
            deviceFeature.set_Value(_mapNoElecFieldIdx, mapNo);
            if (MapOfficeDeviceFieldIndex != -1) deviceFeature.set_Value(MapOfficeDeviceFieldIndex, mapOffice);
        }

        #endregion Private Methods
    }

    internal class DistributionMapFeature : IComparable<DistributionMapFeature>
    {
        //IFeature _mapFeature = null;

        /// <summary>
        /// Creates an instance of the DistributionMapFeature record.
        /// </summary>
        /// <param name="mapFeature"></param>
        internal DistributionMapFeature(IFeature mapFeature)
        {
        }

        /// <summary>
        /// Creates an instance of the DistributionMapFeature record.
        /// </summary>
        internal DistributionMapFeature()
        {
        }

        /// <summary>
        /// The the identifier of the feature.
        /// </summary>
        public string MapNumber { get; set; }
        /// <summary>
        /// The type of the map.
        /// </summary>
        public int MapType { get; set; }
        /// <summary>
        /// The local office this map is from or used by.
        /// </summary>
        public string LocalOffice { get; set; }
        /// <summary>
        /// The map office this map is from or used by.
        /// </summary>
        public string MapOffice { get; set; }

        public int CompareTo(DistributionMapFeature other)
        {
            //Sort by the map type field
            int sortValue = MapType.CompareTo(other.MapType);
            if (sortValue == 0)
            {
                sortValue = LocalOffice.CompareTo(other.LocalOffice);
            }
            return sortValue;
        }
    }
}
