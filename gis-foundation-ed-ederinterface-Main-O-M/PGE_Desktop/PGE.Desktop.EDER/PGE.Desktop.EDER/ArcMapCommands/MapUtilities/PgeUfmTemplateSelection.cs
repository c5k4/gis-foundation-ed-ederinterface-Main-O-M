using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.MapUtilities
{
    [ComVisible(true)]
    [Guid("AF67255B-D7F2-4F15-B3A7-5574DBDD6565")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PgeUfmTemplateSelection")]
    public class PgeUfmTemplateSelection: BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication _mApplication;

        private const double XDim = 250;
        private const double YDim = 125;
        private const int TargetMapScale = 600;
        private const string Sd1 = "UFM Block Map View (1:50)";
        private const string Sd2 = "UFM Pri UG Duct View (1:50)";
        private const string Sd3 = "UFM Sec UG Duct View (1:50)";
        private const string Sd4 = "UFM DC Duct View (1:50)";

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public PgeUfmTemplateSelection()
        {
            m_category = "PGE Tools"; //localizable text 
            m_caption = "UFM Template Selection";  //localizable text 
            m_message = "UFM Template Selection";  //localizable text
            m_toolTip = "Used to select the area to be printed using the Block Map template";  //localizable text
            m_name = "UfmTmpl_Select";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                string path = GetType().Assembly.GetName().Name + ".ArcMapCommands.MapUtilities." + GetType().Name + ".bmp";
                m_bitmap =
                    new Bitmap(
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream(path));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        public override void OnCreate(object hook)
        {
            if (hook == null) return;

            _mApplication = (IApplication)hook;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                m_enabled = true;
            }
            else
                m_enabled = false;
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override void OnMouseDown(int button, int shift, int deviceX, int deviceY)
        {
            IScreenDisplay disp = ((IMxDocument)_mApplication.Document).ActiveView.ScreenDisplay;
            IPointCollection points = (IPointCollection)new RubberLineClass().TrackNew(disp, null);
            if (points.PointCount < 2) return;

            double rotation = GetTheta(points.Point[0], points.Point[1]);
            IPointCollection rectanglePts = DisplayRectangleAtLocation(points.Point[0], rotation);

            DialogResult dialogResult = MessageBox.Show(@"Is this the selection you'd like?", @"Selection Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                ((IMxDocument) _mApplication.Document).ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewForeground, null, null);
                return;
            }

            PopulateTemplate(-rotation, ((IPolygon)rectanglePts).Envelope);

            IMxDocument mxDoc = (IMxDocument) _mApplication.Document;
            mxDoc.ActiveView = (IActiveView)mxDoc.PageLayout;
        }

        private void PopulateTemplate(double rotation, IEnvelope targetExtent)
        {
            IMxDocument mxDoc = (IMxDocument)_mApplication.Document;

            IMaps templateMaps = mxDoc.Maps;

            while (templateMaps.Count < 4)
            {
                templateMaps.Add(new MapClass());
            }
            
            IMap templateMap1 = templateMaps.Item[0];
            IMap templateMap2 = templateMaps.Item[1];
            IMap templateMap3 = templateMaps.Item[2];
            IMap templateMap4 = templateMaps.Item[3];

            IWorkspace ws = GetWorkspace();
            SetMapView(ws, templateMap1, Sd1, rotation, targetExtent);
            SetMapView(ws, templateMap2, Sd2, rotation, targetExtent);
            SetMapView(ws, templateMap3, Sd3, rotation, targetExtent);
            SetMapView(ws, templateMap4, Sd4, rotation, targetExtent);


            //((IMxDocument)_mApplication.Document).ActiveView.Refresh();
        }

        /// <summary>
        /// Returns a workspace
        /// </summary>
        /// <returns></returns>
        private IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            //Logger.Debug("Entered " + name);

            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;
        }

        private void SetMapView(IWorkspace ws, IMap mapView, string storedDisplay, double rotation, IEnvelope extent)
        {
            IMMStoredDisplay sd = GetStoredDisplay(ws, storedDisplay);
            sd.Open(mapView);
            IActiveView viewMap1 = (IActiveView) mapView;
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

        private IPointCollection DisplayRectangleAtLocation(IPoint mapLoc, double theta)
        {
            IActiveView view = ((IMxDocument)_mApplication.Document).ActiveView;

            double x = mapLoc.X;
            double y = mapLoc.Y;
            double scaledXDim = XDim*TargetMapScale/view.FocusMap.MapScale;
            double scaledYDim = YDim*TargetMapScale/view.FocusMap.MapScale;

            IPointCollection rectPoints = new PolygonClass();

            rectPoints.AddPoint(CreatePoint(x - scaledXDim / 2, y - scaledYDim / 2));
            rectPoints.AddPoint(CreatePoint(x - scaledXDim / 2, y + scaledYDim / 2));
            rectPoints.AddPoint(CreatePoint(x + scaledXDim / 2, y + scaledYDim / 2));
            rectPoints.AddPoint(CreatePoint(x + scaledXDim / 2, y - scaledYDim / 2));

            for (int i = 0; i < rectPoints.PointCount; i++)
            {
                IPoint currPoint = rectPoints.Point[0];
                IPoint resultPoint = RotatePointAboutPoint(currPoint, mapLoc, theta);
                rectPoints.RemovePoints(0, 1);
                rectPoints.AddPoint(resultPoint);
            }

            IGeometry rectPoly = (IPolygon)rectPoints;

            ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass
            {
                Style = esriSimpleLineStyle.esriSLSSolid,
                Color = new RgbColorClass { Red = 0, Green = 0, Blue = 0 }
            };

            ISimpleFillSymbol fillSymbol = new SimpleFillSymbolClass
            {
                Style = esriSimpleFillStyle.esriSFSSolid,
                Outline = lineSymbol,
                Color = new RgbColorClass { Red = 255, Green = 0, Blue = 0 }
            };

            IScreenDisplay screenDisplay = view.ScreenDisplay;
            ITransparencyDisplayFilter transparencyDisplayFilter = new TransparencyDisplayFilterClass
            {
                Transparency = 100
            };

            try
            {
                screenDisplay.StartDrawing(0, (short)esriScreenCache.esriNoScreenCache);
                screenDisplay.Filter = transparencyDisplayFilter;
                screenDisplay.SetSymbol((ISymbol)fillSymbol);
                screenDisplay.DrawPolygon(rectPoly);
                //screenDisplay.DrawRectangle(rectPoly);
            }
            finally
            {
                screenDisplay.FinishDrawing();
            }

            return rectPoints;
        }

        private static double GetTheta(IPoint a, IPoint b)
        {
            double dy = b.Y - a.Y;
            double dx = b.X - a.X;
            double theta;
            if (dx != 0)
            {
                double tan = dy / dx;
                theta = Math.Atan(tan);
                theta *= 180 / Math.PI;
            }
            else
            {
                theta = 90;
            }

            return theta;
        }

        public static IPoint RotatePointAboutPoint(IPoint toRotate, IPoint aboutPoint, double deg)
        {
            IPoint newPt = new PointClass();
            newPt.X = toRotate.X;
            newPt.Y = toRotate.Y;

            ITransform2D transform2D = newPt as ITransform2D;
            double radianAngle = (Math.PI / 180.0) * deg;
            transform2D.Rotate(aboutPoint, radianAngle);

            return transform2D as IPoint;
        }

        private static IPoint CreatePoint(double x, double y)
        {
            IPoint newPt = new PointClass();
            newPt.X = x;
            newPt.Y = y;

            return newPt;
        }
    }
}
