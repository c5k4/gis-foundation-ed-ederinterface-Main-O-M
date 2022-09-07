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
    /// Summary description for PGEMapUtilitiesToolbar.
    /// </summary>
    [Guid("ffffe04b-1591-4de9-a41b-867ba6ac16a4")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGEMapUtilitiesToolbar")]
    [ComVisible(true)]
    public class PGEMapUtilitiesToolbar : BaseToolbar
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

        #region Class Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEMapUtilitiesToolbar"/>
        /// </summary>
        public PGEMapUtilitiesToolbar()
        {
            //Add the PGE Print Command to the Toolbar
            AddItem("PGE.Desktop.EDER.ArcMapCommands.PGEPrintMapCommand");
        }
        #endregion

        #region Overridden Toolbar Methods
        /// <summary>
        /// The caption of the Toolbar
        /// </summary>
        public override string Caption
        {
            get
            {
                return "PG&E Map Utilities";
            }
        }
        /// <summary>
        /// the internal name of the toolbar
        /// </summary>
        public override string Name
        {
            get
            {
                return "PGEMapUtilitiesToolbar";
            }
        }
        #endregion
    }
}