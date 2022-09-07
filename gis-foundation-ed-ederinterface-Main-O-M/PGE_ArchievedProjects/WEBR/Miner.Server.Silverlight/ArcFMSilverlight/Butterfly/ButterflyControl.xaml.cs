using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using PGnE.Printing;
using ESRI.ArcGIS.Client.Symbols;
using ArcFMSilverlight.Controls.CADExport;
namespace ArcFMSilverlight.Butterfly
{
    public partial class ButterflyControl : UserControl
    {
        private const string BUTTERFLY_PRINT_GRAPHICS_LAYER = "ButterflyPrintGraphics";
        private const string MSG_BUTTERFLY_SUBMITTED = "Submitted for Print";
        private const int SUBMIT_SIZE = 25;
        private const double PRINT_BUTTON_OPACITY = 0.25;

        private Geometry _geometry;

        private string _printTemplate;
        private string _basemapUrl;
        private string _commonLandbaseUrl;
        private string _ufmUrl;
        private int[] _ufmLayerIds;
        private string _facilityId;
        private const string LAYER_ID_UFM = "UFM";
        private ArcGISDynamicMapServiceLayer _ufmLayer;

        private int _numRemainingExecutingQueries = 0;
        private Envelope _geometriesToUnion = null;
        private GraphicsLayer _graphicsLayer;
        private SimpleFillSymbol _fillSymbol;
        private TextSymbol _textSymbol;
        private MapPoint _textPosition = null;
        private Size _textSize;

        private HiResPrint _hiResPrint = null;

        private string _asyncPrintService = "";
        private string _syncPrintService = "";
        /*FME Parameters*/
        private Dictionary<string, string> restParameters;
        PngExport butterflyPngExport = null;
        private string _DCURL = "";
        private string _StreetLightURL = "";
        private string _strButterflyLayers = "";
        private string _strLandbaseLayers = "";
        private string _strFMEPNGExportUrl = "";
        private string AsyncPrintService
        {
            get
            {
                if (String.IsNullOrEmpty(_asyncPrintService))
                {
                    GetPrintServices();
                }
                return _asyncPrintService;
            }
        }
        private string SyncPrintService
        {
            get
            {
                if (String.IsNullOrEmpty(_syncPrintService))
                {
                    GetPrintServices();
                }
                return _syncPrintService;
            }
        }
        private TextSymbol _dwgTextSymbol;
        private Size _dwgTextSize;
        private TextSymbol _pngTextSymbol;
        private Size _pngTextSize;
        private TextSymbol _dwgPngTextSymbol;
        private Size _dwgPngTextSize;

        public ButterflyControl(string basemapUrl, string commonLandbaseUrl, string ufmUrl, int[] ufmLayerIds, string printTemplate)
        {
            InitializeComponent();
            int fontSize = 25;
            System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            _dwgTextSize = GetTextSize("Submitted for DWG Export", fontFamily, fontSize);
            _dwgTextSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Cyan),
                FontSize = 25,
                Text = "Submitted for DWG Export",
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };

