using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ArcMapCommands.MapUtilities
{
    [ComVisible(true)]
    [Guid("7CD9104A-B16B-40c5-85C1-82A65EF712D0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public class PgeUfmMapCommand : BaseCommand
    {
        private const string CAPTION = "Create UFM Template";

        private const string SD1 = "UFM Block Map View (1:50)";
        private const string SD2 = "UFM Pri UG Duct View (1:50)";
        private const string SD3 = "UFM Sec UG Duct View (1:50)";
        private const string SD4 = "UFM DC Duct View (1:50)";
        

        #region Member vars
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private IMap _map = null;

        #endregion

        #region COM Registration Function(s)

        [ComRegisterFunction()]
        [ComVisible(false)]
        private static void Register(string regKey)
        {
            // Required for ArcGIS Component Category Registrar support
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        private static void UnRegister(string regKey)
        {
            // Required for ArcGIS Component Category Registrar support
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Constructor

        public PgeUfmMapCommand()
        {
            // Setup command info
            base.m_category = "PGE"; //localizable text 
            base.m_caption = CAPTION;  //localizable text 
            base.m_message = CAPTION;  //localizable text
            base.m_toolTip = CAPTION;  //localizable text
            base.m_name = "PgeTools_PgeUfmMapCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.CrossSection.CrossSection.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

        #region Button overrides

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
            {
                return;
            }

            // Store the application and the map
            IApplication application = hook as IApplication;
            IMxDocument mxDocument = application.Document as IMxDocument;
            IActiveView activeView = mxDocument.FocusMap as IActiveView;
            _map = mxDocument.FocusMap as IMap;
        }

        /// <summary>
        /// Called to determine if the button should be enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Called when the button is clicked
        /// </summary>
        public override void OnClick()
        {
            // Get a list of the selected vaults
            //ISelectionSet vaults = GetSelectedVaults();
            ISelectionSet vaults = GetSelectedConduits();

            if (vaults.Count > 1)
            {
                // Figure out the orientation and rotate the map
                double orientation = GetLineOrientation(vaults);

                // Populate the map template
                PopulateTemplate(orientation);
            }
        }

        #endregion

        #region Get list of features to print

        private ISelectionSet GetSelectedVaults()
        {
            // Get the vault feature class
            IWorkspace ws = GetWorkspace();
            IFeatureClass fcVault = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure);

            // Find the layer
            IFeatureLayer vaultLayer = GetFeatureLayer(_map, fcVault);

            // Get the current selection for the vault layer
            IFeatureSelection selection = vaultLayer as IFeatureSelection;

            // Return the selection as a SelectionSet
            return selection.SelectionSet;
        }

        private ISelectionSet GetSelectedConduits()
        {
            // Get the vault feature class
            IWorkspace ws = GetWorkspace();
            IFeatureClass fcConduit = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.UFM.ClassModelNames.Conduit);

            // Find the layer

            IFeatureLayer conduitLayer = GetFeatureLayer(_map, fcConduit);
            IWorkspace ws2 = (conduitLayer.FeatureClass as IDataset).Workspace;
            ws = ws2;

            // Get the current selection for the vault layer
            IFeatureSelection selection = conduitLayer as IFeatureSelection;

            // Return the selection as a SelectionSet
            return selection.SelectionSet;
        }

        #endregion

        #region Get map orientation

        private double GetLineOrientation(ISelectionSet features)
        {
            double angle = 0;

            IList<double> angles = new List<double>();

            // For each selected feature
            ICursor cursorFeatures = null;
            features.Search(null, false, out cursorFeatures);
            int count = 0;
            IRow row = cursorFeatures.NextRow();

            while (row != null)
            {
                count++;

                // Get the feature
                IFeature feat = row as IFeature;

                // Get the vertices
                IPointCollection points = feat.Shape as IPointCollection;
                IPoint start = points.get_Point(0);
                IPoint end = points.get_Point(points.PointCount - 1);
                if (end.X < start.X)
                {
                    IPoint temp = start;
                    start = end;
                    end = temp;
                }

                // Calculate the angle between them
                double tempAngle = Math.Atan2(start.Y - end.Y, start.X - end.X) * 180.0 / Math.PI;
                angles.Add(tempAngle);
                //angle = angle + tempAngle;

                // Add to 
                row = cursorFeatures.NextRow();
            }

            int newCount = 0;
            foreach (double angle2 in angles)
            {
                if (angle == 0)
                {
                    angle = angle2;
                    newCount = 1;
                }
                else
                {
                    if (angle > 0)
                    {
                        if (angle2 < (angle * 1.1) && angle2 > (angle * 0.9))
                        {
                            newCount++;
                            angle = angle + angle2;
                        }
                    }
                    else
                    {
                        if (angle2 > (angle * 1.1) && angle2 < (angle * 0.9))
                        {
                            newCount++;
                            angle = angle + angle2;
                        }
                    }
                }
            }

            if (count > 1)
            {
                angle = angle / newCount;
            }

            return angle + 90;
        }

        private double GetOrientation(ISelectionSet features)
        {
            // For each selected feature
            ICursor cursorFeatures = null;
            features.Search(null, false, out cursorFeatures);

            IFeature lowestX = null;
            IFeature highestX = null;

            IFeature lowestY = null;
            IFeature highestY = null;

            double averageX = 0;
            double averageY = 0;
            int count = 0;
            IRow row = cursorFeatures.NextRow();

            if (row != null)
            {
                count++;
                lowestX = row as IFeature;
                highestX = row as IFeature;
                lowestY = row as IFeature;
                highestY = row as IFeature;
                row = cursorFeatures.NextRow();
                averageX = lowestX.Shape.Envelope.XMax;
                averageY = lowestY.Shape.Envelope.YMax;
                while (row != null)
                {
                    count++;

                    // Track the highest and lowest x value
                    IFeature feat = row as IFeature;
                    averageX = averageX + feat.Shape.Envelope.XMax;
                    averageY = averageY + feat.Shape.Envelope.YMax;
                    if (feat.Shape.Envelope.XMin < lowestX.Shape.Envelope.XMin)
                    {
                        lowestX = feat;
                    }
                    if (feat.Shape.Envelope.XMax > highestX.Shape.Envelope.XMax)
                    {
                        highestX = feat;
                    }

                    // Track the highest and lowest y value
                    // Track the highest and lowest x value
                    if (feat.Shape.Envelope.YMin < lowestX.Shape.Envelope.YMin)
                    {
                        lowestY = feat;
                    }
                    if (feat.Shape.Envelope.YMax > highestX.Shape.Envelope.YMax)
                    {
                        highestY = feat;
                    }

                    // Move to the next row
                    row = cursorFeatures.NextRow();
                }
            }

            averageX = averageX / count;
            averageY = averageY / count;

            double xDiff;
            double yDiff;

            // From the results, determine the biggest delta... x or y
            double deltaX = highestX.Shape.Envelope.XMax - lowestX.Shape.Envelope.XMin;
            double deltaY = highestY.Shape.Envelope.YMax - lowestY.Shape.Envelope.YMin;
            if (deltaX > deltaY)
            {
                xDiff = highestX.Shape.Envelope.XMax - lowestX.Shape.Envelope.XMin;
                yDiff = highestX.Shape.Envelope.YMax - lowestX.Shape.Envelope.YMin;
            }
            else
            {
                xDiff = highestY.Shape.Envelope.XMax - lowestY.Shape.Envelope.XMin;
                yDiff = highestY.Shape.Envelope.YMax - lowestY.Shape.Envelope.YMin;
            }

            IPointCollection averageLine = new PolylineClass();
            IPoint height = new PointClass();
            height.X = lowestX.Shape.Envelope.XMin;
            height.Y = averageY;
            averageLine.AddPoint(height);
            IPoint right = new PointClass();
            right.X = highestX.Shape.Envelope.XMax;
            right.Y = averageY;
            averageLine.AddPoint(right);
            AddGraphicToMap(averageLine as IGeometry);

            // Use the following code to visualize the line for debugging
            IPointCollection collection = new PolylineClass();
            collection.AddPoint(highestX.Shape as IPoint);
            collection.AddPoint(lowestX.Shape as IPoint);
            AddGraphicToMap(collection as IGeometry);

            // Determine the angle
            double angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

            return angle - 90;
        }

        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///      
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IGeometry interface. It can be of the geometry type: esriGeometryPoint, esriGeometryPolyline, or esriGeometryPolygon.</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For those geometry's with an outline it will be this color.</param>
        ///      
        ///<remarks>Calling this function will not automatically make the graphics appear in the map area. Refresh the map area after after calling this function with Methods like IActiveView.Refresh or IActiveView.PartialRefresh.</remarks>
        public void AddGraphicToMap(IGeometry geometry)
        {
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            IApplication arcMapApp = appRefObj as IApplication;
            IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
            IMap map = mxDoc.FocusMap;

            ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = (ESRI.ArcGIS.Carto.IGraphicsContainer)map; // Explicit Cast
            ESRI.ArcGIS.Carto.IElement element = null;

            IRgbColor color = new RgbColorClass();
            color.Blue = 200;
            color.Red = 10;
            color.Green = 10;

            if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                // Marker symbols
                ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                simpleMarkerSymbol.Color = color;
                simpleMarkerSymbol.Outline = false;
                simpleMarkerSymbol.Size = 1;
                simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;

                ESRI.ArcGIS.Carto.IMarkerElement markerElement = new ESRI.ArcGIS.Carto.MarkerElementClass();
                markerElement.Symbol = simpleMarkerSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)markerElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                //  Line elements
                ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
                simpleLineSymbol.Color = color;
                simpleLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = 1;

                ESRI.ArcGIS.Carto.ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
                lineElement.Symbol = simpleLineSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)lineElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                // Polygon elements
                ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                simpleFillSymbol.Color = color;
                simpleFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSForwardDiagonal;
                ESRI.ArcGIS.Carto.IFillShapeElement fillShapeElement = new ESRI.ArcGIS.Carto.PolygonElementClass();
                fillShapeElement.Symbol = simpleFillSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)fillShapeElement; // Explicit Cast
            }
            if (!(element == null))
            {
                element.Geometry = geometry;
                graphicsContainer.AddElement(element, 0);
            }
        }

        #endregion

        #region Populate Template

        private void PopulateTemplate(double rotation)
        {
            IApplication application = ApplicationFacade.Application;
            IPageLayout pageLayout = (application.Document as IMxDocument).PageLayout;
            IMxDocument mxDoc = (application.Document as IMxDocument);

            //// Get the active map
            //IActiveView view = mxDoc.FocusMap as IActiveView;
            //IScreenDisplay display = view.ScreenDisplay;

            //// Apply a rotation
            //display.DisplayTransformation.Rotation = rotation;

            IMaps templateMaps = mxDoc.Maps;
            if (templateMaps.Count > 1)
            {
                IMap templateMap1 = templateMaps.get_Item(0);

                // Get extent
                IActiveView view = templateMap1 as IActiveView;
                IEnvelope currExtent = view.Extent;

                IMap templateMap2 = templateMaps.get_Item(1);
                IMap templateMap3 = templateMaps.get_Item(2);
                IMap templateMap4 = templateMaps.get_Item(3);

                IWorkspace ws = GetWorkspace();
                SetMapView(ws, templateMap1, SD1, rotation, currExtent);
                SetMapView(ws, templateMap2, SD2, rotation, currExtent);
                SetMapView(ws, templateMap3, SD3, rotation, currExtent);
                SetMapView(ws, templateMap4, SD4, rotation, currExtent);
            }


            //IMMMapProductionInfo pMapProdInfo = null;
            //pMapProdInfo.PageLayo
            RefreshView();
        }

        private void SetMapView(IWorkspace ws, IMap mapView, string storedDisplay, double rotation, IEnvelope extent)
        {
            IMMStoredDisplay sd = GetStoredDisplay(ws, storedDisplay);
            sd.Open(mapView);
            IActiveView viewMap1 = mapView as IActiveView;
            viewMap1.Extent = extent;
            viewMap1.ScreenDisplay.DisplayTransformation.Rotation = rotation;
            try
            {
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
            }
        }

        private void RefreshView()
        {
            try
            {
                // Do a view refresh
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                IApplication arcMapApp = appRefObj as IApplication;
                if (arcMapApp != null)
                {
                    IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                    (mxDoc.FocusMap as IActiveView).Refresh();
                }
            }
            catch (Exception ex)
            {
                // Not the end of the world, log and move on
                _logger.Warn("Failed to refresh view: " + ex.ToString());
            }
        }

        private IMMStoredDisplay GetStoredDisplay(IWorkspace ws, string name)
        {
            IMMStoredDisplay sd = null;

            // Define the stored display we want
            IMMStoredDisplayName sdName = new MMStoredDisplayNameClass();
            sdName.Name = name;
            sdName.Type = mmStoredDisplayType.mmSDTSystem;

            // Then get it
            IMMStoredDisplayManager sdMgr = new MMStoredDisplayManagerClass();
            sdMgr.Workspace = ws;
            IMMEnumStoredDisplayName names = sdMgr.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem);
            IMMStoredDisplayName sdName2 = names.Next();
            while (sdName2 != null)
            {
                Console.WriteLine(sdName2.Name);
                if (sdName2.Name == name)
                {
                    break;
                }
                sdName2 = names.Next();
            }
            if (sdName2 != null)
            {
                sd = sdMgr.GetUnopenedStoredDisplay(sdName2);
            }

            // Return the result
            return sd;
        }

        /// <summary>
        /// Returns a workspace
        /// </summary>
        /// <returns></returns>
        private IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;
        }

        /// <summary>
        /// Returns the layer for the supplied feature class
        /// </summary>
        /// <param name="map"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private IFeatureLayer GetFeatureLayer(IMap map, IFeatureClass featureClass)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            IFeatureLayer featLayer = null;
            IEnumLayer enumLayer = null;

            try
            {
                // Get all layers in the map
                UID uid = new UID();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                enumLayer = map.get_Layers(uid, true);

                // For each layer
                ILayer layer;
                while ((layer = enumLayer.Next()) != null)
                {
                    // Check to see if the layer is for our feature class
                    featLayer = layer as IFeatureLayer;
                    if (featLayer != null)
                    {
                        if ((featLayer.FeatureClass as IDataset).Name == (featureClass as IDataset).Name)
                        {
                            // If it is, save it and bail out
                            featLayer = layer as IFeatureLayer;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Release object
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
            }

            return featLayer;
        }

        #endregion
    }
}
