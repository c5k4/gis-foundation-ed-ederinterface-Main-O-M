using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PGE.BatchApplication.DLMTools.Utility;
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
    [Guid("7D7866B4-C2FB-498D-8F1E-6405CC6B94AE")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.DLMTools.DeviceGroupAnnoReplacement")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class DeviceGroupAnnoReplacement : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
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
        private IMxDocument _mxDoc;
        private IFeature _selectedFeature;
        private IWorkspaceEdit _editingWksp;
        private IEditor _editor;
        private bool _initialized;

        //Order of anno placement by fc id: 1005 is switch, 1001 is transformer
        private readonly int[] _fcAnnoOrder = {1005, 1001};
        private readonly IDictionary<int, IFeatureLayer> _featLayers;
        private const double BufferWidth = 2.5;
        private const string AnnoClassNameDelete = "Job";

        
        public DeviceGroupAnnoReplacement()
        {
            m_category = "PGE Conversion Tools"; //localizable text 
            m_caption = "Device Group Annotation Replacement";  //localizable text 
            m_message = "Device Group Annotation Replacement";  //localizable text
            m_toolTip = "Device Group Annotation Replacement";  //localizable text
            m_name = "DvcGrp_AnnoRepl";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.DLMTools.Bitmaps.DeviceGroupAnno.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }

            _featLayers = new Dictionary<int, IFeatureLayer>();
            _initialized = false;
        }

        #region Overridden Class Methods,

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            _mApplication = (IApplication) hook;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                m_enabled = true;
                _mxDoc = _mApplication.Document as IMxDocument;
            }
            else
                m_enabled = false;
        }

        private void Initialize()
        {
            if (_editor == null)
            {
                //Get a reference to the editor.
                UID editorUid = new UIDClass();
                editorUid.Value = "esriEditor.Editor";
                _editor = (IEditor)_mApplication.FindExtensionByCLSID(editorUid);
            }

            if (_featLayers.Count == 0)
            {
                //Get all of the IFeatureLayers
                UID uid = new UIDClass();
                uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                IEnumLayer featLayers = _mxDoc.FocusMap.Layers[uid];

                for (IFeatureLayer featLayer = (IFeatureLayer)featLayers.Next();
                    featLayer != null;
                    featLayer = (IFeatureLayer)featLayers.Next())
                {
                    _featLayers[featLayer.FeatureClass.FeatureClassID] = featLayer;
                }
            }

            _initialized = true;
        }

        /// <summary>
        /// When the user clicks down, if they have already selected the annotation object, a rubber band will draw
        /// from the first click to specify the new location, then the second click to specify the angle, and then a
        /// popup for the user to click and specify the horizontal alignment.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void OnMouseDown(int button, int shift, int x, int y)
        {
            if (!_initialized) {Initialize();}

            //Check to see if a workspace is already being edited.
            if (_editor.EditState == esriEditState.esriStateNotEditing) {MessageBox.Show("Must be editing"); return;}

            IPoint firstClickPoint = GetMapPointFromDevicePoint(x, y);
            IFeatureLayer deviceGroupLayer =
                ModelNameFacade.FeatureLayerByModelName(ModelNames.DeviceGroup, _mxDoc.FocusMap, false)[0];
            IFeature deviceGroupFeature = GetFeatureNearPoint(firstClickPoint, 4, ModelNames.DeviceGroup,
                ( (IDataset) deviceGroupLayer).Workspace);
                
            if (deviceGroupFeature == null) return;

            IPoint devGrpPoint = (IPoint) deviceGroupFeature.Shape;
            FlashObject(deviceGroupFeature.Class as IFeatureClass, devGrpPoint.X, devGrpPoint.Y);

            IScreenDisplay screenDisplay = _mxDoc.ActiveView.ScreenDisplay;
            IPointCollection points = (IPointCollection) new RubberLineClass().TrackNew(screenDisplay, null);
            bool changed = false;

            if (points.PointCount < 3)
            {
                MessageBox.Show(
                    "Three clicks are required: first click to identify the device group, second click " +
                    "for the starting location of the anno, and third click for the anno angle.");
                return;
            }
            if (_editingWksp == null)
                _editingWksp = ((IDataset) deviceGroupFeature.Class).Workspace as IWorkspaceEdit;
                
            _editor.StartOperation();

            try
            {
                IPoint secondClickPoint = points.Point[1], thirdClickPoint = points.Point[2];
                    
                IList<IObject> featsToIterate = ModelNameFacade.GetRelatedObjectsByModelNamesList(
                    deviceGroupFeature,
                    new[] {ModelNames.Switch, ModelNames.Transformer});
                int alignment = secondClickPoint.X - devGrpPoint.X > 0
                    ? AnnotationFacade.AlignLeft
                    : AnnotationFacade.AlignRight;
                double theta = GetTheta(secondClickPoint, thirdClickPoint), offsetForNextFeatureGroup = 0;
    
                foreach (IGrouping<int, IObject> annoParentGrouping in 
                    featsToIterate.GroupBy(annoP => (annoP.Class).ObjectClassID)
                    .OrderBy(feat =>
                {
                    for (int i = 0; i < _fcAnnoOrder.Length; i++)
                        if (_fcAnnoOrder[i] == (feat.First().Class).ObjectClassID) return i;
                    return _fcAnnoOrder.Length;
                }))
                {
                    double currAnnoYLoc = 0;
                    IList<double> annoGroupMaxWidths = new List<double>();

                    foreach (
                        IObject annoParent in
                            annoParentGrouping.OrderBy(parent => -((IPoint) ((IFeature) parent).Shape).Y))
                    {
                        IFeature ft = (IFeature) annoParent;
                        IList<IRelationshipClass> objAnnoRels = AnnotationFacade.GetAnnoRelClassesForObject(
                            _featLayers, ft);
                        

                        foreach (IRelationshipClass annoRelClass in objAnnoRels)
                        {
                            IPoint locToPlace = AnnotationFacade.RotatePointAboutPoint(secondClickPoint, devGrpPoint,
                                -theta);
                            locToPlace.X += offsetForNextFeatureGroup;
                            locToPlace.Y -= currAnnoYLoc;
                            currAnnoYLoc += AnnotationFacade.GetSumAnnoHeights(annoParent, annoRelClass) / 2;
                            locToPlace = AnnotationFacade.RotatePointAboutPoint(locToPlace, devGrpPoint, theta);

                            AnnotationFacade.RecreateAnno(ft, annoRelClass);

                            if (!ModelNameFacade.ContainsClassModelName(ft.Class, new[] {ModelNames.Transformer}))
                            {
                                AnnotationFacade.DeleteAnno(annoParent, annoRelClass, false, AnnoClassNameDelete);
                            }

                            AnnotationFacade.ApplySettingsToNewAnno(annoRelClass, ft, alignment, theta, 0, locToPlace);

                            changed = true;
                        }

                        annoGroupMaxWidths.Add(
                            objAnnoRels.Select(annoRel => AnnotationFacade.GetLongestAnnoWidth(annoParent, annoRel))
                                .Max());
                    }
                    offsetForNextFeatureGroup += (annoGroupMaxWidths.Max() + BufferWidth)*
                                                 (secondClickPoint.X - devGrpPoint.X > 0 ? 1 : -1);
                }

                if (changed) _editor.StopOperation("Recreate and align device group anno");
                else _editor.AbortOperation();

                _mxDoc.ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGeoSelection |
                    esriViewDrawPhase.esriViewGraphicSelection, null, null);

                AnnotationFacade.AddAllRelatedAnnosToSelection(_featLayers, featsToIterate);
                
                _mxDoc.ActiveView.PartialRefresh(
                    esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGeoSelection |
                    esriViewDrawPhase.esriViewGraphicSelection, null, null);
            }
            catch (Exception) {_editor.AbortOperation(); throw;}
        }

        private static IFeature GetFeatureNearPoint(IPoint p, double rad, String modelName, IWorkspace editWorkspace)
        {
            ITopologicalOperator topoOperator = (ITopologicalOperator) p;
            IGeometry searchArea = topoOperator.Buffer(rad);

            IFeatureClass devGrpClass = ModelNameFacade.FeatureClassByModelName(editWorkspace, modelName);

            ISpatialFilter spFilter = new SpatialFilterClass();
            spFilter.Geometry = searchArea;
            spFilter.GeometryField = devGrpClass.ShapeFieldName;
            spFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

            IFeatureCursor deviceGroupsCursor = devGrpClass.Search(spFilter, true);
            return deviceGroupsCursor.NextFeature();
        }

        public override bool Deactivate()
        {
            _selectedFeature = null;
            return base.Deactivate();
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

        private IPoint GetMapPointFromDevicePoint(int x, int y)
        {
            IMxApplication mxApp = (IMxApplication) _mApplication;
            IAppDisplay appDisplay = mxApp.Display;

            if (appDisplay == null)
                return null;

            IScreenDisplay screenDisplay = appDisplay.FocusScreen;
            IDisplay display = screenDisplay;

            if (display == null)
                return null;

            IDisplayTransformation transform = display.DisplayTransformation;

            return transform == null ? null : transform.ToMapPoint(x, y);
        }

        private void FlashObject(IFeatureClass fc, double x, double y)
        {
            foreach (IFeatureLayer featLayer in _featLayers.Values)
            {
                if (featLayer.FeatureClass.FeatureClassID != fc.FeatureClassID) continue;

                IPoint geom = new PointClass();
                geom.X = x;
                geom.Y = y;
                IIdentify identify = (IIdentify) featLayer;
                IArray identified = identify.Identify(geom);
                if (identified == null || identified.Count <= 0) continue;

                IFeatureIdentifyObj identifiedObject = (IFeatureIdentifyObj)identified.Element[0];
                IRowIdentifyObject identifyObject = (IRowIdentifyObject) identifiedObject;
                int oid = identifyObject.Row.OID;
                _selectedFeature = featLayer.FeatureClass.GetFeature(oid);
                if (_selectedFeature == null) continue;

                IIdentifyObj flashObj = (IIdentifyObj) identifiedObject;
                flashObj.Flash(_mxDoc.ActiveView.ScreenDisplay);
            }
        }

        #endregion
    }
}
