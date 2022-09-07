using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace PGEElecCleanup
{
    public partial class frmSpatialDatabaseConnection : Form
    {
        clsGlobalFunctions _objClsGlobalFunctions = new clsGlobalFunctions();
        
        public frmSpatialDatabaseConnection()
        {
            InitializeComponent();
            LoadInitProperties();
        }

        private void LoadInitProperties()
        {

            txtTestServer.Text = "";
            //txtTestServices.Text = System.Configuration.ConfigurationSettings.AppSettings["SDEDATABASEINSTANCE"].ToString();
            //txtTestUsername.Text = "";
            //txtTestPassword.Text = "";
            txtTestDatabase.Text = "";
        }

        private void enableTestServerTextboxes(bool blnEnable)
        {
            txtTestServer.Enabled = blnEnable;
            txtTestServices.Enabled = blnEnable;
            txtTestUsername.Enabled = blnEnable;
            txtTestPassword.Enabled = blnEnable;
            txtTestDatabase.Enabled = blnEnable;
        }

        private void cmbTestVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTestTestConnection.Enabled = true;
        }

        private void cmbTestVersion_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                string strUserName = string.Empty;
                string strPassword = string.Empty;
                string strServer = string.Empty;
                string strService = string.Empty;
                string strDatabase = string.Empty;
                string strVersion = string.Empty;

                strServer = txtTestServer.Text;
                strService = txtTestServices.Text;
                strUserName = txtTestUsername.Text;
                strPassword = txtTestPassword.Text;
                strVersion = ConstantValues.consdedefault;
                strDatabase = txtTestDatabase.Text;

                //if ((strService != string.Empty) && (strPassword != string.Empty) && (strServer != string.Empty) && (strUserName != string.Empty) && (strDatabase != string.Empty))
                {
                    Boolean blnStatus = _objClsGlobalFunctions.loadVersionInCombobox(strUserName, strPassword, strServer, strService, strVersion, strDatabase, cmbTestVersion);

                    if (blnStatus == true)
                    {
                        btnTestTestConnection.Enabled = true;
                    }
                    else
                    {
                        if (cmbTestVersion.Items.Count == 0)
                        {
                            MessageBox.Show("Unable to get the versions for the given details..Please verify the details.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Unable to get the versions for the given details..Please verify the details.","Connection Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnTestTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                string strUserName = string.Empty;
                string strPassword = string.Empty;
                string strServer = string.Empty;
                string strService = string.Empty;
                string strDatabase = string.Empty;
                string strVersion = string.Empty;


                strServer = txtTestServer.Text;
                strService = txtTestServices.Text;
                strUserName = txtTestUsername.Text;
                strPassword = txtTestPassword.Text;
                strVersion = cmbTestVersion.SelectedItem.ToString();
                strDatabase = txtTestDatabase.Text;

                clsGeoDBconnections objGeodbCon = new clsGeoDBconnections();
                objGeodbCon.initTestConProperties(strUserName, strPassword, strServer, strService, strVersion, strDatabase);

                Boolean blnStatus = clsTestWorkSpace.initSDEWorkspace(clsGeoDBconnections.pTestConProperties);

                if (blnStatus == true)
                {
                    MessageBox.Show("Connection success", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    enableTestServerTextboxes(false);
                    btnCancel.Enabled = false;
                    btnOk.Enabled = true;
                    clsTestWorkSpace.State = true;
                }
                else
                {
                    MessageBox.Show("Unable to connect to SDE..Please verify the details.", ConstantValues.conApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch(Exception Ex)
            {
               MessageBox.Show("Failed to connect to Source SDE ..Please check the details. " + Ex.Message, ConstantValues.conApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
          this.Close();
        }

        private void frmSpatialDatabaseConnection_Load(object sender, EventArgs e)
        {
            btnOk.Enabled = false;
            btnCancel.Enabled = false;
        }

    }
}