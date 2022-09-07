using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.ArcMapCommands.RestrictedTools
{
    /// <summary>
    /// Summary description for RestrictedToolbar.
    /// </summary>
    [Guid("e88467c8-5ac7-484b-a588-a3d2b2076994")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    [ClassInterface(ClassInterfaceType.None)]
    
    [ProgId("PGE.Desktop.EDER.RestrictedToolbar")]
    [ComVisible(true)]
    public sealed class RestrictedToolbar : BaseToolbar
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

        public RestrictedToolbar()
        {
            AddItem("PGE.Desktop.EDER.ReconcileParallelTool");
            AddItem("PGE.Desktop.EDER.AutoupdatersOnTool");
            AddItem("PGE.Desktop.EDER.AutoupdatersOffTool");
        }

        public override string Caption
        {
            get
            {
                //TODO: Replace bar caption
                return "PG&E Restricted Toolbar";
            }
        }
        public override string Name
        {
            get
            {
                //TODO: Replace bar ID
                return "PG&E_RestrictedToolbar";
            }
        }       
    }
}