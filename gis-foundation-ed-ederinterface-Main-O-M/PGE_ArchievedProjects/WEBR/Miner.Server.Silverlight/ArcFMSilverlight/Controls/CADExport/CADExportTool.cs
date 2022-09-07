using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using AreaUnit = ESRI.ArcGIS.Client.Actions.AreaUnit;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;

namespace ArcFMSilverlight
{
    public class CADExportTool : Control, IActiveControl
    {
        private const string ElementCADExportToggleButton = "PART_CADExportToggleButton";

        private ToggleButton _cadExportToggleButton;
        private const string ExportCursor = @"/Images/cursor_measure.png";

        public FloatableWindow _fw = null;
        private Grid _mapArea;
        private MapTools _mapTools;
        private CADExportControl _cadExportControl;
        public CADExportControl CADExportControl
        {
            get { return _cadExportControl; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public CADExportTool(ToggleButton cadExportToggleButton, Map map, string geometryService, Grid mapArea, MapTools mapTools)
        {
            DefaultStyleKey = typeof(CADExportTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _cadExportToggleButton = cadExportToggleButton;
            this.Map = map;
            _mapTools = mapTools;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _cadExportToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_cadExportToggleButton, "Export to CAD.");
            _cadExportControl = new CADExportControl(map, mapTools);
            _cadExportControl.txtSendTo.Text = WebContext.Current.User.Name.Replace("PGE\\", "");

        }


        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(CADExportTool), null);

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
            typeof(CADExportTool),
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

            _cadExportToggleButton.Click += new RoutedEventHandler(_exportToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _cadExportToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_cadExportToggleButton, "No Geometry Service Configured.");
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
                _cadExportToggleButton.IsChecked = false;
                _cadExportControl.ClearGraphics();
                _cadExportControl.StopListeningToMapExtentChanged();
            }
            else // Active
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                CursorSet.SetID(this.Map, ExportCursor);
                _cadExportToggleButton.IsChecked = true;
                _cadExportControl.IsEnabled = true;
                _cadExportControl.Export.IsEnabled = true;
                _cadExportControl.InitializeDrawGraphics();
                _fw.Visibility = System.Windows.Visibility.Visible;
                _cadExportControl.StartListeningToMapExtentChanged();
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(CADExportTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cadExportTool = (CADExportTool)d;

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
            var measure = d as CADExportTool;
            if (measure == null) return;

        }

        void _exportToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                /***********************************31Oct2016 Start*******************************/
                _fw.Width = 265;
                _fw.Height = 270;
                /***********************************31Oct2016 Ends*******************************/
                _fw.Title = "DWG Export";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _cadExportControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                _cadExportControl.PopulateCircuitIds();

                /***********************************31Oct2016 Start*******************************/
                _cadExportControl.DwgFloatWindow = _fw;
                _cadExportControl.populateMapType();
                _cadExportControl.populateVoltageFilterDropdown();
                _cadExportControl.populateFeederFilterDropdown();
                _cadExportControl.populateExportLayoutDropdown();
                if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                {
                    _fw.Height = 420;
                    CADExportControl.Height = 420;
                    CADExportControl.stdwgPanel.Height = 420;
                }
                else if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("DIST") && _mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("50"))
                {
                    _fw.Height = 405;
                    CADExportControl.Height = 405;
                    CADExportControl.stdwgPanel.Height = 405;
                }
                /***********************************31Oct2016 Ends*******************************/
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _cadExportToggleButton.IsChecked = true;
                    _cadExportControl.PopulateCircuitIds();
                    _cadExportControl.cboExportFormat.SelectedItem = _cadExportControl.cboExportFormat.Items[0];
                    /***********************************31Oct2016 Start*******************************/
                    _cadExportControl.populateMapType();
                    _cadExportControl.populateVoltageFilterDropdown();
                    _cadExportControl.populateFeederFilterDropdown();
                    _cadExportControl.populateExportLayoutDropdown();
                    if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATIC"))
                    {
                        _fw.Height = 420;
                        CADExportControl.Height = 420;
                        CADExportControl.stdwgPanel.Height = 420;
                    }
                    else if (_mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("DIST") && _mapTools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("50"))
                    {
                        _fw.Height = 405;
                        CADExportControl.Height = 405;
                        CADExportControl.stdwgPanel.Height = 405;
                    }
                    /***********************************31Oct2016 Ends*******************************/
                    setActive(true);
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportToggleButton.IsChecked = false;
                    _cadExportControl.ClearGraphics();
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
            _cadExportControl.StopListeningToMapExtentChanged();
            _cadExportControl.ClearGraphics();
            if (this.PreviousControl != null) this.PreviousControl();
        }


    }
}
