namespace PGE.Desktop.Custom.Commands.AnnotationComposite.UI
{
    partial class CALMSettingsUC
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
            this.chkDeleteAnno = new System.Windows.Forms.CheckBox();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.txtOffsetX = new System.Windows.Forms.TextBox();
            this.txtOffsetY = new System.Windows.Forms.TextBox();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.txtRotation = new System.Windows.Forms.TextBox();
            this.lblRotation = new System.Windows.Forms.Label();
            this.lblAlignment = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.cboAlignment = new System.Windows.Forms.ComboBox();
            this.chkInlineAnno = new System.Windows.Forms.CheckBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.btnResetSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkDeleteAnno
            // 
            this.chkDeleteAnno.AutoSize = true;
            this.chkDeleteAnno.Location = new System.Drawing.Point(15, 152);
            this.chkDeleteAnno.Name = "chkDeleteAnno";
            this.chkDeleteAnno.Size = new System.Drawing.Size(145, 17);
            this.chkDeleteAnno.TabIndex = 7;
            this.chkDeleteAnno.Text = "Delete Job Number Anno";
            this.chkDeleteAnno.UseVisualStyleBackColor = true;
            this.chkDeleteAnno.CheckedChanged += new System.EventHandler(this.AnnoCtrl_CheckedChanged);
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(12, 15);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(45, 13);
            this.lblOffsetX.TabIndex = 1;
            this.lblOffsetX.Text = "Offset X";
            // 
            // txtOffsetX
            // 
            this.txtOffsetX.Location = new System.Drawing.Point(83, 12);
            this.txtOffsetX.Name = "txtOffsetX";
            this.txtOffsetX.Size = new System.Drawing.Size(100, 20);
            this.txtOffsetX.TabIndex = 1;
            this.txtOffsetX.TextChanged += new System.EventHandler(this.ApplyToProperties);
            this.txtOffsetX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Location = new System.Drawing.Point(83, 38);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new System.Drawing.Size(100, 20);
            this.txtOffsetY.TabIndex = 2;
            this.txtOffsetY.TextChanged += new System.EventHandler(this.ApplyToProperties);
            this.txtOffsetY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(12, 41);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(45, 13);
            this.lblOffsetY.TabIndex = 3;
            this.lblOffsetY.Text = "Offset Y";
            // 
            // txtRotation
            // 
            this.txtRotation.Location = new System.Drawing.Point(83, 64);
            this.txtRotation.Name = "txtRotation";
            this.txtRotation.Size = new System.Drawing.Size(100, 20);
            this.txtRotation.TabIndex = 3;
            this.txtRotation.TextChanged += new System.EventHandler(this.ApplyToProperties);
            this.txtRotation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // lblRotation
            // 
            this.lblRotation.AutoSize = true;
            this.lblRotation.Location = new System.Drawing.Point(12, 67);
            this.lblRotation.Name = "lblRotation";
            this.lblRotation.Size = new System.Drawing.Size(47, 13);
            this.lblRotation.TabIndex = 5;
            this.lblRotation.Text = "Rotation";
            // 
            // lblAlignment
            // 
            this.lblAlignment.AutoSize = true;
            this.lblAlignment.Location = new System.Drawing.Point(12, 122);
            this.lblAlignment.Name = "lblAlignment";
            this.lblAlignment.Size = new System.Drawing.Size(53, 13);
            this.lblAlignment.TabIndex = 7;
            this.lblAlignment.Text = "Alignment";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(110, 202);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cboAlignment
            // 
            this.cboAlignment.DisplayMember = "Value";
            this.cboAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAlignment.FormattingEnabled = true;
            this.cboAlignment.Location = new System.Drawing.Point(82, 119);
            this.cboAlignment.Name = "cboAlignment";
            this.cboAlignment.Size = new System.Drawing.Size(100, 21);
            this.cboAlignment.TabIndex = 5;
            this.cboAlignment.ValueMember = "Key";
            this.cboAlignment.SelectedValueChanged += new System.EventHandler(this.ApplyToProperties);
            this.cboAlignment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // chkInlineAnno
            // 
            this.chkInlineAnno.AutoSize = true;
            this.chkInlineAnno.Location = new System.Drawing.Point(15, 173);
            this.chkInlineAnno.Name = "chkInlineAnno";
            this.chkInlineAnno.Size = new System.Drawing.Size(123, 17);
            this.chkInlineAnno.TabIndex = 8;
            this.chkInlineAnno.Text = "Inline Grouped Anno";
            this.chkInlineAnno.UseVisualStyleBackColor = true;
            this.chkInlineAnno.CheckedChanged += new System.EventHandler(this.AnnoCtrl_CheckedChanged);
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(83, 91);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(100, 20);
            this.txtSize.TabIndex = 4;
            this.txtSize.TextChanged += new System.EventHandler(this.ApplyToProperties);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(12, 94);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(27, 13);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "Size";
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.Location = new System.Drawing.Point(12, 202);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new System.Drawing.Size(75, 23);
            this.btnResetSettings.TabIndex = 9;
            this.btnResetSettings.Text = "Reset";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new System.EventHandler(this.btnResetSettings_Click);
            // 
            // CALMSettingsUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 233);
            this.Controls.Add(this.btnResetSettings);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.chkInlineAnno);
            this.Controls.Add(this.cboAlignment);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblAlignment);
            this.Controls.Add(this.txtRotation);
            this.Controls.Add(this.lblRotation);
            this.Controls.Add(this.txtOffsetY);
            this.Controls.Add(this.lblOffsetY);
            this.Controls.Add(this.txtOffsetX);
            this.Controls.Add(this.lblOffsetX);
            this.Controls.Add(this.chkDeleteAnno);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(205, 221);
            this.Name = "CALMSettingsUC";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CALM Annotation Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDeleteAnno;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.TextBox txtOffsetX;
        private System.Windows.Forms.TextBox txtOffsetY;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.TextBox txtRotation;
        private System.Windows.Forms.Label lblRotation;
        private System.Windows.Forms.Label lblAlignment;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cboAlignment;
        private System.Windows.Forms.CheckBox chkInlineAnno;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Button btnResetSettings;
    }
}