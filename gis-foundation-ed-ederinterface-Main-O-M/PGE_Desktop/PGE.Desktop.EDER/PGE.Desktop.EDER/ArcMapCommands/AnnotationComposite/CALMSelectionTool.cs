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
using Miner.ComCategories;
using PGE.Desktop.EDER.Utility;
using PGE.Desktop.EDER.Custom.Commands.AnnotationComposite;

namespace PGE.Desktop.EDER.ArcMapCommands.AnnotationComposite
{
    /// <summary>
    /// Part of the CALM Toolbar - a command that applies configurable movement settings
    /// to all selected annotation features.
    /// </summary>
    [Guid("8ED72E80-CF79-455A-9500-EBCAFCBF25E0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.CALMSelectionTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class CALMSelectionTool : BaseCALMTool
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
        public CALMSelectionTool() : base()
        {
            base.m_caption = "Move All Selected Annotation with Configured Settings";  //localizable text 
            base.m_message = "Move All Selected Annotation with Configured Settings";  //localizable text
            base.m_toolTip = "Move All Selected Annotation with Configured Settings";  //localizable text
            base.m_name = "CALM Selection";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.CALMSelection.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
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

            List<IFeature> selectedFeatures = new List<IFeature>();
            foreach (IFeatureLayer featLayer in _annoFeatLayers)
            {
                ISelectionSet featSelection = (featLayer as IFeatureSelection).SelectionSet;
                if (featSelection != null)
                {
                    IEnumIDs selIDs = featSelection.IDs;
                    selIDs.Reset();
                    for (int oid = selIDs.Next(); oid != -1; oid = selIDs.Next())
                    {
                        selectedFeatures.Add(featLayer.FeatureClass.GetFeature(oid));
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

                    TransformAnno(selectedFeatures);

                    // Turn AUs back on.
                    AUD.AUsEnabled = true;

                    _pEditor.StopOperation("CALM Annotation Selection Tool");
                }
            }
            catch (Exception)
            {
                _pEditor.AbortOperation();
            }

            _mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, null); //Bug#13460

            System.Windows.Forms.Cursor.Current = cacheCursor;

            _application.CurrentTool = null;
        }
        #endregion

        #region Overridden Class Properties

        public override bool Checked
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
