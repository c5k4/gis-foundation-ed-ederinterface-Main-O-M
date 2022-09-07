using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ESRI.ArcGIS.Client.Geometry;
using System.Windows.Shapes;
using System.Windows.Browser;
using Miner.Silverlight.Logging.Client;

using NLog;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using AreaUnit = ESRI.ArcGIS.Client.Actions.AreaUnit;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;
using Geometry = ESRI.ArcGIS.Client.Geometry;
using Path = System.IO.Path;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;

namespace ArcFMSilverlight
{
    public partial class UfmPrintControl : UserControl
    {
        private const string UFM_PRINT_GRAPHICS_LAYER = "UfmPrintGraphics";
        private const string MSG_UFM_SUBMITTED = "Submitted for UFM Print";

        private GraphicsLayer _graphicsLayer;
        private GeometryService _geometryService;
        private Logger logger = LogManager.GetCurrentClassLogger();

        private Map _map;
        private SimpleFillSymbol _fillSymbol;
        private TextSymbol _textSymbol;
        private MapPoint _textPosition = null;
        private Size _textSize;

        private Polygon _polygon;
        private Envelope _envelope;
        private ESRI.ArcGIS.Client.Geometry.Polyline _polyline;
        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private string _ufmPrintServiceUrl;
        private Geoprocessor _geoprocessorTask;
        private const int SUBMIT_SIZE = 25;

        public UfmPrintControl(Map map, string ufmPrintServiceUrl)
        {
            InitializeComponent();
            _map = map;
            _ufmPrintServiceUrl = ufmPrintServiceUrl;

            string text = "Submitted for UFM Print";
            int fontSize = SUBMIT_SIZE;
            FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            _textSize = GetTextSize(text, fontFamily, fontSize);

            _textSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = fontSize,
                Text = text,
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };           
           

