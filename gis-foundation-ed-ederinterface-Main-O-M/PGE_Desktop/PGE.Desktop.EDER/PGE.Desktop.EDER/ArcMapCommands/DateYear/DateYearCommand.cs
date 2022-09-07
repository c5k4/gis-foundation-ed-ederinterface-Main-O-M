using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

using Miner.ComCategories;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Systems.Configuration;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.UI.Framework;
using ESRI.ArcGIS.Editor;

namespace PGE.Desktop.EDER.ArcMapCommands.DateYear
{
    /// <summary>
    /// Summary description for DateYearCommand.
    /// </summary>
    [Guid("aec4b7bf-889b-402f-90a6-7f9a5b18f015")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.DateYear.DateYearCommand")]

    public sealed class DateYearCommand : BaseCommand
    {
        private DateYearUpdate dateYearForm = null;

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
        public static IWorkspace _loginWorkspace;
        private IApplication m_application;
        public DateYearCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "Date and Year Installed Tool";  //localizable text
            base.m_message = "Date and Year Installed Tool";  //localizable text 
            base.m_toolTip = "Date and Year Installed Tool";  //localizable text 
            base.m_name = "PGEDATEYEARTOOL";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

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
        private static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
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

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }
        //to enable this button

        public override bool Enabled
        {

            get
            {

                //Should only be enabled if the user is currently in an edit session

                if (Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateEditing) { return true; }

                else { return false; }

            }

        }
        public DateYearUpdate dym;
        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {


            try
            {

                IntPtr p = new IntPtr(m_application.hWnd);


                if (dateYearForm == null)
                {
                    dateYearForm = new DateYearUpdate(m_application);
                    dateYearForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    dateYearForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));


                }
                else if (dateYearForm.IsDisposed)
                {
                    // Form is disposed release it and create a new one.
                    dateYearForm = null;
                    dateYearForm = new DateYearUpdate(m_application);
                    dateYearForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    dateYearForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));


                }
                else if (!dateYearForm.Visible)
                {
                    // The form is hidden.  Show it.
                    dateYearForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));

                }
                else
                {
                    // Bring the form to the front 
                    dateYearForm.BringToFront();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to open Transformer Search window: " + ex.Message);
            }



        }
    }

       #endregion
    }

