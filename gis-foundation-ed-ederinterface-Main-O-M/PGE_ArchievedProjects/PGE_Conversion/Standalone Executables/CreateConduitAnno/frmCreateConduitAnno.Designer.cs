namespace CreateConduitAnno
{
    partial class frmCreateConduitAnno
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateConduitAnno));
            this.btnRun = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtAnno_class_id_to_copy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAnno_class_id = new System.Windows.Forms.TextBox();
            this.txtAnno_symbol_id = new System.Windows.Forms.TextBox();
            this.txtAnno_cursor_table_name = new System.Windows.Forms.TextBox();
            this.txtConductor_feature_cursor_table_name = new System.Windows.Forms.TextBox();
            this.txtAnno_feature_class_alias = new System.Windows.Forms.TextBox();
            this.txtConductor_feature_class_alias = new System.Windows.Forms.TextBox();
            this.txtConductor_labeltext_attrib = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSecUGConduit = new System.Windows.Forms.Button();
            this.btnPriUGConduit = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(3, 378);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(73, 24);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(104, 378);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtAnno_class_id_to_copy
            // 
            this.txtAnno_class_id_to_copy.Location = new System.Drawing.Point(12, 26);
            this.txtAnno_class_id_to_copy.Name = "txtAnno_class_id_to_copy";
            this.txtAnno_class_id_to_copy.Size = new System.Drawing.Size(47, 20);
            this.txtAnno_class_id_to_copy.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ANNOTATIONCLASSID to be copied";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "New ANNOTATIONCLASSID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "New SYMBOLID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Table Name for Annotation";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Table Name for Conductor";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(170, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Alias Name for Anno Feature Class";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 249);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(194, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Alias Name for Conductor Feature Class";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 288);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Attribute Name for Conductor Labeltext";
            // 
            // txtAnno_class_id
            // 
            this.txtAnno_class_id.Location = new System.Drawing.Point(12, 65);
            this.txtAnno_class_id.Name = "txtAnno_class_id";
            this.txtAnno_class_id.Size = new System.Drawing.Size(47, 20);
            this.txtAnno_class_id.TabIndex = 12;
            // 
            // txtAnno_symbol_id
            // 
            this.txtAnno_symbol_id.Location = new System.Drawing.Point(12, 108);
            this.txtAnno_symbol_id.Name = "txtAnno_symbol_id";
            this.txtAnno_symbol_id.Size = new System.Drawing.Size(47, 20);
            this.txtAnno_symbol_id.TabIndex = 13;
            // 
            // txtAnno_cursor_table_name
            // 
            this.txtAnno_cursor_table_name.Location = new System.Drawing.Point(12, 147);
            this.txtAnno_cursor_table_name.Name = "txtAnno_cursor_table_name";
            this.txtAnno_cursor_table_name.Size = new System.Drawing.Size(218, 20);
            this.txtAnno_cursor_table_name.TabIndex = 14;
            // 
            // txtConductor_feature_cursor_table_name
            // 
            this.txtConductor_feature_cursor_table_name.Location = new System.Drawing.Point(12, 187);
            this.txtConductor_feature_cursor_table_name.Name = "txtConductor_feature_cursor_table_name";
            this.txtConductor_feature_cursor_table_name.Size = new System.Drawing.Size(218, 20);
            this.txtConductor_feature_cursor_table_name.TabIndex = 15;
            // 
            // txtAnno_feature_class_alias
            // 
            this.txtAnno_feature_class_alias.Location = new System.Drawing.Point(12, 226);
            this.txtAnno_feature_class_alias.Name = "txtAnno_feature_class_alias";
            this.txtAnno_feature_class_alias.Size = new System.Drawing.Size(218, 20);
            this.txtAnno_feature_class_alias.TabIndex = 16;
            // 
            // txtConductor_feature_class_alias
            // 
            this.txtConductor_feature_class_alias.Location = new System.Drawing.Point(12, 265);
            this.txtConductor_feature_class_alias.Name = "txtConductor_feature_class_alias";
            this.txtConductor_feature_class_alias.Size = new System.Drawing.Size(218, 20);
            this.txtConductor_feature_class_alias.TabIndex = 17;
            // 
            // txtConductor_labeltext_attrib
            // 
            this.txtConductor_labeltext_attrib.Location = new System.Drawing.Point(12, 304);
            this.txtConductor_labeltext_attrib.Name = "txtConductor_labeltext_attrib";
            this.txtConductor_labeltext_attrib.Size = new System.Drawing.Size(218, 20);
            this.txtConductor_labeltext_attrib.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(311, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(352, 389);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // btnSecUGConduit
            // 
            this.btnSecUGConduit.Location = new System.Drawing.Point(7, 343);
            this.btnSecUGConduit.Name = "btnSecUGConduit";
            this.btnSecUGConduit.Size = new System.Drawing.Size(134, 22);
            this.btnSecUGConduit.TabIndex = 20;
            this.btnSecUGConduit.Text = "Secondary UG Conduit";
            this.btnSecUGConduit.UseVisualStyleBackColor = true;
            this.btnSecUGConduit.Click += new System.EventHandler(this.btnSecUGConduit_Click);
            // 
            // btnPriUGConduit
            // 
            this.btnPriUGConduit.Location = new System.Drawing.Point(158, 343);
            this.btnPriUGConduit.Name = "btnPriUGConduit";
            this.btnPriUGConduit.Size = new System.Drawing.Size(134, 22);
            this.btnPriUGConduit.TabIndex = 21;
            this.btnPriUGConduit.Text = "Primary UG Conduit";
            this.btnPriUGConduit.UseVisualStyleBackColor = true;
            this.btnPriUGConduit.Click += new System.EventHandler(this.btnPriUGConduit_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(205, 378);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(75, 24);
            this.btnVerify.TabIndex = 22;
            this.btnVerify.Text = "Verify";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // frmCreateConduitAnno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 407);
            this.Controls.Add(this.btnVerify);
            this.Controls.Add(this.btnPriUGConduit);
            this.Controls.Add(this.btnSecUGConduit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtConductor_labeltext_attrib);
            this.Controls.Add(this.txtConductor_feature_class_alias);
            this.Controls.Add(this.txtAnno_feature_class_alias);
            this.Controls.Add(this.txtConductor_feature_cursor_table_name);
            this.Controls.Add(this.txtAnno_cursor_table_name);
            this.Controls.Add(this.txtAnno_symbol_id);
            this.Controls.Add(this.txtAnno_class_id);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAnno_class_id_to_copy);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Name = "frmCreateConduitAnno";
            this.Text = "Create Conduit Anno";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtAnno_class_id_to_copy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox txtAnno_class_id;
        public System.Windows.Forms.TextBox txtAnno_symbol_id;
        public System.Windows.Forms.TextBox txtAnno_cursor_table_name;
        public System.Windows.Forms.TextBox txtConductor_feature_cursor_table_name;
        public System.Windows.Forms.TextBox txtAnno_feature_class_alias;
        public System.Windows.Forms.TextBox txtConductor_feature_class_alias;
        public System.Windows.Forms.TextBox txtConductor_labeltext_attrib;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSecUGConduit;
        private System.Windows.Forms.Button btnPriUGConduit;
        public System.Windows.Forms.Button btnRun;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnVerify;
    }
}