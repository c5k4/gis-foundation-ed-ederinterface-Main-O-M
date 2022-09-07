using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;
namespace PGE.Desktop.EDER.ArcMapCommands.AnnotationComposite
{
    /// <summary>
    /// Implements the CALM Toolbar and all its components.
    /// </summary>
    [Guid("FFB8D729-AB6B-46C7-AD8F-6F78FA7C3E4E")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ProgId("PGE.Desktop.EDER.AnnotationCompositeToolbar")]
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
            AddItem("PGE.Desktop.EDER.CALMClickTool");
            AddItem("PGE.Desktop.EDER.CALMSelectionTool");
            AddItem("PGE.Desktop.EDER.CALMToolbarComposite");
            AddItem("PGE.Desktop.EDER.CALMSettingsTool");
            AddItem("PGE.Desktop.EDER.ReplaceAnnotation");

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
                return "PG&E CALM Toolbar";
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
