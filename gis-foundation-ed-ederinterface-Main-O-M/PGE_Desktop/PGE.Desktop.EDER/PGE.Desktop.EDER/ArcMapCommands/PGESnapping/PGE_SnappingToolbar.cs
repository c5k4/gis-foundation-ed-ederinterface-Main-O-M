using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for PGE_SnappingToolbar.
    /// </summary>
    [Guid("2972DEB0-725B-45C6-990A-540E7172C480")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_SnappingToolbar")]
    public sealed class PGE_SnappingToolbar : BaseToolbar
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

        public PGE_SnappingToolbar()
        {
            //
            // TODO: Define your toolbar here by adding items
            //
            AddItem("PGE.Desktop.EDER.ArcMapCommands.PGE_SnappingToggle");
            AddItem("PGE.Desktop.EDER.ArcMapCommands.PGE_SaveClassicSnapping");
            AddItem("PGE.Desktop.EDER.ArcMapCommands.PGE_LoadDefaultSnapping");
            AddItem("esriArcMapUI.BuildMapCacheCommand");
        }

        public override string Caption
        {
            get
            {
                //TODO: Replace bar caption
                return "PG&E Snapping Environment";
            }
        }
        public override string Name
        {
            get
            {
                //TODO: Replace bar ID
                return "PGE_SnappingToolbar";
            }
        }
    }
}