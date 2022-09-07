using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER
{
    public abstract class BasePointInPolyATE : BaseAutoTextElement
    {
        #region Private Fields
        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Polygon Feature Class Model Name
        /// </summary>
        private readonly string _polygonFeatureClassModelName;
        /// <summary>
        /// Polygon Layer Name
        /// </summary>
        private readonly string _polygonLayerName;
        /// <summary>
        /// Field Model Name of field in Polygon Feature Class
        /// </summary>
        private readonly string _fieldModelName;
        /// <summary>
        /// Field Name of field in Polygon Feature Class
        /// </summary>
        private readonly string _fieldName;
        /// <summary>
        /// Indicates whether the reference to the Polygon Feature Class and Field is retrieved.
        /// </summary>
        private bool? _referencesValid = null;

        #endregion Private Fields

        #region Protected Fields
        /// <summary>
        /// Reference to the polygon feature class
        /// </summary>
        protected IFeatureClass _polygonFeatureClass;
        /// <summary>
        /// Reference to a field index in polygon feature class
        /// </summary>
        protected int _fieldIndex = -1;
        #endregion Protected Fields

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            MMCustomTextSources.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            MMCustomTextSources.Unregister(regKey);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the new instance of <see cref="BasePointInPolyATE"/>
        /// </summary>
        /// <param name="progID">ProgID of ATE</param>
        /// <param name="defaultText">DefaultText of ATE</param>
        /// <param name="caption">Caption of ATE that is visible under ArcMap Insert menu</param>
        /// <param name="message">ATE message</param>
        /// <param name="polygonFeatureClassModelName">Model name of polygon feature class</param>
        /// <param name="polygonLayerName">Layer name of the polygon feature class. This is used only if <paramref name="polygonFeatureClassModelName"/> = null.</param>
        /// <param name="fieldModelName">Mode name assigned to a field in polygon feature class</param>
        /// <param name="fieldName">Name of field in polygon feature class</param>
        public BasePointInPolyATE(string progID, string defaultText, string caption, string message, string polygonFeatureClassModelName, string polygonLayerName, string fieldModelName, string fieldName) :
            base(progID, defaultText, caption, message)
        {
            _polygonFeatureClassModelName = polygonFeatureClassModelName;
            _polygonLayerName = polygonLayerName;
            _fieldModelName = fieldModelName;
            _fieldName = fieldName;
        }

        #endregion Constructor

        #region Overridden Methods

        /// <summary>
        /// Content to be displayed in AutoTextElement
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns the content to be displayed in AutoTextElement</returns>
        protected override string GetText(Miner.Interop.mmAutoTextEvents eTextEvent, Miner.Interop.IMMMapProductionInfo pMapProdInfo)
        {
            string returnText = _DefaultText;

            try
            {
                //For the first time, retrieve the references of Feature Class and Field to be used
                if (_referencesValid == null)
                {
                    _referencesValid = RetrieveReferences(pMapProdInfo);
                }
                //Validate whether the references are valid
                if (_referencesValid == false) return returnText = " ";

                //Get the polygon feature to retrieve the required attribute information
                IFeature polygonFeature = GetUnderlyingPolygonFeature(pMapProdInfo);
                if (polygonFeature == null)
                {
                    _logger.Debug("Could not retrieve the polygon feature overlapping the current Map Grid feature.");
                    return returnText = " ";
                }

                //Get the Field value from the polygon feature
                object fieldValue = polygonFeature.get_Value(_fieldIndex);
                if (fieldValue == null || DBNull.Value == fieldValue) return returnText = " ";
                else returnText = fieldValue.ToString();

                return returnText;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                returnText = " "; return returnText;
            }
        }

        protected override string OnStart(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return GetText(eTextEvent, pMapProdInfo);
        }

        /// <summary>
        /// Returns the <see cref="Caption"/>
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns <see cref="Caption"/> as the content to be displayed in AutoTextElement</returns>
        protected override string OnCreate(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return base.Caption;
        }

        public override bool NeedRefresh(mmAutoTextEvents eTextEvent)
        {
            return eTextEvent == mmAutoTextEvents.mmPrint
                   || eTextEvent == mmAutoTextEvents.mmPlotNewPage;
        }

        #endregion Overridden Methods

        #region Private Methods

        /// <summary>
        /// Retrieves the Polygon feature class and field indexes using Model Names and/or Names
        /// </summary>
        /// <param name="pMapProdInfo">MapProductionInfo object</param>
        /// <returns>Returns <c>true</c> if the references retrieved are valid; false, otherwise.</returns>
        private bool RetrieveReferences(Miner.Interop.IMMMapProductionInfo pMapProdInfo)
        {
            bool referenceValid = false;

            try
            {
                /*Retrieve the feature class using Polygon feature class model name.
                 * If the Polygon feature class model name is null, retrieve the same using the layer name*/
                if (!string.IsNullOrEmpty(_polygonFeatureClassModelName))
                {
                    //Get the login workspace
                    IWorkspace loginWorkspace = GetLoginWorkspace();
                    if (loginWorkspace == null)
                    {
                        _logger.Debug("ArcFM Login Database reference is null."); return referenceValid;
                    }
                    //Retrieve the Polygon Feature Class using Feature Class Model Name
                    _polygonFeatureClass = ModelNameFacade.FeatureClassByModelName(loginWorkspace, _polygonFeatureClassModelName);
                }
                else if (!string.IsNullOrEmpty(_polygonLayerName))
                {
                    //Retrieve the Polygon Feature Class using Feature Class Name
                    if (pMapProdInfo != null) //pMapProdInfo <> null only if eTextEvent = NewPlot or FinishPlot. Refer ArcFM help.
                    {
                        _polygonFeatureClass = GetFeatureClass(pMapProdInfo.Map, _polygonLayerName);
                    }
                    else
                    {
                        _polygonFeatureClass = GetFeatureClass(MapProductionFacade.PGEMap, _polygonLayerName);
                    }
                }
                else
                {
                    _logger.Debug("Both Polygon Feature Class Model Name and Feature Class Name can not be null."); return referenceValid;
                }

                //Validate the feature class
                if (_polygonFeatureClass == null)
                {
                    _logger.Debug("Feature Class retrieved using the Class Model Name or Feature Class Name is null."); return referenceValid;
                }
                else if (_polygonFeatureClass.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    _logger.Debug("Feature Class retrieved using the Class Model Name or Feature Class Name is not of Polygon geometry type.");
                    _polygonFeatureClass = null;//Reset to null
                    return referenceValid;
                }

                //Retrieve the field using the field model name. If the Field Model Name is null, then retrieve using field name
                if (!string.IsNullOrEmpty(_fieldModelName))
                {
                    //Retrieve the field using Model Name
                    _fieldIndex = ModelNameFacade.FieldIndexFromModelName(_polygonFeatureClass, _fieldModelName);
                }
                else if (!string.IsNullOrEmpty(_fieldName))
                {
                    //Retrieve the field using Name
                    _fieldIndex = _polygonFeatureClass.FindField(_fieldName);
                }
                else
                {
                    _logger.Debug("Both Field Model Name and Field Name can not be null."); return referenceValid;
                }

                //Validate the field
                if (_fieldIndex == -1)
                {
                    _logger.Debug("Field retrieved using the Model Name or Field Name is null."); return referenceValid;
                }

                return referenceValid = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex); return referenceValid;
            }
        }

        /// <summary>
        /// Retrieves the reference to ArcFM login Database.
        /// </summary>
        /// <returns>Returns the reference to the ArcFM login Database</returns>
        private IWorkspace GetLoginWorkspace()
        {
            //Get the ArcFM Login extension
            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            System.Object obj = Activator.CreateInstance(type);
            ESRI.ArcGIS.Framework.IApplication app = obj as ESRI.ArcGIS.Framework.IApplication;
            IMMLogin2 login = (IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT");
            return login.LoginObject.LoginWorkspace;
        }

        /// <summary>
        /// Get the Feature Class of required layer in the map
        /// </summary>
        /// <param name="map">Reference to the map</param>
        /// <param name="layerName">Name of the Layer to be retrieved</param>
        /// <returns>Returns the feature reference of the layer</returns>
        private IFeatureClass GetFeatureClass(IMap map, string layerName)
        {
            if (map == null) return null;
            //get all layers in Map
            IEnumLayer enumLayer = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            IFeatureClass featureClass = null;

            while ((layer = enumLayer.Next()) != null)//loop through each layer
            {
                //Validate the layer and check layer name
                if (layer.Valid && layer.Name.ToLower() == layerName.ToLower()) //check layer name matching
                {
                    _logger.Debug(layerName + " layer found.");
                    featLayer = layer as IFeatureLayer;
                    featureClass = featLayer.FeatureClass;
                    break;
                }
            }

            //Release object
            if (enumLayer != null) Marshal.ReleaseComObject(enumLayer);

            return featureClass;
        }

        /// <summary>
        /// Get the polygon feature overlapping the centroid of the current Map Grid feature being plotted
        /// </summary>
        /// <param name="pMapProdInfo">MapProductionInfo object</param>
        /// <returns>Returns the polygon feature overlapping the centroid of the current Map Grid feature being plotted</returns>
        private IFeature GetUnderlyingPolygonFeature(IMMMapProductionInfo pMapProdInfo)
        {
            IFeature polygonFeature = null;
            try
            {
                //Get the current map grid feature
                IFeature mapGridFeature = null;
                if (pMapProdInfo != null)
                {
                    mapGridFeature = GetCurrentMapGrid(pMapProdInfo);
                }
                else
                {
                    mapGridFeature = GetCurrentMapGrid(MapProductionFacade.PGEPageLayout, MapProductionFacade.PGEMap);
                }
                if (mapGridFeature == null)
                {
                    _logger.Debug("Could not retrieve current Map Grid feature.");
                    return null;
                }

                //Prepare spatial filter
                ISpatialFilter filter = new SpatialFilterClass();
                filter.GeometryField = _polygonFeatureClass.ShapeFieldName;
                filter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                filter.Geometry = GetCentroid(mapGridFeature.Shape);

                //Get the Polygon feature having the centroid of the Map Grid feature
                IFeatureCursor cursor = _polygonFeatureClass.Search(filter, true);
                //Get the first feature
                polygonFeature = cursor.NextFeature();
                //Release the cursor
                Marshal.ReleaseComObject(cursor);
                return polygonFeature;
            }
            catch (Exception ex)
            {
                _logger.Error("Could not retrieve the polygon feature having the centroid of the map grid feature.", ex);
                return null;
            }
        }

        /// <summary>
        /// Get the Map Grid Feature currently being plotted.
        /// </summary>
        /// <param name="pMapProdInfo">MapProductionInfo object</param>
        /// <returns>Returns the Map Grid Feature currently being plotted.</returns>
        private IFeature GetCurrentMapGrid(IMMMapProductionInfo pMapProdInfo)
        {
            IFeature currentMapGrid = null;
            try
            {
                if (pMapProdInfo == null) return currentMapGrid;
                IMMMapProductionInfoEx pMapProdInfoEx = pMapProdInfo as IMMMapProductionInfoEx;
                IMMStandardMapBook mapSet = pMapProdInfoEx.MapBook as IMMStandardMapBook;
                //Get the current Map Sheet
                IMMMapSheet currentMapSheet = mapSet.CurrentMapSheet;
                //Get the current map grid feature
                currentMapGrid = (currentMapSheet as IMMFeatureMapSheet).ExtentFeature;
                return currentMapGrid;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get the current map grid being plotted.", ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the centroid of the geometry
        /// </summary>
        /// <param name="sourceGeometry">Source Geometry</param>
        /// <returns>Returns the centroid of the geometry</returns>
        private IGeometry GetCentroid(IGeometry sourceGeometry)
        {
            IGeometry centroid = null;
            IArea area = sourceGeometry as IArea;
            centroid = area.Centroid;
            return centroid;
        }

        /// <summary>
        /// Get the Map Grid Feature currently being plotted by reading the IElementProperties.CustomProperty of the AutoText Element
        /// </summary>
        /// <param name="mxDocument">MxDocument Object</param>
        /// <returns>Returns the Map Grid Feature currently being plotted.</returns>
        private IFeature GetCurrentMapGrid(IPageLayout pageLayout, IMap map)
        {
            IFeature currentMapGrid = null;
            try
            {
                if (pageLayout == null || map == null) return null;
                //Get the MapGrid Number as the CustomProperty of the AutoTextElement
                IElement ateElement = MapProductionFacade.GetArcFMAutoTextElement(pageLayout, this.Caption);
                string mapGridNumber = MapProductionFacade.GetMapGridNumberFromATE(ateElement);
                if (mapGridNumber == string.Empty) return null;
                string[] gridNameMapNoandOfficeValues = mapGridNumber.Split(":".ToCharArray());

                // Get the first Map Grid Layer
                List<IFeatureLayer> mapGridLayers = MapProductionFacade.FeatureLayerByModelName(SchemaInfo.General.ClassModelNames.MapGridMN, map, true);
                if (mapGridLayers.Count < 1)
                {
                    _logger.Debug(string.Format("No layer assigned with {0} model name is present active data frame.", SchemaInfo.General.ClassModelNames.MapGridMN));
                    return null;
                }

                string featureClassName, mapNumber, mapOffice;
                IFeatureClass featureClass = null;

                if (gridNameMapNoandOfficeValues.Length > 2)
                {
                    featureClassName = gridNameMapNoandOfficeValues[0];
                    mapNumber = gridNameMapNoandOfficeValues[1];
                    mapOffice = gridNameMapNoandOfficeValues[2];

                    for (int i = 0; i < mapGridLayers.Count; i++)
                    {
                        if ((mapGridLayers[i].FeatureClass as IDataset).Name == featureClassName)
                        {
                            featureClass = mapGridLayers[i].FeatureClass;
                            break;
                        }
                    }
                }
                else if (gridNameMapNoandOfficeValues.Length > 1)
                {
                    mapNumber = gridNameMapNoandOfficeValues[0];
                    mapOffice = gridNameMapNoandOfficeValues[1];
                    //Get the feature class associated with first layer
                    featureClass = mapGridLayers[0].FeatureClass;
                }
                else
                {
                    mapOffice = string.Empty;
                    mapNumber = gridNameMapNoandOfficeValues[0];
                    featureClass = mapGridLayers[0].FeatureClass;
                }


                if (featureClass == null) return null;
                //Get the Map Grid Feature from MapNumber and MapOffice
                string mapNumberFieldName = ModelNameFacade.FieldNameFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapNumberMN);
                string mapOfficeFieldName = ModelNameFacade.FieldNameFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);
                string whereClause = string.Format("{0}='{1}'", mapNumberFieldName, mapNumber);
                if (mapOffice != string.Empty)
                {
                    whereClause += string.Format(" AND {0}='{1}'", mapOfficeFieldName, mapOffice);
                }
                _logger.Debug("WhereClause to get the mapgrid feature = " + whereClause);
                //Get the Grid Feature with given MapNumber and Mapoffice
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = whereClause;
                //PGE IT small change - problem with recycling cursor - change to non-recycling 
                IFeatureCursor featureCursor = featureClass.Search(filter, false);
                currentMapGrid = featureCursor.NextFeature();
                while (Marshal.ReleaseComObject(featureCursor) > 0) { };
                while (Marshal.ReleaseComObject(filter) > 0) { };
                return currentMapGrid;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get the current map grid being plotted.", ex);
                return null;
            }
        }

        #endregion Private Methods

    }
}
