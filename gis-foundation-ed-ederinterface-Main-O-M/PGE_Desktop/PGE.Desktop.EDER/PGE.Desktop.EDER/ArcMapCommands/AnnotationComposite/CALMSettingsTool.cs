using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using PGE.Desktop.Custom.Commands.AnnotationComposite.UI;

namespace PGE.Desktop.EDER.ArcMapCommands.AnnotationComposite
{
    /// <summary>
    /// Part of the CALM Toolbar - a command that launches the Settings UI.
    /// </summary>
    [Guid("D39FC567-6C05-4C48-B499-E816E70E27CA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.CALMSettingsTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class CALMSettingsTool : BaseCommand
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

        #region Private Members
        private List<IFeatureLayer> _annoFeatLayers = new List<IFeatureLayer>();
        private IApplication _application;
        private IMxDocument _mxDoc = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CALMSettingsTool()
        {
            base.m_category = "PGE Tools"; //localizable text 
            base.m_caption = "CALM Annotation Settings";  //localizable text 
            base.m_message = "CALM Annotation Settings";  //localizable text 
            base.m_toolTip = "CALM Annotation Settings";  //localizable text 
            base.m_name = "CALM Settings";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.CALMSettings.bmp"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            _application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
                _mxDoc = _application.Document as IMxDocument;
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
            CALMSettingsUC.Instance.Show(System.Windows.Forms.Control.FromHandle((IntPtr)_application.hWnd));
            CALMSettingsUC.Instance.Activate();
        }
        #endregion
    }
}
