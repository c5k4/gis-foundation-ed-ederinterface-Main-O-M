using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PGE.BatchApplication.ROBCService;
using System.Windows.Input;

namespace PGE.BatchApplication.ROBCApp
{
    public class InvalidPCPViewModel:DataGridCommonViewModel
    {
        private const string PAGE_NAME = "INVALID_PCP";

        #region Constructor
        public InvalidPCPViewModel()
        {
            this.IsRefreshData = false;
            LoadButtonContent = "Load Data";
        }
        #endregion
        #region Properties
        private bool _isRefreshData;
        public bool IsRefreshData
        {
            get
            {
                return _isRefreshData;
            }
            private set
            {
                if (object.ReferenceEquals(_isRefreshData, value) != true)
                {
                    _isRefreshData = value;
                    NotifyPropertyChanged("IsRefreshData");
                }
            }
        }
        private string _loadButtonContent;
        public string LoadButtonContent
        {
            get
            {
                return _loadButtonContent;
            }
            private set
            {
                if (object.ReferenceEquals(_loadButtonContent, value) != true)
                {
                    _loadButtonContent = value;
                    NotifyPropertyChanged("LoadButtonContent");
                }
            }
        }
        private ObservableCollection<PCPModel> _invalidPCPList;
        public ObservableCollection<PCPModel> InvalidPCPList
        {
            get
            {
                return _invalidPCPList;
            }
            private set
            {
                if (object.ReferenceEquals(_invalidPCPList, value) != true)
                {
                    _invalidPCPList = value;
                    NotifyPropertyChanged("InvalidPCPList");
                }
            }
        }
        #endregion
        #region Command Properties
        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand
                    (
                      param =>
                      {
                          RefreshDataGrid(PAGE_NAME);
                          this.LoadButtonContent = "Refresh Data";
                          IsRefreshData = true;
                      }
                    );
                }

                return _refreshCommand;
            }
        }
        #endregion
        /// <summary>
        /// Refreshes the list of PCPs. Called by navigation commands.
        /// </summary>
        public override void RefreshDataGrid(object pageName)
        {
            try
            {
                if (pageName.ToString() == PAGE_NAME)
                {
                    Dictionary<string, string> gridParams = new Dictionary<string, string>() {{"Start","0"},{"ItemsPerPage","10"},{"SortColumn","DeviceType"},{"Ascending","true"}};
                    InvalidPCPList = PCPManagementService.GetInvalidPcps(this.IsRefreshData,gridParams, out totalItems);
                }
                NotifyPropertyChanged("Start");
                NotifyPropertyChanged("End");
                NotifyPropertyChanged("TotalItems");
            }
            catch (Exception ex)
            {
                popup(string.Format("An error occurred loading list of invalid PCPs.\n{0}\nStackTrace: {1}",ex.Message,ex.StackTrace), "Error");
            }
        }

    }
}
