using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using Miner.ComCategories;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.ViewWebr
{
    /// <summary>
    /// Summary description for PGE_ViewWebViewerToolbar.
    /// </summary>
    [Guid("ff100039-7a66-4438-8f45-76108922f70d")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ComVisible(true)]
    [ProgId("Telvent.PGE.ED.Desktop.ArcMapCommands.PGE_ViewWebViewerToolbar")]
    public sealed class PGE_ViewWebViewerToolbar : BaseToolbar
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
            MxCommandBars.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

        public PGE_ViewWebViewerToolbar()
        {
            //
            // TODO: Define your toolbar here by adding items
            //
            //AddItem("esriArcMapUI.ZoomInTool");
            //BeginGroup(); //Separator
            //AddItem("{FBF8C3FB-0480-11D2-8D21-080009EE4E51}", 1); //undo command
            //AddItem(new Guid("FBF8C3FB-0480-11D2-8D21-080009EE4E51"), 2); //redo command
            AddItem("Telvent.PGE.ED.Desktop.ArcMapCommands.ViewWebr");
        }

        public override string Caption
        {
            get
            {
                //TODO: Replace bar caption
                return "PG&E Web Viewer Toolbar";
            }
        }
        public override string Name
        {
            get
            {
                //TODO: Replace bar ID
                return "PGE_ViewWebViewerToolbar";
            }
        }
    }
}