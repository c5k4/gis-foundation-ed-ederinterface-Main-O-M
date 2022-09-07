using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.AutoUpdaters.Attribute
{
    /// <summary>
    /// Date: Dec-26-2018
    /// Module Name: PNodeIDDisplayForm
    /// Description: This algorithm provides drop down combo box with CNODE-IDs and BusIds, for user to choose from.
    /// </summary>
    public partial class PNodeIDDisplayForm : Form
    {
        public PNodeIDDisplayForm()
        {
            InitializeComponent();
        }

        private void frmDisplayNames_Load(object sender, EventArgs e)
        {
            try
            {
                PNodeInitialize.ErrorOccured = false;
                if (!PNodeInitialize.InitialiseVariables())
                    return;
                LoadFNM_Data();
            }
            catch (Exception ex) {ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            
        }               

        //private void cboIDs_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //try
        //    //{
        //    //    ._IDValue = cboIDs.Text;
        //    //}
        //    //catch (Exception Ex)
        //    //{ ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        //}

        //private void cboIDs_DragDrop(object sender, DragEventArgs e)
        //{

        //}

        /// <summary>
        /// Set the focus back to combobox and helps with auto populate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboIDs_DropDown(object sender, EventArgs e)
        {
            try
            {
                ComboBox cbo = (ComboBox)sender;
                cbo.PreviewKeyDown += new PreviewKeyDownEventHandler(cboIDs_PreviewKeyDown);
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// Set the foucus back to combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboIDs_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                ComboBox cbo = (ComboBox)sender;
                cbo.PreviewKeyDown -= cboIDs_PreviewKeyDown;
                if (cbo.DroppedDown) cbo.Focus();
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// Enable the Ok command if combox-box text changed and is not null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboIDs_TextChanged(object sender, EventArgs e)
        {
            try
            {
                cmdOk.Enabled = false;
                if (!string.IsNullOrEmpty(cboIDs.Text))
                    cmdOk.Enabled = true;
            }
            catch (Exception ex)
            { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }

        }

        //private void frmDisplayNames_FormClosed(object sender, FormClosedEventArgs e)
        //{
            
        //}

        /// <summary>
        /// On click of the cancel button, set pnode-initialize PNode and FnmGuid variables to null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                PNodeInitialize.PNodeIDValue = null;
                PNodeInitialize.FnmGUIDValue = null;
                PNodeInitialize.FNM_OID = -1;
            }
            catch (Exception Ex)
            { ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// On click of the OK button, get the Key-value pair from the data dictionary, and extract cnodeId and busID values.
        /// Pass those values to pnode-initialize class for further processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboIDs.SelectedItem == null)
                {
                    MessageBox.Show("NodeID typed is incorrect. Please choose or type correct NodeID.", "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmdOk.Enabled = false;
                    return;
                }

                KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)cboIDs.SelectedItem;
                string nodeValue = selectedItem.Value.ToString();
                nodeValue = nodeValue.Substring(0, nodeValue.IndexOf(" | "));

                PNodeInitialize.PNodeIDValue = nodeValue;
                PNodeInitialize.FnmGUIDValue = selectedItem.Key.ToString();
                this.Close();                
            }
            catch (Exception Ex)
            { ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        /// <summary>
        /// Load FNM data from the dictionary by binding the comboBox to data dictionary.
        /// </summary>
        private void LoadFNM_Data()
        {
            try
            {
                PNodeInitialize._gMouseCur.SetCursor(2);
                cboIDs.Enabled = true;
                cboIDs.AllowDrop = true;
                lblIDs.Enabled = true;
                lblSelect.Enabled = true;

                //Bind combobox to data-dictionary.
                cboIDs.DataSource = new BindingSource(PNodeInitialize._gCNODE_Guid_Dict, null);
                cboIDs.DisplayMember = "Value";
                cboIDs.ValueMember = "Key";
                //foreach (var a in arryofIDs)
                //{
                //    cboIDs.Items.Add(a);
                //}
            }
            catch (Exception ex) { ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }

        }

        /// <summary>
        /// Display the error on the screen.
        /// </summary>
        /// <param name="ex">Occured error execption</param>
        /// <param name="strFunctionName">Function name error occured at</param>
        internal static void ErrorMessage(Exception ex, string strFunctionName)
        {
            try
            {
                System.Windows.Forms.MessageBox.Show("[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception pException) { }
        }
    }
}
