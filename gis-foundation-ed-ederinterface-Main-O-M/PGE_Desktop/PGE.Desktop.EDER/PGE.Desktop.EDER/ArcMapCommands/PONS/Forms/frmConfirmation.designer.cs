namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    partial class frmConfirmation
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfirmation));
            this.dgConfirmation = new System.Windows.Forms.DataGridView();
            this.ColDevice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFeeder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.feeder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colObjectID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnConfirmationOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgConfirmation)).BeginInit();
            this.SuspendLayout();
            // 
            // dgConfirmation
            // 
            this.dgConfirmation.AllowUserToAddRows = false;
            this.dgConfirmation.AllowUserToDeleteRows = false;
            this.dgConfirmation.AllowUserToResizeRows = false;
            this.dgConfirmation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgConfirmation.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgConfirmation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgConfirmation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColDevice,
            this.colFeeder,
            this.feeder,
            this.colObjectID});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgConfirmation.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgConfirmation.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgConfirmation.Location = new System.Drawing.Point(12, 10);
            this.dgConfirmation.MultiSelect = false;
            this.dgConfirmation.Name = "dgConfirmation";
            this.dgConfirmation.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgConfirmation.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgConfirmation.RowHeadersVisible = false;
            this.dgConfirmation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgConfirmation.ShowEditingIcon = false;
            this.dgConfirmation.Size = new System.Drawing.Size(472, 224);
            this.dgConfirmation.TabIndex = 3;
            // 
            // ColDevice
            // 
            this.ColDevice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColDevice.FillWeight = 30F;
            this.ColDevice.HeaderText = "Device Name";
            this.ColDevice.Name = "ColDevice";
            this.ColDevice.ReadOnly = true;
            // 
            // colFeeder
            // 
            this.colFeeder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFeeder.FillWeight = 30F;
            this.colFeeder.HeaderText = "Device ID";
            this.colFeeder.Name = "colFeeder";
            this.colFeeder.ReadOnly = true;
            // 
            // feeder
            // 
            this.feeder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.feeder.FillWeight = 40F;
            this.feeder.HeaderText = "Feeder";
            this.feeder.Name = "feeder";
            this.feeder.ReadOnly = true;
            // 
            // colObjectID
            // 
            this.colObjectID.HeaderText = "Object ID";
            this.colObjectID.Name = "colObjectID";
            this.colObjectID.ReadOnly = true;
            // 
            // btnConfirmationOK
            // 
            this.btnConfirmationOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnConfirmationOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirmationOK.Location = new System.Drawing.Point(211, 242);
            this.btnConfirmationOK.Name = "btnConfirmationOK";
            this.btnConfirmationOK.Size = new System.Drawing.Size(75, 23);
            this.btnConfirmationOK.TabIndex = 4;
            this.btnConfirmationOK.Text = "&OK";
            this.btnConfirmationOK.UseVisualStyleBackColor = true;
            this.btnConfirmationOK.Click += new System.EventHandler(this.btnConfirmationOK_Click);
            // 
            // frmConfirmation
            // 
            this.AcceptButton = this.btnConfirmationOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 272);
            this.Controls.Add(this.btnConfirmationOK);
            this.Controls.Add(this.dgConfirmation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfirmation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Device";
            ((System.ComponentModel.ISupportInitialize)(this.dgConfirmation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgConfirmation;
        private System.Windows.Forms.Button btnConfirmationOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDevice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFeeder;
        private System.Windows.Forms.DataGridViewTextBoxColumn feeder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colObjectID;
    }
}