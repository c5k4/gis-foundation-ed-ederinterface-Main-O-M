using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.BatchApplication.ROBCService;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace PGE.BatchApplication.ROBCApp
{
    public class UnassignedROBCViewModel : DataGridCommonViewModel, IViewModel
    {
        private const string PAGE_NAME = "UNASSIGNED_ROBC";

        private ROBCManagementService _robcService;
        private readonly BackgroundWorker worker;
        //private readonly ICommand _loadButtonCommand;

        public UnassignedROBCViewModel()
        {
            try
            {
                _robcService = new ROBCManagementService();
                this.DataGridVisibility = false;
                this.LoadButtonVisibility = true;
                //this._loadButtonCommand = new
                //      RelayCommand(o => this.worker.RunWorkerAsync(), o => !this.worker.IsBusy);
                this.worker = new BackgroundWorker();
                this.worker.DoWork += this.DoWork;
                this.worker.ProgressChanged += this.ProgressChanged;
                //this.ProcessingImageVisibility = false;
            }
            catch (Exception ex)
            {
                var Info = string.Format("{0}////{1}",ROBCService.Common.Constants.EDERDatabaseTNSName, ROBCService.Common.Constants.EDERSUBDatabaseTNSName);
                popup(string.Format("An exption from Get method:{0}\n\n\nStackTrace:{1}\n\n\n{2}\n {3}", ex.Message, ex.StackTrace, Info,ex.InnerException),"Error");
                //popup(string.Format("An error has occurred while loading:\n{0}\n\n{1}",ex.Message,ex.StackTrace), "Error");
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
        private ICommand _loadButtonCommand;
        
        public ICommand LoadButtonCommand
        {
            get
            {
                if (_loadButtonCommand == null)
                {
                    _loadButtonCommand = new RelayCommand
                    (
                      param =>
                      {
                          LoadUnassignedROBC();
                      },
                      param =>
                      {
                          return _loadButtonVisibility;
                      }
                    );
                }

                return _loadButtonCommand;
            }
        }
        //public ICommand LoadButtonCommand
        //{
        //    get { return this._loadButtonCommand; }
        //}

        private ICommand _cancelButtonCommand;
        public ICommand CancelButtonCommand
        {
            get
            {
                if (_cancelButtonCommand == null)
                {
                    _cancelButtonCommand = new RelayCommand
                    (
                      param =>
                      {
                          CancelUI();
                      },
                      param =>
                      {
                          return true;
                      }
                    );
                }

                return _cancelButtonCommand;
            }
        }

       

        
        private bool _loadButtonVisibility;
        public bool LoadButtonVisibility
        {
            get { return _loadButtonVisibility; }
            set
            {
                _loadButtonVisibility = value;
                NotifyPropertyChanged("LoadButtonVisibility");
            }
        }
        private bool _dataGridVisibility;
        public bool DataGridVisibility
        {
            get { return _dataGridVisibility; }
            set
            {
                _dataGridVisibility = value;
                NotifyPropertyChanged("DataGridVisibility");
            }
        }
        private bool _processingImageVisibility;
        public bool ProcessingImageVisibility
        {
            get { return _processingImageVisibility; }
            set
            {
                _processingImageVisibility = value;
                NotifyPropertyChanged("ProcessingImageVisibility");
            }
        }
        private CircuitSourceModel _selectedCircuitWithoutROBC;
        public CircuitSourceModel SelectedCircuitWithoutROBC
        {
            get
            {
                return _selectedCircuitWithoutROBC;
            }
            set
            {
                if (object.ReferenceEquals(_selectedCircuitWithoutROBC, value) != true)
                {
                    _selectedCircuitWithoutROBC = value;
                    NotifyPropertyChanged("SelectedCircuitWithoutROBC");
                }
            }
        }

        private ObservableCollection<CircuitSourceModel> unassignedROBCList;
        public ObservableCollection<CircuitSourceModel> UnassignedROBCList
        {
            get
            {
                return unassignedROBCList;
            }
            private set
            {
                if (object.ReferenceEquals(unassignedROBCList, value) != true)
                {
                    unassignedROBCList = value;
                    NotifyPropertyChanged("UnassignedROBCList");
                }
            }
        }

        public void PopulateCircuitWithoutROBCList()
        {
            _robcService.PopulateCircuitWithoutROBCList();
        }
        /// <summary>
        /// Refreshes the list of ROBCs. Called by navigation commands.
        /// </summary>
        public override void RefreshDataGrid(object pageName)
        {
            string sortColumn = "CircuitId";
            bool ascending = true;
            try
            {
                if (pageName.ToString() == PAGE_NAME)
                {
                    if (_robcService != null)
                        UnassignedROBCList = _robcService.GetCircuitsWithUnassignedROBCs(start, itemCount, sortColumn, ascending, out totalItems);
                }
                NotifyPropertyChanged("Start");
                NotifyPropertyChanged("End");
                NotifyPropertyChanged("TotalItems");
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        private void LoadUnassignedROBC()
        {
            try
            {
                ProcessingImageVisibility = true;
                //System.Threading.Thread.Sleep(10000);
                PopulateCircuitWithoutROBCList();
                RefreshDataGrid(PAGE_NAME);
                LoadButtonVisibility = false;
                DataGridVisibility = true;
                ProcessingImageVisibility = false;
            }
            catch (Exception ex)
            {
                popup(string.Format("An error occured loading list of unassigned ROBCs:\n{0}\n{1}", ex.Message, ex.StackTrace), "Error");
                LoadButtonVisibility = true;
                DataGridVisibility = false;
                ProcessingImageVisibility = false;
            }
        }


        private void CancelUI()
        {
            try
            {
                this.LoadButtonVisibility = true;
                this.DataGridVisibility = false;
                this.ProcessingImageVisibility = false;
            }
            catch(Exception ex)
            {
                popup(string.Format("An error occurred:{0}\n\n{1}", ex.Message, ex.StackTrace), "Error");
            }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return this._currentProgress; }
            private set
            {
                if (this._currentProgress != value)
                {
                    this._currentProgress = value;
                    NotifyPropertyChanged("CurrentProgress");
                }
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.CurrentProgress = e.ProgressPercentage;
        }
       
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // do time-consuming work here, calling ReportProgress as and when you can   
            LoadUnassignedROBC();
            //BackgroundWorker worker = sender as BackgroundWorker;
            //worker.ReportProgress((int)e.Argument);
            //e.Result = UpdateProgressBar((int)e.Argument, worker);
            
            //worker.ReportProgress(percentageDone);
            
            worker.WorkerReportsProgress = true;
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(1000);
                CurrentProgress = i;
                worker.ReportProgress(i);
                NotifyPropertyChanged("CurrentProgress");
            }
            
        }

    }
}
