using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Printing;
using System.Json;
using System.Windows.Browser;

namespace PageTemplates
{
    public static class TemplateOptions
    {
        private static bool _isStandardMapTypeScaleRatioSet = false;
        private static bool _isAdHocMapTypeScaleRatioSet = false;
        private static double _adHocResolutionRatio = double.NaN;
        private static double _standardResolutionRatio = double.NaN;
        private static double _newResolution = double.NaN;

        private static double _scale;
        private static Map _map;
        private static string _mapType;
        private static string _mapTypeName;
        private static TextBlock _scaleTextBlock = null;
        private static TextBlock _gridNumberTextBlock = null;
        private static string _gridNumber;        

        private static List<Tuple<string, string, string>> _endpoints;
        private static string _standardGridLayerServiceUrl;
        private static double _templateScale = double.NaN;
        private static double _initialAdHocScale = double.NaN;

        private static string _adHocScaleLayerUrl;

        /// <summary>
        /// This method fires when a page template is requested initially.
        /// </summary>
        /// <param name="TemplateMap">Map control</param>
        /// <param name="Name">String representing the selected map type</param>
        public static void TemplateMap_Loaded(Map TemplateMap, string Name)
        {
            _map = TemplateMap;
            _mapTypeName = Name;

            if (Name != "Ad Hoc")
            {
                _mapType = "Standard";
                _endpoints = GetTemplateConfiguration(Name);
                if (_endpoints != null)
                {
                    SetScaleRatio();
                }
            }
            else if (Name == "Ad Hoc")
            {
                _mapType = "Ad Hoc";

                /*********************************/
                /*
                PrintTask printTask = new PrintTask("http://vm-pgeweb101/arcgis/rest/services/Utilities/PrintingTools/GPServer/Export%20Web%20Map%20Task");
                printTask.DisableClientCaching = true;
                printTask.ExecuteCompleted += new EventHandler<PrintEventArgs>(printTask_ExecuteCompleted);
                printTask.GetServiceInfoCompleted += new EventHandler<ServiceInfoEventArgs>(printTask_GetServiceInfoCompleted);
                printTask.GetServiceInfoAsync();
                */
                /*********************************/






                if (_isAdHocMapTypeScaleRatioSet && _adHocResolutionRatio != double.NaN)
                {
                    ScaleAdHocMap();
                    _scaleTextBlock.Text = SetTemplateScaleDisplay();
                }
                else
                {
                    //Ad Hoc First time through
                    foreach (Layer layer in _map.Layers)
                    {
                        if (layer is ArcGISDynamicMapServiceLayer)
                        {
                            _adHocScaleLayerUrl = ((ArcGISDynamicMapServiceLayer)layer).Url;
                            SetScaleRatio();
                            break;
                        }
                    }
                }
            }
        }

        static void printTask_GetServiceInfoCompleted(object sender, ServiceInfoEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show(
                "Would you like to create a printable PDF from this Map Template?", 
                "Create PDF", 
                MessageBoxButton.OKCancel);

            if (mbResult == MessageBoxResult.OK)
            {

            }
        }

        static void printTask_ExecuteCompleted(object sender, PrintEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(e.PrintResult.Url, "_blank");
        }
        

        /// <summary>
        /// This method is fired on subsequent requests for a page template.
        /// </summary>
        /// <param name="TemplateMap">Map Control</param>
        /// <param name="Name">String representing the selected map type</param>
        /// <param name="userSelectedScale">User selected scale for Ad Hoc template type</param>
        public static void TemplateMap_Loaded(Map TemplateMap, string Name, string userSelectedScale)
        {
            _map = TemplateMap;

            if (Name != "Ad Hoc")
            {
                _endpoints = GetTemplateConfiguration(Name);
                if (_endpoints != null)
                {
                    InitializeTemplateLayers();
                }
            }
            else if (Name == "Ad Hoc")
            {
                //Ad Hoc subsequent times through
                ScaleAdHocMap(userSelectedScale);
                _scaleTextBlock.Text = SetTemplateScaleDisplay();
            }
        }

