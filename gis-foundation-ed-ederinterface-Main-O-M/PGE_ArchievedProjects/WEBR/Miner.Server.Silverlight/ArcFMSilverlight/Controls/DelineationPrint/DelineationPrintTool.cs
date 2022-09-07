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
    public class DelineationPrintTool : Control, IActiveControl
    {
        private ToggleButton _delineationPrintToggleButton;
        private const string ExportCursor = @"/Images/cursor_measure.png";
        public FloatableWindow _fw = null;
        private Grid _mapArea;
        private MapTools _mapTools;
        private DelineationPrintControl _delineationPrintControl;
        public DelineationPrintControl DelineationPrintControl
        {
            get { return _delineationPrintControl; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public DelineationPrintTool(ToggleButton cadExportToggleButton, Map map, string geometryService, Grid mapArea, MapTools mapTools)
        {
            DefaultStyleKey = typeof(DelineationPrintTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _delineationPrintToggleButton = cadExportToggleButton;
            this.Map = map;
            _mapTools = mapTools;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _delineationPrintToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_delineationPrintToggleButton, "Delineation Print");
            _delineationPrintControl = new DelineationPrintControl(map, mapTools);
            _delineationPrintControl.txtSendTo.Text = WebContext.Current.User.Name.Replace("PGE\\", "");            
        }
        
        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(DelineationPrintTool), null);

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
            typeof(DelineationPrintTool),
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

            _delineationPrintToggleButton.Click += new RoutedEventHandler(_delineationPrintToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _delineationPrintToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_delineationPrintToggleButton, "No Geometry Service Configured.");
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
                _fw.Visibility = System.Windows.Visibility.Collapsed;
                _delineationPrintToggleButton.IsChecked = false;
                _delineationPrintControl.ClearGraphics();
                _delineationPrintControl.StopListeningToMapExtentChanged();
            }
            else // Active
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                CursorSet.SetID(this.Map, ExportCursor);
                _delineationPrintToggleButton.IsChecked = true;
                _delineationPrintControl.IsEnabled = true;
                _delineationPrintControl.PrintButton.IsEnabled = true;
                _delineationPrintControl.InitializeDrawGraphics();
                _fw.Visibility = System.Windows.Visibility.Visible;
                _delineationPrintControl.StartListeningToMapExtentChanged();
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(DelineationPrintTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cadExportTool = (DelineationPrintTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                //                measure.MeasureMode = null;
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(cadExportTool);
                CursorSet.SetID(cadExportTool.Map, ExportCursor);
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
            var measure = d as DelineationPrintTool;
            if (measure == null) return;

        }

        void _delineationPrintToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _fw.Width = 340;
                _fw.Height = 290;
                //_fw.Height = 210;
                _fw.Title = "Delineation Print";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _delineationPrintControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                {
                    _fw.Height = 360;
                }
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _delineationPrintToggleButton.IsChecked = true;
                    //_cadExportControl.PopulateCircuitIds();
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    {
                        _fw.Height = 360;
                    }
                    setActive(true);
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _delineationPrintToggleButton.IsChecked = false;
                    _delineationPrintControl.ClearGraphics();
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
            _delineationPrintControl.StopListeningToMapExtentChanged();
            _delineationPrintControl.ClearGraphics();
            if (this.PreviousControl != null) this.PreviousControl();
        }
    }
}
