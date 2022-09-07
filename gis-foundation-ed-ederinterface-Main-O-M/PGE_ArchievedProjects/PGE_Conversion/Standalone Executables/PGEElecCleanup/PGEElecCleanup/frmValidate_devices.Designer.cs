namespace PGEElecCleanup
{
    partial class frmValidate_devices
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
            this.txtDistTol = new System.Windows.Forms.TextBox();
            this.lblDistance = new System.Windows.Forms.Label();
            this.stbInfo = new System.Windows.Forms.StatusBar();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeViewTasks = new System.Windows.Forms.TreeView();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDistTol
            // 
            this.txtDistTol.Location = new System.Drawing.Point(177, 238);
            this.txtDistTol.Name = "txtDistTol";
            this.txtDistTol.Size = new System.Drawing.Size(196, 20);
            this.txtDistTol.TabIndex = 19;
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(51, 246);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(100, 13);
            this.lblDistance.TabIndex = 18;
            this.lblDistance.Text = "Tolerance Distance";
            // 
            // stbInfo
            // 
            this.stbInfo.Location = new System.Drawing.Point(0, 326);
            this.stbInfo.Name = "stbInfo";
            this.stbInfo.Size = new System.Drawing.Size(415, 22);
            this.stbInfo.TabIndex = 17;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(221, 281);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(100, 281);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 15;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeViewTasks);
            this.groupBox1.Controls.Add(this.txtDistTol);
            this.groupBox1.Controls.Add(this.lblDistance);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnExecute);
            this.groupBox1.Location = new System.Drawing.Point(5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 315);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report features fall more than one feature with in given tolerance";
            // 
            // treeViewTasks
            // 
            this.treeViewTasks.CheckBoxes = true;
            this.treeViewTasks.Location = new System.Drawing.Point(11, 30);
            this.treeViewTasks.Name = "treeViewTasks";
            this.treeViewTasks.Size = new System.Drawing.Size(386, 193);
            this.treeViewTasks.TabIndex = 22;
            this.treeViewTasks.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewTasks_NodeMouseClick);
            this.treeViewTasks.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeViewTasks_MouseClick);
            // 
            // frmValidate_devices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 348);
            this.Controls.Add(this.stbInfo);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmValidate_devices";
            this.Text = "frmValidate_devices";
            this.Load += new System.EventHandler(this.frmValidate_devices_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtDistTol;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.StatusBar stbInfo;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeViewTasks;
    }
}