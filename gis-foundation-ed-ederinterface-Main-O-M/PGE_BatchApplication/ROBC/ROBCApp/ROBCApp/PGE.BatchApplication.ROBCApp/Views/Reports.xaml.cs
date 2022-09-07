using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PGE.BatchApplication.ROBCService.Common;
using System.IO;

namespace PGE.BatchApplication.ROBCApp
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Page
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void BtnEEPReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = Constants.EepReportFilePath;
                myProcess.Start();
                */
                Process.Start("Excel", Constants.EepReportFilePath);
            }
            catch (Exception ex)
            {
                try
                {
                    Process.Start(Constants.EepReportFilePath);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(string.Format("Following error has occurred while opening the EEP report.\n{0}", exc.Message));
                }
            }
        }

        private void BtnECTPReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Excel", Constants.EctpReportFilePath);
            }
            catch (Exception ex)
            {
                try
                {
                    Process.Start(Constants.EctpReportFilePath);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(string.Format("Following error has occurred while opening the ECTP report.\n{0}", exc.Message));
                }
            }
        }
    }
}
