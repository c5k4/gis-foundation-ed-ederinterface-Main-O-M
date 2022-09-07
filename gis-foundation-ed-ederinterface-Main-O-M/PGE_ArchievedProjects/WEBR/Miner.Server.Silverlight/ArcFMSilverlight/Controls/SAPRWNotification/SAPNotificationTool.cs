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

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;

namespace ArcFMSilverlight
{
    public class SAPNotificationTool : Control, IActiveControl
    {       

        private ToggleButton _sapNotificationToggleButton;
        protected const string PanCursor = @"/Images/cursor_pan.png";
        private FloatableWindow _fw = null;
        private Grid _mapArea;
        private SAPNotificationControl _sapNotificationControl;
        public SAPNotificationControl SAPNotificationControl
        {
            get { return _sapNotificationControl; }
        }

        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public SAPNotificationTool(ToggleButton sapNotificationToggleButton, Map map)
        {
            DefaultStyleKey = typeof(SAPNotificationTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _sapNotificationToggleButton = sapNotificationToggleButton;
            this.Map = map;
            _mapArea = map.Parent as Grid;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _sapNotificationToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_sapNotificationToggleButton, "Map Correction Request");
            _sapNotificationControl = new SAPNotificationControl(map);
            _sapNotificationControl.txtLanId.Content = WebContext.Current.User.Name.Replace("PGE\\", "");
        }


        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(SAPNotificationTool), null);

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
            typeof(SAPNotificationTool),
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

            _sapNotificationToggleButton.Click += new RoutedEventHandler(_sapToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _sapNotificationToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_sapNotificationToggleButton, "No Geometry Service Configured.");
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
                _sapNotificationToggleButton.IsChecked = false;
                _sapNotificationControl.MapButtonClicked = false;
                
            }
            else // Active
            {
                //To retain the selected value when focus lost. It happens when we draw a WIP polygon

                if (_sapNotificationControl.IsOH == true)
                {
                    _sapNotificationControl.rbLineTypeUG.IsChecked = true;
                    _sapNotificationControl.rbLineTypeOH.IsChecked = true;
                }
                else
                {
                    _sapNotificationControl.rbLineTypeOH.IsChecked = true;
                    _sapNotificationControl.rbLineTypeUG.IsChecked = true;
                }
                if (_sapNotificationControl.IsEDCorrection == true)
                {
                    _sapNotificationControl.rbCorrectionTypeLB.IsChecked = true; 
                    _sapNotificationControl.rbCorrectionTypeED.IsChecked = true;
                }
                else
                {
                    _sapNotificationControl.rbCorrectionTypeED.IsChecked = true;
                    _sapNotificationControl.rbCorrectionTypeLB.IsChecked = true;

                }
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                _sapNotificationToggleButton.IsChecked = true;
                CursorSet.SetID(Map, PanCursor);        
                _fw.Visibility = System.Windows.Visibility.Visible;                
            }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(SAPNotificationTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sapExportTool = (SAPNotificationTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                //                measure.MeasureMode = null;
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(sapExportTool);                
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
            var measure = d as SAPNotificationTool;
            if (measure == null) return;

        }

        void _sapToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _fw.Width = 600;               
                _fw.Title = "Map Correction Form";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _sapNotificationControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _sapNotificationToggleButton.IsChecked = true;
                    setActive(true);
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _sapNotificationToggleButton.IsChecked = false;                    
                    if (this.PreviousControl != null) this.PreviousControl();
                }
            }
        }


        void _fw_Closing(object sender, CancelEventArgs e)
        {
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _sapNotificationControl.ResettheValues(); 
            _fw.Visibility = System.Windows.Visibility.Collapsed;            
            if (this.PreviousControl != null) this.PreviousControl();
        }
    }
}
