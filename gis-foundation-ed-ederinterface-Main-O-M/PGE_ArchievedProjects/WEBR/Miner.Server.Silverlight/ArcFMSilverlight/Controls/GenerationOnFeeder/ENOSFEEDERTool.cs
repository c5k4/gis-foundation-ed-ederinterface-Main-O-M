using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ESRI.ArcGIS.Client;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using ArcFMSilverlight.Controls.GenerationOnFeeder;

namespace ArcFMSilverlight
{
    public class ENOSFEEDERTool : Control, IActiveControl
    {
        private ToggleButton _genFeederToggleButton;
        private const string ExportCursor = @"/Images/cursor_measure.png";
        public FloatableWindow _fw = null;
        private Grid _mapArea;
        private MapTools _mapTools;
        private FEEDERToolPage _FEEDERTool;

        public FEEDERToolPage FEEDERToolPage
        {
            get { return _FEEDERTool; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public ENOSFEEDERTool(ToggleButton GenFeederToggleButton, Map map, string geometryService, Grid mapArea, MapTools mapTools, string DeviceSettingUrl)
        {
            DefaultStyleKey = typeof(ENOSFEEDERTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _genFeederToggleButton = GenFeederToggleButton;
            this.Map = map;
            _mapTools = mapTools;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _genFeederToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_genFeederToggleButton, "Select Feeder Name/ID");
            _FEEDERTool = new FEEDERToolPage(map, mapTools, DeviceSettingUrl, this);

            // _FEEDERTool.txtSendTo.Text = WebContext.Current.User.Name.Replace("PGE\\", "");            
        }

        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(ENOSFEEDERTool), null);

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
            typeof(ENOSFEEDERTool),
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

            _genFeederToggleButton.Click += new RoutedEventHandler(_genFeederToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _genFeederToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_genFeederToggleButton, "No Geometry Service Configured.");
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

                bool polygonDrawn = _FEEDERTool.isPolygonDrawn;
                //  _FEEDERTool.DisableDrawPolygon();
                _FEEDERTool.isPolygonDrawn = polygonDrawn;
                _FEEDERTool.ResetFilterGraphics();
                //   Map.ExtentChanged -= new EventHandler<ExtentEventArgs>(_FEEDERTool.Map_ExtentChanged);
                _fw.Visibility = System.Windows.Visibility.Collapsed;
                _genFeederToggleButton.IsChecked = false;
            }
            else // Active
            {


                _genFeederToggleButton.IsChecked = true;


                _fw.Visibility = System.Windows.Visibility.Visible;
                //   Map.ExtentChanged += new EventHandler<ExtentEventArgs>(_FEEDERTool.Map_ExtentChanged);   //added for map selection option
                // _FEEDERTool.EnableDisableRadioButtons();
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ENOSFEEDERTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cadExportTool = (ENOSFEEDERTool)d;

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
            var measure = d as ENOSFEEDERTool;
            if (measure == null) return;

        }

        void _genFeederToggleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(null);
        }

        public void OpenDialog(string CircuitId) //ENOS Tariff Change- Added Parameter
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                //_fw.Width = 460;
                //_fw.Height = 10;
                //ENOS Tariff Change- Start
                if (!string.IsNullOrEmpty(CircuitId))
                {
                    _fw.Title = "Generation On Feeder (Feeder Number: " + CircuitId + ")";
                }
                else
                {
                    _fw.Title = "Generation On Feeder";
                }
                //ENOS Tariff Change - End
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _FEEDERTool;
                _FEEDERTool.FeederToolBorder.Height = 100;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                _FEEDERTool.GetFloatingWindowHeight(_fw);

                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _genFeederToggleButton.IsChecked = true;

                    setActive(true);
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _genFeederToggleButton.IsChecked = false;

                    _FEEDERTool.ResetFilterGraphics();
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
            _FEEDERTool.ResetFilterGraphics();
            //  _FEEDERTool.StopListeningToMapExtentChanged();
            //  _FEEDERTool.ClearGraphics();
            if (this.PreviousControl != null) this.PreviousControl();
            string layerId = GetGenOnFeeder.feederZoomGraphicID;
            _FEEDERTool.FeederToolBorder.Height = 100;
            GraphicsLayer layer = this.Map.Layers[layerId] as GraphicsLayer;
            if (layer != null)
            {
                layer.Graphics.Clear();
            }
            _FEEDERTool.PART_FeederAutoCompleteTextBlock.Text = "";
            _FEEDERTool.txtFeederId.Text = ""; //ENOS Tariff Change
            _FEEDERTool.genOnFeederGrid.ClearValue(DataGrid.ItemsSourceProperty);
            _FEEDERTool.GenOnFeederStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            _FEEDERTool.SettingsButton.Visibility = System.Windows.Visibility.Collapsed;
            _FEEDERTool.ExportToExcel_GenOnFeeder.Visibility = System.Windows.Visibility.Collapsed;
            _FEEDERTool.BusyIndicator.IsBusy = false;
            _FEEDERTool.isCalledFromTool = false;
        }
    }
}
