using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PGE.BatchApplication.DLMTools.Utility.Annotation;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite
{
    /// <summary>
    /// Summary description for ReplaceAnnotation.
    /// </summary>
    [Guid("7B3A035A-7FF1-4479-A8A3-8F9DC4DC6909")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.ReplaceAnnotation")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class ReplaceAnnotation : BaseTool
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

        private List<IFeatureLayer> AnnoFeatLayers = new List<IFeatureLayer>();
        private IApplication m_application;
        private IMxDocument mxDoc = null;
        private IPoint RotateAnglePoint = null;
        private IEditor pEditor = null;
        private IPoint point1 = null;
        private IPoint point2 = null;
        private int annoClassID = -1;
        private IPoint newPlacementPoint = null;
        private double XOffset = 0.0;
        private double YOffset = 0.0;
        private IFeature selectedFeature = null;

        public ReplaceAnnotation()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Conversion Tools"; //localizable text 
            base.m_caption = "Replace Annotation";  //localizable text 
            base.m_message = "Replace Annotation";  //localizable text
            base.m_toolTip = "Replace Annotation";  //localizable text
            base.m_name = "Replace Annotation";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //

                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.ReplaceAnnotation.bmp"));
                base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.DLMTools.Cursors.ReplaceAnnotation.cur"));
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
            {
                base.m_enabled = true;
                mxDoc = m_application.Document as IMxDocument;
            }
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            
        }

        
        /// <summary>
        /// When the user clicks down, if they have already selected the annotation object, a rubber band will draw
        /// from the first click to specify the new location, then the second click to specify the angle, and then a
        /// popup for the user to click and specify the horizontal alignment.
        /// </summary>
        /// <param name="Button"></param>
        /// <param name="Shift"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (selectedFeature == null) { return; }

            //Begin drawing out line for the user to guage their angle
            IPolyline polyLine = GetPolylineFromMouseClicks(mxDoc.ActiveView);
            IPointCollection pointCollection = polyLine as IPointCollection;
            point1 = pointCollection.get_Point(0);
            point2 = pointCollection.get_Point(1);

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            menuStrip.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip_ItemClicked);
            menuStrip.Items.Add("Left Aligned");
            menuStrip.Items.Add("Right Aligned");
            menuStrip.Items.Add("Center Aligned");
            MouseCursor cursor = new MouseCursorClass();
            menuStrip.Show(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

        }
        
        public override bool Deactivate()
        {
            selectedFeature = null;
            return base.Deactivate();
        }

        /// <summary>
        /// Event to handle when the user choses a horizontal alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int alignment = 0;

                if (e.ClickedItem.ToString() == "Center Aligned") { alignment = 1; }
                else if (e.ClickedItem.ToString() == "Right Aligned") { alignment = 2; }

                newPlacementPoint = point1;

                //Calculate the angle
                double dy = point2.Y - point1.Y;
                double dx = point2.X - point1.X;
                double theta = 0.0;
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

                annoClassID = Convert.ToInt32(selectedFeature.get_Value(selectedFeature.Fields.FindField("ANNOTATIONCLASSID")));


                pEditor.StartOperation();

                IEnumRelationshipClass relClasses = selectedFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass rel = relClasses.Next();
                //Loop through and delete.
                if (rel != null)
                {
                    IObject parentObject = AnnotationFacade.FindAnnoParent(selectedFeature, rel);
                    if (parentObject == null)
                    {
                        MessageBox.Show("Could not find parent feature for this object.");
                        return;
                    }
                    AnnotationFacade.RecreateAnno(parentObject, rel);
                    AnnotationFacade.ApplySettingsToNewAnno(rel, parentObject, alignment, theta, annoClassID, newPlacementPoint);

                    pEditor.StopOperation("Recreate Annotation");

                    mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
            catch (Exception ex)
            {
                pEditor.AbortOperation();
                MessageBox.Show("Error during execution: " + ex.Message);
            }

            //Clear our selected feature now as we are finished with it and need to prep for next run
            selectedFeature = null;
        }

        /// <summary>
        /// Method that will draw the rubber band for the user to specify the angle
        /// </summary>
        /// <param name="activeView">IActiveView of the focusmap</param>
        /// <returns></returns>
        private IPolyline GetPolylineFromMouseClicks(IActiveView activeView)
        {
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

            IRubberBand rubberBand = new RubberLineClass();
            IGeometry geometry = rubberBand.TrackNew(screenDisplay, null);

            IPolyline polyline = (IPolyline)geometry;

            return polyline;
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (selectedFeature == null)
            {
                if (pEditor == null)
                {
                    UID pID = new UIDClass();
                    pID.Value = "esriEditor.Editor";
                    pEditor = m_application.FindExtensionByCLSID(pID) as IEditor;
                }
                if (pEditor.EditState == esriEditState.esriStateNotEditing)
                {
                    MessageBox.Show("Must be editing");
                    return;
                }

                if (AnnoFeatLayers.Count < 1)
                {
                    //Get all of the IFeatureLayers
                    ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                    uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                    IEnumLayer featLayers = mxDoc.FocusMap.get_Layers(uid);
                    IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                    while (featLayer != null)
                    {
                        if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            AnnoFeatLayers.Add(featLayer);
                        }
                        featLayer = featLayers.Next() as IFeatureLayer;
                    }
                }

                ESRI.ArcGIS.ArcMapUI.IMxApplication mxApp = m_application as IMxApplication;
                ESRI.ArcGIS.Display.IAppDisplay appDisplay;
                IScreenDisplay screenDisplay;
                IDisplay display;
                IDisplayTransformation transform;

                appDisplay = mxApp.Display;

                if (appDisplay == null)
                    return;

                screenDisplay = appDisplay.FocusScreen;
                display = screenDisplay;

                if (display == null)
                    return;

                transform = display.DisplayTransformation;

                if (transform == null)
                    return;


                IPoint geom = transform.ToMapPoint(X, Y);

                foreach (IFeatureLayer featLayer in AnnoFeatLayers)
                {
                    IIdentify identify = featLayer as IIdentify;
                    IArray identified = identify.Identify(geom);
                    if (identified != null && identified.Count > 0)
                    {
                        IFeatureIdentifyObj identifiedObject = identified.get_Element(0) as IFeatureIdentifyObj;
                        IRowIdentifyObject identifyObject = identifiedObject as IRowIdentifyObject;
                        int oid = identifyObject.Row.OID;
                        selectedFeature = featLayer.FeatureClass.GetFeature(oid);
                        if (selectedFeature != null)
                        {
                            IIdentifyObj flashObj = identifiedObject as IIdentifyObj;
                            flashObj.Flash(mxDoc.ActiveView.ScreenDisplay);
                            break;
                        }
                    }
                }
            }
            else
            {

                //Clear our selected feature now as we are finished with it and need to prep for next run
                selectedFeature = null;
            }
        }
        #endregion
    }
}
