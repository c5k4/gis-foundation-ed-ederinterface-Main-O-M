using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using Miner.ComCategories;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite
{
    /// <summary>
    /// Implements the CALM Toolbar and all its components.
    /// </summary>
    [Guid("728E4EE5-C9C6-44B9-A521-689BCC957252")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ProgId("PGE.BatchApplication.DLMTools.AnnotationCompositeToolbar")]
    [ComVisible(true)]
    public class AnnotationCompositeToolbar : BaseToolbar
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
            ControlsToolbars.Register(regKey);
            MxCommandBars.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsToolbars.Unregister(regKey);
            MxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AnnotationCompositeToolbar()
        {
            AddItem("PGE.BatchApplication.DLMTools.MOWEDClickTool");
            AddItem("PGE.BatchApplication.DLMTools.MOWEDSelectionTool");
            AddItem("PGE.BatchApplication.DLMTools.MOWEDToolbarComposite");
            AddItem("PGE.BatchApplication.DLMTools.MOWEDSettingsTool");
            AddItem("PGE.BatchApplication.DLMTools.ReplaceAnnotation");
            AddItem("PGE.BatchApplication.DLMTools.AlignAnnoCommand");
            AddItem("PGE.BatchApplication.DLMTools.DeviceGroupAnnoReplacement");
            AddItem("PGE.BatchApplication.DLMTools.MirrorDucts.cmdExecuteScript");
            AddItem("PGE.BatchApplication.DLMTools.MassUpdate");
            AddItem("PGE.BatchApplication.DLMTools.ConduitHighlightButton");
            AddItem("PGE.BatchApplication.DLMTools.AnnoStackingValidation");
            AddItem("PGE.BatchApplication.DLMTools.SwapAnnotationPosition");
            AddItem("PGE.BatchApplication.DLMTools.RotationAnnotation180");
        }

        #endregion Constructor

        #region Overriden Toolbar Methods

        /// <summary>
        /// The caption of the toolbar
        /// </summary>
        public override string Caption
        {
            get
            {
                return "PG&E DLM MOWED Toolbar";
            }
        }

        /// <summary>
        /// The internal name of the toolbar
        /// </summary>
        public override string Name
        {
            get
            {
                return "PG&E_AnnotationCompositeToolbar";
            }
        }

        #endregion Overriden Toolbar Methods
    }
}
