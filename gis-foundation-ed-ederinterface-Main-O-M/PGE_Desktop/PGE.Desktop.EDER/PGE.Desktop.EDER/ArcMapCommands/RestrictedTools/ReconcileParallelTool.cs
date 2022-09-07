using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using Miner.Interop.Process;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.ComCategories;
using System.Reflection;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.ArcMapCommands.RestrictedTools
{
    /// <summary>
    /// Summary description for ReconcileParallelTool.
    /// </summary>
    [Guid("ad39b453-813a-4db1-a632-40c76b048d62")]
    [ClassInterface(ClassInterfaceType.None)]
    
    [ProgId("PGE.Desktop.EDER.ReconcileParallelTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class ReconcileParallelTool : BaseTool
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
        private IMMPxApplication _pxApp = null;
        private IEditor _pEditor = null;
        private UID conflictUID = null;
        private IConflictsWindow3 pConflictWindow = null;
        private IWorkspace pWorkspace = null;
        private IVersion pVersion = null;
        private IVersionEdit4 pVersionEdit4 = null;
        private string _currentUser = string.Empty;
        public ReconcileParallelTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_caption = "Reconcile Parallel Tool";  //localizable text 
            base.m_message = "Reconcile Parallel Tool";  //localizable text
            base.m_toolTip = "Reconcile Parallel Tool";  //localizable text
            base.m_name = "ReconcileParallelTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.ReconcileParallelTool.bmp"));
                base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.ReconcileParallelTool.cur"));
                //Getting current logged in user name
                _currentUser = DBFacade.GetCurrentLoginUser();
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
            bool isvalid = false;
            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {                
                if (!string.IsNullOrEmpty(_currentUser))
                {
                    IMMPxIntegrationCache mmSessionMangerIntegrationExt = FindExtesionByName("Session Manager Integration Extension") as IMMPxIntegrationCache;

                    if (mmSessionMangerIntegrationExt != null)
                    {                        
                        _pxApp = mmSessionMangerIntegrationExt.Application;
                        ADODB.Connection connection = _pxApp.Connection;
                        DBFacade objDBFacade = new DBFacade(connection);
                        isvalid = objDBFacade.GetValidUser(_currentUser);
                    }
                }
                else
                {
                    isvalid = false;
                    // MessageBox.Show("User Name is Null or Empty", "Reconcile Parallel Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                }
                if (isvalid == true)
                    base.m_enabled = true;
                else
                    base.m_enabled = false;
            }
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

       
        private static IExtension FindExtesionByName(string name)
        {
            IExtensionManager extensionMgr = Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.ExtensionManager")) as IExtensionManager;
            IExtension extension = extensionMgr.FindExtension(name);
            return extension;
        }
        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                System.Windows.Forms.Cursor cacheCursor = System.Windows.Forms.Cursor.Current;
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                IMMAutoUpdater autoupdater = obj as IMMAutoUpdater;
                mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;


                if (_pEditor == null)
                {
                    UID pID = new UIDClass();
                    pID.Value = "esriEditor.Editor";
                    _pEditor = m_application.FindExtensionByCLSID(pID) as IEditor;
                }

                if (conflictUID == null)
                {
                    conflictUID = new UIDClass();
                    conflictUID.Value = "{11725780-F274-11D2-9F2B-00C04F6BC979}";
                    pConflictWindow = _pEditor.FindExtension(conflictUID) as IConflictsWindow3;
                }
                if (pConflictWindow.ShowOnConflictsDetected == true)
                {
                    pConflictWindow.Reset();
                }
                bool hasConflicts = false;
                if (_pEditor.EditState == esriEditState.esriStateNotEditing)
                {
                    MessageBox.Show("Must be editing!", "Reconcile Parallel Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    pWorkspace = _pEditor.EditWorkspace;
                    pVersion = (IVersion)pWorkspace;
                    pVersionEdit4 = (IVersionEdit4)pVersion;
                    if (pVersion.HasParent() != false)
                    {
                        hasConflicts = pVersionEdit4.Reconcile4(pVersion.VersionInfo.Parent.VersionName, false, false, false, false);
                        if (hasConflicts)
                        {
                            pConflictWindow.Visible = true;
                            pConflictWindow.Reset();
                        }
                        else
                            MessageBox.Show("Reconcile Completed", "Reconcile Parallel Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                autoupdater.AutoUpdaterMode = oldMode;
                System.Windows.Forms.Cursor.Current = cacheCursor;
                m_application.CurrentTool = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Reconclie: " + ex.Message, "Reconcile Parallel Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _pEditor.AbortOperation();
                m_application.CurrentTool = null;
                return;
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ReconcileParallelTool.OnMouseDown implementation
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ReconcileParallelTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add ReconcileParallelTool.OnMouseUp implementation
        }
        #endregion
    }
}
