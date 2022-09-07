using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Editor;
using Miner.ComCategories;

namespace PGE.BatchApplication.AllignAnnotation
{
    /// <summary>
    /// Summary description for Tool1.
    /// </summary>
    [Guid("971195b0-94c7-4491-98cb-96ee976b8d2e")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.AllignAnnotation.AnnoAllignment")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class AnnoAllignment : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
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

        private IApplication m_application;
        //private IWorkspaceEdit _WorkspaceEdit;
        public double dblang = 0.0;
        public static bool gBlnStart = false;
        public static bool gBlnAllign = false;
        public static bool gBlnPerp = false;
        public static bool gBlnJobAnno = false;

        public AnnoAllignment()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "MAP_QC_TOOLS"; //localizable text 
            base.m_caption = "AnnoAllignment";  //localizable text 
            base.m_message = "AnnoAllignment";  //localizable text
            base.m_toolTip = "AnnoAllignment";  //localizable text
            base.m_name = "AnnoAllignment";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add Tool1.OnClick implementation
        }

        public ESRI.ArcGIS.Display.IRgbColor CreateRGBColor(System.Byte myRed, System.Byte myGreen, System.Byte myBlue)
        {
            ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
            rgbColor.Red = myRed;
            rgbColor.Green = myGreen;
            rgbColor.Blue = myBlue;
            rgbColor.UseWindowsDithering = true;
            return rgbColor;
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Tool1.OnMouseDown implementation
            //Get the active view from the application object (ie. hook)
            IActiveView activeView = GetActiveViewFromArcMap(m_application);

            //Get the polyline object from the users mouse clicks
            IPolyline polyline = GetPolylineFromMouseClicks(activeView);

            //Make a color to draw the polyline 
            IRgbColor rgbColor = CreateRGBColor(0, 255, 123);

            //Add the users drawn graphics as persistent on the map
            AddGraphicToMap(activeView.FocusMap, polyline, rgbColor, rgbColor);

            //Only redraw the portion of the active view that contains the graphics 
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            Form1 frm = new Form1();
            
            if (polyline != null)
            {
                if (true == chk4JobAnno())
                    gBlnJobAnno = true;

                frm.ShowDialog();

                if (true == gBlnStart)
                {
                    Testapp(polyline);
                }
            }
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Tool1.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Tool1.OnMouseUp implementation
        }
        #endregion

        #region "Get Polyline From Mouse Clicks"

        ///<summary>
        ///Create a polyline geometry object using the RubberBand.TrackNew method when a user click the mouse on the map control. 
        ///</summary>
        ///<param name="activeView">An ESRI.ArcGIS.Carto.IActiveView interface that will user will interace with to draw a polyline.</param>
        ///<returns>An ESRI.ArcGIS.Geometry.IPolyline interface that is the polyline the user drew</returns>
        ///<remarks>Double click the left mouse button to end tracking the polyline.</remarks>
        public IPolyline GetPolylineFromMouseClicks(IActiveView activeView)
        {
           
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            IRubberBand rubberBand = new RubberLineClass();
            IGeometry geometry = rubberBand.TrackNew(screenDisplay, null);
            IPolyline polyline = (IPolyline)geometry;
            
            return polyline;

        }
        #endregion

