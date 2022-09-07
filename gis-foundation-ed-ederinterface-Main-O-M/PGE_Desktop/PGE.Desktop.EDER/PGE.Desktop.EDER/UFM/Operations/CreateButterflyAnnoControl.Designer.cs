namespace PGE.Desktop.EDER.UFM.Operations
{
    partial class CreateButterflyAnnoControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateButterflyAnnoControl));
            this.groupCreateAnno = new System.Windows.Forms.GroupBox();
            this.butGenerateAnno = new System.Windows.Forms.Button();
            this.cboFontSize = new System.Windows.Forms.ComboBox();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupCreateAnno.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCreateAnno
            // 
            this.groupCreateAnno.Controls.Add(this.butGenerateAnno);
            this.groupCreateAnno.Controls.Add(this.cboFontSize);
            this.groupCreateAnno.Controls.Add(this.lblFontSize);
            this.groupCreateAnno.Controls.Add(this.label1);
            this.groupCreateAnno.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCreateAnno.Location = new System.Drawing.Point(0, 0);
            this.groupCreateAnno.Name = "groupCreateAnno";
            this.groupCreateAnno.Size = new System.Drawing.Size(296, 112);
            this.groupCreateAnno.TabIndex = 4;
            this.groupCreateAnno.TabStop = false;
            this.groupCreateAnno.Text = "Create Duct Bank Conductor Annotation";
            // 
            // butGenerateAnno
            // 
            this.butGenerateAnno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butGenerateAnno.AutoSize = true;
            this.butGenerateAnno.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.butGenerateAnno.Image = ((System.Drawing.Image)(resources.GetObject("butGenerateAnno.Image")));
            this.butGenerateAnno.Location = new System.Drawing.Point(235, 56);
            this.butGenerateAnno.Name = "butGenerateAnno";
            this.butGenerateAnno.Size = new System.Drawing.Size(36, 36);
            this.butGenerateAnno.TabIndex = 4;
            this.butGenerateAnno.UseVisualStyleBackColor = true;
            this.butGenerateAnno.Click += new System.EventHandler(this.butGenerateAnno_Click);
            // 
            // cboFontSize
            // 
            this.cboFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFontSize.BackColor = System.Drawing.SystemColors.Menu;
            this.cboFontSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFontSize.FormattingEnabled = true;
            this.cboFontSize.Location = new System.Drawing.Point(211, 29);
            this.cboFontSize.Name = "cboFontSize";
            this.cboFontSize.Size = new System.Drawing.Size(60, 21);
            this.cboFontSize.TabIndex = 7;
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(6, 32);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(87, 13);
            this.lblFontSize.TabIndex = 6;
            this.lblFontSize.Text = "Select Font Size:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 34);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Duct Bank:";
            // 
            // CreateButterflyAnnoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupCreateAnno);
            this.Name = "CreateButterflyAnnoControl";
            this.Size = new System.Drawing.Size(296, 282);
            this.Load += new System.EventHandler(this.CreateButterflyAnnoControl_Load);
            this.groupCreateAnno.ResumeLayout(false);
            this.groupCreateAnno.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupCreateAnno;
        private System.Windows.Forms.Button butGenerateAnno;
        private System.Windows.Forms.ComboBox cboFontSize;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.Label label1;

    }
}
