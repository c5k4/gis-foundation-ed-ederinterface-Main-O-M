using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using Miner.ComCategories;
using Miner.Geodatabase.Edit;
using Telvent.PGE.ED.Desktop.Forms;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PTTTools
{
    /// <summary>
    /// Summary description for dockable window toggle command
    /// </summary>
    [Guid("4be9f3c0-fa8e-4290-bd0e-625686ac73b2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Telvent.PGE.ED.Desktop.ArcMapCommands.PTTTools.PTTDockableWindowCommand")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class PTTDockableWindowCommand : BaseCommand
    {
        private IApplication m_application;
        private IDockableWindow m_dockableWindow;
        private PTTDockableWindow pttDockableWindow;

        private const string DockableWindowGuid = "{052c2344-7c28-49e5-9a75-5c5a23ec6528}";

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

        public PTTDockableWindowCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PTT Reconciliation Tools";  //localizable text
            base.m_message = "PTT Reconciliation Tools";  //localizable text 
            base.m_toolTip = "Toggle the Pole Test & Treat Reconciliation Tools";  //localizable text 
            base.m_name = "PTT Reconciliation Tools";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                PTTDockableWindow.CaptionBitmap = base.m_bitmap;
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
            if (hook != null)
                m_application = hook as IApplication;

            if (m_application != null)
            {
                SetupDockableWindow();
                
                Miner.Geodatabase.Edit.Editor.OnStartEditing += new EventHandler<Miner.Geodatabase.Edit.EditEventArgs>(Editor_OnStartEditing);
                Miner.Geodatabase.Edit.Editor.OnStopEditing += new EventHandler<SaveEditEventArgs>(Editor_OnStopEditing);
                base.m_enabled = m_dockableWindow != null;
            }
            else
                base.m_enabled = false;
        }

        /// <summary>
        /// Toggle visiblity of dockable window and show the visible state by its checked property
        /// </summary>
        public override void OnClick()
        {
            if (m_dockableWindow == null)
                return;

            if (m_dockableWindow.IsVisible())
                m_dockableWindow.Show(false);
            else
                m_dockableWindow.Show(true);

            base.m_checked = m_dockableWindow.IsVisible();
        }

        public override bool Checked
        {
            get
            {
                return false;
            }
        }
        #endregion

        private void SetupDockableWindow()
        {
            if (m_dockableWindow == null)
            {
                IDockableWindowManager dockWindowManager = m_application as IDockableWindowManager;
                if (dockWindowManager != null)
                {
                    UID windowID = new UIDClass();
                    windowID.Value = DockableWindowGuid;
                    m_dockableWindow = dockWindowManager.GetDockableWindow(windowID);
                }
            }
        }

        #region Private Methods

        void Editor_OnStartEditing(object sender, EditEventArgs e)
        {
            //User has started editing a session.  This is a signal to refresh the content of this window
            PTTDockableWindow.Instance.GetPoleInformation(Miner.Geodatabase.Edit.Editor.EditWorkspace);
        }

        void Editor_OnStopEditing(object sender, SaveEditEventArgs e)
        {
            //User has stopped editing a session.  This is a signal to clear the content of this window
            PTTDockableWindow.Instance.ClearPoleInformation();
        }

        #endregion
    }
}
