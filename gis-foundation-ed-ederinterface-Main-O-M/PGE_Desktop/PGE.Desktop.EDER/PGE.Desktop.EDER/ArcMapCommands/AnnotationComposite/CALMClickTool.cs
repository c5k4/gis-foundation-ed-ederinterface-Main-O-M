using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
using PGE.Desktop.EDER.Utility;
using PGE.Desktop.EDER.Custom.Commands.AnnotationComposite;

namespace PGE.Desktop.EDER.ArcMapCommands.AnnotationComposite
{
    /// <summary>
    /// Part of the CALM Toolbar - a tool that applies configurable movement settings
    /// to any annotation feature that is clicked.
    /// </summary>
    [Guid("5718C887-AA29-4A14-B5D0-76016AE5D02D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.CALMClickTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class CALMClickTool : BaseCALMTool
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

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CALMClickTool() : base()
        {
            base.m_caption = "Move Annotation with Configured Settings";  //localizable text 
            base.m_message = "Move Annotation with Configured Settings";  //localizable text
            base.m_toolTip = "Move Annotation with Configured Settings";  //localizable text
            base.m_name = "CALM";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.CALM.bmp"));
                base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Cursors.ReplaceAnnotation.cur"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

        #region Overridden Class Methods

        #region Unused Mouse Methods

        public override void OnClick() { }
        public override void OnMouseMove(int Button, int Shift, int X, int Y) { }
        public override void OnMouseDown(int Button, int Shift, int X, int Y) { }

        #endregion

        /// <summary>
        /// This method is called when a mouse button is released, when this tool is active.
        /// </summary>
        /// <param name="Button">
        ///     Specifies which mouse button is released; 1 for the left mouse button, 2 for the right 
        ///     mouse button, and 4 for the middle mouse button.
        /// </param>
        /// <param name="Shift">
        ///     Specifies an integer corresponding to the state of the SHIFT (bit 0), CTRL (bit 1) and
        ///     ALT (bit 2) keys. When none, some, or all of these keys are pressed none, some, or all
        ///     the bits get set. These bits correspond to the values 1, 2, and 4, respectively. For
        ///     example, if both SHIFT and ALT were pressed, Shift would be 5. 
        /// </param>
        /// <param name="X">The X coordinate, in device units, of the mouse event.</param>
        /// <param name="Y">The Y coordinate, in device units, of the mouse event.</param>
        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (_pEditor == null)
            {
                UID pID = new UIDClass();
                pID.Value = "esriEditor.Editor";
                _pEditor = _application.FindExtensionByCLSID(pID) as IEditor;
            }
            if (_pEditor.EditState == esriEditState.esriStateNotEditing)
            {
                MessageBox.Show("Must be editing!", "CALM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_annoFeatLayers.Count < 1)
                GetEligibleLayers();

            IDisplayTransformation transform = GetDisplayTransformation(_application as IMxApplication);

            if (transform == null)
                return;


            IPoint geom = transform.ToMapPoint(X, Y);

            IFeature selectedFeature = null;
            foreach (IFeatureLayer featLayer in _annoFeatLayers)
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
                        flashObj.Flash(_mxDoc.ActiveView.ScreenDisplay);
                        break;
                    }
                }
            }

            System.Windows.Forms.Cursor cacheCursor = System.Windows.Forms.Cursor.Current;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (ReadSettings())
                {
                    _pEditor.StartOperation();

                    // Turn off AUs.
                    AUDisabler AUD = new AUDisabler();
                    AUD.AUsEnabled = false;

                    if (selectedFeature != null) //Bug#13705:CALM tool bar error msg due to null selected feature
                        TransformAnno(new List<IFeature> { selectedFeature });

                    // Turn AUs back on.
                    AUD.AUsEnabled = true;

                    _pEditor.StopOperation("CALM Annotation Tool");
                }
            }
            catch (Exception)
            {
                _pEditor.AbortOperation();
            }

            _mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null); //Bug#13460

            System.Windows.Forms.Cursor.Current = cacheCursor;
        }
        #endregion
    }
}
