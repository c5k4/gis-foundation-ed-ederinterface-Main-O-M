namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    partial class PGE_TraceResults
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.includeFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBoxResults = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeFeaturesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(440, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // includeFeaturesToolStripMenuItem
            // 
            this.includeFeaturesToolStripMenuItem.Name = "includeFeaturesToolStripMenuItem";
            this.includeFeaturesToolStripMenuItem.Size = new System.Drawing.Size(105, 20);
            this.includeFeaturesToolStripMenuItem.Text = "Include Features";
            // 
            // listBoxResults
            // 
            this.listBoxResults.FormattingEnabled = true;
            this.listBoxResults.Location = new System.Drawing.Point(12, 31);
            this.listBoxResults.Name = "listBoxResults";
            this.listBoxResults.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxResults.Size = new System.Drawing.Size(416, 381);
            this.listBoxResults.TabIndex = 4;
            // 
            // PGE_TraceResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 424);
            this.Controls.Add(this.listBoxResults);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PGE_TraceResults";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PGE_TraceResults";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem includeFeaturesToolStripMenuItem;
        private System.Windows.Forms.ListBox listBoxResults;
    }
}