        #region "Add Graphic to Map"

        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///      
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IGeometry interface. It can be of the geometry type: esriGeometryPoint, esriGeometryPolyline, or esriGeometryPolygon.</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For those geometry's with an outline it will be this color.</param>
        ///      
        ///<remarks>Calling this function will not automatically make the graphics appear in the map area. Refresh the map area after after calling this function with Methods like IActiveView.Refresh or IActiveView.PartialRefresh.</remarks>
        public void AddGraphicToMap(IMap map, IGeometry geometry, IRgbColor rgbColor, IRgbColor outlineRgbColor)
        {
            try
            {
                IGraphicsContainer graphicsContainer = (IGraphicsContainer)map; // Explicit Cast
                IElement element = null;
                if ((geometry.GeometryType) == esriGeometryType.esriGeometryPoint)
                {
                    // Marker symbols
                    ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    simpleMarkerSymbol.Color = rgbColor;
                    simpleMarkerSymbol.Outline = true;
                    simpleMarkerSymbol.OutlineColor = outlineRgbColor;
                    simpleMarkerSymbol.Size = 15;
                    simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;

                    IMarkerElement markerElement = new MarkerElementClass();
                    markerElement.Symbol = simpleMarkerSymbol;
                    element = (IElement)markerElement; // Explicit Cast
                }
                else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolyline)
                {
                    //  Line elements
                    ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                    simpleLineSymbol.Color = rgbColor;
                    simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    simpleLineSymbol.Width = 5;

                    ILineElement lineElement = new LineElementClass();
                    lineElement.Symbol = simpleLineSymbol;
                    element = (IElement)lineElement; // Explicit Cast
                }
                else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolygon)
                {
                    // Polygon elements
                    ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                    simpleFillSymbol.Color = rgbColor;
                    simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSForwardDiagonal;
                    IFillShapeElement fillShapeElement = new PolygonElementClass();
                    fillShapeElement.Symbol = simpleFillSymbol;
                    element = (IElement)fillShapeElement; // Explicit Cast
                }
                if (!(element == null))
                {
                    element.Geometry = geometry;
                    graphicsContainer.AddElement(element, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in AddGraphicToMap: " + ex.Message);
            }
        }
        #endregion

        #region "GetActiveViewFromArcMap"
        ///<summary>Get ActiveView from ArcMap</summary>
        ///  
        ///<param name="application">An IApplication interface that is the ArcMap application.</param>
        ///   
        ///<returns>An IActiveView interface.</returns>
        ///   
        ///<remarks></remarks>
        public IActiveView GetActiveViewFromArcMap(IApplication application)
        {
            if (application == null)
            {
                return null;
            }
            IMxDocument mxDocument = application.Document as IMxDocument; // Dynamic Cast
            IActiveView activeView = mxDocument.ActiveView;

            return activeView;
        }
        #endregion

        private bool chk4JobAnno()
        {
            IDocument pDoc = m_application.Document;
            IMxDocument pmxDoc = pDoc as IMxDocument;
            IMap pMap = pmxDoc.FocusMap;
            bool blnJob = false;

            IEnumFeature pEnumFeat = (IEnumFeature)pMap.FeatureSelection;
            pEnumFeat.Reset();

            IFeature pfeat = pEnumFeat.Next();
            while (pfeat != null)
            {
                if (pfeat.Class.AliasName == "Job History Note")
                {
                    blnJob = true;
                }
                pfeat = pEnumFeat.Next();
            }

            return blnJob;
        }

