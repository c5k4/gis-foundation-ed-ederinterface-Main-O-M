using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ArcFMSilverlight.Controls.ShowRolloverInfo;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using System;
using System.Collections.Generic;
using ArcFMSilverlight.ViewModels;
using System.Windows.Browser;
using System.Linq;
using System.Xml.Linq;
using ArcFM.Silverlight.PGE.CustomTools;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Net;
using System.Json;
namespace ArcFMSilverlight
{
    /// <summary>
    ///   MapTools control supporting pan, zoom and rotation.
    /// </summary>
    public partial class MapTools : UserControl
    {
        private const int WgsWKID = 4326;
        private const string google_sufix = "/data=!3m1!1e3";
        private const string google_window = "https://www.google.com/maps/@";

        public int RuralView_CommonLandBase_LayerID;
        List<string> scalelist = new List<string>();
        public ShowRolloverInfo ShowRolloverInfo { get; set; }
        public Dictionary<string, string> ShareCurrentUrlSPIndex = new Dictionary<string, string>();   //INC000005346851 
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MapTools" /> class.
        /// </summary>
        public MapTools()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                ViewModel = new MapToolsViewModel(this);
            }

            InitializeComponent();

            //Add
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            var ScaleOptionNodes = from son in xe.Descendants("ScaleOptions")
                                   select son;
            //var a = ScaleOptionNodes.ToList();
            foreach (XElement ScaleOptionNode in ScaleOptionNodes.Elements())
            {
                string displayText = ScaleOptionNode.Attribute("DisplayText").Value;
                string value = ScaleOptionNode.Attribute("Value").Value;
                scalelist.Add(displayText);

            }
            ZoomToScaleTextbox.ItemsSource = scalelist;
        }

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map", 
            typeof(Map), 
            typeof(MapTools), 
            new PropertyMetadata(OnMapChanged));

      
        public MapToolsViewModel ViewModel
        {
            get { return this.DataContext as MapToolsViewModel; }
            set { DataContext = value; }
        }

        public double Resolution
        {
            get { return (double)GetValue(ResolutionProperty); }
            set { SetValue(ResolutionProperty, value); }
        }

        public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register(
            "Resolution",
            typeof(double),
            typeof(MapTools),
            new PropertyMetadata(OnResolutionChanged));

        public double UnscaledMarkerSize
        {
            get { return (double)GetValue(UnscaledMarkerSizeProperty); }
            set { SetValue(UnscaledMarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty UnscaledMarkerSizeProperty = DependencyProperty.Register(
            "UnscaledMarkerSize",
            typeof(double),
            typeof(MapTools),
            new PropertyMetadata(OnUnscaledMarkerSizeChanged));
        
        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapTools)d;
                Map map = e.NewValue as Map;
                tools.LayerControl.Map = map;
                tools.MapInset.Map = map;
                tools.MapInset.InsetMap = tools.InsetMap;
                tools.ViewModel.Map = map;

                var width = tools.LayerControl.Width;
                var height = tools.LayerControl.Height;
                tools.LayerControl.Width = 0;
                tools.LayerControl.Height = 0;
                Application.Current.Host.Content.Resized += (s, args) =>
                {
                    tools.LayerControl.MaxHeight = Application.Current.Host.Content.ActualHeight * 0.55;
                };

                tools.LayerToggle.IsChecked = true;

                tools.InsetToggle.IsChecked = true;
                tools.MapInsetContainer.Opacity = 0;

                tools.MapInset.InsetTOC.LayerRefreshed += (s, args) =>
                {
                    tools.LayerControl.Width = width;
                    tools.LayerControl.Height = height;

                    tools.LayerToggle.IsChecked = false;
                    tools.InsetToggle.IsChecked = false;
                    tools.MapInsetContainer.Opacity = 1;
                };
            }
        }

        private static void OnResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapTools)d;
                tools.MapInset.Resolution = (double)e.NewValue;
            }
        }

        private static void OnUnscaledMarkerSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapTools)d;
                tools.MapInset.UnscaledMarkerSize = (double)e.NewValue;
            }
        }

        private void ShowRolloverInfoToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ApplicationCacheManager cacheManager = new ApplicationCacheManager();
            cacheManager.StoreLocalValue("ShowRollover", "true");

            //Grid lcGrid = (Grid)VisualTreeHelper.GetChild(this.StoredViewControl.LocateControl, 0);
            //int selectedIndex = ((ComboBox)lcGrid.FindName("PART_LocateTypeComboBox")).SelectedIndex;

            ShowRolloverInfo.Show(this.StoredViewControl.LocateControl);
            //((ComboBox) lcGrid.FindName("PART_LocateTypeComboBox")).SelectedIndex = selectedIndex;
        }

        private void ShowRolloverInfoToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ApplicationCacheManager cacheManager = new ApplicationCacheManager();
            cacheManager.StoreLocalValue("ShowRollover", "false");
            ShowRolloverInfo.Hide();
        }

        private void WIPLayerVisibleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Map != null)
            {
                Layer WIPLayer = Map.Layers["WIP(Search)"];
                if (WIPLayer != null)
                    WIPLayer.Visible = true;

                WIPLayer = Map.Layers["WIP Cloud"];
                if (WIPLayer != null)
                    WIPLayer.Visible = true;

                WIPLayer = Map.Layers["WIP Label"];
                if (WIPLayer != null)
                    WIPLayer.Visible = true;
            }
        }

        private void WIPLayerVisibleToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Map != null)
            {
                Layer WIPLayer = Map.Layers["WIP(Search)"];
                if (WIPLayer != null)
                    WIPLayer.Visible = false;

                WIPLayer = Map.Layers["WIP Cloud"];
                if (WIPLayer != null)
                    WIPLayer.Visible = false;

                WIPLayer = Map.Layers["WIP Label"];
                if (WIPLayer != null)
                    WIPLayer.Visible = false;
                               
                
            }
        }

        private void ZoomFullExtent_Click(object sender, RoutedEventArgs e)
        {

            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "Layers")
                    {
                        Map.Extent = ConfigUtility.EnvelopeFromXElement(element, "Extent");
                        break;
                    }
                }
            }
        }

        private void BaseMapToggle_Click(object sender, RoutedEventArgs e)
        {
            ArcGISDynamicMapServiceLayer agsSatl = Map.Layers["Satellite"] as ArcGISDynamicMapServiceLayer;
            ArcGISTiledMapServiceLayer agsBase = Map.Layers["Streets"] as ArcGISTiledMapServiceLayer;
            ArcGISDynamicMapServiceLayer agsRural = Map.Layers["Rural Map View"] as ArcGISDynamicMapServiceLayer;
            Uri uriBase = new Uri("/Images/basemap.png", UriKind.Relative);
            System.Windows.Media.ImageSource imgBase = new System.Windows.Media.Imaging.BitmapImage(uriBase);
            Uri uriSatl = new Uri("/Images/Satellite_map.png", UriKind.Relative);
            System.Windows.Media.ImageSource imgSatl = new System.Windows.Media.Imaging.BitmapImage(uriSatl);


            if (agsSatl.Visible == false)
            {
                agsSatl.Visible = true;
                agsBase.Visible = false;
                imgSatellite.Source = imgBase;

                string selectedStoredView = StoredViewControl._selectedStoredView.StoredViewName;
                if (selectedStoredView == "Rural Map View")
                    agsRural.SetLayerVisibility(RuralView_CommonLandBase_LayerID, false);
                else
                    agsRural.SetLayerVisibility(RuralView_CommonLandBase_LayerID, true);

            }
            else
            {
                agsSatl.Visible = false;
                agsBase.Visible = true;
                imgSatellite.Source = imgSatl;
                agsRural.SetLayerVisibility(RuralView_CommonLandBase_LayerID, true);
            }

            if (Map.Scale < 6000 && Map.Layers["Satellite"].Visible)
            {
                Map.Layers["Commonlandbase"].Visible = false;

            }
            else
            {
                Map.Layers["Commonlandbase"].Visible = true;
            }
        }

        private void GoogleMapsExtent_Click(object sender, RoutedEventArgs e)
        {
            ESRI.ArcGIS.Client.Geometry.Envelope ext = Map.Extent;
            double xmin = ext.XMin;
            double xmax = ext.XMax;
            double ymin = ext.YMin;
            double ymax = ext.YMax;

            double centerx = ((xmax + xmin) / 2);
            double centery = ((ymax + ymin) / 2);

            ESRI.ArcGIS.Client.Geometry.MapPoint pt_projected = new ESRI.ArcGIS.Client.Geometry.MapPoint();
            pt_projected.X = centerx;
            pt_projected.Y = centery;
            pt_projected.SpatialReference = Map.SpatialReference;
            GeometryService geometryService = new GeometryService(ConfigUtility.GeometryServiceURL);
            var graphic = new Graphic
            {
                Geometry = pt_projected
            };

            var graphicList = new List<Graphic> { graphic };

            SpatialReference sp = new SpatialReference(WgsWKID);

            geometryService.Failed += new EventHandler<TaskFailedEventArgs>(geometryService_Failed); ;
            geometryService.ProjectCompleted += new EventHandler<GraphicsEventArgs>(geometryService_ProjectCompleted);
            geometryService.ProjectAsync(graphicList, sp);

        }

        private void geometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Failed to Open Google Map";

        }


        private void ZoomToScaleTextbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        ZoomToScale(e.AddedItems[0].ToString());
                    }
                }
            }
            catch { }
        }

        private void ZoomToScale(string scaleValue)
        {
            double zoomScale, resolution;
            try
            {
                if (scaleValue.Contains(":"))
                {
                    char[] delimiterChars = { ':' };
                    string[] scale = scaleValue.Split(delimiterChars);
                    scaleValue = scale[1].ToString();
                }
                zoomScale = Convert.ToDouble(scaleValue);

                resolution = (zoomScale) / 96;
                Map.ZoomToResolution(resolution);
            }
            catch { }
        }

        private void ZoomToScaleTextbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.Key.ToString() == "Enter")
                    {
                        ZoomToScale(ZoomToScaleTextbox.Text.ToString());
                    }
                }
            }
            catch { }

        }

        private void ZoomToScaleTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                ZoomToScale(ZoomToScaleTextbox.Text.ToString());
            }
        }

        private void geometryService_ProjectCompleted(object sender, GraphicsEventArgs e)
        {

            var pt_geographic = (MapPoint)e.Results[0].Geometry;
            double scale = Map.Scale;
            //int zoomlevel;
            string eyelevel;
            if (scale > 4494425)
                //zoomlevel = 7;
                eyelevel = "675105m";
            else if (scale <= 4494425 && scale > 2247212)
                //zoomlevel = 8;
                eyelevel = "337553m";
            else if (scale <= 2247212 && scale > 1123606)
                //zoomlevel = 9;
                eyelevel = "169023m";
            else if (scale <= 1123606 && scale > 561803)
                //zoomlevel = 10;
                eyelevel = "84321m";
            else if (scale <= 561803 && scale > 280901)
                //zoomlevel = 11;
                eyelevel = "42145m";
            else if (scale <= 280901 && scale > 140450)
                //zoomlevel = 12;
                eyelevel = "21120m";
            else if (scale <= 140450 && scale > 70225)
                //zoomlevel = 13;
                eyelevel = "10553m";
            else if (scale <= 70225 && scale > 35112)
                //zoomlevel = 14;
                eyelevel = "5274m";
            else if (scale <= 35112 && scale > 17556)
                //zoomlevel = 15;
                eyelevel = "2641m";
            else if (scale < 17556 && scale > 8778)
                //zoomlevel = 16;
                eyelevel = "1321m";
            else if (scale <= 8778 && scale > 4389)
                //zoomlevel = 17;
                eyelevel = "661m";
            else if (scale <= 4389 && scale > 2194)
                //zoomlevel = 18;
                eyelevel = "330m";
            else if (scale < 2194 && scale > 1097)
                //zoomlevel = 19;
                eyelevel = "165m";
            else if (scale <= 1097 && scale > 548)
                //zoomlevel = 20;
                eyelevel = "83m";
            else if (scale <= 548 && scale > 274)
                //zoomlevel = 21;
                eyelevel = "41m";
            else
                //zoomlevel = 21;
                eyelevel = "41m";

            //Uri uri = new Uri(google_window + pt_geographic.Y + "," + pt_geographic.X + "," + zoomlevel + "z");
            Uri uri = new Uri(google_window + pt_geographic.Y + "," + pt_geographic.X + "," + eyelevel + google_sufix);
            System.Windows.Browser.HtmlPage.Window.Navigate(uri, "_blank");


        }

        //INC000005346851 
        private void ShareCurrentURLBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var centerPoint = Map.Extent.GetCenter();
                string X = centerPoint.X.ToString();
                string Y = centerPoint.Y.ToString();
                double scale = Math.Round(Map.Resolution * 96);
                string selectedStoredView = ShareCurrentUrlSPIndex.Where(s => s.Value == StoredViewControl._selectedStoredView.StoredViewName.ToString()).First().Key;

                string pageUri;
                if (System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != null && System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != "")
                    pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString().Replace(System.Windows.Browser.HtmlPage.Document.DocumentUri.Query.ToString(), "");
                else
                    pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();
                string clipboardText = pageUri + "?X=" + X + "&Y=" + Y + "&SCALE=" + scale.ToString() + "&SP=" + selectedStoredView;

                Clipboard.SetText(clipboardText);
                //MessageBox.Show("Current Extent URL copied to Clipboard.");
                ConfigUtility.StatusBar.Text = "Current View URL copied to Clipboard";

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error in copying Current Extent URL.");
                ConfigUtility.StatusBar.Text = "Error in copying Current View URL";
            }

        }

    }
}
