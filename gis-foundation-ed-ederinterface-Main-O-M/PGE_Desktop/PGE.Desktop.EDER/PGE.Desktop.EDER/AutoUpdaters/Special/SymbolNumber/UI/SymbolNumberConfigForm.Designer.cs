namespace PGE.Desktop.EDER.SymbolNumber.UI
{
    partial class SymbolNumberConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolNumberConfigForm));
            this.featureClassNameLabel = new System.Windows.Forms.Label();
            this.featureClassComboBox = new System.Windows.Forms.ComboBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.validateRuleGroupBox = new System.Windows.Forms.GroupBox();
            this.ruleTextBox = new System.Windows.Forms.TextBox();
            this.datasetComboBox = new System.Windows.Forms.ComboBox();
            this.datasetLbael = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.validateRuleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // featureClassNameLabel
            // 
            this.featureClassNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featureClassNameLabel.AutoSize = true;
            this.featureClassNameLabel.Location = new System.Drawing.Point(12, 38);
            this.featureClassNameLabel.Name = "featureClassNameLabel";
            this.featureClassNameLabel.Size = new System.Drawing.Size(74, 13);
            this.featureClassNameLabel.TabIndex = 0;
            this.featureClassNameLabel.Text = "Feature Class:";
            // 
            // featureClassComboBox
            // 
            this.featureClassComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featureClassComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.featureClassComboBox.FormattingEnabled = true;
            this.featureClassComboBox.Location = new System.Drawing.Point(92, 33);
            this.featureClassComboBox.Name = "featureClassComboBox";
            this.featureClassComboBox.Size = new System.Drawing.Size(329, 21);
            this.featureClassComboBox.Sorted = true;
            this.featureClassComboBox.TabIndex = 2;
            this.featureClassComboBox.SelectionChangeCommitted += new System.EventHandler(this.featureClassComboBox_SelectionChangeCommitted);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(508, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // validateButton
            // 
            this.validateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.validateButton.Enabled = false;
            this.validateButton.Location = new System.Drawing.Point(427, 4);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(75, 23);
            this.validateButton.TabIndex = 5;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(508, 33);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // validateRuleGroupBox
            // 
            this.validateRuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.validateRuleGroupBox.Controls.Add(this.ruleTextBox);
            this.validateRuleGroupBox.Location = new System.Drawing.Point(15, 62);
            this.validateRuleGroupBox.Name = "validateRuleGroupBox";
            this.validateRuleGroupBox.Size = new System.Drawing.Size(568, 527);
            this.validateRuleGroupBox.TabIndex = 3;
            this.validateRuleGroupBox.TabStop = false;
            this.validateRuleGroupBox.Text = "Validation Rule Defintion";
            // 
            // ruleTextBox
            // 
            this.ruleTextBox.AcceptsReturn = true;
            this.ruleTextBox.AcceptsTab = true;
            this.ruleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ruleTextBox.Enabled = false;
            this.ruleTextBox.Location = new System.Drawing.Point(6, 19);
            this.ruleTextBox.Multiline = true;
            this.ruleTextBox.Name = "ruleTextBox";
            this.ruleTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ruleTextBox.Size = new System.Drawing.Size(556, 502);
            this.ruleTextBox.TabIndex = 4;
            this.ruleTextBox.TextChanged += new System.EventHandler(this.ruleTextBox_TextChanged);
            this.ruleTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ruleTextBox_KeyDown);
            // 
            // datasetComboBox
            // 
            this.datasetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datasetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.datasetComboBox.FormattingEnabled = true;
            this.datasetComboBox.Location = new System.Drawing.Point(92, 6);
            this.datasetComboBox.Name = "datasetComboBox";
            this.datasetComboBox.Size = new System.Drawing.Size(329, 21);
            this.datasetComboBox.Sorted = true;
            this.datasetComboBox.TabIndex = 1;
            this.datasetComboBox.SelectionChangeCommitted += new System.EventHandler(this.datasetComboBox_SelectionChangeCommitted);
            // 
            // datasetLbael
            // 
            this.datasetLbael.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datasetLbael.AutoSize = true;
            this.datasetLbael.Location = new System.Drawing.Point(12, 12);
            this.datasetLbael.Name = "datasetLbael";
            this.datasetLbael.Size = new System.Drawing.Size(47, 13);
            this.datasetLbael.TabIndex = 7;
            this.datasetLbael.Text = "Dataset:";
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(427, 33);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 7;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // SymbolNumberConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(595, 597);
            this.Controls.Add(this.datasetLbael);
            this.Controls.Add(this.datasetComboBox);
            this.Controls.Add(this.validateRuleGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.featureClassComboBox);
            this.Controls.Add(this.featureClassNameLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SymbolNumberConfigForm";
            this.Text = "Rule Defintion Editor";
            this.Load += new System.EventHandler(this.SymbolNumberConfigForm_Load);
            this.VisibleChanged += new System.EventHandler(this.SymbolNumberConfigForm_VisibleChanged);
            this.validateRuleGroupBox.ResumeLayout(false);
            this.validateRuleGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label featureClassNameLabel;
        private System.Windows.Forms.ComboBox featureClassComboBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox validateRuleGroupBox;
        private System.Windows.Forms.TextBox ruleTextBox;
        private System.Windows.Forms.ComboBox datasetComboBox;
        private System.Windows.Forms.Label datasetLbael;
        private System.Windows.Forms.Button deleteButton;
    }
}