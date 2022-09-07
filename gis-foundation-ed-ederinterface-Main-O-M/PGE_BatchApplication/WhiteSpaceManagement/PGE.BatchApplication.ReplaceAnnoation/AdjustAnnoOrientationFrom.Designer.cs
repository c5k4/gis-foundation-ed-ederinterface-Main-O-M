namespace PGE.BatchApplication.ReplaceAnnoation
{
    partial class AdjustAnnoOrientationFrom
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
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.txtMinDegrees = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMaxDegrees = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblAnalyze = new System.Windows.Forms.Label();
            this.resultsList = new System.Windows.Forms.ListBox();
            this.btnFixExtent = new System.Windows.Forms.Button();
            this.failedResultList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFixAll = new System.Windows.Forms.Button();
            this.lblResultsCount = new System.Windows.Forms.Label();
            this.lblFailedResultsCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(391, 337);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(127, 23);
            this.btnAnalyze.TabIndex = 0;
            this.btnAnalyze.Text = "Analyze Anno Layers";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // txtMinDegrees
            // 
            this.txtMinDegrees.Location = new System.Drawing.Point(205, 25);
            this.txtMinDegrees.Name = "txtMinDegrees";
            this.txtMinDegrees.Size = new System.Drawing.Size(27, 20);
            this.txtMinDegrees.TabIndex = 1;
            this.txtMinDegrees.Text = "97";
            this.txtMinDegrees.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(171, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Minimum Degrees";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Maximum Degrees";
            // 
            // txtMaxDegrees
            // 
            this.txtMaxDegrees.Location = new System.Drawing.Point(303, 25);
            this.txtMaxDegrees.Name = "txtMaxDegrees";
            this.txtMaxDegrees.Size = new System.Drawing.Size(27, 20);
            this.txtMaxDegrees.TabIndex = 4;
            this.txtMaxDegrees.Text = "263";
            this.txtMaxDegrees.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 51);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(506, 23);
            this.progressBar.Step = 100;
            this.progressBar.TabIndex = 6;
            // 
            // lblAnalyze
            // 
            this.lblAnalyze.AutoSize = true;
            this.lblAnalyze.Location = new System.Drawing.Point(12, 77);
            this.lblAnalyze.Name = "lblAnalyze";
            this.lblAnalyze.Size = new System.Drawing.Size(125, 13);
            this.lblAnalyze.TabIndex = 7;
            this.lblAnalyze.Text = "Currently Analyzing None";
            // 
            // resultsList
            // 
            this.resultsList.FormattingEnabled = true;
            this.resultsList.Location = new System.Drawing.Point(12, 133);
            this.resultsList.Name = "resultsList";
            this.resultsList.Size = new System.Drawing.Size(250, 186);
            this.resultsList.TabIndex = 8;
            // 
            // btnFixExtent
            // 
            this.btnFixExtent.Location = new System.Drawing.Point(268, 337);
            this.btnFixExtent.Name = "btnFixExtent";
            this.btnFixExtent.Size = new System.Drawing.Size(117, 23);
            this.btnFixExtent.TabIndex = 9;
            this.btnFixExtent.Text = "Fix Extent";
            this.btnFixExtent.UseVisualStyleBackColor = true;
            this.btnFixExtent.Click += new System.EventHandler(this.btnFixExtent_Click);
            // 
            // failedResultList
            // 
            this.failedResultList.FormattingEnabled = true;
            this.failedResultList.Location = new System.Drawing.Point(268, 133);
            this.failedResultList.Name = "failedResultList";
            this.failedResultList.Size = new System.Drawing.Size(250, 186);
            this.failedResultList.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(250, 35);
            this.label3.TabIndex = 11;
            this.label3.Text = "The following features were identified as being within the specified angle";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(268, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(250, 35);
            this.label4.TabIndex = 12;
            this.label4.Text = "For the following features the angle could not be determined.  These should be re" +
    "viewed manually.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFixAll
            // 
            this.btnFixAll.Location = new System.Drawing.Point(145, 337);
            this.btnFixAll.Name = "btnFixAll";
            this.btnFixAll.Size = new System.Drawing.Size(117, 23);
            this.btnFixAll.TabIndex = 13;
            this.btnFixAll.Text = "Fix All";
            this.btnFixAll.UseVisualStyleBackColor = true;
            this.btnFixAll.Click += new System.EventHandler(this.btnFixAll_Click);
            // 
            // lblResultsCount
            // 
            this.lblResultsCount.AutoSize = true;
            this.lblResultsCount.Location = new System.Drawing.Point(9, 321);
            this.lblResultsCount.Name = "lblResultsCount";
            this.lblResultsCount.Size = new System.Drawing.Size(90, 13);
            this.lblResultsCount.TabIndex = 14;
            this.lblResultsCount.Text = "0 Features Found";
            this.lblResultsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFailedResultsCount
            // 
            this.lblFailedResultsCount.AutoSize = true;
            this.lblFailedResultsCount.Location = new System.Drawing.Point(265, 321);
            this.lblFailedResultsCount.Name = "lblFailedResultsCount";
            this.lblFailedResultsCount.Size = new System.Drawing.Size(90, 13);
            this.lblFailedResultsCount.TabIndex = 15;
            this.lblFailedResultsCount.Text = "0 Features Found";
            this.lblFailedResultsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AdjustAnnoOrientationFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 370);
            this.Controls.Add(this.btnFixAll);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.failedResultList);
            this.Controls.Add(this.btnFixExtent);
            this.Controls.Add(this.resultsList);
            this.Controls.Add(this.lblAnalyze);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMaxDegrees);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMinDegrees);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.lblFailedResultsCount);
            this.Controls.Add(this.lblResultsCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AdjustAnnoOrientationFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Flip Annotation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.TextBox txtMinDegrees;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMaxDegrees;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblAnalyze;
        private System.Windows.Forms.ListBox resultsList;
        private System.Windows.Forms.Button btnFixExtent;
        private System.Windows.Forms.ListBox failedResultList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnFixAll;
        private System.Windows.Forms.Label lblResultsCount;
        private System.Windows.Forms.Label lblFailedResultsCount;
    }
}