            LineSymbol = new SimpleLineSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                Width = 2,
                Style = SimpleLineSymbol.LineStyle.Solid
            };
            FillSymbol = new SimpleFillSymbol
            {
                Fill = new SolidColorBrush(Color.FromArgb(0x22, 255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                BorderThickness = 2
            };
            _fillSymbol = new SimpleFillSymbol
            {
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = 2,
                Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.25 }
            };

            _markerSymbol = new SimpleMarkerSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)),
                Size = 5,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };
        }

        public NumericUpDown RotationUpDown
        {
            get
            {
                return nudRotation;
            }
        }
        
        public MeasureAction.Mode MeasureMode
        { get; set; }

        private List<double> SegmentLengths;
        private readonly SimpleMarkerSymbol _markerSymbol;
        public FillSymbol FillSymbol { get; set; }
        public LineSymbol LineSymbol { get; set; }
        public double AreaTotal { get; set; }

        private bool _clearAll;


        #region Private Methods       




        private void PrintButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearGraphics();
            SubmitPrintJob();
        }

        private void SubmitPrintJob()
        {
            _geoprocessorTask = new Geoprocessor(_ufmPrintServiceUrl);

            List<GPParameter> parameters = new List<GPParameter>();
            parameters.Add(new GPString("To", txtSendTo.Text.Replace("admin", "") + "@pge.com"));
            parameters.Add(new GPBoolean("splitYn", radio11x17.IsChecked.Value));
            // NB Silverlight Rotation is not ArcMap Rotation
            parameters.Add(new GPDouble("rotation", 360 - nudRotation.Value)); 
            parameters.Add(new GPDouble("xMin", _map.Extent.XMin));
            parameters.Add(new GPDouble("yMin", _map.Extent.YMin));
            parameters.Add(new GPDouble("xMax", _map.Extent.XMax));
            parameters.Add(new GPDouble("yMax", _map.Extent.YMax));

            _geoprocessorTask.JobCompleted += new EventHandler<JobInfoEventArgs>(geoprocessorTask_JobCompleted);
            _geoprocessorTask.StatusUpdated += new EventHandler<JobInfoEventArgs>(geoprocessorTask_StatusUpdated);
            _geoprocessorTask.SubmitJobAsync(parameters);
            //CreateFeedbackPolygon();
        }

        void geoprocessorTask_StatusUpdated(object sender, JobInfoEventArgs e)
        {
            if (e.JobInfo.JobStatus == esriJobStatus.esriJobSubmitted)
            {
                _geoprocessorTask.StatusUpdated -= geoprocessorTask_StatusUpdated;
                CreateFeedbackText();
            }
        }

        void geoprocessorTask_JobCompleted(object sender, JobInfoEventArgs e)
        {
            if (e.JobInfo.JobStatus == esriJobStatus.esriJobFailed)
            {
                MessageBox.Show("Failed to Print UFM View");
            }
        }


        //private void CreateFeedbackPolygon()
        //{
        //    Graphic exportedFillGraphic = new Graphic()
        //    {
        //        Geometry = CADExportControl.GetPolygonFromEnvelope(_map.Extent),
        //        Symbol = _fillSymbol
        //    };

        //    _graphicsLayer.Graphics.Add(exportedFillGraphic);
        //}

        private void CreateFeedbackText()
        {
            _textPosition = _map.Extent.GetCenter();
            RotatingTextSymbol rotatingTextSymbol = new RotatingTextSymbol("Arial", SUBMIT_SIZE, "Cyan")
            {
                Angle = 360 - nudRotation.Value,
                Text = MSG_UFM_SUBMITTED
            };
            Graphic graphicText = new Graphic()
            {
                Geometry = _textPosition,
                Symbol = rotatingTextSymbol,
            };

            _graphicsLayer.Graphics.Add(graphicText);

        }


        #endregion

        #region Public Methods

        public bool MapEventIsOn;

        public void StartListeningToMapExtentChanged()
        {
        }

        public void StopListeningToMapExtentChanged()
        {
        }

        static public Size GetTextSize(string text, FontFamily fontFamily, int fontSize)
        {
            Size size = new Size(0, 0);

            TextBlock l = new TextBlock();
            l.FontFamily = fontFamily;
            l.FontSize = fontSize;
            l.Text = text;
            size.Height = l.ActualHeight;
            size.Width = l.ActualWidth;

            return size;
        }

        public void InitializeDrawGraphics()
        {
            if (_graphicsLayer == null)
            {
                _fillSymbol = new SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                    Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.25 }
                };
                _graphicsLayer = new GraphicsLayer();
                _graphicsLayer.ID = UFM_PRINT_GRAPHICS_LAYER;
                if (!_map.Layers.Contains(_graphicsLayer))
                {
                    _map.Layers.Add(_graphicsLayer);
                }

                Binding valueBinding = new Binding() { Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay };
                valueBinding.Source = nudRotation;
                _map.SetBinding(Map.RotationProperty, valueBinding);
            }
            _polygon = null;


        }

        public void ClearGraphics()
        {
            _graphicsLayer.ClearGraphics();
        }

        public void ResetGraphics()
        {
            _graphicsLayer.Graphics.Clear();
            _polygon = null;
            _envelope = null;
            PrintButton.IsEnabled = true;
        }

        #endregion

        private void NudRotation_OnValueChanging(object sender, RoutedPropertyChangingEventArgs<double> e)
        {
            // The NUD takes care of values < -1 & > 360. Those values are used for a wraparound
            if (e.NewValue == -1)
            {
                nudRotation.Value = 359;
            }
            if (e.NewValue == 360)
            {
                nudRotation.Value = 0;
            }
        }
    }

    public class UfmPrintControlParameters
    {
        public Dictionary<string, string> Scales;
        public Dictionary<string, string> MapTypes;
        public string DefaultScale;
        public string ExportRESTService;
        public string GeometryService;
        public int BufferGeometryFeet;
    }


}
