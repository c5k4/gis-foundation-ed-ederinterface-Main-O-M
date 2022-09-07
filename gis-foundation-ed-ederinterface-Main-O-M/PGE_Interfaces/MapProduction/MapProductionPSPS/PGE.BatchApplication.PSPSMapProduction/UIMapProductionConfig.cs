using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// UI for Configuring the PSPSMapProduction Toolset
    /// </summary>
    public partial class UIMapProductionConfig : Form
    {
        /// <summary>
        /// Constructor Initializes UI Components
        /// </summary>
        public UIMapProductionConfig()
        {
            InitializeComponent();
            txtUserName.Text = MapProductionConfigurationHandler.UserName;
            txtPassword.Text = MapProductionConfigurationHandler.Password;
            txtDataSource.Text = MapProductionConfigurationHandler.DataSource;
            //txtDBServer.Text = MapProductionConfigurationHandler.DatabaseServer;
            txtServer.Text = MapProductionConfigurationHandler.GeodatabaseSetting.Server;
            txtInstance.Text = MapProductionConfigurationHandler.GeodatabaseSetting.Instance;
            txtVersion.Text = MapProductionConfigurationHandler.GeodatabaseSetting.Version;
            txtDatabase.Text = MapProductionConfigurationHandler.GeodatabaseSetting.Database;
            foreach(KeyValuePair<string,string> setting in MapProductionConfigurationHandler.Settings)
            {
                dgSettings.Rows.Add(setting.Key, setting.Value);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MapProductionConfigurationHandler.UserName = txtUserName.Text;
            MapProductionConfigurationHandler.Password = txtPassword.Text;
            MapProductionConfigurationHandler.DataSource = txtDataSource.Text;
           // MapProductionConfigurationHandler.DatabaseServer = txtDBServer.Text;
            MapProductionConfigurationHandler.GeodatabaseSetting.Server = txtServer.Text;
            MapProductionConfigurationHandler.GeodatabaseSetting.Instance = txtInstance.Text;
            MapProductionConfigurationHandler.GeodatabaseSetting.Version = txtVersion.Text;
            MapProductionConfigurationHandler.GeodatabaseSetting.Database = txtDatabase.Text;
            MapProductionConfigurationHandler.ClearAllSetting();
            foreach (DataGridViewRow dgRow in  dgSettings.Rows)
            {
                if (dgRow.Cells[0].Value == null)
                    continue;
                MapProductionConfigurationHandler.AddSetting(dgRow.Cells[0].Value.ToString(), dgRow.Cells[1].Value==null?string.Empty:dgRow.Cells[1].Value.ToString());
            }
            MapProductionConfigurationHandler.SaveConfiguration();
        }

        private void btnAddSetting_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkShowChar_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowChar.Checked;
        }
    }
}
