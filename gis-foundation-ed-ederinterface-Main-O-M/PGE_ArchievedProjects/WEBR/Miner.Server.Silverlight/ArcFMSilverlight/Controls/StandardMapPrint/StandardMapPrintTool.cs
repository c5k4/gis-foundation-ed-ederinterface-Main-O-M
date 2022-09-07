using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ESRI.ArcGIS.Client;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;

namespace ArcFMSilverlight
{
    public class StandardMapPrintTool : Control, IActiveControl
    {
        private ToggleButton _StandardPrintToggleButton;
        private const string ExportCursor = @"/Images/cursor_measure.png";
        public FloatableWindow _fw = null;
        private Grid _mapArea;
        private MapTools _mapTools;
        private StandardPrintPage _StandardMapPrintTool;
        public StandardPrintPage StandardPrintPage
        {
            get { return _StandardMapPrintTool; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public StandardMapPrintTool(ToggleButton StandardPrintToggleButton, Map map, string geometryService, Grid mapArea, MapTools mapTools)
        {
            DefaultStyleKey = typeof(StandardMapPrintTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _StandardPrintToggleButton = StandardPrintToggleButton;
            this.Map = map;
            _mapTools = mapTools;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _StandardPrintToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_StandardPrintToggleButton, "Standard Map Print");
            _StandardMapPrintTool = new StandardPrintPage(map, mapTools);
            // _StandardMapPrintTool.txtSendTo.Text = WebContext.Current.User.Name.Replace("PGE\\", "");            
        }

        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(StandardMapPrintTool), null);

        [Category("Measure Properties")]
        [Description("Geometry Service URL")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string GeometryService
        {
            get { return (string)GetValue(GeometryServiceProperty); }
            set
            {
                SetValue(GeometryServiceProperty, value);
            }
        }


        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(StandardMapPrintTool),
            new PropertyMetadata(OnMapChanged));

        [Category("Export Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        #region Public Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _StandardPrintToggleButton.Click += new RoutedEventHandler(_StandardPrintToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _StandardPrintToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_StandardPrintToggleButton, "No Geometry Service Configured.");
            }

        }

        #endregion Public Overrides

        #region IActiveControl Members

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { setActive(value); }
        }

        private void setActive(bool isActive)
        {
            _isActive = isActive;
            if (_fw == null) return;

            if (!isActive)
            {
                // Clear everything

                bool polygonDrawn = _StandardMapPrintTool.isPolygonDrawn;
                _StandardMapPrintTool.DisableDrawPolygon();
                _StandardMapPrintTool.isPolygonDrawn = polygonDrawn;
                _StandardMapPrintTool.ResetFilterGraphics();
                Map.ExtentChanged -= new EventHandler<ExtentEventArgs>(_StandardMapPrintTool.Map_ExtentChanged);
                _fw.Visibility = System.Windows.Visibility.Collapsed;
                _StandardPrintToggleButton.IsChecked = false;
            }
            else // Active
            {

                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                _StandardPrintToggleButton.IsChecked = true;


                _fw.Visibility = System.Windows.Visibility.Visible;
                Map.ExtentChanged += new EventHandler<ExtentEventArgs>(_StandardMapPrintTool.Map_ExtentChanged);   //added for map selection option
                _StandardMapPrintTool.Map_ExtentChanged(null, null);
              //  _StandardMapPrintTool.EnableDisableRadioButtons();
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(StandardMapPrintTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var standPrintTool = (StandardMapPrintTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                //                measure.MeasureMode = null;
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(standPrintTool);
                CursorSet.SetID(standPrintTool.Map, ExportCursor);
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        #endregion IActiveControl Members

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measure = d as StandardMapPrintTool;
            if (measure == null) return;

        }

        void _StandardPrintToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _fw.Width = 460;
                _fw.Height = 380;

                _fw.Title = "Standard Map Print";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _StandardMapPrintTool;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                _StandardMapPrintTool.GetFloatingWindowHeight(_fw);
                //  _StandardMapPrintTool.ClearSession = false;
                //if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                //{
                //    _fw.Height = 360;
                //}
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _StandardPrintToggleButton.IsChecked = true;
                    //_cadExportControl.PopulateCircuitIds();
                    //if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    //{
                    //    _fw.Height = 360;
                    //}
                    setActive(true);
                    if (_StandardMapPrintTool.Rdpoly.IsChecked == true)
                    {
                        _StandardMapPrintTool.Rdpoly.IsChecked = false;
                        _StandardMapPrintTool.Rdpoly.IsChecked = true;
                    }
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _StandardPrintToggleButton.IsChecked = false;
                    _StandardMapPrintTool.ClearGraphics();
                    _StandardMapPrintTool.SelectPolygonEvent(false);
                    _StandardMapPrintTool.ResetFilterGraphics();
                    if (this.PreviousControl != null) this.PreviousControl();
                }
            }
        }

        void getCircuitIds()
        {

        }

        void _fw_Closing(object sender, CancelEventArgs e)
        {
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            _StandardMapPrintTool.ResetFilterGraphics();
            //  _StandardMapPrintTool.StopListeningToMapExtentChanged();
            _StandardMapPrintTool.ClearGraphics();
            if (this.PreviousControl != null) this.PreviousControl();
        }
    }
}
