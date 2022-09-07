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
using Miner.Server.Client.Toolkit;

//ENOS2EDGIS Start, “Changes made for Service Location Grid More Info 16/05/2017” 
using ArcFMSilverlight.Controls.Generation;
using ESRI.ArcGIS.Client.Tasks;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client;
//ENOS2EDGIS, showing Gen Type Domain values
using ESRI.ArcGIS.Client.FeatureService;
//********************************************

namespace ArcFMSilverlight.Controls.DeviceSettings
{
    public class DeviceSettings : IDataGridCustomButton
    {
        //        private AttributesViewerControl _attributesViewerControl;
        public Button _deviceSettingsButton = new Button();

        //        private DataGrid _attributeDataGrid;
        public Dictionary<string, Dictionary<string, DeviceSettingsLayer>> _servicesLayersSubtypes = new Dictionary<string, Dictionary<string, DeviceSettingsLayer>>();
        public Dictionary<string, string> _deviceSettingsLayerMappings = new Dictionary<string, string>();
        private string _deviceSettingsURL;

        private Miner.Server.Client.Tasks.ResultSet _resultSet;
        private IDictionary<string, object> _attributes;
        private string _globalID;
        private string _layerName;
        private string _serviceURL;
        private bool _visible;
        //private bool _enabled;

        //****ENOS2EDGIS Start, “Changes made for Service Location Grid More Info 16/05/2017”***
        private string _servicePointURL;
        private string _generationInfoURL;
        private string _objectID;
        private List<Graphic> generations = new List<Graphic>();
        private List<Graphic> servicePointsOnSL = new List<Graphic>();
        private int genrationQueryCounter = 0;
        private int servicePointFeatureCount;
        //*****************************************
        public IDictionary<string, object> Attributes
        {
            set
            {
                _attributes = value;
            }
        }
        public string GlobalID
        {
            set
            {
                _globalID = value;
            }
        }
        public bool Visible
        {
            get
            {
                return _visible;
            }
        }

        //ENOS2EDGIS Start, Add a public get only variable for the device settings URL
        public string DeviceSettingURL
        {
            get
            {
                return _deviceSettingsURL;
            }
        }
        //*********ENOS2EDGIS End*************************

        // bug in esri http://support.esri.com/en/bugs/nimbus/TklNMDkxNDMy -- NIM091432 so we are not sending subtypecd

        public bool ShowMe { get { return false; } }

        void SetDeviceSettingsEnabled(string layerName)
        {


            if (SelectionIsDeviceSettingsEnabled(layerName))
            {
                _deviceSettingsButton.IsEnabled = true;
                _layerName = layerName;
                SetURLPropertiesFromSelection();
            }
            else
            {
                _deviceSettingsButton.IsEnabled = false;
            }

        }

        void SetURLPropertiesFromSelection()
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

            //**************ENOS2EDGIS Start, “Changes made for Service Location Grid More Info 16/05/2017” *******
            if (_layerName == "Service Location")
            {
                _objectID = "";
                _objectID = _attributes["OBJECTID"].ToString();
            }
            //*****************ENOS2EDGIS End**************************************
        }

        bool SelectionIsDeviceSettingsEnabled(string layerName)
        {
            bool selectionIsDeviceSettingsEnabled = SelectionIsDeviceSettingsEnabled(_resultSet.Service, layerName);

            // If it's not then let's check the mappings
            if (!selectionIsDeviceSettingsEnabled)
            {
                if (_deviceSettingsLayerMappings.ContainsKey(_resultSet.Service))
                {
                    _serviceURL = _deviceSettingsLayerMappings[_resultSet.Service];
                    selectionIsDeviceSettingsEnabled = SelectionIsDeviceSettingsEnabled(_serviceURL, _resultSet.Name);
                }
            }

            return selectionIsDeviceSettingsEnabled;
        }

        public bool SelectionIsDeviceSettingsEnabled(string serviceURL, string layerName)
        {
            if (_servicesLayersSubtypes.ContainsKey(serviceURL))
            {
                if (_servicesLayersSubtypes[serviceURL].ContainsKey(layerName))
                {
                    _serviceURL = serviceURL;
                    return true;
                }
            }

            _serviceURL = "";
            return false;
        }

