namespace Telvent.PGE.MapProduction
{
    partial class UIMapProductionConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpBoxDBSetting = new System.Windows.Forms.GroupBox();
            this.lblDBServer = new System.Windows.Forms.Label();
            this.txtDBServer = new System.Windows.Forms.TextBox();
            this.lblDatasource = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.grpBoxGDBSettings = new System.Windows.Forms.GroupBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblInstance = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.grpBoxCredentials = new System.Windows.Forms.GroupBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.dgSettings = new System.Windows.Forms.DataGridView();
            this.SettingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkShowChar = new System.Windows.Forms.CheckBox();
            this.grpBoxDBSetting.SuspendLayout();
            this.grpBoxGDBSettings.SuspendLayout();
            this.grpBoxCredentials.SuspendLayout();
            this.grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBoxDBSetting
            // 
            this.grpBoxDBSetting.Controls.Add(this.lblDBServer);
            this.grpBoxDBSetting.Controls.Add(this.txtDBServer);
            this.grpBoxDBSetting.Controls.Add(this.lblDatasource);
            this.grpBoxDBSetting.Controls.Add(this.txtDataSource);
            this.grpBoxDBSetting.Location = new System.Drawing.Point(9, 97);
            this.grpBoxDBSetting.Name = "grpBoxDBSetting";
            this.grpBoxDBSetting.Size = new System.Drawing.Size(250, 75);
            this.grpBoxDBSetting.TabIndex = 0;
            this.grpBoxDBSetting.TabStop = false;
            this.grpBoxDBSetting.Text = "Database Setting";
            // 
            // lblDBServer
            // 
            this.lblDBServer.AutoSize = true;
            this.lblDBServer.Location = new System.Drawing.Point(48, 48);
            this.lblDBServer.Name = "lblDBServer";
            this.lblDBServer.Size = new System.Drawing.Size(38, 13);
            this.lblDBServer.TabIndex = 15;
            this.lblDBServer.Text = "Server";
            // 
            // txtDBServer
            // 
            this.txtDBServer.Location = new System.Drawing.Point(89, 45);
            this.txtDBServer.Name = "txtDBServer";
            this.txtDBServer.Size = new System.Drawing.Size(143, 20);
            this.txtDBServer.TabIndex = 14;
            // 
            // lblDatasource
            // 
            this.lblDatasource.AutoSize = true;
            this.lblDatasource.Location = new System.Drawing.Point(19, 22);
            this.lblDatasource.Name = "lblDatasource";
            this.lblDatasource.Size = new System.Drawing.Size(67, 13);
            this.lblDatasource.TabIndex = 13;
            this.lblDatasource.Text = "Data Source";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(89, 19);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(143, 20);
            this.txtDataSource.TabIndex = 6;
            // 
            // grpBoxGDBSettings
            // 
            this.grpBoxGDBSettings.Controls.Add(this.lblDatabase);
            this.grpBoxGDBSettings.Controls.Add(this.lblVersion);
            this.grpBoxGDBSettings.Controls.Add(this.lblInstance);
            this.grpBoxGDBSettings.Controls.Add(this.lblServer);
            this.grpBoxGDBSettings.Controls.Add(this.txtDatabase);
            this.grpBoxGDBSettings.Controls.Add(this.txtVersion);
            this.grpBoxGDBSettings.Controls.Add(this.txtInstance);
            this.grpBoxGDBSettings.Controls.Add(this.txtServer);
            this.grpBoxGDBSettings.Location = new System.Drawing.Point(262, 12);
            this.grpBoxGDBSettings.Name = "grpBoxGDBSettings";
            this.grpBoxGDBSettings.Size = new System.Drawing.Size(250, 160);
            this.grpBoxGDBSettings.TabIndex = 1;
            this.grpBoxGDBSettings.TabStop = false;
            this.grpBoxGDBSettings.Text = "Geodatabase Settings";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(33, 109);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(53, 13);
            this.lblDatabase.TabIndex = 17;
            this.lblDatabase.Text = "Database";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(44, 80);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(42, 13);
            this.lblVersion.TabIndex = 16;
            this.lblVersion.Text = "Version";
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Location = new System.Drawing.Point(38, 51);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(48, 13);
            this.lblInstance.TabIndex = 15;
            this.lblInstance.Text = "Instance";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(48, 22);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(38, 13);
            this.lblServer.TabIndex = 14;
            this.lblServer.Text = "Server";
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(89, 106);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(143, 20);
            this.txtDatabase.TabIndex = 10;
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(89, 77);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(143, 20);
            this.txtVersion.TabIndex = 9;
            this.txtVersion.Text = "SDE.DEFAULT";
            // 
            // txtInstance
            // 
            this.txtInstance.Location = new System.Drawing.Point(89, 48);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.Size = new System.Drawing.Size(143, 20);
            this.txtInstance.TabIndex = 8;
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(89, 19);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(143, 20);
            this.txtServer.TabIndex = 7;
            // 
            // grpBoxCredentials
            // 
            this.grpBoxCredentials.Controls.Add(this.chkShowChar);
            this.grpBoxCredentials.Controls.Add(this.lblPassword);
            this.grpBoxCredentials.Controls.Add(this.lblUserName);
            this.grpBoxCredentials.Controls.Add(this.txtPassword);
            this.grpBoxCredentials.Controls.Add(this.txtUserName);
            this.grpBoxCredentials.Location = new System.Drawing.Point(6, 6);
            this.grpBoxCredentials.Name = "grpBoxCredentials";
            this.grpBoxCredentials.Size = new System.Drawing.Size(250, 85);
            this.grpBoxCredentials.TabIndex = 1;
            this.grpBoxCredentials.TabStop = false;
            this.grpBoxCredentials.Text = "User Credentials";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(33, 45);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 12;
            this.lblPassword.Text = "Password";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(26, 22);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(60, 13);
            this.lblUserName.TabIndex = 11;
            this.lblUserName.Text = "User Name";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(89, 42);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(143, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(89, 19);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(143, 20);
            this.txtUserName.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(432, 224);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(432, 279);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.dgSettings);
            this.grpSettings.Location = new System.Drawing.Point(6, 178);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(420, 153);
            this.grpSettings.TabIndex = 4;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // dgSettings
            // 
            this.dgSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SettingName,
            this.SettingValue});
            this.dgSettings.Location = new System.Drawing.Point(3, 16);
            this.dgSettings.Name = "dgSettings";
            this.dgSettings.Size = new System.Drawing.Size(411, 131);
            this.dgSettings.TabIndex = 0;
            // 
            // SettingName
            // 
            this.SettingName.HeaderText = "Name";
            this.SettingName.Name = "SettingName";
            this.SettingName.Width = 150;
            // 
            // SettingValue
            // 
            this.SettingValue.HeaderText = "Value";
            this.SettingValue.Name = "SettingValue";
            this.SettingValue.Width = 200;
            // 
            // chkShowChar
            // 
            this.chkShowChar.AutoSize = true;
            this.chkShowChar.Location = new System.Drawing.Point(89, 65);
            this.chkShowChar.Name = "chkShowChar";
            this.chkShowChar.Size = new System.Drawing.Size(107, 17);
            this.chkShowChar.TabIndex = 13;
            this.chkShowChar.Text = "Show Characters";
            this.chkShowChar.UseVisualStyleBackColor = true;
            this.chkShowChar.CheckedChanged += new System.EventHandler(this.chkShowChar_CheckedChanged);
            // 
            // UIMapProductionConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 343);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpBoxCredentials);
            this.Controls.Add(this.grpBoxGDBSettings);
            this.Controls.Add(this.grpBoxDBSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "UIMapProductionConfig";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Map Production Configuration";
            this.TopMost = true;
            this.grpBoxDBSetting.ResumeLayout(false);
            this.grpBoxDBSetting.PerformLayout();
            this.grpBoxGDBSettings.ResumeLayout(false);
            this.grpBoxGDBSettings.PerformLayout();
            this.grpBoxCredentials.ResumeLayout(false);
            this.grpBoxCredentials.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBoxDBSetting;
        private System.Windows.Forms.GroupBox grpBoxGDBSettings;
        private System.Windows.Forms.GroupBox grpBoxCredentials;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblDatasource;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblDBServer;
        private System.Windows.Forms.TextBox txtDBServer;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.DataGridView dgSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingValue;
        private System.Windows.Forms.CheckBox chkShowChar;
    }
}