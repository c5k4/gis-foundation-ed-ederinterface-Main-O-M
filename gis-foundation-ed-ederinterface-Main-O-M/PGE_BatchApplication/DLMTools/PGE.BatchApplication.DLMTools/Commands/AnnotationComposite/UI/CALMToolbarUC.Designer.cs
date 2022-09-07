namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite.UI
{
    partial class CALMToolbarUC
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
            this.CboCompositeType = new System.Windows.Forms.ComboBox();
            this.lblCompositeType = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CboCompositeType
            // 
            this.CboCompositeType.Dock = System.Windows.Forms.DockStyle.Top;
            this.CboCompositeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CboCompositeType.Enabled = false;
            this.CboCompositeType.Location = new System.Drawing.Point(62, 0);
            this.CboCompositeType.Margin = new System.Windows.Forms.Padding(0);
            this.CboCompositeType.Name = "CboCompositeType";
            this.CboCompositeType.Size = new System.Drawing.Size(129, 21);
            this.CboCompositeType.TabIndex = 0;
            this.CboCompositeType.SelectedIndexChanged += new System.EventHandler(this.CboCompositeType_SelectedIndexChanged);
            // 
            // lblCompositeType
            // 
            this.lblCompositeType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCompositeType.AutoSize = true;
            this.lblCompositeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompositeType.Location = new System.Drawing.Point(12, 3);
            this.lblCompositeType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 0);
            this.lblCompositeType.Name = "lblCompositeType";
            this.lblCompositeType.Size = new System.Drawing.Size(48, 13);
            this.lblCompositeType.TabIndex = 1;
            this.lblCompositeType.Text = "Favorite:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this.tableLayoutPanel1.Controls.Add(this.lblCompositeType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.CboCompositeType, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 20);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // CALMToolbarUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "MOWEDMToolbarUC";
            this.Size = new System.Drawing.Size(200, 20);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected internal System.Windows.Forms.ComboBox CboCompositeType;
        private System.Windows.Forms.Label lblCompositeType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