        public void ReadConfiguration(XElement element)
        {
            _deviceSettingsURL = element.Element("DeviceSettingsApp").Attribute("URL").Value;
            _visible = Convert.ToBoolean(element.Attribute("Visible").Value);
            //********ENOS2EDGIS Start,“Changes made for Service Location Grid More Info 16/05/2017” *****************
            _servicePointURL = element.Element("DeviceSettingServicePoint").Attribute("URL").Value;
            _generationInfoURL = element.Element("DeviceSetingGenInfo").Attribute("URL").Value;
            //***************************ENOS2EDGIS End************************************************************

            foreach (XElement dssElement in element.Element("DeviceSettingsServices").Elements())
            {
                string serviceURL = dssElement.Attribute("URL").Value;

                Dictionary<string, DeviceSettingsLayer> deviceSettingsLayers = new Dictionary<string, DeviceSettingsLayer>();
                foreach (XElement dssLayerElement in dssElement.Element("DeviceLayers").Elements())
                {
                    DeviceSettingsLayer deviceSettingsLayer = new DeviceSettingsLayer();
                    deviceSettingsLayer.LayerName = dssLayerElement.Attribute("Name").Value;
                    deviceSettingsLayers.Add(dssLayerElement.Attribute("Name").Value, deviceSettingsLayer);
                }
                _servicesLayersSubtypes.Add(serviceURL, deviceSettingsLayers);
            }

            foreach (XElement dlMappingElement in element.Element("DeviceLayerMappings").Elements())
            {
                _deviceSettingsLayerMappings.Add(dlMappingElement.Attribute("SourceURL").Value, dlMappingElement.Attribute("DestURL").Value);
            }
        }


        void _deviceSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        public void OpenSettings()
        {
            OpenSettings(_globalID, _layerName);
        }

