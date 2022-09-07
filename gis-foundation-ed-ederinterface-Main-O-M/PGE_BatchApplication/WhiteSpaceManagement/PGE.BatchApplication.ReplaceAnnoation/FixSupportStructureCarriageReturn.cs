using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;

namespace PGE.BatchApplication.ReplaceAnnoation
{
    /// <summary>
    /// Summary description for AdjustAnnoOrientation.
    /// </summary>
    [Guid("4A2CE50E-6958-45E1-8E76-38CCBCF96B9C")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.ReplaceAnnoation.FixSupportStructureCarriageReturn")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class FixSupportStructureCarriageReturn : BaseCommand
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
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication m_application;
        private IMxDocument mxDoc = null;
        FixSupportStructureCarriageReturnForm fixSupportStructureForm = null;
        public FixSupportStructureCarriageReturn()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text 
            base.m_caption = "Fix Support Structure Anno";  //localizable text 
            base.m_message = "Fix Support Structure Anno";  //localizable text
            base.m_toolTip = "Recreates the default support structure annotation class and moves it back to original position";  //localizable text
            base.m_name = "Fix Support Structure Anno";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                
                //
                // TODO: change resource name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
                mxDoc = m_application.Document as IMxDocument;
            }
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }


        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            if (fixSupportStructureForm == null)
            {
                fixSupportStructureForm = new FixSupportStructureCarriageReturnForm(mxDoc, m_application);
            }
            fixSupportStructureForm.Show(new ModelessDialog(m_application.hWnd));
        }

        #endregion


    }
}
