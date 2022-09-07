using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Summary description for PGE_SessionCheckerToolBar.
    /// </summary>
    [Guid("57a1bef9-4364-40c2-95a9-6500e80e184c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.PGE_SessionCheckerToolBar")]
    [ComponentCategory(ComCategory.ArcMapCommandBars)]
    public sealed class PGE_SessionCheckerToolBar : BaseToolbar
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

        public PGE_SessionCheckerToolBar()
        {
            //
            // TODO: Define your toolbar here by adding items
            AddItem("PGE.Desktop.EDER.SelectEdits");
            BeginGroup();
            AddItem("PGE.Desktop.EDER.SelectReviewMode");
            BeginGroup();
            AddItem("Miner.Desktop.Commands.AttributeEditorCommand");
        }

        public override string Caption
        {
            get
            {
                //TODO: Replace bar caption
                return "PGE Session Checker";
            }
        }
        public override string Name
        {
            get
            {
                //TODO: Replace bar ID
                return "PGE_SessionCheckerToolBar";
            }
        }
    }
}