        public void Open(string globalId, string layerName = "")
        {
            OpenSettings(globalId, layerName);
        }
        public void OpenSettings(string globalID, string layerName)
        {
            string layerType = "";
            if (layerName.Contains("Closed") || layerName.Contains("Opened"))
            {
                if (layerName.Contains("Closed"))
                {
                    layerType = "C";
                }
                else if (layerName.Contains("Opened"))
                {
                    layerType = "O";
                }

            }

            else
            {
                layerType = "";
            }

            //ENOS2EDGIS Start, “Changes made for Service Location Grid More Info 16/05/2017**********
            if (layerName == "Service Location")
            {
                getServicePoint();

            }
            else
            {
                HtmlPage.Window.Navigate(new Uri(BuildURL(globalID, layerName, layerType)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
            }
            //ENOS2EDGIS, Below line commented
            //  HtmlPage.Window.Navigate(new Uri(BuildURL(globalID, layerName, layerType)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
            //************************************ENOS2EDGIS End**************************************************
        }


        string BuildURL(string globalID, string layerName, string layerType)
        {


            string url;
            url = BuildSettingsURL(globalID, layerName, layerType);


            if (layerName.ToUpper().Contains("OVERHEAD CONDUCTOR") || layerName.ToUpper().Contains("UNDERGROUND CONDUCTOR"))
            {
                url = url.Replace(@"/home/integration", "/conductor/specialload");
            }
            // Removed per Hitest 7/22
            //else if (layerName.ToUpper().Contains("PRIMARY METER") || layerName.ToUpper() == "PRIMARY GENERATION")
            //{
            //    url = url.Replace(@"/home/integration", "/primarymeter/specialload");
            //}

            return url;
        }

        string BuildSettingsURL(string globalID, string layerName, string layerType)
        {
            string url = _deviceSettingsURL + "?";
            if (layerName.Contains("Closed") || layerName.Contains("Opened"))
            {
                string[] lyname = layerName.Split('-');
                layerName = lyname[0].ToString().Trim();
                if (layerName.Contains("Closed"))
                {
                    layerType = "C";
                }
                else if (layerName.Contains("Opened"))
                {
                    layerType = "O";
                }

                url += "globalID=" + HttpUtility.UrlEncode(globalID) + "&";
                url += "layerName=" + HttpUtility.UrlEncode(layerName) + "&";
                url += "layerType=" + HttpUtility.UrlEncode(layerType) + "&";
                url = url.TrimEnd('&');

            }
            else
            {
                url += "globalID=" + HttpUtility.UrlEncode(globalID) + "&";
                url += "layerName=" + HttpUtility.UrlEncode(layerName) + "&";

                url = url.TrimEnd('&');
            }

            return url;
        }

        public Button CreateButton()
        {
            TextBlock txtword = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "S",//for text that will be display               
                Foreground = new SolidColorBrush(Colors.Red),//color of text
                FontSize = 16,//size of text
                FontFamily = new FontFamily("Arial"),//font type for text
                FontWeight = FontWeights.Bold
            };

            _deviceSettingsButton.Height = 30;
            _deviceSettingsButton.Width = 30;
            _deviceSettingsButton.Margin = new Thickness(0, 2, 0, 2);
            _deviceSettingsButton.Content = txtword;
            _deviceSettingsButton.HorizontalAlignment = HorizontalAlignment.Center;
            _deviceSettingsButton.VerticalAlignment = VerticalAlignment.Center;

            _deviceSettingsButton.Click += new RoutedEventHandler(_deviceSettingsButton_Click);
            _deviceSettingsButton.IsEnabled = false;
            _deviceSettingsButton.Visibility = Visibility.Collapsed;

            return _deviceSettingsButton;
        }

        public void SetEnabled(string layerName)
        {

            SetDeviceSettingsEnabled(layerName);
        }


        public Button UnderlyingButton
        {
            get { return _deviceSettingsButton; }
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

        public bool IsManuallyEnabled { get; set; }

        public string Name
        {
            get
            {
                return "Settings...";
            }
        }


        public Action ButtonClicked
        {
            get
            {
                return OpenSettings;
            }
        }

        //**************ENOS2EDGIS Start,“Changes made for Service Location Grid More Info 16/05/2017” *********
        void getServicePoint()
        {
            Query query = new Query(); //query on service point layer          
            query.OutFields.AddRange(new string[] { "GLOBALID", "STREETNUMBER","STREETNAME1","STREETNAME2","STATE", "COUNTY", "OBJECTID"});
            query.Where = "SERVICELOCATIONGUID='" + _globalID + "'";
            QueryTask queryTask = new QueryTask(_servicePointURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
            queryTask.ExecuteAsync(query);

        }

        void spQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if ((e.FeatureSet).Features.Count > 0)
            {
                genrationQueryCounter = 0;
                servicePointFeatureCount = (e.FeatureSet).Features.Count;
               
                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    string _servicePtGlobalID = ((e.FeatureSet).Features[i].Attributes)["GLOBALID"].ToString();
                    getGenerationInfo(_servicePtGlobalID);
                    servicePointsOnSL.Add((e.FeatureSet).Features[i]);
                }

            }
            else
            {
                MessageBox.Show("No Generations are attached to this service location", "No Generations Found", MessageBoxButton.OK);
            }
        }

        void getGenerationInfo(string _servicePtGlobalID)
        {
            //ENOS2EDGIS, showing Gen Type Domain values 
            GetGenTypeDomainValues();
            Query query = new Query(); //query on geneartion info layer
            query.ReturnGeometry = true;
            query.OutFields.AddRange(new string[] { "*" });
            query.Where = "SERVICEPOINTGUID='" + _servicePtGlobalID + "'";
            QueryTask queryTask = new QueryTask(_generationInfoURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(genInfoQueryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(genInfoQueryTask_ExecuteFailed);
            queryTask.ExecuteAsync(query);
        }

        //ENOS2EDGIS, showing Gen Type Domain values 
        private void GetGenTypeDomainValues()
        {
            FeatureLayer pFeatureLayer = new FeatureLayer();
            pFeatureLayer.Url = _generationInfoURL;
            pFeatureLayer.Initialized += new EventHandler<EventArgs>(GenInfoFeatureLayer_Initialized);
            pFeatureLayer.Initialize();
        }
        private void GenInfoFeatureLayer_Initialized(object sender, System.EventArgs e)
        {
            FeatureLayer _featureLayer = sender as FeatureLayer;
            FeatureLayerInfo pLayerInfo = _featureLayer.LayerInfo;
            foreach (ESRI.ArcGIS.Client.Field field in pLayerInfo.Fields)
            {
                if (field.Domain != null)
                {
                    if (field.Name.ToUpper() == "GENTYPE")
                    {
                        if (field.Domain is CodedValueDomain)
                        {
                            ConfigUtility.DivisionCodedDomains = field.Domain as CodedValueDomain;
                        }
                    }
                }
            }
        }
        //*******************ENOS2EDGIS End*********************
        void genInfoQueryTask_ExecuteFailed(object sender, TaskFailedEventArgs e)
        {
            genrationQueryCounter++;
            if (genrationQueryCounter == servicePointFeatureCount)
            {
                showGenInfoData(generations);
                genrationQueryCounter = 0;
                generations.Clear();
            }
        }

        void genInfoQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            genrationQueryCounter++;
            if ((e.FeatureSet).Features.Count > 0)
            {                     
               generations.Add((e.FeatureSet).Features[0]);
            }
            if (genrationQueryCounter == servicePointFeatureCount)
            {
                if (generations.Count > 1)
                {
                    foreach (var graphic in generations)
                    {
                        foreach (var spGraphic in servicePointsOnSL)
                        {
                            if (graphic.Attributes["SERVICEPOINTGUID"].ToString().Replace("}", "").Replace("{", "").ToUpper() == spGraphic.Attributes["GLOBALID"].ToString().Replace("}", "").Replace("{", "").ToUpper())
                            {
                                graphic.Attributes["STREETNAME1"] = spGraphic.Attributes["STREETNAME1"];
                                graphic.Attributes["STREETNAME2"] = spGraphic.Attributes["STREETNAME2"];
                                graphic.Attributes["STATE"] = spGraphic.Attributes["STATE"];
                                graphic.Attributes["COUNTY"] = spGraphic.Attributes["COUNTY"];
                                graphic.Attributes["STREETNUMBER"] = spGraphic.Attributes["STREETNUMBER"];
                                graphic.Attributes["SPID"] = spGraphic.Attributes["OBJECTID"];
                            }
                        }
                    }
                }
                showGenInfoData(generations);
                genrationQueryCounter = 0;
                generations.Clear();
            }
        }

        void showGenInfoData(IList<Graphic> generations)
        {
            //var x = 0;
            if (generations.Count == 0)
            {
                MessageBox.Show("No Generations are attached to this service location", "No Generations Found", MessageBoxButton.OK);
            }

            else if (generations.Count == 1)
            {
                string globalID = generations[0].Attributes["GLOBALID"].ToString();
                string layerName = "EDGIS.GenerationInfo";

                string layerType = "";
                if (layerName.Contains("Closed") || layerName.Contains("Opened"))
                {
                    if (layerName.Contains("Closed"))
                    {
                        layerType = "C";
                    }
                    else if (layerName.Contains("Opened"))
                    {
                        layerType = "O";
                    }
                }

                else
                {
                    layerType = "";
                }
                HtmlPage.Window.Navigate(new Uri(BuildURL(globalID, layerName, layerType)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");

                // HtmlPage.Window.Navigate(new Uri(BuildURL(globalID, layerName)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
            }
            else
            {
                GenerationInfoWindow genInfoWindow = new GenerationInfoWindow();
                genInfoWindow.DeviceSettingURL = _deviceSettingsURL;
                genInfoWindow.ServiceLocationOID = _objectID;
                genInfoWindow.InitializeViewModel(generations);
                genInfoWindow.Closed += new EventHandler(genInfoWindow_Closed);
                genInfoWindow.Show();
            }
        }
        void genInfoWindow_Closed(object sender, EventArgs e)
        {
            GenerationInfoWindow genInfoWindow = sender as GenerationInfoWindow;
        }

        //*****************************ENOS2EDGIS End****************************************************


    }

    // Helper
    public class DeviceSettingsLayer
    {
        public string LayerName;
        public Dictionary<string, int> Subtypes = new Dictionary<string, int>();
    }

}
