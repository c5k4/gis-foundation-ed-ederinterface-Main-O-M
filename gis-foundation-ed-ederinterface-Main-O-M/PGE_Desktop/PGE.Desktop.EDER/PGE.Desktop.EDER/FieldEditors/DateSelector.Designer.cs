namespace PGE.Desktop.EDER.FieldEditors
{
    partial class DateSelector
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
            this.dateTimeSelection = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // dateTimeSelection
            // 
            this.dateTimeSelection.Checked = false;
            this.dateTimeSelection.CustomFormat = "MM/dd/yyyy";
            this.dateTimeSelection.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeSelection.Location = new System.Drawing.Point(0, 0);
            this.dateTimeSelection.MinDate = new System.DateTime(1801, 1, 1, 0, 0, 0, 0);
            this.dateTimeSelection.Name = "dateTimeSelection";
            this.dateTimeSelection.Size = new System.Drawing.Size(147, 20);
            this.dateTimeSelection.TabIndex = 0;
            this.dateTimeSelection.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            this.dateTimeSelection.Enter += new System.EventHandler(this.dateTimePicker1_Enter);
            this.dateTimeSelection.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dateTimePicker1_KeyDown);
            this.dateTimeSelection.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dateTimePicker1_KeyPress);
            this.dateTimeSelection.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dateTimePicker1_KeyUp);
            this.dateTimeSelection.MouseEnter += new System.EventHandler(this.dateTimePicker1_MouseEnter);
            // 
            // DateSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dateTimeSelection);
            this.Name = "DateSelector";
            this.Size = new System.Drawing.Size(150, 26);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeSelection;
    }
}
