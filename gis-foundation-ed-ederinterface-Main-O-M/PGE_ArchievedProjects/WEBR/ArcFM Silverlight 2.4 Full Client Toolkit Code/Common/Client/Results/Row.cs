using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Represents a dynamic row of data in the attribute viewer
    /// </summary>
    public class Row : INotifyPropertyChanged
    {
        private Graphic _graphic;
        private bool _isExpanded;

        public Row()
        {
            RowToolCommand = new DelegateCommand<string>(RowToolClicked, RowToolCanExecute);
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
        public event EventHandler<RowToolEventArgs> ToolClicked;

        /// <summary>
        /// Graphic associated with the current row
        /// </summary>
        public Graphic Graphic
        {
            get { return _graphic; }
            set
            {
                _graphic = value;
                OnPropertyChanged("Graphic");
            }
        }

        /// <summary>
        /// The brush used to render the graphic on the map
        /// </summary>
        public Brush GraphicBrush { get; set; }

        /// <summary>
        /// Display field of the row
        /// </summary>
        public string DisplayFieldName { get; set; }

        public ICommand RowToolCommand { get; private set; }

        /// <summary>
        /// Work around for Silverlight 4 not having FindAncestor Binding. 
        /// Will be removed with Silverlight 5.
        /// </summary>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set 
            {
                _isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        protected virtual void OnToolClicked(RowToolEventArgs args)
        {
            EventHandler<RowToolEventArgs> handler = ToolClicked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RowToolClicked(string toolName)
        {
            if (toolName != null)
            {
                OnToolClicked(new RowToolEventArgs(this, toolName.ToString()));
            }
        }

        private bool RowToolCanExecute(string toolName)
        {
            bool canExecute = true;

            switch (toolName)
            {
                case "Zoom":
                case "Pan":
                    if (this.Graphic.Geometry == null)
                    {
                        canExecute = false;
                    }
                    break;
                default:
                    canExecute = true;
                    break;
            }

            return canExecute;
        }
    }

    /// <summary>
    /// Event arguments for the current row
    /// </summary>
    public class RowToolEventArgs : EventArgs
    {
        public RowToolEventArgs(Row row, string toolName)
        {
            Row = row;
            ToolName = toolName;
        }

        public Row Row { get; set; }
        public string ToolName { get; set; }
    }
}
