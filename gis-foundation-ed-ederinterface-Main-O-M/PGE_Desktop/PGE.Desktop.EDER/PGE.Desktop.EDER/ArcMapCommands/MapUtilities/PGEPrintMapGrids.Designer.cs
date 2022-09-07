namespace PGE.Desktop.EDER.ArcMapCommands
{
    partial class PGEPrintMapGrids
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PGEPrintMapGrids));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstMapNo = new System.Windows.Forms.ListBox();
            this.lstMapOffice = new System.Windows.Forms.ListBox();
            this.lblMapNo = new System.Windows.Forms.Label();
            this.lblMapOffice = new System.Windows.Forms.Label();
            this.comboLayer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(107, 276);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(188, 276);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstMapNo
            // 
            this.lstMapNo.FormattingEnabled = true;
            this.lstMapNo.Location = new System.Drawing.Point(13, 73);
            this.lstMapNo.Name = "lstMapNo";
            this.lstMapNo.Size = new System.Drawing.Size(114, 186);
            this.lstMapNo.Sorted = true;
            this.lstMapNo.TabIndex = 4;
            this.lstMapNo.SelectedIndexChanged += new System.EventHandler(this.lstMapNo_SelectedIndexChanged);
            // 
            // lstMapOffice
            // 
            this.lstMapOffice.FormattingEnabled = true;
            this.lstMapOffice.Location = new System.Drawing.Point(147, 73);
            this.lstMapOffice.Name = "lstMapOffice";
            this.lstMapOffice.Size = new System.Drawing.Size(116, 186);
            this.lstMapOffice.Sorted = true;
            this.lstMapOffice.TabIndex = 5;
            // 
            // lblMapNo
            // 
            this.lblMapNo.AutoSize = true;
            this.lblMapNo.Location = new System.Drawing.Point(11, 55);
            this.lblMapNo.Name = "lblMapNo";
            this.lblMapNo.Size = new System.Drawing.Size(116, 13);
            this.lblMapNo.TabIndex = 6;
            this.lblMapNo.Text = "Select a Map Number :";
            // 
            // lblMapOffice
            // 
            this.lblMapOffice.AutoSize = true;
            this.lblMapOffice.Location = new System.Drawing.Point(144, 55);
            this.lblMapOffice.Name = "lblMapOffice";
            this.lblMapOffice.Size = new System.Drawing.Size(107, 13);
            this.lblMapOffice.TabIndex = 7;
            this.lblMapOffice.Text = "Select a Map Office :";
            // 
            // comboLayer
            // 
            this.comboLayer.FormattingEnabled = true;
            this.comboLayer.Location = new System.Drawing.Point(76, 17);
            this.comboLayer.Name = "comboLayer";
            this.comboLayer.Size = new System.Drawing.Size(187, 21);
            this.comboLayer.TabIndex = 8;
            this.comboLayer.SelectedIndexChanged += new System.EventHandler(this.comboLayer_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Grid Layer:";
            // 
            // PGEPrintMapGrids
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 312);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboLayer);
            this.Controls.Add(this.lblMapOffice);
            this.Controls.Add(this.lblMapNo);
            this.Controls.Add(this.lstMapOffice);
            this.Controls.Add(this.lstMapNo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PGEPrintMapGrids";
            this.Text = "PGE Print Map";
            this.Load += new System.EventHandler(this.PGEPrintMapGrids_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstMapNo;
        private System.Windows.Forms.ListBox lstMapOffice;
        private System.Windows.Forms.Label lblMapNo;
        private System.Windows.Forms.Label lblMapOffice;
        private System.Windows.Forms.ComboBox comboLayer;
        private System.Windows.Forms.Label label1;
    }
}