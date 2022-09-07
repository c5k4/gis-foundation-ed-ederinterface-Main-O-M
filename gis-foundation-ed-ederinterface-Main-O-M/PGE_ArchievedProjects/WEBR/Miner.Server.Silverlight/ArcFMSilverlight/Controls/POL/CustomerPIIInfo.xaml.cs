using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client;
using System.Windows.Browser;
using System.Xml.Linq;
using Miner.Server.Client.Toolkit ;
using Miner.Server.Client.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Printing;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Symbols;

namespace ArcFMSilverlight
{
    public partial class CustomerPIIInfo : UserControl,INotifyPropertyChanged 
    {
        public static CustomerPIIInfo customerPIIInfo = null;
        public event PropertyChangedEventHandler PropertyChanged;


        public CustomerPIIInfo()
        {
            InitializeComponent();
            this.DataContext = this;
            customerPIIInfo = this;
            DisplayResults = false;
        }


        

        public void showCustomerPIIInfo()
        {
            try
            {
                ConfigUtility.StatusBar.Text  = "";
                DisplayResults = true;
            }
            catch (Exception ex)
            {
                ConfigUtility.StatusBar.Text = "Failed to show Customer PII Information.";
            }            
        }

        public void hideCustomerPIIInfo()
        {
            DisplayResults = false;
            setPIIInfoEmpty();
        }

        private void CloseWindowCmd_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "";
            DisplayResults = false;
        }

        private void setPIIInfoEmpty()
        {
            OwnerName = "";
            OwnerStreetAddress = "";
            OwnerStreetAddress2 = "";
            OwnerCity = "";
            OwnerState = "";
            OwnerZip = "";
            OwnerPhone = "";
            POLNumber = "";
            PremiseID = "";
            AgreementDate = "";

            //BEGIN AG modified 20200919   
            //CustomerPIITitle.Text = "Customer PII";
            CustomerPIITitle.Text = "CONFIDENTIAL: FOR INTERNAL PGE USE ONLY";
            //END AG modified 20200919   
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region POL Properties
        private string _OwnerName = "";
        public string OwnerName
        {
            get { return _OwnerName; }
            set
            {
                _OwnerName = value;
                OnPropertyChanged("OwnerName");
            }
        }

        private string _OwnerStreetAddress = "";
        public string OwnerStreetAddress
        {
            get { return _OwnerStreetAddress; }
            set
            {
                _OwnerStreetAddress = value;
                OnPropertyChanged("OwnerStreetAddress");
            }
        }

        private string _OwnerStreetAddress2 = "";
        public string OwnerStreetAddress2
        {
            get { return _OwnerStreetAddress2; }
            set
            {
                _OwnerStreetAddress2 = value;
                OnPropertyChanged("OwnerStreetAddress2");
            }
        }

        private string _OwnerCity = "";
        public string OwnerCity
        {
            get { return _OwnerCity; }
            set
            {
                _OwnerCity = value;
                OnPropertyChanged("OwnerCity");
            }
        }

        private string _OwnerState = "";
        public string OwnerState
        {
            get { return _OwnerState; }
            set
            {
                _OwnerState = value;
                OnPropertyChanged("OwnerState");
            }

        }

        private string _OwnerZip = "";
        public string OwnerZip
        {
            get { return _OwnerZip; }
            set
            {
                _OwnerZip = value;
                OnPropertyChanged("OwnerZip");
            }
        }

        private string _OwnerPhone = "";
        public string OwnerPhone
        {
            get { return _OwnerPhone; }
            set
            {
                _OwnerPhone = value;
                OnPropertyChanged("OwnerPhone");
            }
        }

        private string _POLNumber = "";
        public string POLNumber
        {
            get { return _POLNumber; }
            set
            {
                _POLNumber = value;
                OnPropertyChanged("POLNumber");
            }
        }

        private string _PremiseID = "";
        public string PremiseID
        {
            get { return _PremiseID; }
            set
            {
                _PremiseID = value;
                OnPropertyChanged("PremiseID");
            }
        }
        private string _AgreementDate = "";
        public string AgreementDate
        {
            get { return _AgreementDate; }
            set
            {
                _AgreementDate = value;
                OnPropertyChanged("AgreementDate");
            }
        }
        #endregion

        #region Dependency Properties

        public bool DisplayResults
        {
            get { return (bool)GetValue(DisplayResultsProperty); }
            set { SetValue(DisplayResultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayResults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayResultsProperty =
            DependencyProperty.Register("DisplayResults", typeof(bool), typeof(CustomerPIIInfo), null);


        #endregion
    }
}
