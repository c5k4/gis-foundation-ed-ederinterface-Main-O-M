#region references
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using Miner.ComCategories;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
#endregion

namespace PGE.Desktop.EDER.ArcMapCommands.PSPS_CircuitFilter
{
    /// <summary>
    /// Summary description for PSPS_CircuitFilter Command
    /// </summary>
    [Guid("3EBF0893-A8F2-44D8-BC77-2E34B7633271")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PSPS_CircuitFilter.PSPS_CircuitFilter")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public class PSPS_CircuitFilter : BaseCommand
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

        #region Global Varriables

        private IApplication m_application;
        private PSPS_CircuitFilter_Form PSPSCircuitFilterForm = null;

        #endregion

        #region Constructor
        public PSPS_CircuitFilter()
        {
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PSPS Circuit Filter Tool";  //localizable text
            base.m_message = "PSPS Circuit Filter Tool";  //localizable text 
            base.m_toolTip = "Allows PSPS Maps Stored dispay layeres to be filtered by Circuit ID or Circuit Name";  //localizable text 
            base.m_name = "PSPS Circuit Filter Tool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {                
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }
        #endregion

        #region Overridden Class Methods

        public override bool Enabled
        {
            get
            {
                string selectedStoredDisplay = getSelectedStoredDisplay();
                if(selectedStoredDisplay.ToUpper() == "PSPS Maps".ToUpper())
                {
                    return true;
                }
                else
                {
                    return false;
                }                
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
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                // Only display a new form if this is the first call of if the original form is disposed.  This allows 
                // the UI to retain it's current state if the user closes the form.
                IntPtr p = new IntPtr(m_application.hWnd);
                
                //  Form is not initialized.  Create a new ofrm and display it.
                if (PSPSCircuitFilterForm == null)
                {
                    PSPSCircuitFilterForm = new PSPS_CircuitFilter_Form(m_application);
                    PSPSCircuitFilterForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    PSPSCircuitFilterForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));

                }
                else if (PSPSCircuitFilterForm.IsDisposed)
                {
                    // Form is disposed release it and create a new one.
                    PSPSCircuitFilterForm = null;
                    PSPSCircuitFilterForm = new PSPS_CircuitFilter_Form(m_application);
                    PSPSCircuitFilterForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    PSPSCircuitFilterForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));

                }
                else if (!PSPSCircuitFilterForm.Visible)
                {                    
                    // The form is hidden.  Show it.
                    PSPSCircuitFilterForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));
                }
                else
                {
                    // Bring the form to the front                     
                    PSPSCircuitFilterForm.BringToFront();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to open PSPS Circuit Filter window: " + ex.Message);
            }
        }

        #endregion

        #region private methods
        private string getSelectedStoredDisplay()
        {
            Type type = Type.GetTypeFromProgID("esriSystem.ExtensionManager");
            object objExt = Activator.CreateInstance(type);
            IExtensionManager extMgr = objExt as IExtensionManager;
            IMMStoredDisplayManager objStoredDisplayMan = (IMMStoredDisplayManager)extMgr.FindExtension("MMStoredDisplayMgr");
            string sCurrentStoreddisplayName = objStoredDisplayMan.CurrentStoredDisplayName();
            return sCurrentStoreddisplayName;
        }

        #endregion
    }
}
