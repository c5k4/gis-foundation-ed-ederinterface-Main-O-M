namespace Telvent.PGE.MapProduction.SwizzleLayers
{
    partial class SwizzleLayersUI
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
            this.btnSelectSdeFile = new System.Windows.Forms.Button();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSelectSDEFile = new System.Windows.Forms.Label();
            this.folderBrowserDialogMXD = new System.Windows.Forms.FolderBrowserDialog();
            this.sdeOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblSelectFolder = new System.Windows.Forms.Label();
            this.textBoxSdeFilePath = new System.Windows.Forms.TextBox();
            this.textBoxMxdFolder = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressBar = new System.Windows.Forms.Label();
            this.gbArcSDEDatabase = new System.Windows.Forms.GroupBox();
            this.txtBoxDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.txtBoxService = new System.Windows.Forms.TextBox();
            this.lblService = new System.Windows.Forms.Label();
            this.txtBoxServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.radSetAll = new System.Windows.Forms.RadioButton();
            this.radSetSingle = new System.Windows.Forms.RadioButton();
            this.lblMessage = new System.Windows.Forms.Label();
            this.gbArcSDEDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectSdeFile
            // 
            this.btnSelectSdeFile.Location = new System.Drawing.Point(386, 68);
            this.btnSelectSdeFile.Name = "btnSelectSdeFile";
            this.btnSelectSdeFile.Size = new System.Drawing.Size(25, 21);
            this.btnSelectSdeFile.TabIndex = 0;
            this.btnSelectSdeFile.Text = "...";
            this.btnSelectSdeFile.UseVisualStyleBackColor = true;
            this.btnSelectSdeFile.Click += new System.EventHandler(this.btnSelectSdeFile_Click);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(386, 40);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(25, 21);
            this.btnSelectFolder.TabIndex = 1;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(298, 123);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Swizzle";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(358, 123);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSelectSDEFile
            // 
            this.lblSelectSDEFile.AutoSize = true;
            this.lblSelectSDEFile.Location = new System.Drawing.Point(10, 73);
            this.lblSelectSDEFile.Name = "lblSelectSDEFile";
            this.lblSelectSDEFile.Size = new System.Drawing.Size(78, 13);
            this.lblSelectSDEFile.TabIndex = 5;
            this.lblSelectSDEFile.Text = "Select SDE file";
            // 
            // sdeOpenFileDialog
            // 
            this.sdeOpenFileDialog.Filter = "\"Sde files (*.sde)|*.sde|All files (*.*)|*.*\"";
            // 
            // lblSelectFolder
            // 
            this.lblSelectFolder.AutoSize = true;
            this.lblSelectFolder.Location = new System.Drawing.Point(11, 44);
            this.lblSelectFolder.Name = "lblSelectFolder";
            this.lblSelectFolder.Size = new System.Drawing.Size(66, 13);
            this.lblSelectFolder.TabIndex = 6;
            this.lblSelectFolder.Text = "Select folder";
            // 
            // textBoxSdeFilePath
            // 
            this.textBoxSdeFilePath.Location = new System.Drawing.Point(92, 70);
            this.textBoxSdeFilePath.Name = "textBoxSdeFilePath";
            this.textBoxSdeFilePath.ReadOnly = true;
            this.textBoxSdeFilePath.Size = new System.Drawing.Size(288, 20);
            this.textBoxSdeFilePath.TabIndex = 7;
            // 
            // textBoxMxdFolder
            // 
            this.textBoxMxdFolder.Location = new System.Drawing.Point(92, 41);
            this.textBoxMxdFolder.Name = "textBoxMxdFolder";
            this.textBoxMxdFolder.ReadOnly = true;
            this.textBoxMxdFolder.Size = new System.Drawing.Size(288, 20);
            this.textBoxMxdFolder.TabIndex = 8;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(14, 129);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(278, 17);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 9;
            this.progressBar.Visible = false;
            // 
            // lblProgressBar
            // 
            this.lblProgressBar.AutoSize = true;
            this.lblProgressBar.Location = new System.Drawing.Point(12, 102);
            this.lblProgressBar.Name = "lblProgressBar";
            this.lblProgressBar.Size = new System.Drawing.Size(0, 13);
            this.lblProgressBar.TabIndex = 10;
            // 
            // gbArcSDEDatabase
            // 
            this.gbArcSDEDatabase.Controls.Add(this.txtBoxDatabase);
            this.gbArcSDEDatabase.Controls.Add(this.lblDatabase);
            this.gbArcSDEDatabase.Controls.Add(this.txtBoxService);
            this.gbArcSDEDatabase.Controls.Add(this.lblService);
            this.gbArcSDEDatabase.Controls.Add(this.txtBoxServer);
            this.gbArcSDEDatabase.Controls.Add(this.lblServer);
            this.gbArcSDEDatabase.Location = new System.Drawing.Point(15, 95);
            this.gbArcSDEDatabase.Name = "gbArcSDEDatabase";
            this.gbArcSDEDatabase.Size = new System.Drawing.Size(388, 115);
            this.gbArcSDEDatabase.TabIndex = 12;
            this.gbArcSDEDatabase.TabStop = false;
            this.gbArcSDEDatabase.Text = "ArcSDE Database";
            this.gbArcSDEDatabase.Visible = false;
            // 
            // txtBoxDatabase
            // 
            this.txtBoxDatabase.Location = new System.Drawing.Point(72, 76);
            this.txtBoxDatabase.Name = "txtBoxDatabase";
            this.txtBoxDatabase.Size = new System.Drawing.Size(293, 20);
            this.txtBoxDatabase.TabIndex = 5;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 79);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(56, 13);
            this.lblDatabase.TabIndex = 4;
            this.lblDatabase.Text = "Database:";
            // 
            // txtBoxService
            // 
            this.txtBoxService.Location = new System.Drawing.Point(72, 50);
            this.txtBoxService.Name = "txtBoxService";
            this.txtBoxService.Size = new System.Drawing.Size(293, 20);
            this.txtBoxService.TabIndex = 3;
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(6, 53);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(46, 13);
            this.lblService.TabIndex = 2;
            this.lblService.Text = "Service:";
            // 
            // txtBoxServer
            // 
            this.txtBoxServer.Location = new System.Drawing.Point(72, 24);
            this.txtBoxServer.Name = "txtBoxServer";
            this.txtBoxServer.Size = new System.Drawing.Size(293, 20);
            this.txtBoxServer.TabIndex = 1;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(6, 27);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // radSetAll
            // 
            this.radSetAll.AutoSize = true;
            this.radSetAll.Checked = true;
            this.radSetAll.Location = new System.Drawing.Point(14, 15);
            this.radSetAll.Name = "radSetAll";
            this.radSetAll.Size = new System.Drawing.Size(55, 17);
            this.radSetAll.TabIndex = 13;
            this.radSetAll.TabStop = true;
            this.radSetAll.Text = "Set All";
            this.radSetAll.UseVisualStyleBackColor = true;
            this.radSetAll.CheckedChanged += new System.EventHandler(this.radSetAll_CheckedChanged);
            // 
            // radSetSingle
            // 
            this.radSetSingle.AutoSize = true;
            this.radSetSingle.Location = new System.Drawing.Point(75, 15);
            this.radSetSingle.Name = "radSetSingle";
            this.radSetSingle.Size = new System.Drawing.Size(73, 17);
            this.radSetSingle.TabIndex = 14;
            this.radSetSingle.Text = "Set Single";
            this.radSetSingle.UseVisualStyleBackColor = true;
            this.radSetSingle.CheckedChanged += new System.EventHandler(this.radSetSingle_CheckedChanged);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(154, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(257, 34);
            this.lblMessage.TabIndex = 15;
            this.lblMessage.Text = "All layers and standalone table of the mxd documents will be set to selected sde " +
    "file.";
            // 
            // SwizzleLayersUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 155);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.radSetSingle);
            this.Controls.Add(this.radSetAll);
            this.Controls.Add(this.lblProgressBar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.textBoxMxdFolder);
            this.Controls.Add(this.textBoxSdeFilePath);
            this.Controls.Add(this.lblSelectFolder);
            this.Controls.Add(this.lblSelectSDEFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnSelectSdeFile);
            this.Controls.Add(this.gbArcSDEDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SwizzleLayersUI";
            this.Text = "Swizzle Layer";
            this.gbArcSDEDatabase.ResumeLayout(false);
            this.gbArcSDEDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectSdeFile;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSelectSDEFile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogMXD;
        private System.Windows.Forms.OpenFileDialog sdeOpenFileDialog;
        private System.Windows.Forms.Label lblSelectFolder;
        private System.Windows.Forms.TextBox textBoxSdeFilePath;
        private System.Windows.Forms.TextBox textBoxMxdFolder;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgressBar;
        private System.Windows.Forms.GroupBox gbArcSDEDatabase;
        private System.Windows.Forms.TextBox txtBoxDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox txtBoxService;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.TextBox txtBoxServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.RadioButton radSetAll;
        private System.Windows.Forms.RadioButton radSetSingle;
        private System.Windows.Forms.Label lblMessage;
    }
}

