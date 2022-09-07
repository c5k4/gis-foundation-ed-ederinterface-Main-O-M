using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PdfSharp.Pdf.AcroForms;
using System.ServiceModel;

namespace ArcFMSilverlight
{
    public partial class JobReserveOperatingNumber : UserControl
    {
        private JetRonTool _jetRonTool;
        private string _endpointaddress = string.Empty;

        public Dictionary<Int16, JetEquipmentType> EquipmentIdTypeValues
        {
            set
            {
                cboEquipmentType.ItemsSource = value;
                cboEquipmentType.DisplayMemberPath = "Value";
                cboEquipmentType.SelectedIndex = 0;
            }
        }

        public JobReserveOperatingNumber(JetRonTool jetRonTool)
        {
            _jetRonTool = jetRonTool;
            InitializeComponent();
            _jetRonTool.Initialize();
            _endpointaddress = _jetRonTool.GetWCFEndPointAddress();  //WEBR Stability - Replace Feature Service with WCF
        }

        private void SaveContinueButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveContinueButton.IsEnabled = false;
            BusyIndicator.IsBusy = true;
            BusyIndicator.UpdateLayout();
            _jetRonTool.GenerateIds();
            if (chkLastLocation.IsChecked == true)
            {
                //_jetRonTool.GetLastEquipLoc();     //To Populate location of last reserved equipment - INC000004150734
                //call WCF to fetch last equipment location
                EndpointAddress endPoint = new EndpointAddress(_endpointaddress);
                BasicHttpBinding httpbinding = new BasicHttpBinding();
                httpbinding.MaxBufferSize = 2147483647;
                httpbinding.MaxReceivedMessageSize = 2147483647;
                httpbinding.TransferMode = TransferMode.Buffered;
                httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
                IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

                try
                {
                    cusservice.BeginGetJetLastEquipLocation(_jetRonTool.GetSelectedJobNumber(),
                    delegate(IAsyncResult result)
                    {
                        jetequipmentlist lastEquipLocResult = ((IServicePons)result.AsyncState).EndGetJetLastEquipLocation(result);
                        this.Dispatcher.BeginInvoke(
                        delegate()
                        {
                            //populate last equipment location
                            if (lastEquipLocResult.Address != null)
                            {
                                TxtAddress.Text = Convert.ToString(lastEquipLocResult.Address);
                                TxtCity.Text = Convert.ToString(lastEquipLocResult.City);
                                TxtLatitude.Text = Convert.ToString(lastEquipLocResult.LATITUDE);
                                TxtLongitude.Text = Convert.ToString(lastEquipLocResult.LONGITUDE);
                            }
                        }
                        );
                    }, cusservice);
                }
                catch
                {
                }
            }
        }


        private bool Form1IsValid
        {
            get
            {
                return cboEquipmentType.SelectedIndex > 0;
            }
        }

        private bool InstallTypeIsValid
        {
            get
            {
                return cboInstallType.SelectedIndex > 0;
            }
        }

        private bool SketchLocationIsValid
        {
            get
            {
                int numIsValid;
                return TxtSketchLocation.Text.Length > 0 &&
                            Int32.TryParse(TxtSketchLocation.Text, out numIsValid);
                ;
            }
        }

        private bool LatitudeIsValid
        {
            get
            {
                return TxtLatitude.Text.Length > 0 && TxtLatitude.Text != "0";
            }
        }
        private bool LongitudeIsValid
        {
            get
            {
                return TxtLongitude.Text.Length > 0 && TxtLongitude.Text != "0";
            }
        }

        private bool AddressIsValid
        {
            get
            {
                return TxtAddress.Text.Length > 0;
            }
        }
        private bool CityIsValid
        {
            get
            {
                return TxtCity.Text.Length > 0;
            }
        }

        private bool OperatingNumIsValid
        {
            get
            {
                return TxtOperatingNumber.Text != null &&
                        TxtOperatingNumber.Text.Length > 1;
            }
        }

        private bool Cgc12IsValid
        {
            get
            {
                return TxtCgc12.Text != null &&
                        TxtCgc12.Text.Length > 1;
            }
        }

        private bool Form2IsValid
        {
            get
            {
                return (OperatingNumIsValid || Cgc12IsValid) &&
                    InstallTypeIsValid &&
                    SketchLocationIsValid &&
                    LatitudeIsValid &&
                    LongitudeIsValid &&
                    AddressIsValid &&
                    CityIsValid;
            }
        }

