namespace PGE.Desktop.EDER.ValidationRules.UI
{
    partial class QAQCErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QAQCErrorForm));
            this.validateRuleGroupBox = new System.Windows.Forms.GroupBox();
            this.tvwResults = new System.Windows.Forms.TreeView();
            this.imgErrorImages = new System.Windows.Forms.ImageList(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.validateRuleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // validateRuleGroupBox
            // 
            this.validateRuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.validateRuleGroupBox.Controls.Add(this.tvwResults);
            this.validateRuleGroupBox.Location = new System.Drawing.Point(7, 4);
            this.validateRuleGroupBox.Name = "validateRuleGroupBox";
            this.validateRuleGroupBox.Size = new System.Drawing.Size(873, 528);
            this.validateRuleGroupBox.TabIndex = 3;
            this.validateRuleGroupBox.TabStop = false;
            this.validateRuleGroupBox.Text = "Please fix the following QAQC errors in order to proceed";
            // 
            // tvwResults
            // 
            this.tvwResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvwResults.CheckBoxes = true;
            this.tvwResults.ImageIndex = 0;
            this.tvwResults.ImageList = this.imgErrorImages;
            this.tvwResults.Location = new System.Drawing.Point(6, 19);
            this.tvwResults.Name = "tvwResults";
            this.tvwResults.SelectedImageIndex = 0;
            this.tvwResults.Size = new System.Drawing.Size(859, 503);
            this.tvwResults.TabIndex = 12;
            this.tvwResults.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvwResults_MouseUp);
            // 
            // imgErrorImages
            // 
            this.imgErrorImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgErrorImages.ImageStream")));
            this.imgErrorImages.TransparentColor = System.Drawing.Color.Transparent;
            this.imgErrorImages.Images.SetKeyName(0, "PointFeature_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(1, "LineFeature_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(2, "PolygonFeat_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(3, "Table_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(4, "AnnoFeature_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(5, "Error.bmp");
            this.imgErrorImages.Images.SetKeyName(6, "Warning.bmp");
            this.imgErrorImages.Images.SetKeyName(7, "Folder.bmp");
            this.imgErrorImages.Images.SetKeyName(8, "Layer_errT5.bmp");
            this.imgErrorImages.Images.SetKeyName(9, "imagesCAHCLBCQ.jpg");
            this.imgErrorImages.Images.SetKeyName(10, "imagesCAX9TUPO.bmp");
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(805, 538);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // QAQCErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 566);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.validateRuleGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QAQCErrorForm";
            this.ShowInTaskbar = false;
            this.Text = "QAQC Errors";
            this.Load += new System.EventHandler(this.ValidationErrorForm_Load);
            this.validateRuleGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox validateRuleGroupBox;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ImageList imgErrorImages;
        private System.Windows.Forms.TreeView tvwResults;
    }
}