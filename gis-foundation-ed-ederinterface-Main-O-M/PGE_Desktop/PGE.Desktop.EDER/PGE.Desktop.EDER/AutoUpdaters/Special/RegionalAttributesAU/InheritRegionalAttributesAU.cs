// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing - Point in Polygon Component Specification v0.7
// Shaikh Rizuan 10/15/2012	Created
// </history>
// All rights reserved.
// ========================================================================

using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using System;
using System.IO;

using log4net;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.ValidationRules;

namespace PGE.Desktop.EDER.AutoUpdaters.Special.RegionalAttributesAU
{
    /// <summary>
    /// Look of the value in the external system based on the GIS value
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    /// <summary>
    /// Populate  Install Job Year Field Value.
    /// </summary>
    [ComVisible(true)]
    [Guid("AA4E5B72-888E-448F-BB9C-10C43DBB5BC6")]
    [ProgId("PGE.Desktop.EDER.InheritRegionalAttributesAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class InheritRegionalAttributesAU : BaseSpecialAU
    {
        #region Class Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.LineStartPoint, SchemaInfo.Electric.ClassModelNames.LineEndPoint, SchemaInfo.Electric.ClassModelNames.LineMidPoint, SchemaInfo.Electric.ClassModelNames.PointEnforceRegion };
        private static string _warningMsg = "Field Model name :{0} not found.";
        private string _domainName;
        private static string _fieldModelName = "MODEL_NAME";
        private static string _className = "CLASS_NAME";
        private static string _fieldName = "FIELD_NAME";
        private static string _derivedFieldName = "DERIVED_FIELD_NAME";
        private static string _settingName = "Setting_Name";
        private static string _canBeNull = "canbenull";
        private static string _useDomain = "usedomain";
        private static string _settingValue = "Setting_Value";
        private static string _integratnName = "PointInPolygon";
        private List<string> _lstArrayFldModelNames = new List<string>();
        private IMap _currentMap = null;
        private int _layerCount;
        #endregion

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public InheritRegionalAttributesAU()
            : base("PGE Inherit Regional Attributes AU")
        {

        }

        /// <summary>
        /// The name of the mapping to use
        /// </summary>
        [XmlAttributeAttribute()]
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }
        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {


            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, _modelNames);
                _logger.Debug("Field model name :" + _modelNames[0] + "," + _modelNames[1] + " Found-" + enabled);

            }

            return enabled;
        }

