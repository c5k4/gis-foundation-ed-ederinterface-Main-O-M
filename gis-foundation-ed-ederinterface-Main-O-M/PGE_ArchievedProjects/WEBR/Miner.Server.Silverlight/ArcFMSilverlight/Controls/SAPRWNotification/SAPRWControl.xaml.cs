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
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using Miner.Server.Client.Toolkit;
using System.Windows.Controls.Primitives;
using ESRI.SilverlightViewer.Controls;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client.Geometry;

namespace ArcFMSilverlight
{
    public partial class SAPRWControl : UserControl
    {
        private static bool isMapNotificationCurrentlyActive = false;
        private static bool isGraphicSelectionCurrentlyActive = false;
        private static bool isDrawLineCurrentlyActive = false;
        private SAPNotificationTool _sapNotificationTool;
        private const string DrawCursor = @"/Images/cursor_measure.png";
        private static Draw _drawControl = null;
        public LineSymbol LineSymbol { get; set; }
        public FillSymbol FillSymbol { get; set; }
        private GraphicsLayer _graphicsLayer = null;
        private GeometryService _GeometryService;        
        private GraphicCollection _theGraphicsCollection;

        private const string SelectionCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/NewSelection.png";

        public SAPRWControl()
        {
            InitializeComponent();
                        
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            
        }

        private void InitialSetting()
        {
            if (Map == null)
                return;

            if (_sapNotificationTool == null)
                _sapNotificationTool = new SAPNotificationTool(MapNotificationToggleButton, Map);

            LineSymbol = new SimpleLineSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                Width = 2,
                Style = SimpleLineSymbol.LineStyle.Solid                 
            };

            FillSymbol = new SimpleFillSymbol
            {
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = 2,
                Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.25 }
            };

