using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;

namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync
{
    /// <summary>
    /// Implements a toolbar that hosts the PopulateLastJobNumber ToolControl as well as the PopulateLastJobNumber command
    /// </summary>
    [Guid("0A6A81A7-99E0-4D56-9101-A5BF83C4BECD")]
    [ComponentCategory(ComCategory.ArcCatalogCommandBars)]
    [ProgId("PGE.Desktop.EDER.UpdateFieldOrder")]
    [ComVisible(true)]
    public class PG_E_Utilities_Toolbar : BaseToolbar
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
            GxCommandBars.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsToolbars.Unregister(regKey);
            GxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

         #region Class Constructor
        public PG_E_Utilities_Toolbar()
        {
            AddItem("PGE.Desktop.EDER.UpdateFieldOrderCommand");

        }
        #endregion Class Constructor

        #region Overriden Toolbar Methods
        /// <summary>
        /// the caption of the toolbar
        /// </summary>
        public override string Caption
        {
            get
            {
                return "PG&E Utilities";
            }
        }

        /// <summary>
        /// the internal name of the toolbar
        /// </summary>
        public override string Name
        {
            get
            {
                return "PG&E_Utilities_Toolbar";
            }
        }
        #endregion Overriden Toolbar Methods
    }
}
