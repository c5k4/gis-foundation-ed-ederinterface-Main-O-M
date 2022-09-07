using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.Subtasks
{
    public partial class SAPErrorsUI : Form
    {
        string[] _errorList = null;
        public SAPErrorsUI(string[] errorList)
        {
            _errorList = errorList;
            InitializeComponent();
        }

        private void SAPErrorsUI_Load(object sender, EventArgs e)
        {
            if (_errorList == null || _errorList.Length < 1) return;
            lstErrors.BeginUpdate();
            lstErrors.Items.AddRange(_errorList);
            lstErrors.EndUpdate();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //Validate the error list in the listbox
            if (lstErrors.Items.Count < 1)
            {
                lblStatus.Text = "No errors found to export."; 
                lblStatus.Update(); return;
            }

            //Dispay SaveAs dialog
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.Filter = "Text Files (*.txt)|*.txt";
            fileDialog.Title = "Save As";
            fileDialog.OverwritePrompt = true;
            fileDialog.FilterIndex = 0;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = fileDialog.FileName;
                if (string.IsNullOrEmpty(fileName)) return;
                Export(_errorList, fileName);
                lblStatus.Text = "Successfully exported."; lblStatus.Update();
            }
        }

        private void Export(string[] messages, string fileName)
        {
            if (messages == null || messages.Length < 1) return;
            FileStream stream = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }
            StreamWriter writer = new StreamWriter(stream);
            foreach (string message in messages)
            {
                writer.WriteLine(message);
            }
            writer.Flush();
            writer.Close();
        }
    }
}
