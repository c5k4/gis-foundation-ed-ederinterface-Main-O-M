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
using System.Windows.Browser;
using Miner.Silverlight.Logging.Client;

using NLog;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using AreaUnit = ESRI.ArcGIS.Client.Actions.AreaUnit;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;
using Geometry = ESRI.ArcGIS.Client.Geometry;
using Path = System.IO.Path;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;
using Miner.Server.Client;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client.FeatureService;
using PGnE.Printing;
using ArcFMSilverlight.Controls.CADExport;
namespace ArcFMSilverlight
{
    public partial class CADExportControl : UserControl
    {
        private const string CAD_EXPORT_GRAPHICS_LAYER = "ExportGraphics";

        private GraphicsLayer _graphicsLayer;
        private GeometryService _geometryService;
        private string _exportRESTService;
        private string _schematicExportPath;
        private string _schematicSDEFilePath;
        private string _exportWYSWYGRESTService;
        private string _exportSCHEMATICSRESTService;

        private string _schematicsGridLayer;
        private string[] _schematicsGridLayerIds;//UMG layer divided into 5 - INC000004402997, INC000004402999
        private double _schematicQueryScale;
        private string _schematicGPService;
        private Dictionary<string, string> _schematicColors;
        private List<string> _schematicSearchLayers;
        private Logger logger = LogManager.GetCurrentClassLogger();

        private Map _map;
        private MapTools _mapTools;
        private Draw _drawControl = null;
        private SimpleFillSymbol _fillSymbol;
        private TextSymbol _textSymbol;
        private MapPoint _textPosition = null;
        private Size _textSize;
        private TextSymbol _pngTextSymbol;
        private Size _pngTextSize;
        private TextSymbol _pngDwgTextSymbol;
        private Size _pngDwgTextSize;
        private int _maximumAreaSquareFeet;
        private int _circuitmaximumAreaSquareFeet;
        private XElement _annotationElement;
        private Dictionary<string, string> restParameters;
        private int _bufferGeometryFeet;
        private Geoprocessor _schematicGeoprocessor = null;

        //UMG layer divided into 5 - INC000004402997, INC000004402999
        private List<QueryTask> _schemticqueryTaskList = new List<QueryTask>();    
        private int _schemticqueryTaskCount = 0;  
        private string _exportedgridno = "";

        private Polygon _polygon;
        private Envelope _envelope;
        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private string[] _visibleLayers;
        private string[] _defaultvisibleLayers;
        private CADExportControlParameters _cadparams;
        private int _queryresultcount;
        private Dictionary<String, String> _circuitIdandColors;
        private List<string> _jobIds;
        private string _MapType;
        private List<string> _MapTypeList;
        private string fmePolygonCoordinates;
        
        private Dictionary<String, String> _pngJobId;
       
        private string _schematicDPI;
        private FloatableWindow _dwgFloatWindow;
        public FloatableWindow DwgFloatWindow
        {
            get { return _dwgFloatWindow; }
            set
            {
                _dwgFloatWindow = value;
            }
        }
        private string _schematicsFilterDefinitionquery=null;
        public string SchematicsFilterDefinitionquery
        {
            get { return _schematicsFilterDefinitionquery; }
            set
            {
                _schematicsFilterDefinitionquery = value;
            }
        }

        /*******************************31Oct2016 Starts*************************************************/
        private string _syncPrintService = string.Empty;
        PngExport PngExport = null;
        
        //private string _eD50ExportRestService;
        private string _schematicsPNGExportURL;
        private string _schematics250PNGExportURL;
        private bool isMapLayerChange = false;
        private ObservableCollection<ListOfRecords> _allStoredViewList;
        public ObservableCollection<ListOfRecords> AllStoredViewList
        {
            get { return _allStoredViewList; }
            set
            {
                _allStoredViewList = value;

            }
        }
        private string _selectedPageTemplate;
        private double _selectedExportScale;
        private Envelope _schematicsPrintPreviewExtent;
        public Envelope SchematicsPrintPreviewExtent
        {
            get { return _schematicsPrintPreviewExtent; }
            set
            {
                _schematicsPrintPreviewExtent = value;

            }
        }
        /******************************31Oct2016 Ends**************************************************/
        string _domainFieldName;
        public CADExportControl(Map map, MapTools mapTools)
        {
            InitializeComponent();
            _map = map;
            _mapTools = mapTools;
            // dEBUG: TODO: remove, 10503 switch
            //Envelope env = new Envelope(2812841, 12845404, 2813658, 12845948);
            //_map.Extent = env;

            string text = "Submitted for DWG Export";
            int fontSize = 25;
            FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            _textSize = GetTextSize(text, fontFamily, fontSize);

            _textSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = 25,
                Text = text,
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };

            string pngText = "Submitted for PNG Export";
            _pngTextSize = GetTextSize(pngText, fontFamily, fontSize);

            _pngTextSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = 25,
                Text = pngText,
                OffsetX = _pngTextSize.Width / 2,
                OffsetY = _pngTextSize.Height / 2
            };

            string pngDwgText = "Submitted for DWG+PNG Export";
            _pngDwgTextSize = GetTextSize(pngDwgText, fontFamily, fontSize);

