using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.BatchApplication.ROBCService;
using System.ComponentModel;
using System.Windows.Input;
namespace PGE.BatchApplication.ROBCApp
{
    public class ManagePCPViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private BackgroundWorker _worker;
        private int _progressPercentage = 0;
        private const string CustomerCountMethodName = "CUSTOMER_COUNT";
        private const string MakePcpMethodName = "MAKE_PCP";
        #region constructors
        public ManagePCPViewModel()
        {

        }
        public ManagePCPViewModel(PCPModel model)
        {
            Initialize(model);
            _worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        
        }
        #region BackgroundWorker Events

        // Note: This event fires on the background thread.

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            int result = 0;
            
                if (worker.WorkerReportsProgress)
                {
                    if (e.Argument != null)
                    {
                        if (e.Argument.ToString() == CustomerCountMethodName)
                        {
                            GetTotalCustomerCount();
                        }
                        else if (e.Argument.ToString() == MakePcpMethodName)
                        {
                            MakePcp();
                        }
                    }
                    
                }
            
            e.Result = result;
        }

        // Note: This event fires on the UI thread.
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            this.IsBusyFlag = false;
        }

        // Note: This event fires on the UI thread.
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }
        #endregion
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    NotifyPropertyChanged("ProgressPercentage");
                }
            }
        }
       
        #endregion
        #region properties
        private PCPModel _pcpROBC;
        public PCPModel PcpROBC
        {
            get
            {
                return _pcpROBC;
            }
            set
            {
                if (object.ReferenceEquals(_pcpROBC, value) != true)
                {
                    _pcpROBC = value;
                    NotifyPropertyChanged("PcpROBC");
                }
            }
        }


        private bool _isBusyFlag;
        public bool IsBusyFlag
        {
            get { return _isBusyFlag; }
            private set
            {
                _isBusyFlag = value;
                NotifyPropertyChanged("IsBusyFlag");
            }
        }
        
        private bool _pcpDeviceVisibility;
        public bool PcpDeviceVisibility
        {
            get { return _pcpDeviceVisibility; }
            private set
            {
                _pcpDeviceVisibility = value;
                NotifyPropertyChanged("PcpDeviceVisibility");
            }
        }
        
        private bool _nonPcpDeviceVisibility;
        public bool NonPcpDeviceVisibility
        {
            get { return _nonPcpDeviceVisibility; }
            private set
            {
                _nonPcpDeviceVisibility = value;
                NotifyPropertyChanged("NonPcpDeviceVisibility");
            }
        }
        
        private bool _isCustomerCountProcessed;
        public bool IsCustomerCountProcessed
        {
            get { return _isCustomerCountProcessed; }
            private set
            {
                _isCustomerCountProcessed = value;
                NotifyPropertyChanged("IsCustomerCountProcessed");
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

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private string _AlertMessage;
        public string AlertMessage
        {
            get { return _AlertMessage; }
            set
            {
                _AlertMessage = value;
                NotifyPropertyChanged("AlertMessage");
            }
        }
        #endregion
        #region command properties
        private ICommand _calculateCustomerCountCommand;
        public ICommand CalculateCustomerCountCommand
        {
            get
            {
                if (_calculateCustomerCountCommand == null)
                {
                    _calculateCustomerCountCommand = new RelayCommand
                    (
                      param =>
                      {
                          //GetTotalCustomerCount();
                          if (!_worker.IsBusy) { _worker.RunWorkerAsync(CustomerCountMethodName); this.IsBusyFlag = true; }
                      }
                    );
                }

                return _calculateCustomerCountCommand;
            }
        }

        private ICommand _makePCPCommand;
        public ICommand MakePCPCommand
        {
            get
            {
                if (_makePCPCommand == null)
                {
                    _makePCPCommand = new RelayCommand
                    (
                      param =>
                      {
                          //MakePcp();
                          if (!_worker.IsBusy) { _worker.RunWorkerAsync(MakePcpMethodName); this.IsBusyFlag = true; }
                      },
                      param =>
                      {
                          return this.NonPcpDeviceVisibility;
                      }
                    );
                }

                return _makePCPCommand;
            }
        }



        private ICommand _updatePCPCommand;
        public ICommand UpdatePCPCommand
        {
            get
            {
                if (_updatePCPCommand == null)
                {
                    _updatePCPCommand = new RelayCommand
                    (
                      param =>
                      {
                          UpdatePcp();
                      },
                      param =>
                      {
                          return this.PcpDeviceVisibility;
                      }
                    );
                }

                return _updatePCPCommand;
            }
        }
        private ICommand _removePCPCommand;
        public ICommand RemovePCPCommand
        {
            get
            {
                if (_removePCPCommand == null)
                {
                    _removePCPCommand = new RelayCommand
                    (
                      param =>
                      {
                          RemovePcp();
                      },
                      param =>
                      {
                          return this.PcpDeviceVisibility;
                      }
                    );
                }

                return _removePCPCommand;
            }
        }
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand
                    (
                      param =>
                      {
                          //TO DO: command method
                      },
                      param =>
                      {
                          return true;
                      }
                    );
                }

                return _cancelCommand;
            }
        }
        #endregion
        #region methods
        private void Initialize(PCPModel pcpModelObj)
        {
            try
            {
                this.IsCustomerCountProcessed = false;
                this.PcpROBC = pcpModelObj;
                var deviceGUID = PcpROBC.DeviceGuid!=null?this.PcpROBC.DeviceGuid.ToString().Trim():string.Empty;
                if (string.IsNullOrEmpty(deviceGUID))
                {
                    IsValid = false;
                    popup("Invalid Device Global ID!", "Error");
                }
                else
                {
                    GetPcp();
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while loading.\n{0}\nStackTrace:{1}",ex.Message,ex.StackTrace), "Error");
            }
        }

        private void MakePcp()
        {
            try
            {
                string errorMessage = SavePcp("ADD");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    popup("PCP created successfully", "Success");
                    GetPcp();
                }
                
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while creating a PCP.\n{0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error while creating a PCP");
            }

            NotifyPropertyChanged("PcpROBC");

        }
        private void UpdatePcp()
        {
            try
            {
                string errorMessage = SavePcp("UPDATE");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    popup("PCP updated successfully", "Success");
                    GetPcp();
                }
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while updating a PCP.\n{0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error while updating a PCP");
            }

            NotifyPropertyChanged("PcpROBC");
        }
        private string SavePcp(string operation)
        {
            var errorMessage = string.Empty;
            StringBuilder errorMessages = new StringBuilder();
            string summerProjectedStr = this.PcpROBC.SummerProjected!=null?this.PcpROBC.SummerProjected.ToString().Trim():string.Empty;
            string winterProjectedStr = this.PcpROBC.WinterProjected!=null?this.PcpROBC.WinterProjected.ToString().Trim():string.Empty;
            int summerProjectedInt;
            int winterProjectedInt;
            if (int.TryParse(summerProjectedStr, out summerProjectedInt) && int.TryParse(winterProjectedStr, out winterProjectedInt))
            {
                this.PcpROBC.SummerProjected = summerProjectedInt;
                this.PcpROBC.WinterProjected = winterProjectedInt;
            }
            else
            {
                errorMessage = "Summer and Winter load information must be numeric!";
                popup(errorMessage, "Error");
                return errorMessage;
            }
            if(this.PcpROBC.TotalCustomer==null || string.IsNullOrEmpty(this.PcpROBC.TotalCustomer.ToString().Trim()))
            {
                errorMessage = "Customer counts can't be empty! Click on the \"Calculate Customer Count\" button to get customer counts!";
                popup(errorMessage, "Error");
                return errorMessage;
            }
            string errorMessageHeading = string.Empty;
            if (operation == "ADD")
            {
                errorMessageHeading = "Cannot create PCP because of following reason(s).";
                this.PcpROBC = PCPManagementService.CreatePcp(this.PcpROBC, out errorMessages);
            }
            else if (operation == "UPDATE")
            {
                errorMessageHeading = "Cannot update PCP because of following reason(s).";
                this.PcpROBC = PCPManagementService.UpdatePcp(this.PcpROBC, out errorMessages);
            }
            if (errorMessages.Length>0)
            {
                errorMessage += string.Format("{0}\n{1}",errorMessageHeading,errorMessages.ToString());
                /*foreach (var condition in pcpConditions)
                {
                    switch (condition.Key)
                    {
                        case "HAS_PARENT_EST_ROBC_E":
                            if (condition.Value == "FALSE")
                            {
                                errorMessage = "\nCircuit is non-essential. Cannot make a PCP on a non-essential circuit.";
                            }
                            break;
                        case "HAS_ESSENTIAL_CUSTOMER":
                            if (condition.Value == "TRUE")
                            {
                                errorMessage += "\nPCP has essential customer(s).\"E\"";
                            }
                            break;
                        case "IS_PCP_FOUND_FOR_NEW":
                            if (condition.Value == "TRUE")
                            {
                                errorMessage += "\nPCP already exists!";
                            }
                            break;
                        case "IS_PCP_FOUND_FOR_UPDATE":
                            if (condition.Value == "FALSE")
                            {
                                errorMessage += "\nPCP doesn't exist!";
                            }
                            break;
                        case "HAS_ROBC_RECORD":
                            if (condition.Value == "FALSE")
                            {
                                errorMessage += "\nROBC record(s) don't exist!";
                            }
                            break;
                        default:
                            break;
                    }
                }*/
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                popup(errorMessage, "Error");
            }
            return errorMessage;
                
        }
        private void RemovePcp()
        {
            try
            {
                this.PcpROBC = PCPManagementService.RemovePcp(this.PcpROBC);
                popup("PCP removed successfully", "Success");
                GetPcp();
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while removing PCP.\n{0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error while removing a PCP");
            }

            NotifyPropertyChanged("PcpROBC");
        }

        private void GetPcp()
        {
            this.PcpROBC = PCPManagementService.GetPcp(this.PcpROBC);
            if (PcpROBC == null || PcpROBC.PcpGlobalId == null)
            {
                NonPcpDeviceVisibility = true;
                PcpDeviceVisibility = false;
                this.PcpROBC.ModifiedDate = null;
                this.PcpROBC.ModifiedUser = null;
                this.PcpROBC.CreatedDate = null;
                this.PcpROBC.CreatedUser = null;
                this.PcpROBC.SummerProjected = null;
                this.PcpROBC.WinterProjected = null;
            }
            else
            {
                NonPcpDeviceVisibility = false;
                PcpDeviceVisibility = true;
            }
        }
        private void GetTotalCustomerCount()
        {
            try
            {
                this.PcpROBC.TotalCustomer = PCPManagementService.GetPcpCustomerCounts(this.PcpROBC.DeviceGuid != null ? this.PcpROBC.DeviceGuid.ToString().Trim() : string.Empty);
                IsCustomerCountProcessed = true;
                NotifyPropertyChanged("PcpROBC");
            }
            catch (Exception ex)
            {
                popup(string.Format("Following error has occurred while retrieving customer counts.\n{0}\nStackTrace: {1}", ex.Message, ex.StackTrace), "Error");
            }

        }
        #endregion
    }
}