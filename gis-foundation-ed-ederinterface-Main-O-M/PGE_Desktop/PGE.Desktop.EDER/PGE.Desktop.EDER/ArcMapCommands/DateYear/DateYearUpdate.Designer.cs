namespace PGE.Desktop.EDER.ArcMapCommands.DateYear
{
    partial class DateYearUpdate
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.cYear = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bClose = new System.Windows.Forms.Button();
            this.bClear = new System.Windows.Forms.Button();
            this.cApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(128, 17);
            this.dateTimePicker1.MaxDate = new System.DateTime(9998, 11, 23, 0, 0, 0, 0);
            this.dateTimePicker1.MinDate = new System.DateTime(1753, 12, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(115, 20);
            this.dateTimePicker1.TabIndex = 20;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // cYear
            // 
            this.cYear.Location = new System.Drawing.Point(128, 60);
            this.cYear.MaxLength = 4;
            this.cYear.Name = "cYear";
            this.cYear.Size = new System.Drawing.Size(115, 20);
            this.cYear.TabIndex = 21;
            this.cYear.TextChanged += new System.EventHandler(this.cYear_TextChanged);
            this.cYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cYear_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 15);
            this.label2.TabIndex = 18;
            this.label2.Text = "Year Installed:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Date Installed:";
            // 
            // bClose
            // 
            this.bClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bClose.Location = new System.Drawing.Point(175, 101);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(68, 23);
            this.bClose.TabIndex = 24;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bClear
            // 
            this.bClear.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bClear.Location = new System.Drawing.Point(97, 101);
            this.bClear.Name = "bClear";
            this.bClear.Size = new System.Drawing.Size(68, 23);
            this.bClear.TabIndex = 23;
            this.bClear.Text = "Clear";
            this.bClear.UseVisualStyleBackColor = true;
            this.bClear.Click += new System.EventHandler(this.bClear_Click);
            // 
            // cApply
            // 
            this.cApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cApply.Location = new System.Drawing.Point(18, 101);
            this.cApply.Name = "cApply";
            this.cApply.Size = new System.Drawing.Size(68, 23);
            this.cApply.TabIndex = 22;
            this.cApply.Text = "Apply";
            this.cApply.UseVisualStyleBackColor = true;
            this.cApply.Click += new System.EventHandler(this.cApply_Click);
            // 
            // DateYearUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 140);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.cYear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bClose);
            this.Controls.Add(this.bClear);
            this.Controls.Add(this.cApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateYearUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Date & Year Installed Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox cYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bClear;
        private System.Windows.Forms.Button cApply;

    }
}