using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.UI.Commands;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoTextElements;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// PGE Custom Print Command that sets print the Grid with buffer hash
    /// </summary>
    [Guid("f049ff3d-9abf-4e4d-bae1-e28726ffd4d2")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGEPrintMapCommand")]
    [ComVisible(true)]
    public sealed class PGEPrintMapCommand : BaseArcGISCommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #endregion Private Varibales

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEPrintMapCommand"/>
        /// </summary>
        public PGEPrintMapCommand() :
            base("PGETools_PGEPrintMapCommand", "PGE Print Map", "PGE Tools", "PGE Print Map", "PGE Print Map")
        {
            base.m_name = "PGETools_PGEPrintMapCommand";
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                Bitmap bmp = null;
                //Get path for bitmap image 
                string path = GetType().Assembly.GetName().Name + ".ArcMapCommands.MapUtilities." + GetType().Name + ".bmp";
                _logger.Debug("Bitmap image path" + path);
                //Get bitmap image
                bmp = new Bitmap(GetType().Assembly.GetManifestResourceStream(path));
                //Assign bitmap image
                UpdateBitmap(bmp, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Warn("Invalid Bitmap" + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }


        }
        #endregion Constructor

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null) return;
            _application = hook as IApplication;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        protected override void InternalClick()
        {
            IFeatureClass selectedMapGridClass = null;
            try
            {
                //Get the current extend of the map.
                IMxDocument mxDocument = _application.Document as IMxDocument;
                IActiveView activeView = mxDocument.FocusMap as IActiveView;
                IEnvelope envelope = activeView.Extent;

                // Get the first Map Grid Layer
                List<IFeatureLayer> mapGridLayers = MapProductionFacade.FeatureLayerByModelName(SchemaInfo.General.ClassModelNames.MapGridMN, mxDocument.FocusMap, true);
                if (mapGridLayers.Count < 1)
                {
                    string message = string.Format("Please add a Map Grid layer to your map.", SchemaInfo.General.ClassModelNames.MapGridMN);
                    _logger.Debug(string.Format("No layer assigned with {0} model name is present active data frame.", SchemaInfo.General.ClassModelNames.MapGridMN));
                    MessageBox.Show(message);
                    return;
                }
                ////Get the Grid layer
                //IFeatureClass featureClass = mapGridLayers[0].FeatureClass;

                ////Get the index of MapNumber and MapOffice fields
                //int mapNoFldIx = ModelNameFacade.FieldIndexFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapNumberMN);
                //int mapOfficeFldIx = ModelNameFacade.FieldIndexFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);
                //if (mapNoFldIx == -1 || mapOfficeFldIx == -1)
                //{
                //    string[] modelNames = new string[] { SchemaInfo.General.FieldModelNames.MapNumberMN, SchemaInfo.General.FieldModelNames.MapOfficeMN, SchemaInfo.General.ClassModelNames.MapGridMN };
                //    _logger.Debug(string.Format("Either {0} or {1} field model names are missing on class with {2} model name", modelNames));
                //    return;
                //}

                ////Get map grids in the current visible extent and extract MapNumer/MapOffice of each Grid
                //ISpatialFilter spatialFilter = new SpatialFilterClass();
                //spatialFilter.Geometry = envelope;
                //spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                //spatialFilter.GeometryField = featureClass.ShapeFieldName;
                //spatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                //IFeatureCursor featureCursor = featureClass.Search(spatialFilter, true);

                ////Loop each grid feature
                //IFeature feature;
                //string mapNo, mapOffice;
                //object objValue;
                //Dictionary<string, string> mapNoandOffice = new Dictionary<string, string>();
                //while ((feature = featureCursor.NextFeature()) != null)
                //{
                //    //Get Map Number
                //    objValue = feature.get_Value(mapNoFldIx);
                //    if (objValue == null || objValue == DBNull.Value)
                //    {
                //        _logger.Debug(string.Format("Map Number is null for OID ={0}", feature.OID));
                //        continue;
                //    }
                //    mapNo = objValue.ToString();

                //    //Get Map Office
                //    objValue = feature.get_Value(mapOfficeFldIx);
                //    if (objValue == null || objValue == DBNull.Value)
                //    {
                //        _logger.Debug(string.Format("Map Number is null for OID ={0}", feature.OID));
                //        mapOffice = string.Empty;
                //    }
                //    mapOffice = objValue.ToString();

                //    //Add the MapNumber and MapOffice to the dictionary if not already present.
                //    //If already present, then append the MapOffice to exiting MapOffice
                //    if (mapNoandOffice.ContainsKey(mapNo))
                //    {
                //        mapNoandOffice[mapNo] = mapNoandOffice[mapNo] + ":" + mapOffice;
                //    }
                //    else
                //    {
                //        mapNoandOffice.Add(mapNo, mapOffice);
                //    }
                //}
                ////Release the resources
                //Marshal.ReleaseComObject(featureCursor);
                //Marshal.ReleaseComObject(spatialFilter);

                Dictionary<string, IFeatureClass> layerCollection = new Dictionary<string, IFeatureClass>();

                for (int i = 0; i < mapGridLayers.Count; i++)
                {
                    IFeatureClass featureClass = mapGridLayers[i].FeatureClass;
                    layerCollection[featureClass.AliasName] = featureClass;
                }

                //Allow user to select the Grid to be printed

                PGEPrintMapGrids frmMapGrid = new PGEPrintMapGrids(layerCollection, envelope);
                DialogResult resultMapGrid = frmMapGrid.ShowDialog();
                selectedMapGridClass = frmMapGrid.SelectedFeatureClass;
                string gridDatasetName = (selectedMapGridClass as IDataset).Name;
                string selectedMapGrid = string.Empty;
                int mapNoFldIx = ModelNameFacade.FieldIndexFromModelName(selectedMapGridClass, SchemaInfo.General.FieldModelNames.MapNumberMN);
                int mapOfficeFldIx = ModelNameFacade.FieldIndexFromModelName(selectedMapGridClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);
                //Process only if user has cicked OK button the windows
                if (resultMapGrid == DialogResult.OK)
                {
                    //Retreive the user selected MapNumber and MapOffice
                    selectedMapGrid = frmMapGrid.SelectedMapGrid;
                    if (selectedMapGrid == string.Empty) return;
                    string[] mapNoandOfficeValues = selectedMapGrid.Split(":".ToCharArray());

                    //Get the Map Grid Feature from MapNumber and MapOffice & prepare whereClause
                    string whereClause = string.Format("{0}='{1}'", selectedMapGridClass.Fields.get_Field(mapNoFldIx).Name, mapNoandOfficeValues[0]);
                    if (mapNoandOfficeValues.Length > 1 && mapNoandOfficeValues[1].Length > 0)
                    {
                        whereClause += string.Format(" AND {0}='{1}'", selectedMapGridClass.Fields.get_Field(mapOfficeFldIx).Name, mapNoandOfficeValues[1]);
                    }
                    //whereClause = "PLATNUMBER = '3027062'";
                    //Ge tthe Map Grid feature for the user selected MapNumber and MapOffice
                    IQueryFilter filter = new QueryFilterClass();
                    filter.WhereClause = whereClause;
                    IFeatureCursor featureCursor = selectedMapGridClass.Search(filter, true);
                    IFeature selFeature = featureCursor.NextFeature();
                    Marshal.ReleaseComObject(featureCursor);
                    if (selFeature == null) return;

                    //Stores the reference to the current grid being plotted
                    MapProductionFacade.CurrentMapGridFeature = selFeature;

                    //Get the buffred geometry and set the extent of the map.
                    //PGEMapUtilityConfigHelper _pgeConfig = new PGEMapUtilityConfigHelper();
                    //IGeometry buffredGeometry = MapProductionFacade.GetBuffredGeometry(selFeature.ShapeCopy, _pgeConfig.BufferDiameter);
                    //activeView.Extent = buffredGeometry.Envelope;

                    //mxDocument.FocusMap.ClipGeometry = buffredGeometry;
                    //Ge the centriod of the use selected Grid feature and pan to this feature
                    IArea area = selFeature.Shape as IArea;
                    IEnvelope visibleExtent = activeView.Extent;
                    //project the centroid to the map projection.
                    IPoint centroidPoint = new PointClass();
                    centroidPoint.PutCoords(area.Centroid.X, area.Centroid.Y);
                    centroidPoint.SpatialReference = area.Centroid.SpatialReference;
                    IPoint projectedCentroid = MapProductionFacade.ProjectGeometryToMapCoordinateSystem(centroidPoint as IGeometry, mxDocument.FocusMap) as IPoint;
                    visibleExtent.CenterAt(projectedCentroid);
                    activeView.Extent = visibleExtent;

                    //check if PGE Layout graphic element is existing.
                    IGraphicsContainer layoutGraphicsContainer = (_application.Document as IMxDocument).PageLayout as IGraphicsContainer;
                    IElement polygonElement = MapProductionFacade.GetElementByName(MapProductionFacade.PGELayoutPolygonGraphicCaption, layoutGraphicsContainer);


                    //if PGE Layout graphic element is not found then add it on the pagelayout.
                    if (polygonElement == null)
                    {
                        polygonElement = new PolygonElementClass();
                        polygonElement.Geometry = selFeature.ShapeCopy;
                        (polygonElement as IElementProperties).Name = MapProductionFacade.PGELayoutPolygonGraphicCaption;
                        layoutGraphicsContainer.AddElement(polygonElement as IElement, 0);
                    }

                    //Store the original Geometry and Symbol of the polygon element before drawing
                    MapProductionFacade.StoreElementPropeties(polygonElement);

                    //Set the MapGrid Number as the CustomProperty of the AutoTextElement
                    IElement ateElementPGEHashBuffer = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEHashBufferATECaption);
                    IElement ateElementXMin = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEXMinCaption);
                    IElement ateElementXMax = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEXMaxCaption);
                    IElement ateElementYMin = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEYMinCaption);
                    IElement ateElementYMax = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEYMaxCaption);
                    IElement ateElementCounty = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGECountyCaption);
                    IElement ateElementDivision = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEDivisionCaption);
                    IElement ateElementZip = MapProductionFacade.GetArcFMAutoTextElement(mxDocument.PageLayout, MapProductionFacade.PGEZipCaption);
                    //Prefix the feature class name
                    selectedMapGrid = gridDatasetName + ":" + selectedMapGrid;
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementPGEHashBuffer, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementXMin, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementXMax, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementYMin, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementYMax, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementCounty, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementDivision, selectedMapGrid);
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementZip, selectedMapGrid);
                    activeView.Refresh();

                    (mxDocument.PageLayout as IActiveView).Refresh();
                    MapProductionFacade.RefreshMapFromPageLayout(mxDocument.PageLayout);
                    MapProductionFacade.PGEMap = mxDocument.FocusMap;
                    MapProductionFacade.PGEPageLayout = mxDocument.PageLayout;
                    try
                    {
                        //Execute OOTB File.Print command
                        ESRI.ArcGIS.esriSystem.UID printCommandUID = new ESRI.ArcGIS.esriSystem.UIDClass();
                        printCommandUID.Value = "{119591DB-0255-11D2-8D20-080009EE4E51}";
                        printCommandUID.SubType = 7;
                        ICommandItem printCommand = _application.Document.CommandBars.Find(printCommandUID, false, false);
                        if (printCommand == null)
                        {
                            _logger.Debug("Failed to find ICommand with UID = {119591DB-0255-11D2-8D20-080009EE4E51} and Subtype=7");
                        }
                        else printCommand.Execute();

                    }
                    catch { throw; }
                    finally
                    {
                        //Set the PGE Layout graphics element geometry = null so that it will not be visible on the map.
                        if (polygonElement != null)
                        {
                            //Restore the original Geometry and Symbol of the polygon element after drawing
                            MapProductionFacade.RestoreElementPropeties(layoutGraphicsContainer, polygonElement, true);
                        }
                    }

                    //Release the reference to the current grid feature being plotted
                    if (MapProductionFacade.CurrentMapGridFeature != null)
                    {
                        while (Marshal.ReleaseComObject(MapProductionFacade.CurrentMapGridFeature) > 0) { }
                        MapProductionFacade.CurrentMapGridFeature = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        #endregion Overridden Class Methods
    }
}
