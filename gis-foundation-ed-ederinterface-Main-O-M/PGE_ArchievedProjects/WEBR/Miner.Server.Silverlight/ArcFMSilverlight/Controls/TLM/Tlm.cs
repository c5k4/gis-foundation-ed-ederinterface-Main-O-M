using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ArcFMSilverlight.Controls.DeviceSettings;
using Miner.Server.Client.Toolkit;

namespace ArcFMSilverlight.Controls.Tlm
{
    public class Tlm : IDataGridCustomButton
    {
        private const string FC_TRANSFORMER = "Transformer";
        private const string FC_PRIMARYMETER = "Primary Meter";
        //private AttributesViewerControl _attributesViewerControl;
        public Button _tlmButton = new Button();

        private DataGrid _attributeDataGrid;
        private string _tlmURL;

        private Miner.Server.Client.Tasks.ResultSet _resultSet;
        private IDictionary<string, object> _attributes;
        private string _globalID;
        private bool _visible;

        public IDictionary<string, object> Attributes
        {
            set
            {
                _attributes = value;
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
        }

        public bool ShowMe { get { return false; }}
        public bool IsManuallyEnabled { get; set; }

        // bug in esri http://support.esri.com/en/bugs/nimbus/TklNMDkxNDMy -- NIM091432 so we are not sending subtypecd
        //private string _subtypeCD;

        public bool LayerIsTlm(string layerName)
        {
            //********ENOS2EDGIS Start*******************
            return (layerName == FC_TRANSFORMER || layerName == FC_PRIMARYMETER);
            //********ENOS2EDGIS End*******************
        }

        void SetTlmEnabled(string layerName)
        {

            if (SelectionIsTlmEnabled(layerName))
            {
                _tlmButton.IsEnabled = true;
                SetURLPropertiesFromSelection(layerName);
            }
            else
            {
                _tlmButton.IsEnabled = false;
            }

        }

        void SetURLPropertiesFromSelection(string layerName)
        {
            // Inexplicably, the attributes could be in ESRI format (Field Name) or ArcFM Format (Field Alias)
            // The former is from a Search, the latter from an Identify
            _globalID = ""; // for some reason this is empty attributes[resultSet.GlobalIdFieldName];
            if (_attributes.ContainsKey("Global ID"))
            {
                _globalID = _attributes["Global ID"] as string;
            }
            else if (_attributes.ContainsKey("GLOBALID"))
            {
                _globalID = _attributes["GLOBALID"] as string;
            }
            if (layerName == FC_PRIMARYMETER)
                _globalID += _attributes["CGC12"];
        }

        bool SelectionIsTlmEnabled(string layerName)
        {
            bool selectionIsTlmEnabled = SelectionIsTlmEnabled(_resultSet.Service, layerName);

            return selectionIsTlmEnabled;
        }

        public bool SelectionIsTlmEnabled(string serviceURL, string layerName)
        {
            if (layerName == FC_TRANSFORMER || layerName == FC_PRIMARYMETER)
            {
                return true;
            }
            return false;
        }

        public void ReadConfiguration(XElement element)
        {
            _tlmURL = element.Attribute("URL").Value;
            _visible = Convert.ToBoolean(element.Attribute("Visible").Value);            
        }


        void _TlmButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTlm();
        }

        public void Open(string globalId, string layerName = "")
        {
            OpenTlm(globalId);
        }

        public void OpenTlm()
        {
            OpenTlm(_globalID);
        }

        public void OpenTlm(string globalID)
        {
            //            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(BuildURL()), "_blank", "");//"toolbar=no,location=no,status=no,menubar=no,resizable=yes");            
            HtmlPage.Window.Navigate(new Uri(BuildURL(globalID)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
        }

        string BuildURL(string globalID)
        {
            string url = _tlmURL + "?";
            url += "globalID=" + HttpUtility.UrlEncode(globalID) + "&";
            //url += "serviceURL=" + HttpUtility.UrlEncode(_serviceURL) + "&";

            url = url.TrimEnd('&');

            return url;
        }

        public Button CreateButton()
        {
            TextBlock txtword = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "T",//for text that will be display               
                Foreground = new SolidColorBrush(Colors.Blue),//color of text
                FontSize = 16,//size of text
                FontFamily = new FontFamily("Arial"),//font type for text
                FontWeight = FontWeights.Bold
            };

            _tlmButton.Height = 30;
            _tlmButton.Width = 30;
            _tlmButton.Margin = new Thickness(0, 2, 0, 2);
            _tlmButton.Content = txtword;
            _tlmButton.HorizontalAlignment = HorizontalAlignment.Center;
            _tlmButton.VerticalAlignment = VerticalAlignment.Center;
            _tlmButton.Name = "TLM";
            _tlmButton.Click += new RoutedEventHandler(_TlmButton_Click);
            _tlmButton.IsEnabled = false;
            _tlmButton.Visibility = Visibility.Collapsed;
            //TODO: remove this button altogether

            return _tlmButton;
        }

        public void SetEnabled(string layerName)
        {
            SetTlmEnabled(layerName);
        }
        public Button UnderlyingButton
        {
            get { return _tlmButton; }
        }

        public void SetEnabled(IDictionary<string, object> attributes, Miner.Server.Client.Tasks.ResultSet resultSet, string layerName = "")
        {
            _attributes = attributes;
            _resultSet = resultSet;
            if (layerName == "")
            {
                SetEnabled(_resultSet.Name);
            }
            else
            {
                SetEnabled(layerName);
            }
        }
        public bool IsEnabled
        {
            get
            {
                if (_visible && UnderlyingButton.IsEnabled)
                {
                    return true;
                }
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "Load...";
            }
        }

        public Action ButtonClicked
        {
            get
            {
                return OpenTlm;
            }
        }

    }

}
