using System;
using System.ComponentModel;
using System.Windows;


namespace PageTemplates
{
    public class TemplatePresenter : INotifyPropertyChanged
    {
        #region private declarations
        
        string _mapTitle;
        string _mapType;
        

        /*Lance
        private string _countyName;
        private string _divisionName;
        private string _dateCreated;
        private string _thumbnailSource;
        private string _mapTypeName;
        private string _scaleText;
        private string _gridNumberText;
        */
        #endregion private declarations

        #region Lance Properties
        /*        
        public string CountyName
        {
            get { return _countyName; }
            set 
            { 
                    _countyName = value;
                    OnPropertyChanged("CountyName");
            }
        }

        public string DivisionName
        {
            get { return _divisionName; }
            set 
            { 
                    _divisionName = value;
                    OnPropertyChanged("DivisionName");
            }
        }

        public string DateCreated
        {
            get { return _dateCreated; }
            set
            {
                _dateCreated = value;
                OnPropertyChanged("DateCreated");
            }
        }

        public string ThumbnailSource
        {
            get { return _thumbnailSource; }
            set
            {
                _thumbnailSource = value;
                OnPropertyChanged("ThumbnailSource");
            }
        }

        public string MapTypeName
        {
            get { return _mapTypeName; }
            set
            {
                _mapTypeName = value;
                OnPropertyChanged("MapTypeName");
            }
        }

        public string ScaleText
        {
            get { return _scaleText; }
            set
            {
                _scaleText = value;
                OnPropertyChanged("ScaleText");
            }
        }

        public string GridNumberText
        {
            get { return _gridNumberText; }
            set
            {
                _gridNumberText = value;
                OnPropertyChanged("GridNumberText");
            }
        }
        */
        #endregion Lance Properties

        #region Properties
        
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
                return String.Empty;
            }
        }

        public string PrintedOn
        {
            get { return "Date Created: " + DateTime.Today.ToShortDateString(); }
        }

        #endregion properties


        #region INotifyPropertyChanged Members
        
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

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
