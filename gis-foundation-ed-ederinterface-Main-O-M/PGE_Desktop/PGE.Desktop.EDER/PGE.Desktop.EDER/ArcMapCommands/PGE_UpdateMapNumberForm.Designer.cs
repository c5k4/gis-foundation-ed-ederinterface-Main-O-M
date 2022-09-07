namespace PGE.Desktop.EDER.ArcMapCommands
{
    partial class PGE_UpdateMapNumberForm
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
            this.btnCancelUpdates = new System.Windows.Forms.Button();
            this.prgProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancelUpdates
            // 
            this.btnCancelUpdates.Location = new System.Drawing.Point(376, 41);
            this.btnCancelUpdates.Name = "btnCancelUpdates";
            this.btnCancelUpdates.Size = new System.Drawing.Size(75, 23);
            this.btnCancelUpdates.TabIndex = 0;
            this.btnCancelUpdates.Text = "Cancel";
            this.btnCancelUpdates.UseVisualStyleBackColor = true;
            this.btnCancelUpdates.Click += new System.EventHandler(this.btnCancelUpdates_Click);
            // 
            // prgProgressBar
            // 
            this.prgProgressBar.Location = new System.Drawing.Point(12, 12);
            this.prgProgressBar.Name = "prgProgressBar";
            this.prgProgressBar.Size = new System.Drawing.Size(439, 23);
            this.prgProgressBar.TabIndex = 1;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 38);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(105, 13);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "0 out of 0 Processed";
            // 
            // PGE_UpdateMapNumberForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 76);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.prgProgressBar);
            this.Controls.Add(this.btnCancelUpdates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PGE_UpdateMapNumberForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PGE Update Map Number";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancelUpdates;
        private System.Windows.Forms.ProgressBar prgProgressBar;
        private System.Windows.Forms.Label lblProgress;
    }
}