namespace Telvent.PGE.ED.Desktop.Forms
{
    partial class PTTDockableWindow
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
            this.tabActions = new System.Windows.Forms.TabControl();
            this.tabInsert = new System.Windows.Forms.TabPage();
            this.lblPromotesCount = new System.Windows.Forms.Label();
            this.prgPromotes = new System.Windows.Forms.ProgressBar();
            this.btnPromotes = new System.Windows.Forms.Button();
            this.lblPromote = new System.Windows.Forms.Label();
            this.chkPromotes = new System.Windows.Forms.CheckedListBox();
            this.tabDelete = new System.Windows.Forms.TabPage();
            this.lblDeletesCount = new System.Windows.Forms.Label();
            this.prgDeletes = new System.Windows.Forms.ProgressBar();
            this.btnDeletePoles = new System.Windows.Forms.Button();
            this.lblDeletes = new System.Windows.Forms.Label();
            this.chkDeletes = new System.Windows.Forms.CheckedListBox();
            this.tabCombine = new System.Windows.Forms.TabPage();
            this.prgCombine = new System.Windows.Forms.ProgressBar();
            this.btnCombine = new System.Windows.Forms.Button();
            this.lblCombineCount = new System.Windows.Forms.Label();
            this.chkCombine = new System.Windows.Forms.CheckedListBox();
            this.prgCombines = new System.Windows.Forms.ProgressBar();
            this.btnCombinePoles = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tabActions.SuspendLayout();
            this.tabInsert.SuspendLayout();
            this.tabDelete.SuspendLayout();
            this.tabCombine.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabActions
            // 
            this.tabActions.Controls.Add(this.tabInsert);
            this.tabActions.Controls.Add(this.tabDelete);
            this.tabActions.Controls.Add(this.tabCombine);
            this.tabActions.Location = new System.Drawing.Point(3, 0);
            this.tabActions.Name = "tabActions";
            this.tabActions.SelectedIndex = 0;
            this.tabActions.Size = new System.Drawing.Size(331, 504);
            this.tabActions.TabIndex = 2;
            // 
            // tabInsert
            // 
            this.tabInsert.AutoScroll = true;
            this.tabInsert.BackColor = System.Drawing.SystemColors.Control;
            this.tabInsert.Controls.Add(this.lblPromotesCount);
            this.tabInsert.Controls.Add(this.prgPromotes);
            this.tabInsert.Controls.Add(this.btnPromotes);
            this.tabInsert.Controls.Add(this.lblPromote);
            this.tabInsert.Controls.Add(this.chkPromotes);
            this.tabInsert.Location = new System.Drawing.Point(4, 22);
            this.tabInsert.Name = "tabInsert";
            this.tabInsert.Padding = new System.Windows.Forms.Padding(3);
            this.tabInsert.Size = new System.Drawing.Size(323, 478);
            this.tabInsert.TabIndex = 0;
            this.tabInsert.Text = "To Promote";
            // 
            // lblPromotesCount
            // 
            this.lblPromotesCount.AutoSize = true;
            this.lblPromotesCount.Location = new System.Drawing.Point(6, 410);
            this.lblPromotesCount.Name = "lblPromotesCount";
            this.lblPromotesCount.Size = new System.Drawing.Size(156, 13);
            this.lblPromotesCount.TabIndex = 4;
            this.lblPromotesCount.Text = "0 Poles to Promote (0 Selected)";
            // 
            // prgPromotes
            // 
            this.prgPromotes.Location = new System.Drawing.Point(6, 426);
            this.prgPromotes.Name = "prgPromotes";
            this.prgPromotes.Size = new System.Drawing.Size(311, 19);
            this.prgPromotes.TabIndex = 3;
            // 
            // btnPromotes
            // 
            this.btnPromotes.Location = new System.Drawing.Point(6, 451);
            this.btnPromotes.Name = "btnPromotes";
            this.btnPromotes.Size = new System.Drawing.Size(311, 21);
            this.btnPromotes.TabIndex = 2;
            this.btnPromotes.Text = "Promote Selected Poles";
            this.btnPromotes.UseVisualStyleBackColor = true;
            this.btnPromotes.Click += new System.EventHandler(this.btnPromotes_Click);
            // 
            // lblPromote
            // 
            this.lblPromote.Location = new System.Drawing.Point(6, 3);
            this.lblPromote.Name = "lblPromote";
            this.lblPromote.Size = new System.Drawing.Size(311, 65);
            this.lblPromote.TabIndex = 1;
            this.lblPromote.Text = "The following poles are available to promote from the PTT staging table to the Su" +
    "pport Structure table";
            this.lblPromote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkPromotes
            // 
            this.chkPromotes.CheckOnClick = true;
            this.chkPromotes.FormattingEnabled = true;
            this.chkPromotes.Location = new System.Drawing.Point(6, 71);
            this.chkPromotes.Name = "chkPromotes";
            this.chkPromotes.Size = new System.Drawing.Size(311, 334);
            this.chkPromotes.TabIndex = 0;
            this.chkPromotes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkPromotes_ItemCheck);
            // 
            // tabDelete
            // 
            this.tabDelete.BackColor = System.Drawing.SystemColors.Control;
            this.tabDelete.Controls.Add(this.lblDeletesCount);
            this.tabDelete.Controls.Add(this.prgDeletes);
            this.tabDelete.Controls.Add(this.btnDeletePoles);
            this.tabDelete.Controls.Add(this.lblDeletes);
            this.tabDelete.Controls.Add(this.chkDeletes);
            this.tabDelete.Location = new System.Drawing.Point(4, 22);
            this.tabDelete.Name = "tabDelete";
            this.tabDelete.Padding = new System.Windows.Forms.Padding(3);
            this.tabDelete.Size = new System.Drawing.Size(323, 478);
            this.tabDelete.TabIndex = 1;
            this.tabDelete.Text = "To Delete";
            // 
            // lblDeletesCount
            // 
            this.lblDeletesCount.AutoSize = true;
            this.lblDeletesCount.Location = new System.Drawing.Point(6, 410);
            this.lblDeletesCount.Name = "lblDeletesCount";
            this.lblDeletesCount.Size = new System.Drawing.Size(148, 13);
            this.lblDeletesCount.TabIndex = 8;
            this.lblDeletesCount.Text = "0 Poles to Delete (0 Selected)";
            // 
            // prgDeletes
            // 
            this.prgDeletes.Location = new System.Drawing.Point(6, 426);
            this.prgDeletes.Name = "prgDeletes";
            this.prgDeletes.Size = new System.Drawing.Size(311, 19);
            this.prgDeletes.TabIndex = 7;
            // 
            // btnDeletePoles
            // 
            this.btnDeletePoles.Location = new System.Drawing.Point(6, 451);
            this.btnDeletePoles.Name = "btnDeletePoles";
            this.btnDeletePoles.Size = new System.Drawing.Size(311, 21);
            this.btnDeletePoles.TabIndex = 6;
            this.btnDeletePoles.Text = "Delete Selected Poles";
            this.btnDeletePoles.UseVisualStyleBackColor = true;
            this.btnDeletePoles.Click += new System.EventHandler(this.btnDeletePoles_Click);
            // 
            // lblDeletes
            // 
            this.lblDeletes.Location = new System.Drawing.Point(6, 5);
            this.lblDeletes.Name = "lblDeletes";
            this.lblDeletes.Size = new System.Drawing.Size(311, 65);
            this.lblDeletes.TabIndex = 5;
            this.lblDeletes.Text = "The following poles were identified to have the PTTDIDC set to Yes and are availa" +
    "ble to delete from the Support Structure table";
            this.lblDeletes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkDeletes
            // 
            this.chkDeletes.CheckOnClick = true;
            this.chkDeletes.FormattingEnabled = true;
            this.chkDeletes.Location = new System.Drawing.Point(6, 73);
            this.chkDeletes.Name = "chkDeletes";
            this.chkDeletes.Size = new System.Drawing.Size(311, 334);
            this.chkDeletes.TabIndex = 4;
            this.chkDeletes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkDeletes_ItemCheck);
            // 
            // tabCombine
            // 
            this.tabCombine.BackColor = System.Drawing.SystemColors.Control;
            this.tabCombine.Controls.Add(this.prgCombine);
            this.tabCombine.Controls.Add(this.btnCombine);
            this.tabCombine.Controls.Add(this.lblCombineCount);
            this.tabCombine.Controls.Add(this.chkCombine);
            this.tabCombine.Controls.Add(this.prgCombines);
            this.tabCombine.Controls.Add(this.btnCombinePoles);
            this.tabCombine.Controls.Add(this.label1);
            this.tabCombine.Location = new System.Drawing.Point(4, 22);
            this.tabCombine.Name = "tabCombine";
            this.tabCombine.Size = new System.Drawing.Size(323, 478);
            this.tabCombine.TabIndex = 2;
            this.tabCombine.Text = "To Combine";
            // 
            // prgCombine
            // 
            this.prgCombine.Location = new System.Drawing.Point(6, 429);
            this.prgCombine.Name = "prgCombine";
            this.prgCombine.Size = new System.Drawing.Size(311, 19);
            this.prgCombine.TabIndex = 15;
            // 
            // btnCombine
            // 
            this.btnCombine.Location = new System.Drawing.Point(6, 454);
            this.btnCombine.Name = "btnCombine";
            this.btnCombine.Size = new System.Drawing.Size(311, 21);
            this.btnCombine.TabIndex = 14;
            this.btnCombine.Text = "Combine Selected Poles";
            this.btnCombine.UseVisualStyleBackColor = true;
            this.btnCombine.Click += new System.EventHandler(this.btnCombine_Click);
            // 
            // lblCombineCount
            // 
            this.lblCombineCount.AutoSize = true;
            this.lblCombineCount.Location = new System.Drawing.Point(6, 409);
            this.lblCombineCount.Name = "lblCombineCount";
            this.lblCombineCount.Size = new System.Drawing.Size(158, 13);
            this.lblCombineCount.TabIndex = 13;
            this.lblCombineCount.Text = "0 Poles to Combine (0 Selected)";
            // 
            // chkCombine
            // 
            this.chkCombine.CheckOnClick = true;
            this.chkCombine.FormattingEnabled = true;
            this.chkCombine.Location = new System.Drawing.Point(6, 72);
            this.chkCombine.Name = "chkCombine";
            this.chkCombine.Size = new System.Drawing.Size(311, 334);
            this.chkCombine.TabIndex = 12;
            this.chkCombine.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkCombine_ItemCheck);
            // 
            // prgCombines
            // 
            this.prgCombines.Location = new System.Drawing.Point(6, 672);
            this.prgCombines.Name = "prgCombines";
            this.prgCombines.Size = new System.Drawing.Size(311, 19);
            this.prgCombines.TabIndex = 11;
            // 
            // btnCombinePoles
            // 
            this.btnCombinePoles.Location = new System.Drawing.Point(6, 697);
            this.btnCombinePoles.Name = "btnCombinePoles";
            this.btnCombinePoles.Size = new System.Drawing.Size(311, 21);
            this.btnCombinePoles.TabIndex = 10;
            this.btnCombinePoles.Text = "Promote Selected Poles";
            this.btnCombinePoles.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(311, 65);
            this.label1.TabIndex = 9;
            this.label1.Text = "The following poles were identified in both the PTT Staging table as well as the " +
    "Support Structure table and must have attributes combined";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(3, 510);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(331, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // PTTDockableWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.tabActions);
            this.Name = "PTTDockableWindow";
            this.Size = new System.Drawing.Size(340, 750);
            this.tabActions.ResumeLayout(false);
            this.tabInsert.ResumeLayout(false);
            this.tabInsert.PerformLayout();
            this.tabDelete.ResumeLayout(false);
            this.tabDelete.PerformLayout();
            this.tabCombine.ResumeLayout(false);
            this.tabCombine.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabActions;
        private System.Windows.Forms.TabPage tabInsert;
        private System.Windows.Forms.Label lblPromote;
        private System.Windows.Forms.CheckedListBox chkPromotes;
        private System.Windows.Forms.TabPage tabDelete;
        private System.Windows.Forms.TabPage tabCombine;
        private System.Windows.Forms.ProgressBar prgPromotes;
        private System.Windows.Forms.Button btnPromotes;
        private System.Windows.Forms.ProgressBar prgDeletes;
        private System.Windows.Forms.Button btnDeletePoles;
        private System.Windows.Forms.Label lblDeletes;
        private System.Windows.Forms.CheckedListBox chkDeletes;
        private System.Windows.Forms.ProgressBar prgCombines;
        private System.Windows.Forms.Button btnCombinePoles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblPromotesCount;
        private System.Windows.Forms.Label lblDeletesCount;
        private System.Windows.Forms.CheckedListBox chkCombine;
        private System.Windows.Forms.Label lblCombineCount;
        private System.Windows.Forms.ProgressBar prgCombine;
        private System.Windows.Forms.Button btnCombine;

    }
}
