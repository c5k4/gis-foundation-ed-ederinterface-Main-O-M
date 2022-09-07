namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync
{
    partial class PropChooser
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
            this.aliases = new System.Windows.Forms.CheckBox();
            this.order = new System.Windows.Forms.CheckBox();
            this.visibility = new System.Windows.Forms.CheckBox();
            this.fields = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // aliases
            // 
            this.aliases.AutoSize = true;
            this.aliases.Location = new System.Drawing.Point(6, 19);
            this.aliases.Name = "aliases";
            this.aliases.Size = new System.Drawing.Size(59, 17);
            this.aliases.TabIndex = 0;
            this.aliases.Text = "Aliases";
            this.aliases.UseVisualStyleBackColor = true;
            // 
            // order
            // 
            this.order.AutoSize = true;
            this.order.Location = new System.Drawing.Point(6, 42);
            this.order.Name = "order";
            this.order.Size = new System.Drawing.Size(90, 17);
            this.order.TabIndex = 1;
            this.order.Text = "Order/Aliases";
            this.order.UseVisualStyleBackColor = true;
            this.order.CheckedChanged += new System.EventHandler(this.order_CheckedChanged);
            // 
            // visibility
            // 
            this.visibility.AutoSize = true;
            this.visibility.Location = new System.Drawing.Point(6, 19);
            this.visibility.Name = "visibility";
            this.visibility.Size = new System.Drawing.Size(62, 17);
            this.visibility.TabIndex = 2;
            this.visibility.Text = "Visibility";
            this.visibility.UseVisualStyleBackColor = true;
            // 
            // fields
            // 
            this.fields.Location = new System.Drawing.Point(133, 54);
            this.fields.Multiline = true;
            this.fields.Name = "fields";
            this.fields.Size = new System.Drawing.Size(200, 154);
            this.fields.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.aliases);
            this.groupBox1.Controls.Add(this.order);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(109, 69);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simple Properties";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.visibility);
            this.groupBox2.Location = new System.Drawing.Point(127, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(212, 202);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Complex Properties";
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(257, 231);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 6;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(176, 231);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // PropChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 266);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.fields);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "PropChooser";
            this.Text = "Property Chooser";
            this.Load += new System.EventHandler(this.PropChooser_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox aliases;
        private System.Windows.Forms.CheckBox order;
        private System.Windows.Forms.CheckBox visibility;
        private System.Windows.Forms.TextBox fields;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;

    }
}