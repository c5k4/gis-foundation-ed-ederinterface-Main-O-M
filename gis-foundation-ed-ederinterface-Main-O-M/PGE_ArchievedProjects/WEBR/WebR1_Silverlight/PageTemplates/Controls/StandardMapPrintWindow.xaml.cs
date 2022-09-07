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

namespace PageTemplates
{
    public partial class StandardMapPrintWindow : ChildWindow
    {
        #region declarations

        private List<Tuple<string, string, string, Tuple<string, string, string, string, string>>> _sessionGridLayers;
        private string _scaleFieldName;
        private string _regionFieldName;
        private string _divisionFieldName;
        private string _districtFieldName;
        private string _gridnumberFieldName;
        private string _serviceAreaMapServiceUrl = "";
        private int _serviceAreaLayerId = -1;
        private IDictionary<string, object> _serviceAreaAttributes = null;
        List<IdentifyResult> _gridNumberResults = null;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region ctor

        public StandardMapPrintWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PrintPreview_Loaded);
        }

        #endregion ctor

        #region events

        void PrintPreview_Loaded(object sender, RoutedEventArgs e)
        {
            //Changes by Girish: When switching between Standard and AdHoc printing it was not populating the 
            //combo boxes, because it was bypassing ReadTemplatesConfig(). and not clearing AppResources.
            //if (!Application.Current.Resources.Contains("MapTypes") ||
            //    !Application.Current.Resources.Contains("GridLayers") ||
            //    !Application.Current.Resources.Contains("ScaleOptions"))
            //{
            //    ReadTemplatesConfig();
            //}

            ClearAppResource("MapTypes");
            ClearAppResource("GridLayers");
            ClearAppResource("ScaleOptions");
            ClearAppResource("PDFRootUrl");
            ClearAppResource("PDFMaps");

            ReadTemplatesConfig();

            comboTemplateSelection.SelectionChanged += new SelectionChangedEventHandler(comboTemplateSelection_SelectionChanged);
            PopulateTemplateSelectionComboBox();

            comboGridLayerSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridLayerSelection_SelectionChanged);
            PopulateGridLayerSelectionComboBox();

            comboGridNumberSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridNumberSelection_SelectionChanged);
        }

        void GetServiceAreaidentifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            List<IdentifyResult> results = new List<IdentifyResult>();


            if (e.IdentifyResults == null)
            {
                _serviceAreaAttributes = null;
            }
            else
            {

                results = e.IdentifyResults;

                foreach (IdentifyResult result in results)
                {
                    _serviceAreaAttributes = result.Feature.Attributes;
                }
            }

            EnableDisableAll(true);

        }
        void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            if (e.IdentifyResults != null)
            {
                comboGridNumberSelection.ItemsSource = null;
                _gridNumberResults = e.IdentifyResults;

                FilterGridNumbers();

                EnableDisableAll(true);
            }            
        }

        void identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            comboGridNumberSelection.Items.Clear();
            EnableDisableAll(true);
        }

        void comboTemplateSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboTemplateSelection.SelectedIndex < 0) return;

            //comboPageSize.Items.Clear();
            comboPageSize.ItemsSource = null;

            List<Tuple<string, string, string>> pdfMaps = (List<Tuple<string, string, string>>)Application.Current.Resources["PDFMaps"];
            var selectedTemplateItem = comboTemplateSelection.SelectedValue as ComboBoxItem;

            var templateList = from t in pdfMaps
                               where t.Item1.ToString() == selectedTemplateItem.Content.ToString()
                               select t;


            if ((templateList == null) || (templateList.Count() <= 0))
            {
                ObservableCollection<string> sizesListObs = new ObservableCollection<string>();
                sizesListObs.Add("24x36");
                comboPageSize.ItemsSource = sizesListObs;
            }
            else
            {
                var selectedTemplate = templateList.FirstOrDefault();

                var sizeList = selectedTemplate.Item2;
                List<string> sizesList = new List<string>();
                foreach (string size in sizeList.Split(new char[] { ',', ';' }))
                {
                    sizesList.Add(size);
                }
                sizesList.Sort();
                ObservableCollection<string> sizesListObs = new ObservableCollection<string>(sizesList);
                comboPageSize.ItemsSource = sizesListObs;                
            }

            comboPageSize.SelectedIndex = 0;

            // Filter Grid Numbers
            if (_gridNumberResults != null)
            {
                FilterGridNumbers();
            }            
        }

        void comboGridNumberSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGridNumber = comboGridNumberSelection.SelectedValue as ComboBoxItem;
            if (selectedGridNumber == null) return;

            var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
            if (gridData == null) return;

            //Call to Identify on Service Area Layer
            GetOverlappingServiceAreaInfo(selectedGridNumber.Tag as Graphic);


            EnableDisablePrintButton();
        }

        void comboGridLayerSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
            if (selected == null) return;

            EnableDisableAll(false);
            SetGridLayerFieldNames(selected);

            var identifyParameters = new IdentifyParameters();
            identifyParameters.Geometry = CurrentMap.Extent;

            var layerId = GetLayerId(selected);
            if (layerId < 0) return;

            identifyParameters.LayerIds.Add(layerId);
            identifyParameters.MapExtent = CurrentMap.Extent;
            identifyParameters.LayerOption = LayerOption.visible;
            identifyParameters.Height = (int)CurrentMap.ActualHeight;
            identifyParameters.Width = (int)CurrentMap.ActualWidth;
            identifyParameters.SpatialReference = CurrentMap.SpatialReference;
            identifyParameters.Tolerance = 5;
            
            var url = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item2;
            var identifyTask = new IdentifyTask(url);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
            identifyTask.ExecuteAsync(identifyParameters);

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PrintStandardMapButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGridNumber = comboGridNumberSelection.SelectedValue as ComboBoxItem;
            var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
            var selectedTemplate = comboTemplateSelection.SelectedValue as ComboBoxItem;
            var size = comboPageSize.SelectedValue.ToString();
            var template = comboTemplateSelection.SelectedValue as ComboBoxItem;

            //SCALE
            //REGION
            //Division
            //DISTRICT
            //Plat Number

            //Run Checks on Availability of Configured Attributes
            if (!(_serviceAreaAttributes.ContainsKey(_regionFieldName)) && (_serviceAreaAttributes.ContainsKey(_divisionFieldName)) && (_serviceAreaAttributes.ContainsKey(_districtFieldName)))
            {
                string sMessage = "Configured Attributes are not present on the Service Area Layer -- Please Check Configuration";

                logger.Warn(sMessage);
                MessageBox.Show(sMessage);
                return;
            }

            //Run Checks on the Grid Layer for availability of configured Attributes
            if (!(gridData.ContainsKey(_scaleFieldName)) && (gridData.ContainsKey(_gridnumberFieldName)))
            {
                string sMessage = "Configured Attributes are not present on the Grid Layer -- Please Check Configuration";

                logger.Warn(sMessage);
                MessageBox.Show(sMessage);
                return;
            }


            string url = Application.Current.Resources["PDFRootUrl"] + "/"
                         + template.Content + "-"
                         + gridData[_scaleFieldName] + "/"
                         + _serviceAreaAttributes[_regionFieldName] + "/"
                         + _serviceAreaAttributes[_divisionFieldName] + "/"
                         + _serviceAreaAttributes[_districtFieldName] + "/"
                         + template.Content + "_"
                         + gridData[_gridnumberFieldName] + "_"
                         + size + "_"
                         + gridData[_scaleFieldName] + ".pdf";

            //string url = @"http://vm-pgeweb101.miner.com/PDFMaps/DistributionMap-500/CentralValley/Kern/Kern/DistributionMap_2926274_24x36_500.pdf";
            Uri uri = new Uri(url);
            HtmlPage.Window.Navigate(uri, "_blank");
        }

        #endregion events

        #region properties

        public Map CurrentMap { get; set; }

        #endregion properties

        #region methods

        private void ClearAppResource(string resourceName)
        {
            if (Application.Current.Resources.Contains(resourceName))
                Application.Current.Resources.Remove(resourceName);
        }

        private int GetLayerId(ComboBoxItem selected)
        {
            if (selected == null) return -1;

            var layerIdString = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item3;
            int layerId = -1;
            if (Int32.TryParse(layerIdString, out layerId) == true)
            {
                return layerId;
            }
            return -1;
        }

        private void SetGridLayerFieldNames(ComboBoxItem selected)
        {
            if (selected == null) return;
            //(selected.Tag as Tuple<string, string, string, string, string, string, Tuple<string, string, string, string, string>>).Item2;

            Tuple<string, string, string, string, string> fieldElements = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item4;

            _scaleFieldName = fieldElements.Item1;
            _regionFieldName = fieldElements.Item2;
            _divisionFieldName = fieldElements.Item3;
            _districtFieldName = fieldElements.Item4;
            _gridnumberFieldName = fieldElements.Item5;

        }

        private void GetOverlappingServiceAreaInfo(Graphic inGraphic)
        {

            var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
            if ((selected == null) || (inGraphic == null))
            {
                string sMessage = "No Selected Grid Feature";
                logger.Warn(sMessage);
                return;
            }

            if ((_serviceAreaLayerId < 0) || (_serviceAreaMapServiceUrl.Length < 1))
            {
                string sMessage = "Check Configuration: Templates.config -- Service Area Resource configuration are incorrect";
                logger.Warn(sMessage);
                return;
            }

            EnableDisableAll(false);
            SetGridLayerFieldNames(selected);

            var identifyParameters = new IdentifyParameters();
            identifyParameters.Geometry = inGraphic.Geometry;

            var layerId = _serviceAreaLayerId; 
            if (layerId < 0) return;

            identifyParameters.LayerIds.Add(layerId);
            identifyParameters.MapExtent = CurrentMap.Extent;
            identifyParameters.LayerOption = LayerOption.visible;
            identifyParameters.Height = (int)CurrentMap.ActualHeight;
            identifyParameters.Width = (int)CurrentMap.ActualWidth;
            identifyParameters.SpatialReference = CurrentMap.SpatialReference;
            identifyParameters.Tolerance = 5;
            identifyParameters.ReturnGeometry = false;

            var url = _serviceAreaMapServiceUrl; 
            var identifyTask = new IdentifyTask(url);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(GetServiceAreaidentifyTask_ExecuteCompleted);
            identifyTask.ExecuteAsync(identifyParameters);

        }
        private void EnableDisableAll(bool isEnabled)
        {
            IsEnabled = isEnabled;
            EnableDisablePrintButton();

            if (isEnabled == false)
            {
                BusyIndicator.BusyContent = "Loading...";
                BusyIndicator.IsBusy = true;
            }
            else
            {
                BusyIndicator.IsBusy = false;
            }
        }

        private void FilterGridNumbers()
        {
            //comboGridNumberSelection.Items.Clear();
            comboGridNumberSelection.ItemsSource = null;
            List<ComboBoxItem> gridNumberCBIs = new List<ComboBoxItem>();

            foreach (IdentifyResult gridNumberResult in _gridNumberResults)
            {
                var attributes = gridNumberResult.Feature.Attributes;

                string resultScaleFilter = attributes[_scaleFieldName].ToString();//SCALE
                ComboBoxItem cbi = comboTemplateSelection.SelectedItem as ComboBoxItem;
                string cbiScales = cbi.Tag.ToString();
                string[] cbiScalesArray = cbiScales.Split(',');

                if (cbiScalesArray.Contains(resultScaleFilter.ToString()))
                {
                    var resultValue = attributes[_gridnumberFieldName];//Plat Number
                    if (resultValue != null)
                    {
                        //comboGridNumberSelection.Items.Add(new ComboBoxItem
                        //{
                        //    Content = resultValue,
                        //    Tag = gridNumberResult.Feature
                        //});
                        gridNumberCBIs.Add(new ComboBoxItem { Content = resultValue, Tag = gridNumberResult.Feature });                        
                    }
                }
            }

            if (gridNumberCBIs.Count > 0)
            {
                gridNumberCBIs.Sort((x, y) => string.Compare(x.Content.ToString(), y.Content.ToString()));
                comboGridNumberSelection.ItemsSource = gridNumberCBIs; 
                comboGridNumberSelection.SelectedIndex = 0;               
            }
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
                    
                    List<Tuple<string, string, string, Tuple<string, string, string, string, string>>> gridLayerOptions = new List<Tuple<string, string, string, Tuple<string, string, string, string, string>>>();

                    XElement _gle = xe.Element("GridLayers");
                    if (_gle != null)
                    {
                        XAttribute _xatt = _gle.Attribute("ServiceAreaMapService");
                        if (_xatt != null)
                        {
                            _serviceAreaMapServiceUrl = _xatt.Value;
                        }
                        else
                        {
                            _serviceAreaMapServiceUrl = "";
                        }

                        _xatt = _gle.Attribute("ServiceAreaLayerId");

                        if (_xatt != null)
                        {
                            _serviceAreaLayerId = Convert.ToInt32(_xatt.Value.ToString());
                        }
                        else
                        {
                            _serviceAreaLayerId = -1;
                        }
                    }
                        
                    var GridLayerNodes = from gln in xe.Descendants("GridLayers")
                                            select gln;
                        
                    foreach (XElement GridLayerNode in GridLayerNodes.Elements())
                    {
                        string name = GridLayerNode.Attribute("Name").Value;
                        string service = GridLayerNode.Attribute("MapService").Value;
                        string layerId = GridLayerNode.Attribute("LayerId").Value;
                            
                        string scalefieldname = GridLayerNode.Attribute("ScaleFieldName").Value;//7
                        string regionfieldname = GridLayerNode.Attribute("RegionFieldName").Value;//8
                        string divisionfieldname = GridLayerNode.Attribute("DivisionFieldName").Value;//9
                        string districtfieldname = GridLayerNode.Attribute("DistrictFieldName").Value;//10
                        string gridnumberfieldname = GridLayerNode.Attribute("GridNumberFieldName").Value;//11

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(service))
                        {
                            MessageBox.Show("Configuration error: Grid Layer Name and MapService cannot be null.");
                        }
                        else
                        {
                            Tuple<string, string, string, string, string> gridlayerFields = new Tuple<string, string, string, string, string>(scalefieldname, regionfieldname, divisionfieldname, districtfieldname, gridnumberfieldname);
                            Tuple<string, string, string, Tuple<string, string, string, string, string>> GridLayerItem = new Tuple<string, string, string, Tuple<string, string, string, string, string>>(name, service, layerId, gridlayerFields);
                                
                            gridLayerOptions.Add(GridLayerItem);
                        }
                    }

                    Application.Current.Resources.Add("GridLayers", gridLayerOptions);

                    List<Tuple<string, string, string>> pdfMaps = new List<Tuple<string, string, string>>();

                    var PDFMapsNodes = from pdfm in xe.Descendants("PDFMaps")
                                        select pdfm;
                    string rooturl = xe.Element("PDFMaps").Attribute("RootUrl").Value;

                    foreach (var pdfmapNode in PDFMapsNodes.Elements())
                    {
                        string mapType = pdfmapNode.Attribute("MapType").Value;
                        string mapSize = pdfmapNode.Attribute("MapSize").Value;
                        string gridCellScaleFilters = pdfmapNode.Attribute("GridNumberScaleFilters").Value;
                        Tuple<string, string, string> pdfMap = new Tuple<string, string, string>(mapType, mapSize, gridCellScaleFilters);
                        pdfMaps.Add(pdfMap);
                    }

                    if (pdfMaps.Count > 0) pdfMaps.Sort();
                    Application.Current.Resources.Add("PDFRootUrl", rooturl);
                    Application.Current.Resources.Add("PDFMaps", pdfMaps);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void PopulateTemplateSelectionComboBox()
        {
            comboTemplateSelection.Items.Clear();

            List<Tuple<string, string, string>> pdfMaps = (List<Tuple<string, string, string>>)Application.Current.Resources["PDFMaps"];

            if (pdfMaps != null && pdfMaps.Count > 0)
            {
                foreach (var pdfMap in pdfMaps)
                {
                    //comboTemplateSelection.Items.Add(new ComboBoxItem { Content = pdfMap.Item1, Tag = pdfMap.Item2 });
                    comboTemplateSelection.Items.Add(new ComboBoxItem { Content = pdfMap.Item1, Tag = pdfMap.Item3 });
                }

                if (comboTemplateSelection.Items.Count > 0)
                {
                    comboTemplateSelection.SelectedIndex = 0;                    
                }
            }
        }

        private void PopulateGridLayerSelectionComboBox()
        {
            comboGridLayerSelection.Items.Clear();

            _sessionGridLayers = (List<Tuple<string, string, string, Tuple<string, string, string, string, string>>>)Application.Current.Resources["GridLayers"];

            if (_sessionGridLayers != null && _sessionGridLayers.Count > 0)
            {
                List<ComboBoxItem> gridLayerCBIs = new List<ComboBoxItem>();
                for (int i = 0; i < _sessionGridLayers.Count(); i++)
                {
                    gridLayerCBIs.Add(new ComboBoxItem { Content = _sessionGridLayers[i].Item1, Tag = _sessionGridLayers[i]});
                }

                if (gridLayerCBIs.Count > 0)
                {
//                    gridLayerCBIs.Sort((x, y) => string.Compare(x.Content.ToString(), y.Content.ToString()));
                    comboGridLayerSelection.ItemsSource = gridLayerCBIs;
                    comboGridLayerSelection.SelectedIndex = 0;
                }
            }
        }

        private void EnableDisablePrintButton()
        {
            if ((comboTemplateSelection.SelectedIndex >= 0) &&
                (comboGridLayerSelection.SelectedIndex >= 0) &&
                (comboGridNumberSelection.SelectedIndex >= 0) &&
                (comboPageSize.SelectedIndex >= 0))
            {
                PrintStandardMapButton.IsEnabled = true;
            }
            else
            {
                PrintStandardMapButton.IsEnabled = false;
            }
        }

        #endregion methods
    }
}

