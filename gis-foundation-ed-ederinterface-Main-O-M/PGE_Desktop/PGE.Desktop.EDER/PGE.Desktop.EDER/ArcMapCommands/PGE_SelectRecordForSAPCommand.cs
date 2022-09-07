using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;

using PGE.Common.Delivery.Diagnostics;

using Miner.Interop;
using Miner.Interop.Process;
using Miner.ComCategories;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("cf75e324-1444-4dac-bc61-00a1ed42d141")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_SelectRecordForSAPCommand")]
    [ComVisible(true)]
    public sealed class PGE_SelectRecordForSAPCommand : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]

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
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IApplication _application;
        private IEditor3 m_editor;
        //private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IHookHelper m_hookHelper = null;
        public PGE_SelectRecordForSAPCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "Send Records to SAP";  //localizable text 
            base.m_message = "Send Records to SAP";  //localizable text
            base.m_toolTip = "Send Records to SAP";  //localizable text
            base.m_name = "PGETools_SelectRecordsForSAP";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")
           

            try
            {
                //
                // TODO: change bitmap name if necessary
                //

                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
               // base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Cursors.PGE_SelectRecordForSAPCommand.cur"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            _application = hook as IApplication;

            //Get the editor.
            UID editorUid = new UID();
            editorUid.Value = "esriEditor.Editor";
            m_editor = _application.FindExtensionByCLSID(editorUid) as IEditor3;

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
                PGE_SelectRecordForSAPCommandForm selectRecordForSAPCommandForm = new PGE_SelectRecordForSAPCommandForm();
                selectRecordForSAPCommandForm.map = ((IMxDocument)_application.Document).FocusMap;
                selectRecordForSAPCommandForm.ShowDialog(new ArcMapWindow(_application));
                selectRecordForSAPCommandForm = null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update map numbers: " + ex.Message);
            }
        }

        public override bool Enabled
        {
            //Enable the command when map are in edit mode.
            get
            {
                return m_editor.EditState == esriEditState.esriStateEditing;
            }
        }

        #endregion
    }
}
