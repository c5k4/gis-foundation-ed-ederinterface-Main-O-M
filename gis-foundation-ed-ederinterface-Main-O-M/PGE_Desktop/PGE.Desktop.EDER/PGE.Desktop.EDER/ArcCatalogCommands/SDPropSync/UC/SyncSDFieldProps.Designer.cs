namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync.UC
{
    partial class Re_Order_Fields
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
            this.tvStoredDisplay = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chbSelectClear = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvStoredDisplay
            // 
            this.tvStoredDisplay.CheckBoxes = true;
            this.tvStoredDisplay.Location = new System.Drawing.Point(6, 48);
            this.tvStoredDisplay.Name = "tvStoredDisplay";
            this.tvStoredDisplay.Size = new System.Drawing.Size(187, 169);
            this.tvStoredDisplay.TabIndex = 2;
            this.tvStoredDisplay.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvStoredDisplay_AfterCheck);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chbSelectClear);
            this.groupBox1.Controls.Add(this.tvStoredDisplay);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 223);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Stored Display";
            // 
            // chbSelectClear
            // 
            this.chbSelectClear.AutoSize = true;
            this.chbSelectClear.Location = new System.Drawing.Point(6, 25);
            this.chbSelectClear.Name = "chbSelectClear";
            this.chbSelectClear.Size = new System.Drawing.Size(99, 17);
            this.chbSelectClear.TabIndex = 5;
            this.chbSelectClear.Text = "Select\\Clear All";
            this.chbSelectClear.UseVisualStyleBackColor = true;
            this.chbSelectClear.CheckedChanged += new System.EventHandler(this.chbSelectClear_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(140, 241);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(61, 241);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(73, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Re_Order_Fields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 270);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Re_Order_Fields";
            this.Text = "Re_Order_Fields";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvStoredDisplay;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chbSelectClear;
    }
}