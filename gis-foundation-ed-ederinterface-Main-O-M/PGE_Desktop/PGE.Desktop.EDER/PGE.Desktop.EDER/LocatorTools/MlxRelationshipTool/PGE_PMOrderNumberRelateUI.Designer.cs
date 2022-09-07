namespace PGE.Desktop.EDER.LocatorTools.MlxRelationshipTool
{
    partial class PGE_PMOrderNumberRelateUI
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
            this.lblMLXNumber = new System.Windows.Forms.Label();
            this.txtPMOrderNumber = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMLXNumber
            // 
            this.lblMLXNumber.AutoSize = true;
            this.lblMLXNumber.Location = new System.Drawing.Point(3, 32);
            this.lblMLXNumber.Name = "lblMLXNumber";
            this.lblMLXNumber.Size = new System.Drawing.Size(95, 13);
            this.lblMLXNumber.TabIndex = 0;
            this.lblMLXNumber.Text = "PM Order Number:";
            // 
            // txtPMOrderNumber
            // 
            this.txtPMOrderNumber.Location = new System.Drawing.Point(107, 28);
            this.txtPMOrderNumber.Name = "txtPMOrderNumber";
            this.txtPMOrderNumber.Size = new System.Drawing.Size(166, 20);
            this.txtPMOrderNumber.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnOk);
            this.groupBox1.Controls.Add(this.txtPMOrderNumber);
            this.groupBox1.Controls.Add(this.lblMLXNumber);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 91);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Relate Customer Agreement to PM Number";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(189, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(106, 59);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // PGE_PMOrderNumberRelateUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 106);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PGE_PMOrderNumberRelateUI";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PGE Customer Agreement Relate";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PGE_PMOrderNumberRelateUI_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMLXNumber;
        private System.Windows.Forms.TextBox txtPMOrderNumber;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}