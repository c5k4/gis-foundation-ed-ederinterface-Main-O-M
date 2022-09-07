namespace PGE.Interfaces.MapBooksPrintUI
{
    partial class MapBookPrintByCircuitTraceUI
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
            this.lblSubstation = new System.Windows.Forms.Label();
            this.lstCircuitList = new System.Windows.Forms.ListView();
            this.colSubstationID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCircuitID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCircuitName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnTrace = new System.Windows.Forms.Button();
            this.cboSubStation = new System.Windows.Forms.ComboBox();
            this.cboNetwork = new System.Windows.Forms.ComboBox();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrintPDFs = new System.Windows.Forms.Button();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.gridLoading = new System.Windows.Forms.ProgressBar();
            this.lblGridNumber = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.lblPageSize = new System.Windows.Forms.Label();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.lblMapType = new System.Windows.Forms.Label();
            this.btnNetwork = new System.Windows.Forms.Button();
            this.grpNetwork = new System.Windows.Forms.GroupBox();
            this.lblTraceStatus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grpFilter = new System.Windows.Forms.GroupBox();
            this.lblRegion = new System.Windows.Forms.Label();
            this.cboDivision = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboDistrict = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cbPrintMultiple = new System.Windows.Forms.CheckBox();
            this.cboRegion = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.lblDivision = new System.Windows.Forms.Label();
            this.grpMap = new System.Windows.Forms.GroupBox();
            this.cboMapType = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboScale = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboGridNumber = new MapBooksPrintUI.Controls.MultiSelectCheckBox();
            this.menuMapBookPrint = new System.Windows.Forms.MenuStrip();
            this.dBConnectionPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpNetwork.SuspendLayout();
            this.grpFilter.SuspendLayout();
            this.grpMap.SuspendLayout();
            this.menuMapBookPrint.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSubstation
            // 
            this.lblSubstation.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubstation.Location = new System.Drawing.Point(6, 70);
            this.lblSubstation.Name = "lblSubstation";
            this.lblSubstation.Size = new System.Drawing.Size(170, 13);
            this.lblSubstation.TabIndex = 38;
            this.lblSubstation.Text = "Select Substation ID:";
            this.lblSubstation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lstCircuitList
            // 
            this.lstCircuitList.CheckBoxes = true;
            this.lstCircuitList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSubstationID,
            this.colCircuitID,
            this.colCircuitName});
            this.lstCircuitList.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lstCircuitList.FullRowSelect = true;
            this.lstCircuitList.GridLines = true;
            this.lstCircuitList.Location = new System.Drawing.Point(7, 134);
            this.lstCircuitList.MultiSelect = false;
            this.lstCircuitList.Name = "lstCircuitList";
            this.lstCircuitList.Size = new System.Drawing.Size(393, 179);
            this.lstCircuitList.TabIndex = 12;
            this.lstCircuitList.UseCompatibleStateImageBehavior = false;
            this.lstCircuitList.View = System.Windows.Forms.View.Details;
            this.lstCircuitList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstCircuitList_ItemChecked);
            this.lstCircuitList.SelectedIndexChanged += new System.EventHandler(this.lstCircuitList_SelectedIndexChanged);
            // 
            // colSubstationID
            // 
            this.colSubstationID.Text = "Substation ID";
            this.colSubstationID.Width = 94;
            // 
            // colCircuitID
            // 
            this.colCircuitID.Text = "Circuit ID";
            this.colCircuitID.Width = 118;
            // 
            // colCircuitName
            // 
            this.colCircuitName.Text = "Circuit Name";
            this.colCircuitName.Width = 155;
            // 
            // btnTrace
            // 
            this.btnTrace.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnTrace.Location = new System.Drawing.Point(297, 320);
            this.btnTrace.Name = "btnTrace";
            this.btnTrace.Size = new System.Drawing.Size(103, 26);
            this.btnTrace.TabIndex = 13;
            this.btnTrace.Text = "T&race";
            this.btnTrace.UseVisualStyleBackColor = true;
            this.btnTrace.Click += new System.EventHandler(this.btnTrace_Click);
            // 
            // cboSubStation
            // 
            this.cboSubStation.BackColor = System.Drawing.SystemColors.Window;
            this.cboSubStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubStation.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboSubStation.FormattingEnabled = true;
            this.cboSubStation.Location = new System.Drawing.Point(6, 86);
            this.cboSubStation.Name = "cboSubStation";
            this.cboSubStation.Size = new System.Drawing.Size(170, 24);
            this.cboSubStation.Sorted = true;
            this.cboSubStation.TabIndex = 11;
            this.cboSubStation.SelectedIndexChanged += new System.EventHandler(this.cboSubStation_SelectedIndexChanged);
            // 
            // cboNetwork
            // 
            this.cboNetwork.BackColor = System.Drawing.SystemColors.Window;
            this.cboNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNetwork.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboNetwork.FormattingEnabled = true;
            this.cboNetwork.Location = new System.Drawing.Point(6, 36);
            this.cboNetwork.Name = "cboNetwork";
            this.cboNetwork.Size = new System.Drawing.Size(394, 24);
            this.cboNetwork.TabIndex = 10;
            // 
            // lblNetwork
            // 
            this.lblNetwork.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNetwork.Location = new System.Drawing.Point(6, 20);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(369, 13);
            this.lblNetwork.TabIndex = 49;
            this.lblNetwork.Text = "Select Network:";
            this.lblNetwork.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(304, 176);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 26);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPrintPDFs
            // 
            this.btnPrintPDFs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintPDFs.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnPrintPDFs.Location = new System.Drawing.Point(5, 176);
            this.btnPrintPDFs.Name = "btnPrintPDFs";
            this.btnPrintPDFs.Size = new System.Drawing.Size(170, 26);
            this.btnPrintPDFs.TabIndex = 14;
            this.btnPrintPDFs.Text = "&Print Standard Map";
            this.btnPrintPDFs.UseVisualStyleBackColor = true;
            this.btnPrintPDFs.Click += new System.EventHandler(this.btnPrintPDFs_Click);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdvanced.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdvanced.Location = new System.Drawing.Point(230, 123);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(170, 29);
            this.btnAdvanced.TabIndex = 5;
            this.btnAdvanced.Text = "&Advanced ▾";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // gridLoading
            // 
            this.gridLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gridLoading.Location = new System.Drawing.Point(230, 85);
            this.gridLoading.MarqueeAnimationSpeed = 25;
            this.gridLoading.Name = "gridLoading";
            this.gridLoading.Size = new System.Drawing.Size(170, 24);
            this.gridLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.gridLoading.TabIndex = 22;
            this.gridLoading.UseWaitCursor = true;
            this.gridLoading.Visible = false;
            // 
            // lblGridNumber
            // 
            this.lblGridNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGridNumber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridNumber.Location = new System.Drawing.Point(230, 69);
            this.lblGridNumber.Name = "lblGridNumber";
            this.lblGridNumber.Size = new System.Drawing.Size(170, 13);
            this.lblGridNumber.TabIndex = 21;
            this.lblGridNumber.Text = "Select Grid Number:";
            this.lblGridNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblScale
            // 
            this.lblScale.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblScale.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScale.Location = new System.Drawing.Point(7, 69);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(170, 13);
            this.lblScale.TabIndex = 20;
            this.lblScale.Text = "Select Map Scale:";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPageSize
            // 
            this.lblPageSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPageSize.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageSize.Location = new System.Drawing.Point(230, 20);
            this.lblPageSize.Name = "lblPageSize";
            this.lblPageSize.Size = new System.Drawing.Size(170, 13);
            this.lblPageSize.TabIndex = 17;
            this.lblPageSize.Text = "Select Page Size:";
            this.lblPageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboPageSize
            // 
            this.cboPageSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboPageSize.BackColor = System.Drawing.SystemColors.Window;
            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPageSize.FormattingEnabled = true;
            this.cboPageSize.Location = new System.Drawing.Point(230, 37);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(170, 24);
            this.cboPageSize.Sorted = true;
            this.cboPageSize.TabIndex = 2;
            // 
            // lblMapType
            // 
            this.lblMapType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMapType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapType.Location = new System.Drawing.Point(5, 19);
            this.lblMapType.Name = "lblMapType";
            this.lblMapType.Size = new System.Drawing.Size(170, 14);
            this.lblMapType.TabIndex = 15;
            this.lblMapType.Text = "Select Map Type:";
            this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNetwork
            // 
            this.btnNetwork.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnNetwork.Location = new System.Drawing.Point(5, 123);
            this.btnNetwork.Name = "btnNetwork";
            this.btnNetwork.Size = new System.Drawing.Size(170, 29);
            this.btnNetwork.TabIndex = 4;
            this.btnNetwork.Text = "&Trace Circuit▾";
            this.btnNetwork.UseVisualStyleBackColor = true;
            this.btnNetwork.Click += new System.EventHandler(this.btnNetwork_Click);
            // 
            // grpNetwork
            // 
            this.grpNetwork.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpNetwork.Controls.Add(this.lblTraceStatus);
            this.grpNetwork.Controls.Add(this.lblNetwork);
            this.grpNetwork.Controls.Add(this.cboNetwork);
            this.grpNetwork.Controls.Add(this.lblSubstation);
            this.grpNetwork.Controls.Add(this.cboSubStation);
            this.grpNetwork.Controls.Add(this.btnTrace);
            this.grpNetwork.Controls.Add(this.label4);
            this.grpNetwork.Controls.Add(this.lstCircuitList);
            this.grpNetwork.Location = new System.Drawing.Point(2, 377);
            this.grpNetwork.Name = "grpNetwork";
            this.grpNetwork.Size = new System.Drawing.Size(406, 352);
            this.grpNetwork.TabIndex = 62;
            this.grpNetwork.TabStop = false;
            this.grpNetwork.Text = "Circuit tracing";
            this.grpNetwork.Visible = false;
            // 
            // lblTraceStatus
            // 
            this.lblTraceStatus.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lblTraceStatus.Location = new System.Drawing.Point(6, 320);
            this.lblTraceStatus.Name = "lblTraceStatus";
            this.lblTraceStatus.Size = new System.Drawing.Size(285, 25);
            this.lblTraceStatus.TabIndex = 50;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Select Circuit:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpFilter
            // 
            this.grpFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpFilter.Controls.Add(this.lblRegion);
            this.grpFilter.Controls.Add(this.cboDivision);
            this.grpFilter.Controls.Add(this.cbPrintMultiple);
            this.grpFilter.Controls.Add(this.cboRegion);
            this.grpFilter.Controls.Add(this.lblDistrict);
            this.grpFilter.Controls.Add(this.cboDistrict);
            this.grpFilter.Controls.Add(this.lblDivision);
            this.grpFilter.Location = new System.Drawing.Point(2, 247);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Size = new System.Drawing.Size(406, 124);
            this.grpFilter.TabIndex = 63;
            this.grpFilter.TabStop = false;
            this.grpFilter.Text = "Advanced search";
            this.grpFilter.Visible = false;
            // 
            // lblRegion
            // 
            this.lblRegion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRegion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegion.Location = new System.Drawing.Point(6, 16);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(170, 23);
            this.lblRegion.TabIndex = 3;
            this.lblRegion.Text = "Filter Region:";
            this.lblRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboDivision
            // 
            this.cboDivision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboDivision.DependentBox = this.cboDistrict;
            this.cboDivision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDivision.Enabled = false;
            this.cboDivision.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDivision.FormattingEnabled = true;
            this.cboDivision.Items.AddRange(new object[] {
            ""});
            this.cboDivision.Location = new System.Drawing.Point(220, 41);
            this.cboDivision.Name = "cboDivision";
            this.cboDivision.NullOption = true;
            this.cboDivision.Size = new System.Drawing.Size(180, 24);
            this.cboDivision.Sorted = true;
            this.cboDivision.TabIndex = 7;
            this.cboDivision.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDivision.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            this.cboDivision.SelectedIndexChanged += new System.EventHandler(this.cboDivision_SelectedIndexChanged);
            // 
            // cboDistrict
            // 
            this.cboDistrict.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboDistrict.DependentBox = null;
            this.cboDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDistrict.Enabled = false;
            this.cboDistrict.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDistrict.FormattingEnabled = true;
            this.cboDistrict.Items.AddRange(new object[] {
            ""});
            this.cboDistrict.Location = new System.Drawing.Point(8, 85);
            this.cboDistrict.Name = "cboDistrict";
            this.cboDistrict.NullOption = true;
            this.cboDistrict.Size = new System.Drawing.Size(170, 24);
            this.cboDistrict.Sorted = true;
            this.cboDistrict.TabIndex = 8;
            this.cboDistrict.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDistrict.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            this.cboDistrict.SelectedIndexChanged += new System.EventHandler(this.cboDistrict_SelectedIndexChanged);
            // 
            // cbPrintMultiple
            // 
            this.cbPrintMultiple.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbPrintMultiple.AutoSize = true;
            this.cbPrintMultiple.Location = new System.Drawing.Point(265, 92);
            this.cbPrintMultiple.Name = "cbPrintMultiple";
            this.cbPrintMultiple.Size = new System.Drawing.Size(113, 17);
            this.cbPrintMultiple.TabIndex = 9;
            this.cbPrintMultiple.Text = "Print Multiple Grids";
            this.cbPrintMultiple.UseVisualStyleBackColor = true;
            this.cbPrintMultiple.CheckedChanged += new System.EventHandler(this.cbPrintMultiple_CheckedChanged);
            // 
            // cboRegion
            // 
            this.cboRegion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboRegion.DependentBox = this.cboDivision;
            this.cboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRegion.Enabled = false;
            this.cboRegion.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRegion.FormattingEnabled = true;
            this.cboRegion.Items.AddRange(new object[] {
            ""});
            this.cboRegion.Location = new System.Drawing.Point(7, 41);
            this.cboRegion.Name = "cboRegion";
            this.cboRegion.NullOption = true;
            this.cboRegion.Size = new System.Drawing.Size(170, 24);
            this.cboRegion.Sorted = true;
            this.cboRegion.TabIndex = 6;
            this.cboRegion.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboRegion.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            this.cboRegion.SelectedIndexChanged += new System.EventHandler(this.cboRegion_SelectedIndexChanged);
            // 
            // lblDistrict
            // 
            this.lblDistrict.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDistrict.Location = new System.Drawing.Point(6, 68);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(170, 13);
            this.lblDistrict.TabIndex = 7;
            this.lblDistrict.Text = "Filter District:";
            this.lblDistrict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDivision
            // 
            this.lblDivision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDivision.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDivision.Location = new System.Drawing.Point(227, 25);
            this.lblDivision.Name = "lblDivision";
            this.lblDivision.Size = new System.Drawing.Size(170, 13);
            this.lblDivision.TabIndex = 5;
            this.lblDivision.Text = "Filter Division:";
            this.lblDivision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpMap
            // 
            this.grpMap.Controls.Add(this.btnNetwork);
            this.grpMap.Controls.Add(this.btnClose);
            this.grpMap.Controls.Add(this.cboPageSize);
            this.grpMap.Controls.Add(this.btnPrintPDFs);
            this.grpMap.Controls.Add(this.lblPageSize);
            this.grpMap.Controls.Add(this.cboMapType);
            this.grpMap.Controls.Add(this.btnAdvanced);
            this.grpMap.Controls.Add(this.lblMapType);
            this.grpMap.Controls.Add(this.gridLoading);
            this.grpMap.Controls.Add(this.cboGridNumber);
            this.grpMap.Controls.Add(this.lblGridNumber);
            this.grpMap.Controls.Add(this.lblScale);
            this.grpMap.Controls.Add(this.cboScale);
            this.grpMap.Location = new System.Drawing.Point(2, 25);
            this.grpMap.Name = "grpMap";
            this.grpMap.Size = new System.Drawing.Size(406, 217);
            this.grpMap.TabIndex = 64;
            this.grpMap.TabStop = false;
            this.grpMap.Text = "Map";
            // 
            // cboMapType
            // 
            this.cboMapType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboMapType.BackColor = System.Drawing.SystemColors.Window;
            this.cboMapType.DependentBox = this.cboScale;
            this.cboMapType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMapType.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMapType.FormattingEnabled = true;
            this.cboMapType.Location = new System.Drawing.Point(5, 37);
            this.cboMapType.Name = "cboMapType";
            this.cboMapType.NullOption = false;
            this.cboMapType.Size = new System.Drawing.Size(170, 24);
            this.cboMapType.Sorted = true;
            this.cboMapType.TabIndex = 0;
            this.cboMapType.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboMapType.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cboScale
            // 
            this.cboScale.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboScale.DependentBox = this.cboRegion;
            this.cboScale.DisplayMember = "Value";
            this.cboScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScale.Enabled = false;
            this.cboScale.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboScale.FormattingEnabled = true;
            this.cboScale.Items.AddRange(new object[] {
            ""});
            this.cboScale.Location = new System.Drawing.Point(5, 85);
            this.cboScale.Name = "cboScale";
            this.cboScale.NullOption = true;
            this.cboScale.Size = new System.Drawing.Size(170, 24);
            this.cboScale.Sorted = true;
            this.cboScale.TabIndex = 3;
            this.cboScale.ValueMember = "Key";
            this.cboScale.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboScale.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            this.cboScale.SelectedIndexChanged += new System.EventHandler(this.cboScale_SelectedIndexChanged);
            // 
            // cboGridNumber
            // 
            this.cboGridNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboGridNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridNumber.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboGridNumber.FormattingEnabled = true;
            this.cboGridNumber.IntegralHeight = false;
            this.cboGridNumber.Location = new System.Drawing.Point(230, 85);
            this.cboGridNumber.Name = "cboGridNumber";
            this.cboGridNumber.Size = new System.Drawing.Size(170, 24);
            this.cboGridNumber.Sorted = true;
            this.cboGridNumber.TabIndex = 23;
            this.cboGridNumber.UseMultiOption = false;
            this.cboGridNumber.MultiItemChecked += new System.Windows.Forms.ItemCheckEventHandler(this.cboGridNumber_MultiItemChecked);
            this.cboGridNumber.SelectedIndexChanged += new System.EventHandler(this.cboGridNumber_SelectedIndexChanged);
            // 
            // menuMapBookPrint
            // 
            this.menuMapBookPrint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dBConnectionPropertiesToolStripMenuItem});
            this.menuMapBookPrint.Location = new System.Drawing.Point(0, 0);
            this.menuMapBookPrint.Name = "menuMapBookPrint";
            this.menuMapBookPrint.Size = new System.Drawing.Size(411, 24);
            this.menuMapBookPrint.TabIndex = 65;
            this.menuMapBookPrint.Text = "menuStrip1";
            // 
            // dBConnectionPropertiesToolStripMenuItem
            // 
            this.dBConnectionPropertiesToolStripMenuItem.Name = "dBConnectionPropertiesToolStripMenuItem";
            this.dBConnectionPropertiesToolStripMenuItem.Size = new System.Drawing.Size(155, 20);
            this.dBConnectionPropertiesToolStripMenuItem.Text = "&DB Connection Properties";
            this.dBConnectionPropertiesToolStripMenuItem.Click += new System.EventHandler(this.dBConnectionPropertiesToolStripMenuItem_Click);
            // 
            // MapBookPrintByCircuitTraceUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(411, 731);
            this.Controls.Add(this.grpMap);
            this.Controls.Add(this.grpNetwork);
            this.Controls.Add(this.grpFilter);
            this.Controls.Add(this.menuMapBookPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MapBookPrintByCircuitTraceUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print Options - Standard Map";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapBookPrintByCircuitTraceUI_FormClosing);
            this.Load += new System.EventHandler(this.MapBookPrintByCircuitTraceUI_Load);
            this.grpNetwork.ResumeLayout(false);
            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            this.grpMap.ResumeLayout(false);
            this.menuMapBookPrint.ResumeLayout(false);
            this.menuMapBookPrint.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSubstation;
        private System.Windows.Forms.ListView lstCircuitList;
        private System.Windows.Forms.Button btnTrace;
        private System.Windows.Forms.ColumnHeader colCircuitID;
        private System.Windows.Forms.ColumnHeader colCircuitName;
        private System.Windows.Forms.ColumnHeader colSubstationID;
        private System.Windows.Forms.ComboBox cboSubStation;
        private System.Windows.Forms.ComboBox cboNetwork;
        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrintPDFs;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.ProgressBar gridLoading;
        private Controls.MultiSelectCheckBox cboGridNumber;
        private System.Windows.Forms.Label lblGridNumber;
        private Controls.FolderDependentComboBox cboScale;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.Label lblPageSize;
        private System.Windows.Forms.ComboBox cboPageSize;
        private Controls.FolderDependentComboBox cboMapType;
        private System.Windows.Forms.Label lblMapType;
        private System.Windows.Forms.GroupBox grpNetwork;
        private System.Windows.Forms.Button btnNetwork;
        private Controls.FolderDependentComboBox cboRegion;
        private Controls.FolderDependentComboBox cboDivision;
        private Controls.FolderDependentComboBox cboDistrict;
        private System.Windows.Forms.GroupBox grpFilter;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.CheckBox cbPrintMultiple;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.GroupBox grpMap;
        private System.Windows.Forms.MenuStrip menuMapBookPrint;
        private System.Windows.Forms.ToolStripMenuItem dBConnectionPropertiesToolStripMenuItem;
        private System.Windows.Forms.Label lblTraceStatus;
        private System.Windows.Forms.Label label4;
    }
}
