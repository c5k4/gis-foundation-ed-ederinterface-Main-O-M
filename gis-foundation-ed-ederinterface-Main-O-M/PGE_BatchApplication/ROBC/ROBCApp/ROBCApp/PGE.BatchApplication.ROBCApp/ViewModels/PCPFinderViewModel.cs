using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PGE.BatchApplication.ROBCService;
using System.Collections.ObjectModel;

namespace PGE.BatchApplication.ROBCApp
{
    public class PCPFinderViewModel : BaseViewModel
    {
        public PCPFinderViewModel()
        {
            this.IsValid = false;
            this.IsProcessed = false;
        }
        private string _circuitId;
        public string CircuitId
        {
            get { return _circuitId; }
            set
            {
                _circuitId = value;
                NotifyPropertyChanged("CircuitId");
            }
        }

        private string _operatingNo;
        public string OperatingNo
        {
            get { return _operatingNo; }
            set
            {
                _operatingNo = value;
                NotifyPropertyChanged("OperatingNo");
            }
        }

        private ObservableCollection<PCPModel> _pcpDevicesList;
        public ObservableCollection<PCPModel> PCPDevicesList
        {
            get
            {
                return _pcpDevicesList;
            }
            private set
            {
                if (object.ReferenceEquals(_pcpDevicesList, value) != true)
                {
                    _pcpDevicesList = value;
                    NotifyPropertyChanged("PCPDevicesList");
                }
            }
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand
                    (
                      param =>
                      {
                          SearchPCP();
                      }
                    );
                }

                return _searchCommand;
            }
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand
                    (
                      param =>
                      {
                          ClearFields();
                      }
                    );
                }

                return _clearCommand;
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            private set
            {
                _isValid = value;
                NotifyPropertyChanged("IsValid");
            }
        }

        private bool _isSingleDevice;
        public bool IsSingleDevice
        {
            get { return _isSingleDevice; }
            private set
            {
                _isSingleDevice = value;
                NotifyPropertyChanged("IsSingleDevice");
            }
        }

        private bool _isProcessed;
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set
            {
                _isProcessed = value;
                NotifyPropertyChanged("IsProcessed");
            }
        }
        private void ClearFields()
        {
            this.OperatingNo = string.Empty;
            this.CircuitId = string.Empty;
        }

        private void SearchPCP()
        {
            try
            {
                if (IsProcessed)
                    return;
                if (string.IsNullOrEmpty(this.CircuitId) || string.IsNullOrEmpty(this.OperatingNo))
                {
                    IsValid = false;
                    popup("Circuit Id and Operating No are required!", "Error");
                }
                else
                {
                    int totalItems = 0;
                    Dictionary<string, string> gridParams = new Dictionary<string, string>() {{"Start","0"},{"ItemsPerPage","10"},{"SortColumn","DeviceType"},{"Ascending","true"}};
                    ObservableCollection<PCPModel> devices = PCPManagementService.GetPcpDevices(this.CircuitId, this.OperatingNo, gridParams, out totalItems);
                    this.PCPDevicesList = devices;
                    if (devices == null || !devices.Any())
                    {
                        IsSingleDevice = true;
                        IsValid = false;
                        popup("No devices found! Please make sure circuit id and operating number are valid!", "Devices Search");
                    }
                    else if (devices != null && devices.Count == 1)
                    {
                        IsSingleDevice = true;
                        IsValid = true;
                    }
                    else if (devices != null && devices.Count > 1)
                    {
                        IsSingleDevice = false;
                        IsValid = true;
                    }

                }
            }
            catch (Exception ex)
            {
                
                popup(string.Format("Following error has occurred while searching for devices.\n{0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error");
            }
            
        }
    }
   
}
