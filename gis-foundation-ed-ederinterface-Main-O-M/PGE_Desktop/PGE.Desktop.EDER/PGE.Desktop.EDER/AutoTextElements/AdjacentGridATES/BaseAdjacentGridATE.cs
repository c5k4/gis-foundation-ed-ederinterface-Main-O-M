using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using Miner.Interop;
using ESRI.ArcGIS.Carto;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Base class for displaying the Primary Display Field value of the adjacent grids
    /// </summary>
    public abstract class BaseAdjacentGridATE : PGE.Common.Delivery.ArcFM.BaseAutoTextElement
    {
        #region Private Variables
        /// <summary>
        /// Direction of the Adjacent Grid direction of the Grid to be plotted
        /// </summary>
        private AdjacentGridDirection _adjacentGridDirection = AdjacentGridDirection.None;
        /// <summary>
        /// IMMConfigLevel object used to retrieve the PrimaryDisplay field of a feature class
        /// </summary>
        private IMMConfigTopLevel _configTopLevel = null;
        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Initilizes new instance of <see cref="BaseAdjacentGridATE"/>
        /// </summary>
        /// <param name="progID">ProgID of the ATE</param>
        /// <param name="defaultText">DefaultText of the ATE</param>
        /// <param name="caption">Caption of the ATE</param>
        /// <param name="message">Message of the ATE</param>
        /// <param name="adjacentDirection">Adjacent direction of the ATE</param>
        public BaseAdjacentGridATE(string progID, string defaultText, string caption, string message, AdjacentGridDirection adjacentDirection)
            : base(progID, defaultText, caption, message)
        {
            _adjacentGridDirection = adjacentDirection;
        }
        #endregion Constructor

        #region Public & Private Methods

        /// <summary>
        /// Returns the Primary Display Field value to be displayed adjacent grid ( in given direction) to the current grid being plotted.
        /// </summary>
        /// <param name="mapProdInfo">MapProduction object that fires this event. This value should be null, otherwise.</param>
        /// <param name="direction">Adjacent grid direction</param>
        /// <returns>Returns the Primary Display Field value to be displayed adjacent grid ( in given direction) to the current grid being plotted.</returns>
        public string GetAdjacentGrid(IMMMapProductionInfo mapProdInfo, AdjacentGridDirection direction)
        {
            string adjacentGridDisplayText = String.Empty;

            // Create new points
            IPoint centerPoint = new PointClass();
            IPoint NPoint = new PointClass();
            IPoint SPoint = new PointClass();
            IPoint EPoint = new PointClass();
            IPoint WPoint = new PointClass();
            IPoint NWPoint = new PointClass();
            IPoint NEPoint = new PointClass();
            IPoint SWPoint = new PointClass();
            IPoint SEPoint = new PointClass();

            IEnvelope mapGridEnvelope = null;
            IFeature currentMapGridFeature = null;
            IFeature adjGridFeature;
            double envelopeHeight;
            double envelopeWidth;
            IFeatureClass mapGridFClass;
            ISpatialFilter spatialFilter;
            IFeatureCursor featureCursor;
            string mapGridPriDisplayField;

            try
            {
                //Get the MapGrid from the MapProduction if MapProduction is not null
                if (mapProdInfo != null)
                {
                    currentMapGridFeature = GetCurrentMapGrid(mapProdInfo);
                }
                else
                {
                    //Get the MapGridFeature from MapProductionFacade
                    currentMapGridFeature = MapProductionFacade.CurrentMapGridFeature;
                    if (currentMapGridFeature == null)
                    {
                        // if MapGridFeature = null, then Get the MapGridFeature from MapGrid Primary stored in MapProductionFacade
                        currentMapGridFeature = GetCurrentMapGrid(MapProductionFacade.PGEPageLayout, MapProductionFacade.PGEMap);
                        MapProductionFacade.CurrentMapGridFeature = currentMapGridFeature;
                    }
                }

                // if MapGridFeature = null,then log message and exit
                if (currentMapGridFeature == null)
                {
                    _logger.Debug("Not able to find current Map Grid Feature.");
                    return adjacentGridDisplayText;
                }
                else
                {
                    //Get the map grid feature class and extent of the map grid feature
                    mapGridFClass = currentMapGridFeature.Class as IFeatureClass;
                    mapGridEnvelope = currentMapGridFeature.Extent;

                    //Get the Primary Display field of the Grid feature class
                    mapGridPriDisplayField = GetArcFMPrimaryDisplayField(mapGridFClass);

                    //Find the center point of the current map grid feature
                    envelopeWidth = mapGridEnvelope.XMax - mapGridEnvelope.XMin;
                    envelopeHeight = mapGridEnvelope.YMax - mapGridEnvelope.YMin;
                    centerPoint.X = mapGridEnvelope.XMin + (envelopeWidth / 2);
                    centerPoint.Y = mapGridEnvelope.YMin + (envelopeHeight / 2);

                    //set the points to the N,S,E,W,NW,NE,SW and SE of current map grid feature
                    NPoint.X = centerPoint.X;
                    NPoint.Y = centerPoint.Y + envelopeHeight;
                    SPoint.X = centerPoint.X;
                    SPoint.Y = centerPoint.Y - envelopeHeight;
                    EPoint.X = centerPoint.X + envelopeWidth;
                    EPoint.Y = centerPoint.Y;
                    WPoint.X = centerPoint.X - envelopeWidth;
                    WPoint.Y = centerPoint.Y;
                    NWPoint.X = centerPoint.X - envelopeWidth;
                    NWPoint.Y = centerPoint.Y + envelopeHeight;
                    NEPoint.X = centerPoint.X + envelopeWidth;
                    NEPoint.Y = centerPoint.Y + envelopeHeight;
                    SWPoint.X = centerPoint.X - envelopeWidth;
                    SWPoint.Y = centerPoint.Y - envelopeHeight;
                    SEPoint.X = centerPoint.X + envelopeWidth;
                    SEPoint.Y = centerPoint.Y - envelopeHeight;

                    // Create a new spatial filter and set the properties
                    spatialFilter = new SpatialFilterClass();
                    spatialFilter.GeometryField = mapGridFClass.ShapeFieldName;
                    if (mapGridPriDisplayField != null)
                    {
                        spatialFilter.SubFields = mapGridFClass.OIDFieldName + "," + mapGridPriDisplayField;
                    }
                    else
                    {
                        spatialFilter.SubFields = mapGridFClass.OIDFieldName;
                    }
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                    spatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                    spatialFilter.WhereClause = mapGridFClass.OIDFieldName + "<>" + currentMapGridFeature.OID;

                    //Get the MapScale of the current MapGrid feature
                    int mapScaleFldIx = ModelNameFacade.FieldIndexFromModelName(currentMapGridFeature.Class, SchemaInfo.General.FieldModelNames.MapScaleeMN);
                    if (mapScaleFldIx != -1)
                    {
                        string mapScaleFieldName = currentMapGridFeature.Fields.get_Field(mapScaleFldIx).Name;
                        object mapScaleValue = currentMapGridFeature.get_Value(mapScaleFldIx);
                        if (mapScaleValue == null || mapScaleValue == DBNull.Value) { }
                        else
                        {
                            //Get the Grids having the same scale as that of the grid being plotted
                            spatialFilter.WhereClause += " AND " + mapScaleFieldName + " ='" + mapScaleValue.ToString() + "'";
                        }
                    }

                    //The PGE_HASDATA field value must be '1' if at all exists
                    int hasDataFldIx = ModelNameFacade.FieldIndexFromModelName(currentMapGridFeature.Class, SchemaInfo.General.FieldModelNames.HasDataMN);
                    if (hasDataFldIx != -1)
                    {
                        //Get the Grids having the HASDATA='1'
                        string hasDataFieldName = currentMapGridFeature.Fields.get_Field(hasDataFldIx).Name;
                        spatialFilter.WhereClause += " AND " + hasDataFieldName + " ='1'";
                    }


                    switch (direction)
                    {
                        case AdjacentGridDirection.North:
                            spatialFilter.Geometry = NPoint;
                            break;
                        case AdjacentGridDirection.South:
                            spatialFilter.Geometry = SPoint;
                            break;
                        case AdjacentGridDirection.East:
                            spatialFilter.Geometry = EPoint;
                            break;
                        case AdjacentGridDirection.West:
                            spatialFilter.Geometry = WPoint;
                            break;
                        case AdjacentGridDirection.NorthWest:
                            spatialFilter.Geometry = NWPoint;
                            break;
                        case AdjacentGridDirection.NorthEast:
                            spatialFilter.Geometry = NEPoint;
                            break;
                        case AdjacentGridDirection.SouthWest:
                            spatialFilter.Geometry = SWPoint;
                            break;
                        case AdjacentGridDirection.SouthEast:
                            spatialFilter.Geometry = SEPoint;
                            break;
                    }

                    //Project the Query Geometry to that of the Grid feature class
                    spatialFilter.Geometry.Project(currentMapGridFeature.Shape.SpatialReference);

                    //Find the adjacent map grid feature
                    featureCursor = mapGridFClass.Search(spatialFilter, true);
                    adjGridFeature = featureCursor.NextFeature();
                    if (adjGridFeature == null)
                    {
                        //return empty string if adjacent grid = null
                        adjacentGridDisplayText = " ";
                    }
                    else
                    {
                        if (mapGridPriDisplayField == null) adjacentGridDisplayText = "(" + adjGridFeature.OID.ToString() + ")";
                        else
                        {
                            //Get the PrimaryField index
                            int fieldIx = adjGridFeature.Fields.FindField(mapGridPriDisplayField);
                            //Get the Domain description
                            object objAdjGridID = GetValueFromDomain(adjGridFeature, adjGridFeature.Fields.get_Field(fieldIx));

                            if (objAdjGridID != null)
                            {
                                adjacentGridDisplayText = objAdjGridID.ToString();
                            }

                            //handle null values in primary display field
                            if (adjacentGridDisplayText.Length < 1)
                            {
                                adjacentGridDisplayText = "(" + adjGridFeature.OID.ToString() + ")";
                            }
                        }
                    }

                    //Release the resources
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(spatialFilter);
                    if (adjGridFeature != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(adjGridFeature);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to retrieve Adjacant Grid Primary Display Text.", ex);
            }

            return adjacentGridDisplayText;
        }

        /// <summary>
        /// Returns the Primary Display Field value to be displayed current grid (in None direction) being plotted.
        /// </summary>
        /// <param name="mapProdInfo">MapProduction object that fires this event. This value should be null, otherwise.</param>
        /// <returns>Returns the Primary Display Field value to be displayed current grid (in None direction) being plotted.</returns>
        public string GetCurrentGridPrimaryDisplayFieldValue(IMMMapProductionInfo mapProdInfo)
        {
            string currentGridDisplayText = String.Empty;
            IFeature currentMapGridFeature = null;
            string mapGridPriDisplayField;
            IFeatureClass mapGridFClass;
            try
            {
                if (mapProdInfo != null)
                {
                    currentMapGridFeature = GetCurrentMapGrid(mapProdInfo);
                }
                else
                {
                    //Get the MapGridFeature from MapProductionFacade
                    currentMapGridFeature = MapProductionFacade.CurrentMapGridFeature;
                    if (currentMapGridFeature == null)
                    {
                        // if MapGridFeature = null, then Get the MapGridFeature from MapGrid Primary stored in MapProductionFacade
                        currentMapGridFeature = GetCurrentMapGrid(MapProductionFacade.PGEPageLayout, MapProductionFacade.PGEMap);
                        MapProductionFacade.CurrentMapGridFeature = currentMapGridFeature;
                    }
                }

                // if MapGridFeature = null,then log message and exit
                if (currentMapGridFeature == null)
                {
                    _logger.Debug("Not able to find current Map Grid Feature.");
                    return currentGridDisplayText;
                }
                else
                {
                    //Get the map grid feature class 
                    mapGridFClass = currentMapGridFeature.Class as IFeatureClass;
                    //Get the Primary Display field of the Grid feature class
                    mapGridPriDisplayField = GetArcFMPrimaryDisplayField(mapGridFClass);

                    if (mapGridPriDisplayField == null) currentGridDisplayText = "(" + currentMapGridFeature.OID.ToString() + ")";
                    else
                    {
                        //Get the PrimaryField index
                        int fieldIx = currentMapGridFeature.Fields.FindField(mapGridPriDisplayField);
                        //Get the Domain description
                        object objcurrGridID = GetValueFromDomain(currentMapGridFeature, currentMapGridFeature.Fields.get_Field(fieldIx));

                        if (objcurrGridID != null)
                        {
                            currentGridDisplayText = objcurrGridID.ToString();
                        }

                        //handle null values in primary display field
                        if (currentGridDisplayText.Length < 1)
                        {
                            currentGridDisplayText = "(" + currentMapGridFeature.OID.ToString() + ")";
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to retrieve Current Grid Primary Display Text.", ex);
            }

            return currentGridDisplayText;
        }

        /// <summary>
        ///  Get the reference to IMMConfigTopLevel
        /// </summary>
        private IMMConfigTopLevel ConfigTopLevel
        {
            get
            {
                if (_configTopLevel == null)
                {
                    Type type = Type.GetTypeFromProgID("mmGeodatabase.MMConfigTopLevel");
                    object mmObject = Activator.CreateInstance(type);
                    _configTopLevel = (IMMConfigTopLevel)mmObject;
                    if (_configTopLevel == null)
                    {
                        _logger.Debug("Could not retrieve mmGeodatabase.MMConfigTopLevel extension.");
                    }
                }
                return _configTopLevel;
            }

        }

        /// <summary>
        /// Get the Primary Display Field of the feature class
        /// </summary>
        /// <param name="featureClass">Reference to the feature class</param>
        /// <returns>Returns the PrimaryDisplayField name of the feature class</returns>
        private string GetArcFMPrimaryDisplayField(IFeatureClass featureClass)
        {
            string primaryDisplayField = null;
            try
            {
                if (featureClass == null || ConfigTopLevel == null) return primaryDisplayField;
                IMMFeatureClass fClass = ConfigTopLevel.GetFeatureClassOnly(featureClass);
                if (fClass == null || string.IsNullOrEmpty(fClass.PriDisplayField)) return primaryDisplayField;
                if (featureClass.FindField(fClass.PriDisplayField) != -1)
                {
                    primaryDisplayField = fClass.PriDisplayField;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get the Primary Display Field Name", ex);
            }
            return primaryDisplayField;
        }

        /// <summary>
        /// Get the domain description from the field value of the object
        /// </summary>
        /// <param name="iObj">Reference to the IObject</param>
        /// <param name="field">Reference to Field to retrieve the domain description for the field value</param>
        /// <returns>Returns the domain descriptions for the field value</returns>
        private string GetValueFromDomain(IObject iObj, IField field)
        {
            string valueFromDomain = null;

            try
            {
                if (iObj == null || field == null)
                {
                    return valueFromDomain;
                }

                //Get the field value
                valueFromDomain = iObj.get_Value(iObj.Fields.FindField(field.Name)).ToString();

                //Get the domain attached to the field
                IDomain domain = field.Domain;
                if (domain == null || !(domain.GetType() is ICodedValueDomain))
                {
                    return valueFromDomain;
                }

                ICodedValueDomain codeValueDomain = domain as ICodedValueDomain;
                if (codeValueDomain == null)
                {
                    return valueFromDomain;
                }

                int codeCount = codeValueDomain.CodeCount;
                if (codeCount < 1)
                {
                    return valueFromDomain;
                }

                //Get domain description matching domain code
                for (int i = 0; i < codeCount; i++)
                {
                    if (codeValueDomain.get_Value(i).ToString() == valueFromDomain)
                    {
                        valueFromDomain = codeValueDomain.get_Name(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to retrieve Domain Descritpion from Value.", ex);
            }

            return valueFromDomain;
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
        /// Get the Map Grid Feature currently being plotted by reading the IElementProperties.CustomProperty of the AutoText Element
        /// </summary>
        /// <param name="mxDocument">MxDocument Object</param>
        /// <returns>Returns the Map Grid Feature currently being plotted.</returns>
        private IFeature GetCurrentMapGrid(IPageLayout pageLayout, IMap map)
        {
            IFeature currentMapGrid = null;
            try
            {
                //Get the MapGrid Number as the CustomProperty of the AutoTextElement
                string mapGridNumber = MapProductionFacade.CurrentMapGridPrimaryKey;
                string[] gridNameMapNoandOfficeValues = mapGridNumber.Split(":".ToCharArray());


                if (mapGridNumber == string.Empty) return null;

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
                IFeatureCursor featureCursor = featureClass.Search(filter, true);
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

        #endregion Public & Private Methods

        #region Overridden Methods

        /// <summary>
        /// Returns the Primary Display Field value of the adjacent grid in the specified direction
        /// </summary>
        /// <param name="eTextEvent">Source MapProduction event </param>
        /// <param name="pMapProdInfo">Source MapProduction object</param>
        /// <returns>Returns the Primary Display Field value of the adjacent grid in the specified direction</returns>
        protected override string GetText(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            if (_adjacentGridDirection == AdjacentGridDirection.None)
            {
                return GetCurrentGridPrimaryDisplayFieldValue(pMapProdInfo);
            }
            else
            {
                return GetAdjacentGrid(pMapProdInfo, _adjacentGridDirection);
            }
        }

        /// <summary>
        /// Returns the Primary Display Field value of the adjacent grid in the specified direction
        /// </summary>
        /// <param name="eTextEvent">Source MapProduction event </param>
        /// <param name="pMapProdInfo">Source MapProduction object</param>
        /// <returns>Returns the Primary Display Field value of the adjacent grid in the specified direction</returns>
        /// <remarks>
        /// This is executes only if the <paramref name="eTextEvent"/>=<c>mmPrint</c>; otherwise returns default text.
        /// </remarks>
        protected override string OnStart(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            if (eTextEvent == mmAutoTextEvents.mmPrint)
            {
                if (_adjacentGridDirection == AdjacentGridDirection.None)
                {
                    return GetCurrentGridPrimaryDisplayFieldValue(pMapProdInfo);
                }
                else
                {
                    return GetAdjacentGrid(pMapProdInfo, _adjacentGridDirection);
                }
            }
            else return base._DefaultText;
        }

        /// <summary>
        /// Returns the Caption of the AutoText Element
        /// </summary>
        /// <param name="eTextEvent">Source MapProduction event </param>
        /// <param name="pMapProdInfo">Source MapProduction object</param>
        /// <returns>Returns the Caption of the AutoText Element</returns>
        protected override string OnCreate(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return base.Caption;
        }

        #endregion Overridden Methods
    }

    /// <summary>
    /// Direction of the Adjacent Grids
    /// </summary>
    public enum AdjacentGridDirection
    {
        None = 0,
        North = 1,
        South = 2,
        East = 3,
        West = 4,
        NorthWest = 5,
        NorthEast = 6,
        SouthWest = 7,
        SouthEast = 8
    }
}