        // Add a sneaky option allowing us to bypass ArcMap only check. The bypass is disabled by default but callers can turn it on via property
        private static bool _allowExecutionOutsideOfArcMap = false;
        private static IWorkspace LandbaseWorkspace = null;
        public static void AllowExecutionOutsideOfArcMap(IWorkspace landbaseWorkspace)
        {
            if (landbaseWorkspace != null)
            {
                LandbaseWorkspace = landbaseWorkspace;
                _allowExecutionOutsideOfArcMap = true;
            }
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap || _allowExecutionOutsideOfArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.Debug("Entering InheritRegionalAttriutes AU for " + obj.Class.AliasName + " oid: " + obj.OID.ToString());

            if (!_allowExecutionOutsideOfArcMap)
            {
                if (_currentMap == null)
                {
                    _currentMap = RegionalAttributeMethod.GetMapFromArcMap(base.Application);
                    //current map layer count
                    _layerCount = _currentMap.LayerCount;
                    //_currentMap.
                }
                else
                {
                    if (_currentMap.LayerCount != _layerCount)//if current map layer count not match with previous map layer count
                    {
                        _layerCount = _currentMap.LayerCount;
                        //add layer to layer list
                        RegionalAttributeMethod.GetFeatureLayerList(_currentMap);
                    }
                }
            }
            IFeature feat = obj as IFeature;
            // IFeatureClass pFeatClass = obj.Class as IFeatureClass;
            IFeatureChanges featureChange = feat as IFeatureChanges;

            IFeature searchedFeature = null;
            bool fireAU = true;

            #region get Workspace
            ITable table = feat.Table;
            IDataset dataSet = table as IDataset;
            IWorkspace wrkSpace = dataSet.Workspace;
            #endregion
            //Get object class from PGE_CONFIG Table
            IObjectClass configObjClas = ModelNameFacade.ObjectClassByModelName(wrkSpace, SchemaInfo.Electric.ClassModelNames.PGEConfig);

            if (configObjClas != null)//if confiObjClass is not null
            {
                //Get model list name from PGE_CONFIG Table
                if (_lstArrayFldModelNames.Count == 0)
                {
                    _lstArrayFldModelNames = RegionalAttributeMethod.GetModelNamesList(configObjClas, _fieldModelName);
                    _lstArrayFldModelNames.ToArray();
                }
            }
            else//if confiObjClass is null
            {
                fireAU = false;//dont fire AU
                _logger.Warn(string.Format(_warningMsg, SchemaInfo.Electric.ClassModelNames.PGEConfig));
                return;
            }

            if (eEvent == mmEditEvent.mmEventFeatureUpdate)// if event is OnFeatureUpdate
            {
                fireAU = false;
                if (featureChange.ShapeChanged)// if feature shape changed
                {
                    fireAU = true;
                }
            }


            if (fireAU)//fire AU if true
            {
                //INC00003866620 - Fix for performance problem 
                //Check if we have a cache at this point and that the current 
                //point is within the cache zone 
                bool populatedFromCache = false;

                if (!_allowExecutionOutsideOfArcMap)
                {
                    if (ValidationEngine.Instance.Application == null)
                        ValidationEngine.Instance.Application = base.Application;
                    if (ValidationEngine.Instance.AdminCache != null)
                    {
                        IPoint pSearchPoint = GetSearchPoint(feat);
                        IRelationalOperator pRelOp = (IRelationalOperator)
                            ValidationEngine.Instance.AdminCache.AdminIntersect;
                        if (pRelOp.Contains((IGeometry)pSearchPoint))
                        {
                            //Can populate the regional attributes from the cache 
                            for (int i = 0; i < _lstArrayFldModelNames.Count; i++)
                            {
                                //check if model name assigned to object
                                if (ModelNameFacade.ContainsFieldModelName(obj.Class, _lstArrayFldModelNames[i]))
                                {
                                    // get field index for field
                                    int objFldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, _lstArrayFldModelNames[i]);
                                    obj.set_Value(objFldIndex, ValidationEngine.Instance.AdminCache.
                                        RegionalAttributes[_lstArrayFldModelNames[i]]);
                                }
                            }
                            populatedFromCache = true;
                        }
                    }
                }

                //loop through all model names
                if (!populatedFromCache)
                {

                    //Try to re-build the cache, create a list of all regional 
                    //attribute values for caching 
                    Hashtable hshRegionalAttValues = new Hashtable();
                    Hashtable hshRegionalAttPolygons = new Hashtable();
                    for (int i = 0; i < _lstArrayFldModelNames.Count; i++)
                    {
                        //These ones are not used by any featureclasses (should be removed from 
                        //PGE_CONFIG table!)  
                        if ((_lstArrayFldModelNames[i] != "PGE_INHERITLOCALOFFICE") &&
                            (_lstArrayFldModelNames[i] != "PGE_INHERITMAPOFFICE"))
                        {
                            if (!hshRegionalAttValues.ContainsKey(_lstArrayFldModelNames[i]))
                            {
                                hshRegionalAttValues.Add(_lstArrayFldModelNames[i], "NOT_POPULATED");
                                hshRegionalAttPolygons.Add(_lstArrayFldModelNames[i], "NOT_POPULATED");
                            }
                        }
                    }

                    for (int i = 0; i < _lstArrayFldModelNames.Count; i++)
                    {
                        //check if model name assigned to object
                        if (ModelNameFacade.ContainsFieldModelName(obj.Class, _lstArrayFldModelNames[i]))
                        {
                            // get field index for field
                            int objFldIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, _lstArrayFldModelNames[i]);

                            //IObjectClass configObjClas = ModelNameFacade.ObjectClassByModelName(pWorkSpace, "PGE_CONFIG");
                            if (configObjClas != null) //if configObjClass not null
                            {
                                #region Get Layer and Field name
                                ITable configTbl = configObjClas as ITable;
                                IQueryFilter queryFltr = new QueryFilterClass();
                                // pass where clause in query filter
                                queryFltr.WhereClause = _fieldModelName + "='" + _lstArrayFldModelNames[i] + "'";
                                _logger.Debug(queryFltr.WhereClause);
                                //search through table
                                ICursor cursor = configTbl.Search(queryFltr, false);
                                IRow configRow = null;
                                try
                                {
                                    if ((configRow = cursor.NextRow()) != null)
                                    {
                                        object layerName = configRow.get_Value((configRow.Fields.FindField(_className)));
                                        _logger.Debug(layerName.ToString());
                                        object fieldName = configRow.get_Value((configRow.Fields.FindField(_fieldName)));
                                        _logger.Debug(fieldName.ToString());
                                #endregion
                                        //get polygon feature class for search
                                        if (_allowExecutionOutsideOfArcMap && !RegionalAttributeMethod.RegionalAttributeFeatClasses.ContainsKey(layerName.ToString().ToUpper()))
                                        {
                                            Dictionary<string, string> layerNameToFeatClassName = new Dictionary<string, string>();
                                            layerNameToFeatClassName.Add("Division".ToUpper(), "LBGIS.ElecDivision");
                                            layerNameToFeatClassName.Add("Districts".ToUpper(), "LBGIS.ElecMCDistricts");
                                            layerNameToFeatClassName.Add("County".ToUpper(), "LBGIS.CountyUnClipped");
                                            layerNameToFeatClassName.Add("Region".ToUpper(), "LBGIS.ElecRegion");
                                            //layerNameToFeatClassName.Add("PostDistrict".ToUpper(), "LBGIS.postdist");
                                            layerNameToFeatClassName.Add("PostDistrict".ToUpper(), "LBGIS.MaponicsZip");  // ME Q3-19 - Release
                                            if (layerNameToFeatClassName.ContainsKey(layerName.ToString().ToUpper()))
                                            {
                                                IFeatureClass layerFeatClass = ((IFeatureWorkspace)LandbaseWorkspace).OpenFeatureClass(layerNameToFeatClassName[layerName.ToString().ToUpper()]);
                                                RegionalAttributeMethod.AddRegionalAttributeClass(layerName.ToString(), layerFeatClass);
                                            }
                                        }
                                        IFeatureClass polyFeatClass = RegionalAttributeMethod.GetFeatureClassToSearch(layerName.ToString());
                                        if (polyFeatClass != null)//if feature class present
                                        {
                                            //get searched polygon feature
                                            searchedFeature = GetFeature(feat, obj, polyFeatClass);

                                            if (searchedFeature != null)//Check if feature is not null
                                            {
                                                object searchedFeatreval = searchedFeature.GetFieldValue(fieldName.ToString(), false, null);
                                                if (searchedFeatreval == null) { continue; } //if the value is null, go to the next PGE_INHERITxxx field.

                                                IRelationshipClass pRelClass = ModelNameFacade.RelationshipClassFromModelName(configObjClas, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PgeAppsettings);
                                                if (pRelClass != null)
                                                {
                                                    ISet relatedObjects = pRelClass.GetObjectsRelatedToObject(configRow as IObject);
                                                    if (relatedObjects.Count > 0)
                                                    {
                                                        IObject relatedObject = null;
                                                        relatedObjects.Reset();
                                                        //relatedObject = relatedObjects.Next() as IObject;
                                                        while ((relatedObject = relatedObjects.Next() as IObject) != null)
                                                        {
                                                            object getObjvalue = relatedObject.get_Value((relatedObject.Fields.FindField(_settingName)));
                                                            if (getObjvalue != DBNull.Value) //if value is not null
                                                            {
                                                                if (getObjvalue.ToString().ToLower() == _canBeNull)//if value is "CanBeNull"
                                                                {
                                                                    // get value for CanBeNull
                                                                    object getNullValue = relatedObject.get_Value((relatedObject.Fields.FindField(_settingValue)));
                                                                    _logger.Debug("getNullValue: " + getNullValue.ToString());
                                                                    if (getNullValue != DBNull.Value)//if value not null
                                                                    {
                                                                        //if canbenull is "NO" and featurevalue is null
                                                                        if (getNullValue.ToString().ToLower() == "no" && searchedFeatreval == DBNull.Value)
                                                                        {
                                                                            //throw error message and cancel the edit
                                                                            throw new COMException("The feature " + searchedFeature.Class.AliasName + " " + fieldName.ToString() + " value should not be <Null>.", (int)mmErrorCodes.MM_E_CANCELEDIT);

                                                                        }
                                                                    }
                                                                }
                                                                if (getObjvalue.ToString().ToLower() == _useDomain)//if value is "UseDomain
                                                                {
                                                                    //get value for UseDomain
                                                                    object getDomainValue = relatedObject.get_Value((relatedObject.Fields.FindField(_settingValue)));
                                                                    _logger.Debug("getDomainValue: " + getDomainValue.ToString());
                                                                    //if usedomain value is not null and feature value is not null
                                                                    if (getDomainValue != DBNull.Value && searchedFeatreval != DBNull.Value)
                                                                    {
                                                                        if (getDomainValue.ToString().ToLower() == "yes")//if usedomain value is yes
                                                                        {

                                                                            //get the default integration name
                                                                            RegionalAttributeMethod.IntegrationName = _integratnName;

                                                                            //get the value from xml config 
                                                                            string value = RegionalAttributeMethod.GetValueFromXml(obj.Fields.get_Field(objFldIndex).Domain.Name, searchedFeatreval.ToString());
                                                                            if (!string.IsNullOrEmpty(value))//if value is not null
                                                                            {
                                                                                obj.set_Value(objFldIndex, (object)value);
                                                                                //INC00003866620 - Fix for performance problem  
                                                                                UpdateRegionalAttsCache(ref hshRegionalAttValues,
                                                                                    ref hshRegionalAttPolygons,
                                                                                    _lstArrayFldModelNames[i],
                                                                                    (object)value,
                                                                                    (IPolygon)searchedFeature.ShapeCopy);
                                                                            }
                                                                            else //if null add error log message
                                                                            {
                                                                                _logger.Error("Domain Value not found");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            //set the default value
                                                                            obj.set_Value(objFldIndex, searchedFeatreval);
                                                                            //INC00003866620 - Fix for performance problem  
                                                                            UpdateRegionalAttsCache(ref hshRegionalAttValues,
                                                                                    ref hshRegionalAttPolygons,
                                                                                    _lstArrayFldModelNames[i],
                                                                                    (object)searchedFeatreval,
                                                                                    (IPolygon)searchedFeature.ShapeCopy);
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        _logger.Warn(pRelClass.DestinationClass.AliasName + "does not have related rows.");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Log, but don't throw an error if executing outside of ArcMap.
                                                _logger.Error(feat.Class.AliasName + " is not present within" + polyFeatClass.AliasName);
                                                if (!_allowExecutionOutsideOfArcMap)
                                                {
                                                    throw new COMException("The feature " + feat.Class.AliasName + " OID:" + feat.OID + " is not within proper organizational boundaries.", (int)mmErrorCodes.MM_E_CANCELEDIT);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Warn(layerName + "layer not present");
                                            throw new COMException("The feature " + feat.Class.AliasName + " OID:" + feat.OID + " is not within proper " + layerName.ToString() + " organizational boundaries.", (int)mmErrorCodes.MM_E_CANCELEDIT);


                                        }
                                    }
                                }
                                finally
                                {
                                    //Release COM object
                                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                                    cursor = null;
                                }
                            }
                            else
                            {
                                //_logger.Warn("Model Name " + "PGE_CONFIG" + "not Found");
                            }
                        }
                    }

                    //Try to rebuild the cacche if it is fully populated
                    if (!_allowExecutionOutsideOfArcMap)
                    {
                        AdminCache pAdminCache = GetAdminCache(hshRegionalAttValues, hshRegionalAttPolygons);
                        ValidationEngine.Instance.AdminCache = pAdminCache;
                    }
                }
            }

            _logger.Debug("Leaving InheritRegionalAttriutes AU for " + obj.Class.AliasName + " oid: " + obj.OID.ToString());
            _logger.Debug("Stopwatch Elapsed Milliseconds: " + stopWatch.ElapsedMilliseconds.ToString());
            stopWatch.Stop();
        }

        /// <summary>
        /// INC00003866620 - Fix for performance problem - cache the 
        /// regional attributres so as to limit the number of spatial 
        /// queries performed against DB 
        /// </summary>
        /// <param name="hshRegionalAttValues"></param>
        /// <param name="hshRegionalAttPolygons"></param>
        /// <param name="modelName"></param>
        /// <param name="value"></param>
        /// <param name="pPolygon"></param>
        private void UpdateRegionalAttsCache(
            ref Hashtable hshRegionalAttValues,
            ref Hashtable hshRegionalAttPolygons,
            string modelName,
            object value,
            IPolygon pPolygon)
        {
            try
            {
                //Update cached list of values and the polygon 
                hshRegionalAttValues[modelName] = value;
                hshRegionalAttPolygons[modelName] = pPolygon;
            }
            catch
            {
                throw new Exception("Error updating regional attributes cache");
            }
        }

        private AdminCache GetAdminCache(Hashtable hshRegionalAttValues, Hashtable hshRegionalAttPolygons)
        {
            try
            {
                //First check that the attributes corresponding at each model 
                //name has been populated 
                bool allAttsPopulated = true;
                foreach (string key in hshRegionalAttValues.Keys)
                {
                    if (hshRegionalAttValues[key].ToString() == "NOT_POPULATED")
                    {
                        allAttsPopulated = false;
                        break;
                    }
                }

                //If we do not have all attributes populated we cannot build 
                //the cache 
                if (!allAttsPopulated)
                    return null;

                //Now loop through all of the polygons and intersect them all 
                //together to find the intersection of ALL regional polygons
                string refPolyKey = "";
                IPolygon pRefPolygon = null;
                foreach (string key in hshRegionalAttPolygons.Keys)
                {
                    refPolyKey = key;
                    pRefPolygon = (IPolygon)hshRegionalAttPolygons[key];
                    break;
                }

                IPolygon pIntersectPolygon = pRefPolygon;
                foreach (string key in hshRegionalAttPolygons.Keys)
                {
                    if (key != refPolyKey)
                    {
                        IPolygon pOtherPolygon = (IPolygon)hshRegionalAttPolygons[key];
                        ITopologicalOperator pTopo = (ITopologicalOperator)pIntersectPolygon;
                        pIntersectPolygon = (IPolygon)pTopo.Intersect(pOtherPolygon,
                            esriGeometryDimension.esriGeometry2Dimension);
                        ITopologicalOperator2 pTopo2 = (ITopologicalOperator2)pIntersectPolygon;
                        pTopo2.IsKnownSimple_2 = false;
                        pTopo2.Simplify();
                    }
                }

                if (pIntersectPolygon.IsEmpty)
                    return null;

                AdminCache pAdminCach = new AdminCache(hshRegionalAttValues, pIntersectPolygon);
                return pAdminCach;
            }
            catch (Exception ex)
            {
                throw new Exception("Error building admin cache");
            }
        }

        private IPoint GetSearchPoint(IFeature feat)
        {
            ICurve curve = feat.Shape as ICurve;
            IPoint searchPoint = null;

            if (feat.FeatureType == esriFeatureType.esriFTSimpleEdge || feat.FeatureType == esriFeatureType.esriFTComplexEdge)
            {
                //IGeometry pGeom = pFeature.Shape;

                #region check for search type
                //if line end point model name assigned
                if (ModelNameFacade.ContainsClassModelName(feat.Class, SchemaInfo.Electric.ClassModelNames.LineEndPoint))
                {
                    searchPoint = curve.ToPoint;//it will return the end point
                }
                else if (ModelNameFacade.ContainsClassModelName(feat.Class, SchemaInfo.Electric.ClassModelNames.LineMidPoint))
                {
                    curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, searchPoint);//it will return mid point
                }
                else
                {
                    //get line start point
                    searchPoint = curve.FromPoint;
                }
                #endregion
            }
            else
            {
                searchPoint = (IPoint)feat.ShapeCopy;
            }
            return searchPoint;
        }


        private IFeature GetFeature(IFeature feat, IObject obj, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            ICurve curve = feat.Shape as ICurve;
            IPoint linePoint = new Point();
            if (feat.FeatureType == esriFeatureType.esriFTSimpleEdge || feat.FeatureType == esriFeatureType.esriFTComplexEdge)
            {
                //IGeometry pGeom = pFeature.Shape;

                #region check for search type
                //if line end point model name assigned
                if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.LineEndPoint))
                {
                    linePoint = curve.ToPoint;//it will return the end point
                    //get polygon feature from spatial search for line end point
                    searchedFeature = RegionalAttributeMethod.GetFeatureFromSpatialQuery(linePoint as IGeometry, polyFeatClass);
                }
                else if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.LineMidPoint))
                {
                    curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, linePoint);//it will return mid point
                    //get polygon feature from spatial search for line mid point
                    searchedFeature = RegionalAttributeMethod.GetFeatureFromSpatialQuery(linePoint as IGeometry, polyFeatClass);
                }
                else
                {
                    //get line start point
                    linePoint = curve.FromPoint;
                    searchedFeature = RegionalAttributeMethod.GetFeatureFromSpatialQuery(linePoint as IGeometry, polyFeatClass);
                    //searchedFeature = RegionalAttributeMethod.GetFeatureFromSpatialQuery(feat, polyFeatClass);

                }
                #endregion
            }
            else
            {
                //get polygon feature from spatial search for point
                searchedFeature = RegionalAttributeMethod.GetFeatureFromSpatialQuery(feat, polyFeatClass);
            }
            return searchedFeature;

        }




        #endregion
    }
}
