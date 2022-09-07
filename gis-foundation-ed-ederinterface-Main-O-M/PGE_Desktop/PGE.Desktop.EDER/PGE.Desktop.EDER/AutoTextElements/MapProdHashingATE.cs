using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoTextElement that creates hashing buffer around the grid being printed.
    /// </summary>
    [Guid("32D6113C-E9B9-4362-8439-67BA8B9B4E86")]
    [ProgId("PGE.Desktop.EDER.MapProdHashingATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class MapProdHashingATE : BaseAutoTextElement
    {
        #region Private Fields
        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Holds the reference to currently runnng application
        /// </summary>
        //private IApplication _application = null;
        private IMap _map = null;
        /// <summary>
        /// Layout object
        /// </summary>
        private IPageLayout _pageLayout = null;
        /// <summary>
        /// Largest Map Frame
        /// </summary>
        private IMapFrame _mapFrame = null;
        /// <summary>
        /// Decides whether to execute <see cref="OnFinish(eTextEvent,pMapProdInfo)"/> when fired
        /// </summary>
        private bool _shouldExecuteOnFinishPlot = false;
        #endregion

        #region Private Members
        /// <summary>
        /// Layout object currenlty working with for Plotting/Exporting
        /// </summary>
        private IPageLayout PageLayoutToWork
        {
            get
            {
                return _pageLayout;
            }
            set
            {
                _pageLayout = value;
                SetMap();
            }
        }

        /// <summary>
        /// Map object currenlty working with for Plotting/Exporting
        /// </summary>
        private IMap MapToWork
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;
            }
        }
        #endregion
        /// <summary>
        /// Holds the configuration parameter values used in Map Production
        /// </summary>
        private PGEMapUtilityConfigHelper _pgeConfig = null;
        public PGEMapUtilityConfigHelper PGEMapUtilityConfig
        {
            get
            {
                if (_pgeConfig == null)
                {
                    _pgeConfig = new PGEMapUtilityConfigHelper();
                }
                return _pgeConfig;
            }
        }

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
        /// Creates an instance of <see cref="MapProdHashingATE"/>
        /// </summary>
        public MapProdHashingATE()
            : base("PGE.Desktop.EDER.MapProdHashingATE", MapProductionFacade.PGEHashBufferATECaption, MapProductionFacade.PGEHashBufferATECaption, MapProductionFacade.PGEHashBufferATECaption)
        {

        }

        #endregion Constructor

        #region Overridden Method

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

        /// <summary>
        /// Returns the <see cref="Caption"/>
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns <see cref="Caption"/> as the content to be displayed in AutoTextElement</returns>
        protected override string OnFinish(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            if (_shouldExecuteOnFinishPlot)
            {
                try
                {
                    IElement polygonElement = null;
                    IApplication application = ApplicationFacade.Application;
                    if (application != null)
                    {
                        //MapToWork = (application.Document as IMxDocument).FocusMap;
                        IPageLayout layout = (application.Document as IMxDocument).PageLayout;
                        polygonElement = MapProductionFacade.GetElementByName(MapProductionFacade.PGELayoutPolygonGraphicCaption, (IGraphicsContainer)layout);
                        MapProductionFacade.RestoreElementPropeties((IGraphicsContainer)PageLayoutToWork, polygonElement, false);
                        (layout as IActiveView).Refresh();
                    }
                    polygonElement = MapProductionFacade.GetElementByName(MapProductionFacade.PGELayoutPolygonGraphicCaption, (IGraphicsContainer)PageLayoutToWork);
                    MapProductionFacade.RestoreElementPropeties((IGraphicsContainer)PageLayoutToWork, polygonElement, true);
                }
                finally
                {
                    _shouldExecuteOnFinishPlot = false;
                }
            }
            return base.Caption;
        }
        /// <summary>
        /// Returns the <see cref="Caption"/>
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns <see cref="Caption"/> as the content to be displayed in AutoTextElement</returns>
        protected override string OnRefresh(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return base.Caption;
        }
        /// <summary>
        /// Returns the <see cref="Caption"/>
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns <see cref="Caption"/> as the content to be displayed in AutoTextElement</returns>
        protected override string OnDraw(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            return base.Caption;
        }
        /// <summary>
        /// This method will be called whenever the printing/plotting begins.
        /// </summary>
        /// <param name="eTextEvent">AutoTextElement event</param>
        /// <param name="pMapProdInfo">MapProduction object that fired this event</param>
        /// <returns></returns>
        /// <remarks>
        /// Basically, draws a hash buffer around the Map Grid being printed and adds the Hash Buffer as graphic element
        /// </remarks>
        protected override string OnStart(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            //Should allow execution of OnFinishPlot method if fired
            _shouldExecuteOnFinishPlot = true;

            IFeature mapGridFeature = null;
            if (pMapProdInfo != null) // check if event is fired from for plot then pMapProdInfo will not be null.
            {
                //MapToWork = pMapProdInfo.Map;
                PageLayoutToWork = pMapProdInfo.PageLayout;

            }
            // if event is fired from arcmap or standalone application then the arcmap and standalone application should
            //set the PGEMap and PGEPageLayout.
            else if (MapProductionFacade.PGEMap != null && MapProductionFacade.PGEPageLayout != null)
            {
                //MapToWork = MapProductionFacade.PGEMap;
                PageLayoutToWork = MapProductionFacade.PGEPageLayout;

            }
            else // if map and pagelayout not found from above two condition then try to get the application if arcmap is running.
            {
                IApplication application = ApplicationFacade.Application;
                if (application != null)
                {
                    //MapToWork = (application.Document as IMxDocument).FocusMap;
                    PageLayoutToWork = (application.Document as IMxDocument).PageLayout;
                }
            }

            if (MapToWork == null)
            {
                _logger.Error("Map not found.");
                return string.Empty;
            }
            if (PageLayoutToWork == null)
            {
                _logger.Error("Pagelayout not found.");
                return string.Empty;
            }

            if (pMapProdInfo != null)
            {
                mapGridFeature = GetCurrentMapGrid(pMapProdInfo);
            }
            else
            {
                mapGridFeature = GetCurrentMapGrid(PageLayoutToWork, MapToWork);
            }
            //Retrieve the Map Grid Feature from IElementProperties.CustomProperty

            if (mapGridFeature == null)
            {
                _logger.Error("MapGrid feature not found.");
                return string.Empty;
            }

            //get the MapFrame.
            //IGraphicsContainer layoutGraphicsContainer = PageLayoutToWork as IGraphicsContainer;
            //layoutGraphicsContainer.Reset();
            //IElement elementMap = layoutGraphicsContainer.Next();
            IMapFrame mapFrame = _mapFrame;
            //while (elementMap != null)
            //{
            //    mapFrame = elementMap as IMapFrame;
            //    if (mapFrame != null)
            //    {
            //        break;
            //    }

            //    elementMap = layoutGraphicsContainer.Next();
            //}

            //Creating the polygon of the MapFrame Boundary.
            IPolygon mapFramePolygon = GeometryFacade.PolygonFromEnvelope((mapFrame as IElement).Geometry.Envelope);
            //Converting the mapgridfeature geometry polygon coordinate to page layout coordinate
            Boolean canGraphicsVisibleWithinMapFrameBoundary = false;
            IPolygon LayoutPolygon = convertMapGeometryToLayoutGraphic(mapFrame, (mapGridFeature.Shape as IPolygon), out canGraphicsVisibleWithinMapFrameBoundary);
            
            IFillSymbol hashSymbol = null;

            //Get the symbol config values
            PGEMapUtilityConfigHelper _pgeConfig = new PGEMapUtilityConfigHelper();
            if (_pgeConfig.Valid)
            {

                string borderColor = _pgeConfig.BufferBorderColorRGB;
                string fillColor = _pgeConfig.BufferFillColorRGB;
                double borderWidth = _pgeConfig.BufferBorderWidth;
                int fillStyle = _pgeConfig.BufferFillStyle;
                byte bufferTransperancy = _pgeConfig.BufferTransperancy;
                int[] borderColorRGB = new int[] { 255, 0, 0 }; // default red
                int[] fillColorRGB = new int[] { 0, 255, 0 }; // default green
                if (!string.IsNullOrEmpty(borderColor))
                {
                    borderColorRGB = System.Array.ConvertAll(borderColor.Split(",".ToCharArray()), s => int.Parse(s));
                }
                if (!string.IsNullOrEmpty(fillColor))
                {
                    fillColorRGB = System.Array.ConvertAll(fillColor.Split(",".ToCharArray()), s => int.Parse(s));
                }
                esriSimpleFillStyle esriFillStyle = esriSimpleFillStyle.esriSFSForwardDiagonal; //default forwardDiagonal
                // parse the fill style
                if (Enum.IsDefined(typeof(esriSimpleLineStyle), fillStyle as object))
                {
                    string styleName = Enum.GetName(typeof(esriSimpleFillStyle), fillStyle as object);
                    esriFillStyle = (esriSimpleFillStyle)Enum.Parse(typeof(esriSimpleFillStyle), styleName);
                }

                //Prepare Hash Symbol
                hashSymbol = CreateHashSymbol(fillColorRGB, borderColorRGB, esriFillStyle, borderWidth, bufferTransperancy);
            }


            //Add Hash Element to pagelayout

            //Case the grid is not within current map extent.
            // Do not add graphics and making the graphics geometry to null, so that it will not be visible in the printing.


            //if (canGraphicsVisibleWithinMapFrameBoundary)
            //{
            //    mapFramePolygon = new PolygonClass();
            //}
            //else
            //{

            //Adding the hole (layoutpolygon) into the MapFramePolygon
            IGeometryCollection extRrings = ((IPolygon4)LayoutPolygon).ExteriorRingBag as IGeometryCollection;
            IRing holeRing = extRrings.get_Geometry(0) as IRing;
            if (holeRing.IsExterior)
                holeRing.ReverseOrientation();
            object missing = Type.Missing;
            ((IGeometryCollection)mapFramePolygon).AddGeometry(holeRing, ref missing, ref missing);

            //}


            bool shouldStorePropeties = (eTextEvent == mmAutoTextEvents.mmStartPlot || eTextEvent == mmAutoTextEvents.mmPrint);
            //Adding the hash graphic to the layout element.
            AddHashLayoutElement(PageLayoutToWork, mapFramePolygon, hashSymbol, shouldStorePropeties);
            (PageLayoutToWork as IActiveView).Refresh();
            MapProductionFacade.RefreshMapFromPageLayout(PageLayoutToWork);
            return " ";//base._DefaultText;
        }
        /// <summary>
        /// Content to be displayed in AutoTextElement
        /// </summary>
        /// <param name="eTextEvent">AutotextElement event</param>
        /// <param name="pMapProdInfo">MapProduction info</param>
        /// <returns>Returns the content to be displayed in AutoTextElement</returns>
        /// <remarks>
        /// Basically, draws a hash buffer around the Map Grid being printed and adds the Hash Buffer as graphic element
        /// </remarks>
        protected override string GetText(Miner.Interop.mmAutoTextEvents eTextEvent, Miner.Interop.IMMMapProductionInfo pMapProdInfo)
        {
            return OnStart(eTextEvent, pMapProdInfo);
            ////Miner.Framework.Document.ActiveView
            //if (pMapProdInfo == null) return base._DefaultText;
            ////Retrieve the Map Grid Feature from IElementProperties.CustomProperty
            //IFeature mapGridFeature = GetCurrentMapGrid(pMapProdInfo);

            ////get the HashGeometry
            //IGeometry hashGeometry = PrepareHashGeometry(mapGridFeature);

            ////Get the Hash Symbol
            ////Get the symbol config values
            //PGEMapUtilityConfigHelper _pgeConfig = new PGEMapUtilityConfigHelper();
            //string borderColor = _pgeConfig.BufferBorderColorRGB;
            //string fillColor = _pgeConfig.BufferFillColorRGB;
            //double borderWidth = _pgeConfig.BufferBorderWidth;
            //int fillStyle = _pgeConfig.BufferFillStyle;
            //byte bufferTransperancy = _pgeConfig.BufferTransperancy;
            //int[] borderColorRGB = new int[] { 255, 0, 0 }; // default red
            //int[] fillColorRGB = new int[] { 0, 255, 0 }; // default green
            //if (!string.IsNullOrEmpty(borderColor))
            //{
            //    borderColorRGB = System.Array.ConvertAll(borderColor.Split(",".ToCharArray()), s => int.Parse(s));
            //}
            //if (!string.IsNullOrEmpty(fillColor))
            //{
            //    fillColorRGB = System.Array.ConvertAll(borderColor.Split(",".ToCharArray()), s => int.Parse(s));
            //}
            //esriSimpleFillStyle esriFillStyle = esriSimpleFillStyle.esriSFSForwardDiagonal; //default forwardDiagonal
            //// parse the fill style
            //if (Enum.IsDefined(typeof(esriSimpleLineStyle), fillStyle as object))
            //{
            //    string styleName = Enum.GetName(typeof(esriSimpleFillStyle), fillStyle as object);
            //    esriFillStyle = (esriSimpleFillStyle)Enum.Parse(typeof(esriSimpleFillStyle), styleName);
            //}
            //IFillSymbol hashSymbol = CreateHashSymbol(fillColorRGB, borderColorRGB, esriFillStyle, borderWidth, bufferTransperancy);

            ////Add Hash Element to Map
            //IMap map = pMapProdInfo.Map;
            //AddHashElement(map, hashGeometry, hashSymbol);

            //return base._DefaultText;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepare the Hash Geometry with the specified Buffer. Note this will only return Buffer geometry excluding the future like a disc.
        /// </summary>
        /// <param name="gridFeature">Grid Feature to created hash buffer around</param>
        /// <returns>Returns buffered has geometry</returns>
        private IGeometry PrepareHashGeometry(IFeature gridFeature)
        {
            if (gridFeature == null) return null;
            IClone geometryClone = gridFeature.ShapeCopy as IClone;
            IGeometry geometry = geometryClone.Clone() as IGeometry;
            ITopologicalOperator topologicalOperator = geometry as ITopologicalOperator;

            //Get the buffer distance from config file
            IGeometry bufferGeometry = topologicalOperator.Buffer(PGEMapUtilityConfig.BufferDiameter);
            topologicalOperator = bufferGeometry as ITopologicalOperator;
            IGeometry hashGeometry = topologicalOperator.Difference(geometry);
            Marshal.ReleaseComObject(topologicalOperator);
            return hashGeometry;
        }
        /// <summary>
        /// Adds the Hash Element to Map
        /// </summary>
        /// <param name="map">Current Map</param>
        /// <param name="hashGeometry">Hash Geometry</param>
        /// <param name="hashSymbol">Hash Symbol</param>
        private void AddHashElement(IMap map, IGeometry hashGeometry, IFillSymbol hashSymbol)
        {

            //Creates IElement with given geometry and symbol
            IElement hashElement = new PolygonElementClass();
            hashElement.Geometry = hashGeometry;
            (hashElement as IFillShapeElement).Symbol = hashSymbol;

            //Set the name for the element
            //(hashElement as IElementProperties).Name = PGE.Common.Delivery.ArcFM.MapProductionFacade.PGEHashBufferATECaption;
            //Adds the hash element to map

            IGraphicsContainer graphicsContainer = map as IGraphicsContainer;
            graphicsContainer.Reset();
            IElement element = graphicsContainer.Next();
            while (element != null)
            {
                if ((element as IElementProperties).Name.ToUpper().Equals(PGE.Common.Delivery.Framework.MapProductionFacade.PGEHashBufferATECaption.ToUpper()))
                {
                    element.Geometry = hashGeometry;
                    graphicsContainer.UpdateElement(element);
                    break;
                }
                element = graphicsContainer.Next();
            }
            //graphicsContainer.AddElement(hashElement, 0);
            IActiveView activeView = map as IActiveView;
            activeView.PartialRefresh(esriViewDrawPhase.esriViewAll, hashElement, null);
            activeView.Refresh();
        }

        /// <summary>
        /// Add hash element to the Pagelayout
        /// </summary>
        /// <param name="map">Current Map</param>
        /// <param name="hashGeometry">Hash Geometry</param>
        /// <param name="hashSymbol">Hash Symbol</param>
        private void AddHashLayoutElement(IPageLayout pageLayOut, IGeometry hashGeometry, IFillSymbol hashSymbol, bool shouldStoreEleProps)
        {
            IGraphicsContainer layoutGraphicsContainer = pageLayOut as IGraphicsContainer;
            //Finding the mapframe element in pagelayout.
            layoutGraphicsContainer.Reset();
            IElement elementMap = layoutGraphicsContainer.Next();
            //IMapFrame mapFrame = null;
            //while (elementMap != null)
            //{

            //    mapFrame = elementMap as IMapFrame;
            //    if (mapFrame != null)
            //    {
            //        break;
            //    }

            //    elementMap = layoutGraphicsContainer.Next();
            //}


            //Finding layout polygon element.
            IElement elementGeo = MapProductionFacade.GetElementByName(MapProductionFacade.PGELayoutPolygonGraphicCaption, PageLayoutToWork as IGraphicsContainer);

            //Store the original Geometry and Symbol of the polygon element before drawing
            if (shouldStoreEleProps) MapProductionFacade.StoreElementPropeties(elementGeo);

            //checking if both mapframe and layout graphic polygon found then add the hash geometry.
            if (_mapFrame != null && elementGeo != null)
            {
                IElement mapElement = _mapFrame as IElement;
                //graphicsContainer = mapFrame.Container;
                IPolygon hashLayoutPolygon = hashGeometry as IPolygon;

                elementGeo.Geometry = hashLayoutPolygon as IGeometry;
                if (hashSymbol != null)
                {
                    (elementGeo as IFillShapeElement).Symbol = hashSymbol;
                }
                layoutGraphicsContainer.UpdateElement(elementGeo);
                layoutGraphicsContainer.UpdateElement(_mapFrame as IElement);
                (_mapFrame.Map as IActiveView).Refresh();
            }
            else
            {
                _logger.Error("MapFrame or graphic polygon '" + MapProductionFacade.PGELayoutPolygonGraphicCaption + "' not found in the pagelayout.");
            }
        }

        /// <summary>
        /// Convert the Map Geometry to Layout Graphic
        /// </summary>
        /// <param name="mapFrame">MapFrame based on this the convertion will be made.</param>
        /// <param name="polygon">IPolygon. Polygon to convert</param>
        /// <returns>Polygon.</returns>
        private IPolygon convertMapGeometryToLayoutGraphic(IMapFrame mapFrame, IPolygon polygon, out Boolean isGeometryOutOfExtent)
        {
            isGeometryOutOfExtent = false;
            IElement mapElement = mapFrame as IElement;

            //mapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
            // Get the bounds of the MapFrame.
            Double dLayoutXMin = mapElement.Geometry.Envelope.XMin;
            Double dLayoutXMax = mapElement.Geometry.Envelope.XMax;
            Double dLayoutYMin = mapElement.Geometry.Envelope.YMin;
            Double dLayoutYMax = mapElement.Geometry.Envelope.YMax;
            //mapFrame.ExtentType = esriExtentTypeEnum.esriExtentScale;
            IEnvelope mapEnv = (mapFrame.Map as IActiveView).Extent;
            mapFrame.MapBounds = mapEnv;
            //IEnvelope newMapEnv = GetExtentToDraw(mapFrame.Map, mapElement.Geometry.Envelope.Width, mapElement.Geometry.Envelope.Height);
            (mapFrame.Map as IActiveView).Extent = mapEnv;
            mapEnv = (mapFrame.Map as IActiveView).Extent;
            //Get the bounds of the Map.
            Double dMapXMin = mapEnv.XMin;
            Double dMapXMax = mapEnv.XMax;
            Double dMapYMin = mapEnv.YMin;
            Double dMapyMax = mapEnv.YMax;
            Double dMapScale = mapFrame.MapScale;


            double pageHeight = 0;//(_pageLayout as IActiveView).Extent.Height;
            double pageWidth = 0;
            //_pageLayout.Page.QuerySize(out pageWidth, out pageHeight);
            pageWidth = mapElement.Geometry.Envelope.Width;
            pageHeight = mapElement.Geometry.Envelope.Height;
            IClone polygonClone = polygon as IClone;
            IGeometry geometryPolygon = polygonClone.Clone() as IGeometry;
            geometryPolygon.SpatialReference = polygon.SpatialReference;

            IPolygon4 polygonGeometry = MapProductionFacade.ProjectGeometryToMapCoordinateSystem(geometryPolygon, MapToWork) as IPolygon4;  //polygonClone.Clone() as IPolygon4;
            //Getting all the exteriorRing of the polygon
            IGeometryCollection exteriorRingGeometryCollection = polygonGeometry.ExteriorRingBag as IGeometryCollection;
            //Loop through each exterior ring
            for (int i = 0; i < exteriorRingGeometryCollection.GeometryCount; i++)
            {
                //Getting exterior ring.
                IRing exteriorRing = exteriorRingGeometryCollection.get_Geometry(i) as IRing;
                //Getting all the interior ring in the current exterior ring.
                IGeometryCollection interiorRingGeometryCollection = polygonGeometry.InteriorRingBag[exteriorRing] as IGeometryCollection;

                //loop through each exterior ring points.
                IPointCollection exteriorRingPoints = exteriorRingGeometryCollection.get_Geometry(i) as IPointCollection;
                for (int k = 0; k < (exteriorRingPoints.PointCount - 1); k++)
                {
                    //converting the map point coordinate to page layout coordinate.
                    IPoint layoutPoint = new PointClass();
                    //double xX = (((exteriorRingPoints.get_Point(k).X - dMapXMin) * pageWidth) / dMapScale) + dLayoutXMin;
                    //double yY = (((exteriorRingPoints.get_Point(k).Y - dMapYMin) * pageHeight) / dMapScale) + dLayoutYMin;
                    double xX = (((exteriorRingPoints.get_Point(k).X - dMapXMin)) / (mapEnv.Width / pageWidth)) + dLayoutXMin;
                    double yY = (((exteriorRingPoints.get_Point(k).Y - dMapYMin)) / (mapEnv.Height / pageHeight)) + dLayoutYMin;
                    
                    ////The extent of the polygon is not within the current map extent.
                    //if (xX < dLayoutXMin || xX > dLayoutXMax)
                    //{
                    //    isGeometryOutOfExtent = true;
                    //}
                    //if (yY < dLayoutYMin || yY > dLayoutYMax)
                    //{
                    //    isGeometryOutOfExtent = true;
                    //}

                    layoutPoint.PutCoords(xX, yY);
                    //Updating the current point to layout point.
                    exteriorRingPoints.UpdatePoint(k, layoutPoint);

                }

                //Loop through each interior ring.
                for (int interiorGeometryIndx = 0; interiorGeometryIndx < interiorRingGeometryCollection.GeometryCount; interiorGeometryIndx++)
                {
                    //Getting the points of the current interior ring.
                    IPointCollection interiorRingPoints = interiorRingGeometryCollection.get_Geometry(interiorGeometryIndx) as IPointCollection;
                    //Loop through each interior ring points.
                    for (int j = 0; j < (interiorRingPoints.PointCount - 1); j++)
                    {
                        //converting the map point coordinate to page layout coordinate.
                        IPoint layoutPoint = new PointClass();
                        //double xX = (((interiorRingPoints.get_Point(j).X - dMapXMin) * 12) / dMapScale) + dLayoutXMin;
                        //double yY = (((interiorRingPoints.get_Point(j).Y - dMapYMin) * 12) / dMapScale) + dLayoutYMin;
                        double xX = (((interiorRingPoints.get_Point(j).X - dMapXMin)) / (mapEnv.Width / pageWidth)) + dLayoutXMin;
                        double yY = (((interiorRingPoints.get_Point(j).Y - dMapYMin)) / (mapEnv.Height / pageHeight)) + dLayoutYMin;
                        
                        //if (xX < dLayoutXMin || xX > dLayoutXMax)
                        //{
                        //    isGeometryOutOfExtent = true;
                        //}
                        //if (yY < dLayoutYMin || yY > dLayoutYMax)
                        //{
                        //    isGeometryOutOfExtent = true;
                        //}

                        layoutPoint.PutCoords(xX, yY);
                        //Updating the current point to layout point.
                        interiorRingPoints.UpdatePoint(j, layoutPoint);
                    }
                }
            }

            return polygonGeometry as IPolygon;

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
                IElement ateElement = MapProductionFacade.GetArcFMAutoTextElement(pageLayout, PGE.Common.Delivery.Framework.MapProductionFacade.PGEHashBufferATECaption);
                string mapGridNumber = MapProductionFacade.GetMapGridNumberFromATE(ateElement);
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

        /// <summary>
        /// Create a hash symbol by specifying Boder/buffer.
        /// </summary>
        /// <param name="fillColorRGB">A System.int[] that is the RGB color of the fill symbol. Example: {0,255,0}</param>
        /// <param name="borderColorRGB">A System.int[] that is the RGB color of the outside line border. Example: {0,255,0}</param>
        /// <param name="fillStyle">A System.int that is the fill style of the fill symbol. Exmple: 0 = Solid</param>
        /// <param name="borderLineWidth">A System.Double that is the width of the outside line border in points. Example: 2</param>
        /// <param name="bufferTransperancy">A System.byte that is the transperancy of the fill symbol.</param>
        /// <returns></returns>
        private IFillSymbol CreateHashSymbol(int[] fillColorRGB, int[] borderColorRGB, esriSimpleFillStyle fillStyle, double borderLineWidth, byte bufferTransperancy)
        {
            //Create fill color
            IRgbColor fillColor = new RgbColorClass();
            fillColor.Red = fillColorRGB[0];
            fillColor.Green = fillColorRGB[1];
            fillColor.Blue = fillColorRGB[2];
            //fillColor.Transparency = bufferTransperancy;

            //Create border color
            IRgbColor borderColor = new RgbColorClass();
            borderColor.Red = borderColorRGB[0];
            borderColor.Green = borderColorRGB[1];
            borderColor.Blue = borderColorRGB[2];

            //Create simple line symbol
            //ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            //simpleLineSymbol.Width = borderLineWidth;
            //simpleLineSymbol.Color = borderColor;

            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = borderLineWidth;
            simpleLineSymbol.Color = borderColor;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;

            IFillSymbol fillSymbol = null;
            if (fillStyle == esriSimpleFillStyle.esriSFSSolid)
            {
                // Create simple fill symbol
                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                simpleFillSymbol.Outline = simpleLineSymbol;
                simpleFillSymbol.Style = fillStyle;
                simpleFillSymbol.Color = fillColor;
                fillSymbol = simpleFillSymbol;
            }
            else
            {
                double angle = 45;
                switch (fillStyle)
                {
                    case esriSimpleFillStyle.esriSFSForwardDiagonal:
                        angle = -45; break;
                    case esriSimpleFillStyle.esriSFSBackwardDiagonal:
                        angle = 45; break;
                    case esriSimpleFillStyle.esriSFSHorizontal:
                        angle = 0; break;
                    case esriSimpleFillStyle.esriSFSVertical:
                        angle = 90; break;

                }
                ILineFillSymbol lineFilleSymbol = new LineFillSymbolClass();
                lineFilleSymbol.Angle = angle;
                //lineFilleSymbol.LineSymbol = simpleLineSymbol;
                lineFilleSymbol.Color = fillColor;
                lineFilleSymbol.Outline = simpleLineSymbol;
                lineFilleSymbol.Separation = 20;
                fillSymbol = lineFilleSymbol;
            }

            //return simpleFillSymbol;
            return fillSymbol;
        }

        /// <summary>
        /// Loops through each IElement in the PageLayout and gets the Map and MapFrame objects of the largest DataFrame.
        /// </summary>
        /// <returns></returns>
        private IMap SetMap()
        {
            IMap retVal = null;
            IGraphicsContainer gc = (IGraphicsContainer)PageLayoutToWork;
            IActiveView activeView = (IActiveView)PageLayoutToWork;
            IElement element = null;
            gc.Reset();
            double currWidth = 0.0;
            double currHeight = 0.0;
            double width = 0.0;
            double height = 0.0;
            IElement largestMapFrame = null;
            while ((element = gc.Next()) != null)
            {
                if (element is IMapFrame)
                {
                    GetWidthAndHeight(element, activeView, out currWidth, out currHeight);
                    if ((width * height) < (currWidth * currHeight))
                    {
                        largestMapFrame = element;
                        width = currWidth;
                        height = currHeight;
                    }
                }
            }
            gc.Reset();
            element = null;
            if (largestMapFrame != null)
            {
                _mapFrame = (IMapFrame)largestMapFrame;
                MapToWork = _mapFrame.Map;
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Height and Widht of the passed in IElement object
        /// </summary>
        /// <param name="element"></param>
        /// <param name="activeView"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void GetWidthAndHeight(IElement element, IActiveView activeView, out double width, out double height)
        {
            width = 0.0;
            height = 0.0;
            IEnvelope env = new EnvelopeClass();
            element.QueryBounds(activeView.ScreenDisplay, env);
            width = Math.Round(env.Width);
            height = Math.Round(env.Height);
        }
               
        #endregion

    }
}
