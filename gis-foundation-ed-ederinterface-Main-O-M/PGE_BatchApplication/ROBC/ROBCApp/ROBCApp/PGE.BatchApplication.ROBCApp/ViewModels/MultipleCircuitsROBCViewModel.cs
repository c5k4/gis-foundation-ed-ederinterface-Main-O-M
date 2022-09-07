using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.BatchApplication.ROBCService;
using System.Collections.ObjectModel;
using PGE.BatchApplication.ROBCService;
//using PGE.BatchApplication.ROBCService.Common;

namespace PGE.BatchApplication.ROBCApp
{
    /// <summary>
    /// View model for Circuit search with Substation Name and Circuit Name
    /// </summary>
    public class MultipleCircuitsROBCViewModel : DataGridCommonViewModel, IViewModel
    {
        private const string PAGE_NAME = "MULTIPLE_ROBC";
        private ROBCManagementService _robcService;

        public MultipleCircuitsROBCViewModel() { _robcService = new ROBCManagementService(); }

        public MultipleCircuitsROBCViewModel(string subStationName, string circuitName)
        {
            _robcService = new ROBCManagementService();
            _robcService.PopulateMultipleCircuitROBCs(subStationName, circuitName);
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

        private ROBCModel _selectedROBC;
        public ROBCModel SelectedROBC
        {
            get
            {
                return _selectedROBC;
            }
            set
            {
                if (object.ReferenceEquals(_selectedROBC, value) != true)
                {
                    _selectedROBC = value;
                    NotifyPropertyChanged("SelectedROBC");
                }
            }
        }


        private ObservableCollection<ROBCModel> _multipleCircuitROBCList;
        public ObservableCollection<ROBCModel> MultipleCircuitROBCList
        {
            get
            {
                return _multipleCircuitROBCList;
            }
            private set
            {
                if (object.ReferenceEquals(_multipleCircuitROBCList, value) != true)
                {
                    _multipleCircuitROBCList = value;
                    NotifyPropertyChanged("MultipleCircuitROBCList");
                }
            }
        }
        /// <summary>
        /// Refreshes the list of ROBCs. Called by navigation commands.
        /// </summary>
        public override void RefreshDataGrid(object pageName)
        {

            string sortColumn = "CircuitId";
            bool ascending = true;
            if (pageName.ToString() == PAGE_NAME)
            {
                MultipleCircuitROBCList = _robcService.GetMultipleCircuitROBCs(start, itemCount, sortColumn, ascending, out totalItems);
            }
            NotifyPropertyChanged("Start");
            NotifyPropertyChanged("End");
            NotifyPropertyChanged("TotalItems");
        }

    }
}
