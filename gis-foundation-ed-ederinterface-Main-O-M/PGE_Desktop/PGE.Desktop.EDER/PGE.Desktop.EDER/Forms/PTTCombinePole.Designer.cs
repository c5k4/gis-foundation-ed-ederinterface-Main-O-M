namespace Telvent.PGE.ED.Desktop.Forms
{
    partial class PTTCombinePole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PTTCombinePole));
            this.gridAttributes = new System.Windows.Forms.DataGridView();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StagingPole = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExistingPole = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrueFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCombingToExisting = new System.Windows.Forms.Button();
            this.btnCombineToStaging = new System.Windows.Forms.Button();
            this.lblCombineDescription = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridAttributes)).BeginInit();
            this.SuspendLayout();
            // 
            // gridAttributes
            // 
            this.gridAttributes.AllowUserToAddRows = false;
            this.gridAttributes.AllowUserToDeleteRows = false;
            this.gridAttributes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.gridAttributes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridAttributes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAttributes.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gridAttributes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FieldName,
            this.StagingPole,
            this.ExistingPole,
            this.TrueFieldName});
            this.gridAttributes.Location = new System.Drawing.Point(12, 44);
            this.gridAttributes.Name = "gridAttributes";
            this.gridAttributes.ReadOnly = true;
            this.gridAttributes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridAttributes.Size = new System.Drawing.Size(880, 294);
            this.gridAttributes.TabIndex = 0;
            this.gridAttributes.SelectionChanged += new System.EventHandler(this.gridAttributes_SelectionChanged);
            // 
            // FieldName
            // 
            this.FieldName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FieldName.FillWeight = 30F;
            this.FieldName.HeaderText = "Field Name";
            this.FieldName.Name = "FieldName";
            this.FieldName.ReadOnly = true;
            this.FieldName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FieldName.Width = 66;
            // 
            // StagingPole
            // 
            this.StagingPole.HeaderText = "Staging Pole";
            this.StagingPole.Name = "StagingPole";
            this.StagingPole.ReadOnly = true;
            this.StagingPole.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ExistingPole
            // 
            this.ExistingPole.HeaderText = "Existing Pole";
            this.ExistingPole.Name = "ExistingPole";
            this.ExistingPole.ReadOnly = true;
            // 
            // TrueFieldName
            // 
            this.TrueFieldName.HeaderText = "TrueFieldName";
            this.TrueFieldName.Name = "TrueFieldName";
            this.TrueFieldName.ReadOnly = true;
            this.TrueFieldName.Visible = false;
            // 
            // btnCombingToExisting
            // 
            this.btnCombingToExisting.Location = new System.Drawing.Point(757, 344);
            this.btnCombingToExisting.Name = "btnCombingToExisting";
            this.btnCombingToExisting.Size = new System.Drawing.Size(140, 23);
            this.btnCombingToExisting.TabIndex = 1;
            this.btnCombingToExisting.Text = "Combine to Existing Pole";
            this.btnCombingToExisting.UseVisualStyleBackColor = true;
            this.btnCombingToExisting.Click += new System.EventHandler(this.btnCombingToExisting_Click);
            // 
            // btnCombineToStaging
            // 
            this.btnCombineToStaging.Location = new System.Drawing.Point(611, 344);
            this.btnCombineToStaging.Name = "btnCombineToStaging";
            this.btnCombineToStaging.Size = new System.Drawing.Size(140, 23);
            this.btnCombineToStaging.TabIndex = 2;
            this.btnCombineToStaging.Text = "Combine to Staging Pole";
            this.btnCombineToStaging.UseVisualStyleBackColor = true;
            this.btnCombineToStaging.Click += new System.EventHandler(this.btnCombineToStaging_Click);
            // 
            // lblCombineDescription
            // 
            this.lblCombineDescription.Location = new System.Drawing.Point(12, 9);
            this.lblCombineDescription.Name = "lblCombineDescription";
            this.lblCombineDescription.Size = new System.Drawing.Size(880, 32);
            this.lblCombineDescription.TabIndex = 3;
            this.lblCombineDescription.Text = resources.GetString("lblCombineDescription.Text");
            this.lblCombineDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(465, 344);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // PTTCombinePole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 373);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblCombineDescription);
            this.Controls.Add(this.btnCombineToStaging);
            this.Controls.Add(this.btnCombingToExisting);
            this.Controls.Add(this.gridAttributes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PTTCombinePole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PTT Combine Pole";
            ((System.ComponentModel.ISupportInitialize)(this.gridAttributes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridAttributes;
        private System.Windows.Forms.Button btnCombingToExisting;
        private System.Windows.Forms.Button btnCombineToStaging;
        private System.Windows.Forms.Label lblCombineDescription;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn StagingPole;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExistingPole;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrueFieldName;
    }
}