        /// <summary>
        /// This method sets the Scale text Block text that is visible
        /// in printed map templates.
        /// </summary>
        /// <returns>A string  representation of map scale.</returns>
        public static string SetTemplateScaleDisplay()
        {
            string scaleCaptionText = "1 Inch = " + (_templateScale / 12).ToString() + " Feet";
            _templateScale = double.NaN;

            return scaleCaptionText;
        }

        /// <summary>
        /// This method sets the Map Number text Block in the template
        /// </summary>
        public static string SetMapNumberDisplay()
        {
            if (!string.IsNullOrEmpty(_gridNumber) && !string.IsNullOrEmpty(_mapTypeName)) 
            {
                switch (_mapTypeName)
                {
                    case "Circuit":
                        return "CR" + _gridNumber;
                    case "Distribution":
                        return "DIST" + _gridNumber;
                    case "Duct/Block":
                        return "DB" + _gridNumber;
                    case "Schematic":
                        return "SC" + _gridNumber;
                    case "Street Light":
                        return "SL" + _gridNumber;
                    case "Ad Hoc":
                        return "AH";                        
                    default:
                        return null;
                }                
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method sends a request to the Export Map service will return
        /// some JSON elements regarding the map. The returned scale value will 
        /// be utilized in another method to set the resolution ratios for 
        /// standard and ad hoc map types.
        /// </summary>
        private static void SetScaleRatio()
        {
            if (_mapType == "Ad Hoc" && _isAdHocMapTypeScaleRatioSet)
            {
                return;
            }
            else if (_mapType == "Standard" && _isStandardMapTypeScaleRatioSet)
            {
                InitializeTemplateLayers();
            }
            else
            {
                string exportMapUrl = null;

                if (_mapType == "Ad Hoc" && !_isAdHocMapTypeScaleRatioSet)
                {
                    exportMapUrl = _adHocScaleLayerUrl + "/export?bbox=" + HttpUtility.UrlEncode(_map.Extent.ToString()) + "&size=" + _map.ActualWidth + "," + _map.ActualHeight + "&format=png8&f=json";
                }
                else if (_mapType == "Standard" && !_isStandardMapTypeScaleRatioSet)
                {
                    exportMapUrl = _standardGridLayerServiceUrl + "/export?bbox=" + HttpUtility.UrlEncode(_map.Extent.ToString()) + "&size=" + _map.ActualWidth + "," + _map.ActualHeight + "&format=png8&f=json";
                }

                if (!string.IsNullOrEmpty(exportMapUrl))
                {
                    WebClient _wsc = new WebClient();
                    _wsc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(_wsc_DownloadStringCompleted);
                    if (!_wsc.IsBusy && !string.IsNullOrEmpty(exportMapUrl))
                    {
                        _wsc.DownloadStringAsync(new Uri(exportMapUrl));                        
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to set a scaling ratio for the Page Template Viewer.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">A JSON string containing a scale key</param>
        static void _wsc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string json = e.Result;
                var jsonObject = JsonValue.Parse(json) as JsonObject;
                if (jsonObject != null)
                {
                    if (jsonObject.ContainsKey("scale"))
                    {
                        _scale = jsonObject["scale"];
                        double resolutionRatio = _map.Resolution / _scale;

                        if (_mapType == "Ad Hoc")
                        {
                            _adHocResolutionRatio = resolutionRatio;
                            _isAdHocMapTypeScaleRatioSet = true;

                            ScaleAdHocMap();
                            _scaleTextBlock.Text = SetTemplateScaleDisplay();
                        }
                        else if (_mapType == "Standard")
                        {
                            _standardResolutionRatio = resolutionRatio;
                            _isStandardMapTypeScaleRatioSet = true;

                            InitializeTemplateLayers();
                        }
                    }
                }                
            }
            catch
            {
                return;
            }                        
        }

        /// <summary>
        /// This method drives the process of preparing the page template 
        /// viewer for standard / NON ad-hoc map templates.
        /// </summary>
        public static void InitializeTemplateLayers()
        {            
            /*_map.Layers.Clear();
            foreach (Layer l in _map.Layers)
            {
                if (l is ArcGISDynamicMapServiceLayer || l is ArcGISTiledMapServiceLayer || l is ArcGISImageServiceLayer)
                {
                    _map.Layers.Remove(l);
                }
            }*/
            PositionAndScaleMap();
            //AddLayersToTemplateMap(_endpoints);
            _scaleTextBlock.Text = SetTemplateScaleDisplay();
            _gridNumberTextBlock.Text = SetMapNumberDisplay();
            ApplyMapLayerOpacities();
        }

        private static void ApplyMapLayerOpacities()
        {
            Dictionary<string, double> _layerOpacities = new Dictionary<string, double>();

            if (_layerOpacities.Count == 0)
            {
                _layerOpacities = (Dictionary<string, double>)Application.Current.Resources["MapLayerOpacities"];
            }

            if (_layerOpacities.Count != 0)
            {
                foreach (Layer l in _map.Layers)
                {
                    if (_layerOpacities.ContainsKey(l.ID))
                    {
                        l.Opacity = _layerOpacities[l.ID];
                    }
                }
            }
        }
                
        /// <summary>
        /// This method scales the page template viewer map for Ad Hoc maps
        /// on the intial launch of an Ad Hoc map template.
        /// </summary>
        private static void ScaleAdHocMap()
        {
            //_newResolution = _map.Resolution;
            if (System.Double.IsNaN(_initialAdHocScale))
            {
                _initialAdHocScale = _map.Scale;
            }

            //_templateScale = _map.Scale;
            _templateScale = _initialAdHocScale;
            _newResolution = _adHocResolutionRatio * _templateScale;
            LockAndZoomToResolution();
            ApplyMapLayerOpacities();
        }

        /// <summary>
        /// This method scales the page template viewer map for Ad Hoc maps
        /// on subsequent launches of an Ad Hoc map template.
        /// </summary>
        /// <param name="userSelectedScale"></param>
        private static void ScaleAdHocMap(string userSelectedScale)
        {
            if (double.TryParse(userSelectedScale, out _templateScale))
            {
                _newResolution = _adHocResolutionRatio * _templateScale;

                LockAndZoomToResolution();
            }
        }

        /// <summary>
        /// This method prevents the user from changing the scale of all map
        /// template maps. Ad Hoc maps may have their scale changed, but only
        /// to administrator configured options as outlined in Templates.config.
        /// </summary>
        private static void LockAndZoomToResolution()
        {
            _map.MinimumResolution = _newResolution;
            _map.MaximumResolution = _newResolution;

            _map.ZoomToResolution(_newResolution);
        }
        /// <summary>
        /// Adds a new ArcGISDynamicMapServiceLayer to the templatemap with
        /// specific layers set to visible at the DEFAULT _templateScale.
        /// </summary>
        /// <param name="map">The map to apply the layer(s)</param>
        /// <param name="layers">Map service endpoint string and a string of layers
        /// to me made visible</param>
        private static void PositionAndScaleMap()
        {
            if ((Graphic)Application.Current.Resources["SelectedCell"] != null || (string)Application.Current.Resources["SelectedGridLayerScale"] != null)
            {
                Graphic gridCell = (Graphic)Application.Current.Resources["SelectedCell"];
                _gridNumber = (string)Application.Current.Resources["SelectedCellNumber"];
                
                string stringScale = (string)Application.Current.Resources["SelectedGridLayerScale"];
                
                if (double.TryParse(stringScale, out _templateScale))
                {
                    _newResolution = _standardResolutionRatio * _templateScale;
                    _map.Extent = gridCell.Geometry.Extent;

                    LockAndZoomToResolution();
                }
            }
        }

        /// <summary>
        /// This method adds layers to the template viewer map based off of configuration
        /// settings found in the Templates.config file.
        /// </summary>
        /// <param name="layers">A list containing a map service url and an optional proxy url</param>
        private static void AddLayersToTemplateMap(List<Tuple<string, string, string>> layers)
        {            
            for (int i = 0; i < layers.Count; i++)
            {
                Layer newLayer = null;
                switch (layers[i].Item3)
                {
                    case "ArcGISTiledMapServiceLayer":
                        {
                            newLayer = new ArcGISTiledMapServiceLayer()
                            {
                                Url = layers[i].Item1,
                                ProxyURL = string.IsNullOrEmpty(layers[i].Item2) ? null : layers[i].Item2
                            };
                            break;
                        }
                    //case when type is ArcGISDynamicMapServiceLayer
                    default:
                        {
                            newLayer = new ArcGISDynamicMapServiceLayer()
                            {
                                Url = layers[i].Item1,
                                ProxyURL = string.IsNullOrEmpty(layers[i].Item2) ? null : layers[i].Item2
                            };
                            break;
                        }
                }

                //_map.Layers.Add(newLayer);
                _map.Layers.Insert(i, newLayer);
            }
        }

        /// <summary>
        /// This scales the size of the Page Template to fit within the
        /// screen view of the PageTemplate viewer.
        /// </summary>
        /// <param name="controlWidth">Width of the Template</param>
        /// <param name="controlHeight">Height of the Template</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        /// <returns></returns>
        public static ScaleTransform TransformTemplateScale(double controlWidth, double controlHeight, double screenWidth, double screenHeight)
        {
            ScaleTransform scaledDimensions = new ScaleTransform();
            //scaledDimensions.CenterX = screenWidth / 2;

            double newWidthPercentage = (screenWidth * 0.8) / controlWidth;
            double newHeightPercentage = (screenHeight * 0.8) / controlHeight;
            double scalePercent = newWidthPercentage < newHeightPercentage ? newWidthPercentage : newHeightPercentage;
            if (scalePercent > 1.0) scalePercent = 1.0;

            //scaledDimensions.CenterX = ((screenWidth / 2) - ((controlWidth * scalePercent) / 2));
            scaledDimensions.CenterX = 0;
            scaledDimensions.CenterY = 0;          

            scaledDimensions.ScaleX = scalePercent;
            scaledDimensions.ScaleY = scalePercent;

            return scaledDimensions;
        }

        public static void ScaleTemplate(object sender)
        {
            UserControl uc = sender as UserControl;
            double ucWidth = uc.Width;
            double ucHeight = uc.Height;

            string testW = HtmlPage.Window.Eval("screen.availWidth").ToString();
            string testH = HtmlPage.Window.Eval("screen.availHeight").ToString();

            double width = Convert.ToDouble((string)HtmlPage.Window.Eval("screen.availWidth").ToString());
            double height = Convert.ToDouble((string)HtmlPage.Window.Eval("screen.availHeight").ToString());

            ScaleTransform st = TransformTemplateScale(ucWidth, ucHeight, width, height);
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(st);

            uc.RenderTransform = transformGroup;
        }

        /// <summary>
        /// This method reads in configuration settings from the Templates.config
        /// file and gets a map service endpoint string as well as a comma
        /// separated string of layer ids to make visible within the template.
        /// </summary>
        /// <returns>A Tuple containing the map service string and the string
        /// of layer ids.</returns>
        public static List<Tuple<string, string, string>> GetTemplateConfiguration(string Name)
        {
            try
            {
                if (Application.Current.Resources.Contains("SelectedGridLayerService"))
                    _standardGridLayerServiceUrl = (string)Application.Current.Resources["SelectedGridLayerService"];

                if (!Application.Current.Resources.Contains("MapTypes")) return null;
                else
                {
                    List<Tuple<string, List<Tuple<string, string, string>>>> templateComponents = (List<Tuple<string, List<Tuple<string, string, string>>>>)Application.Current.Resources["MapTypes"];

                    List<Tuple<string, string, string>> layerServices = new List<Tuple<string, string, string>>();

                    for (int i = 0; i < templateComponents.Count; i++)
                    {
                        if (templateComponents[i].Item1 == Name)
                        {
                            List<Tuple<string, string, string>> urls = templateComponents[i].Item2;

                            for (int j = 0; j < urls.Count; j++)
                            {
                                string mapService = urls[j].Item1;
                                string proxyUrl = urls[j].Item2;
                                string serviceType = urls[j].Item3;
                                Tuple<string, string, string> serviceAndProxy = new Tuple<string, string, string>(mapService, proxyUrl, serviceType);
                                layerServices.Add(serviceAndProxy);
                            }
                            break;
                        }
                    }
                    return layerServices;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }              
        }

        public static TextBlock ScaleTextBlock
        {
            get { return TemplateOptions._scaleTextBlock; }
            set { TemplateOptions._scaleTextBlock = value; }
        }

        public static TextBlock GridNumberTextBlock
        {
            get { return TemplateOptions._gridNumberTextBlock; }
            set { TemplateOptions._gridNumberTextBlock = value; }
        }
    }
}
