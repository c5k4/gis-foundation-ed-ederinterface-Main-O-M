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

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit;
using System.Xml.Linq;
using System.Windows.Browser;

namespace ArcFMSilverlight
{
    public partial class PrintPreview : ChildWindow
    {
        private string _gridLayerService = null;
        private string _gridFieldName = string.Empty;
        private int _gridMaxRecords = 0;
                
        
        #region Private Fields

        private List<Tuple<string, string, string, string, string, string>> _sessionGridLayers;
        private Map _map;
        private Graphic _selectedGrid;

        #endregion Private Fields

        public PrintPreview()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PrintPreview_Loaded);
        }

        void PrintPreview_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Resources.Contains("MapTypes") || 
                !Application.Current.Resources.Contains("GridLayers") || 
                !Application.Current.Resources.Contains("ScaleOptions"))
            {
                ReadTemplatesConfig();
            }
            
            PopulateTemplateSelectionComboBox();
            comboTemplateSelection.SelectionChanged += new SelectionChangedEventHandler(comboTemplateSelection_SelectionChanged);
            
            PopulateGridLayerSelectionComboBox();
            comboGridLayerSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridLayerSelection_SelectionChanged);

            comboGridNumberSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridNumberSelection_SelectionChanged);
        }

        private void ReadTemplatesConfig()
        {
            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("TemplatesConfig"))
                {
                    string config = Application.Current.Host.InitParams["TemplatesConfig"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement xe = XElement.Parse(config);
                    
                    if (!xe.HasElements) return;
                    else
                    {
                        List<Tuple<string, List<Tuple<string, string, string>>>> mapConfigurations = new List<Tuple<string, List<Tuple<string, string, string>>>>();

                        var PageTemplateNodes = from pts in xe.Descendants("PageTemplates")
                                                select pts;

                        foreach (XElement PageTemplateNode in PageTemplateNodes.Elements())
                        {
                            string templateType = PageTemplateNode.Attribute("MapType").Value;

                            // List<MapService, ProxyUrl>
                            List<Tuple <string, string, string>> templateTypeLayers = new List<Tuple<string, string, string>>();

                            foreach (XElement LayerNode in PageTemplateNode.Elements())
                            {
                                string mapService = LayerNode.Attribute("MapService").Value;
                                string proxyUrl = null;

                                if (!string.IsNullOrEmpty((string)LayerNode.Attribute("ProxyUrl")))
                                {
                                    proxyUrl = LayerNode.Attribute("ProxyUrl").Value;
                                }
                                string serviceType = string.IsNullOrEmpty((string)LayerNode.Attribute("Type")) ? string.Empty : LayerNode.Attribute("Type").Value;
                                Tuple<string, string, string> layerServiceAndProxy = new Tuple<string, string, string>(mapService, proxyUrl, serviceType);
                                templateTypeLayers.Add(layerServiceAndProxy);
                            }

                            // List<MapType,List<MapService, ProxyUrl>>
                            Tuple<string, List<Tuple<string, string, string>>> mapConfiguration = new Tuple<string, List<Tuple<string, string, string>>>(templateType, templateTypeLayers);

                            mapConfigurations.Add(mapConfiguration);
                        }

                        Application.Current.Resources.Add("MapTypes", mapConfigurations);


                        List<Tuple<string, string, string, string, string, string>> gridLayerOptions = new List<Tuple<string, string, string, string, string, string>>();

                        var GridLayerNodes = from gln in xe.Descendants("GridLayers")
                                             select gln;

                        foreach (XElement GridLayerNode in GridLayerNodes.Elements())
                        {
                            string name = GridLayerNode.Attribute("Name").Value;
                            string service = GridLayerNode.Attribute("MapService").Value;
                            string layerId = GridLayerNode.Attribute("LayerId").Value;
                            string scale = GridLayerNode.Attribute("DefaultScale").Value;
                            string fieldName = GridLayerNode.Attribute("Field").Value;
                            string maxRecords = GridLayerNode.Attribute("MaxRecords").Value;

                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(service))
                            {
                                MessageBox.Show("Configuration error: Grid Layer Name and MapService cannot be null.");
                            }
                            else
                            {
                                Tuple<string, string, string, string,string, string> GridLayerItem = new Tuple<string, string, string, string,string, string>(name, service, scale, fieldName, layerId, maxRecords);
                                gridLayerOptions.Add(GridLayerItem);
                            }
                        }

                        Application.Current.Resources.Add("GridLayers", gridLayerOptions);

                        List<Tuple<string, string>> scaleOptions = new List<Tuple<string, string>>();

                        var ScaleOptionNodes = from son in xe.Descendants("ScaleOptions")
                                               select son;

                        foreach (XElement ScaleOptionNode in ScaleOptionNodes.Elements())
                        {
                            string displayText = ScaleOptionNode.Attribute("DisplayText").Value;
                            string value = ScaleOptionNode.Attribute("Value").Value;
                            Tuple<string, string> ScaleOption = new Tuple<string, string>(displayText, value);
                            scaleOptions.Add(ScaleOption);                            
                        }

                        Application.Current.Resources.Add("ScaleOptions", scaleOptions);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        void comboTemplateSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboTemplateSelection.SelectedIndex != -1)
            {
                SetPrintPreviewEnabled();                
            }
        }

        void comboGridLayerSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string gridLayerID = string.Empty;
            if (comboGridLayerSelection.SelectedIndex == -1) return;
            else
            {
                string gridLayerName = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag.ToString();
                string gridLayerScale = null;

                for (int i = 0; i < _sessionGridLayers.Count(); i++)
                {
                    if (_sessionGridLayers[i].Item1 == gridLayerName)
                    {
                        _gridLayerService = _sessionGridLayers[i].Item2;
                        gridLayerScale = _sessionGridLayers[i].Item3;
                        _gridFieldName = _sessionGridLayers[i].Item4;
                        gridLayerID = _sessionGridLayers[i].Item5;
                        int.TryParse(_sessionGridLayers[i].Item6, out _gridMaxRecords) ;

                        if (Application.Current.Resources.Contains("SelectedGridLayerScale")) Application.Current.Resources.Remove("SelectedGridLayerScale");
                        if (Application.Current.Resources.Contains("SelectedGridLayerService")) Application.Current.Resources.Remove("SelectedGridLayerService");

                        Application.Current.Resources.Add("SelectedGridLayerScale", gridLayerScale);
                        Application.Current.Resources.Add("SelectedGridLayerService", _gridLayerService);

                        txtBlkScaleLabel.Visibility = Visibility.Visible;
                        txtBlkScaleLabel.Text = "Grid Scale: (1:" + (Convert.ToInt32(gridLayerScale) / 12) + ")";

                        break;
                    }
                }
            }


            if (!string.IsNullOrEmpty(_gridLayerService))
            {
                _gridLayerService += "/" + gridLayerID;
                PopulateGridNumberSelectionComboBox(_gridLayerService);
            }
            
            
            SetPrintPreviewEnabled();
        }

        void comboGridNumberSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboGridLayerSelection.SelectedIndex == -1) return;
            else
            {
                string selectedGridNumber = comboGridNumberSelection.SelectedItem.ToString();
                QueryTask queryTask = new QueryTask(_gridLayerService);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(QueryGetSelectedGrid_ExecuteCompleted);
                queryTask.Failed += new EventHandler<TaskFailedEventArgs>(QueryGetSelectedGrid_Failed);

                Query query = new Query();
                query.OutFields.Add("*");
                query.Where = _gridFieldName + " ='" + selectedGridNumber + "'";
                query.ReturnGeometry = true;

                queryTask.ExecuteAsync(query);

                SetPrintPreviewEnabled();
            }
        }
        #region Private Methods

        private void PopulateTemplateSelectionComboBox()
        {
            //List<Tuple<string, List<string>>> sessionMapTypes = (List<Tuple<string, List<string>>>)Application.Current.Resources["MapTypes"];
            List<Tuple<string, List<Tuple<string, string, string>>>> sessionMapTypes = (List<Tuple<string, List<Tuple<string, string, string>>>>)Application.Current.Resources["MapTypes"];
            
            if (sessionMapTypes.Count > 0 || sessionMapTypes != null)
            {
                comboTemplateSelection.Items.Add(new ComboBoxItem { Content = "Ad Hoc" });
                
                foreach (var mapType in sessionMapTypes)
                {
                    comboTemplateSelection.Items.Add(new ComboBoxItem { Content = mapType.Item1 });
                }
            }
        }

        private void PopulateGridLayerSelectionComboBox()
        {
            _sessionGridLayers = (List<Tuple<string, string, string, string, string, string>>)Application.Current.Resources["GridLayers"];

            if (_sessionGridLayers.Count > 0 || _sessionGridLayers != null)
            {
                for (int i = 0; i < _sessionGridLayers.Count(); i++)
                {
                    comboGridLayerSelection.Items.Add(new ComboBoxItem 
                    { 
                        Content = _sessionGridLayers[i].Item1 + " (1:" + _sessionGridLayers[i].Item3 + ")",
                        Tag = _sessionGridLayers[i].Item1 
                    });
                }
            }           
        }

        void QueryGetSelectedGrid_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            FeatureSet featureSet = e.FeatureSet;

            if (featureSet == null || featureSet.Features.Count < 1)
            {
                MessageBox.Show("Error retrieving Grid Cell");
            }
            else
            {
                _selectedGrid = featureSet.Features[0];

                if (Application.Current.Resources.Contains("SelectedCell")) Application.Current.Resources.Remove("SelectedCell");

                Application.Current.Resources.Add("SelectedCell", _selectedGrid);
                
                //Expose Grid Number for display in page templates.
                string gridNumber = (string)(comboGridNumberSelection.SelectedItem.ToString());
                if (Application.Current.Resources.Contains("SelectedCellNumber")) Application.Current.Resources.Remove("SelectedCellNumber");
                Application.Current.Resources.Add("SelectedCellNumber", gridNumber);
            }
        }

        void QueryGetSelectedGrid_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("QueryGetSelectedGrid Failed.");
        }

        private void PopulateGridNumberSelectionComboBox(string gridServiceEndpoint)
        {
            QueryTask queryTask = new QueryTask(gridServiceEndpoint);
            queryTask.ExecuteCompleted += QueryGridNumbers_ExecuteCompleted;
            queryTask.Failed += QueryGridNumbers_Failed;

            Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutFields.AddRange(new string[] { _gridFieldName });
            query.Geometry = Map.Extent;
            query.ReturnGeometry = true;
            query.OutSpatialReference = Map.SpatialReference;
            //if (!string.IsNullOrEmpty(_gridMaxRecords.ToString()))
            
            // Definition queries cause this where clause to fail.   
            //query.Where = "ROWNUM <= " + _gridMaxRecords.ToString();

            queryTask.ExecuteAsync(query);
            Cursor = Cursors.Wait;
        }

        private void QueryGridNumbers_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            Cursor = Cursors.Arrow;
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet == null || featureSet.Features.Count < 1)
            {
                MessageBox.Show("No Grid Number features returned from query");
            }

            /*
             * Need to find a way to alert the user to zoom in to reduce results under the threshold.
             * MessageBox is not working.
             * 
             * else if (featureSet.Features.Count >= _gridMaxRecords)
            {
                MessageBox.Show("Sorry! Your selection exceeded the Maximum Records Limit. \nPlease zoom in to your Area of Interest, and try again.", "Max Records Limit Crossed!", MessageBoxButton.OK);                            
                CancelButton_Click(CancelButton, new RoutedEventArgs());                
            }*/

            else
            {
                List<string> gridNumbers = new List<string>();
                foreach (Graphic feature in featureSet.Features)
                {
                    gridNumbers.Add(feature.Attributes[_gridFieldName].ToString());
                }

                comboGridNumberSelection.ItemsSource = gridNumbers;                 
            }
        }

        private void QueryGridNumbers_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Grid Number Query Failed: " + args.Error);
        }

        private void SetPrintPreviewEnabled()
        {
            if (comboTemplateSelection.SelectedIndex != -1)
            {
                bool isAdHoc = (((ComboBoxItem)comboTemplateSelection.SelectedItem).Content.ToString() == "Ad Hoc") ? true : false;

                if (isAdHoc)
                {
                    PrintPreviewButton.IsEnabled = true;

                    comboGridLayerSelection.SelectedIndex = -1;
                    comboGridLayerSelection.IsEnabled = false;

                    if (txtBlkScaleLabel.Visibility == Visibility.Visible)
                    {
                        txtBlkScaleLabel.Text = "";
                        txtBlkScaleLabel.Visibility = Visibility.Collapsed;
                    }

                    comboGridNumberSelection.SelectedIndex = -1;
                    comboGridNumberSelection.ItemsSource = null;
                    comboGridNumberSelection.IsEnabled = false;
                }
                else
                {
                    EnableGridLayerDropDowns();
                    PrintPreviewButton.IsEnabled =
                            (comboGridLayerSelection.SelectedIndex != -1 && comboGridNumberSelection.SelectedIndex != -1)
                            ? true
                            : false;
                }
            }
        }

        private void EnableGridLayerDropDowns()
        {
            comboGridLayerSelection.IsEnabled = true;
            comboGridNumberSelection.IsEnabled = true;
        }

        #endregion Private Methods

        #region Public Properties

        public Map Map
        {
            get { return _map; }
            set { _map = value;  }
        }
            
        public string MapTypeSelection
        {
            get { return ((ComboBoxItem)comboTemplateSelection.SelectedItem).Content.ToString(); }
        }

        public Graphic SelectedGrid
        {
            get { return _selectedGrid; }
            set { _selectedGrid = value; }
        }

        #endregion Public Properties

        #region Private Events

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PrintPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion Private Events
    }
}