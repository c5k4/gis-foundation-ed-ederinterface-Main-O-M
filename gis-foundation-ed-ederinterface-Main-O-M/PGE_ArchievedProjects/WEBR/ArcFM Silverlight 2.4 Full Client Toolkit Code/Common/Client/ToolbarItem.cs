using System.ComponentModel;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// This is anabstract class which clients can derive from
    /// to place items on a toolbar.
    /// </summary>
    public abstract class ToolbarItem: INotifyPropertyChanged
    {
        #region declarations
        private bool _isEnabled;
        #endregion declarations

        #region abstract methods/properties
        /// <summary>
        /// Name of the tool.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Contant to associate the tool with
        /// (e.g. an icon to display on a toolbar).
        /// </summary>
        public abstract object Content { get; set; }

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mapService"></param>
        public abstract void ExecuteAsync(Map map, string mapService);

        /// <summary>
        /// Cancels the running command.
        /// </summary>
        public abstract void CancelAsync();
        #endregion abstract methods/properties

        #region public properties
        /// <summary>
        /// Is the tool/command enabled?
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("Enabled");
            }
        }
        #endregion public properties

        #region overrides
        /// <summary>
        /// The string value of the tool/command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
        #endregion overrides

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion INotifyPropertyChanged
    }
}
