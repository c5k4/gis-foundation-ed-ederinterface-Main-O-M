namespace PGE.Desktop.EDER.FieldEditors
{
    partial class CircuitIDSelector
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
            this.btnOpenPicker = new System.Windows.Forms.Button();
            this.txtCircuitID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenPicker
            // 
            this.btnOpenPicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenPicker.Location = new System.Drawing.Point(142, 0);
            this.btnOpenPicker.Name = "btnOpenPicker";
            this.btnOpenPicker.Size = new System.Drawing.Size(26, 19);
            this.btnOpenPicker.TabIndex = 1;
            this.btnOpenPicker.Text = "...";
            this.btnOpenPicker.UseVisualStyleBackColor = true;
            this.btnOpenPicker.Click += new System.EventHandler(this.btnOpenPicker_Click);
            // 
            // txtCircuitID
            // 
            this.txtCircuitID.Location = new System.Drawing.Point(0, 0);
            this.txtCircuitID.Name = "txtCircuitID";
            this.txtCircuitID.Size = new System.Drawing.Size(168, 17);
            this.txtCircuitID.TabIndex = 2;
            this.txtCircuitID.Text = "label1";
            this.txtCircuitID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtCircuitID.Click += new System.EventHandler(this.txtCircuitID_Click);
            // 
            // CircuitIDSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOpenPicker);
            this.Controls.Add(this.txtCircuitID);
            this.Name = "CircuitIDSelector";
            this.Size = new System.Drawing.Size(168, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenPicker;
        private System.Windows.Forms.Label txtCircuitID;


    }
}
