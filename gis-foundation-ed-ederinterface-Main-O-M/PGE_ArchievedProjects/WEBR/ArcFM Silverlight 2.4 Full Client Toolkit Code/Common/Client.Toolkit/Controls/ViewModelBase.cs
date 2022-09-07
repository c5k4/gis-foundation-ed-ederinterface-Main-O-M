using System.ComponentModel;
using System.Windows;

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// The base class for ViewModels, provides the basic INotifyPropertyChanged implementation
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies the listeners when a property changed
        /// </summary>
        /// <param name="p"></param>
        protected void OnNotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        /// <summary>
        /// Specifies whether we are in the design mode.
        /// </summary>
        public bool IsDesignTime
        {
            get
            {
                return (Application.Current == null) || (Application.Current.GetType() == typeof(Application));
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Fires when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}
