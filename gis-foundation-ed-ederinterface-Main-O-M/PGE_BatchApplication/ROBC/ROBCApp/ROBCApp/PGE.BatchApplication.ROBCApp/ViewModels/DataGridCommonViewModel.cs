using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PGE.BatchApplication.ROBCService;

namespace PGE.BatchApplication.ROBCApp
{
    public abstract class DataGridCommonViewModel :BaseViewModel, INotifyPropertyChanged
    {
        public DataGridCommonViewModel()
        {
            
        }


        
        
        protected int start = 0;
        protected int itemCount = 10;
        
        protected int totalItems = 0;
        private ICommand firstCommand;
        private ICommand previousCommand;
        private ICommand nextCommand;
        private ICommand lastCommand;



        

        /// <summary>
        /// Gets the index of the first item in the ROBCs list.
        /// </summary>
        public int Start { get { return start + 1; } }

        /// <summary>
        /// Gets the index of the last item in the ROBCs list.
        /// </summary>
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }

        /// <summary>
        /// The number of total items in the data store.
        /// </summary>
        public int TotalItems { get { return totalItems; } }

        /// <summary>
        /// Gets the command for moving to the first page of ROBCs.
        /// </summary>
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new RelayCommand
                    (
                      param =>
                      {
                          start = 0;
                          RefreshDataGrid(param);
                      },
                      param =>
                      {
                          return start - itemCount >= 0 ? true : false;
                      }
                    );
                }

                return firstCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the previous page of ROBCs.
        /// </summary>
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new RelayCommand
                    (
                      param =>
                      {
                          start -= itemCount;
                          RefreshDataGrid(param);
                      },
                      param =>
                      {
                          return start - itemCount >= 0 ? true : false;
                      }
                    );
                }

                return previousCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the next page of ROBCs.
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new RelayCommand
                    (
                      param =>
                      {
                          start += itemCount;
                          RefreshDataGrid(param);
                      },
                      param =>
                      {
                          return start + itemCount < totalItems ? true : false;
                      }
                    );
                }

                return nextCommand;
            }
        }

        /// <summary>
        /// Gets the command for moving to the last page of ROBCs.
        /// </summary>
        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new RelayCommand
                    (
                      param =>
                      {
                          start = (totalItems / itemCount - 1) * itemCount;
                          start += totalItems % itemCount == 0 ? 0 : itemCount;
                          RefreshDataGrid(param);
                      },
                      param =>
                      {
                          return start + itemCount < totalItems ? true : false;
                      }
                    );
                }

                return lastCommand;
            }
        }

        public abstract void RefreshDataGrid(object param);
        
        
        //protected void NotifyPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

    }
}
