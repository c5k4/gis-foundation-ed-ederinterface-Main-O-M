using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using Miner.Interop.Process;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.ComCategories;
using System.Reflection;

namespace PGE.Desktop.EDER.ArcMapCommands.RestrictedTools
{
    /// <summary>
    /// Summary description for AutoupdatersOnTool.
    /// </summary>
    [Guid("87bbd1c2-779b-4ec7-93d4-d4d9ac7705ce")]
    [ClassInterface(ClassInterfaceType.None)]
    
    [ProgId("PGE.Desktop.EDER.AutoupdatersOnTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class AutoupdatersOnTool : BaseTool
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
        private string _currentUser = string.Empty;
        public AutoupdatersOnTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_caption = "AutoupdatersOn Tool";  //localizable text 
            base.m_message = "AutoupdatersOn Tool";  //localizable text
            base.m_toolTip = "AutoupdatersOn Tool";  //localizable text
            base.m_name = "AutoupdatersOnTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                //base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.AutoupdatersOnTool.bmp"));
                base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.AutoupdatersOnTool.cur"));
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
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;

                System.Windows.Forms.Cursor.Current = cacheCursor;
                m_application.CurrentTool = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in On (enable) the Autoupdaters: " + ex.Message, "Autoupdaters On Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_application.CurrentTool = null;
                return;
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOnTool.OnMouseDown implementation
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOnTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOnTool.OnMouseUp implementation
        }
        #endregion
    }
}
