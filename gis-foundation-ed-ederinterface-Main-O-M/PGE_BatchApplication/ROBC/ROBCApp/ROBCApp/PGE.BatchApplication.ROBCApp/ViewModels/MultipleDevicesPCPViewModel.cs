using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PGE.BatchApplication.ROBCService;

namespace PGE.BatchApplication.ROBCApp
{
    public class MultipleDevicesPCPViewModel : DataGridCommonViewModel
    {
        private const string PAGE_NAME = "DUPLICATE_DEVICES";
        private string CircuitId = string.Empty;
        private string OperatingNo = string.Empty;
        public MultipleDevicesPCPViewModel()
        {

        }
        public MultipleDevicesPCPViewModel(string CircuitId, string OperatingNo)
        {
            this.CircuitId = CircuitId;
            this.OperatingNo = OperatingNo;
        }
        public MultipleDevicesPCPViewModel(ObservableCollection<PCPModel> pcpDevicesList)
        {
            this.DuplicateDevicesForPCPList = pcpDevicesList;
        }
        private ObservableCollection<PCPModel> _duplicateDevicesForPCPList;
        public ObservableCollection<PCPModel> DuplicateDevicesForPCPList
        {
            get
            {
                return _duplicateDevicesForPCPList;
            }
            private set
            {
                if (object.ReferenceEquals(_duplicateDevicesForPCPList, value) != true)
                {
                    _duplicateDevicesForPCPList = value;
                    NotifyPropertyChanged("DuplicateDevicesForPCPList");
                }
            }
        }


        private PCPModel _selectedPCP;
        public PCPModel SelectedPCP
        {
            get
            {
                return _selectedPCP;
            }
            set
            {
                if (object.ReferenceEquals(_selectedPCP, value) != true)
                {
                    _selectedPCP = value;
                    NotifyPropertyChanged("SelectedPCP");
                }
            }
        }
        
        public override void RefreshDataGrid(object pageName)
        {
            try
            {

                if (pageName.ToString() == PAGE_NAME)
                {
                    Dictionary<string, string> gridParams = new Dictionary<string, string>() {{"Start","0"},{"ItemsPerPage","10"},{"SortColumn","DeviceType"},{"Ascending","true"}};
                    DuplicateDevicesForPCPList =PCPManagementService.GetPcpDevices(CircuitId, OperatingNo,gridParams , out totalItems);
                }
                NotifyPropertyChanged("Start");
                NotifyPropertyChanged("End");
                NotifyPropertyChanged("TotalItems");
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while loading devices. \n {0} \n StackTrace: {1}", ex.Message, ex.StackTrace), "Error while loading devices");
            }
        }

    }
}
