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

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for PGE_TransformerSearch.
    /// </summary>
    [Guid("ADC623B5-39FD-4527-B6E9-C3165AB71515")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_TransformerSearch")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class PGE_TransformerSearch : BaseCommand
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

        /// <summary>
        /// The Transformer Search form. 
        /// </summary>
        private PGE_TransformerSearchForm transSearchForm = null;

        public PGE_TransformerSearch()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text
            base.m_caption = "PGE Transformer Search";  //localizable text
            base.m_message = "PGE Transformer Search";  //localizable text 
            base.m_toolTip = "Allows Transformer search using CGC, Transformer Address, Customer Address, Meter Number or Service Point ID";  //localizable text 
            base.m_name = "PGE Transformer Search";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

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
                //Should only be enabled if the user is currently in a stored display which contains Transformers
                //PGE_TransformerSearchForm form = new PGE_TransformerSearchForm(m_application);
                //ILayer TransformerLayer = null;
                //if (!form.FindLayerByName("Transformer", out TransformerLayer))
                //    return false;
                //else

                //ME Q4-19 : DA#190903
                    if (PGEExtension.pDataTableFormInfo.Rows.Count > 0)
                    {
                        if (transSearchForm == null)
                        {
                            transSearchForm = new PGE_TransformerSearchForm(m_application);
                            transSearchForm.Show();
                        }

                    }
                    return true;
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
                // Only display a new form if this is the first call of if the original form is disposed.  This allows 
                // the UI to retain it's current state if the user closes the form.
                IntPtr p = new IntPtr(m_application.hWnd);


                //  Form is not initialized.  Create a new ofrm and display it.
                if (transSearchForm == null)
                {
                    transSearchForm = new PGE_TransformerSearchForm(m_application);
                    transSearchForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    transSearchForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));

                }
                else if (transSearchForm.IsDisposed)
                {
                    // Form is disposed release it and create a new one.
                    transSearchForm = null;
                    transSearchForm = new PGE_TransformerSearchForm(m_application);
                    transSearchForm.map = ((IMxDocument)m_application.Document).FocusMap;
                    // Keep the form on top of the ArcMap window
                    transSearchForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));

                }
                else if (!transSearchForm.Visible)
                {
                    string sqlUpdate = "update " + PGE_TransformerSearchForm.tableName_Tx_form_save + " set form_state = 'OPEN'  where LAN_ID = '" + PGE_TransformerSearchForm.userLanID.ToUpper() + "'";
                    PGE_TransformerSearchForm._loginWorkspace.ExecuteSQL(sqlUpdate);
                    // The form is hidden.  Show it.
                    transSearchForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));
                }
                else
                {
                    // Bring the form to the front 
                    string sqlUpdate = "update " + PGE_TransformerSearchForm.tableName_Tx_form_save + " set form_state = 'OPEN'  where LAN_ID = '" + PGE_TransformerSearchForm.userLanID.ToUpper() + "'";
                    PGE_TransformerSearchForm._loginWorkspace.ExecuteSQL(sqlUpdate);
                    transSearchForm.BringToFront();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to open Transformer Search window: " + ex.Message);
            }
        }

        #endregion
    }
}