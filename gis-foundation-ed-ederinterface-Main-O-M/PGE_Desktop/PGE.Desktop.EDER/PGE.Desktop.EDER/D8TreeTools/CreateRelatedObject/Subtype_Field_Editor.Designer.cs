using Miner.Interop;
using System.Windows.Forms;
namespace PGE.Desktop.EDER.D8TreeTools.CreateRelatedObject
{
    partial class Subtype_Field_Editor
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
            this.cbo_Subtype = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbo_Subtype
            // 
            this.cbo_Subtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo_Subtype.FormattingEnabled = true;
            this.cbo_Subtype.Location = new System.Drawing.Point(0, 0);
            this.cbo_Subtype.Name = "cbo_Subtype";
            this.cbo_Subtype.Size = new System.Drawing.Size(289, 21);
            this.cbo_Subtype.TabIndex = 0;
            this.cbo_Subtype.SelectedIndexChanged += new System.EventHandler(this.cbo_Subtype_SelectedIndexChanged_1);
            this.cbo_Subtype.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbo_Subtype_KeyDown);
            // 
            // Subtype_Field_Editor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.cbo_Subtype);
            this.Name = "Subtype_Field_Editor";
            this.Size = new System.Drawing.Size(289, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbo_Subtype;

    }
}