        public bool SaveAndContinue
        {
            set
            {
                if (value)
                {
                    grdOperatingNumberParts2.Visibility = Visibility.Collapsed;
                    stpSaveContinue.Visibility = Visibility.Visible;
                    cboEquipmentType.IsEnabled = true;
                    chkCustomerOwned.IsEnabled = true;
                    SaveContinueButton.IsEnabled = true;
                    chkLastLocation.IsEnabled = _jetRonTool.ChkLastLocEnability();    // to disable/enable use last equip location checkbox - INC000004150734
                }
                else
                {
                    grdOperatingNumberParts2.Visibility = Visibility.Visible;
                    stpSaveContinue.Visibility = Visibility.Collapsed;
                    cboEquipmentType.IsEnabled = false;
                    chkCustomerOwned.IsEnabled = false;
                    chkLastLocation.IsEnabled = false; // to disable use last equip location checkbox - INC000004150734
                }
            }
            get
            {
                return cboEquipmentType.IsEnabled;
            }
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton messageBoxButton = MessageBoxButton.OK)
        {
            return MessageBox.Show(message, caption, messageBoxButton);
        }


        private void SetMapLocationButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetRonTool.EnableLocationCapture();

            const int FONT_SIZE = 28;
            SetMapLocationTtTb.FontSize = FONT_SIZE;
            SetMapLocationTtTb.Text = "Click the Map to get Location";
            ((ToolTip)ToolTipService.GetToolTip((DependencyObject)sender)).IsOpen = true;

            DispatcherTimer closeMapLocationToolTip = new DispatcherTimer();
            closeMapLocationToolTip.Interval = new TimeSpan(0, 0, 0, 0, 10000); // 20 seconds or when they click the map
            closeMapLocationToolTip.Tick += new EventHandler(closeMapLocationToolTip_Tick);
            closeMapLocationToolTip.Start();

        }

        void closeMapLocationToolTip_Tick(object sender, EventArgs e)
        {
            CloseSetMapLocationTtTb();
        }

        public void CloseSetMapLocationTtTb()
        {
            SetMapLocationToolTip.IsOpen = false;
            SetMapLocationTtTb.FontSize = 11;
            SetMapLocationTtTb.Text = "Click Set Location and then Click the Map";
        }

        private void ResetMapLocationButton_OnClick(object sender, RoutedEventArgs e)
        {
            TxtAddress.Text = TxtCity.Text = TxtLatitude.Text = TxtLongitude.Text = "";
            CloseSetMapLocationTtTb();
            _jetRonTool.ClearGraphics();
        }

        public void SetForm1Valid()
        {
            SaveContinueButton.IsEnabled = Form1IsValid;
        }

        public void SetForm2Valid()
        {
            OkButton.IsEnabled = Form2IsValid;
        }

        private void CboEquipmentType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Form1IsValid)
            {
                cboEquipmentType.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                cboEquipmentType.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }

            if (cboEquipmentType.SelectedIndex > 0 &&
                ((KeyValuePair<short, JetEquipmentType>)cboEquipmentType.SelectedItem).Value.HasOperatingNumberBool)
            {
                chkCustomerOwned.IsEnabled = true;
            }
            else
            {
                chkCustomerOwned.IsEnabled = false;
            }

            SetForm1Valid();

        }


        private void CboInstallType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InstallTypeIsValid)
            {
                cboInstallType.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                cboInstallType.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();

        }

        private void JobReserveOperatingNumber_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelButton2_OnClick(null, null);
            }
            if (e.Key == Key.Enter)
            {
                if (SaveAndContinue && SaveContinueButton.IsEnabled)
                {
                    SaveContinueButton_OnClick(null, null);
                }
                else if (OkButton.IsEnabled)
                {
                    OkButton_OnClick(null, null);
                }
            }

        }

        private void CancelButton2_OnClick(object sender, RoutedEventArgs e)
        {
            _jetRonTool.IsActive = false;
            this.SaveAndContinue = true;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetRonTool.SaveEquipment();
        }

        private void TxtSketchLocation_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SketchLocationIsValid)
            {
                TxtSketchLocation.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TxtSketchLocation.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();
        }

        private void TxtAddress_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (AddressIsValid)
            {
                TxtAddress.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TxtAddress.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();
        }

        private void TxtCity_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CityIsValid)
            {
                TxtCity.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                TxtCity.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();
        }

        private void TxtLatitude_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (LatitudeIsValid)
            {
                TxtLatitude.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                TxtLatitude.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();
        }

        private void TxtLongitude_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (LongitudeIsValid)
            {
                TxtLongitude.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                TxtLongitude.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 224));
            }
            SetForm2Valid();
        }

        private void TxtSketchLocation_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab) return;

            // Handle Shift case
            if (Keyboard.Modifiers == ModifierKeys.Shift || e.Key == Key.Tab)
            {
                e.Handled = true;
            }

            // Handle all other cases
            if (!e.Handled && (e.Key < Key.D0 || e.Key > Key.D9))
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if (e.Key != Key.Back)
                    {
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
