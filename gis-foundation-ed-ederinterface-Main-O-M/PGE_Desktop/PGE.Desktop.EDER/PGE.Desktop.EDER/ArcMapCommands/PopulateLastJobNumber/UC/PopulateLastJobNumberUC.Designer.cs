namespace PGE.Desktop.EDER.ArcMapCommands
{
    partial class PopulateLastJobNumberUC
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
            this.JobNumberLabel = new System.Windows.Forms.Label();
            this.JobNumberTxtBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // JobNumberLabel
            // 
            this.JobNumberLabel.AutoSize = true;
            this.JobNumberLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JobNumberLabel.Location = new System.Drawing.Point(3, 3);
            this.JobNumberLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.JobNumberLabel.Name = "JobNumberLabel";
            this.JobNumberLabel.Size = new System.Drawing.Size(74, 13);
            this.JobNumberLabel.TabIndex = 0;
            this.JobNumberLabel.Text = "Job Number";
            // 
            // JobNumberTxtBox
            // 
            this.JobNumberTxtBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.JobNumberTxtBox.Enabled = false;
            this.JobNumberTxtBox.Location = new System.Drawing.Point(80, 0);
            this.JobNumberTxtBox.Margin = new System.Windows.Forms.Padding(0);
            this.JobNumberTxtBox.Name = "JobNumberTxtBox";
            this.JobNumberTxtBox.Size = new System.Drawing.Size(172, 20);
            this.JobNumberTxtBox.TabIndex = 1;
            this.JobNumberTxtBox.TextChanged += new System.EventHandler(this.EnableButtons);
            this.JobNumberTxtBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JobNumberTxtBox_KeyDown);
            this.JobNumberTxtBox.Leave += new System.EventHandler(this.JobNumberTxtBox_Leave);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.JobNumberLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.JobNumberTxtBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 20);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // PopulateLastJobNumberUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PopulateLastJobNumberUC";
            this.Size = new System.Drawing.Size(252, 20);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label JobNumberLabel;
        protected internal System.Windows.Forms.TextBox JobNumberTxtBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
