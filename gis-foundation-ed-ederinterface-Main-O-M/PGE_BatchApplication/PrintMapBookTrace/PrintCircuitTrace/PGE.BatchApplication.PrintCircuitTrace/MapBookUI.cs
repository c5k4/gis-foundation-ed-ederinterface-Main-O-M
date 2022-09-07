using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace PGE.BatchApplication.PrintCircuitTrace
{
    public partial class MapBookUI : Form
    {
        DataTable dataTable = new DataTable();
        public MapBookUI()
        {
            InitializeComponent();

            try
            {
                IEnumerable<string[]> csvRows = File.ReadAllLines(@"C:\Work\EDAMGIS\Source_Development\IBM GIS\Standalone Executables\MapProductionAutomation\MapBooksPrintUI\bin\Debug\Result.csv").Select(a => a.Split(';'));
                dataTable.Columns.Add("", typeof(string));
                dataTable.Columns.Add("", typeof(string));
                dataTable.Columns.Add("", typeof(string));
               

                List<DataGridViewRow> row = new List<DataGridViewRow>();
                dataGridView1.Columns.Add("TEST", "TEST");
                dataGridView1.Columns.Add("TEST", "TEST");
                dataGridView1.Columns.Add("TEST", "TEST");
                foreach (var item in csvRows)
                {
                    string[] splitValue = Regex.Split(item[0].ToString(), @",");
                    dataTable.Rows.Add(splitValue);
                    row.Add(new DataGridViewRow());
                    row[row.Count - 1].CreateCells(dataGridView1, splitValue);
                }
               
                dataGridView1.Rows.AddRange(row.ToArray());
                dataGridView1.Update();
                //dataGridView1.DataSource = csv;
            }
            catch (Exception ex)
            {
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int index = ((System.Windows.Forms.DataGridView)(sender)).CurrentRow.Index;
                comboBox1.Items.Add(dataTable.Rows[index][2]);
                //comboBox1.Items.AddRange();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
