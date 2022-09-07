using System;
using System.ComponentModel;
using System.Windows;

namespace ArcFMSilverlight.PageTemplates
{
    public class TemplatePresenter : INotifyPropertyChanged
    {
        #region private declarations
        string _mapTitle;
        string _mapType;

        #endregion private declarations

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        public string MapTitle
        {
            get { return _mapTitle; }
            set
            {
                _mapTitle = value;
                OnPropertyChanged("MapTitle");
            }
        }

        public string MapType
        {
            get { return _mapType; }
            set
            {
                _mapType = value;
                OnPropertyChanged("MapType");
            }
        }

        public string PrintedBy
        {
            get 
            {
                if (Application.Current.Resources.Contains("UserName"))
                    return (string)Application.Current.Resources["UserName"];
                else
                    return String.Empty;
            }
        }

        public string PrintedOn
        {
            get { return DateTime.Today.ToShortDateString(); }
        }

        #endregion properties

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
