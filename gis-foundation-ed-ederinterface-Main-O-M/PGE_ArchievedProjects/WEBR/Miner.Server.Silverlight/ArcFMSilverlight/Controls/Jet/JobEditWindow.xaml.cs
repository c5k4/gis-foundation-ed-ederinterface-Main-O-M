using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

using NLog;
using System.Collections.ObjectModel;

namespace ArcFMSilverlight
{
    public partial class JobEditWindow : ChildWindow
    {
        private JetJob _jetJob = null;
        private JetJobsTool _jetJobsTool = null;
        #region declarations

        public static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region ctor

        public JobEditWindow()
        {
            
        }

        #endregion ctor

        public void Initialize(ComboBox cboDivision, JetJobsTool jetJobsTool, JetJob jetJob = null)
        {
            _jetJobsTool = jetJobsTool;
            if (jetJob == null)
            {
                // Smelly. TODO: move this
                jetJob = new JetJob() { ReservedBy = jetJobsTool.User, EntryDate = DateTime.Now, Status = JetModel.STATUS_ACTIVE, UserAudit = jetJobsTool.User};
            }
            _jetJob = jetJob;
            InitializeComponent();
            this.DataContext = _jetJob;
            this.cboDivision.ItemsSource = JetModel._divisionValuesSelect;
            if (jetJob.ObjectId > 0)
            {
                this.cboDivision.SelectedItem = new KeyValuePair<int, string>(jetJob.Division, JetModel._divisionValuesSelect[jetJob.Division]);
            }
            else
            {
                this.cboDivision.SelectedItem = cboDivision.SelectedItem;
                if (this.cboDivision.SelectedItem == null)
                {
                    this.cboDivision.SelectedIndex = 0;
                }
            }

            this.cboDivision.DisplayMemberPath = cboDivision.DisplayMemberPath;
            if (_jetJob.JobNumber != null)
                this.TxtJobNumber.Text = _jetJob.JobNumber;
            //Code change-TCS-INC000004186986
            if (_jetJob.Description != null)
                this.TxtDescription.Text = _jetJob.Description;
            else
                this.TxtDescription.Text = "";
            DialogResult = false;
        }

        public JetJob JetJob
        {
            get
            {
                return _jetJob;
            }
        }

        #region events

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            //Code change-TCS-INC000004186986
            JetJob.JobNumber = JetJob.PreviousJobNumber;
        }

        #endregion events

        #region properties

        public Map CurrentMap { get; set; }

        public bool EditExisting
        {
            set
            {
                if (value)
                {
                    //change code by TCS-01/08/2016
                    Title = "Edit Job";
                    TxtJobNumber.IsEnabled = true;
                }
                else
                {
                    Title = "New Job";
                    TxtJobNumber.IsEnabled = true;
                    TxtJobNumber.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
                    TxtDescription.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
                }
            }
        }

        #endregion properties

        #region methods

        private bool JobNumberIsValidFormat
        {
            get
            {
                int numIsValid;
                return TxtJobNumber.Text.Length == 8 &&
                        Int32.TryParse(TxtJobNumber.Text, out numIsValid);
            }
        }

        private bool DescriptionIsValidFormat
        {
            get
            {
                return TxtDescription.Text.Length > 0;
            }
        }

        private bool DivisionIsValid
        {
            get
            {
                return cboDivision.SelectedIndex > 0;
            }
        }

        private bool FormIsValid
        {
            get
            {
                return JobNumberIsValidFormat && 
                    DescriptionIsValidFormat &&
                    DivisionIsValid;                
            }
        }

        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
            BusyIndicator.IsBusy = false;
        }

        #endregion methods

        private void JobEditWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton_Click(null, null);
            }
            if (e.Key == Key.Enter && OkButton.IsEnabled == true)
            {
                OkButton_OnClick(null, null);                
            }
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Check for existing JobNumber
            // this.DialogResult = true will close the form?
            this.BusyIndicator.IsBusy = true;
            _jetJobsTool.ValidateJob(_jetJob);
        }

        public void ResetInternalVariables()
        {
            _jetJob = null;
            cboDivision.ItemsSource = null;
        }
        private void SetFormValid()
        {
            OkButton.IsEnabled = FormIsValid;            
        }

        private void TxtJobNumber_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (JobNumberIsValidFormat)
            {
                TxtJobNumber.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TxtJobNumber.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));                
            }

            SetFormValid();
        }

        private void TxtDescription_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (DescriptionIsValidFormat)
            {
                TxtDescription.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TxtDescription.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }

            SetFormValid();
        }

        private void CboDivision_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DivisionIsValid)
            {
                cboDivision.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                cboDivision.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetFormValid();
        }

    }
}

