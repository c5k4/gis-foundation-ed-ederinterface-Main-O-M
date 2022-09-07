using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite.UI
{
    partial class CALMSettingsUC
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.chkDeleteAnno = new CheckBox();
            this.lblOffsetX = new Label();
            this.txtOffsetX = new TextBox();
            this.txtOffsetY = new TextBox();
            this.lblOffsetY = new Label();
            this.txtRotation = new TextBox();
            this.lblRotation = new Label();
            this.lblAlignment = new Label();
            this.btnClose = new Button();
            this.cboAlignment = new ComboBox();
            this.chkInlineAnno = new CheckBox();
            this.lblSize = new Label();
            this.boldRadioYes = new RadioButton();
            this.boldRadioNo = new RadioButton();
            this.boldRadioNa = new RadioButton();
            this.boldGroupBox = new GroupBox();
            this.btnResetSettings = new Button();
            this.cboSize = new ComboBox();
            this.boldGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDeleteAnno
            // 
            this.chkDeleteAnno.AutoSize = true;
            this.chkDeleteAnno.Location = new Point(15, 200);
            this.chkDeleteAnno.Name = "chkDeleteAnno";
            this.chkDeleteAnno.Size = new Size(145, 17);
            this.chkDeleteAnno.TabIndex = 7;
            this.chkDeleteAnno.Text = "Delete Job Number Anno";
            this.chkDeleteAnno.UseVisualStyleBackColor = true;
            this.chkDeleteAnno.CheckedChanged += new EventHandler(this.AnnoCtrl_CheckedChanged);
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new Point(12, 15);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new Size(45, 13);
            this.lblOffsetX.TabIndex = 1;
            this.lblOffsetX.Text = "Offset X";
            // 
            // txtOffsetX
            // 
            this.txtOffsetX.Location = new Point(83, 12);
            this.txtOffsetX.Name = "txtOffsetX";
            this.txtOffsetX.Size = new Size(100, 20);
            this.txtOffsetX.TabIndex = 1;
            this.txtOffsetX.TextChanged += new EventHandler(this.ApplyToProperties);
            this.txtOffsetX.KeyDown += new KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Location = new Point(83, 38);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new Size(100, 20);
            this.txtOffsetY.TabIndex = 2;
            this.txtOffsetY.TextChanged += new EventHandler(this.ApplyToProperties);
            this.txtOffsetY.KeyDown += new KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new Point(12, 41);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new Size(45, 13);
            this.lblOffsetY.TabIndex = 3;
            this.lblOffsetY.Text = "Offset Y";
            // 
            // txtRotation
            // 
            this.txtRotation.Location = new Point(83, 64);
            this.txtRotation.Name = "txtRotation";
            this.txtRotation.Size = new Size(100, 20);
            this.txtRotation.TabIndex = 3;
            this.txtRotation.TextChanged += new EventHandler(this.ApplyToProperties);
            this.txtRotation.KeyDown += new KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // lblRotation
            // 
            this.lblRotation.AutoSize = true;
            this.lblRotation.Location = new Point(12, 67);
            this.lblRotation.Name = "lblRotation";
            this.lblRotation.Size = new Size(47, 13);
            this.lblRotation.TabIndex = 5;
            this.lblRotation.Text = "Rotation";
            // 
            // lblAlignment
            // 
            this.lblAlignment.AutoSize = true;
            this.lblAlignment.Location = new Point(12, 122);
            this.lblAlignment.Name = "lblAlignment";
            this.lblAlignment.Size = new Size(53, 13);
            this.lblAlignment.TabIndex = 7;
            this.lblAlignment.Text = "Alignment";
            // 
            // btnClose
            // 
            this.btnClose.Location = new Point(110, 250);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            // 
            // cboAlignment
            // 
            this.cboAlignment.DisplayMember = "Value";
            this.cboAlignment.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboAlignment.FormattingEnabled = true;
            this.cboAlignment.Location = new Point(82, 119);
            this.cboAlignment.Name = "cboAlignment";
            this.cboAlignment.Size = new Size(100, 21);
            this.cboAlignment.TabIndex = 5;
            this.cboAlignment.ValueMember = "Key";
            this.cboAlignment.SelectedValueChanged += new EventHandler(this.ApplyToProperties);
            this.cboAlignment.KeyDown += new KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // chkInlineAnno
            // 
            this.chkInlineAnno.AutoSize = true;
            this.chkInlineAnno.Location = new Point(15, 221);
            this.chkInlineAnno.Name = "chkInlineAnno";
            this.chkInlineAnno.Size = new Size(123, 17);
            this.chkInlineAnno.TabIndex = 8;
            this.chkInlineAnno.Text = "Inline Grouped Anno";
            this.chkInlineAnno.UseVisualStyleBackColor = true;
            this.chkInlineAnno.CheckedChanged += new EventHandler(this.AnnoCtrl_CheckedChanged);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new Point(12, 94);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new Size(27, 13);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "Size";
            // 
            // boldRadioYes
            // 
            this.boldRadioYes.AutoSize = true;
            this.boldRadioYes.Enabled = false;
            this.boldRadioYes.Location = new Point(70, 19);
            this.boldRadioYes.Name = "boldRadioYes";
            this.boldRadioYes.Size = new Size(46, 17);
            this.boldRadioYes.TabIndex = 12;
            this.boldRadioYes.Text = "Bold";
            this.boldRadioYes.UseVisualStyleBackColor = true;
            this.boldRadioYes.MouseUp += new MouseEventHandler(this.ApplyToProperties);
            // 
            // boldRadioNo
            // 
            this.boldRadioNo.AutoSize = true;
            this.boldRadioNo.Enabled = false;
            this.boldRadioNo.Location = new Point(6, 19);
            this.boldRadioNo.Name = "boldRadioNo";
            this.boldRadioNo.Size = new Size(63, 17);
            this.boldRadioNo.TabIndex = 11;
            this.boldRadioNo.Text = "No Bold";
            this.boldRadioNo.UseVisualStyleBackColor = true;
            this.boldRadioNo.MouseUp += new MouseEventHandler(this.ApplyToProperties);
            // 
            // boldRadioNa
            // 
            this.boldRadioNa.AutoSize = true;
            this.boldRadioNa.Enabled = false;
            this.boldRadioNa.Location = new Point(122, 19);
            this.boldRadioNa.Name = "boldRadioNa";
            this.boldRadioNa.Size = new Size(45, 17);
            this.boldRadioNa.TabIndex = 13;
            this.boldRadioNa.Text = "N/A";
            this.boldRadioNa.UseVisualStyleBackColor = true;
            this.boldRadioNa.MouseUp += new MouseEventHandler(this.ApplyToProperties);
            // 
            // boldGroupBox
            // 
            this.boldGroupBox.Controls.Add(this.boldRadioNo);
            this.boldGroupBox.Controls.Add(this.boldRadioNa);
            this.boldGroupBox.Controls.Add(this.boldRadioYes);
            this.boldGroupBox.Enabled = false;
            this.boldGroupBox.Location = new Point(12, 151);
            this.boldGroupBox.Name = "boldGroupBox";
            this.boldGroupBox.Size = new Size(173, 43);
            this.boldGroupBox.TabIndex = 6;
            this.boldGroupBox.TabStop = false;
            this.boldGroupBox.Text = "Bold Settings";
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.Location = new Point(12, 250);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new Size(75, 23);
            this.btnResetSettings.TabIndex = 9;
            this.btnResetSettings.Text = "Reset";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new EventHandler(this.btnResetSettings_Click);
            // 
            // cboSize
            // 
            this.cboSize.DisplayMember = "Value";
            this.cboSize.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboSize.FormattingEnabled = true;
            this.cboSize.Location = new Point(82, 90);
            this.cboSize.Name = "cboSize";
            this.cboSize.Size = new Size(100, 21);
            this.cboSize.TabIndex = 4;
            this.cboSize.SelectedValueChanged += new EventHandler(this.ApplyToProperties);
            this.cboSize.KeyDown += new KeyEventHandler(this.AnnoCtrl_KeyDown);
            // 
            // CALMSettingsUC
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(197, 282);
            this.Controls.Add(this.cboSize);
            this.Controls.Add(this.btnResetSettings);
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
            this.Controls.Add(this.boldGroupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(205, 221);
            this.Name = "CALMSettingsUC";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "MOWED Annotation Settings";
            this.boldGroupBox.ResumeLayout(false);
            this.boldGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox chkDeleteAnno;
        private Label lblOffsetX;
        private TextBox txtOffsetX;
        private TextBox txtOffsetY;
        private Label lblOffsetY;
        private TextBox txtRotation;
        private Label lblRotation;
        private Label lblAlignment;
        private Button btnClose;
        private ComboBox cboAlignment;
        private CheckBox chkInlineAnno;
        private Label lblSize;
        private RadioButton boldRadioYes;
        private RadioButton boldRadioNo;
        private RadioButton boldRadioNa;
        private GroupBox boldGroupBox;
        private Button btnResetSettings;
        private ComboBox cboSize;
    }
}