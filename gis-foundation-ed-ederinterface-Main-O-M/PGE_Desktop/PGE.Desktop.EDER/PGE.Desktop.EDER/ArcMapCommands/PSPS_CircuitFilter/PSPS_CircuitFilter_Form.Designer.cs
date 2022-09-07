namespace PGE.Desktop.EDER.ArcMapCommands.PSPS_CircuitFilter
{
    partial class PSPS_CircuitFilter_Form
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
            this.CircuitIDLabel = new System.Windows.Forms.Label();
            this.PSPSLabel = new System.Windows.Forms.Label();
            this.CircuitID_TextBox = new System.Windows.Forms.TextBox();
            this.Segment_ComboBox = new System.Windows.Forms.ComboBox();
            this.ResetAll_Btn = new System.Windows.Forms.Button();
            this.Close_Btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 99); 
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";            

            // 
            // CircuitIDLabel
            // 
            this.CircuitIDLabel.AutoSize = true;
            this.CircuitIDLabel.Location = new System.Drawing.Point(13, 13);
            this.CircuitIDLabel.Name = "CircuitIDLabel";
            this.CircuitIDLabel.Size = new System.Drawing.Size(102, 13);
            this.CircuitIDLabel.TabIndex = 0;
            this.CircuitIDLabel.Text = "Circuit ID/Circuit Name";
            // 
            // PSPSLabel
            // 
            this.PSPSLabel.AutoSize = true;
            this.PSPSLabel.Location = new System.Drawing.Point(13, 39);
            this.PSPSLabel.Name = "PSPSLabel";
            this.PSPSLabel.Size = new System.Drawing.Size(80, 13);
            this.PSPSLabel.TabIndex = 1;
            this.PSPSLabel.Text = "PSPS Segment";
            // 
            // CircuitID_TextBox
            // 
            this.CircuitID_TextBox.Location = new System.Drawing.Point(139, 10);
            this.CircuitID_TextBox.Name = "CircuitID_TextBox";
            this.CircuitID_TextBox.Size = new System.Drawing.Size(238, 20);
            this.CircuitID_TextBox.TabIndex = 2;
            this.CircuitID_TextBox.TextChanged += new System.EventHandler(this.CircuitID_TextBox_TextChanged);
            // 
            // Segment_ComboBox
            // 
            this.Segment_ComboBox.FormattingEnabled = true;
            this.Segment_ComboBox.Location = new System.Drawing.Point(139, 39);
            this.Segment_ComboBox.Name = "Segment_ComboBox";
            this.Segment_ComboBox.Size = new System.Drawing.Size(238, 21);
            this.Segment_ComboBox.TabIndex = 3;
            this.Segment_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Segment_ComboBox_SelectedIndexChanged);
            // 
            // ResetAll_Btn
            // 
            this.ResetAll_Btn.Location = new System.Drawing.Point(158, 74);
            this.ResetAll_Btn.Name = "ResetAll_Btn";
            this.ResetAll_Btn.Size = new System.Drawing.Size(88, 23);
            this.ResetAll_Btn.TabIndex = 5;
            this.ResetAll_Btn.Text = "Reset";
            this.ResetAll_Btn.UseVisualStyleBackColor = true;
            this.ResetAll_Btn.Click += new System.EventHandler(this.ResetAll_Btn_Click);
            // 
            // Close_Btn
            // 
            this.Close_Btn.Location = new System.Drawing.Point(252, 74);
            this.Close_Btn.Name = "Close_Btn";
            this.Close_Btn.Size = new System.Drawing.Size(88, 23);
            this.Close_Btn.TabIndex = 6;
            this.Close_Btn.Text = "Close";
            this.Close_Btn.UseVisualStyleBackColor = true;
            this.Close_Btn.Click += new System.EventHandler(this.Close_Btn_Click);
            // 
            // PSPS_CircuitFilter_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 117);
            this.Controls.Add(this.Close_Btn);
            this.Controls.Add(this.ResetAll_Btn);
            this.Controls.Add(this.Segment_ComboBox);
            this.Controls.Add(this.CircuitID_TextBox);
            this.Controls.Add(this.PSPSLabel);
            this.Controls.Add(this.CircuitIDLabel);            
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PSPS_CircuitFilter_Form";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PSPS Circuit Filter Tool";
            this.Load += new System.EventHandler(this.PSPS_CircuitFilter_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CircuitIDLabel;
        private System.Windows.Forms.Label PSPSLabel;
        private System.Windows.Forms.TextBox CircuitID_TextBox;
        private System.Windows.Forms.ComboBox Segment_ComboBox;
        private System.Windows.Forms.Button ResetAll_Btn;
        private System.Windows.Forms.Button Close_Btn;
    }
}