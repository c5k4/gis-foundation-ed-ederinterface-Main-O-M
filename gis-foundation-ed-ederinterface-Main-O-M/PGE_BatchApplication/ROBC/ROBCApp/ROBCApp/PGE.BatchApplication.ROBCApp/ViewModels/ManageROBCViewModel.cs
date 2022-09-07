using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PGE.BatchApplication.ROBCService;
using PGE.BatchApplication.ROBCService.Common;

namespace PGE.BatchApplication.ROBCApp
{
    public class ManageROBCViewModel : INotifyPropertyChanged
    {
        private bool _saveParent;
        private bool _saveChild;
        private BackgroundWorker _worker;
        private ROBCManagementService _robcManagementService;
        protected Action<string, string> popup = (Action<string, string>)((msg, capt) => System.Windows.MessageBox.Show(msg, capt));
        private Func<string, string, bool> confirm = (Func<string, string, bool>)((msg, capt) =>
        System.Windows.MessageBox.Show(msg, capt, System.Windows.MessageBoxButton.YesNo) ==
                  System.Windows.MessageBoxResult.Yes);



        #region Constructors

        public ManageROBCViewModel(string circuitID)
        {
            try
            {
                _robcManagementService = new ROBCManagementService();
                if (!string.IsNullOrEmpty(circuitID))
                {
                    CoreService.WriteLog("Start-->FindCircuit(CircuitID)");
                    CircuitSourceROBCFieldValues = new CircuitService().FindCircuit(circuitID, true);
                    CoreService.WriteLog("End-->FindCircuit(CircuitID)");
                }

                CircuitROBC = _robcManagementService.GetROBCModel(CircuitSourceROBCFieldValues);
                Initialize();

            }
            catch (Exception ex)
            {
                popup(ex.Message, "Error");
            }
        }

        public ManageROBCViewModel(Dictionary<string, string> circuitSourceROBCFieldValues)
        {
            try
            {

                _robcManagementService = new ROBCManagementService();
                CircuitSourceROBCFieldValues = circuitSourceROBCFieldValues;

                CircuitROBC = _robcManagementService.GetROBCModel(circuitSourceROBCFieldValues);
                Initialize();

            }
            catch (Exception ex)
            {
                popup(ex.Message, "Error");
            }
        }

        public ManageROBCViewModel(IViewModel parentViewModel)
        {
            try
            {

                _robcManagementService = new ROBCManagementService();
                ParentViewModel = parentViewModel;
                if (parentViewModel is MultipleCircuitsROBCViewModel)
                    CircuitROBC = (parentViewModel as MultipleCircuitsROBCViewModel).SelectedROBC;

                Initialize();

            }
            catch (Exception ex)
            {
                popup(ex.Message, "Error");
            }
        }

        public ManageROBCViewModel(ROBCModel robcModel)
        {
            try
            {
                _robcManagementService = new ROBCManagementService();
                CircuitROBC = robcModel;
                Initialize();

            }
            catch (Exception ex)
            {
                popup(ex.Message, "Error");
            }
        }

        public ManageROBCViewModel()
        {
            try
            {
                _robcManagementService = new ROBCManagementService();
                Initialize();

            }
            catch (Exception ex)
            {
                popup(ex.Message, "Error");
            }
        }

        #endregion
        #region Properties

        //To interact with MultipleCircuitsROBC
        public IViewModel ParentViewModel { get; set; }

        public Dictionary<string, string> CircuitSourceROBCFieldValues { get; set; }
        public List<ROBCModel> FeederCircuitSourcesROBCs { get; set; }
        public List<ROBCModel> ChildCircuitSourcesROBCs { get; set; }

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

        private ROBCModel _circuitROBC;
        public ROBCModel CircuitROBC
        {
            get
            {
                return _circuitROBC;
            }
            set
            {
                if (object.ReferenceEquals(_circuitROBC, value) != true)
                {
                    _circuitROBC = value;
                    NotifyPropertyChanged("CircuitROBC");
                }
            }
        }

        private bool _isRobcSubblockEnabled;
        public bool IsRobcSubblockEnabled
        {
            get { return _isRobcSubblockEnabled; }
            set
            {
                _isRobcSubblockEnabled = value;
                NotifyPropertyChanged("IsRobcSubblockEnabled");
            }
        }

