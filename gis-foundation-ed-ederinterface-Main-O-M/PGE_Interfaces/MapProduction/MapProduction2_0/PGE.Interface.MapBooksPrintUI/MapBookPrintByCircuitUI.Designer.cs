namespace PGE.Interfaces.MapBooksPrintUI
{
    partial class MapBookPrintByCircuitUI
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
            this.grpFilter = new System.Windows.Forms.GroupBox();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.cboDistrict = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.grpCircuit = new System.Windows.Forms.GroupBox();
            this.tabSelectCircuits = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblCircuitSelection = new System.Windows.Forms.Label();
            this.lstCircuitList = new System.Windows.Forms.ListView();
            this.colCircuitID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lstSelectedCircuitList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCircuitSearch = new System.Windows.Forms.Button();
            this.lblRegion = new System.Windows.Forms.Label();
            this.cboDivision = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cbPrintMultiple = new System.Windows.Forms.CheckBox();
            this.cboRegion = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.lblDivision = new System.Windows.Forms.Label();
            this.grpMap = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.btnPrintPDFs = new System.Windows.Forms.Button();
            this.lblPageSize = new System.Windows.Forms.Label();
            this.cboMapType = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.cboScale = new MapBooksPrintUI.Controls.FolderDependentComboBox();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.lblMapType = new System.Windows.Forms.Label();
            this.gridLoading = new System.Windows.Forms.ProgressBar();
            this.cboGridNumber = new MapBooksPrintUI.Controls.MultiSelectCheckBox();
            this.lblGridNumber = new System.Windows.Forms.Label();
            this.lblScale = new System.Windows.Forms.Label();
            this.grpFilter.SuspendLayout();
            this.grpCircuit.SuspendLayout();
            this.tabSelectCircuits.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFilter
            // 
            this.grpFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpFilter.Controls.Add(this.lblDistrict);
            this.grpFilter.Controls.Add(this.cboDistrict);
            this.grpFilter.Controls.Add(this.grpCircuit);
            this.grpFilter.Controls.Add(this.lblRegion);
            this.grpFilter.Controls.Add(this.cboDivision);
            this.grpFilter.Controls.Add(this.cbPrintMultiple);
            this.grpFilter.Controls.Add(this.cboRegion);
            this.grpFilter.Controls.Add(this.lblDivision);
            this.grpFilter.Location = new System.Drawing.Point(3, 191);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Size = new System.Drawing.Size(416, 410);
            this.grpFilter.TabIndex = 63;
            this.grpFilter.TabStop = false;
            this.grpFilter.Text = "Advanced search";
            this.grpFilter.Visible = false;
            // 
            // lblDistrict
            // 
            this.lblDistrict.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDistrict.Location = new System.Drawing.Point(8, 70);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(170, 13);
            this.lblDistrict.TabIndex = 66;
            this.lblDistrict.Text = "Filter District:";
            this.lblDistrict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.cboDistrict.Location = new System.Drawing.Point(11, 86);
            this.cboDistrict.Name = "cboDistrict";
            this.cboDistrict.NullOption = true;
            this.cboDistrict.Size = new System.Drawing.Size(170, 24);
            this.cboDistrict.Sorted = true;
            this.cboDistrict.TabIndex = 67;
            this.cboDistrict.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDistrict.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // grpCircuit
            // 
            this.grpCircuit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpCircuit.Controls.Add(this.tabSelectCircuits);
            this.grpCircuit.Controls.Add(this.btnCircuitSearch);
            this.grpCircuit.Location = new System.Drawing.Point(5, 119);
            this.grpCircuit.Name = "grpCircuit";
            this.grpCircuit.Size = new System.Drawing.Size(406, 286);
            this.grpCircuit.TabIndex = 65;
            this.grpCircuit.TabStop = false;
            this.grpCircuit.Text = "Filter Grid Number By Circuit";
            this.grpCircuit.Visible = false;
            // 
            // tabSelectCircuits
            // 
            this.tabSelectCircuits.Controls.Add(this.tabPage1);
            this.tabSelectCircuits.Controls.Add(this.tabPage2);
            this.tabSelectCircuits.Location = new System.Drawing.Point(6, 20);
            this.tabSelectCircuits.Name = "tabSelectCircuits";
            this.tabSelectCircuits.SelectedIndex = 0;
            this.tabSelectCircuits.Size = new System.Drawing.Size(397, 229);
            this.tabSelectCircuits.TabIndex = 47;
            this.tabSelectCircuits.Tag = "";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblCircuitSelection);
            this.tabPage1.Controls.Add(this.lstCircuitList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(389, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Circuit List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblCircuitSelection
            // 
            this.lblCircuitSelection.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCircuitSelection.Location = new System.Drawing.Point(0, 3);
            this.lblCircuitSelection.Name = "lblCircuitSelection";
            this.lblCircuitSelection.Size = new System.Drawing.Size(170, 13);
            this.lblCircuitSelection.TabIndex = 47;
            this.lblCircuitSelection.Text = "Select Circuit(s):";
            this.lblCircuitSelection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lstCircuitList
            // 
            this.lstCircuitList.CheckBoxes = true;
            this.lstCircuitList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCircuitID});
            this.lstCircuitList.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lstCircuitList.FullRowSelect = true;
            this.lstCircuitList.GridLines = true;
            this.lstCircuitList.Location = new System.Drawing.Point(3, 19);
            this.lstCircuitList.Name = "lstCircuitList";
            this.lstCircuitList.Size = new System.Drawing.Size(381, 179);
            this.lstCircuitList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstCircuitList.TabIndex = 46;
            this.lstCircuitList.UseCompatibleStateImageBehavior = false;
            this.lstCircuitList.View = System.Windows.Forms.View.Details;
            this.lstCircuitList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstCircuitList_ItemChecked);
            // 
            // colCircuitID
            // 
            this.colCircuitID.Text = "Circuit ID";
            this.colCircuitID.Width = 367;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lstSelectedCircuitList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(389, 203);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Selected Circuit(s)";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lstSelectedCircuitList
            // 
            this.lstSelectedCircuitList.CheckBoxes = true;
            this.lstSelectedCircuitList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstSelectedCircuitList.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lstSelectedCircuitList.FullRowSelect = true;
            this.lstSelectedCircuitList.GridLines = true;
            this.lstSelectedCircuitList.Location = new System.Drawing.Point(3, 3);
            this.lstSelectedCircuitList.Name = "lstSelectedCircuitList";
            this.lstSelectedCircuitList.Size = new System.Drawing.Size(383, 194);
            this.lstSelectedCircuitList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstSelectedCircuitList.TabIndex = 48;
            this.lstSelectedCircuitList.UseCompatibleStateImageBehavior = false;
            this.lstSelectedCircuitList.View = System.Windows.Forms.View.Details;
            this.lstSelectedCircuitList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSelectedCircuitList_ItemCheck);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Circuit ID";
            this.columnHeader1.Width = 369;
            // 
            // btnCircuitSearch
            // 
            this.btnCircuitSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCircuitSearch.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCircuitSearch.Location = new System.Drawing.Point(248, 255);
            this.btnCircuitSearch.Name = "btnCircuitSearch";
            this.btnCircuitSearch.Size = new System.Drawing.Size(151, 26);
            this.btnCircuitSearch.TabIndex = 46;
            this.btnCircuitSearch.Text = "&Filter By Circuit(s)";
            this.btnCircuitSearch.UseVisualStyleBackColor = true;
            this.btnCircuitSearch.Click += new System.EventHandler(this.btnCircuitSearch_Click);
            // 
            // lblRegion
            // 
            this.lblRegion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRegion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegion.Location = new System.Drawing.Point(8, 15);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Size = new System.Drawing.Size(170, 16);
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
            this.cboDivision.Location = new System.Drawing.Point(224, 34);
            this.cboDivision.Name = "cboDivision";
            this.cboDivision.NullOption = true;
            this.cboDivision.Size = new System.Drawing.Size(180, 24);
            this.cboDivision.Sorted = true;
            this.cboDivision.TabIndex = 7;
            this.cboDivision.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboDivision.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // cbPrintMultiple
            // 
            this.cbPrintMultiple.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbPrintMultiple.AutoSize = true;
            this.cbPrintMultiple.Location = new System.Drawing.Point(223, 93);
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
            this.cboRegion.Location = new System.Drawing.Point(11, 34);
            this.cboRegion.Name = "cboRegion";
            this.cboRegion.NullOption = true;
            this.cboRegion.Size = new System.Drawing.Size(170, 24);
            this.cboRegion.Sorted = true;
            this.cboRegion.TabIndex = 6;
            this.cboRegion.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboRegion.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            // 
            // lblDivision
            // 
            this.lblDivision.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDivision.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDivision.Location = new System.Drawing.Point(221, 16);
            this.lblDivision.Name = "lblDivision";
            this.lblDivision.Size = new System.Drawing.Size(115, 16);
            this.lblDivision.TabIndex = 5;
            this.lblDivision.Text = "Filter Division:";
            this.lblDivision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpMap
            // 
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
            this.grpMap.Location = new System.Drawing.Point(3, 12);
            this.grpMap.Name = "grpMap";
            this.grpMap.Size = new System.Drawing.Size(416, 173);
            this.grpMap.TabIndex = 64;
            this.grpMap.TabStop = false;
            this.grpMap.Text = "Map";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(318, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 29);
            this.btnClose.TabIndex = 30;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cboPageSize
            // 
            this.cboPageSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboPageSize.BackColor = System.Drawing.SystemColors.Window;
            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPageSize.FormattingEnabled = true;
            this.cboPageSize.Location = new System.Drawing.Point(236, 37);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(170, 24);
            this.cboPageSize.Sorted = true;
            this.cboPageSize.TabIndex = 25;
            // 
            // btnPrintPDFs
            // 
            this.btnPrintPDFs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintPDFs.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.btnPrintPDFs.Location = new System.Drawing.Point(140, 124);
            this.btnPrintPDFs.Name = "btnPrintPDFs";
            this.btnPrintPDFs.Size = new System.Drawing.Size(170, 29);
            this.btnPrintPDFs.TabIndex = 28;
            this.btnPrintPDFs.Text = "&Print Standard Map";
            this.btnPrintPDFs.UseVisualStyleBackColor = true;
            this.btnPrintPDFs.Click += new System.EventHandler(this.btnPrintPDFs_Click);
            // 
            // lblPageSize
            // 
            this.lblPageSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPageSize.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageSize.Location = new System.Drawing.Point(236, 20);
            this.lblPageSize.Name = "lblPageSize";
            this.lblPageSize.Size = new System.Drawing.Size(170, 13);
            this.lblPageSize.TabIndex = 31;
            this.lblPageSize.Text = "Select Page Size:";
            this.lblPageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboMapType
            // 
            this.cboMapType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboMapType.BackColor = System.Drawing.SystemColors.Window;
            this.cboMapType.DependentBox = this.cboScale;
            this.cboMapType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMapType.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMapType.FormattingEnabled = true;
            this.cboMapType.Location = new System.Drawing.Point(11, 37);
            this.cboMapType.Name = "cboMapType";
            this.cboMapType.NullOption = false;
            this.cboMapType.Size = new System.Drawing.Size(170, 24);
            this.cboMapType.Sorted = true;
            this.cboMapType.TabIndex = 24;
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
            this.cboScale.Location = new System.Drawing.Point(11, 85);
            this.cboScale.Name = "cboScale";
            this.cboScale.NullOption = true;
            this.cboScale.Size = new System.Drawing.Size(170, 24);
            this.cboScale.Sorted = true;
            this.cboScale.TabIndex = 26;
            this.cboScale.ValueMember = "Key";
            this.cboScale.SelectedIndexChangeCompleted += new System.EventHandler(this.DefaultDropDown_IndexChangeCompleted);
            this.cboScale.DropDownInitializing += new MapBooksPrintUI.Controls.FolderDependentComboBox.DropDownInitializingEventHandler(this.DefaultDropDown_Initializing);
            this.cboScale.SelectedIndexChanged += new System.EventHandler(this.cboScale_SelectedIndexChanged);
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdvanced.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdvanced.Location = new System.Drawing.Point(11, 124);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(121, 29);
            this.btnAdvanced.TabIndex = 27;
            this.btnAdvanced.Text = "&Advanced ▾";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // lblMapType
            // 
            this.lblMapType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMapType.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapType.Location = new System.Drawing.Point(11, 19);
            this.lblMapType.Name = "lblMapType";
            this.lblMapType.Size = new System.Drawing.Size(170, 14);
            this.lblMapType.TabIndex = 29;
            this.lblMapType.Text = "Select Map Type:";
            this.lblMapType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gridLoading
            // 
            this.gridLoading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gridLoading.Location = new System.Drawing.Point(236, 85);
            this.gridLoading.MarqueeAnimationSpeed = 25;
            this.gridLoading.Name = "gridLoading";
            this.gridLoading.Size = new System.Drawing.Size(170, 24);
            this.gridLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.gridLoading.TabIndex = 34;
            this.gridLoading.UseWaitCursor = true;
            this.gridLoading.Visible = false;
            // 
            // cboGridNumber
            // 
            this.cboGridNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cboGridNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGridNumber.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboGridNumber.FormattingEnabled = true;
            this.cboGridNumber.IntegralHeight = false;
            this.cboGridNumber.Location = new System.Drawing.Point(236, 85);
            this.cboGridNumber.Name = "cboGridNumber";
            this.cboGridNumber.Size = new System.Drawing.Size(170, 24);
            this.cboGridNumber.Sorted = true;
            this.cboGridNumber.TabIndex = 35;
            this.cboGridNumber.UseMultiOption = false;
            this.cboGridNumber.MultiItemChecked += new System.Windows.Forms.ItemCheckEventHandler(this.cboGridNumber_MultiItemChecked);
            this.cboGridNumber.SelectedIndexChanged += new System.EventHandler(this.cboGridNumber_SelectedIndexChanged);
            // 
            // lblGridNumber
            // 
            this.lblGridNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGridNumber.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridNumber.Location = new System.Drawing.Point(236, 69);
            this.lblGridNumber.Name = "lblGridNumber";
            this.lblGridNumber.Size = new System.Drawing.Size(170, 13);
            this.lblGridNumber.TabIndex = 33;
            this.lblGridNumber.Text = "Select Grid Number:";
            this.lblGridNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblScale
            // 
            this.lblScale.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblScale.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScale.Location = new System.Drawing.Point(9, 69);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(170, 13);
            this.lblScale.TabIndex = 32;
            this.lblScale.Text = "Select Map Scale:";
            this.lblScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MapBookPrintByCircuitUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(423, 604);
            this.Controls.Add(this.grpMap);
            this.Controls.Add(this.grpFilter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MapBookPrintByCircuitUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print Options - Standard Map";
            this.Load += new System.EventHandler(this.MapBookPrintByCircuitUI_Load);
            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            this.grpCircuit.ResumeLayout(false);
            this.tabSelectCircuits.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.grpMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.FolderDependentComboBox cboRegion;
        private Controls.FolderDependentComboBox cboDivision;
        private System.Windows.Forms.GroupBox grpFilter;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.CheckBox cbPrintMultiple;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.GroupBox grpMap;
        private System.Windows.Forms.GroupBox grpCircuit;
        private System.Windows.Forms.Button btnCircuitSearch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cboPageSize;
        private System.Windows.Forms.Button btnPrintPDFs;
        private System.Windows.Forms.Label lblPageSize;
        private Controls.FolderDependentComboBox cboMapType;
        private Controls.FolderDependentComboBox cboScale;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.Label lblMapType;
        private System.Windows.Forms.ProgressBar gridLoading;
        private Controls.MultiSelectCheckBox cboGridNumber;
        private System.Windows.Forms.Label lblGridNumber;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.Label lblDistrict;
        private Controls.FolderDependentComboBox cboDistrict;
        private System.Windows.Forms.TabControl tabSelectCircuits;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label lblCircuitSelection;
        private System.Windows.Forms.ListView lstCircuitList;
        private System.Windows.Forms.ColumnHeader colCircuitID;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lstSelectedCircuitList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}