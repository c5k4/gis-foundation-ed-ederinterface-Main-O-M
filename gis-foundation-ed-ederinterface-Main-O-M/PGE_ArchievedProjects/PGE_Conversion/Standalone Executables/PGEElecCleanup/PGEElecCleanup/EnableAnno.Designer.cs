namespace PGEElecCleanup
{
    partial class EnableAnno
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.stbInfo = new System.Windows.Forms.StatusBar();
            this.btnStopAnnos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(48, 48);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start Annos";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(171, 48);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // stbInfo
            // 
            this.stbInfo.Location = new System.Drawing.Point(0, 84);
            this.stbInfo.Name = "stbInfo";
            this.stbInfo.Size = new System.Drawing.Size(321, 22);
            this.stbInfo.TabIndex = 16;
            // 
            // btnStopAnnos
            // 
            this.btnStopAnnos.Location = new System.Drawing.Point(48, 19);
            this.btnStopAnnos.Name = "btnStopAnnos";
            this.btnStopAnnos.Size = new System.Drawing.Size(75, 23);
            this.btnStopAnnos.TabIndex = 17;
            this.btnStopAnnos.Text = "Stop Annos";
            this.btnStopAnnos.UseVisualStyleBackColor = true;
            this.btnStopAnnos.Click += new System.EventHandler(this.btnStopAnnos_Click);
            // 
            // EnableAnno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 106);
            this.Controls.Add(this.btnStopAnnos);
            this.Controls.Add(this.stbInfo);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EnableAnno";
            this.Text = "EnableAnno";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusBar stbInfo;
        private System.Windows.Forms.Button btnStopAnnos;
    }
}