            _pngDwgTextSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = 25,
                Text = pngDwgText,
                OffsetX = _pngDwgTextSize.Width / 2,
                OffsetY = _pngDwgTextSize.Height / 2
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
            _markerSymbol = new SimpleMarkerSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)),
                Size = 5,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };

            SegmentLengths = new List<double>();
            MeasureMode = MeasureAction.Mode.Polygon;
           
            _circuitIdandColors = new Dictionary<string, string>();
        }




        public MeasureAction.Mode MeasureMode
        { get; set; }

        private List<double> SegmentLengths;
        private readonly SimpleMarkerSymbol _markerSymbol;
        public FillSymbol FillSymbol { get; set; }
        public LineSymbol LineSymbol { get; set; }
        public double AreaTotal { get; set; }

        private bool _firstTime;
        private bool _finishingSegment;
        private bool _finishingArea;
        private bool _finishMeasure;
        private bool _finishedSegment;
        private bool _isMeasuring;
        private bool _clearAll;

        private const double MetersToMiles = 0.0006213700922;
        private const double MetersToFeet = 3.280839895;
        private const double SqMetersToSqMiles = 0.0000003861003;
        private const double SqMetersToSqFeet = 10.76391;
        private const double SqMetersToHectare = 0.0001;
        private const double SqMetersToAcre = 0.00024710538;
        private const int TotalDistanceSymbolPosition = 0;
        private const int TotalAreaSymbolPosition = 1;
        private int _graphicCount, _areaAsyncCalls, _mouseMove;
        private double _mapDistance;
        private double _distanceFactor = 0.3048;



        #region Map Events

        private void _map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            _firstTime = true;

            if (_clearAll == true)
            {
                _layers.Add(new SegmentLayer());
                _map.Layers.Add(_layers.Last().Layer);
                _clearAll = false;


            }
            else if (!_layers.Any())
            {
                _layers.Add(new SegmentLayer());
                _map.Layers.Add(_layers.Last().Layer);
            }

            Point pt = e.GetPosition(null);

            if (!_finishedSegment && Math.Abs(pt.X - _layers.Last().LastClick.X) < 2 && Math.Abs(pt.Y - _layers.Last().LastClick.Y) < 2)
            {
                _finishMeasure = true;
                MapMouseDoubleClick(sender, e);
                _graphicsLayer.Graphics.Clear();
                ClearPreviousLayers();
                _clearAll = true;
            }

            else if (!(_finishingSegment || _finishingArea))
            {
                _finishedSegment = false;

                var map = sender as Map;
                if (map == null) return;

                if (_layers.Last().Points.Count == 0)
                {
                    if (MeasureMode == MeasureAction.Mode.Polygon)
                    {
                        var areaGraphic = new Graphic
                        {
                            Symbol = FillSymbol
                        };

                        _layers.Last().Layer.Graphics.Add(areaGraphic);
                        var areaTotalGraphic = new Graphic
                        {
                            Symbol = new RotatingTextSymbol()
                        };

                        // Bump the Z up so fill will be underneath.
                        areaTotalGraphic.SetZIndex(2);

                        _layers.Last().Layer.Graphics.Add(areaTotalGraphic);
                    }
                }

                if (_layers.Last().OriginPoint != null) _layers.Last().PrevOrigin = _layers.Last().OriginPoint;

                _layers.Last().OriginPoint = map.ScreenToMap(e.GetPosition(map));
                _layers.Last().EndPoint = map.ScreenToMap(e.GetPosition(map));

                var line = new ESRI.ArcGIS.Client.Geometry.Polyline();
                var points = new PointCollection { _layers.Last().OriginPoint, _layers.Last().EndPoint };

                line.Paths.Add(points);
                _layers.Last().Points.Add(_layers.Last().EndPoint);

                if (_layers.Last().Points.Count == 2) _layers.Last().Points.Add(_layers.Last().EndPoint);
                _layers.Last().LineCount++;

                if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
                {
                    var poly = new Polygon();
                    poly.Rings.Add(_layers.Last().Points);
                    _layers.Last().Layer.Graphics[0].Geometry = poly;
                    _layers.Last().Layer.Graphics[0].Symbol = _fillSymbol;
                }

                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totalSymbol = new RotatingTextSymbol();
                    totalSymbol.OffsetY += 25;

                    var totalTextGraphic = new Graphic
                    {
                        Geometry = _layers.Last().OriginPoint,
                        Symbol = totalSymbol
                    };

                    // Bump the Z up so lines will be underneath.
                    totalTextGraphic.SetZIndex(2);

                    _layers.Last().Layer.Graphics.Add(totalTextGraphic);
                }

                var marker = new Graphic
                {
                    Geometry = _layers.Last().EndPoint,
                    Symbol = _markerSymbol
                };

                _layers.Last().Layer.Graphics.Add(marker);

                var lineGraphic = new Graphic
                {
                    Geometry = line,
                    Symbol = LineSymbol
                };

                _layers.Last().Layer.Graphics.Add(lineGraphic);

                var textGraphic = new Graphic
                {
                    Geometry = _layers.Last().EndPoint,
                    Symbol = new RotatingTextSymbol()
                };

                textGraphic.SetZIndex(1);

                _layers.Last().Layer.Graphics.Add(textGraphic);

                if (_layers.Last().Points.Count > 1)
                {

                    var graphics = new List<Graphic>();
                    var graphic1 = new Graphic();
                    var graphic2 = new Graphic();
                    graphic1.Geometry = _layers.Last().PrevOrigin;
                    graphic2.Geometry = _layers.Last().EndPoint;
                    graphics.Add(graphic1);
                    graphics.Add(graphic2);
                }
                else
                {
                    _layers.Last().TotalLength = 0;
                    SegmentLengths = new List<double>();
                }

                _isMeasuring = true;
            }
            _layers.Last().LastClick = pt;

        }

        private void MapMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            if (!_isMeasuring) return;
            if (!_layers.Any()) return;

            var map = sender as Map;
            if (map == null) return;

            int lastone = _layers.Last().Layer.Graphics.Count - 1;
            if (lastone < 0)
            {
                ResetValues();
                return;
            }

            _firstTime = true;
            _finishedSegment = true;
            _layers.Last().Layer.Graphics.RemoveAt(lastone);

            if (MeasureMode == MeasureAction.Mode.Polygon)
            {
                if (_layers.Last().Layer.Graphics.Count < 1)
                {
                    ResetValues();
                    return;
                }

                var poly1 = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
                if (poly1 == null)
                {
                    ResetValues();
                    return;
                }

                MapPoint firstpoint = poly1.Rings[0][0];
                poly1.Rings[0].Add(new MapPoint(firstpoint.X, firstpoint.Y, firstpoint.SpatialReference));

                _layers.Last().Layer.Graphics[0].Geometry = poly1;
                _layers.Last().Layer.Graphics[0].Symbol = _fillSymbol;

                if (_layers.Last().Points.Count > 2)
                {
                    var midpoint = new MapPoint((firstpoint.X + _layers.Last().OriginPoint.X) / 2, (firstpoint.Y + _layers.Last().OriginPoint.Y) / 2);

                    double angle = Math.Atan2((firstpoint.X - _layers.Last().OriginPoint.X), (firstpoint.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
                    if (angle > 90 || angle < -90) angle -= 180;

                    var symb = new RotatingTextSymbol { Angle = angle };

                    var textGraphic = new Graphic
                    {
                        Geometry = midpoint,
                        Symbol = symb
                    };

                    textGraphic.SetZIndex(1);

                    _layers.Last().Layer.Graphics.Add(textGraphic);

                    _finishingSegment = true;
                    _finishMeasure = true;

                    var geometryService = new GeometryService(_geometryService.Url);
                    geometryService.Failed += _geometryService_Failed;
                    geometryService.ProjectCompleted += GeometryServiceAreaFinalSegmentDistanceProjectCompleted;

                    var graphics = new List<Graphic>();
                    var graphic1 = new Graphic();
                    var graphic2 = new Graphic();
                    graphic1.Geometry = _layers.Last().OriginPoint;
                    graphic2.Geometry = firstpoint;
                    graphics.Add(graphic1);
                    graphics.Add(graphic2);
                    geometryService.ProjectAsync(graphics, _map.SpatialReference);

                    _finishingArea = true;

                    // Get the final area.
                    var areaGeometryService = new GeometryService(_geometryService.Url);
                    areaGeometryService.Failed += _geometryService_Failed;
                    areaGeometryService.ProjectCompleted += GeometryServiceFinalAreaProjectCompleted;

                    var polyGraphic = new Graphic { Geometry = poly1 };
                    polyGraphic.Symbol = _fillSymbol;
                    polyGraphic.Geometry.SpatialReference = map.SpatialReference;

                    //Set Extract Polygon

                    _polygon = poly1;
                    Export.IsEnabled = true;

                    var polyGraphicList = new List<Graphic> { polyGraphic };

                    areaGeometryService.ProjectAsync(polyGraphicList, _map.SpatialReference);
                }
            }
            else
            {
                if (!_finishingSegment) ResetValues();
            }

        }

        private void _map_ExtentChanged(object sender, ExtentEventArgs e)
        {
            this.Export.IsEnabled = true;
            PopulateCircuitIds();
        }

        private void _map_MouseMove(object sender, MouseEventArgs e)
        {
            var map = sender as Map;
            if (map == null) return;

            if (!_layers.Any() || _layers.Last().OriginPoint == null || !_isMeasuring) return;

            if (_finishMeasure) return;

            _graphicCount = _layers.Last().Layer.Graphics.Count;
            int g = _graphicCount - 1;

            MapPoint p = _map.ScreenToMap(e.GetPosition(_map));

            // Update the total distance geometry for Polyline mode.
            if (MeasureMode == MeasureAction.Mode.Polyline)
            {
                var totSymGeo = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry;
                if (totSymGeo == null) return;

                totSymGeo = p;

                _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry = totSymGeo;
            }

            // We could use a geometry service.
            #region Geometry Service Length

            var mapDistance = _layers.Last().OriginPoint.DistanceTo(p);
            if (_firstTime)
            {
                _firstTime = false;
                _mapDistance = mapDistance;

                var geometryService = new GeometryService(_geometryService.Url);
                geometryService.Failed += _geometryService_Failed;
                geometryService.ProjectCompleted += GeometryServiceMoveDistanceProjectCompleted;


                var graphics = new List<Graphic>();
                var graphic1 = new Graphic();
                var graphic2 = new Graphic();
                graphic1.Geometry = _layers.Last().OriginPoint;
                graphic2.Geometry = p;
                graphics.Add(graphic1);
                graphics.Add(graphic2);

                geometryService.ProjectAsync(graphics, _map.SpatialReference);
            }
            else
            {
                var dist = _distanceFactor * mapDistance;

                var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
                if (symb != null)
                {
                    var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnit.Feet);

                    symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnit.Feet));
                    _layers.Last().Layer.Graphics[g].Symbol = symb;
                }

                _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                    if (totSym == null) return;

                    totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(_layers.Last().TempTotalLength), PrettyUnits(DistanceUnit.Feet), PrettyUnits(DistanceUnit.Feet));

                    _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
                }
            }

            #endregion Geometry Service Length

            // Or we could estimate.
            #region Estimate Length

            var midpoint = new MapPoint((p.X + _layers.Last().OriginPoint.X) / 2, (p.Y + _layers.Last().OriginPoint.Y) / 2);
            var polypoints = new PointCollection();
            Polygon poly;

            if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
            {
                Graphic graphic = _layers.Last().Layer.Graphics[0];
                poly = graphic.Geometry as Polygon;

                if (poly != null)
                {
                    polypoints = poly.Rings[0];
                    int lastPt = polypoints.Count - 1;
                    polypoints[lastPt] = p;
                }
            }
            _layers.Last().Layer.Graphics[g - 2].Geometry = midpoint;

            var line = _layers.Last().Layer.Graphics[g - 1].Geometry as ESRI.ArcGIS.Client.Geometry.Polyline;
            if (line != null) line.Paths[0][1] = p;
            _layers.Last().Layer.Graphics[g].Geometry = midpoint;

            double angle = Math.Atan2((p.X - _layers.Last().OriginPoint.X), (p.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
            if (angle > 90 || angle < -90) angle -= 180;

            var symbol = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symbol != null)
            {
                symbol.Angle = angle;
                _layers.Last().Layer.Graphics[g].Symbol = symbol;
            }

            if (MeasureMode == MeasureAction.Mode.Polyline)
            {
                var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                if (totSym == null) return;

                _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
            }

            #endregion Estimate Length

            if (MeasureMode != MeasureAction.Mode.Polygon) return;

            if (polypoints.Count <= 2) return;

            poly = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
            if (poly == null) return;

            MapPoint anchor = poly.Extent.GetCenter();

            _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Geometry = anchor;

            // Don't fire too many in mouse move.
            if (_areaAsyncCalls >= 1) return;

            _mouseMove++;
            if (_mouseMove >= 50)
            {
                _mouseMove = 0;
                var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly };
                polyGraphic.Geometry.SpatialReference = map.SpatialReference;
                var polyGraphicList = new List<Graphic> { polyGraphic };
                _areaAsyncCalls++;
            }
        }

        #endregion

        #region Geometry Service Cals

        private void GeometryServiceFinalAreaProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var poly = e.Results[0].Geometry as Polygon;

            var geometryService = new GeometryService(_geometryService.Url);
            geometryService.Failed += _geometryService_Failed;
            geometryService.AreasAndLengthsCompleted += GeometryServiceFinalAreaCompleted;

            var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly };
            polyGraphic.Geometry.SpatialReference = _map.SpatialReference;

            var polyGraphicList = new List<Graphic> { polyGraphic };

            geometryService.AreasAndLengthsAsync(polyGraphicList, LinearUnit.Meter, LinearUnit.Meter, string.Empty);
        }

        private void GeometryServiceFinalAreaCompleted(object sender, AreasAndLengthsEventArgs e)
        {
            // Re-enable clicks
            _finishingArea = false;

            var area = Math.Abs(e.Results.Areas[0]);

            // Set the property now that we have the final area.
            AreaTotal = area;
            _layers.Last().AreaTotal = area;

            if (_layers.Last().Layer.Graphics.Count < 2) return;

            var totSym = _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym != null) totSym.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(ConvertUnits(area, AreaUnit.SquareFeet)), PrettyUnits(AreaUnit.SquareFeet));

            _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol = totSym;

            if (!_finishingSegment && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
        }

        private void GeometryServiceAreaFinalSegmentDistanceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var geometryService = new GeometryService(_geometryService.Url);
            geometryService.Failed += _geometryService_Failed;
            geometryService.DistanceCompleted += GeometryServiceAreaFinalSegmentDistanceCompleted;

            int g = _layers.Last().Layer.Graphics.Count - 1;

            var distParams = new DistanceParameters
            {
                DistanceUnit = LinearUnit.Meter,
                Geodesic = true
            };

            geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
        }

        private void GeometryServiceAreaFinalSegmentDistanceCompleted(object sender, DistanceEventArgs e)
        {
            // Re-enable clicks
            _finishingSegment = false;

            var g = (int)e.UserState;
            var layer = _layers.Last().Layer.Graphics.Count < g + 1 ? _layers[_layers.Count - 2] : _layers.Last();

            var dist = e.Distance;

            SegmentLengths.Add(dist);
            layer.SegmentLengths.Add(dist);

            // Used to trigger Property Change.
            SegmentLengths = new List<double>(SegmentLengths);

            if (layer.Layer.Graphics.Count < g + 1) return;

            var symb = layer.Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symb == null) return;

            var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnit.Feet); ;

            symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnit.Feet));
            layer.Layer.Graphics[g].Symbol = symb;

            if (!_finishingArea && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
        }

        private void GeometryServiceMoveDistanceProjectCompleted(object sender, GraphicsEventArgs e)
        {

            var geometryService = new GeometryService(_geometryService.Url);
            geometryService.Failed += _geometryService_Failed;
            geometryService.DistanceCompleted += GeometryServiceMoveDistanceCompleted;

            int g = _layers.Last().Layer.Graphics.Any() ? _layers.Last().Layer.Graphics.Count - 1 : _layers[_layers.Count - 2].Layer.Graphics.Count - 1;

            var distParams = new DistanceParameters
            {
                DistanceUnit = LinearUnit.Meter,
                Geodesic = true
            };

            geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
        }

        private void GeometryServiceMoveDistanceCompleted(object sender, DistanceEventArgs e)
        {
            var dist = e.Distance;
            _distanceFactor = dist / _mapDistance;

            var g = (int)e.UserState;

            if (_layers.Last().Layer.Graphics.Count < g + 1) return;

            var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symb != null)
            {
                var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnit.Feet);

                symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnit.Feet));
                _layers.Last().Layer.Graphics[g].Symbol = symb;
            }

            _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

            if (MeasureMode != MeasureAction.Mode.Polyline) return;

            var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym == null) return;

            totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(ConvertMetersToActiveUnits(_layers.Last().TempTotalLength, DistanceUnit.Feet)), PrettyUnits(DistanceUnit.Feet));

            _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
        }

        private void _geometryService_SimplifyCompleted(object sender, GraphicsEventArgs e)
        {
            _geometryService.SimplifyCompleted -= _geometryService_SimplifyCompleted;
            _geometryService.Failed -= _geometryService_Failed;

            // Multiple rings will be a no-go for dispatching to FME so let's check. Simplify creates multiple rings
            Polygon polygon = e.Results[0].Geometry as Polygon;

            if (polygon.Rings.Count == 1)
            {
                MeasureAreaAsync(e.Results[0]);
            }
            else
            {
                MessageBox.Show("Intersecting Polygons are not allowed", "Invalid Polygon", MessageBoxButton.OK);
            }
        }

        private void _geometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            _geometryService.Failed -= _geometryService_Failed;
            MessageBox.Show("An error occurred in Exporting (geoprocessing)");
        }

        private void _geometryService_AreasAndLengthsCompleted(object sender, AreasAndLengthsEventArgs args)
        {
            _geometryService.AreasAndLengthsCompleted -= _geometryService_AreasAndLengthsCompleted;
            _geometryService.Failed -= _geometryService_Failed;

            double sqft = Math.Abs(args.Results.Areas[0]);

            if (ValidateSquareFeet(sqft))
            {
                // On with the Chained GP Call Show...to buffer
                BufferAsync();
            }
            else
            {
                ResetGraphics();
            }

        }

        private void _geometryService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            _geometryService.BufferCompleted -= _geometryService_BufferCompleted;
            _geometryService.Failed -= _geometryService_Failed;

            // Passed all 3 tests! ready for FME
            Polygon polygon = e.Results[0].Geometry as Polygon;

            SendPolygonToFME(polygon);
        }

        #endregion

        public void SetDefaultScale(string storedDisplayName)
        {
            Dictionary<string, string> scales = cboScale.ItemsSource as Dictionary<string, string>;
            if (storedDisplayName.Contains(" 50") && cboScale.SelectedItem.ToString() != "50")
            {
                cboScale.SelectedItem = scales.Where(s => s.Key == "50").First();
            }
            else if (!storedDisplayName.Contains(" 50") && ((KeyValuePair<string, string>)cboScale.SelectedItem).Key == "50" &&
                !storedDisplayName.Contains("Schematics"))
            {
                cboScale.SelectedItem = scales.Where(s => s.Key == "100").First();
            }

        }


        private static string PrettyUnits(AreaUnit unit)
        {
            string display = string.Empty;

            // (Alt+0178) is the ASCII for superscript 2

            switch (unit)
            {
                case AreaUnit.SquareMiles:
                    display = "mi²";
                    break;
                case AreaUnit.SquareKilometers:
                    display = "km²";
                    break;
                case AreaUnit.SquareFeet:
                    display = "ft²";
                    break;
                case AreaUnit.SquareMeters:
                    display = "m²";
                    break;
                case AreaUnit.Acres:
                    display = "a";
                    break;
                case AreaUnit.Hectares:
                    display = "ha";
                    break;
            }

            return display;
        }

        private static double ConvertUnits(double item, AreaUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case AreaUnit.SquareMiles:
                    convertedItem = item * SqMetersToSqMiles;
                    break;
                case AreaUnit.SquareFeet:
                    convertedItem = item * SqMetersToSqFeet;
                    break;
                case AreaUnit.SquareKilometers:
                    convertedItem = item / 1000000;
                    break;
                case AreaUnit.SquareMeters:
                    convertedItem = item;
                    break;
                case AreaUnit.Hectares:
                    convertedItem = item * SqMetersToHectare;
                    break;
                case AreaUnit.Acres:
                    convertedItem = item * SqMetersToAcre;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }

        #region Private Methods

        private void ResetValues()
        {
            _layers.Last().MeasureMode = MeasureMode;
            _isMeasuring = false;
            _finishMeasure = false;

            //_layers.Add(new SegmentLayer());
            //_map.Layers.Add(_layers.Last().Layer);
        }

        private void ClearPreviousLayers()
        {
            //This is to retain previously created grahpic 

            if (_layers.Count > 1)
            {

                for (int i = 0; i < _layers.Count - 1; i++)
                {
                    var layer = _layers[i];
                    layer.Layer.ClearGraphics();
                    _map.Layers.Remove(layer.Layer);
                }
                _layers.RemoveAt(0);
            }
        }

        private void ClearLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Layer.ClearGraphics();
                _map.Layers.Remove(layer.Layer);
            }

            _layers.Clear();
        }

        private double DistanceTo(MapPoint start, MapPoint end)
        {

            double x = Math.Abs(start.X - end.X);
            double y = Math.Abs(start.Y - end.Y);
            double dist = Math.Sqrt((x * x) + (y * y));

            return dist;
        }

        private double ConvertMetersToActiveUnits(double dist, DistanceUnit DistanceUnits)
        {
            const double metersToMiles = 0.0006213700922;
            const double metersToFeet = 3.280839895;

            double displayMeasurement = dist;

            // Assume from meters.
            switch (DistanceUnits)
            {
                case DistanceUnit.Miles:
                    displayMeasurement = dist * metersToMiles;
                    break;
                case DistanceUnit.Kilometers:
                    displayMeasurement = dist / 1000;
                    break;
                case DistanceUnit.Feet:
                    displayMeasurement = dist * metersToFeet;
                    break;
                case DistanceUnit.Meters:
                    displayMeasurement = dist;
                    break;
            }

            return displayMeasurement;
        }

        private string PrettyUnits(DistanceUnit unit)
        {
            string display = string.Empty;

            switch (unit)
            {
                case DistanceUnit.Miles:
                    display = "mi";
                    break;
                case DistanceUnit.Kilometers:
                    display = "km";
                    break;
                case DistanceUnit.Feet:
                    display = "ft";
                    break;
                case DistanceUnit.Meters:
                    display = "m";
                    break;
            }

            return display;
        }

        private double RoundToSignificantDigit(double value)
        {
            int SignificantDigits = 2;
            int roundDigits = (SignificantDigits - 2) >= 0 ? SignificantDigits - 2 : 0;

            if (value == 0) return 0;
            else if (value < .01)
            {
                roundDigits = SignificantDigits;
                while (roundDigits < 15 && Math.Round(value, ++roundDigits) == 0) ;
            }
            else if (value < 1) roundDigits = SignificantDigits; // Was 3 but changed per DE254
            else if (value < 10) roundDigits = SignificantDigits;
            else if (value < 100) roundDigits = SignificantDigits - 1;

            double round = value >= 100 ? Math.Round(value) : Math.Round(value, roundDigits);

            return round;
        }

        private void DrawControl_DrawComplete(object sender, DrawEventArgs args)
        {
            Export.IsEnabled = true;
            ToolTipService.SetToolTip(Export, "Export to CAD.");

            if (args.Geometry is ESRI.ArcGIS.Client.Geometry.Envelope)
            {
                _envelope = args.Geometry as ESRI.ArcGIS.Client.Geometry.Envelope;
                _polygon = GetPolygonFromEnvelope(args.Geometry);
            }
            else
            {
                _polygon = args.Geometry as ESRI.ArcGIS.Client.Geometry.Polygon;
                _polygon.SpatialReference = _map.SpatialReference;
                _envelope = null;
            }

            DrawExtractedArea(args.Geometry);

        }

        void SimplifyAsync(Graphic graphic)
        {
            _geometryService.SimplifyCompleted += new EventHandler<GraphicsEventArgs>(_geometryService_SimplifyCompleted);
            _geometryService.Failed += new EventHandler<TaskFailedEventArgs>(_geometryService_Failed);

            IList<Graphic> graphicList = new List<Graphic>();
            graphicList.Add(graphic);
            _geometryService.SimplifyAsync(graphicList);
        }

        void MeasureAreaAsync(Graphic graphic)
        {
            // Ship it off to measure the area
            _geometryService.AreasAndLengthsCompleted += new EventHandler<AreasAndLengthsEventArgs>(_geometryService_AreasAndLengthsCompleted);
            _geometryService.Failed += new EventHandler<TaskFailedEventArgs>(_geometryService_Failed);
            IList<Graphic> graphicList = new List<Graphic>();
            graphicList.Add(graphic);
            _geometryService.AreasAndLengthsAsync(graphicList);
        }

        private bool ValidateSquareFeet(double sqft)
        {
            ComboBoxItem cbExportFormatItem = (ComboBoxItem)cboExportFormat.SelectedItem;
            string selectedExportFormat = cbExportFormatItem.Content.ToString();
            if (selectedExportFormat == "DWG" || selectedExportFormat == "DWG+PNG")
            {
                if (lblSD.Content.ToString().ToUpper().Contains("SCHEMATICS"))
                {

                    if (sqft > _circuitmaximumAreaSquareFeet)
                    {
                        MessageBox.Show("Areas over " + _circuitmaximumAreaSquareFeet.ToString("#,##0") + " sqFt are not allowed\nYour area was " + sqft.ToString("#,##0") + " sqFt\nPlease re-select area", "Area Too Large", MessageBoxButton.OK);
                        return false;
                    }
                }

                else
                {
                    if (sqft > _maximumAreaSquareFeet)
                    {
                        MessageBox.Show("Areas over " + _maximumAreaSquareFeet.ToString("#,##0") + " sqFt are not allowed\nYour area was " + sqft.ToString("#,##0") + " sqFt\nPlease re-select area", "Area Too Large", MessageBoxButton.OK);
                        return false;
                    }
                }
            }

            return true;
        }

        private void BufferAsync()
        {
            _geometryService.BufferCompleted += new EventHandler<GraphicsEventArgs>(_geometryService_BufferCompleted);
            _geometryService.Failed += new EventHandler<TaskFailedEventArgs>(_geometryService_Failed);
            BufferParameters bufferParams = new BufferParameters()
            {
                Unit = LinearUnit.Foot,
                BufferSpatialReference = _map.SpatialReference,
                OutSpatialReference = _map.SpatialReference
            };
            bufferParams.Features.Add(_geometryService.SimplifyLastResult[0]);
            bufferParams.Distances.Add(_bufferGeometryFeet);

            _geometryService.BufferAsync(bufferParams);

        }

       
        private void callED50PNGGPService(Polygon polygon)
        {
            double scale = Convert.ToDouble(((KeyValuePair<string, string>)this.cboScale.SelectedItem).Key);
           // ComboBoxItem cbItem = (ComboBoxItem)cboExportLayout.SelectedItem;
            string selectedExportLayout = cboExportLayout.SelectedValue.ToString();
            string fmePolygonCoordinates = PolygonToFMECoordinates(polygon);
            string outputfileName = GetSanitizedFileName();
            string email = txtSendTo.Text + "@pge.com";
          //should be from UI. Values shall be 
            //PNG_8-5x11_Landscape PNG_8-5x11_Portrait PNG_11x17_Landscape PNG_11X17_Portrait PNG_22x34_Landscape PNG_22x34_Portrait PNG_34x44_Landscape PNG_34x44_Portrait
            string adHocTemplateNameOnServer = selectedExportLayout;
            ComboBoxItem cbExportFormatItem = (ComboBoxItem)cboExportFormat.SelectedItem;
            string selectedExportFormat = cbExportFormatItem.Content.ToString();
            string facilityID = "-1";
            GetPrintServices();
            string PrintServiceUrl = _syncPrintService;
            PngExport = new PngExport(new Uri(PrintServiceUrl));
            PngExport.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(PngExport_PrintJobSubmitted);
            Map PNGExportMap = _map;
            Envelope pngExportExtent = null;
            if (_schematicsPrintPreviewExtent != null)
            {
                pngExportExtent = _schematicsPrintPreviewExtent;
            }
            else
            {
                if (_polygon != null)
                {
                    pngExportExtent = _polygon.Extent;
                }
                else
                {
                    pngExportExtent = PNGExportMap.Extent;
                }
            }
            //PNGExportMap = updateMapLayers("Elec Dist 50 Scale Primary View", _map);
            List<string> mapTypelst = _MapTypeList;
            List<string> mapList=new List<string>();
            for (int i = 0; i < mapTypelst.Count; i++)
            {
                if (mapTypelst[i] == "Primary View")
                {
                    string temp = mapTypelst[i].Replace(" View", string.Empty);
                  //  mapTypelst[i] = temp.ToUpper();
                    mapList.Add(temp.ToUpper());
                }
                else if (mapTypelst[i] == "Secondary View")
                {
                    string temp = mapTypelst[i].Replace(" View", string.Empty);
                   // mapTypelst[i] = temp.ToUpper();
                    mapList.Add(temp.ToUpper());
                }
                else if (mapTypelst[i] == "StreetLight")
                {
                    string temp = mapTypelst[i];
                   // mapTypelst[i] = temp.ToUpper();
                    mapList.Add(temp.ToUpper());
                }
                else if(mapTypelst[i] == "Duct View")
                {
                    string temp = mapTypelst[i].Replace(" ", string.Empty);
                   // mapTypelst[i] = temp.ToUpper();
                    mapList.Add(temp.ToUpper());
                }
                else if (mapTypelst[i] == "DC View")
                {
                    string temp = mapTypelst[i].Replace(" ", string.Empty);
                   // mapTypelst[i] = temp.ToUpper();
                    mapList.Add(temp.ToUpper());
                }
                else if (mapTypelst[i] == "ED")
                {
                    CallEDFMEService(polygon);
                }

            }
            String[] strMapTypes = mapList.ToArray();

            TextSymbol textSymbol;
            if (selectedExportFormat == "DWG")
            {
                textSymbol = _textSymbol;
            }
            else if (selectedExportFormat == "PNG")
            {
                textSymbol = _pngTextSymbol;
            }
            else
            {
                textSymbol = _pngDwgTextSymbol;
            }

           
            PngExport.strDcviewURL = _cadparams.DCURL;
            PngExport.strStreetLightURL=_cadparams.StreetLightURL;
            PngExport.lstButterflyVisibleLayers = _cadparams.strButterflyLayers;
            PngExport.lstLandBaseVisibleLayers = _cadparams.strLandbaseLayers;
            PngExport.FMEPNGExportURL = _cadparams.strFMEPNGExportUrl;
            if (mapList.Count > 0)
            {
                PngExport.PrintED50(fmePolygonCoordinates, scale, outputfileName, email, selectedExportFormat, facilityID, PNGExportMap, selectedExportLayout, pngExportExtent, null, "PNG32", 300, false, strMapTypes, textSymbol, _graphicsLayer, polygon);
            }
                
        }

        void PngExport_PrintJobSubmitted(object sender, PrintJobSubmittedEventArgs e)
        {
            string _printJobId = string.Empty;
            _printJobId = e.JobId;
            PngExport.PrintJobSubmitted -= PngExport_PrintJobSubmitted;
        }



        private void GetPrintServices()
        {
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            _syncPrintService =
                Convert.ToString(xe.Element("PrintServiceUrls").Element("PrintServiceUrl").Attribute("Url").Value);

        }

      
       
       

        

        private void CallEDFMEService(Polygon polygon)
        {
            int scale = Convert.ToInt32(((KeyValuePair<string, string>)this.cboScale.SelectedItem).Key);

            string _layerNames = "";
            string _storedViewName = "";

            _storedViewName = _mapTools.StoredViewControl._selectedStoredView.StoredViewName;

            Dictionary<string, string> _assetLayers;
            Dictionary<string, string> _landbaseLayer;

            _landbaseLayer = _cadparams.LandbaseLayers;

            List<int> _visibleLayerIdsFromService = null;

            string _visibleLyrNames = "";

            if (_storedViewName == "Electric Distribution")
            {
                if (((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers != null)
                {
                    _visibleLayerIdsFromService = ((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers.ToList();
                }

                _assetLayers = _cadparams.EDLayers;

            }
            else if (_storedViewName == "ED EDMaster")
            {
                if (((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers != null)
                {
                    _visibleLayerIdsFromService = ((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers.ToList();
                }
                _assetLayers = _cadparams.EDMasterLayers;

            }
            else
            {
                _storedViewName = "Electric Distribution";
                if (((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers != null)
                {
                    _visibleLayerIdsFromService = ((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers[_storedViewName])).VisibleLayers.ToList();
                }

                _assetLayers = _cadparams.EDLayers;

            }

            if (_visibleLayerIdsFromService == null)
            {
                foreach (string key in _assetLayers.Keys)
                {
                    _visibleLyrNames = _visibleLyrNames + ":" + key;
                }


            }
            else
            {
                //Get ED - AutoCAD Mapping from ED Visible Layer 
                foreach (string key in _assetLayers.Keys)
                {
                    string[] _edLyrsids = _assetLayers[key].ToString().Split(',');
                    for (int iCount = 0; iCount < _edLyrsids.Length; iCount++)
                    {
                        if (_visibleLayerIdsFromService.Contains(Convert.ToInt16(_edLyrsids[iCount])))
                        {
                            _visibleLyrNames = _visibleLyrNames + ":" + key;
                            break;
                        }
                    }
                }


            }


            _visibleLayerIdsFromService = null;

            if (((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers["Commonlandbase"])).VisibleLayers != null)
            {
                _visibleLayerIdsFromService = ((ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer)(_map.Layers["Commonlandbase"])).VisibleLayers.ToList();
            }

            if (_visibleLayerIdsFromService == null)
            {
                foreach (string key in _landbaseLayer.Keys)
                {
                    _visibleLyrNames = _visibleLyrNames + ":" + key;
                }
            }
            else
            {
                //Get Landbase - AutoCAD Mapping from ED Visible Layer 

                foreach (string key in _landbaseLayer.Keys)
                {
                    string[] _landLyrsids = _landbaseLayer[key].ToString().Split(',');
                    for (int iCount = 0; iCount < _landLyrsids.Length; iCount++)
                    {
                        if (_visibleLayerIdsFromService.Contains(Convert.ToInt16(_landLyrsids[iCount])))
                        {
                            _visibleLyrNames = _visibleLyrNames + ":" + key;
                            break;
                        }
                    }
                }
            }


            string fmePolygonCoordinates = PolygonToFMECoordinates(polygon);
            _textPosition = polygon.Extent.GetCenter();
            Dictionary<string, string> restParameters = new Dictionary<string, string>();
            restParameters.Add("opt_showresult", "false");
            restParameters.Add("opt_servicemode", "async");

            restParameters.Add("output_file_name", GetSanitizedFileName());
            restParameters.Add("scale", scale.ToString());
            restParameters.Add("MAPTYPE", "ED");
            restParameters.Add("EMAIL", txtSendTo.Text + "@pge.com");

            WebClient wc = new WebClient();
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringCompleted);
            wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
            // For some reason, "+" doesn't work in place of spaces for FME Server but %20 does

            if (cboWYSWYG.SelectedIndex != 0)
            {
                restParameters.Add("LayerIds", _visibleLyrNames);
                wc.UploadStringAsync(new Uri(_exportWYSWYGRESTService), "POST", "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + fmePolygonCoordinates.Replace(" ", "%20"));
            }
            else
            {
                string data = "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + fmePolygonCoordinates.Replace(" ", "%20");
                wc.UploadStringAsync(new Uri(_exportRESTService), "POST", "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + fmePolygonCoordinates.Replace(" ", "%20"));
            }
        }

        private void CallSCHEMATICService(Polygon polygon)
        {
            Query _query = new Query();
            _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _query.ReturnGeometry = false;
            _query.Geometry = polygon;
            _query.OutFields.AddRange(new string[] { "MAPNO", "ALPHA", "NUM" });
            _polygon = polygon;

            _schemticqueryTaskCount = 0;
            _exportedgridno = "";
            for (int i = 0; i < _schemticqueryTaskList.Count(); i++)   //UMG layer divided into 5 - INC000004402997, INC000004402999
            {
                _schemticqueryTaskList[i].ExecuteAsync(_query);
            }
        }

        /***********************************31Oct2016 Start*******************************/
        // function to add checkboxes for different view when ED 50 scale is selected
        public void populateVoltageFilterDropdown()
        {
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "Schematics")
                    {
                        if (element.HasElements)
                        {
                            string codedDescriptionsCSV =
                                element.Element("SchematicsFilter")
                                    .Element("PrimaryVoltageDomain")
                                    .Attribute("CodedDescriptionsCSV")
                                    .Value;
                            string codedValuesCSV =
                                element.Element("SchematicsFilter").Element("PrimaryVoltageDomain").Attribute("CodedValuesCSV").Value;
                            IList<string> codedDescriptions = codedDescriptionsCSV.Split(',').ToList();
                            IList<string> codedValues = codedValuesCSV.Split(',').ToList();
                            Dictionary<string, string> primaryVoltageDomainDictionary =
                                codedValues.Zip(codedDescriptions, (k, v) => new { k, v })
                                    .ToDictionary(x => x.k, x => x.v);
                            Dictionary<string, string> placeHolder = new Dictionary<string, string>() {{"", "--Select--"}
        };
                            Dictionary<string, string> voltageDictionary = placeHolder.Concat(primaryVoltageDomainDictionary).ToDictionary(x => x.Key, x => x.Value);
                            cboSchematicVoltageFilter.ItemsSource = voltageDictionary;
                            cboSchematicVoltageFilter.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        public void populateFeederFilterDropdown()
        {
            string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "CADFeederFilter")
                    {
                        if (element.HasElements)
                        {
                            string codedDescriptionsCSV =
                                 element.Element("FeederFilterDomain")
                                     .Attribute("CodedDescriptionsCSV")
                                     .Value;
                            string codedValuesCSV =
                                element.Element("FeederFilterDomain").Attribute("CodedValuesCSV").Value;
                            IList<string> codedDescriptions = codedDescriptionsCSV.Split(',').ToList();
                            IList<string> codedValues = codedValuesCSV.Split(',').ToList();
                            Dictionary<string, string> feederDomainDictionary =
                                codedValues.Zip(codedDescriptions, (k, v) => new { k, v })
                                    .ToDictionary(x => x.k, x => x.v);
                            Dictionary<string, string> placeHolder = new Dictionary<string, string>() {{"", "--Select--"}
        };
                            Dictionary<string, string> feederDictionary = placeHolder.Concat(feederDomainDictionary).ToDictionary(x => x.Key, x => x.Value);
                            cboSchematicFeederFilter.ItemsSource = feederDictionary;
                            cboSchematicFeederFilter.SelectedIndex = 0;
                        
                        }
                    }
                }
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
                        cboExportLayout.ItemsSource = feederDictionary;
                        cboExportLayout.SelectedIndex = 0;
                        _selectedPageTemplate = cboExportLayout.SelectedValue.ToString();
                    }
                }
            }
        }

        public void populatePNGMapType()
        {
            if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("DIST") && _mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("50"))
            {

                _allStoredViewList = new ObservableCollection<ListOfRecords>();

                try
                {
                    if (Application.Current.Host.InitParams.ContainsKey("Config"))
                    {
                        string config = Application.Current.Host.InitParams["Config"];
                        //var attribute = "";
                        config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                        XElement elements = XElement.Parse(config);
                        foreach (XElement element in elements.Elements())
                        {
                            if (element.Name.LocalName == "DWGMapType")
                            {
                                if (element.HasElements)
                                {
                                    var i = 0;
                                    foreach (XElement childelement in element.Elements())
                                    {
                                        if (childelement.LastAttribute.Value == "ED")
                                        {
                                        }
                                        else
                                        {
                                            ListOfRecords l = new ListOfRecords()
                                            {
                                                ID = i,
                                                Content = childelement.LastAttribute.Value,
                                                IsSelected = true,
                                                BrushObj = new SolidColorBrush(Colors.Black)
                                            };
                                            _allStoredViewList.Add(l);
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                // AllobjList = _allobjList;

                mapTypeList.ItemsSource = AllStoredViewList;

            }
        }

        public void populateMapType()
        {
            if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("DIST") && _mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("50"))
            {

                _allStoredViewList = new ObservableCollection<ListOfRecords>();

                try
                {
                    if (Application.Current.Host.InitParams.ContainsKey("Config"))
                    {
                        string config = Application.Current.Host.InitParams["Config"];
                        //var attribute = "";
                        config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                        XElement elements = XElement.Parse(config);
                        foreach (XElement element in elements.Elements())
                        {
                            if (element.Name.LocalName == "DWGMapType")
                            {
                                if (element.HasElements)
                                {
                                    var i = 0;
                                    foreach (XElement childelement in element.Elements())
                                    {
                                        ListOfRecords l = new ListOfRecords()
                                        {
                                            ID = i,
                                            Content = childelement.LastAttribute.Value,
                                            IsSelected = true,
                                            BrushObj = new SolidColorBrush(Colors.Black)
                                        };
                                        _allStoredViewList.Add(l);
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { }

                // AllobjList = _allobjList;

                mapTypeList.ItemsSource = AllStoredViewList;

            }
        }

        private void MapTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            _MapType = "";

            _MapTypeList = new List<string>();

            foreach (var item in mapTypeList.ItemsSource)
            {

                if (((ListOfRecords)item).IsSelected || ((ListOfRecords)item).Content == cb.Content.ToString())
                {

                    _MapTypeList.Add(((ListOfRecords)item).Content);

                    FormatString();

                }



            }


        }

        private void MapTypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            _MapType = "";
            _MapTypeList = new List<string>();
            foreach (var item in mapTypeList.ItemsSource)
            {

                if (((ListOfRecords)item).IsSelected && ((ListOfRecords)item).Content != cb.Content.ToString())
                {
                    _MapTypeList.Add(((ListOfRecords)item).Content);
                    FormatString();

                }


            }


        }

        private void FormatString()
        {

            _MapType = String.Join(":", _MapTypeList);
            _MapType = _MapType.Replace(" ", String.Empty);
            _MapType = _MapType.ToUpper();

        }

        private void schematicFilterCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (cboSchematicFilter != null && cboSchematicFilter.Visibility == Visibility.Visible)
            //{
            //    ComboBoxItem cbItem = (ComboBoxItem)cboSchematicFilter.SelectedItem;
            //    string selectedExportFilter = cbItem.Content.ToString();
            //    ComboBoxItem cbExportFormatItem = (ComboBoxItem)cboExportFormat.SelectedItem;
            //    string selectedExportFormat = cbExportFormatItem.Content.ToString();
            //    if (selectedExportFilter == "Voltage")
            //    {
            //        lblSchematicFilterType.Visibility = Visibility.Visible;
            //        lblSchematicFilterType.Content = "Voltage";
            //        cboSchematicVoltageFilter.Visibility = Visibility.Visible;
            //        cboSchematicFeederFilter.Visibility = Visibility.Collapsed;
            //        if (selectedExportFormat == "DWG")
            //        {
            //            this.Height = 420;
            //            this.stdwgPanel.Height = 420;
            //            _dwgFloatWindow.Height = 420;
            //        }
            //        else if (selectedExportFormat == "PNG")
            //        {
            //            this.Height = 340;
            //            this.stdwgPanel.Height = 340;
            //            _dwgFloatWindow.Height = 350;
            //        }
            //        else
            //        {
            //            this.Height = 460;
            //            this.stdwgPanel.Height = 460;
            //            _dwgFloatWindow.Height = 470;
            //        }
                    
            //    }
            //    else if (selectedExportFilter == "Feeder Type")
            //    {
            //        lblSchematicFilterType.Visibility = Visibility.Visible;
            //        lblSchematicFilterType.Content = "Feeder Type";
            //        cboSchematicFeederFilter.Visibility = Visibility.Visible;
            //        cboSchematicVoltageFilter.Visibility = Visibility.Collapsed;
            //        if (selectedExportFormat == "DWG")
            //        {
            //            this.Height = 420;
            //            this.stdwgPanel.Height = 420;
            //            _dwgFloatWindow.Height = 420;
            //        }
            //        else if (selectedExportFormat == "PNG")
            //        {
            //            this.Height = 340;
            //            this.stdwgPanel.Height = 340;
            //            _dwgFloatWindow.Height = 350;
            //        }
            //        else
            //        {
            //            this.Height = 460;
            //            this.stdwgPanel.Height = 460;
            //            _dwgFloatWindow.Height = 470;
            //        }
            //    }
            //    else
            //    {
            //        lblSchematicFilterType.Visibility = Visibility.Collapsed;
            //        lblSchematicFilterType.Content = "";
            //        cboSchematicFeederFilter.Visibility = Visibility.Collapsed;
            //        cboSchematicVoltageFilter.Visibility = Visibility.Collapsed;
            //        if (selectedExportFormat == "DWG")
            //        {
            //            this.Height = 400;
            //            this.stdwgPanel.Height = 400;
            //            _dwgFloatWindow.Height = 400;
            //        }
            //        else if (selectedExportFormat == "PNG")
            //        {
            //            this.Height = 320;
            //            this.stdwgPanel.Height = 320;
            //            _dwgFloatWindow.Height = 330;
            //        }
            //        else
            //        {
            //            this.Height = 420;
            //            this.stdwgPanel.Height = 420;
            //            _dwgFloatWindow.Height = 430;
            //        }
            //    }
            //}
            
        }

        private void schematicVoltageFilterCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        private void schematicFeederFilterCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        private void exportLayoutCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            printPreviewBtn.Background = new SolidColorBrush(Color.FromArgb(255, 233, 233, 233));
        }

        private void exportScaleCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                 }

        private void exportFormatCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cboExportFormat != null)
            {
                ComboBoxItem cbItem = (ComboBoxItem)cboExportFormat.SelectedItem;
                string selectedExportFormat = cbItem.Content.ToString();
                _schematicsPrintPreviewExtent = null;
                printPreviewBtn.Background = new SolidColorBrush(Color.FromArgb(255, 233, 233, 233));
                if (selectedExportFormat == "PNG")
                {
                    printPreviewBtn.Background = new SolidColorBrush(Color.FromArgb(255, 233, 233, 233));
                    cboExportLayout.Visibility = System.Windows.Visibility.Visible;
                    lblExportLayout.Visibility = System.Windows.Visibility.Visible;
                    cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    printPreviewBtn.Visibility = Visibility.Visible;
                    populatePNGMapType();
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    {
                        cboScale.ItemsSource = _cadparams.SchematicPNGScales;
                        cboScale.DisplayMemberPath = "Value";
                        cboScale.SelectedValuePath = "Key";
                        if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("250"))
                        {
                            cboScale.SelectedItem = _cadparams.SchematicPNGScales.Where(s => s.Key == _cadparams.DefaultSchematic250Scale).First();
                        }
                        else
                        {
                            cboScale.SelectedItem = _cadparams.SchematicPNGScales.Where(s => s.Key == _cadparams.DefaultSchematicScale).First();
                        }
                       // cboScale.SelectedItem = _cadparams.SchematicPNGScales.Where(s => s.Key == _cadparams.DefaultSchematicScale).First();
                        lstAll.Visibility = System.Windows.Visibility.Collapsed;
                        lstCircuitIds.Visibility = System.Windows.Visibility.Collapsed;
                        lblCircuits.Visibility = System.Windows.Visibility.Collapsed;
                        printPreviewBtn.Visibility = Visibility.Visible;
                        //lblVoltage.Visibility = System.Windows.Visibility.Collapsed;
                        //cboVoltageFilter.Visibility = System.Windows.Visibility.Collapsed;
                        //cboFeederType.Visibility = System.Windows.Visibility.Collapsed;
                        //lblFeederType.Visibility = System.Windows.Visibility.Collapsed;
                        cboSchematicFeederFilter.Visibility = Visibility.Collapsed;
                        cboSchematicVoltageFilter.Visibility = Visibility.Collapsed;
                        lblSchematicFilterVoltage.Visibility = Visibility.Collapsed;
                        lblSchematicFilterFeeder.Visibility = Visibility.Collapsed;
                        if (lblFilterType.Content =="" && lblFilterValue.Content == "")
                        {
                            lblFilterType.Visibility = Visibility.Collapsed;
                            lblFilterValue.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            lblFilterType.Visibility = Visibility.Visible;
                            lblFilterValue.Visibility = Visibility.Visible;
                        }
                        if (cboSchematicFeederFilter.Visibility == Visibility.Visible || cboSchematicVoltageFilter.Visibility == Visibility.Visible)
                        {
                            this.Height = 300;
                            this.stdwgPanel.Height = 300;
                            _dwgFloatWindow.Height = 300;
                        }
                        else
                        {
                            this.Height = 310;
                            this.stdwgPanel.Height = 310;
                            _dwgFloatWindow.Height = 310;
                        }

                    }



                }
                else if (selectedExportFormat == "DWG")
                {
                    cboExportLayout.Visibility = System.Windows.Visibility.Collapsed;
                    lblExportLayout.Visibility = System.Windows.Visibility.Collapsed;
                    cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    populateMapType();
                    printPreviewBtn.Visibility = Visibility.Collapsed;
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    {

                        cboScale.ItemsSource = _cadparams.SchematicScales;
                        cboScale.DisplayMemberPath = "Value";
                        cboScale.SelectedValuePath = "Key";
                        if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("250"))
                        {
                            cboScale.SelectedItem = _cadparams.SchematicScales.Where(s => s.Key == _cadparams.DefaultSchematic250Scale).First();
                        }
                        else
                        {
                            cboScale.SelectedItem = _cadparams.SchematicScales.Where(s => s.Key == _cadparams.DefaultSchematicScale).First();
                        }
                        cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                        lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                        lblFilterType.Visibility = Visibility.Collapsed;
                        lblFilterValue.Visibility = Visibility.Collapsed;
                        if (lblFilterType.Visibility == Visibility.Collapsed && lblFilterValue.Visibility == Visibility.Collapsed)
                        {
                            lstAll.Visibility = System.Windows.Visibility.Visible;
                            lstCircuitIds.Visibility = System.Windows.Visibility.Visible;
                            lblCircuits.Visibility = System.Windows.Visibility.Visible;
                            cboSchematicFeederFilter.Visibility = Visibility.Visible;
                            cboSchematicVoltageFilter.Visibility = Visibility.Visible;
                            lblSchematicFilterVoltage.Visibility = Visibility.Visible;
                            lblSchematicFilterFeeder.Visibility = Visibility.Visible;
                            if (cboSchematicFeederFilter.Visibility == Visibility.Visible || cboSchematicVoltageFilter.Visibility == Visibility.Visible)
                            {
                                this.Height = 420;
                                this.stdwgPanel.Height = 420;
                                _dwgFloatWindow.Height = 420;
                            }
                            else
                            {
                                this.Height = 410;
                                this.stdwgPanel.Height = 410;
                                _dwgFloatWindow.Height = 410;
                            }
                           
                        }
                        //lblVoltage.Visibility = System.Windows.Visibility.Visible;
                        //cboVoltageFilter.Visibility = System.Windows.Visibility.Visible;
                        //cboFeederType.Visibility = System.Windows.Visibility.Visible;
                        //lblFeederType.Visibility = System.Windows.Visibility.Visible;


                    }


                }
                else
                {
                    cboExportLayout.Visibility = System.Windows.Visibility.Visible;
                    lblExportLayout.Visibility = System.Windows.Visibility.Visible;
                    cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    printPreviewBtn.Visibility = Visibility.Visible;
                    printPreviewBtn.Background = new SolidColorBrush(Color.FromArgb(255, 233, 233, 233));
                    populatePNGMapType();
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    {
                        printPreviewBtn.Visibility = Visibility.Visible;
                        cboScale.ItemsSource = _cadparams.SchematicScales;
                        cboScale.DisplayMemberPath = "Value";
                        cboScale.SelectedValuePath = "Key";
                        if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("250"))
                        {
                            cboScale.SelectedItem = _cadparams.SchematicScales.Where(s => s.Key == _cadparams.DefaultSchematic250Scale).First();
                        }
                        else
                        {
                            cboScale.SelectedItem = _cadparams.SchematicScales.Where(s => s.Key == _cadparams.DefaultSchematicScale).First();
                        }
                        cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                        lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                        lblFilterType.Visibility = Visibility.Collapsed;
                        lblFilterValue.Visibility = Visibility.Collapsed;
                        if (lblFilterType.Visibility == Visibility.Collapsed && lblFilterValue.Visibility == Visibility.Collapsed)
                        {
                            lstAll.Visibility = System.Windows.Visibility.Visible;
                            lstCircuitIds.Visibility = System.Windows.Visibility.Visible;
                            lblCircuits.Visibility = System.Windows.Visibility.Visible;
                            //lblVoltage.Visibility = System.Windows.Visibility.Visible;
                            //cboVoltageFilter.Visibility = System.Windows.Visibility.Visible;
                            //cboFeederType.Visibility = System.Windows.Visibility.Visible;
                            //lblFeederType.Visibility = System.Windows.Visibility.Visible;
                            cboSchematicFeederFilter.Visibility = Visibility.Visible;
                            cboSchematicVoltageFilter.Visibility = Visibility.Visible;
                            lblSchematicFilterVoltage.Visibility = Visibility.Visible;
                            lblSchematicFilterFeeder.Visibility = Visibility.Visible;
                            if (cboSchematicFeederFilter.Visibility == Visibility.Visible || cboSchematicVoltageFilter.Visibility == Visibility.Visible)
                            {
                                this.Height = 450;
                                this.stdwgPanel.Height = 450;
                                _dwgFloatWindow.Height = 450;
                            }
                            else
                            {
                                this.Height = 420;
                                this.stdwgPanel.Height = 420;
                                _dwgFloatWindow.Height = 420;
                            }

                           
                        }

                    }
                }

            }
            //string selectedExportFormat = ((string)cboExportFormat.SelectedItem);
            //if(selectedExportFormat!=null){
            //}
        }

        /***********************************31Oct2016 Ends*******************************/


        public void PopulateCircuitIds()
        {
            if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC") && _map.Scale <= _schematicQueryScale)
            {
                _queryresultcount = 0;
                _circuitIdandColors.Clear();

                foreach (string url in _schematicSearchLayers)
                {

                    Query _query = new Query();
                    _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                    _query.ReturnGeometry = false;
                    _query.Geometry = GetPolygonFromEnvelope(_map.Extent);
                    _query.OutFields.AddRange(new string[] { "CIRCUITID", "CIRCUITCOLOR" });

                    QueryTask _circuitqueryTask = new QueryTask(url);
                    _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_circuitqueryTask_ExecuteCompleted);
                    _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_circuitqueryTask_Failed);
                    _circuitqueryTask.ExecuteAsync(_query);
                }
            }

        }

        void _circuitqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
            _queryresultcount++;
        }

        void _circuitqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _queryresultcount++;
            if (e.FeatureSet.Features.Count > 0)
            {
                foreach (Graphic _graphic in e.FeatureSet.Features)
                {
                    if (_graphic.Attributes["CIRCUITID"] != null & _graphic.Attributes["CIRCUITCOLOR"] != null)
                    {
                        if (!_circuitIdandColors.ContainsKey(_graphic.Attributes["CIRCUITID"].ToString()))
                        {
                            _circuitIdandColors.Add(_graphic.Attributes["CIRCUITID"].ToString(), _graphic.Attributes["CIRCUITCOLOR"].ToString());
                        }
                    }
                }
            }
            //throw new NotImplementedException();
            if (_queryresultcount == _schematicSearchLayers.Count)
            {
                _objList = new ObservableCollection<ListOfRecords>();
                _allobjList = new ObservableCollection<ListOfRecords>();
                ListOfRecords supNode = new ListOfRecords() { ID = 0, Content = "Select All", IsSelected = true, BrushObj = new SolidColorBrush(Colors.Blue) };
                _allobjList.Add(supNode);
                AllobjList = _allobjList;
                lstAll.ItemsSource = AllobjList;
                string colorvalue = "";
                string[] RGBValues;
                int itemCount = 1;
                foreach (string key in _circuitIdandColors.Keys)
                {


                    colorvalue = "";
                    colorvalue = _schematicColors[_circuitIdandColors[key]];
                    if (colorvalue != "")
                    {
                        RGBValues = colorvalue.Split(',');

                        Color color = Color.FromArgb(255, (byte)Convert.ToInt16(RGBValues[0]), (byte)Convert.ToInt16(RGBValues[1]), (byte)Convert.ToInt16(RGBValues[2]));
                        ListOfRecords l = new ListOfRecords() { ID = itemCount, Content = key, IsSelected = true, BrushObj = new SolidColorBrush(color) };
                        _objList.Add(l);
                    }
                    else
                    {
                        ListOfRecords l = new ListOfRecords() { ID = itemCount, Content = key, IsSelected = true, BrushObj = new SolidColorBrush(Colors.Black) };
                        _objList.Add(l);
                    }
                    itemCount++;

                }

                objList = _objList;
                lstCircuitIds.ItemsSource = objList;

                foreach (ListOfRecords item in lstCircuitIds.Items)
                {

                }
                /***********************************31Oct2016 Start*******************************/
                this.Height = 485;
                this.stdwgPanel.Height = 485;
                /***********************************31Oct2016 Ends*******************************/
            }
        }

        void _queryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("An error occurred communicating with the ETL Server\r\n[ " + _exportRESTService + "]");
        }

        void _queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _schemticqueryTaskCount++;  //INC000004402997, INC000004402999

            //throw new NotImplementedException();
            if (e.FeatureSet.Features.Count > 0)
            {
                foreach (Graphic _graphic in e.FeatureSet.Features)
                {
                    _exportedgridno = _exportedgridno + _graphic.Attributes["ALPHA"] + "\\" + _graphic.Attributes["NUM"] + "\\" + _graphic.Attributes["MAPNO"] + ",";
                }
            }

            if (_schemticqueryTaskCount == _schematicsGridLayerIds.Count())   //UMG layer divided into 5 - INC000004402997, INC000004402999
            {
                if (_exportedgridno.Length > 0)
                {
                    Guid _guid = Guid.NewGuid();
                    int scale = Convert.ToInt32(((KeyValuePair<string, string>)this.cboScale.SelectedItem).Key);
                    _exportedgridno = _exportedgridno.Substring(0, _exportedgridno.Length - 1);
                    string _circuitids = "";
                    string _voltageFilter = "";
                    string _feederFilter = "";
                    string _exportFormat = "";
                    ComboBoxItem cbexportFormatItem = (ComboBoxItem)cboExportFormat.SelectedItem;
                    string selectedExportFormatValue = cbexportFormatItem.Content.ToString();
                    string selectedVoltageValue = cboSchematicVoltageFilter.SelectedValue.ToString();
                    //  string selectedVoltageValue = cbVoltageItem.Content.ToString();
                    //  ComboBoxItem cbexportLayoutItem = (ComboBoxItem)cboExportLayout.SelectedItem;
                    string selectedExportLayoutValue = cboExportLayout.SelectedValue.ToString();
                    if (selectedVoltageValue == "")
                    {
                        _voltageFilter = "-1";

                    }
                    else
                    {
                        _voltageFilter = selectedVoltageValue;

                    }

                    // ComboBoxItem cbFeederItem = (ComboBoxItem)cboSchematicFeederFilter.SelectedItem;
                    string selectedFeederValue = cboSchematicFeederFilter.SelectedValue.ToString();
                    if (selectedFeederValue == "")
                    {
                        _feederFilter = "-1";

                    }
                    else
                    {
                        _feederFilter = selectedFeederValue;

                    }

                    string filterLayerDef = "-1";

                    if (SchematicsFilterDefinitionquery == null)
                    {
                        filterLayerDef = "-1";
                    }
                    else
                    {
                        //filterLayerDef = JsonHelperSerialiser.JsonSerialize(SchematicsFilterDefinitionquery);
                        filterLayerDef = SchematicsFilterDefinitionquery;

                    }

                    foreach (ListOfRecords lst in lstCircuitIds.Items)
                    {
                        if (lst.IsSelected == true)
                            _circuitids = _circuitids + ":" + lst.Content;
                    }

                    string pngUrl;
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("250"))
                    {
                        pngUrl = _schematics250PNGExportURL;
                    }
                    else
                    {
                        pngUrl = _schematicsPNGExportURL;
                    }
                    //Geoprocessor _geo

                    _jobIds = new List<string>();

                    string fmePolygonCoordinates = PolygonToFMECoordinates(_polygon);
                    string polyExtent;
                    if (_schematicsPrintPreviewExtent != null)
                    {
                        polyExtent = _schematicsPrintPreviewExtent.XMin + "," + _schematicsPrintPreviewExtent.YMin + "," + _schematicsPrintPreviewExtent.XMax + "," + _schematicsPrintPreviewExtent.YMax;
                    }
                    else
                    {
                        polyExtent = _polygon.Extent.XMin + "," + _polygon.Extent.YMin + "," + _polygon.Extent.XMax + "," + _polygon.Extent.YMax;
                    }
                    List<GPParameter> parameters = new List<GPParameter>();
                    parameters.Add(new GPString("SCHM_GRIDS", _exportedgridno));
                    parameters.Add(new GPString("FGDB_PATH", _schematicExportPath + "\\" + _guid.ToString()));
                    parameters.Add(new GPString("SCHM_SDE_PATH", _schematicSDEFilePath));

                    parameters.Add(new GPString("SCALE", scale.ToString()));
                    parameters.Add(new GPString("OUT_FILE_NAME", GetSanitizedFileName()));
                    parameters.Add(new GPString("FMW_URL", _exportSCHEMATICSRESTService));
                    parameters.Add(new GPString("EMAIL", txtSendTo.Text + "@pge.com"));
                    parameters.Add(new GPString("CIRCUITIDS", _circuitids));
                    if (selectedExportFormatValue == "DWG")
                    {
                        parameters.Add(new GPString("SEARCH_POLYGON", fmePolygonCoordinates));
                        parameters.Add(new GPString("VOLTAGE", _voltageFilter));
                        parameters.Add(new GPString("FEEDER_TYPE", _feederFilter));
                        parameters.Add(new GPString("REQUESTTYPE", "DWG"));
                        parameters.Add(new GPString("PNG_URL", pngUrl));
                        parameters.Add(new GPString("PNG_EXTENT", "-1"));
                        parameters.Add(new GPString("PNG_SIZE", "-1"));
                        parameters.Add(new GPString("PNG_DPI", "-1"));
                        parameters.Add(new GPString("PNG_LAYERDEFS", "-1"));
                        parameters.Add(new GPString("PNG_SCALE", "-1"));
                    }
                    else if (selectedExportFormatValue == "PNG")
                    {
                        string dpi = _schematicDPI;
                        string pngSize = getPNGSize(selectedExportLayoutValue, dpi);
                        int pngscale = scale * 12;
                        parameters.Add(new GPString("SEARCH_POLYGON", "-1"));
                        parameters.Add(new GPString("VOLTAGE", "-1"));
                        parameters.Add(new GPString("FEEDER_TYPE", "-1"));
                        parameters.Add(new GPString("REQUESTTYPE", "PNG"));
                        parameters.Add(new GPString("PNG_URL", pngUrl));
                        parameters.Add(new GPString("PNG_EXTENT", polyExtent));
                        parameters.Add(new GPString("PNG_SIZE", pngSize));
                        parameters.Add(new GPString("PNG_DPI", dpi));
                        parameters.Add(new GPString("PNG_LAYERDEFS", filterLayerDef));
                        parameters.Add(new GPString("PNG_SCALE", pngscale.ToString()));

                    }
                    else if (selectedExportFormatValue == "DWG+PNG")
                    {
                        string dpi = _schematicDPI;
                        int pngscale = scale * 12;
                        string pngSize = getPNGSize(selectedExportLayoutValue, dpi);
                        parameters.Add(new GPString("SEARCH_POLYGON", fmePolygonCoordinates));
                        parameters.Add(new GPString("VOLTAGE", _voltageFilter));
                        parameters.Add(new GPString("FEEDER_TYPE", _feederFilter));
                        parameters.Add(new GPString("REQUESTTYPE", "PNG;DWG"));
                        parameters.Add(new GPString("PNG_URL", pngUrl));
                        parameters.Add(new GPString("PNG_EXTENT", polyExtent));
                        parameters.Add(new GPString("PNG_SIZE", pngSize));
                        parameters.Add(new GPString("PNG_DPI", dpi));
                        parameters.Add(new GPString("PNG_LAYERDEFS", filterLayerDef));
                        parameters.Add(new GPString("PNG_SCALE", pngscale.ToString()));
                    }



                    _textPosition = _polygon.Extent.GetCenter();


                    _schematicGeoprocessor.SubmitJobAsync(parameters);

                }
                else
                {
                    MessageBox.Show("Unable to query Schematics Grid Layer.");
                }
            }
        }

        void _schematicGeoprocessor_Failed(object sender, TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _schematicGeoprocessor_StatusUpdated(object sender, JobInfoEventArgs e)
        {
            ComboBoxItem cbexportFormatItem = (ComboBoxItem)cboExportFormat.SelectedItem;
            string selectedExportFormatValue = cbexportFormatItem.Content.ToString();
            TextSymbol textSymbol;
            if (selectedExportFormatValue == "DWG")
            {
                textSymbol = _textSymbol;
            }
            else if (selectedExportFormatValue == "PNG")
            {
                textSymbol = _pngTextSymbol;
            }
            else
            {
                textSymbol = _pngDwgTextSymbol;
            }

            switch (e.JobInfo.JobStatus)
            {
                case esriJobStatus.esriJobSubmitted:
                    // Disable automatic status checking.
                    //geoprocessingTask.CancelJobStatusUpdates(e.JobInfo.JobId);
                    MapPoint mp = _textPosition;
                    var _jobid = e.JobInfo.JobId;
                    if (!_jobIds.Contains(_jobid.ToString()))
                    {
                        _jobIds.Add(_jobid.ToString());
                        Graphic graphicText = new Graphic()
                        {
                            Geometry = mp,
                            Symbol = textSymbol,
                        };

                        _graphicsLayer.Graphics.Add(graphicText);
                        _polygon = null;
                    }
                    break;
                case esriJobStatus.esriJobSucceeded:
                    // Get the results.
                    //geoprocessingTask.GetResultDataAsync(e.JobInfo.JobId, "<parameter name>");
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //MessageBox.Show("Job Succeeded");
                    break;
                case esriJobStatus.esriJobFailed:
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //ConfigUtility.StatusBar.Text = "Failed to get Elevation..";
                    break;
                case esriJobStatus.esriJobTimedOut:
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //MessageBox.Show("Job TimeOut");
                    //this.Close();
                    break;
            }
        }

        private string getPNGSize(string exportLayout ,string Dpi)
        {
            string sizePng=string.Empty;
            if (exportLayout == "PNG_8-5x11_Portrait")
            {
                sizePng = 8.5 * Convert.ToInt32(Dpi) + "," + 11 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_11X17_Portrait")
            {
                sizePng = 11 * Convert.ToInt32(Dpi) + "," + 17 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_22x34_Portrait")
            {
                sizePng = 22 * Convert.ToInt32(Dpi) + "," + 34 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_34x44_Portrait")
            {
                sizePng = 34 * Convert.ToInt32(Dpi) + "," + 44 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_8-5x11_Landscape")
            {
                sizePng = 11 * Convert.ToInt32(Dpi) + "," + 8.5 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_11x17_Landscape")
            {
                sizePng = 17 * Convert.ToInt32(Dpi) + "," + 11 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_22x34_Landscape")
            {
                sizePng = 34 * Convert.ToInt32(Dpi) + "," + 22 * Convert.ToInt32(Dpi);
            }
            else if (exportLayout == "PNG_34x44_Landscape")
            {
                sizePng = 44 * Convert.ToInt32(Dpi) + "," + 34 * Convert.ToInt32(Dpi);
            }
            return sizePng;
        }

        private void SendPolygonToFME(Polygon polygon)
        {
            ComboBoxItem cbItem = (ComboBoxItem)cboExportFormat.SelectedItem;
            string selectedExportFormat = cbItem.Content.ToString();
            if (selectedExportFormat == "PNG")
            {
                if (lblSD.Visibility != System.Windows.Visibility.Visible)
                {

                    if (_MapTypeList.Count != 0) {/* callED50PNGFMEService(polygon);*/
                        callED50PNGGPService(polygon);
                    } else
                    { 
                        HtmlPage.Window.Alert("No Item Selected!, Please select one");
                        _graphicsLayer.Graphics.Clear(); 
                        Export.IsEnabled = true; return;
                    }
                }
                else
                {
                    CallSCHEMATICService(polygon);
                }
            }
            else if (selectedExportFormat == "DWG")
            {
                if (lblSD.Content.ToString().Contains("ED"))
                    CallEDFMEService(polygon);
                else if (lblSD.Visibility != Visibility.Visible)
                    if (_MapTypeList.Count != 0) {
                        callED50PNGGPService(polygon);
                        /*CallED50FMEService(polygon);*/ } else { HtmlPage.Window.Alert("No Item Selected!, Please select one"); _graphicsLayer.Graphics.Clear(); Export.IsEnabled = true; return; }
                else
                    CallSCHEMATICService(polygon);
            }
            else
            {
                

                if (lblSD.Content.ToString().Contains("ED"))
                {
                    CallEDFMEService(polygon);
                }
                else if (lblSD.Visibility != Visibility.Visible)
                    if (_MapTypeList.Count != 0) { callED50PNGGPService(polygon); } else { HtmlPage.Window.Alert("No Item Selected!, Please select one"); _graphicsLayer.Graphics.Clear(); Export.IsEnabled = true; return; }
                else
                {
                    CallSCHEMATICService(polygon);
                }
            }
        }

        // For some reason there is no InvalidFileNameChars() method on Path in SL
        private string GetIllegalString()
        {
            // This set came from the .NET call
            const string ILLEGAL_FILE_NAME_CHARS = "34,60,62,124,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,58,42,63,92,47";

            string illegalCharString = new string(Path.GetInvalidPathChars());

            foreach (string invalidCharCode in ILLEGAL_FILE_NAME_CHARS.Split(','))
            {
                char c = (char)(Convert.ToInt32(invalidCharCode));
                illegalCharString += c;
            }

            return illegalCharString;
        }

        private string GetSanitizedFileName()
        {
            const int MAX_USER_FILENAME_SIZE = 100;

            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            fileName += "_";
            string userFileName = txtFileName.Text;
            if (String.IsNullOrEmpty(userFileName))
            {
                userFileName = "GISToCAD";
            }
            else
            {
                if (userFileName.Length > MAX_USER_FILENAME_SIZE)
                {
                    userFileName = userFileName.Substring(0, MAX_USER_FILENAME_SIZE);
                }
                string invalid = GetIllegalString();

                foreach (char c in invalid)
                {
                    userFileName = userFileName.Replace(c.ToString(), "");
                }
                userFileName = userFileName.Trim();
                userFileName = userFileName.Replace(" ", "_");
                // FME will add .dwg for us
                if (userFileName.ToUpper().EndsWith(".DWG"))
                {
                    userFileName = userFileName.Substring(0, userFileName.Length - 4);
                }
            }
            fileName += userFileName;

            return fileName;
        }

        void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                MapPoint mp = _textPosition;
               // _graphicsLayer.Graphics.Clear(); 
               //Changing after ME -Q1-2020 relase -Upgrade branch for Submitted message
               if (e.Result.ToUpper().Contains("SUCCESS")) 
               // if (e.Result.Contains("SUCCEEDED"))
                {
                   
                    Graphic graphicText = new Graphic()
                    {
                        Geometry = mp,
                        Symbol = _textSymbol,
                    };
                    if (!_graphicsLayer.Contains(graphicText))
                    {
                        _graphicsLayer.Graphics.Add(graphicText);
                    }
                    _polygon = null;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred communicating with the ETL Server\r\n[ " + _exportRESTService + "]");
            }
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

        private ESRI.ArcGIS.Client.Geometry.Polygon GetPolygonFromEnvelope(ESRI.ArcGIS.Client.Geometry.Geometry geometry)
        {
            Graphic g = new Graphic();
            Polygon p = new ESRI.ArcGIS.Client.Geometry.Polygon();
            p.SpatialReference = geometry.SpatialReference;

            PointCollection pColl = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            MapPoint pLeftBottom = new MapPoint();
            pLeftBottom.X = geometry.Extent.XMin;
            pLeftBottom.Y = geometry.Extent.YMin;
            pColl.Add(pLeftBottom);
            MapPoint pLeftTop = new MapPoint();
            pLeftTop.X = geometry.Extent.XMin;
            pLeftTop.Y = geometry.Extent.YMax;
            pColl.Add(pLeftTop);
            MapPoint pRightTop = new MapPoint();
            pRightTop.X = geometry.Extent.XMax;
            pRightTop.Y = geometry.Extent.YMax;
            pColl.Add(pRightTop);
            MapPoint pRightBottom = new MapPoint();
            pRightBottom.X = geometry.Extent.XMax;
            pRightBottom.Y = geometry.Extent.YMin;
            pColl.Add(pRightBottom);
            MapPoint pLeftBottomEnd = new MapPoint();
            pLeftBottomEnd.X = geometry.Extent.XMin;
            pLeftBottomEnd.Y = geometry.Extent.YMin;
            pColl.Add(pLeftBottomEnd);

            p.Rings.Add(pColl);

            return p;
        }

        /***************Ed Phase 2 Warranty Changes Start*********************/
        private void Printpreview_Click(object sender, RoutedEventArgs e)
        {
           
            _graphicsLayer.Graphics.Clear();

          //  printPreviewBtn.Background = new SolidColorBrush(Color.FromArgb(255,233,233,233));
            PNGPrintPreview pngPreview = new PNGPrintPreview();
            pngPreview.TemplateName = cboExportLayout.SelectedValue.ToString();
            pngPreview.SchematicMap = _map;
            if (_schematicsPrintPreviewExtent != null)
            {
                pngPreview.CurrentExtent = _schematicsPrintPreviewExtent;
            }
            else
            {
                pngPreview.CurrentExtent = _map.Extent;
            }
            pngPreview.exportScale = Convert.ToDouble(((KeyValuePair<string, string>)this.cboScale.SelectedItem).Key);
            pngPreview.CadExportTool = this;
            pngPreview.exportDpi = _schematicDPI;
            pngPreview.Height = _map.ActualHeight - 20d;
            pngPreview.Width = _map.ActualWidth - 20d;
            pngPreview.StoredDisplayName = _mapTools.StoredViewControl._selectedStoredView.StoredViewName;
            pngPreview.exportMapTypeList = _MapTypeList;
            pngPreview.strDcviewURL = _cadparams.DCURL;
            pngPreview.strStreetLightURL = _cadparams.StreetLightURL;
            pngPreview.lstLandBaseVisibleLayers = _cadparams.strLandbaseLayers;
            //pngPreview.Closed += new EventHandler(pngPreview_Closed);
            pngPreview.Show();
        }
        /***************Ed Phase 2 Warranty Changes End*********************/

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Export.IsEnabled = false;
            
            Graphic graphic = new Graphic() { Geometry = _polygon };
            if (_polygon != null)
            {
                SimplifyAsync(graphic);
            }
            else
            {
                // No need to simplify an envelope...
                ProcessEnvelope();
            }
        }
        
        private void DrawExtractedArea(ESRI.ArcGIS.Client.Geometry.Geometry geometry)
        {
            Graphic graphic = new Graphic()
            {
                Geometry = geometry,
                Symbol = _fillSymbol,
            };

            AreaTotal = graphic.Geometry.Extent.Width * graphic.Geometry.Extent.Height;

            MapPoint centerPt = new MapPoint();
            centerPt.SpatialReference = _map.SpatialReference;
            centerPt.X = graphic.Geometry.Extent.XMin + graphic.Geometry.Extent.Width / 2;
            centerPt.Y = graphic.Geometry.Extent.YMin + graphic.Geometry.Extent.Height / 2;


            Graphic ptGraphic = new Graphic();
            ptGraphic.Geometry = centerPt;


            var symb = new RotatingTextSymbol();
            symb.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(AreaTotal), PrettyUnits(AreaUnit.SquareFeet));

            ptGraphic.Symbol = symb;

            _graphicsLayer.ClearGraphics();
            _graphicsLayer.Graphics.Add(graphic);
            _graphicsLayer.Graphics.Add(ptGraphic);

        }

        private void ProcessEnvelope()
        {
            // No graphics mean that someone just clicked the Export area, default to the map extent
            if (_graphicsLayer.Graphics.Count == 0 || _graphicsLayer.Graphics.Count > 1)
            {
                _envelope = _map.Extent;
                DrawExtractedArea(_envelope);
            }

            // Measure Area and validate
            double sqft = _envelope.Width * _envelope.Height;

            if (ValidateSquareFeet(sqft))
            {
                // Expand the _envelope to create "buffer"
                Envelope envelope = new Envelope(
                    _envelope.XMin - _bufferGeometryFeet,
                    _envelope.YMin - _bufferGeometryFeet,
                    _envelope.XMax + _bufferGeometryFeet,
                    _envelope.YMax + _bufferGeometryFeet
                    );

                Polygon polygon = GetPolygonFromEnvelope(envelope);
                SendPolygonToFME(polygon);
            }
            else
            {
                ResetGraphics();
            }
        }

        private void Extent_Click(object sender, RoutedEventArgs e)
        {
            DrawPolygon.IsChecked = false;
            DrawExtent.IsChecked = true;
            _drawControl.DrawMode = DrawMode.Rectangle;

            ClearLayers();

            _map.MouseMove -= _map_MouseMove;
            _map.MouseLeftButtonDown -= _map_MouseLeftButtonDown;
            MapEventIsOn = false;

        }

        private void Polygon_Click(object sender, RoutedEventArgs e)
        {
            DrawExtent.IsChecked = false;
            DrawPolygon.IsChecked = true;
            _drawControl.DrawMode = DrawMode.None;
            ResetGraphics();
            _map.MouseMove += _map_MouseMove;
            _map.MouseLeftButtonDown += _map_MouseLeftButtonDown;
            MapEventIsOn = true;

        }


        #endregion

        #region Public Methods

        public bool MapEventIsOn;

        public void StartListeningToMapExtentChanged()
        {
            this._map.ExtentChanged += new EventHandler<ExtentEventArgs>(_map_ExtentChanged);
            if (!MapEventIsOn)
            {
                if (DrawPolygon.IsChecked == true)
                {
                    this._map.MouseMove += _map_MouseMove;
                    this._map.MouseLeftButtonDown += _map_MouseLeftButtonDown;
                    MapEventIsOn = true;
                }
            }
        }

        public void StopListeningToMapExtentChanged()
        {
            this._map.ExtentChanged -= _map_ExtentChanged;
            this._map.MouseMove -= _map_MouseMove;
            this._map.MouseLeftButtonDown -= _map_MouseLeftButtonDown;
            ClearLayers();
            this.MapEventIsOn = false;
        }

        static public Size GetTextSize(string text, FontFamily fontFamily, int fontSize)
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

        public void InitializeDrawGraphics()
        {
            if (_drawControl == null)
            {
                _drawControl = new Draw(_map);
                _drawControl.LineSymbol = LineSymbol;
                //_lineSymbol = new SimpleLineSymbol(Colors.Red, 2);
                _fillSymbol = new SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                    Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.25 }
                };
                _drawControl.DrawMode = DrawMode.Rectangle;
                _graphicsLayer = new GraphicsLayer();
                _graphicsLayer.ID = CAD_EXPORT_GRAPHICS_LAYER;
                if (!_map.Layers.Contains(_graphicsLayer))
                {
                    _map.Layers.Add(_graphicsLayer);
                }
                _drawControl.DrawComplete += DrawControl_DrawComplete;
            }
            _drawControl.IsEnabled = true;
            _polygon = null;
        }

        public void Initialize(CADExportControlParameters parameters)
        {



            logger.Debug("Initializing CADExportControl, pointing to [ " + parameters.ExportRESTService + " ]");
            _cadparams = parameters;
            cboScale.ItemsSource = parameters.Scales;
            cboScale.DisplayMemberPath = "Value";
            cboScale.SelectedValuePath = "Key";
            cboScale.SelectedItem = parameters.Scales.Where(s => s.Key == parameters.DefaultScale).First();

            //cboSD.ItemsSource = parameters.MapTypes;
            //cboSD.DisplayMemberPath = "Value";
            //cboSD.SelectedValuePath = "Key";
            //cboSD.SelectedItem = parameters.MapTypes.First();

            _exportRESTService = parameters.ExportRESTService;
            _exportWYSWYGRESTService = parameters.ExportWYSWYGRESTService;
            _exportSCHEMATICSRESTService = parameters.ExportSCHEMATICSRESTService;
            _schematicsGridLayer = parameters.SchematicsGridLayer;
            _schematicsGridLayerIds = parameters.SchematicsGridLayerIds.Split(','); //UMG layer divided into 5 - INC000004402997, INC000004402999
            _schematicQueryScale = parameters.SchematicQueryScale;
            _schematicGPService = parameters.SchematicGPService;
            _geometryService = new GeometryService(parameters.GeometryService);
            _bufferGeometryFeet = parameters.BufferGeometryFeet;
            _maximumAreaSquareFeet = parameters.MaximumAreaSquareFeet;
            _circuitmaximumAreaSquareFeet = parameters.CircuitMaximumAreaSquareFeet;
            _annotationElement = parameters.AnnotationElements;
            _schematicExportPath = parameters.SchematicExportPath;
            _schematicSDEFilePath = parameters.SchematicSDEFilePath;
            _schematicSearchLayers = parameters.SchematicSearchLayers;
            _schematicColors = parameters.SchematicColors;
            ////////////////////////////////
           // _eD50ExportRestService = parameters.ED50ExportRestService;
            _schematicDPI = parameters.SchematicsPngDpi;
            _schematicsPNGExportURL=parameters.SchematicsPNGExportService;
           _schematics250PNGExportURL=parameters.Schematics250PNGExportService;
            /////////////////////////////
            cboWYSWYG.SelectedIndex = 0;

            _schematicGeoprocessor = new Geoprocessor(_schematicGPService);
            _schematicGeoprocessor.StatusUpdated += new EventHandler<JobInfoEventArgs>(_schematicGeoprocessor_StatusUpdated);
            _schematicGeoprocessor.Failed += new EventHandler<TaskFailedEventArgs>(_schematicGeoprocessor_Failed);

            _schemticqueryTaskList.Clear();
            for (int i = 0; i < _schematicsGridLayerIds.Count(); i++)   //UMG layer divided into 5 - INC000004402997, INC000004402999
            {
                QueryTask schemticqueryTask = new QueryTask(_schematicsGridLayer + "/" + _schematicsGridLayerIds[i]);
                schemticqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_queryTask_ExecuteCompleted);
                schemticqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_queryTask_Failed);
                _schemticqueryTaskList.Add(schemticqueryTask);
            }
        }

        public void ClearGraphics()
        {
            _graphicsLayer.ClearGraphics();
            _drawControl.IsEnabled = false;
            ClearLayers();
        }

        public void ResetGraphics()
        {
            _graphicsLayer.Graphics.Clear();
            _polygon = null;
            _envelope = null;
            Export.IsEnabled = true;
            ToolTipService.SetToolTip(Export, "Export to CAD");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //bool isSeleceAll = false;
            //CheckBox cb = sender as CheckBox;
            //int index = Convert.ToInt32(cb.Tag);
            //if (index == 0)
            //{
            //    isSeleceAll = true;
            (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>).ToList().ForEach(l1 => l1.IsSelected = true);
            //}
            //else
            //{
            //    int cntRec = (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>).ToList().Count(lst => lst.IsSelected == true);
            //    if (cntRec == lstCircuitIds.Items.Count - 1)
            //    {
            //        (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>)[0].IsSelected = true;
            //        //isSeleceAll = true;
            //    }
            //}


            //if (isSeleceAll)
            //{
            //    TextBlock tb = new TextBlock();
            //    tb.Text = "Select All";
            //    tb.SetValue(Grid.RowProperty, 0);
            //    tb.SetValue(Grid.ColumnProperty, 0);

            //}
            //else
            //{
            //    int cnt = 0;
            //    foreach (var item in lstCircuitIds.ItemsSource)
            //    {
            //        if (((ListOfRecords)item).IsSelected || ((ListOfRecords)item).Content == cb.Content.ToString())
            //        {
            //            TextBlock tb = new TextBlock();
            //            tb.Text = ((ListOfRecords)item).Content.ToString();
            //            tb.SetValue(Grid.RowProperty, cnt);
            //            tb.SetValue(Grid.ColumnProperty, 0);
            //            //grd.Children.Add(tb);
            //            cnt++;
            //        }
            //    }
            //}
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {


            (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>).ToList().ForEach(l1 => l1.IsSelected = false);
            //CheckBox cb = sender as CheckBox;
            //int index = Convert.ToInt32(cb.Tag);
            //Grid grd = scrCriteriaSummaryTemp.Content as Grid;
            //if (index == 0)
            //{
            //    //grd.Children.Clear();
            //}
            //else
            //{

            //    isMainChange = true;
            //    (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>)[0].IsSelected = false;
            //    //grd.Children.Clear();
            //    int cnt = 0;
            //    foreach (var item in lstCircuitIds.ItemsSource)
            //    {
            //        if (((ListOfRecords)item).IsSelected && ((ListOfRecords)item).Content != cb.Content.ToString())
            //        {
            //            TextBlock tb = new TextBlock();
            //            tb.Text = ((ListOfRecords)item).Content.ToString();
            //            tb.SetValue(Grid.RowProperty, cnt);
            //            tb.SetValue(Grid.ColumnProperty, 0);
            //            //grd.Children.Add(tb);
            //            cnt++;
            //        }
            //    }
            //}
            //if (index == 0)
            //{
            //    (lstCircuitIds.ItemsSource as ObservableCollection<ListOfRecords>).ToList().ForEach(l1 => l1.IsSelected = false);
            //}
            //else
            //{
            //    isMainChange = false;
            //}
        }

        private bool isMainChange = false;
        private ObservableCollection<ListOfRecords> _objList;
        public ObservableCollection<ListOfRecords> objList
        {
            get { return _objList; }
            set
            {
                _objList = value;

            }
        }

        private ObservableCollection<ListOfRecords> _allobjList;
        public ObservableCollection<ListOfRecords> AllobjList
        {
            get { return _allobjList; }
            set
            {
                _allobjList = value;

            }
        }

        #endregion

       



    }

    public class CADExportControlParameters
    {
        public Dictionary<string, string> Scales;
        public Dictionary<string, string> SchematicScales;
        public Dictionary<string, string> MapTypes;
        public Dictionary<string, string> EDLayers;
        public Dictionary<string, string> EDMasterLayers;
        public Dictionary<string, string> LandbaseLayers;
        public Dictionary<string, string> SchematicColors;
        public List<string> SchematicSearchLayers;
        public string DefaultScale;
        public string DefaultSchematicScale;
        public string ExportRESTService;
        public string ExportWYSWYGRESTService;
        public string ExportSCHEMATICSRESTService;
       // public string ED50ExportRestService;
        public string SchematicsGridLayer; 
        public string SchematicsGridLayerIds; //UMG layer divided into 5 - INC000004402997, INC000004402999
        public string GeometryService;
        public int MaximumAreaSquareFeet;
        public int CircuitMaximumAreaSquareFeet;
        public int BufferGeometryFeet;
        public string SchematicExportPath;
        public string SchematicSDEFilePath;
        public double SchematicQueryScale;
        public string[] VisibleEDLayerIds;
        public string[] VisibleEDMasterLayerIds;
        public string[] VisibleLandbaseLayerIds;
        public string[] VisibleEDDefaultLayerIds;
        public string[] VisibleEDMasterDefaultLayerIds;
        public string[] VisibleLandbaseDefaultLayerIds;
        public XElement AnnotationElements;
        public string SchematicGPService;
        public string SchematicsPngDpi;
        public string SchematicsPNGExportService;
        public string DCURL;
        public string StreetLightURL;
        public string strButterflyLayers;
        public string strLandbaseLayers;
        //public string URL;
        public string strFMEPNGExportUrl;
        public Dictionary<string, string> SchematicPNGScales;
        public string DefaultSchematic250Scale;
        public string Schematics250PNGExportService;
    }

    public class ListOfRecords : INotifyPropertyChanged
    {
        private bool _IsSelected = false;
        public int ID { get; set; }
        public string Content { get; set; }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        public Brush BrushObj { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    static class Extensions
    {
        public static string ToQueryString(this Dictionary<string, string> source)
        {
            return String.Join("&", source.Select(kvp => String.Format("{0}={1}", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value))).ToArray());
        }
    }

}
