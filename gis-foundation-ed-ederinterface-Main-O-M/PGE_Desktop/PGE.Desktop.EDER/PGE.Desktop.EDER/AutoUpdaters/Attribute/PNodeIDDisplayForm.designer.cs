namespace PGE.Desktop.EDER.AutoUpdaters.Attribute
{
    partial class PNodeIDDisplayForm
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
            this.lblIDs = new System.Windows.Forms.Label();
            this.cboIDs = new System.Windows.Forms.ComboBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.lblSelect = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblIDs
            // 
            this.lblIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIDs.AutoSize = true;
            this.lblIDs.Enabled = false;
            this.lblIDs.Location = new System.Drawing.Point(15, 35);
            this.lblIDs.Name = "lblIDs";
            this.lblIDs.Size = new System.Drawing.Size(26, 13);
            this.lblIDs.TabIndex = 19;
            this.lblIDs.Text = "IDs:";
            // 
            // cboIDs
            // 
            this.cboIDs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboIDs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboIDs.Enabled = false;
            this.cboIDs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboIDs.FormattingEnabled = true;
            this.cboIDs.Location = new System.Drawing.Point(45, 31);
            this.cboIDs.Name = "cboIDs";
            this.cboIDs.Size = new System.Drawing.Size(380, 23);
            this.cboIDs.Sorted = true;
            this.cboIDs.TabIndex = 20;
            this.cboIDs.DropDown += new System.EventHandler(this.cboIDs_DropDown);
            this.cboIDs.TextChanged += new System.EventHandler(this.cboIDs_TextChanged);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(354, 67);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(71, 22);
            this.cmdCancel.TabIndex = 148;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.Enabled = false;
            this.cmdOk.Location = new System.Drawing.Point(277, 67);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(71, 22);
            this.cmdOk.TabIndex = 147;
            this.cmdOk.Text = "OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // lblSelect
            // 
            this.lblSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelect.AutoSize = true;
            this.lblSelect.Enabled = false;
            this.lblSelect.Location = new System.Drawing.Point(15, 9);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(143, 13);
            this.lblSelect.TabIndex = 149;
            this.lblSelect.Text = "Please select ID from the list:";
            // 
            // PNodeIDDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 92);
            this.Controls.Add(this.lblSelect);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.lblIDs);
            this.Controls.Add(this.cboIDs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PNodeIDDisplayForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.frmDisplayNames_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIDs;
        private System.Windows.Forms.ComboBox cboIDs;
        private System.Windows.Forms.Button cmdCancel;
        internal System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Label lblSelect;
    }
}