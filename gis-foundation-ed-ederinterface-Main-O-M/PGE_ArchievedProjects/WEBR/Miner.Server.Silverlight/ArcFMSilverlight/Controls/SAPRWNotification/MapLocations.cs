using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace ArcFMSilverlight
{
    public class MapLocations : INotifyPropertyChanged//, IDataErrorInfo 
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string NotificationId { get; set; }
        public int MapLocationNum { get; set; }

        #region MapLocation
        private string _MapLocation;
        public string MapLocation
        {
            get { return _MapLocation; }
            set
            {                
                Regex rxAllowed = new Regex("(?:[^a-z0-9 /,-.'#@()]|(?<=[`~])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                _MapLocation = rxAllowed.Replace(value, String.Empty);
                if (value.Length > 50)
                {
                    throw new ValidationException("Map Location can contain maximum of 50 chars");
                }
                else if (_MapLocation.Length != value.Length)
                {
                    throw new ValidationException("Invalid character entered." + " Allowed only /,-.'#()");
                }
                else if (_MapLocation.Length == 0)
                {
                    throw new ValidationException("Map Location is required!");
                }  
              
                _MapLocation = value;
                ValidateAttriutes();
                RaisePropertyChanged("MapLocation");
            }
        }
        #endregion

        #region Comments
        private string _Comments;
        public string Comments
        {
            get { return _Comments; }
            set
            {                
                Regex rxAllowed = new Regex("(?:[^a-z0-9 /,-.'#@()]|(?<=[`~])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                _Comments = rxAllowed.Replace(value, String.Empty);
                if (value.Length > 150)
                {
                    throw new ValidationException("Comments can not be more than 150 chars");
                }
                else if (_Comments.Length != value.Length)
                {
                    throw new ValidationException("Invalid character entered." + " Allowed only /,-.'#()");
                }                
                _Comments = value;
                ValidateAttriutes();
                RaisePropertyChanged("Comments");
            }
        }
        #endregion

        #region MapCorrectionType
        private string _MapCorrectionType;
        public string MapCorrectionType
        {
            get { return _MapCorrectionType; }
            set
            {
                
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("MapCorrectionType");
                }
                _MapCorrectionType = value;
                ValidateAttriutes();
                RaisePropertyChanged("MapCorrectionType");
            }
        }
        #endregion

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MapJobId { get; set; }
        public string[] FileName { get; set; }
        //public string MapCorrectionType { get; set; }
        public string Server { get; set; }
        public List<File> Attachments { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        
        private bool _isMapAttached;
        public bool IsMapAttached
        {
            get {
                return _isMapAttached;
            }
            set{
                _isMapAttached = value;
                ValidateAttriutes();
                RaisePropertyChanged("IsMapAttached");
                
            }
        }

        public bool IsControlEnable { get; set; }

        private void ValidateAttriutes()
        {
            if (!string.IsNullOrEmpty(_MapLocation) && !string.IsNullOrEmpty(_MapCorrectionType)
                && _isMapAttached)
            {
                IsControlEnable = true;
            }
            else
                IsControlEnable = false;
        }
    }
}
