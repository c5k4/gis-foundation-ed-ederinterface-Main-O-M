using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using Miner.ComCategories;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Common.Delivery.ArcFM;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for PGE_UpdateMapNumber.
    /// </summary>
    [Guid("dcbf713d-ac59-4b8c-a6dc-0d977e5e17bc")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_UpdateMapNumber")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class PGE_UpdateMapNumber : BaseCommand
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
        public PGE_UpdateMapNumber()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PGE Update Map Number";  //localizable text
            base.m_message = "PGE Update Map Number";  //localizable text 
            base.m_toolTip = "Updates Local Office ID and Distribution Map Number field values";  //localizable text 
            base.m_name = "PGE Update Map Number";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        public override bool Enabled
        {
            get
            {
                //Should only be enabled if the user is currently in an edit session
                if (Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing) { return true; }
                else { return false; }
            }
        }

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                //Open the update map number form to perform all of the updates to selected features
                PGE_UpdateMapNumberForm updateMapNumberForm = new PGE_UpdateMapNumberForm();
                updateMapNumberForm.map = ((IMxDocument)m_application.Document).FocusMap;
                updateMapNumberForm.ShowDialog(new ArcMapWindow(m_application));
                updateMapNumberForm = null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update map numbers: " + ex.Message);
            }
        }

        #endregion
    }
}
