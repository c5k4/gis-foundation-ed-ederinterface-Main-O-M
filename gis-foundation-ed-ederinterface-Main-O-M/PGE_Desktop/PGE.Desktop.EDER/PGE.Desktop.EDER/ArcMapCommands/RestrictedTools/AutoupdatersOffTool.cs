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
    /// Summary description for AutoupdatersOffTool.
    /// </summary>
    [Guid("f487e5a5-ef63-4b32-9ab6-d74f0082e260")]
    [ClassInterface(ClassInterfaceType.None)]
    
    [ProgId("PGE.Desktop.EDER.AutoupdatersOffTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class AutoupdatersOffTool : BaseTool
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
        public AutoupdatersOffTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_caption = "AutoupdatersOff Tool";  //localizable text 
            base.m_message = "AutoupdatersOff Tool";  //localizable text
            base.m_toolTip = "AutoupdatersOff Tool";  //localizable text
            base.m_name = "AutoupdatersOffTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                //base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.AutoupdatersOffTool.bmp"));
                base.m_cursor = new System.Windows.Forms.Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.ArcMapCommands.RestrictedTools.AutoupdatersOffTool.cur"));
                
                ////Getting current logged in user name
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
                autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                System.Windows.Forms.Cursor.Current = cacheCursor;
                m_application.CurrentTool = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Off (disable) the Autoupdaters: " + ex.Message, "Autoupdaters Off Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_application.CurrentTool = null;
                return;
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOffTool.OnMouseDown implementation
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOffTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AutoupdatersOffTool.OnMouseUp implementation
        }
        #endregion
    }
}
