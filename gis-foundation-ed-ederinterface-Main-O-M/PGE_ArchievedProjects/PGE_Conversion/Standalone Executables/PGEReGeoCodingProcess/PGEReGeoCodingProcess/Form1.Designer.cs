namespace PGEReGeoCodingProcess
{
    partial class FormReGeoCoding
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReGeoCoding));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAllFC = new System.Windows.Forms.ComboBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnSDEConn = new System.Windows.Forms.Button();
            this.btnGDB = new System.Windows.Forms.Button();
            this.rad1_MMode = new System.Windows.Forms.RadioButton();
            this.rad1_1 = new System.Windows.Forms.RadioButton();
            this.lblMsg = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbAllFC);
            this.groupBox1.Controls.Add(this.btnExecute);
            this.groupBox1.Controls.Add(this.btnSDEConn);
            this.groupBox1.Controls.Add(this.btnGDB);
            this.groupBox1.Controls.Add(this.rad1_MMode);
            this.groupBox1.Controls.Add(this.rad1_1);
            this.groupBox1.Controls.Add(this.lblMsg);
            this.groupBox1.Location = new System.Drawing.Point(8, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(439, 208);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Where Clause";
            // 
            // cmbAllFC
            // 
            this.cmbAllFC.FormattingEnabled = true;
            this.cmbAllFC.Location = new System.Drawing.Point(106, 136);
            this.cmbAllFC.Name = "cmbAllFC";
            this.cmbAllFC.Size = new System.Drawing.Size(300, 21);
            this.cmbAllFC.TabIndex = 7;
            this.cmbAllFC.Text = "cmbWhereClause";
            this.cmbAllFC.SelectedIndexChanged += new System.EventHandler(this.cmbAllFC_SelectedIndexChanged);
            this.cmbAllFC.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbAllFC_MouseClick);
            // 
            // btnExecute
            // 
            this.btnExecute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnExecute.BackgroundImage")));
            this.btnExecute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExecute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Location = new System.Drawing.Point(331, 34);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 75);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "&Geo Code";
            this.btnExecute.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnSDEConn
            // 
            this.btnSDEConn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSDEConn.BackgroundImage")));
            this.btnSDEConn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSDEConn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSDEConn.Location = new System.Drawing.Point(27, 76);
            this.btnSDEConn.Name = "btnSDEConn";
            this.btnSDEConn.Size = new System.Drawing.Size(75, 46);
            this.btnSDEConn.TabIndex = 5;
            this.btnSDEConn.Text = "SDE Conn Browser";
            this.btnSDEConn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSDEConn.UseVisualStyleBackColor = true;
            this.btnSDEConn.Click += new System.EventHandler(this.btnSDEConn_Click);
            // 
            // btnGDB
            // 
            this.btnGDB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnGDB.BackgroundImage")));
            this.btnGDB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnGDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGDB.Location = new System.Drawing.Point(27, 19);
            this.btnGDB.Name = "btnGDB";
            this.btnGDB.Size = new System.Drawing.Size(75, 46);
            this.btnGDB.TabIndex = 4;
            this.btnGDB.Text = "GDB File Browser";
            this.btnGDB.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGDB.UseVisualStyleBackColor = true;
            this.btnGDB.Click += new System.EventHandler(this.btnGDB_Click);
            // 
            // rad1_MMode
            // 
            this.rad1_MMode.AutoSize = true;
            this.rad1_MMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rad1_MMode.Location = new System.Drawing.Point(183, 95);
            this.rad1_MMode.Name = "rad1_MMode";
            this.rad1_MMode.Size = new System.Drawing.Size(84, 17);
            this.rad1_MMode.TabIndex = 2;
            this.rad1_MMode.TabStop = true;
            this.rad1_MMode.Text = "1_M Mode";
            this.rad1_MMode.UseVisualStyleBackColor = true;
            this.rad1_MMode.CheckedChanged += new System.EventHandler(this.rad1_MMode_CheckedChanged);
            // 
            // rad1_1
            // 
            this.rad1_1.AutoSize = true;
            this.rad1_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rad1_1.Location = new System.Drawing.Point(183, 34);
            this.rad1_1.Name = "rad1_1";
            this.rad1_1.Size = new System.Drawing.Size(81, 17);
            this.rad1_1.TabIndex = 1;
            this.rad1_1.TabStop = true;
            this.rad1_1.Text = "1_1 Mode";
            this.rad1_1.UseVisualStyleBackColor = true;
            this.rad1_1.CheckedChanged += new System.EventHandler(this.rad1_1_CheckedChanged);
            // 
            // lblMsg
            // 
            this.lblMsg.Location = new System.Drawing.Point(4, 183);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(429, 22);
            this.lblMsg.TabIndex = 0;
            // 
            // FormReGeoCoding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(457, 229);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.Color.Sienna;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReGeoCoding";
            this.Text = "Re Geocoding Process";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnSDEConn;
        private System.Windows.Forms.Button btnGDB;
        public System.Windows.Forms.RadioButton rad1_MMode;
        public System.Windows.Forms.RadioButton rad1_1;
        public System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAllFC;
    }
}

