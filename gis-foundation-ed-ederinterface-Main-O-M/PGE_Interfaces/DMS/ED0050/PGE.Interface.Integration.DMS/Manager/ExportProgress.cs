using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace PGE.Interface.Integration.DMS.Manager
{
    public partial class ExportProgress : Form
    {
        public ExportProgress()
        {
            InitializeComponent();
        }

        public void UpdateProgress(Dictionary<string, List<string>> processIDs, ControlTable controlTable, string batchID)
        {
            // #11
            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string statusLogDirectory = location.Substring(0, location.LastIndexOf("\\") + 1) + "StatusLogs";
            int TotalElectricCircuits = 0;
            double ElectricExportProgress = 0.0;
            double totalProgress = 0.0;
            int ElectricCircuitsExported = 0;

            try
            {
                ElectricCircuitsExported = controlTable.CircuitCountByStatus(CircuitStatus.Finished, false, false);
                ElectricCircuitsExported += controlTable.CircuitCountByStatus(CircuitStatus.Finished, true, false);
                TotalElectricCircuits = controlTable.CircuitCount();
                totalProgress = ((((double)ElectricCircuitsExported)) / (((double)TotalElectricCircuits))) * 100.0;

                if (ElectricExportProgress > 100.0) { ElectricExportProgress = 100.0; }

                lblProcessed.Text = ElectricCircuitsExported.ToString();
                lblToProcess.Text = TotalElectricCircuits.ToString();
                lblOverallProgress.Text = (Math.Floor(totalProgress)).ToString();
                progBarOverall.Value = Int32.Parse(Math.Floor(totalProgress).ToString());
                lblOverallProgress.Text = Math.Round(totalProgress, 2) + "%";
            }
            catch (Exception e)
            {

            }
        }
    }
}
