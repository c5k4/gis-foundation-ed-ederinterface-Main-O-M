using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;
namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Implements a toolbar that hosts the PopulateLastJobNumber ToolControl as well as the PopulateLastJobNumber command
    /// </summary>
    [Guid("F8658C18-ECEF-45F6-9E75-B1F062B7B65A")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ProgId("PGE.Desktop.EDER.PopulateLastJobNumberToolbar")]
    [ComVisible(true)]
    public class PopulateLastJobNumberToolbar : BaseToolbar
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
        public PopulateLastJobNumberToolbar()
        {
            AddItem("PGE.Desktop.EDER.PopulateLastJobNumberCommand");

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
                return "PG&E Job Number";
            }
        }

        //the internal name of the toolbar
        public override string Name
        {
            get
            {
                return "PG&E_Job_NumberToolbar";
            }
        }
        #endregion Overriden Toolbar Methods
    }
}
