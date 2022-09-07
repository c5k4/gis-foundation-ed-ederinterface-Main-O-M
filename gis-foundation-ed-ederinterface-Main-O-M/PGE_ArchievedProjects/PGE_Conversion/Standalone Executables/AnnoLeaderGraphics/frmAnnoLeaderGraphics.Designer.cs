namespace AnnoLeaderGraphics
{
    partial class frmAnnoLeaderGraphics
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoColorConv = new System.Windows.Forms.RadioButton();
            this.rdoColorEnhanced = new System.Windows.Forms.RadioButton();
            this.btnProceed = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoHollow = new System.Windows.Forms.RadioButton();
            this.rdoFill = new System.Windows.Forms.RadioButton();
            this.btnDeleteGraphics = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoColorEnhanced);
            this.groupBox1.Controls.Add(this.rdoColorConv);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 49);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color Palette";
            // 
            // rdoColorConv
            // 
            this.rdoColorConv.AutoSize = true;
            this.rdoColorConv.Checked = true;
            this.rdoColorConv.Location = new System.Drawing.Point(12, 21);
            this.rdoColorConv.Name = "rdoColorConv";
            this.rdoColorConv.Size = new System.Drawing.Size(87, 17);
            this.rdoColorConv.TabIndex = 0;
            this.rdoColorConv.TabStop = true;
            this.rdoColorConv.Text = "Conventional";
            this.rdoColorConv.UseVisualStyleBackColor = true;
            // 
            // rdoColorEnhanced
            // 
            this.rdoColorEnhanced.AutoSize = true;
            this.rdoColorEnhanced.Location = new System.Drawing.Point(112, 21);
            this.rdoColorEnhanced.Name = "rdoColorEnhanced";
            this.rdoColorEnhanced.Size = new System.Drawing.Size(74, 17);
            this.rdoColorEnhanced.TabIndex = 1;
            this.rdoColorEnhanced.Text = "Enhanced";
            this.rdoColorEnhanced.UseVisualStyleBackColor = true;
            // 
            // btnProceed
            // 
            this.btnProceed.Location = new System.Drawing.Point(12, 135);
            this.btnProceed.Name = "btnProceed";
            this.btnProceed.Size = new System.Drawing.Size(86, 29);
            this.btnProceed.TabIndex = 2;
            this.btnProceed.Text = "Proceed";
            this.btnProceed.UseVisualStyleBackColor = true;
            this.btnProceed.Click += new System.EventHandler(this.btnProceed_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdoFill);
            this.groupBox2.Controls.Add(this.rdoHollow);
            this.groupBox2.Location = new System.Drawing.Point(15, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(201, 50);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fill";
            // 
            // rdoHollow
            // 
            this.rdoHollow.AutoSize = true;
            this.rdoHollow.Checked = true;
            this.rdoHollow.Location = new System.Drawing.Point(8, 21);
            this.rdoHollow.Name = "rdoHollow";
            this.rdoHollow.Size = new System.Drawing.Size(57, 17);
            this.rdoHollow.TabIndex = 0;
            this.rdoHollow.TabStop = true;
            this.rdoHollow.Text = "Hollow";
            this.rdoHollow.UseVisualStyleBackColor = true;
            // 
            // rdoFill
            // 
            this.rdoFill.AutoSize = true;
            this.rdoFill.Location = new System.Drawing.Point(83, 21);
            this.rdoFill.Name = "rdoFill";
            this.rdoFill.Size = new System.Drawing.Size(37, 17);
            this.rdoFill.TabIndex = 1;
            this.rdoFill.Text = "Fill";
            this.rdoFill.UseVisualStyleBackColor = true;
            // 
            // btnDeleteGraphics
            // 
            this.btnDeleteGraphics.Location = new System.Drawing.Point(125, 135);
            this.btnDeleteGraphics.Name = "btnDeleteGraphics";
            this.btnDeleteGraphics.Size = new System.Drawing.Size(92, 29);
            this.btnDeleteGraphics.TabIndex = 4;
            this.btnDeleteGraphics.Text = "Delete Graphics";
            this.btnDeleteGraphics.UseVisualStyleBackColor = true;
            this.btnDeleteGraphics.Click += new System.EventHandler(this.btnDeleteGraphics_Click);
            // 
            // frmAnnoLeaderGraphics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 176);
            this.Controls.Add(this.btnDeleteGraphics);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnProceed);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmAnnoLeaderGraphics";
            this.Text = "Anno Graphics v0.0.0";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAnnoLeaderGraphics_FormClosed);
            this.Load += new System.EventHandler(this.frmAnnoLeaderGraphics_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnProceed;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDeleteGraphics;
        public System.Windows.Forms.RadioButton rdoColorEnhanced;
        public System.Windows.Forms.RadioButton rdoColorConv;
        public System.Windows.Forms.RadioButton rdoFill;
        public System.Windows.Forms.RadioButton rdoHollow;
    }
}