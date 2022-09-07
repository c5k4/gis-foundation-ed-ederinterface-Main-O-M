namespace PGE.Desktop.EDER.FieldEditors
{
    partial class PGE_LocDesc_FieldEditor
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
            this.LocDesc_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LocDesc_textBox
            // 
            this.LocDesc_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.LocDesc_textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LocDesc_textBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.LocDesc_textBox.Location = new System.Drawing.Point(3, 3);
            this.LocDesc_textBox.Name = "LocDesc_textBox";
            this.LocDesc_textBox.Size = new System.Drawing.Size(209, 13);
            this.LocDesc_textBox.TabIndex = 0;
            this.LocDesc_textBox.TextChanged += new System.EventHandler(this.LocDesc_textBox_TextChanged);
            // 
            // PGE_LocDesc_FieldEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.LocDesc_textBox);
            this.Name = "PGE_LocDesc_FieldEditor";
            this.Size = new System.Drawing.Size(215, 19);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LocDesc_textBox;
    }
}
