namespace PGE.Desktop.EDER.ValidationRules.UI
{
    partial class RunQAQCForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunQAQCForm));
            this.lstImages = new System.Windows.Forms.ImageList(this.components);
            this.tabQAQC = new System.Windows.Forms.TabControl();
            this.pgeRules = new System.Windows.Forms.TabPage();
            this.btnPostingErrors = new System.Windows.Forms.Button();
            this.btnRunQAQC = new System.Windows.Forms.Button();
            this.tvwRules = new System.Windows.Forms.TreeView();
            this.grpRulesToInclude = new System.Windows.Forms.GroupBox();
            this.optRulesAll = new System.Windows.Forms.RadioButton();
            this.optRulesSelection = new System.Windows.Forms.RadioButton();
            this.optRulesSeverityWarning = new System.Windows.Forms.RadioButton();
            this.optRulesSeverityError = new System.Windows.Forms.RadioButton();
            this.grpFeaturesToInclude = new System.Windows.Forms.GroupBox();
            this.optSelection = new System.Windows.Forms.RadioButton();
            this.optVersionDifference = new System.Windows.Forms.RadioButton();
            this.pgeResults = new System.Windows.Forms.TabPage();
            this.btnCollapseAll = new System.Windows.Forms.Button();
            this.btnExpandAll = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.tvwResults = new System.Windows.Forms.TreeView();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabQAQC.SuspendLayout();
            this.pgeRules.SuspendLayout();
            this.grpRulesToInclude.SuspendLayout();
            this.grpFeaturesToInclude.SuspendLayout();
            this.pgeResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstImages
            // 
            this.lstImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lstImages.ImageStream")));
            this.lstImages.TransparentColor = System.Drawing.Color.Transparent;
            this.lstImages.Images.SetKeyName(0, "PointFeature_errT5.bmp");
            this.lstImages.Images.SetKeyName(1, "LineFeature_errT5.bmp");
            this.lstImages.Images.SetKeyName(2, "PolygonFeat_errT5.bmp");
            this.lstImages.Images.SetKeyName(3, "Table_errT5.bmp");
            this.lstImages.Images.SetKeyName(4, "AnnoFeature_errT5.bmp");
            this.lstImages.Images.SetKeyName(5, "Error.bmp");
            this.lstImages.Images.SetKeyName(6, "Warning.bmp");
            this.lstImages.Images.SetKeyName(7, "Folder.bmp");
            this.lstImages.Images.SetKeyName(8, "Layer_errT5.bmp");
            this.lstImages.Images.SetKeyName(9, "imagesCAHCLBCQ.jpg");
            this.lstImages.Images.SetKeyName(10, "imagesCAX9TUPO.bmp");
            // 
            // tabQAQC
            // 
            this.tabQAQC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabQAQC.Controls.Add(this.pgeRules);
            this.tabQAQC.Controls.Add(this.pgeResults);
            this.tabQAQC.Location = new System.Drawing.Point(11, 9);
            this.tabQAQC.Name = "tabQAQC";
            this.tabQAQC.SelectedIndex = 0;
            this.tabQAQC.Size = new System.Drawing.Size(868, 523);
            this.tabQAQC.TabIndex = 7;
            // 
            // pgeRules
            // 
            this.pgeRules.Controls.Add(this.btnPostingErrors);
            this.pgeRules.Controls.Add(this.btnRunQAQC);
            this.pgeRules.Controls.Add(this.tvwRules);
            this.pgeRules.Controls.Add(this.grpRulesToInclude);
            this.pgeRules.Controls.Add(this.grpFeaturesToInclude);
            this.pgeRules.Location = new System.Drawing.Point(4, 22);
            this.pgeRules.Name = "pgeRules";
            this.pgeRules.Padding = new System.Windows.Forms.Padding(3);
            this.pgeRules.Size = new System.Drawing.Size(860, 497);
            this.pgeRules.TabIndex = 0;
            this.pgeRules.Text = "Rules";
            this.pgeRules.UseVisualStyleBackColor = true;
            // 
            // btnPostingErrors
            // 
            this.btnPostingErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPostingErrors.Location = new System.Drawing.Point(664, 468);
            this.btnPostingErrors.Name = "btnPostingErrors";
            this.btnPostingErrors.Size = new System.Drawing.Size(85, 23);
            this.btnPostingErrors.TabIndex = 13;
            this.btnPostingErrors.Text = "Posting Errors";
            this.btnPostingErrors.UseVisualStyleBackColor = true;
            this.btnPostingErrors.Click += new System.EventHandler(this.btnPostingErrors_Click);
            // 
            // btnRunQAQC
            // 
            this.btnRunQAQC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunQAQC.Location = new System.Drawing.Point(755, 468);
            this.btnRunQAQC.Name = "btnRunQAQC";
            this.btnRunQAQC.Size = new System.Drawing.Size(85, 23);
            this.btnRunQAQC.TabIndex = 12;
            this.btnRunQAQC.Text = "Run";
            this.btnRunQAQC.UseVisualStyleBackColor = true;
            this.btnRunQAQC.Click += new System.EventHandler(this.btnRunQAQC_Click);
            // 
            // tvwRules
            // 
            this.tvwRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwRules.CheckBoxes = true;
            this.tvwRules.Enabled = false;
            this.tvwRules.ImageIndex = 0;
            this.tvwRules.ImageList = this.lstImages;
            this.tvwRules.Location = new System.Drawing.Point(6, 220);
            this.tvwRules.Name = "tvwRules";
            this.tvwRules.SelectedImageIndex = 0;
            this.tvwRules.Size = new System.Drawing.Size(834, 242);
            this.tvwRules.TabIndex = 10;
            this.tvwRules.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwRules_AfterCheck);
            // 
            // grpRulesToInclude
            // 
            this.grpRulesToInclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRulesToInclude.Controls.Add(this.optRulesAll);
            this.grpRulesToInclude.Controls.Add(this.optRulesSelection);
            this.grpRulesToInclude.Controls.Add(this.optRulesSeverityWarning);
            this.grpRulesToInclude.Controls.Add(this.optRulesSeverityError);
            this.grpRulesToInclude.Location = new System.Drawing.Point(6, 89);
            this.grpRulesToInclude.Name = "grpRulesToInclude";
            this.grpRulesToInclude.Size = new System.Drawing.Size(834, 125);
            this.grpRulesToInclude.TabIndex = 9;
            this.grpRulesToInclude.TabStop = false;
            this.grpRulesToInclude.Text = "Select Rules to Include";
            // 
            // optRulesAll
            // 
            this.optRulesAll.AutoSize = true;
            this.optRulesAll.Checked = true;
            this.optRulesAll.Location = new System.Drawing.Point(19, 25);
            this.optRulesAll.Name = "optRulesAll";
            this.optRulesAll.Size = new System.Drawing.Size(83, 17);
            this.optRulesAll.TabIndex = 5;
            this.optRulesAll.TabStop = true;
            this.optRulesAll.Text = "Severity - All";
            this.optRulesAll.UseVisualStyleBackColor = true;
            this.optRulesAll.CheckedChanged += new System.EventHandler(this.optRulesIncluded_CheckedChanged);
            // 
            // optRulesSelection
            // 
            this.optRulesSelection.AutoSize = true;
            this.optRulesSelection.Location = new System.Drawing.Point(19, 91);
            this.optRulesSelection.Name = "optRulesSelection";
            this.optRulesSelection.Size = new System.Drawing.Size(132, 17);
            this.optRulesSelection.TabIndex = 4;
            this.optRulesSelection.Text = "Custom Rule Selection";
            this.optRulesSelection.UseVisualStyleBackColor = true;
            this.optRulesSelection.CheckedChanged += new System.EventHandler(this.optRulesIncluded_CheckedChanged);
            // 
            // optRulesSeverityWarning
            // 
            this.optRulesSeverityWarning.AutoSize = true;
            this.optRulesSeverityWarning.Location = new System.Drawing.Point(19, 69);
            this.optRulesSeverityWarning.Name = "optRulesSeverityWarning";
            this.optRulesSeverityWarning.Size = new System.Drawing.Size(112, 17);
            this.optRulesSeverityWarning.TabIndex = 3;
            this.optRulesSeverityWarning.Text = "Severity - Warning";
            this.optRulesSeverityWarning.UseVisualStyleBackColor = true;
            this.optRulesSeverityWarning.CheckedChanged += new System.EventHandler(this.optRulesIncluded_CheckedChanged);
            // 
            // optRulesSeverityError
            // 
            this.optRulesSeverityError.AutoSize = true;
            this.optRulesSeverityError.Location = new System.Drawing.Point(19, 47);
            this.optRulesSeverityError.Name = "optRulesSeverityError";
            this.optRulesSeverityError.Size = new System.Drawing.Size(94, 17);
            this.optRulesSeverityError.TabIndex = 2;
            this.optRulesSeverityError.Text = "Severity - Error";
            this.optRulesSeverityError.UseVisualStyleBackColor = true;
            this.optRulesSeverityError.CheckedChanged += new System.EventHandler(this.optRulesIncluded_CheckedChanged);
            // 
            // grpFeaturesToInclude
            // 
            this.grpFeaturesToInclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFeaturesToInclude.Controls.Add(this.optSelection);
            this.grpFeaturesToInclude.Controls.Add(this.optVersionDifference);
            this.grpFeaturesToInclude.Location = new System.Drawing.Point(6, 6);
            this.grpFeaturesToInclude.Name = "grpFeaturesToInclude";
            this.grpFeaturesToInclude.Size = new System.Drawing.Size(834, 77);
            this.grpFeaturesToInclude.TabIndex = 8;
            this.grpFeaturesToInclude.TabStop = false;
            this.grpFeaturesToInclude.Text = "Select Features to Include";
            // 
            // optSelection
            // 
            this.optSelection.AutoSize = true;
            this.optSelection.Location = new System.Drawing.Point(19, 46);
            this.optSelection.Name = "optSelection";
            this.optSelection.Size = new System.Drawing.Size(93, 17);
            this.optSelection.TabIndex = 3;
            this.optSelection.Text = "Map Selection";
            this.optSelection.UseVisualStyleBackColor = true;
            // 
            // optVersionDifference
            // 
            this.optVersionDifference.AutoSize = true;
            this.optVersionDifference.Checked = true;
            this.optVersionDifference.Location = new System.Drawing.Point(19, 24);
            this.optVersionDifference.Name = "optVersionDifference";
            this.optVersionDifference.Size = new System.Drawing.Size(112, 17);
            this.optVersionDifference.TabIndex = 2;
            this.optVersionDifference.TabStop = true;
            this.optVersionDifference.Text = "Version Difference";
            this.optVersionDifference.UseVisualStyleBackColor = true;
            // 
            // pgeResults
            // 
            this.pgeResults.Controls.Add(this.btnCollapseAll);
            this.pgeResults.Controls.Add(this.btnExpandAll);
            this.pgeResults.Controls.Add(this.btnClear);
            this.pgeResults.Controls.Add(this.tvwResults);
            this.pgeResults.Location = new System.Drawing.Point(4, 22);
            this.pgeResults.Name = "pgeResults";
            this.pgeResults.Padding = new System.Windows.Forms.Padding(3);
            this.pgeResults.Size = new System.Drawing.Size(334, 497);
            this.pgeResults.TabIndex = 1;
            this.pgeResults.Text = "Results";
            this.pgeResults.UseVisualStyleBackColor = true;
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollapseAll.Location = new System.Drawing.Point(157, 471);
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(75, 23);
            this.btnCollapseAll.TabIndex = 15;
            this.btnCollapseAll.Text = "Collapse All";
            this.btnCollapseAll.UseVisualStyleBackColor = true;
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpandAll.Location = new System.Drawing.Point(76, 471);
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(75, 23);
            this.btnExpandAll.TabIndex = 14;
            this.btnExpandAll.Text = "Expand All";
            this.btnExpandAll.UseVisualStyleBackColor = true;
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(239, 471);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tvwResults
            // 
            this.tvwResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwResults.CheckBoxes = true;
            this.tvwResults.ImageIndex = 0;
            this.tvwResults.ImageList = this.lstImages;
            this.tvwResults.Location = new System.Drawing.Point(6, 6);
            this.tvwResults.Name = "tvwResults";
            this.tvwResults.SelectedImageIndex = 0;
            this.tvwResults.Size = new System.Drawing.Size(308, 460);
            this.tvwResults.TabIndex = 11;
            this.tvwResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwResults_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(804, 538);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // RunQAQCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 566);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabQAQC);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunQAQCForm";
            this.Text = "PGE Custom QAQC";
            this.Load += new System.EventHandler(this.RunQAQCForm_Load);
            this.tabQAQC.ResumeLayout(false);
            this.pgeRules.ResumeLayout(false);
            this.grpRulesToInclude.ResumeLayout(false);
            this.grpRulesToInclude.PerformLayout();
            this.grpFeaturesToInclude.ResumeLayout(false);
            this.grpFeaturesToInclude.PerformLayout();
            this.pgeResults.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList lstImages;
        private System.Windows.Forms.TabControl tabQAQC;
        private System.Windows.Forms.TabPage pgeRules;
        private System.Windows.Forms.TabPage pgeResults;
        private System.Windows.Forms.Button btnRunQAQC;
        private System.Windows.Forms.TreeView tvwRules;
        private System.Windows.Forms.GroupBox grpRulesToInclude;
        private System.Windows.Forms.RadioButton optRulesAll;
        private System.Windows.Forms.RadioButton optRulesSelection;
        private System.Windows.Forms.RadioButton optRulesSeverityWarning;
        private System.Windows.Forms.RadioButton optRulesSeverityError;
        private System.Windows.Forms.GroupBox grpFeaturesToInclude;
        private System.Windows.Forms.RadioButton optSelection;
        private System.Windows.Forms.RadioButton optVersionDifference;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TreeView tvwResults;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPostingErrors;
        private System.Windows.Forms.Button btnExpandAll;
        private System.Windows.Forms.Button btnCollapseAll;
    }
}