            if (_drawControl == null)
            {
                _drawControl = new Draw(Map);
                _drawControl.LineSymbol = LineSymbol;
                _drawControl.FillSymbol = FillSymbol;
                _drawControl.DrawMode = DrawMode.None;
                _drawControl.DrawComplete += DrawControl_DrawComplete;

                _drawControl.IsEnabled = true;

                //_graphicsLayer = new GraphicsLayer();
                //_graphicsLayer.ID = "SAPRWGraphicArrow";
                _graphicsLayer = Map.Layers["SAPRWGraphicArrow"] as GraphicsLayer;
                //Map.Layers.Add(_graphicsLayer);

                // Define a GeometryService and add the Asynchronous event handlers.
                _GeometryService = new GeometryService(ConfigUtility.GeometryServiceURL);         
                _GeometryService.IntersectCompleted += GeometryService_IntersectCompleted;
                _GeometryService.Failed += GeometryService_Failed;
            }

        }
               
        private void GeometryService_IntersectCompleted(object sender, ESRI.ArcGIS.Client.Tasks.GraphicsEventArgs e)
        {
            try
            {                
                for (int i = 0; i <= (e.Results.Count - 1); i++)
                {                   
                    if (e.Results[i].Geometry.Extent != null)
                    {
                        
                        _theGraphicsCollection[i].Select();
                        _theGraphicsCollection[i].Symbol = SelectionSymbol;
                        
                    }
                    else
                    {
                        _theGraphicsCollection[i].UnSelect();
                        _theGraphicsCollection[i].Symbol = LineSymbol;
                    }
                }
            }
            catch (Exception)
            {
                
                //throw;
            }
            
        }

        private void GeometryService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs args)
        {
            // This event is a catch all if something goes wrong with either of the GeometryService 
            // Aysnchronous calls.
            MessageBox.Show("Geometry Service error: " + args.Error.Message);
        }

        private void DrawControl_DrawComplete(object sender, DrawEventArgs args)
        {            

            if (args.Geometry is ESRI.ArcGIS.Client.Geometry.Polyline)
            {

                
                ESRI.ArcGIS.Client.Geometry.Polyline _polyline = args.Geometry as ESRI.ArcGIS.Client.Geometry.Polyline ;
                ObservableCollection< ESRI.ArcGIS.Client.Geometry.PointCollection>  path = _polyline.Paths;

                ESRI.ArcGIS.Client.Geometry.PointCollection ptc = path[path.Count-1];

                MapPoint endPoint = ptc[0];
                MapPoint startPoint = ptc[1];

                double dx = endPoint.X - startPoint.X;
                double dy = endPoint.Y - startPoint.Y;

                const double cos = 0.866;
                const double sin = 0.500;

                var length = Math.Sqrt(dx * dx + dy * dy);

                var unitDx = dx / length;
                var unitDy = dy / length;

                // increase this to get a larger arrow head
                const int arrowHeadBoxSize = 10;

                var end1 = new MapPoint(
                    Convert.ToInt32(endPoint.X - unitDx * arrowHeadBoxSize - unitDy * arrowHeadBoxSize),
                    Convert.ToInt32(endPoint.Y - unitDy * arrowHeadBoxSize + unitDx * arrowHeadBoxSize));
                var end2 = new MapPoint(
                    Convert.ToInt32(endPoint.X - unitDx * arrowHeadBoxSize + unitDy * arrowHeadBoxSize),
                    Convert.ToInt32(endPoint.Y - unitDy * arrowHeadBoxSize - unitDx * arrowHeadBoxSize));


                end1.SpatialReference = endPoint.SpatialReference;


                end2.SpatialReference = endPoint.SpatialReference;

                ESRI.ArcGIS.Client.Geometry.PointCollection pt3 = new ESRI.ArcGIS.Client.Geometry.PointCollection();

                pt3.Add(endPoint);
                pt3.Add(end1);
                pt3.Add(endPoint);
                pt3.Add(end2);

                _polyline.Paths.Add(pt3); 

                var _ArrowGraphic = new Graphic();
                _ArrowGraphic.Geometry = _polyline;
                _ArrowGraphic.Symbol = LineSymbol;
                _graphicsLayer.Graphics.Add(_ArrowGraphic);


            }
            else if (args.Geometry is ESRI.ArcGIS.Client.Geometry.Envelope){

               _theGraphicsCollection = _graphicsLayer.Graphics; 
                if (_theGraphicsCollection.Count>0)
                    _GeometryService.IntersectAsync(_theGraphicsCollection, args.Geometry);
            }


        }


        

        public void OnControlActivated(DependencyObject control)
        {
            if (control != this)
            {
                //IsMapNotificationActive = false;
                IsDrawLineActive = false;
                IsGraphicSelectionActive = false;

                
            }
        }

        //public bool IsMapNotificationActive
        //{
        //    get
        //    {
        //        return (bool)GetValue(IsActiveMapNotificationProperty);
        //    }
        //    set
        //    {
        //        SetValue(IsActiveMapNotificationProperty, value);
        //        //if (IsMapNotificationActive && IsDrawLineActive) { IsDrawLineActive = false; }
        //        //if (IsMapNotificationActive && IsGraphicSelectionActive) { IsGraphicSelectionActive = false; }
                
        //        MapNotificationToggleButton.IsChecked = value;
        //    }
        //}

        public bool IsGraphicSelectionActive
        {
            get
            {
                return (bool)GetValue(IsActiveGraphicSelectionProperty);
            }
            set
            {
                SetValue(IsActiveGraphicSelectionProperty, value);
                if (IsGraphicSelectionActive && IsDrawLineActive) { IsDrawLineActive = false; }
                
                SelectGraphicToggleButton.IsChecked = value;                
            }
        }

        public bool IsDrawLineActive
        {
            get
            {
                return (bool)GetValue(IsActiveDrawLineProperty);
            }
            set
            {
                SetValue(IsActiveDrawLineProperty, value);
                if (IsDrawLineActive && IsGraphicSelectionActive) { IsGraphicSelectionActive = false; }
                
                DrawArrowLineToggleButton.IsChecked = value;
            }
        }

        #region Dependency Properties
        /// <summary>
        /// Gets the identifier for the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(SAPRWControl),
            null);

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        public Map Map
        {
            get
            {
                return (Map)GetValue(MapProperty);
            }
            set
            {
               
                SetValue(MapProperty, value);
                InitialSetting();
            }
        }


        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveMapNotificationProperty = DependencyProperty.Register(
            "IsMapNotificationActive",
            typeof(bool),
            typeof(SAPRWControl),
            new PropertyMetadata(OnIsMapNotificationActiveChanged));

        private static void OnIsMapNotificationActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SAPRWControl)d;
            isMapNotificationCurrentlyActive = (bool)e.NewValue;
            if (!isMapNotificationCurrentlyActive)
            {
                //if (control.Map == null) return;
                //control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isDrawLineCurrentlyActive && !isGraphicSelectionCurrentlyActive))
                {
                    
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;                
            }
        }


        public static readonly DependencyProperty IsActiveGraphicSelectionProperty = DependencyProperty.Register(
            "IsActiveGraphicSelection",
            typeof(bool),
            typeof(SAPRWControl),
            new PropertyMetadata(OnIsGraphicSelectionActiveChanged));

        private static void OnIsGraphicSelectionActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SAPRWControl)d;
            isGraphicSelectionCurrentlyActive = (bool)e.NewValue;
            if (!isGraphicSelectionCurrentlyActive)
            {                
                if ((!isDrawLineCurrentlyActive && !isMapNotificationCurrentlyActive))
                {
                    _drawControl.DrawMode = DrawMode.None;
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;
                CursorSet.SetID(control.Map, SelectionCursor);
                
            }
        }

        public static readonly DependencyProperty IsActiveDrawLineProperty = DependencyProperty.Register(
            "IsActiveGraphicSelection",
            typeof(bool),
            typeof(SAPRWControl),
            new PropertyMetadata(OnIsDrawLineActiveChanged));

        private static void OnIsDrawLineActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (SAPRWControl)d;
            isDrawLineCurrentlyActive = (bool)e.NewValue;
            if (!isDrawLineCurrentlyActive)
            {                
                if ((!isGraphicSelectionCurrentlyActive && !isMapNotificationCurrentlyActive))
                {

                }
                _drawControl.DrawMode = DrawMode.None;
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;
                CursorSet.SetID(control.Map, DrawCursor);
                
            }
        }
        #endregion

        private void MapNotificationToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //IsMapNotificationActive = true ;
            _drawControl.DrawMode = DrawMode.None;
            //_drawControl.IsEnabled = false;
            if (_sapNotificationTool.IsActive == true)
            {
                _sapNotificationTool.IsActive = false;
                MapNotificationToggleButton.IsChecked = false;
            }
            else
            {
                _sapNotificationTool.IsActive = true;
                MapNotificationToggleButton.IsChecked = true;
            }
        }

        private void DrawArrowLineToggleButton_Click(object sender, RoutedEventArgs e)
        {
           
            IsDrawLineActive = true;
            _drawControl.IsEnabled = true;
            _drawControl.DrawMode = DrawMode.Polyline;
        }

        private void SelectGraphicToggleButton_Click(object sender, RoutedEventArgs e)
        {
           
            IsGraphicSelectionActive = true;
            _drawControl.IsEnabled = true;
            _drawControl.DrawMode = DrawMode.Rectangle;
        }

        private void DeleteGraphicButton_Click(object sender, RoutedEventArgs e)
        {

            IList<Layer> layers = Map.Layers.Where(l => l is FeatureLayer && l.ID != null && l.ID.StartsWith("Redline")).ToList();
            var graphicToRemove = new List<Graphic>();

            foreach (GraphicsLayer layer in layers)
            {                
                graphicToRemove = new List<Graphic>();
                foreach (var graphic in layer.Graphics)
                {                    
                    if (graphic.Selected == true)
                        graphicToRemove.Add(graphic);
                }

                foreach (var graphic in graphicToRemove)
                {
                    _graphicsLayer.Graphics.Remove(graphic);
                }
            }

            graphicToRemove = new List<Graphic>();
            foreach (var graphic in _graphicsLayer.Graphics)
            {                
                if (graphic.Selected == true)
                    graphicToRemove.Add(graphic); 
            }

            foreach (var graphic in graphicToRemove)
            {
                _graphicsLayer.Graphics.Remove(graphic);
            }
        }

    }
}
