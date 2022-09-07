using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;

using Miner.Server.Client.Toolkit;

namespace ArcFMSilverlight.ViewModels
{
    public class BasicMapToolsViewModel : INotifyPropertyChanged
    {
        #region Member Variables

        private bool _zoomOutChecked;
        private bool _xyCommandChecked;
        private bool _zoomInChecked;
        private bool _panChecked = true;

        protected const string ImagePath = @"/Images/";
        protected const string PanCursor = ImagePath + "cursor_pan.png";
        protected const string ZoomInCursor = ImagePath + "cursor_zoom_in.png";
        protected const string ZoomOutCursor = ImagePath + "cursor_zoom_out.png";
        protected const string XYCursor = ImagePath + "coordinfo_cursor.png";

        #endregion Member Variables

        public BasicMapToolsViewModel()
        {
            ZoomInCommand = new DelegateCommand(ZoomIn);
            ZoomOutCommand = new DelegateCommand(ZoomOut);
            PanCommand = new DelegateCommand(Pan);            
        }

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomOutCommand { get; set; }
        public ICommand PanCommand { get; set; }
        


        public bool ZoomOutChecked
        {
            get { return _zoomOutChecked; }
            set
            {
                _zoomOutChecked = value;
                OnPropertyChanged("ZoomOutChecked");
            }
        }

        public bool ZoomInChecked
        {
            get { return _zoomInChecked; }
            set
            {
                _zoomInChecked = value;
                OnPropertyChanged("ZoomInChecked");
            }
        }

        public bool PanChecked
        {
            get { return _panChecked; }
            set
            {
                _panChecked = value;
                OnPropertyChanged("PanChecked");
            }
        }

        #endregion Public Properties

        #region Protected Properties

        protected Draw ZoomBox { get; set; }

        protected Map MapToControl { get; set; }

        #endregion Protected Properties

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void InitZoomBox(Map map)
        {
            MapToControl = map;
            ZoomBox = new Draw(map)
            {
                FillSymbol = new SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(0x55, 0xFF, 0xFF, 0xFF))
                }
            };
            ZoomBox.DrawMode = DrawMode.Rectangle;
            ZoomBox.IsEnabled = false;
            ZoomBox.DrawComplete += new EventHandler<DrawEventArgs>(ZoomBox_DrawComplete);
        }

        protected virtual void ZoomBox_DrawComplete(object sender, DrawEventArgs e)
        {
            if (e == null) return;

            Envelope envelope = e.Geometry as Envelope;
            if (envelope == null) return;

            if (ZoomInChecked)
            {
                MapToControl.ZoomTo(envelope);
            }
            else if (ZoomOutChecked)
            {
                double factor = envelope.Width / MapToControl.Extent.Width;
                MapToControl.ZoomToResolution(MapToControl.Resolution / factor, envelope.GetCenter());
            }
        }

        protected virtual void ZoomIn(object parameter)
        {
            ZoomBox.IsEnabled = true;
            ZoomOutChecked = false;
            PanChecked = false;            
            CursorSet.SetID(MapToControl, ZoomInCursor);
        }

        protected virtual void ZoomOut(object parameter)
        {
            ZoomBox.IsEnabled = true;
            ZoomInChecked = false;
            PanChecked = false;            
            CursorSet.SetID(MapToControl, ZoomOutCursor);
        }

       

        protected virtual void Pan(object parameter)
        {
            ZoomBox.IsEnabled = false;
            ZoomInChecked = false;
            ZoomOutChecked = false;            
            ConfigUtility.WKID = 0;
            CursorSet.SetID(MapToControl, PanCursor);
        }

        #endregion Protected Methods
    }
}
