using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PGE.BatchApplication.ROBCService;
using System.ComponentModel;

namespace PGE.BatchApplication.ROBCApp
{
    /// <summary>
    /// Interaction logic for PCPFinder.xaml
    /// </summary>
    public partial class PCPFinder : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public PCPFinder()
        {
            InitializeComponent();
        }

        private void btnSearchPCP_Click(object sender, RoutedEventArgs e)
        {
            string circuitID = !string.IsNullOrEmpty(txtCircuitId.Text)?txtCircuitId.Text.Trim():string.Empty;
            string operatingNo = !string.IsNullOrEmpty(txtOperatingNo.Text)?txtOperatingNo.Text.Trim():string.Empty;
            if (string.IsNullOrEmpty(circuitID) || string.IsNullOrEmpty(operatingNo))
            {
                MessageBox.Show("Circuit Id and Operating No are required!", "Error");
            }
            else
            {
                int totalItems = 0;
                Dictionary<string, string> gridParams = new Dictionary<string, string>() { { "Start", "0" }, { "ItemsPerPage", "10" }, { "SortColumn", "DeviceType" }, { "Ascending", "true" } };
                System.Collections.ObjectModel.ObservableCollection<PCPModel> devices = PCPManagementService.GetPcpDevices(circuitID, operatingNo, gridParams, out totalItems);
                if (devices == null || !devices.Any())
                {
                    MessageBox.Show("No devices found! Please make sure circuit id and operating number are valid!", "Devices Search");
                }

                else if (devices != null && totalItems > 1)
                {
                    PCPSearchResult pcpSearchResult = new PCPSearchResult(devices);
                    this.NavigationService.Navigate(pcpSearchResult);
                }
                else if (devices != null && totalItems == 1)
                {
                    ManagePCP managePCP = new ManagePCP(devices.First());
                    this.NavigationService.Navigate(managePCP);
                }
            }
        }

        private void btnClearPCP_Click(object sender, RoutedEventArgs e)
        {
            this.txtCircuitId.Text = string.Empty;
            this.txtOperatingNo.Text = string.Empty;
        }
    }
}
