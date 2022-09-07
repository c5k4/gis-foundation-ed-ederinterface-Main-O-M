using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER.Login
{

    public partial class SessionManager : Form
    {
        #region Private Memebers
        string _originalProvider = string.Empty;
        #endregion

        #region CTOR
        public SessionManager()
        {
            InitializeComponent();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("MS Access OLEDB", "Microsoft.Jet.OLEDB.4.0");
            dict.Add("Oracle OLEDB", "OraOLEDB.Oracle");
            dict.Add("MS SQL Server OLEDB", "SQLOLEDB");
            cmbProvider.DisplayMember = "Key";
            cmbProvider.ValueMember = "Value";
            cmbProvider.SelectedIndexChanged -= new EventHandler(cmbProvider_SelectedIndexChanged);
            cmbProvider.DataSource = new BindingSource(dict, null);
            cmbProvider.SelectedIndexChanged += new EventHandler(cmbProvider_SelectedIndexChanged);
            SetupRegistryValues(true);
        }
        #endregion

        #region EVENTS

        private void chkAutoLogin_CheckedChanged(object sender, EventArgs e)
        {
            grp1grpAll.Enabled = chkAutoLogin.Checked;
            SetupRegistryValues(false);
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_originalProvider)) _originalProvider = MMRegistrySettings.Provider;
            MMRegistrySettings.Provider=cmbProvider.SelectedValue.ToString();
            SetupRegistryValues(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MMRegistrySettings.EnableAutoLogin = chkAutoLogin.Checked;
            AutoLogin = chkAutoLogin.Checked;
            MMRegistrySettings.Provider = cmbProvider.SelectedValue.ToString();
            Provider = cmbProvider.SelectedValue.ToString();
            if (cmbProvider.SelectedValue.ToString() == "SQLOLEDB")
            {
                MMRegistrySettings.SMServer = txtServer.Text;
                //Setup Server
                Server = txtServer.Text;
            }
            MMRegistrySettings.DataSource = txtNetService.Text;
            DataSource = txtNetService.Text;
            _originalProvider = string.Empty;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MMRegistrySettings.Provider = _originalProvider;
            this.Close();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets up the Registry value from teh User input on the forms
        /// </summary>
        /// <param name="verifyEnable"></param>
        private void SetupRegistryValues(bool verifyEnable)
        {
            string provider = MMRegistrySettings.Provider;
            Provider = provider;
            bool enableAutoLogin = MMRegistrySettings.EnableAutoLogin;
            AutoLogin = enableAutoLogin;

            if (!enableAutoLogin && verifyEnable)
            {
                chkAutoLogin.Checked = enableAutoLogin;
                grp1grpAll.Enabled = false;
                return;
            }
            if (provider != string.Empty)
            {
                switch (provider)
                {
                    case "OraOLEDB.Oracle":
                        btnBrowse.Visible = false;
                        txtServer.Text = string.Empty;
                        txtServer.Enabled = false;
                        lblServer.Enabled = false;
                        cmbProvider.SelectedIndex = 1;
                        txtServer.Text = string.Empty;
                        txtNetService.Text = MMRegistrySettings.DataSource;
                        DataSource = txtNetService.Text;
                        break;
                    case "Microsoft.Jet.OLEDB.4.0":
                        btnBrowse.Visible = true;
                        txtServer.Text = string.Empty; 
                        txtServer.Enabled = false;
                        lblServer.Enabled = false;
                        cmbProvider.SelectedIndex = 0;
                        txtNetService.Text = MMRegistrySettings.DataSource;
                        DataSource = txtNetService.Text;
                        txtServer.Text = string.Empty;
                        break;
                    case "SQLOLEDB":
                        btnBrowse.Visible = false;
                        txtServer.Enabled = true;
                        lblServer.Enabled = true;
                        cmbProvider.SelectedIndex = 2;
                        txtNetService.Text = MMRegistrySettings.DataSource;
                        DataSource = txtNetService.Text;
                        txtServer.Text = MMRegistrySettings.SMServer;
                        Server = txtNetService.Text;
                        break;
                }
            }
        }
        #endregion

        #region public properties
        /// <summary>
        /// Provider as selected by the user
        /// </summary>
        public string Provider
        {
            get;
            private set;
        }
        /// <summary>
        /// Datasource as selected by the user
        /// </summary>
        public string DataSource
        {
            get;
            private set;
        }
        /// <summary>
        /// Server as selected by the user
        /// </summary>
        public string Server
        {
            get;
            private set;
        }
        /// <summary>
        ///  EnableAutoLogin as selected by the user
        /// </summary>
        public bool AutoLogin
        {
            get;
            private set;
        }
        #endregion
    }
}
