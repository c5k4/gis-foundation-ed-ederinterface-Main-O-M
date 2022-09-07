using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;

using Miner.ComCategories;

namespace PGE.Desktop.EDER.ArcMapCommands.UFMFilter
{
    /// <summary>
    /// Implements a toolbar that hosts the UFMFilter ToolControl as well as the PopulateLastJobNumber command
    /// </summary>
    [Guid("AFDF99B0-8241-4a31-9E96-1B0191C96E9D")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ProgId("PGE.Desktop.EDER.UfmFilterToolbar")]
    [ComVisible(true)]
    public class UfmFilterToolbar : BaseToolbar
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

        #region Class Constructor
        public UfmFilterToolbar()
        {
            AddItem("PGE.Desktop.EDER.UfmFilterCommand");

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
                return "PG&E UFM Filter";
            }
        }

        //the internal name of the toolbar
        public override string Name
        {
            get
            {
                return "PG&E_UfmFilterToolbar";
            }
        }
        #endregion Overriden Toolbar Methods
    }
}
