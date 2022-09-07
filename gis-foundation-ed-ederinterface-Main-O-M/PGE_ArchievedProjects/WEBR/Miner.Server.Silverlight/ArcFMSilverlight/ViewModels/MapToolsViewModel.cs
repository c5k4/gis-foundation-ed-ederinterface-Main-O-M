using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using ESRI.ArcGIS.Client.Tasks;
using System.Collections.Generic;

namespace ArcFMSilverlight.ViewModels
{
    public class MapToolsViewModel : BasicMapToolsViewModel, INotifyPropertyChanged, IActiveControl
    {
        #region Member Variables

        private Map _map;
        private MapTools _view;
        private bool _magnifyChecked;
        private bool _xyCommandChecked;
        private bool _insetVisible;
        private bool _isActive;
        private string _xCoordinate = EmptyCoordinate;
        private string _yCoordinate = EmptyCoordinate;
        private bool _firstTime = true;

        private const string EmptyCoordinate = " - ";
        private const string MagnifyCursor = ImagePath + "cursor_magnify.png";
        protected const string XYCursor = ImagePath + "coordinfo_cursor.png";

        #endregion Member Variables

        public MapToolsViewModel(MapTools view)
            : base()
        {
            if (view == null) throw new ArgumentNullException("view");

            Extents = new Extents();
            PreviousExtentCommand = new DelegateCommand(PreviousExtent, ContainsPreviousExtent);
            NextExtentCommand = new DelegateCommand(NextExtent, ContainsNextExtent);
            FixedZoomCommand = new DelegateCommand(Zoom);
            MagnifyCommand = new DelegateCommand(Magnify);
            XYCommand = new DelegateCommand(XY);
            IsNewExtent = true;

            _view = view;
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            IsActive = true;
        }

        public void EnablePan()
        {
            ZoomBox.IsEnabled = false;
            PanChecked = true;
            ZoomOutChecked = false;
            ZoomInChecked = false;
            MagnifyChecked = false;
            XYCommandChecked = false;
            CursorSet.SetID(Map, PanCursor);
        }

        #region IActiveControl Members

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value)
                {
                    EventAggregator.GetEvent<ControlActivatedEvent>().Publish(_view);
                }
                _isActive = value;

