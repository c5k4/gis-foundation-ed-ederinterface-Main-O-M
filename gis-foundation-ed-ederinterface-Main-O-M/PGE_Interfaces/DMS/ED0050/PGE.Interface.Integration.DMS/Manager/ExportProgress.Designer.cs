namespace PGE.Interface.Integration.DMS.Manager
{
    partial class ExportProgress
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
            this.label4 = new System.Windows.Forms.Label();
            this.progBarOverall = new System.Windows.Forms.ProgressBar();
            this.lblOverallProgress = new System.Windows.Forms.Label();
            this.lblProcessed = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblToProcess = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(92, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 23);
            this.label4.TabIndex = 12;
            this.label4.Text = "Overall Progress";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progBarOverall
            // 
            this.progBarOverall.Location = new System.Drawing.Point(14, 90);
            this.progBarOverall.Name = "progBarOverall";
            this.progBarOverall.Size = new System.Drawing.Size(303, 23);
            this.progBarOverall.Step = 100;
            this.progBarOverall.TabIndex = 13;
            // 
            // lblOverallProgress
            // 
            this.lblOverallProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOverallProgress.Location = new System.Drawing.Point(12, 116);
            this.lblOverallProgress.Name = "lblOverallProgress";
            this.lblOverallProgress.Size = new System.Drawing.Size(305, 25);
            this.lblOverallProgress.TabIndex = 16;
            this.lblOverallProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblProcessed
            // 
            this.lblProcessed.Location = new System.Drawing.Point(203, 38);
            this.lblProcessed.Name = "lblProcessed";
            this.lblProcessed.Size = new System.Drawing.Size(38, 16);
            this.lblProcessed.TabIndex = 18;
            this.lblProcessed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(91, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Circuits Exported:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblToProcess
            // 
            this.lblToProcess.Location = new System.Drawing.Point(204, 9);
            this.lblToProcess.Name = "lblToProcess";
            this.lblToProcess.Size = new System.Drawing.Size(38, 16);
            this.lblToProcess.TabIndex = 20;
            this.lblToProcess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(92, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 16);
            this.label2.TabIndex = 19;
            this.label2.Text = "Circuits To Export:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExportProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 150);
            this.Controls.Add(this.lblToProcess);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblProcessed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblOverallProgress);
            this.Controls.Add(this.progBarOverall);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Progress";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progBarOverall;
        private System.Windows.Forms.Label lblOverallProgress;
        private System.Windows.Forms.Label lblProcessed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblToProcess;
        private System.Windows.Forms.Label label2;
    }
}