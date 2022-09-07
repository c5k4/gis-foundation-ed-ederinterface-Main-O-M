namespace PGE.Desktop.EDER.UFM.Operations
{
    partial class SetButterflyFilterControl
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
            this.groupCreateAnno = new System.Windows.Forms.GroupBox();
            this.butReset = new System.Windows.Forms.Button();
            this.butRefresh = new System.Windows.Forms.Button();
            this.lblVault = new System.Windows.Forms.Label();
            this.cboVaults = new System.Windows.Forms.ComboBox();
            this.groupCreateAnno.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCreateAnno
            // 
            this.groupCreateAnno.Controls.Add(this.butReset);
            this.groupCreateAnno.Controls.Add(this.butRefresh);
            this.groupCreateAnno.Controls.Add(this.lblVault);
            this.groupCreateAnno.Controls.Add(this.cboVaults);
            this.groupCreateAnno.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCreateAnno.Location = new System.Drawing.Point(0, 0);
            this.groupCreateAnno.Name = "groupCreateAnno";
            this.groupCreateAnno.Size = new System.Drawing.Size(296, 125);
            this.groupCreateAnno.TabIndex = 4;
            this.groupCreateAnno.TabStop = false;
            this.groupCreateAnno.Text = "Set Butterfly Filter";
            // 
            // butReset
            // 
            this.butReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butReset.Location = new System.Drawing.Point(200, 69);
            this.butReset.Name = "butReset";
            this.butReset.Size = new System.Drawing.Size(71, 22);
            this.butReset.TabIndex = 11;
            this.butReset.Text = "Reset";
            this.butReset.UseVisualStyleBackColor = true;
            this.butReset.Click += new System.EventHandler(this.butReset_Click);
            // 
            // butRefresh
            // 
            this.butRefresh.Location = new System.Drawing.Point(123, 69);
            this.butRefresh.Name = "butRefresh";
            this.butRefresh.Size = new System.Drawing.Size(71, 22);
            this.butRefresh.TabIndex = 10;
            this.butRefresh.Text = "Refresh";
            this.butRefresh.UseVisualStyleBackColor = true;
            this.butRefresh.Visible = false;
            this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
            // 
            // lblVault
            // 
            this.lblVault.AutoSize = true;
            this.lblVault.Location = new System.Drawing.Point(6, 32);
            this.lblVault.Name = "lblVault";
            this.lblVault.Size = new System.Drawing.Size(44, 13);
            this.lblVault.TabIndex = 9;
            this.lblVault.Text = "Vault #:";
            // 
            // cboVaults
            // 
            this.cboVaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboVaults.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVaults.FormattingEnabled = true;
            this.cboVaults.ItemHeight = 13;
            this.cboVaults.Location = new System.Drawing.Point(200, 29);
            this.cboVaults.Name = "cboVaults";
            this.cboVaults.Size = new System.Drawing.Size(71, 21);
            this.cboVaults.TabIndex = 8;
            this.cboVaults.DropDown += new System.EventHandler(this.cboVaults_DropDown);
            this.cboVaults.SelectionChangeCommitted += new System.EventHandler(this.cboVaults_SelectionChangeCommitted);
            // 
            // SetButterflyFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupCreateAnno);
            this.Name = "SetButterflyFilterControl";
            this.Size = new System.Drawing.Size(296, 282);
            this.Load += new System.EventHandler(this.SetButterflyFilterControl_Load);
            this.groupCreateAnno.ResumeLayout(false);
            this.groupCreateAnno.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupCreateAnno;
        private System.Windows.Forms.Button butReset;
        private System.Windows.Forms.Button butRefresh;
        private System.Windows.Forms.Label lblVault;
        private System.Windows.Forms.ComboBox cboVaults;

    }
}