                if (value == false)
                {
                    ZoomBox.IsEnabled = false;
                    PanChecked = false;
                    ZoomOutChecked = false;
                    ZoomInChecked = false;
                    MagnifyChecked = false;
                    XYCommandChecked = false;
                    CursorSet.SetID(Map, PanCursor);
                }
                else
                {
                    //ZoomBox.IsEnabled = false;
                    //PanChecked = true;
                    //ZoomOutChecked = false;
                    //ZoomInChecked = false;
                    //MagnifyChecked = false;
                    //CursorSet.SetID(Map, PanCursor);
                }
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == _view) return;

            IsActive = false;
        }

        #endregion IActiveControl Members

        #region Public Properties

        public DelegateCommand PreviousExtentCommand { get; set; }
        public DelegateCommand NextExtentCommand { get; set; }
        public DelegateCommand FixedZoomCommand { get; set; }
        public DelegateCommand MagnifyCommand { get; set; }
        public DelegateCommand XYCommand { get; set; }

        public Map Map
        {
            get { return _map; }
            set
            {
                UnsubscribeMapEvents(_map);
                SubscribeMapEvents(value);
                _map = value;
                InitZoomBox(_map);
                OnPropertyChanged("Map");
            }
        }

        public bool MagnifyChecked
        {
            get { return _magnifyChecked; }
            set
            {
                _magnifyChecked = value;
                OnPropertyChanged("MagnifyChecked");
            }
        }

        public bool InsetVisible
        {
            get { return _insetVisible; }
            set
            {
                _insetVisible = value;
                OnPropertyChanged("InsetVisible");

                if (!_firstTime)
                {
                    _view.MapInset.ViewModel.Visible = InsetVisible;
                }
                else
                {
                    _firstTime = false;
                }
            }
        }

        public string XCoordinate
        {
            get { return _xCoordinate; }
            set
            {
                _xCoordinate = value;
                OnPropertyChanged("XCoordinate");
            }
        }

        public string YCoordinate
        {
            get { return _yCoordinate; }
            set
            {
                _yCoordinate = value;
                OnPropertyChanged("YCoordinate");
            }
        }

        #endregion Public Properties

        #region Private Properties

        private bool IsNewExtent { get; set; }
        private Extents Extents { get; set; }

        

        public bool XYCommandChecked
        {
            get { return _xyCommandChecked; }
            set
            {
                if (value == true)
                    ConfigUtility.WKID = 4326;
                else
                    ConfigUtility.WKID = 0;

                _xyCommandChecked = value;
                OnPropertyChanged("XYCommandChecked");
            }
        }

        #endregion Private Properties

        #region Event Handlers

        void Map_ExtentChanged(object sender, ExtentEventArgs e)
        {
            if (IsNewExtent)
            {
                Extents.Add(e.NewExtent);
            }
            IsNewExtent = true;
            PreviousExtentCommand.RaiseCanExecuteChanged();
            NextExtentCommand.RaiseCanExecuteChanged();
        }

        protected override void ZoomBox_DrawComplete(object sender, DrawEventArgs e)
        {
            if (MagnifyChecked)
            {
                InsetVisible = true;
                _view.MapInset.ViewModel.UpdateExtent(e.Geometry.Extent);
            }
            else
            {
                base.ZoomBox_DrawComplete(sender, e);
            }
        }

        void Map_MouseLeave(object sender, MouseEventArgs e)
        {
            XCoordinate = EmptyCoordinate;
            YCoordinate = EmptyCoordinate;
        }

        void Map_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(Map);
            MapPoint mapPoint = Map.ScreenToMap(position);
            if (mapPoint != null)
            {

                if (ConfigUtility.WKID != 0)
                {
                    SpatialReference sp = new SpatialReference(ConfigUtility.WKID);

                    var graphic = new Graphic
                    {
                        Geometry = mapPoint
                    };

                    var graphicList = new List<Graphic> { graphic };
                    var geometryService = new GeometryService(ConfigUtility.GeometryServiceURL);

                    geometryService.Failed += GeometryServiceFailed;
                    geometryService.ProjectCompleted += GeometryServiceNavigateProjectCompleted;
                    geometryService.ProjectAsync(graphicList, sp);
                }
                else
                {
                    XCoordinate = "X:" + string.Format("{0}", Math.Round(mapPoint.X, 4));
                    YCoordinate = "Y:" + string.Format("{0}", Math.Round(mapPoint.Y, 4));
                }
            }
        }

        private void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            //logger.Error("Coordinates tool geometry task failed! " + e.Error.Message);
            //throw new Exception("CoordinatesControl: ProjectionError: " + e.Error.Message);
        }
                
        private void GeometryServiceNavigateProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var point = (MapPoint)e.Results[0].Geometry;

                XCoordinate = "Lat:" + string.Format("{0}", Math.Round(point.Y, 5));
                YCoordinate = "Long:" + string.Format("{0}", Math.Round(point.X, 5));
                
            }
            catch (Exception ex)
            {
                throw new Exception("CoordinatesControl: An error occurred. " + ex.Message);
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void UnsubscribeMapEvents(Map map)
        {
            if (map == null) return;
            map.ExtentChanged -= new EventHandler<ExtentEventArgs>(Map_ExtentChanged);
            map.MouseMove -= Map_MouseMove;
            map.MouseLeave -= Map_MouseLeave;
            
        }

        
        private void SubscribeMapEvents(Map map)
        {
            if (map == null) return;
            map.ExtentChanged += new EventHandler<ExtentEventArgs>(Map_ExtentChanged);
            map.MouseMove += Map_MouseMove;            
            map.MouseLeave += Map_MouseLeave;
        }

        private void Zoom(object parameter)
        {
            if (parameter == null) return;

            double zoomFactor = 0;
            double.TryParse(parameter.ToString(), out zoomFactor);

            Map.Zoom(zoomFactor);
        }

        protected override void ZoomIn(object parameter)
        {
            base.ZoomIn(parameter);

            MagnifyChecked = false;
            IsActive = true;
        }

        protected override void ZoomOut(object parameter)
        {
            base.ZoomOut(parameter);

            MagnifyChecked = false;
            IsActive = true;
        }

        protected virtual void XY(object parameter)
        {
            ZoomBox.IsEnabled = false;
            ZoomInChecked = false;
            PanChecked = false;
            MagnifyChecked = false;
            IsActive = true;
            CursorSet.SetID(MapToControl, XYCursor);

        }

        protected override void Pan(object parameter)
        {
            base.Pan(parameter);

            MagnifyChecked = false;
            IsActive = true;
        }


        private void Magnify(object parameter)
        {
            ZoomBox.IsEnabled = true;
            ZoomInChecked = false;
            ZoomOutChecked = false;
            PanChecked = false;
            IsActive = true;
            CursorSet.SetID(Map, MagnifyCursor);
        }

        private bool ContainsNextExtent(object parameter)
        {
            return Extents.HasNextExtent;
        }

        private bool ContainsPreviousExtent(object parameter)
        {
            return Extents.HasPreviousExtent;
        }

        private void PreviousExtent(object parameter)
        {
            IsNewExtent = false;
            Map.ZoomTo(Extents.PreviousExtent);
        }

        private void NextExtent(object parameter)
        {
            IsNewExtent = false;
            Map.ZoomTo(Extents.NextExtent);
        }

        #endregion Private Methods
    }
}