        private bool _YesNoGridVisibility;
        public bool YesNoGridVisibility
        {
            get { return _YesNoGridVisibility; }
            set
            {
                _YesNoGridVisibility = value;
                NotifyPropertyChanged("YesNoGridVisibility");
            }
        }

        private bool _AssignCancelButtonGridVisibility;
        public bool AssignCancelButtonGridVisibility
        {
            get { return _AssignCancelButtonGridVisibility; }
            set
            {
                _AssignCancelButtonGridVisibility = value;
                NotifyPropertyChanged("AssignCancelButtonGridVisibility");
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

        private string _modifiedDateInfo;
        public string ModifiedDateInfo
        {
            get { return _modifiedDateInfo; }
            set
            {
                _modifiedDateInfo = value;
                NotifyPropertyChanged("ModifiedDateInfo");
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

        private IEnumerable<RobcCodeValue> _RobcCodeValues;
        public IEnumerable<RobcCodeValue> RobcCodeValues
        {
            get
            {
                return _RobcCodeValues;
            }
            set
            {
                _RobcCodeValues = value;
            }
        }

        private IEnumerable<SubBlockCodeValue> _SubBlockCodeValues;
        public IEnumerable<SubBlockCodeValue> SubBlockCodeValues
        {
            get
            {
                return _SubBlockCodeValues;
            }
        }


        private ICommand _calculateLoadInfoCommand;
        public ICommand CalculateLoadInfoCommand
        {
            get
            {
                if (_calculateLoadInfoCommand == null)
                {
                    _calculateLoadInfoCommand = new RelayCommand
                    (
                      param =>
                      {
                          CalculateLoadInformationUsingWorkerProcess();
                      }
                    );
                }

                return _calculateLoadInfoCommand;
            }
        }



        private ICommand _assignROBCCommand;
        public ICommand AssignROBCCommand
        {
            get
            {
                if (_assignROBCCommand == null)
                {
                    _assignROBCCommand = new RelayCommand
                    (
                      param =>
                      {
                          AssignROBC();
                      },
                      param =>
                      {
                          return _isValid;
                      }
                    );
                }

                return _assignROBCCommand;
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

        private ICommand _setROBCCommand;
        public ICommand SetROBCCommand
        {
            get
            {
                if (_setROBCCommand == null)
                {
                    _setROBCCommand = new RelayCommand
                    (
                      param =>
                      {
                          ExecuteWorkerProcess();
                      },
                      param =>
                      {
                          return true;
                      }
                    );
                }

                return _setROBCCommand;
            }
        }


        private ICommand _NoCommand;
        public ICommand NoCommand
        {
            get
            {
                if (_NoCommand == null)
                {
                    _NoCommand = new RelayCommand
                    (
                      param =>
                      {
                          HideYesNo();
                      },
                      param =>
                      {
                          return true;
                      }
                    );
                }

                return _NoCommand;
            }
        }

        #endregion




        #region Private Methods

        private void Initialize()
        {

            _isValid = !string.IsNullOrEmpty(CircuitROBC.CircuitId) && !string.IsNullOrEmpty(CircuitROBC.CircuitSourceGuid);

            string lasteModifiedDate = string.IsNullOrEmpty(CircuitROBC.ModifiedDate) ? CircuitROBC.CreatedDate : CircuitROBC.ModifiedDate;

            if (!_isValid)
            {
                popup("No records found. The circuit ID may not be valid.", "Error");
            }
            else if (string.IsNullOrEmpty(lasteModifiedDate))
                ModifiedDateInfo = string.Format("ROBC last modified on: ");
            else
                ModifiedDateInfo = string.Format("ROBC last modified on: {0}", lasteModifiedDate);

            PopulateComboData();

            var ParentCircuitSourcesROBCFieldValues = new CircuitService().FindFeedingFeeder(CircuitROBC.CircuitId);
            List<Dictionary<string, string>> ChildCircuitSourcesROBCFieldValuesList = new CircuitService().FindChildFeeders(CircuitROBC.CircuitId);

            List<ROBCModel> parentROBCs = new List<ROBCModel>();
            List<ROBCModel> childROBCs = new List<ROBCModel>();
            string parentCircuitID = string.Empty;
            StringBuilder childCircuitIDs = new StringBuilder();
            if (ParentCircuitSourcesROBCFieldValues.Count > 0)
            {
                parentCircuitID = ParentCircuitSourcesROBCFieldValues["CIRCUITID"];
                parentROBCs.Add(_robcManagementService.GetROBCModel(ParentCircuitSourcesROBCFieldValues));
            }
            foreach (Dictionary<string, string> circuitSourceROBCFieldValues in ChildCircuitSourcesROBCFieldValuesList)
            {
                childCircuitIDs.AppendFormat("{0}; ", circuitSourceROBCFieldValues["CIRCUITID"]);
                childROBCs.Add(_robcManagementService.GetROBCModel(circuitSourceROBCFieldValues));
            }


            FeederCircuitSourcesROBCs = parentROBCs;
            ChildCircuitSourcesROBCs = childROBCs;

            CircuitROBC.ParentCircuitID = parentCircuitID;
            if (childCircuitIDs.Length > 0)
            {
                CircuitROBC.ChildCircuitIDs = childCircuitIDs.ToString().Remove(childCircuitIDs.ToString().Length - 1, 1);
            }
            //Button grid visiblifty
            YesNoGridVisibility = false;
            AssignCancelButtonGridVisibility = true;
            ResetScadaAndLoadInformation();
        }

        private void ResetScadaAndLoadInformation()
        {
            this.CircuitROBC.WinterKVA = this.CircuitROBC.SummerKVA = this.CircuitROBC.TotalCustomer = this.CircuitROBC.IsScada = string.Empty;
        }
        private void PopulateComboData()
        {
            _RobcCodeValues = _robcManagementService.PopulateRobcCodeValues();
            _SubBlockCodeValues = _robcManagementService.PopulateSubBlockCodeValues();
        }



        #endregion


        #region Command Methods


        public bool AssignROBC()
        {
            try
            {
                bool canSave = true;
                if (CircuitROBC.DesiredROBC == Constants.FixedEssentialROBCCode && !string.IsNullOrEmpty(CircuitROBC.DesiredSubBlock))
                {
                    popup("Desired Subblock will be cleared because Desired ROBC is \"F\"!", "Information");
                    CircuitROBC.DesiredSubBlock = "";
                    CircuitROBC.DesiredSubBlockDesc = "";
                }
                if (string.IsNullOrEmpty(CircuitROBC.DesiredROBC) || (CircuitROBC.DesiredROBC != Constants.FixedEssentialROBCCode && string.IsNullOrEmpty(CircuitROBC.DesiredSubBlock)))
                {
                    popup("Desired ROBC and Subblock can't be empty!", "Error");
                    canSave = false;
                    return canSave;
                }
                string robcDesc = string.Empty;
                try
                {
                    robcDesc = RobcCodeValues.ToList<RobcCodeValue>().Find(r => r.RobcCode == CircuitROBC.DesiredROBC).RobcDesc.ToString();
                }
                catch { }
                string robcCode = CircuitROBC.DesiredROBC;

                if (robcCode == Constants.FixedEssentialROBCCode) //ROBC = "F"?
                {
                    /*
                    if (CircuitROBC.EstablishedROBC == Constants.EstablishedROBCCode) //ROBC = "E"?
                    {
                        popup("The Established ROBC has already been set to \"E\". Desired ROBC cannot be set to \"F\".","Information");
                        canSave = false;
                    }*/

                    var hasEssentialCustomer = new CircuitService().GetEssentialCustomer(CircuitROBC.CircuitId);
                    /*if (hasEssentialCustomer)
                    {
                        popup("The circuit has essential customer(s). ROBC cannot be set to \"F\".", "Information");
                        canSave = false;
                    }
                    else
                    {*/
                        if (FeederCircuitSourcesROBCs.Count > 0) //Has parent?
                        {
                            _saveParent = true;
                            string parentCircuitIDs = "";
                            int count = 0;
                            foreach (ROBCModel parentFeederROBC in FeederCircuitSourcesROBCs)
                            {
                                if (count == FeederCircuitSourcesROBCs.Count - 1)
                                {
                                    parentCircuitIDs += parentFeederROBC.CircuitId;
                                }
                                else
                                    parentCircuitIDs += parentFeederROBC.CircuitId + ", ";

                                count++;
                            }
                            //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}. ", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                            AlertMessage = string.Format("Circuit ID: {0}, Parent Circuit ID(s): {1}. Do you want to set all circuits to ROBC: \"{2}\"?", CircuitROBC.CircuitId, parentCircuitIDs, robcDesc);
                        }
                        else // No parent
                        {
                            //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}.", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                            AlertMessage = string.Format("Do you want to set circuit (ID: {0}) to ROBC: \"{1}\"?", CircuitROBC.CircuitId, robcDesc);
                            _saveParent = false;
                        }
                    /*}*/

                }
                else // ROBC is not "F"
                {
                    if (ChildCircuitSourcesROBCs.Count > 0) //Has child?
                    {
                        _saveChild = true;

                        bool hasF = false;
                        int count = 0;
                        string childCircuitIDs = "";
                        // List of Child circuits with "F"
                        foreach (ROBCModel childFeederROBC in ChildCircuitSourcesROBCs)
                        {
                            if (childFeederROBC.DesiredROBC == Constants.FixedEssentialROBCCode) //F?
                            {
                                if (count == ChildCircuitSourcesROBCs.Count - 1)
                                {
                                    childCircuitIDs += childFeederROBC.CircuitId;
                                }
                                else
                                    childCircuitIDs += childFeederROBC.CircuitId + ", ";

                                count++;

                                hasF = true;
                            }
                        }
                        if (hasF)
                        {
                            //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}. ", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                            AlertMessage = string.Format("Child Circuit ID(s): {0} have ROBC \"F\". Do you want to set all circuits to ROBC: \"{1}\"?", childCircuitIDs, robcDesc);
                        }
                        else
                        {
                            //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}.", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                            AlertMessage = string.Format("Do you want to set circuit (ID: {0}) to ROBC: \"{1}\"?", CircuitROBC.CircuitId, robcDesc);
                        }

                    }
                    else //No Child
                    {
                        _saveChild = false;

                        if (FeederCircuitSourcesROBCs.Count > 0) //Has parent?
                        {
                            _saveParent = true;
                            bool hasF = false;
                            int count = 0;
                            string parentCircuitIDs = "";
                            // List of Child circuits with "F"
                            foreach (ROBCModel parentFeederROBC in FeederCircuitSourcesROBCs)
                            {
                                if (parentFeederROBC.DesiredROBC == Constants.FixedEssentialROBCCode) //F?
                                {
                                    hasF = true;
                                }
                                if (count == FeederCircuitSourcesROBCs.Count - 1)
                                    parentCircuitIDs += parentFeederROBC.CircuitId;
                                else
                                    parentCircuitIDs += parentFeederROBC.CircuitId + ", ";

                                count++;
                            }

                            if (hasF)
                            {
                                //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}. ", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                                AlertMessage = string.Format("Circuit ID: {0}. Parent Circuit ID(s): {1} have ROBC \"F\". Do you want to set all circuits to ROBC: \"{2}\"?", CircuitROBC.CircuitId, parentCircuitIDs, robcDesc);
                            }
                            else
                            {
                                //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}. ", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                                AlertMessage = string.Format("Circuit ID: {0}, Parent Circuit ID(s): {1}. Do you want to set all circuits to ROBC: \"{2}\"?", CircuitROBC.CircuitId, parentCircuitIDs, robcDesc);

                            }

                        }
                        else // No parent
                        {
                            _saveParent = false;
                            //AlertMessage = string.Format("SummerKVA: {0}, WinterKVA: {1}, Total Customer: {2}.", CircuitROBC.SummerKVA, CircuitROBC.WinterKVA, CircuitROBC.TotalCustomer);
                            AlertMessage = string.Format("Do you want to set circuit (ID: {0}) to ROBC: \"{1}\"?", CircuitROBC.CircuitId, robcDesc);
                        }
                    }

                }

                if (canSave)
                {
                    YesNoGridVisibility = true;
                    AssignCancelButtonGridVisibility = false;
                }
                NotifyPropertyChanged("CircuitROBC");
                return canSave;
            }
            catch (Exception ex)
            {
                popup(string.Format("An error occurred assigning ROBC: \n\n{0}\n\n{1}", ex.Message, ex.StackTrace), "Error");
                return false;
            }

        }

        public void SetROBC()
        {

            List<ROBCModel> robcModels = new List<ROBCModel>();

            try
            {
                if (_saveParent)
                {
                    foreach (ROBCModel parentRobc in FeederCircuitSourcesROBCs)
                    {
                        robcModels.Add(parentRobc);
                    }
                    _robcManagementService.SaveROBCs(robcModels, CircuitROBC, true);
                    robcModels.Clear();
                }

                this.CircuitROBC = _robcManagementService.SaveROBC(CircuitROBC);

                if (_saveChild)
                {
                    foreach (ROBCModel childRobc in ChildCircuitSourcesROBCs)
                    {
                        robcModels.Add(childRobc);
                    }
                    _robcManagementService.SaveROBCs(robcModels, CircuitROBC, false);
                    robcModels.Clear();
                }






                //Message = "ROBC assigned successfully";
                popup("ROBC assigned successfully", "Success");

                CircuitROBC.DesiredROBCDesc = string.IsNullOrEmpty(CircuitROBC.DesiredROBC) ? string.Empty : _robcManagementService.GetDomainCodeValues(Constants.RobcDomainName)[CircuitROBC.DesiredROBC];
                CircuitROBC.DesiredSubBlockDesc = string.IsNullOrEmpty(CircuitROBC.DesiredSubBlock) ? string.Empty : _robcManagementService.GetDomainCodeValues(Constants.SubBlockDomainName)[CircuitROBC.DesiredSubBlock];
                CircuitROBC.EstablishedROBCDesc = string.IsNullOrEmpty(CircuitROBC.EstablishedROBC) ? string.Empty : _robcManagementService.GetDomainCodeValues(Constants.RobcDomainName)[CircuitROBC.EstablishedROBC];
                CircuitROBC.EstablishedSubBlockDesc = string.IsNullOrEmpty(CircuitROBC.EstablishedSubBlock) ? string.Empty : _robcManagementService.GetDomainCodeValues(Constants.SubBlockDomainName)[CircuitROBC.EstablishedSubBlock];
            }
            catch (Exception ex)
            {
                //Message = string.Format("Error saving ROBC: {0}", ex.Message);
                popup(string.Format("Error saving ROBC: {0}\n{1}", ex.Message, ex.StackTrace), "Error");
                //throw;
            }

            YesNoGridVisibility = false;
            AssignCancelButtonGridVisibility = true;

            //Reset
            _saveParent = false;
            _saveChild = false;

            string lasteModifiedDate = string.IsNullOrEmpty(CircuitROBC.ModifiedDate) ? CircuitROBC.CreatedDate : CircuitROBC.ModifiedDate;

            if (string.IsNullOrEmpty(lasteModifiedDate))
                ModifiedDateInfo = string.Format("ROBC last modified on: ");
            else
                ModifiedDateInfo = string.Format("ROBC last modified on: {0}", lasteModifiedDate);


            NotifyPropertyChanged("CircuitROBC");
        }


        private void CalculateLoadInformation()
        {
            CircuitROBC = new ROBCManagementService().GetLoadingAndScadaInformation(CircuitROBC);
            NotifyPropertyChanged("CircuitROBC");
        }
        public void HideYesNo()
        {
            YesNoGridVisibility = false;
            AssignCancelButtonGridVisibility = true;

        }

        #endregion
        #region BackgroundWorker Events

        public void ExecuteWorkerProcess()
        {
            _worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += worker_DoWork;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            if (!_worker.IsBusy) { _worker.RunWorkerAsync(); this.IsBusyFlag = true; }
        }
        // Note: This event fires on the background thread.

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;

            int result = 0;

            if (worker.WorkerReportsProgress)
            {
                SetROBC();
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

        }


        //Calculate Load Information worker processes


        private void CalculateLoadInformationUsingWorkerProcess()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += worker_DoWork_LoadInfo;
            _worker.RunWorkerCompleted += worker_RunWorkerCompleted_LoadInfo;
            if (!_worker.IsBusy) { _worker.RunWorkerAsync(); this.IsBusyFlag = true; }
        }

        private void worker_DoWork_LoadInfo(object sender, DoWorkEventArgs e)
        {
            CalculateLoadInformation();
        }


        // Note: This event fires on the UI thread.
        private void worker_RunWorkerCompleted_LoadInfo(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsBusyFlag = false;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;


        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




    }

}
