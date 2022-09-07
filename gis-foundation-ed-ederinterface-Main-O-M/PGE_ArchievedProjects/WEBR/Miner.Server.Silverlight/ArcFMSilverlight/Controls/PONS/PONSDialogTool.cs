using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Net;
using System.Json;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using System.Xml.Linq;
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
    public class PONSDialogTool : Control, IActiveControl
    {
        string _DeviceServiceURL = "";
        string _GetCustomerServiceURL = "";
        string _CGCServiceURL = "";
        //ObservableCollection<Pdfcustomerdata> CustomerResult = new ObservableCollection<Pdfcustomerdata>();
        //ObservableCollection<TransformercustomerdataComb> CustomerSearchData = new ObservableCollection<TransformercustomerdataComb>();
        private ToggleButton _NotificationToggleButton;

        public PONSDialogTool _PONSDialogtool;
        private FloatableWindow _fw = null;
        private Grid _mapArea;
        private PONSDialog _ponsDialogControl;
        public PONSDialog PONSDialogControl
        {
            get { return _ponsDialogControl; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }

        public PONSDialogTool(ToggleButton NotificationToggleButton, Map map, Grid mapArea)
        {
            DefaultStyleKey = typeof(PONSDialogTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _NotificationToggleButton = NotificationToggleButton;
            this.Map = map;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _NotificationToggleButton.IsEnabled = true;

            ToolTipService.SetToolTip(NotificationToggleButton, "Planned Outage Customer Notification");
            _ponsDialogControl = new PONSDialog(map);

        }



        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(PONSDialogTool),
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

            _NotificationToggleButton.Click += new RoutedEventHandler(_NotificationToggleButton_Click);



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
                //disable draw polygon
                bool polygonDrawn = _ponsDialogControl.isPolygonDrawn;
                _ponsDialogControl.DisableDrawPolygon();
                _ponsDialogControl.isPolygonDrawn = polygonDrawn;
                Map.ExtentChanged -= new EventHandler<ExtentEventArgs>(_ponsDialogControl.Map_ExtentChanged_Polygon);
                _fw.Visibility = System.Windows.Visibility.Collapsed;
                _NotificationToggleButton.IsChecked = false;

            }
            else // Active
            {

                _NotificationToggleButton.IsChecked = true;


                _fw.Visibility = System.Windows.Visibility.Visible;
                Map.ExtentChanged += new EventHandler<ExtentEventArgs>(_ponsDialogControl.Map_ExtentChanged_Polygon);   //added for map selection option

            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(PONSDialogTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ponsDialogTool = (PONSDialogTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                //                measure.MeasureMode = null;
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
            var measure = d as PONSDialogTool;
            if (measure == null) return;

        }

        void _NotificationToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _fw.Width = 660;
                _fw.Height = 455;
                _fw.Title = "Notification Search";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _ponsDialogControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.Show();
                _ponsDialogControl.GetFloatingWindowHeight(_fw);
                _ponsDialogControl.ClearSession = false;
                setActive(true);
            }
            else
            {
                if (_fw.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fw.Visibility = System.Windows.Visibility.Visible;
                    _NotificationToggleButton.IsChecked = true;
                    setActive(true);
                    if (_ponsDialogControl.RdSelectMap.IsChecked == true)
                    {
                        _ponsDialogControl.RdSelectMap.IsChecked = false;
                        _ponsDialogControl.RdSelectMap.IsChecked = true;
                    }
                    _ponsDialogControl.Stoppreviouspage();
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _NotificationToggleButton.IsChecked = false;
                    _ponsDialogControl.ClearGraphics();
                    _ponsDialogControl.SelectPolygonEvent(false);
                    if (this.PreviousControl != null) this.PreviousControl();
                }
            }
        }


        void _fw_Closing(object sender, CancelEventArgs e)
        {
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            _ponsDialogControl.ClearSession = true;
            _ponsDialogControl.Stoppreviouspage();
            _ponsDialogControl.ClearGraphics();
            if (this.PreviousControl != null) this.PreviousControl();
        }

    }
}
