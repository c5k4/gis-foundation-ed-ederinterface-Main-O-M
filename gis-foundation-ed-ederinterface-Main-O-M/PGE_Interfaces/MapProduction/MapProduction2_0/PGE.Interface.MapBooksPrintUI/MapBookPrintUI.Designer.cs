namespace PGE.Interfaces.MapBooksPrintUI
{
    partial class MapBookPrintUI
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
            this.lblDistrict = new System.Windows.Forms.Label();
            this.lblDivision = new System.Windows.Forms.Label();
            this.lblRegion = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.lblMapType = new System.Windows.Forms.Label();
            this.btnPrintPDFs = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.lblPageSize = new System.Windows.Forms.Label();
            this.divider = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblGridNumber = new System.Windows.Forms.Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.cbPrintMultiple = new System.Windows.Forms.CheckBox();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gridLoading = new System.Windows.Forms.ProgressBar();
            this.cboRegion = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboDivision = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboDistrict = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboGridNumber = new MapBooksPrintUI.Controls.MultiSelectCheckBox();
            this.cboScale = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboMapType = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDistrict
            // 
            this.lblDistrict.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDistrict.Location = new System.Drawing.Point(12, 92);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(170, 13);
            this.lblDistrict.TabIndex = 7;
            this.lblDistrict.Text = "Filter District:";
            this.lblDistrict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDivision
            // 
            this.lblDivision.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDivision.Location = new System.Drawing.Point(207, 41);
            this.lblDivision.Name = "lblDivision";
            this.lblDivision.Size = new System.Drawing.Size(170, 13);
            this.lblDivision.TabIndex = 5;
            this.lblDivision.Text = "Filter Division:";
            this.lblDivision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRegion
            // 
            this.lblRegion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegion.Location = new System.Drawing.Point(12, 41);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(170, 13);
            this.lblRegion.TabIndex = 3;
            this.lblRegion.Text = "Filter Region:";
            this.lblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblScale
            // 
            this.lblScale.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScale.Location = new System.Drawing.Point(12, 118);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(170, 13);
            this.lblScale.TabIndex = 10;
            this.lblScale.Text = "Select Map Scale:";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMapType
            // 
            this.lblMapType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapType.Location = new System.Drawing.Point(12, 27);
            this.lblMapType.Name = "lblMapType";
            this.lblMapType.Size = new System.Drawing.Size(170, 13);
            this.lblMapType.TabIndex = 1;
            this.lblMapType.Text = "Select Map Type:";
            this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnPrintPDFs
            // 
            this.btnPrintPDFs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintPDFs.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnPrintPDFs.Location = new System.Drawing.Point(167, 188);
            this.btnPrintPDFs.Name = "btnPrintPDFs";
            this.btnPrintPDFs.Size = new System.Drawing.Size(145, 26);
            this.btnPrintPDFs.TabIndex = 9;
            this.btnPrintPDFs.Text = "&Print Standard Map";
            this.btnPrintPDFs.UseVisualStyleBackColor = true;
            this.btnPrintPDFs.Click += new System.EventHandler(this.btnPrintPDFs_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // cboPageSize
            // 
            this.cboPageSize.BackColor = System.Drawing.SystemColors.Window;
            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPageSize.FormattingEnabled = true;
            this.cboPageSize.Location = new System.Drawing.Point(207, 43);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(170, 24);
            this.cboPageSize.Sorted = true;
            this.cboPageSize.TabIndex = 1;
            // 
            // lblPageSize
            // 
            this.lblPageSize.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageSize.Location = new System.Drawing.Point(207, 27);
            this.lblPageSize.Name = "lblPageSize";
            this.lblPageSize.Size = new System.Drawing.Size(170, 13);
            this.lblPageSize.TabIndex = 13;
            this.lblPageSize.Text = "Select Page Size:";
            this.lblPageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // divider
            // 
            this.divider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.divider.Location = new System.Drawing.Point(10, 93);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(367, 2);
            this.divider.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(367, 2);
            this.label1.TabIndex = 15;
            // 
            // lblGridNumber
            // 
            this.lblGridNumber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridNumber.Location = new System.Drawing.Point(207, 118);
            this.lblGridNumber.Name = "lblGridNumber";
            this.lblGridNumber.Size = new System.Drawing.Size(170, 13);
            this.lblGridNumber.TabIndex = 16;
            this.lblGridNumber.Text = "Select Grid Number:";
            this.lblGridNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlFilter
            // 
            this.pnlFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFilter.Controls.Add(this.cbPrintMultiple);
            this.pnlFilter.Controls.Add(this.label1);
            this.pnlFilter.Controls.Add(this.lblRegion);
            this.pnlFilter.Controls.Add(this.cboRegion);
            this.pnlFilter.Controls.Add(this.cboDivision);
            this.pnlFilter.Controls.Add(this.lblDivision);
            this.pnlFilter.Controls.Add(this.cboDistrict);
            this.pnlFilter.Controls.Add(this.lblDistrict);
            this.pnlFilter.Location = new System.Drawing.Point(0, 172);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(392, 0);
            this.pnlFilter.TabIndex = 17;
            // 
            // cbPrintMultiple
            // 
            this.cbPrintMultiple.AutoSize = true;
            this.cbPrintMultiple.Location = new System.Drawing.Point(207, 113);
            this.cbPrintMultiple.Name = "cbPrintMultiple";
            this.cbPrintMultiple.Size = new System.Drawing.Size(133, 17);
            this.cbPrintMultiple.TabIndex = 7;
            this.cbPrintMultiple.Text = "Print Multiple Grids";
            this.cbPrintMultiple.UseVisualStyleBackColor = true;
            this.cbPrintMultiple.CheckedChanged += new System.EventHandler(this.cbPrintMultiple_CheckedChanged);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdvanced.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdvanced.Location = new System.Drawing.Point(10, 188);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(95, 26);
            this.btnAdvanced.TabIndex = 8;
            this.btnAdvanced.Text = "&Advanced ▾";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(318, 188);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(59, 26);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gridLoading
            // 
            this.gridLoading.Location = new System.Drawing.Point(207, 134);
            this.gridLoading.MarqueeAnimationSpeed = 25;
            this.gridLoading.Name = "gridLoading";
            this.gridLoading.Size = new System.Drawing.Size(170, 24);
            this.gridLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.gridLoading.TabIndex = 18;
            this.gridLoading.UseWaitCursor = true;
            this.gridLoading.Visible = false;
            // 
            // cboRegion
            // 
            this.cboRegion.DependentBox = this.cboDivision;
            this.cboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRegion.Enabled = false;
            this.cboRegion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRegion.FormattingEnabled = true;
            this.cboRegion.Items.AddRange(new object[] {
            ""});
            this.cboRegion.Location = new System.Drawing.Point(12, 57);
            this.cboRegion.Name = "cboRegion";
            this.cboRegion.NullOption = true;
            this.cboRegion.Size = new System.Drawing.Size(170, 24);
            this.cboRegion.Sorted = true;
            this.cboRegion.TabIndex = 4;
            this.cboRegion.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboRegion.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cboDivision
            // 
            this.cboDivision.DependentBox = this.cboDistrict;
            this.cboDivision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDivision.Enabled = false;
            this.cboDivision.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDivision.FormattingEnabled = true;
            this.cboDivision.Items.AddRange(new object[] {
            ""});
            this.cboDivision.Location = new System.Drawing.Point(207, 57);
            this.cboDivision.Name = "cboDivision";
            this.cboDivision.NullOption = true;
            this.cboDivision.Size = new System.Drawing.Size(170, 24);
            this.cboDivision.Sorted = true;
            this.cboDivision.TabIndex = 5;
            this.cboDivision.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDivision.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cboDistrict
            // 
            this.cboDistrict.DependentBox = null;
            this.cboDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDistrict.Enabled = false;
            this.cboDistrict.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDistrict.FormattingEnabled = true;
            this.cboDistrict.Items.AddRange(new object[] {
            ""});
            this.cboDistrict.Location = new System.Drawing.Point(12, 108);
            this.cboDistrict.Name = "cboDistrict";
            this.cboDistrict.NullOption = true;
            this.cboDistrict.Size = new System.Drawing.Size(170, 24);
            this.cboDistrict.Sorted = true;
            this.cboDistrict.TabIndex = 6;
            this.cboDistrict.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDistrict.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cboGridNumber
            // 
            this.cboGridNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridNumber.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboGridNumber.FormattingEnabled = true;
            this.cboGridNumber.IntegralHeight = false;
            this.cboGridNumber.Location = new System.Drawing.Point(207, 134);
            this.cboGridNumber.Name = "cboGridNumber";
            this.cboGridNumber.Size = new System.Drawing.Size(170, 24);
            this.cboGridNumber.Sorted = true;
            this.cboGridNumber.TabIndex = 3;
            this.cboGridNumber.UseMultiOption = false;
            this.cboGridNumber.MultiItemChecked += new System.Windows.Forms.ItemCheckEventHandler(this.cboGridNumber_MultiItemChecked);
            this.cboGridNumber.SelectedIndexChanged += new System.EventHandler(this.cboGridNumber_SelectedIndexChanged);
            // 
            // cboScale
            // 
            this.cboScale.DependentBox = this.cboRegion;
            this.cboScale.DisplayMember = "Value";
            this.cboScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScale.Enabled = false;
            this.cboScale.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboScale.FormattingEnabled = true;
            this.cboScale.Items.AddRange(new object[] {
            ""});
            this.cboScale.Location = new System.Drawing.Point(12, 134);
            this.cboScale.Name = "cboScale";
            this.cboScale.NullOption = true;
            this.cboScale.Size = new System.Drawing.Size(170, 24);
            this.cboScale.Sorted = true;
            this.cboScale.TabIndex = 2;
            this.cboScale.ValueMember = "Key";
            this.cboScale.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboScale.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cboMapType
            // 
            this.cboMapType.BackColor = System.Drawing.SystemColors.Window;
            this.cboMapType.DependentBox = this.cboScale;
            this.cboMapType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMapType.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMapType.FormattingEnabled = true;
            this.cboMapType.Location = new System.Drawing.Point(12, 43);
            this.cboMapType.Name = "cboMapType";
            this.cboMapType.NullOption = false;
            this.cboMapType.Size = new System.Drawing.Size(170, 24);
            this.cboMapType.Sorted = true;
            this.cboMapType.TabIndex = 0;
            this.cboMapType.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboMapType.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // MapBookPrintUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ClientSize = new System.Drawing.Size(391, 226);
            this.Controls.Add(this.gridLoading);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.cboGridNumber);
            this.Controls.Add(this.lblGridNumber);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.btnPrintPDFs);
            this.Controls.Add(this.lblPageSize);
            this.Controls.Add(this.cboPageSize);
            this.Controls.Add(this.cboScale);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.cboMapType);
            this.Controls.Add(this.lblMapType);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapBookPrintUI";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print Options - Standard Map";
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMapType;
        private Controls.FolderDependentComboBox cboMapType;
        private Controls.FolderDependentComboBox cboRegion;
        private System.Windows.Forms.Label lblRegion;
        private Controls.FolderDependentComboBox cboDivision;
        private System.Windows.Forms.Label lblDivision;
        private Controls.FolderDependentComboBox cboDistrict;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.Button btnPrintPDFs;
        private Controls.FolderDependentComboBox cboScale;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ComboBox cboPageSize;
        private System.Windows.Forms.Label lblPageSize;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Label label1;
        private Controls.MultiSelectCheckBox cboGridNumber;
        private System.Windows.Forms.Label lblGridNumber;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ProgressBar gridLoading;
        private System.Windows.Forms.CheckBox cbPrintMultiple;

    }
}

