namespace PGE.Desktop.EDER.LocatorTools.GUIDSearchLocator
{
    partial class PGE_GUIDSearchLocatorUI
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtGUIDSearch = new System.Windows.Forms.TextBox();
            this.lblGUIDSearch = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.45F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.55F));
            this.tableLayoutPanel1.Controls.Add(this.txtGUIDSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblGUIDSearch, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.MaximumSize = new System.Drawing.Size(256, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(253, 26);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txtGUIDSearch
            // 
            this.txtGUIDSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGUIDSearch.Location = new System.Drawing.Point(110, 3);
            this.txtGUIDSearch.MaximumSize = new System.Drawing.Size(130, 20);
            this.txtGUIDSearch.MinimumSize = new System.Drawing.Size(130, 20);
            this.txtGUIDSearch.Name = "txtGUIDSearch";
            this.txtGUIDSearch.Size = new System.Drawing.Size(130, 20);
            this.txtGUIDSearch.TabIndex = 1;
            // 
            // lblGUIDSearch
            // 
            this.lblGUIDSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGUIDSearch.AutoSize = true;
            this.lblGUIDSearch.Location = new System.Drawing.Point(3, 6);
            this.lblGUIDSearch.Name = "lblGUIDSearch";
            this.lblGUIDSearch.Size = new System.Drawing.Size(101, 13);
            this.lblGUIDSearch.TabIndex = 0;
            this.lblGUIDSearch.Text = "   GLOBAL ID:";
            // 
            // PGE_GUIDSearchLocatorUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(253, 26);
            this.Name = "PGE_GUIDSearchLocatorUI";
            this.Size = new System.Drawing.Size(253, 26);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblGUIDSearch;
        private System.Windows.Forms.TextBox txtGUIDSearch;

    }
}
