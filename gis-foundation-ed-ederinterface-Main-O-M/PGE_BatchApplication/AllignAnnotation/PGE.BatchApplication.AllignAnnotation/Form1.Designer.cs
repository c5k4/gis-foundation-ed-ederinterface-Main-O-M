namespace PGE.BatchApplication.AllignAnnotation
{
    partial class Form1
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
            this.chkAlignment = new System.Windows.Forms.CheckBox();
            this.chkAngle = new System.Windows.Forms.CheckBox();
            this.btnAllign = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkAlignment
            // 
            this.chkAlignment.AutoSize = true;
            this.chkAlignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAlignment.Location = new System.Drawing.Point(15, 13);
            this.chkAlignment.Name = "chkAlignment";
            this.chkAlignment.Size = new System.Drawing.Size(128, 17);
            this.chkAlignment.TabIndex = 0;
            this.chkAlignment.Text = "Change Alignment";
            this.chkAlignment.UseVisualStyleBackColor = true;
            this.chkAlignment.CheckedChanged += new System.EventHandler(this.chkAlignment_CheckedChanged);
            // 
            // chkAngle
            // 
            this.chkAngle.AutoSize = true;
            this.chkAngle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAngle.Location = new System.Drawing.Point(15, 40);
            this.chkAngle.Name = "chkAngle";
            this.chkAngle.Size = new System.Drawing.Size(143, 17);
            this.chkAngle.TabIndex = 1;
            this.chkAngle.Text = "Perpendicular to line";
            this.chkAngle.UseVisualStyleBackColor = true;
            this.chkAngle.CheckedChanged += new System.EventHandler(this.chkAngle_CheckedChanged);
            // 
            // btnAllign
            // 
            this.btnAllign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllign.Location = new System.Drawing.Point(15, 69);
            this.btnAllign.Name = "btnAllign";
            this.btnAllign.Size = new System.Drawing.Size(135, 33);
            this.btnAllign.TabIndex = 2;
            this.btnAllign.Text = "Allign Anno Features";
            this.btnAllign.UseVisualStyleBackColor = true;
            this.btnAllign.Click += new System.EventHandler(this.btnAllign_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(169, 114);
            this.Controls.Add(this.btnAllign);
            this.Controls.Add(this.chkAngle);
            this.Controls.Add(this.chkAlignment);
            this.Name = "Form1";
            this.Text = "Allign Annotations";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAlignment;
        private System.Windows.Forms.CheckBox chkAngle;
        private System.Windows.Forms.Button btnAllign;
    }
}