namespace PGE.BatchApplication.FGDBExtraction
{
    partial class ExtractDataForm
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
            this.btnConnectDatabase = new System.Windows.Forms.Button();
            this.txtSDEConnection = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFGDBLocation = new System.Windows.Forms.TextBox();
            this.cboDivision = new System.Windows.Forms.ComboBox();
            this.lblPolygon = new System.Windows.Forms.Label();
            this.pnlExtraction = new System.Windows.Forms.Panel();
            this.btnExtract = new System.Windows.Forms.Button();
            this.treeDatabase = new System.Windows.Forms.TreeView();
            this.lblPrimaryProgress = new System.Windows.Forms.Label();
            this.prgPrimaryProgress = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLandbaseConnection = new System.Windows.Forms.TextBox();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.lblSecondaryProgress = new System.Windows.Forms.Label();
            this.prgSecondaryProgress = new System.Windows.Forms.ProgressBar();
            this.lblProcessLogging = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExtractionPolygon = new System.Windows.Forms.TextBox();
            this.lblExtractionField = new System.Windows.Forms.Label();
            this.txtExtractionField = new System.Windows.Forms.TextBox();
            this.pnlExtraction.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnectDatabase
            // 
            this.btnConnectDatabase.Location = new System.Drawing.Point(12, 168);
            this.btnConnectDatabase.Name = "btnConnectDatabase";
            this.btnConnectDatabase.Size = new System.Drawing.Size(632, 23);
            this.btnConnectDatabase.TabIndex = 0;
            this.btnConnectDatabase.Text = "Connect Database";
            this.btnConnectDatabase.UseVisualStyleBackColor = true;
            this.btnConnectDatabase.Click += new System.EventHandler(this.btnConnectDatabase_Click);
            // 
            // txtSDEConnection
            // 
            this.txtSDEConnection.Location = new System.Drawing.Point(12, 25);
            this.txtSDEConnection.Name = "txtSDEConnection";
            this.txtSDEConnection.Size = new System.Drawing.Size(632, 20);
            this.txtSDEConnection.TabIndex = 1;
            this.txtSDEConnection.Text = "EDGIS@EDGISG1Q.sde";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "GIS Connection File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Extraction Location";
            // 
            // txtFGDBLocation
            // 
            this.txtFGDBLocation.Location = new System.Drawing.Point(12, 103);
            this.txtFGDBLocation.Name = "txtFGDBLocation";
            this.txtFGDBLocation.Size = new System.Drawing.Size(632, 20);
            this.txtFGDBLocation.TabIndex = 3;
            this.txtFGDBLocation.Text = "EightMiles";
            // 
            // cboDivision
            // 
            this.cboDivision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDivision.FormattingEnabled = true;
            this.cboDivision.Location = new System.Drawing.Point(14, 28);
            this.cboDivision.Name = "cboDivision";
            this.cboDivision.Size = new System.Drawing.Size(220, 21);
            this.cboDivision.TabIndex = 5;
            // 
            // lblPolygon
            // 
            this.lblPolygon.AutoSize = true;
            this.lblPolygon.Location = new System.Drawing.Point(14, 12);
            this.lblPolygon.Name = "lblPolygon";
            this.lblPolygon.Size = new System.Drawing.Size(149, 13);
            this.lblPolygon.TabIndex = 6;
            this.lblPolygon.Text = "EDGIS.UDC_Extract_Polygon";
            // 
            // pnlExtraction
            // 
            this.pnlExtraction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlExtraction.Controls.Add(this.btnExtract);
            this.pnlExtraction.Controls.Add(this.treeDatabase);
            this.pnlExtraction.Controls.Add(this.lblPolygon);
            this.pnlExtraction.Controls.Add(this.cboDivision);
            this.pnlExtraction.Enabled = false;
            this.pnlExtraction.Location = new System.Drawing.Point(12, 197);
            this.pnlExtraction.Name = "pnlExtraction";
            this.pnlExtraction.Size = new System.Drawing.Size(632, 366);
            this.pnlExtraction.TabIndex = 9;
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(14, 333);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(591, 23);
            this.btnExtract.TabIndex = 10;
            this.btnExtract.Text = "Extract Data";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // treeDatabase
            // 
            this.treeDatabase.CheckBoxes = true;
            this.treeDatabase.Location = new System.Drawing.Point(14, 55);
            this.treeDatabase.Name = "treeDatabase";
            this.treeDatabase.Size = new System.Drawing.Size(591, 272);
            this.treeDatabase.TabIndex = 9;
            // 
            // lblPrimaryProgress
            // 
            this.lblPrimaryProgress.Location = new System.Drawing.Point(3, 479);
            this.lblPrimaryProgress.Name = "lblPrimaryProgress";
            this.lblPrimaryProgress.Size = new System.Drawing.Size(723, 27);
            this.lblPrimaryProgress.TabIndex = 10;
            // 
            // prgPrimaryProgress
            // 
            this.prgPrimaryProgress.Location = new System.Drawing.Point(3, 509);
            this.prgPrimaryProgress.Name = "prgPrimaryProgress";
            this.prgPrimaryProgress.Size = new System.Drawing.Size(723, 23);
            this.prgPrimaryProgress.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Landbase Connection File";
            // 
            // txtLandbaseConnection
            // 
            this.txtLandbaseConnection.Location = new System.Drawing.Point(12, 64);
            this.txtLandbaseConnection.Name = "txtLandbaseConnection";
            this.txtLandbaseConnection.Size = new System.Drawing.Size(632, 20);
            this.txtLandbaseConnection.TabIndex = 13;
            this.txtLandbaseConnection.Text = "default.gdb";
            // 
            // pnlProgress
            // 
            this.pnlProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlProgress.Controls.Add(this.lblSecondaryProgress);
            this.pnlProgress.Controls.Add(this.prgSecondaryProgress);
            this.pnlProgress.Controls.Add(this.lblProcessLogging);
            this.pnlProgress.Controls.Add(this.lblPrimaryProgress);
            this.pnlProgress.Controls.Add(this.prgPrimaryProgress);
            this.pnlProgress.Location = new System.Drawing.Point(650, 25);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(731, 538);
            this.pnlProgress.TabIndex = 15;
            // 
            // lblSecondaryProgress
            // 
            this.lblSecondaryProgress.Location = new System.Drawing.Point(3, 423);
            this.lblSecondaryProgress.Name = "lblSecondaryProgress";
            this.lblSecondaryProgress.Size = new System.Drawing.Size(723, 27);
            this.lblSecondaryProgress.TabIndex = 13;
            // 
            // prgSecondaryProgress
            // 
            this.prgSecondaryProgress.Location = new System.Drawing.Point(3, 453);
            this.prgSecondaryProgress.Name = "prgSecondaryProgress";
            this.prgSecondaryProgress.Size = new System.Drawing.Size(723, 23);
            this.prgSecondaryProgress.TabIndex = 14;
            // 
            // lblProcessLogging
            // 
            this.lblProcessLogging.Location = new System.Drawing.Point(6, 3);
            this.lblProcessLogging.Multiline = true;
            this.lblProcessLogging.Name = "lblProcessLogging";
            this.lblProcessLogging.ReadOnly = true;
            this.lblProcessLogging.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.lblProcessLogging.Size = new System.Drawing.Size(720, 417);
            this.lblProcessLogging.TabIndex = 12;
            this.lblProcessLogging.TextChanged += new System.EventHandler(this.txtLogs_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Extraction Polygon";
            // 
            // txtExtractionPolygon
            // 
            this.txtExtractionPolygon.Location = new System.Drawing.Point(12, 142);
            this.txtExtractionPolygon.Name = "txtExtractionPolygon";
            this.txtExtractionPolygon.Size = new System.Drawing.Size(287, 20);
            this.txtExtractionPolygon.TabIndex = 16;
            this.txtExtractionPolygon.Text = "Export_Output1";
            this.txtExtractionPolygon.TextChanged += new System.EventHandler(this.txtExtractionPolygon_TextChanged);
            // 
            // lblExtractionField
            // 
            this.lblExtractionField.AutoSize = true;
            this.lblExtractionField.Location = new System.Drawing.Point(305, 126);
            this.lblExtractionField.Name = "lblExtractionField";
            this.lblExtractionField.Size = new System.Drawing.Size(151, 13);
            this.lblExtractionField.TabIndex = 19;
            this.lblExtractionField.Text = "Extraction Polygon Field Name";
            // 
            // txtExtractionField
            // 
            this.txtExtractionField.Location = new System.Drawing.Point(305, 142);
            this.txtExtractionField.Name = "txtExtractionField";
            this.txtExtractionField.Size = new System.Drawing.Size(287, 20);
            this.txtExtractionField.TabIndex = 18;
            this.txtExtractionField.Text = "Id";
            this.txtExtractionField.TextChanged += new System.EventHandler(this.txtExtractionField_TextChanged);
            // 
            // ExtractDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 584);
            this.Controls.Add(this.lblExtractionField);
            this.Controls.Add(this.txtExtractionField);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtExtractionPolygon);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLandbaseConnection);
            this.Controls.Add(this.pnlExtraction);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFGDBLocation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSDEConnection);
            this.Controls.Add(this.btnConnectDatabase);
            this.Name = "ExtractDataForm";
            this.Text = "Data Extraction";
            this.pnlExtraction.ResumeLayout(false);
            this.pnlExtraction.PerformLayout();
            this.pnlProgress.ResumeLayout(false);
            this.pnlProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnectDatabase;
        private System.Windows.Forms.TextBox txtSDEConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFGDBLocation;
        private System.Windows.Forms.ComboBox cboDivision;
        private System.Windows.Forms.Label lblPolygon;
        private System.Windows.Forms.Panel pnlExtraction;
        private System.Windows.Forms.TreeView treeDatabase;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label lblPrimaryProgress;
        private System.Windows.Forms.ProgressBar prgPrimaryProgress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLandbaseConnection;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.TextBox lblProcessLogging;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExtractionPolygon;
        private System.Windows.Forms.Label lblExtractionField;
        private System.Windows.Forms.TextBox txtExtractionField;
        private System.Windows.Forms.Label lblSecondaryProgress;
        private System.Windows.Forms.ProgressBar prgSecondaryProgress;
    }
}