            _pngTextSize = GetTextSize("Submitted for PNG Export", fontFamily, fontSize);
            _pngTextSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Cyan),
                FontSize = 25,
                Text = "Submitted for PNG Export",
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };

            _dwgPngTextSize = GetTextSize("Submitted for DWG+PNG Export", fontFamily, fontSize);
            _dwgPngTextSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Cyan),
                FontSize = 25,
                Text = "Submitted for DWG+PNG Export",
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };
            _basemapUrl = basemapUrl;
            _commonLandbaseUrl = commonLandbaseUrl;
            _ufmUrl = ufmUrl;
            _ufmLayerIds = ufmLayerIds;
            _printTemplate = printTemplate;
            ButterflyPrintButton.Opacity = PRINT_BUTTON_OPACITY;
        }

        private void GetPrintServices()
        {
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            _asyncPrintService = xe.Element("ExtractSendService").Attribute("Url").Value;

            _syncPrintService =
                Convert.ToString(xe.Element("PrintServiceUrls").Element("PrintServiceUrl").Attribute("Url").Value);

        }

        static public Size GetTextSize(string text, System.Windows.Media.FontFamily fontFamily, int fontSize)
        {
            Size size = new Size(0, 0);

            TextBlock l = new TextBlock();
            l.FontFamily = fontFamily;
            l.FontSize = fontSize;
            //l.FontStyle = FontStyle ; 
            //l.FontWeight = font.Weight; 
            l.Text = text;
            size.Height = l.ActualHeight;
            size.Width = l.ActualWidth;

            return size;
        }

        private void AddToEnvelope(FeatureSet featureSet)
        {
            if (featureSet == null) return;

            foreach (Graphic feature in featureSet.Features)
            {
                _geometriesToUnion = _geometriesToUnion.Union(feature.Geometry.Extent);
            }
        }

        private void GetExtentAsync()
        {
            _numRemainingExecutingQueries = _ufmLayerIds.Length;
            _geometriesToUnion = _geometry.Extent;

            Query query = new Query();
            query.ReturnGeometry = true;
            query.Where = "FACILITYID='" + _facilityId + "'";

            foreach (int ufmLayerId in _ufmLayerIds)
            {
                QueryTask queryTask = new QueryTask(_ufmUrl + "/" + ufmLayerId);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
                queryTask.ExecuteAsync(query);
            }
        }

        void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _numRemainingExecutingQueries--;

            AddToEnvelope(e.FeatureSet);

            if (_numRemainingExecutingQueries == 0)
            {
                SetExtent(_geometriesToUnion.Expand(1.25));
            }
        }


        // Graphic could be a Vault Polygon or a Subsurface Structure (subtype:vault)
        public void CenterAt(Graphic graphic)
        {
            ConfigUtility.UpdateStatusBarText("Setting Butterfly Extent...");
            _geometry = graphic.Geometry;
            _facilityId = graphic.Attributes["FACILITYID"].ToString();

            if (ButterflyMap.Layers == null || ButterflyMap.Layers.Count == 0)
            {
                InitializeMap();
            }
            else
            {
                ClearGraphics();
                GetExtentAsync();
            }
        }

        private void SetExtent(Geometry geometry)
        {
            _ufmLayer.LayerDefinitions.Clear();
            _ufmLayer.LayerDefinitions = GetLayerDefinitions();
            ButterflyMap.ZoomTo(geometry);
            ConfigUtility.UpdateStatusBarText("");
        }

        private void InitializeMap()
        {
//            ButterflyMap.Layers.Add(new ArcGISTiledMapServiceLayer() { Url = _basemapUrl});
// Removed per Nick but left in pending UAT
//            ButterflyMap.Layers.Add(new ArcGISDynamicMapServiceLayer() { Url = _commonLandbaseUrl });
            
            _ufmLayer = new ArcGISDynamicMapServiceLayer()
            {
                ID = LAYER_ID_UFM,
                Url = _ufmUrl,
                VisibleLayers = _ufmLayerIds,
                LayerDefinitions = GetLayerDefinitions()
            };
            ButterflyMap.Layers.Add(_ufmLayer);
        }

        private ObservableCollection<LayerDefinition> GetLayerDefinitions()
        {
            ObservableCollection<LayerDefinition> layerDefinitions = new ObservableCollection<LayerDefinition>();

            foreach (int ufmLayerId in _ufmLayerIds)
            {
                LayerDefinition layerDefinition = new LayerDefinition();
                layerDefinition.LayerID = ufmLayerId;
                layerDefinition.Definition = "FACILITYID='" + _facilityId + "'";
                layerDefinitions.Add(layerDefinition);
            }

            return layerDefinitions;
        }

        private void MapIsLoaded()
        {
            GetExtentAsync();

            _graphicsLayer = new GraphicsLayer();
            _graphicsLayer.ID = BUTTERFLY_PRINT_GRAPHICS_LAYER;
            if (!ButterflyMap.Layers.Contains(_graphicsLayer))
            {
                ButterflyMap.Layers.Add(_graphicsLayer);
            }

        }

        private void ButterflyMap_OnProgress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                ButterflyMap.Progress -= ButterflyMap_OnProgress;

                MapIsLoaded();
            }
        }

        private void SetPrintButtonOpacity()
        {
            if (ButterflyPrintButton.IsMouseOver)
            {
                ButterflyPrintButton.Opacity = 1.0;
            }
            else
            {
                ButterflyPrintButton.Opacity = PRINT_BUTTON_OPACITY;
            }
        }

        private void ButterflyPrintButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SetPrintButtonOpacity();
        }

        private void ButterflyPrintButton_OnMouseLeave(object sender, MouseEventArgs e)
        {
            SetPrintButtonOpacity();
        }

        public void ClearGraphics()
        {
            _graphicsLayer.ClearGraphics();
        }

        private void SubmitPrintJob()
        {
            if (_hiResPrint == null)
            {
                string userEmail = WebContext.Current.User.Name.Replace("PGE\\", "").ToLower().Replace("admin", "") + "@pge.com";
                // Note that the last param here means that we will default to using the async service at all times...
                _hiResPrint = new HiResPrint(new Uri(SyncPrintService), new Uri(AsyncPrintService), userEmail, 0) { DisablePopups = true };
                // Not going to listen to Completed event because it's asynchronous
            }
            else
            {
                ClearGraphics();                
            }

            _hiResPrint.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(_hiResPrint_PrintJobSubmitted);
            _hiResPrint.Print(ButterflyMap.Scale, ButterflyMap, _printTemplate, ButterflyMap.Extent, null, Utilites.PrintFormat.PDF, 300);
        }

        void _hiResPrint_PrintJobSubmitted(object sender, PrintJobSubmittedEventArgs e)
        {
            _hiResPrint.PrintJobSubmitted -= _hiResPrint_PrintJobSubmitted;
            CreateFeedbackText();
        }


        private void CreateFeedbackText()
        {
            _textPosition = ButterflyMap.Extent.GetCenter();
            RotatingTextSymbol rotatingTextSymbol = new RotatingTextSymbol("Arial", SUBMIT_SIZE, "Cyan")
            {
                //Angle = 360 - nudRotation.Value,
                Text = MSG_BUTTERFLY_SUBMITTED
            };
            Graphic graphicText = new Graphic()
            {
                Geometry = _textPosition,
                Symbol = rotatingTextSymbol,
            };

            _graphicsLayer.Graphics.Add(graphicText);

        }

        private void callButterflyExportDWGFMEService(){
           
            //int scale = Convert.ToInt32(((KeyValuePair<string, string>)this.cboScale.SelectedItem).Key);
           // fmePolygonCoordinates = PolygonToFMECoordinates(polygon);
           // _textPosition = polygon.Extent.GetCenter();
            string landid = WebContext.Current.User.Name.Replace("PGE\\", "");
            string email = landid + "@pge.com";
            Polygon tempPolygon = GetPolygonFromEnvelope(ButterflyMap.Extent);
            string polyToFMECoordinate = PolygonToFMECoordinates(tempPolygon);
            GetPrintServicesUrl();
            string PrintServiceUrl = _syncPrintService;
            PngExport butterflyPngExport = new PngExport(new Uri(PrintServiceUrl));
            butterflyPngExport.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(PngExport_PrintJobSubmitted);
            Map PNGExportMap = ButterflyMap;
            ComboBoxItem cbItem = (ComboBoxItem)cboButterflyExportFormat.SelectedItem;
            string selectedExportFormat = cbItem.Content.ToString();
            String[] strMapTypes = {"BUTTERFLY"};
            string selectedExportLayout = cboButterflyExportLayout.SelectedValue.ToString();
            //string adHocTemplateNameOnServer = "AdHocMap_8-5x11_Portrait";
            TextSymbol textSymbol;
            
            if (selectedExportFormat == "DWG")
            {
                textSymbol = _dwgTextSymbol;
            }
            else if (selectedExportFormat == "PNG")
            {
                textSymbol = _pngTextSymbol;
            }
            else
            {
                textSymbol = _dwgPngTextSymbol;
            }
    

            butterflyPngExport.strDcviewURL = _DCURL;
            butterflyPngExport.strStreetLightURL = _StreetLightURL;
            butterflyPngExport.lstButterflyVisibleLayers = _strButterflyLayers;
            butterflyPngExport.lstLandBaseVisibleLayers = _strLandbaseLayers;
            butterflyPngExport.FMEPNGExportURL = _strFMEPNGExportUrl;
            butterflyPngExport.strButterflyURL = _ufmUrl;
            butterflyPngExport.PrintED50(polyToFMECoordinate, ButterflyMap.Scale, GetSanitizedFileName(), email, selectedExportFormat, _facilityId, PNGExportMap, selectedExportLayout, PNGExportMap.Extent, null, "PNG32", 300, false, strMapTypes, textSymbol, _graphicsLayer, null);

                

            
        }

        public void populateExportLayoutDropdown()
        {
            string config = Application.Current.Host.InitParams["Config"];
            //var attribute = "";
            config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement elements = XElement.Parse(config);
            foreach (XElement element in elements.Elements())
            {
                if (element.Name.LocalName == "CADExportPngTemplate")
                {
                    if (element.HasElements)
                    {
                        string codedDescriptionsCSV =
                             element.Element("ExportLayoutDrpDwnDomain")
                                 .Attribute("CodedDescriptionsCSV")
                                 .Value;
                        string codedValuesCSV =
                            element.Element("ExportLayoutDrpDwnDomain").Attribute("CodedValuesCSV").Value;
                        IList<string> codedDescriptions = codedDescriptionsCSV.Split(',').ToList();
                        IList<string> codedValues = codedValuesCSV.Split(',').ToList();
                        Dictionary<string, string> feederDomainDictionary =
                            codedValues.Zip(codedDescriptions, (k, v) => new { k, v })
                                .ToDictionary(x => x.k, x => x.v);
                        Dictionary<string, string> placeHolder = new Dictionary<string, string>();
                        Dictionary<string, string> feederDictionary = placeHolder.Concat(feederDomainDictionary).ToDictionary(x => x.Key, x => x.Value);
                        cboButterflyExportLayout.ItemsSource = feederDictionary;
                        cboButterflyExportLayout.SelectedIndex = 0;

                    }
                }
            }
        }

        void PngExport_PrintJobSubmitted(object sender, PrintJobSubmittedEventArgs e)
        {
            string _printJobId = string.Empty;
            _printJobId = e.JobId;
            butterflyPngExport.PrintJobSubmitted -= PngExport_PrintJobSubmitted;
        }


        private void GetPrintServicesUrl()
        {
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            _syncPrintService =
                Convert.ToString(xe.Element("PrintServiceUrls").Element("PrintServiceUrl").Attribute("Url").Value);

            string config1 = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
            config1 = System.Windows.Browser.HttpUtility.HtmlDecode(config1).Replace("***", ",");
                XElement elements = XElement.Parse(config1);

                _DCURL = Convert.ToString(elements.Element("CADExport").Element("DCViewMAPService").Attribute("URL").Value);
                _StreetLightURL = Convert.ToString(elements.Element("CADExport").Element("StreetLightMAPService").Attribute("URL").Value);
                _strButterflyLayers = Convert.ToString(elements.Element("CADExport").Element("PNGButterflyVisibleLayers").Attribute("LayerIDs").Value);
                _strLandbaseLayers = Convert.ToString(elements.Element("CADExport").Element("PNGLandbaseVisibleLayers").Attribute("LayerIDs").Value);
                _strFMEPNGExportUrl = Convert.ToString(elements.Element("CADExport").Element("FMEPNGExportURL").Attribute("URL").Value);
        }

        private string PolygonToFMECoordinates(Polygon polygon)
        {
            string fmeCoordinates = "";

            foreach (MapPoint point in polygon.Rings[0])
            {
                fmeCoordinates += Math.Round(point.X).ToString() + " " + Math.Round(point.Y).ToString() + " ";
            }

            return fmeCoordinates;
        }

        private ESRI.ArcGIS.Client.Geometry.Polygon GetPolygonFromEnvelope(Envelope Extent)
        {
            Graphic g = new Graphic();
            Polygon p = new ESRI.ArcGIS.Client.Geometry.Polygon();
            p.SpatialReference = Extent.SpatialReference;

            PointCollection pColl = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            MapPoint pLeftBottom = new MapPoint();
            pLeftBottom.X = Extent.XMin;
            pLeftBottom.Y = Extent.YMin;
            pColl.Add(pLeftBottom);
            MapPoint pLeftTop = new MapPoint();
            pLeftTop.X = Extent.XMin;
            pLeftTop.Y = Extent.YMax;
            pColl.Add(pLeftTop);
            MapPoint pRightTop = new MapPoint();
            pRightTop.X = Extent.XMax;
            pRightTop.Y = Extent.YMax;
            pColl.Add(pRightTop);
            MapPoint pRightBottom = new MapPoint();
            pRightBottom.X = Extent.XMax;
            pRightBottom.Y = Extent.YMin;
            pColl.Add(pRightBottom);
            MapPoint pLeftBottomEnd = new MapPoint();
            pLeftBottomEnd.X = Extent.XMin;
            pLeftBottomEnd.Y = Extent.YMin;
            pColl.Add(pLeftBottomEnd);

            p.Rings.Add(pColl);

            return p;
        }

        

           private string GetSanitizedFileName()
        {
            const int MAX_USER_FILENAME_SIZE = 100;

            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            fileName += "_";
            string userFileName = "";
           
                userFileName = "GISToCAD";
          
            fileName += userFileName;

            return fileName;
        }

        private void butterflyExportFormatCbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cboButterflyExportFormat != null)
            {
                ComboBoxItem cbItem = (ComboBoxItem)cboButterflyExportFormat.SelectedItem;
                string selectedExportFormat = cbItem.Content.ToString();
                if (selectedExportFormat == "PNG" || selectedExportFormat == "DWG+PNG")
                {
                    ClearGraphics();
                    butterflyPngExportLayoutLbl.Visibility = Visibility.Visible;
                    cboButterflyExportLayout.Visibility = Visibility.Visible;
                }
                else
                {
                    ClearGraphics();
                    butterflyPngExportLayoutLbl.Visibility = Visibility.Collapsed;
                    cboButterflyExportLayout.Visibility = Visibility.Collapsed;
                }
            }
        }
        

        private void ButterflyPrintButton_OnClick(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbItem = (ComboBoxItem)cboButterflyExportFormat.SelectedItem;
            string selectedExportFormat = cbItem.Content.ToString();
            if (selectedExportFormat == "PDF" )
            {
                ClearGraphics();
                SubmitPrintJob();
            }else{
                ClearGraphics();
                callButterflyExportDWGFMEService();
               // callButterflyPNGFMEService();
            }

        }
    }
}