        private void Testapp(IPolyline pLine)
        {
            IWorkspace pSdeWorkspace = null;
            IDocument pDoc = m_application.Document;
            IMxDocument pmxDoc = pDoc as IMxDocument;
            IMap pMap = pmxDoc.FocusMap;
            IActiveView actview = pmxDoc.ActiveView;
            List<IFeature> pointFeatLst = new List<IFeature>();
            List<IFeature> AnnoLst = new List<IFeature>();
            List<IFeature> JobNumberAnno100Lst = new List<IFeature>();
            List<IFeature> JobNumberAnno50Lst = new List<IFeature>();

            IEnumFeature pEnumFeat = (IEnumFeature)pMap.FeatureSelection;

            pEnumFeat.Reset();

            IFeature pSelFeature = pEnumFeat.Next();

            IFeatureClass pSelFClass = (IFeatureClass)pSelFeature.Class;

            pSdeWorkspace = ((IDataset)pSelFClass).Workspace;

            if (pSdeWorkspace == null)
            {
                MessageBox.Show("Unable to get workspace.");
            }
            else
            {
               

                UID editorUID = new UID();
                editorUID.Value = "esriEditor.Editor";
                IEditor editor = (IEditor)m_application.FindExtensionByCLSID(editorUID);

                try
                {
                    pEnumFeat.Reset();
                    IFeature pfeat = pEnumFeat.Next();
                    while (pfeat != null)
                    {
                        if (pfeat.Class.AliasName == "Job History Note")
                        {
                            pointFeatLst.Add(pfeat);
                        }
                        else if (pfeat.Class.AliasName.Contains("EDGIS.JobNumberAnno"))
                        {
                            JobNumberAnno100Lst.Add(pfeat);
                        }
                        else if (pfeat.Class.AliasName.Contains("EDGIS.JobNumber50Anno"))
                        {
                            JobNumberAnno50Lst.Add(pfeat);
                        }
                        else if (pfeat.Class.AliasName.Contains("Anno"))
                        {
                            AnnoLst.Add(pfeat);
                        }

                        pfeat = pEnumFeat.Next();
                    }

                    if (pLine != null)
                    {
                        double dblLnAngle = GetAngleBetweenPoints(pLine.FromPoint, pLine.ToPoint);

                        if (dblLnAngle > 0)
                        {
                            dblLnAngle = dblLnAngle + 90;
                            if (dblLnAngle > 360)
                            {
                                dblLnAngle = dblLnAngle -  360;
                            }
                        }
                        else
                        {
                            dblLnAngle = dblLnAngle - 90;
                            if (dblLnAngle < -360)
                            {
                                dblLnAngle = dblLnAngle + 360;
                            }
                        }

                        if (dblLnAngle > 95 && dblLnAngle < 180)
                                dblLnAngle = dblLnAngle + 180;
                        if (dblLnAngle >=180 && dblLnAngle < 275)
                                dblLnAngle = dblLnAngle - 180;
                        if (dblLnAngle <-85 && dblLnAngle >= -180)
                                dblLnAngle = dblLnAngle + 180;
                        if (dblLnAngle < -180 && dblLnAngle > -265)
                                dblLnAngle = dblLnAngle + 180;

                        if (true == gBlnPerp)
                        {
                            RotateFeatures(pointFeatLst, AnnoLst, pLine, actview, dblLnAngle);
                        }

                        if (pointFeatLst.Count > 0)
                        {
                            //AlignJobFeatures(pointFeatLst, AnnoLst, pLine, actview, dblLnAngle);
                            if (JobNumberAnno100Lst.Count > 0)
                            {
                                if (editor.EditState == esriEditState.esriStateEditing)
                                {
                                    editor.StartOperation();
                                    AlignJobFeatures(pointFeatLst, JobNumberAnno100Lst, pLine, 13.0, 12.0, actview, dblLnAngle);
                                    editor.StopOperation("Allign Anno");

                                }
                                else
                                {
                                    MessageBox.Show("Session required active edit session");
                                }
                            }
                            else if (JobNumberAnno50Lst.Count > 0)
                            {
                                if (editor.EditState == esriEditState.esriStateEditing)
                                {
                                    editor.StartOperation();
                                    AlignJobFeatures(pointFeatLst, JobNumberAnno50Lst, pLine, 6.5, 6.0, actview, dblLnAngle);
                                    editor.StopOperation("Allign Anno");

                                }
                                else
                                {
                                    MessageBox.Show("Session required active edit session");
                                }
                            }
                        }
                        else if (AnnoLst.Count > 0)
                        {
                             if (editor.EditState == esriEditState.esriStateEditing)
                                {
                                    editor.StartOperation();
                                    AlignFeatures(pointFeatLst, AnnoLst, pLine, actview, dblLnAngle);
                                    editor.StopOperation("Allign Anno");

                                }
                             else
                             {
                                 MessageBox.Show("Session required active edit session");
                             }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                
            }

            IGraphicsContainer igc = (IGraphicsContainer)pmxDoc.ActiveView.GraphicsContainer;
            igc.DeleteAllElements();

            gBlnAllign = false;
            gBlnPerp = false;
            gBlnJobAnno = false;

            pMap.ClearSelection();
            pmxDoc.ActiveView.Refresh();
            Application.DoEvents();
        }

        private void AlignJobFeatures(List<IFeature> lstPoints, List<IFeature> AnnoHistoryFeatLst, IPolyline pline, double distPointOffset, double distAnnoPointOffset, IActiveView activeView, double dblLineAngle)
        {
            if (lstPoints.Count > 0)
            {
                bool bAsRatio = true;
                string strmsg = string.Empty;
                double distanceOnCurve = 0;
                double nearestDistance = 0;
                bool isRightSide = false;
                double distOld = 0;
                IScreenDisplay screenDisplay = activeView.ScreenDisplay;
                IPoint pNewPnt = new PointClass();
                bool okGo = false;
                SortedList<double, IFeature> sortLSt = new SortedList<double, IFeature>();

                // Add the point features along with the distance from the start point of the line.
                foreach (IFeature pfeat in lstPoints)
                {
                    IPoint ptProject = null;
                    IGeometry igem = pfeat.Shape;
                    ptProject = (IPoint)igem;
                    nearestDistance = GetDistance(ptProject, pline.FromPoint);
                    ICurve2 pCurve = (ICurve2)pline;
                    sortLSt.Add(nearestDistance, pfeat);
                }

                // Add the point features to the list based on the distance from start point by sorting.
                List<IFeature> sorteLstFromDist = new List<IFeature>();
                foreach (KeyValuePair<double, IFeature> pfeat in sortLSt)
                {
                    sorteLstFromDist.Add(pfeat.Value);
                }

                foreach (IFeature pfeat in sorteLstFromDist)
                {
                    IPoint ptProject = null;
                    IPoint targetPoint = new PointClass();
                    IGeometry igem = pfeat.Shape;
                    ptProject = (IPoint)igem;
                    ICurve2 pCurve = (ICurve2)pline;
                    if (okGo == false)
                    {
                        pCurve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, ptProject, bAsRatio, pNewPnt, ref distanceOnCurve, ref nearestDistance, ref isRightSide);
                        okGo = true;
                    }

                    IPoint pNewPnt2 = new PointClass();
                    if (pNewPnt != null)
                    {
                        if (distOld == 0)
                        {
                            distanceOnCurve = GetDistance(pNewPnt, pline.FromPoint);
                            distOld = distanceOnCurve;
                            pfeat.Shape = pNewPnt;
                            targetPoint = pNewPnt;

                            ILine AngLine = new Line();
                            AngLine.ToPoint = ptProject;
                            AngLine.FromPoint = pNewPnt; 
                            dblang = (AngLine.Angle * 180) / Math.PI;
                            //MessageBox.Show(dblang.ToString());
                        }
                        else
                        {
                            distOld = distOld + distPointOffset;
                            pCurve.QueryPoint(esriSegmentExtension.esriNoExtension, distOld, false, pNewPnt2);
                            if (pNewPnt2 != null)
                            {
                                pfeat.Shape = pNewPnt2;
                                targetPoint = pNewPnt2;
                            }
                        }

                        pfeat.Store();

                        List<IFeature> ModAnnoLst = new List<IFeature>();

                        foreach (IFeature annoFeat in AnnoHistoryFeatLst)
                        {
                            string Val = annoFeat.get_Value(annoFeat.Fields.FindField("FEATUREID")).ToString();
                            if (pfeat.get_Value(pfeat.Fields.FindField("OBJECTID")).ToString() == Val)
                            {
                                IAnnotationFeature2 pAnnoFeature;
                                IElement pAnnoElement;
                                pAnnoFeature = (IAnnotationFeature2)annoFeat;
                                pAnnoElement = pAnnoFeature.Annotation;

                                ITransform2D transform2d;
                                transform2d = (ITransform2D)pAnnoElement;
                                IPolygon annopoly = new PolygonClass();
                                pAnnoElement.QueryOutline(screenDisplay, annopoly);

                                IPointCollection pointColl = (IPointCollection)annopoly;
                                IPoint nearestAnnoPoint = new PointClass();

                                IPoint pnt1 = pointColl.get_Point(0);
                                IPoint pnt2 = pointColl.get_Point(1);
                                IPoint pnt3 = pointColl.get_Point(2);
                                IPoint pnt4 = pointColl.get_Point(3);

                                IPoint pTempPoint = new PointClass();
                                pTempPoint.X = (pnt1.X + pnt2.X) / 2;
                                pTempPoint.Y = (pnt1.Y + pnt2.Y) / 2;

                                ILine vecLine = new Line();
                                vecLine.ToPoint = targetPoint;
                                vecLine.FromPoint = pTempPoint;
                                transform2d.MoveVector(vecLine);

                                pAnnoFeature.Annotation = pAnnoElement;
                                annoFeat.Store();

                                try
                                {
                                    //pAnnoFeature = (IAnnotationFeature2)annoFeat;
                                    //pAnnoElement = pAnnoFeature.Annotation;
                                    pAnnoElement.QueryOutline(screenDisplay, annopoly);

                                    pointColl = (IPointCollection)annopoly;

                                    pnt1 = pointColl.get_Point(0);
                                    pnt2 = pointColl.get_Point(1);
                                    pnt3 = pointColl.get_Point(2);
                                    pnt4 = pointColl.get_Point(3);

                                    pTempPoint = new PointClass();
                                    pTempPoint.X = (pnt3.X + pnt4.X) / 2;
                                    pTempPoint.Y = (pnt3.Y + pnt4.Y) / 2;

                                    IPoint nearestPoint = new PointClass();
                                    ILine pTempLine = new LineClass();
                                    pTempLine.PutCoords(targetPoint, pTempPoint);
                                    ICurve circularArc = (ICurve)pTempLine;

                                    circularArc.QueryPoint(esriSegmentExtension.esriNoExtension, distAnnoPointOffset, false, nearestPoint);

                                    vecLine = new Line();
                                    vecLine.ToPoint = nearestPoint;
                                    vecLine.FromPoint = targetPoint;
                                    transform2d.MoveVector(vecLine);

                                    pAnnoFeature.Annotation = pAnnoElement;

                                    annoFeat.Store();

                                    ModAnnoLst.Add(annoFeat);
                                }
                                catch (Exception Ex)
                                {
                                    MessageBox.Show(Ex.Message);
                                }

                                break;
                            }

                        }
                     
                    }
                }
            }
        }
        private double GetDistance(IPoint p1, IPoint p2)
        {
            return ((IProximityOperator)p1).ReturnDistance(p2);
        }

        private void MoveAnnotationToSpecificPoint(IFeature pInptFeat, IPoint pTrgtPnt)
        {
            IAnnotationFeature pAnnoFeature = null;
            IElement pElemnt = null;
            ITextElement pTextElement = null;
            ITextSymbol pTextSymbol = null;
            IPoint pPnt = new PointClass();
            IPoint pNewPnt = new PointClass();
            IGeometry pNewGeom = null;
            try
            {
                pAnnoFeature = pInptFeat as IAnnotationFeature;
                pElemnt = pAnnoFeature.Annotation;

                if (pElemnt.Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    pPnt = pElemnt.Geometry as IPoint;
                    pTextElement = pElemnt as ITextElement;
                    pTextSymbol = pTextElement.Symbol;
                    pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft;
                    pTextSymbol.VerticalAlignment = esriTextVerticalAlignment.esriTVACenter;
                    pTextElement.Symbol = pTextSymbol;
                    pAnnoFeature.Annotation = pTextElement as IElement;
                    pElemnt = pTextElement as IElement;
                    pNewPnt = pElemnt.Geometry as IPoint;
                    pInptFeat = pAnnoFeature as IFeature;
                    pInptFeat.Store();

                    pNewGeom = pInptFeat.Shape;
                    pElemnt = pAnnoFeature.Annotation;
                    pNewPnt.PutCoords(pPnt.X + pTrgtPnt.X, pPnt.Y + pTrgtPnt.Y);

                    pElemnt.Geometry = pNewPnt as IGeometry;
                    pAnnoFeature.Annotation = pElemnt;

                    pInptFeat = pAnnoFeature as IFeature;
                    pInptFeat.Store();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in MoveAnnotationToSpecificPoint: " + ex.Message);
            }
        }

        private void RotateFeatures(List<IFeature> lstPoints, List<IFeature> AnnoFeatLst, IPolyline pline, IActiveView activeView, double dblAng)
        {
            IGeometry geometryBag = new GeometryBagClass();
            IGeometryCollection geometryCollection = geometryBag as IGeometryCollection;
            IPoint pOrgPnt = new PointClass();
            ISet pset = new SetClass();
            IFeatureEdit pFeatureEdit = null;
            double dblAnnoAng = 0.0, dblRotAng = 0.0;

            foreach (IFeature annoFeat in AnnoFeatLst)
            {
                try
                {
                    object missing = Type.Missing;
                    geometryCollection.AddGeometry(annoFeat.Shape, ref missing, ref missing);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("RotateFeatures" + Ex.Message);
                }
            }

            try
            {
                ITopologicalOperator unionedPolygon = new PolygonClass();
                unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);

                IPolygon pTotFeatShape = (IPolygon)unionedPolygon;

                ILine vecLine = new Line();
                vecLine.FromPoint = pTotFeatShape.Envelope.LowerLeft;
                vecLine.ToPoint = pTotFeatShape.Envelope.UpperRight;
                double dblLength = vecLine.Length / 2;

                ICurve2 pCurve = (ICurve2)vecLine;
                pCurve.QueryPoint(esriSegmentExtension.esriNoExtension, dblLength, false, pOrgPnt);

                foreach (IFeature annoFeat in AnnoFeatLst)
                {
                    int intAngleIdx = annoFeat.Fields.FindField("ANGLE");
                    dblAnnoAng = Convert.ToDouble(annoFeat.get_Value(intAngleIdx));
                    pset.Add(annoFeat);
                    pFeatureEdit = (IFeatureEdit)annoFeat;
                }               

                dblRotAng = dblAng - dblAnnoAng;
                dblRotAng = (dblRotAng * Math.PI) / 180;
                pFeatureEdit.RotateSet(pset, pOrgPnt, dblRotAng);

                foreach (IFeature annoFeat in AnnoFeatLst)
                {
                    int intAngleIdx = annoFeat.Fields.FindField("ANGLE");
                    dblAnnoAng = Convert.ToDouble(annoFeat.get_Value(intAngleIdx));

                    if (dblAnnoAng > 95 && dblAnnoAng < 180)
                        dblAnnoAng = dblAnnoAng + 180;
                    if (dblAnnoAng >= 180 && dblAnnoAng < 275)
                        dblAnnoAng = dblAnnoAng - 180;
                    if (dblAnnoAng < -85 && dblAnnoAng >= -180)
                        dblAnnoAng = dblAnnoAng + 180;
                    if (dblAnnoAng < -180 && dblAnnoAng > -265)
                        dblAnnoAng = dblAnnoAng + 180;

                    annoFeat.set_Value(intAngleIdx, dblAnnoAng);
                    annoFeat.Store();
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show("RotateFeatures" + Ex.Message);
            }
        }

     
        private void AlignFeatures(List<IFeature> lstPoints, List<IFeature> AnnoFeatLst, IPolyline pline, IActiveView activeView, double dblAng)
        {
            bool bAsRatio = true;
            string strmsg = string.Empty;
            double distanceOnCurve = 0;
            double nearestDistance = 0;
            bool isRightSide = false;
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;
            IPoint pNewPnt = new PointClass();
            //bool okGo = false;
            if (lstPoints.Count > 0)
            {
                //ICurve2 pCurve = (ICurve2)pline;
            }
            else if (AnnoFeatLst.Count > 0)
            {
                foreach (IFeature annoFeat in AnnoFeatLst)
                {
                    try
                    {
                        //TO CHANGE THE JUSTIFICATION
                        int intJust = 0, intAlgnIdx = 0;    //, intAngleIdx = 0;
                        intAlgnIdx = annoFeat.Fields.FindField("HORIZONTALALIGNMENT");
                        intJust = Convert.ToInt32(annoFeat.get_Value(intAlgnIdx));
                        if (true == gBlnAllign && intAlgnIdx > 0)
                        {
                            if (0 == intJust)
                            {
                                annoFeat.set_Value(intAlgnIdx, 2);
                            }
                            else
                            {
                                annoFeat.set_Value(intAlgnIdx, 0);
                            }
                            annoFeat.Store();
                        }
                        intJust = Convert.ToInt32(annoFeat.get_Value(intAlgnIdx));

                        IAnnotationFeature2 pAnnoFeature;
                        IElement pAnnoElement;
                        pAnnoFeature = (IAnnotationFeature2)annoFeat;
                        pAnnoElement = pAnnoFeature.Annotation;

                        ITransform2D transform2d;
                        transform2d = (ITransform2D)pAnnoElement;
                        IPolygon annopoly = new PolygonClass();
                        pAnnoElement.QueryOutline(screenDisplay, annopoly);

                        IPointCollection pointColl = (IPointCollection)annopoly;
                        IPoint nearestAnnoPoint = new PointClass();

                        IPoint pnt1 = pointColl.get_Point(0);
                        IPoint pnt2 = pointColl.get_Point(1);
                        IPoint pnt3 = pointColl.get_Point(2);
                        IPoint pnt4 = pointColl.get_Point(3);

                        IPoint pTempPoint = new PointClass();

                        if (0 == intJust)
                        {
                            pTempPoint.X = (pnt1.X + pnt2.X) / 2;
                            pTempPoint.Y = (pnt1.Y + pnt2.Y) / 2;
                        }
                        else
                        {
                            pTempPoint.X = (pnt3.X + pnt4.X) / 2;
                            pTempPoint.Y = (pnt3.Y + pnt4.Y) / 2;
                        }

                        ICurve2 pCurve = (ICurve2)pline;
                        pCurve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, pTempPoint, bAsRatio, pNewPnt, ref distanceOnCurve, ref nearestDistance, ref isRightSide);

                        ILine vecLine = new Line();
                        vecLine.ToPoint = pNewPnt;
                        vecLine.FromPoint = pTempPoint;
                        transform2d.MoveVector(vecLine);

                        pAnnoFeature.Annotation = pAnnoElement;
                        annoFeat.Store();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("AlignFeatures" + Ex.Message);
                    }
                }
            }
        }

        internal static double GetAngleBetweenPoints(IPoint pStrtPnt, IPoint pEndPnt)
        {
            double x1 = pStrtPnt.X;
            double y1 = pStrtPnt.Y;
            double x2 = pEndPnt.X;
            double y2 = pEndPnt.Y;
            double pxRes = x2 - x1;
            double pyRes = y2 - y1;
            double angle = 0.0;
            // Calculate the angle 

            try
            {
                if (pxRes == 0.0)
                {
                    if (pyRes == 0.0)
                        angle = 0.0;
                    else if (pyRes > 0.0) angle = System.Math.PI / 2.0;
                    else

                        angle = System.Math.PI * 3.0 / 2.0;
                }
                else if (pyRes == 0.0)
                {
                    if (pxRes > 0.0)
                        angle = 0.0;
                    else
                        angle = System.Math.PI;
                }
                else
                {
                    if (pxRes < 0.0)
                        angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;

                    else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
                    else

                        angle = System.Math.Atan(pyRes / pxRes);
                }

                // Convert to degrees 
                angle = angle * 180 / System.Math.PI;
               
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetAngleBetweenPoints" + Ex.Message);
            }

            return angle;
        }
    }
}
