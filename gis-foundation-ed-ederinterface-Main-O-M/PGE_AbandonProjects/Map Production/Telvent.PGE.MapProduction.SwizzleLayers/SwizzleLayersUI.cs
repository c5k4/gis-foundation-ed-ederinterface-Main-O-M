using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using Miner.Interop;


namespace Telvent.PGE.MapProduction.SwizzleLayers
{
    public partial class SwizzleLayersUI : Form
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\telvent.mapproduction.log4net.config", "MapProduction");
        Boolean isClosed = false;

        /// <summary>
        /// Forminitialization and license checking
        /// </summary>
        public SwizzleLayersUI()
        {
            InitializeComponent();
            textBoxSdeFilePath.Text = "";
            textBoxMxdFolder.Text = "";
        }

        /// <summary>
        /// Select the sde connection file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectSdeFile_Click(object sender, EventArgs e)
        {
            //showing diaog. if selected then file name will be printed in the textbox.
            if (sdeOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxSdeFilePath.Text = sdeOpenFileDialog.FileName;
            }
        }

        /// <summary>
        /// Select the folder where mxd file available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            //Showing dialog. If selected then folder name will be printed in the textbox.
            if (folderBrowserDialogMXD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxMxdFolder.Text = folderBrowserDialogMXD.SelectedPath;
            }
        }

        /// <summary>
        /// Execute the datasource swazziling.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Telvent.Delivery.Framework.LicenseManager licenseManager = new Delivery.Framework.LicenseManager();

            try
            {
                //Initialize the license
                if (!licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo,mmLicensedProductCode.mmLPArcFM))
                {
                    MessageBox.Show("Failed to acquire ArcInfo License. Process is aborted."); return;
                }
                
                isClosed = false;
                btnOK.Enabled = false;
                //taking folder path and selected sde file path.
                string folderPath = textBoxMxdFolder.Text;
                string sdeFilePath = textBoxSdeFilePath.Text;
                // checking if folder path or sdefilepath is empty then prompting to select.
                if (string.IsNullOrEmpty(folderPath.Trim()) || string.IsNullOrEmpty(sdeFilePath.Trim()))
                {
                    MessageBox.Show("Either folder selection or sde file selection is not selected.");
                    btnOK.Enabled = true;
                    return;
                }
                string error = string.Empty;
                //Getting all mxd file name from the selected folderpath
                IFileNames fNames = Swizzle.GetFileNames(folderPath);
                int counter = 1;
                //counting the total mxd file found.
                string fName = fNames.Next();
                while (fName != null)
                {
                    counter++;
                    fName = fNames.Next();
                }
                //if not any mxd file found the showing message.
                if (counter < 2)
                {
                    MessageBox.Show("Map document file (mxd) not found in the folder : " + folderPath);
                    btnOK.Enabled = true;
                    return;
                }
                _logger.Debug("Total number of mxd file found = " + (counter - 1));
                //setting progress bar
                progressBar.Maximum = counter;
                lblProgressBar.Text = "Reading sde connection file";
                progressBar.Visible = true;
                lblProgressBar.Visible = true;
                progressBar.Value = 1;
                Application.DoEvents();
                //reading connection 
                IPropertySet connectionProperties = Swizzle.ReadConnection(sdeFilePath, out error);
                // if any error comes while reading connection like automation error or user or password not found in the sde connection file.
                //showing the message.
                if (error.Trim().Length > 0)
                {
                    MessageBox.Show(error);
                    btnOK.Enabled = true;
                    return;
                }

                //check if radio set single selected then get the connection properties.
                IPropertySet arcSDEConnectionProperties=  null;
                if (radSetSingle.Checked) 
                {
                    //setting the connection properties.
                    arcSDEConnectionProperties = new PropertySetClass();
                    arcSDEConnectionProperties.SetProperty("Server", txtBoxServer.Text.Trim());
                    arcSDEConnectionProperties.SetProperty("Instance", txtBoxService.Text.Trim());
                    arcSDEConnectionProperties.SetProperty("Database", txtBoxDatabase.Text.Trim());
                }
                //swizzling datasource of the feature layer and standalone table.
                swizzleLayersDataSource(arcSDEConnectionProperties, connectionProperties, fNames);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                lblProgressBar.Visible = false;
                btnOK.Enabled = true;
                Application.DoEvents();
                //Release the checkedout licenses
                licenseManager.Shutdown();
            }
        }

        /// <summary>
        /// loops throw each mxdfile path and calling the swizzle datasource for each mxd.
        /// </summary>
        /// <param name="originConnectionProperties">Connection to match with feature and standalone table source.</param>
        /// <param name="destinationConnectionProperties">sde connection property set.</param>
        /// <param name="fNames">All mxd file path</param>
        private void swizzleLayersDataSource(IPropertySet originConnectionProperties, IPropertySet destinationConnectionProperties, IFileNames fNames)
        {
            fNames.Reset();
            string fname = fNames.Next();
            while (fname != null)
            {
                progressBar.Value = progressBar.Value + 1;
                lblProgressBar.Text = "Processing (" + fname + ")";
                Application.DoEvents();
                //checking is user has clicked on cancel button or closed the form while processing.
                if (isClosed) return;
                //calling the function to change the datasource of the feature layer in an mxd file.
                Swizzle.FindAndReplaceSourceConnection(fname, destinationConnectionProperties,originConnectionProperties);
                fname = fNames.Next();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isClosed = true;
            this.Close();
            this.Dispose(true);
        }

        private void frmSwizzle_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosed = true;
        }

        private void radSetAll_CheckedChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "All layers and standalone table of the mxd documents will be set to selected sde file.";
            //hide the ArcSDE Database control.
            gbArcSDEDatabase.Hide();
            //clear all the text from ArcSDE database parameter
            txtBoxServer.Clear();
            txtBoxService.Clear();
            txtBoxDatabase.Clear();
           

            //move up the controls like progressbar, cancel and swizell button s and lblstatus.
           

            lblProgressBar.Location = new Point(12, 102);
            progressBar.Location = new Point(14, 129);
            btnOK.Location = new Point(298, 123);
            btnCancel.Location = new Point(358,123);
            this.Height = 180;

        }

        private void radSetSingle_CheckedChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "Only layers and standalone table matches the ArcSDE Database will be set to selected sde file.";

            gbArcSDEDatabase.Show();
            //move down the controls like progressbar, cancel and swizell button s and lblstatus.
            lblProgressBar.Location = new Point(12, 234);
            progressBar.Location = new Point(14, 261);
            btnOK.Location = new Point(298, 255);
            btnCancel.Location = new Point(358, 255);
            this.Height = 312;
        }
    }
}
