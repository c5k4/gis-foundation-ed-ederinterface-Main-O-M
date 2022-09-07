using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER
{
    [Guid("273DBCE1-6F1C-473C-BC6E-7AB79011E27C")]
    [ProgId("PGE.Desktop.EDER.XMinATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class XMinATE : BaseAutoTextElement
    {
        #region Private Fields

        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IMap _map = null;
        private IPageLayout _pageLayout = null;
        
        #endregion

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
        /// Creates an instance of <see cref="XMinATE"/>
        /// </summary>
        public XMinATE()
            : base("PGE.Desktop.EDER.XMinATE", MapProductionFacade.PGEXMinCaption, MapProductionFacade.PGEXMinCaption, MapProductionFacade.PGEXMinCaption)
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
        /// Basically, returns the xMin coordinate of the printing,exporting grid.
        /// </remarks>
        protected override string OnStart(mmAutoTextEvents eTextEvent, IMMMapProductionInfo pMapProdInfo)
        {
            IFeature mapGridFeature = null;
            if (pMapProdInfo != null) // check if event is fired from for plot then pMapProdInfo will not be null.
            {
                _map = pMapProdInfo.Map;
                _pageLayout = pMapProdInfo.PageLayout;
            }
            // if event is fired from arcmap or standalone application then the arcmap and standalone application should
            //set the PGEMap and PGEPageLayout.
            else if (MapProductionFacade.PGEMap != null && MapProductionFacade.PGEPageLayout != null)
            {
                _map = MapProductionFacade.PGEMap;
                _pageLayout = MapProductionFacade.PGEPageLayout;
            }
            else // if map and pagelayout not found from above two condition then try to get the application if arcmap is running.
            {
                IApplication application = ApplicationFacade.Application;
                if (application != null)
                {
                    _map = (application.Document as IMxDocument).FocusMap;
                    _pageLayout = (application.Document as IMxDocument).PageLayout;
                }
            }

            if (_map == null)
            {
                _logger.Error("Map not found.");
                return string.Empty;
            }
            if (_pageLayout == null)
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
                mapGridFeature = GetCurrentMapGrid(_pageLayout, _map);
            }
            //Retrieve the Map Grid Feature from IElementProperties.CustomProperty

            if (mapGridFeature == null)
            {
                _logger.Error("MapGrid feature not found.");
                return string.Empty;
            }

            //Get the envelope of the mapGridFeature.
            IEnvelope mapGridFeatureEnvelope = mapGridFeature.ShapeCopy.Envelope;
            //convert the envelope coordinate to current activeview map coordinate.
            IGeometry mapGridEnvelopeGeometry = (IGeometry)mapGridFeatureEnvelope;
            //IGeometry transformedMapGridEnvelopeGeometry = MapProductionFacade.ProjectGeometryToMapCoordinateSystem(mapGridEnvelopeGeometry, _map);
            IGeometry transformedMapGridEnvelopeGeometry = MapProductionFacade.ProjectToWGS84(mapGridEnvelopeGeometry);

            return Math.Round(transformedMapGridEnvelopeGeometry.Envelope.XMin,6).ToString();
        }

        protected override string GetText(Miner.Interop.mmAutoTextEvents eTextEvent, Miner.Interop.IMMMapProductionInfo pMapProdInfo)
        {
            return OnStart(eTextEvent, pMapProdInfo);
        }
        #endregion

        #region Private Method
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
                IElement ateElement = MapProductionFacade.GetArcFMAutoTextElement(pageLayout, this.Caption);
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

        #endregion
    }
}
