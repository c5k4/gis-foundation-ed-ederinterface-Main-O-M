namespace PGE.Desktop.EDER.ArcMapCommands.VersionDifference
{
    partial class VersionChangesForm
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
            this.treeChanges = new System.Windows.Forms.TreeView();
            this.gridViewChanges = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewChanges)).BeginInit();
            this.SuspendLayout();
            // 
            // treeChanges
            // 
            this.treeChanges.Location = new System.Drawing.Point(12, 12);
            this.treeChanges.Name = "treeChanges";
            this.treeChanges.Size = new System.Drawing.Size(216, 240);
            this.treeChanges.TabIndex = 0;
            // 
            // gridViewChanges
            // 
            this.gridViewChanges.AllowUserToAddRows = false;
            this.gridViewChanges.AllowUserToDeleteRows = false;
            this.gridViewChanges.AllowUserToOrderColumns = true;
            this.gridViewChanges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridViewChanges.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridViewChanges.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridViewChanges.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridViewChanges.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gridViewChanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridViewChanges.Location = new System.Drawing.Point(234, 12);
            this.gridViewChanges.Name = "gridViewChanges";
            this.gridViewChanges.ReadOnly = true;
            this.gridViewChanges.Size = new System.Drawing.Size(458, 240);
            this.gridViewChanges.TabIndex = 1;
            // 
            // VersionChangesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 262);
            this.Controls.Add(this.gridViewChanges);
            this.Controls.Add(this.treeChanges);
            this.Name = "VersionChangesForm";
            this.Text = "VersionChangesForm";
            ((System.ComponentModel.ISupportInitialize)(this.gridViewChanges)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeChanges;
        private System.Windows.Forms.DataGridView gridViewChanges;
    }
}