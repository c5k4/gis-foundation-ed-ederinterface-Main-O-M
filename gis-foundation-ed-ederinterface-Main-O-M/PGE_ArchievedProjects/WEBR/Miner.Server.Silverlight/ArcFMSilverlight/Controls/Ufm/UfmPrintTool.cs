using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
    public class UfmPrintTool: Control
    {
        private ToggleButton _ufmPrintToggleButton;
        private FloatableWindow _fw = null;
        private Grid _mapArea;
        private UfmPrintControl _ufmPrintControl;
        public UfmPrintControl UfmPrintControl
        {
            get { return _ufmPrintControl; }
        }

        public UfmPrintTool(ToggleButton ufmPrintToggleButton, Map map, string geometryService, Grid mapArea)
        {
            _ufmPrintToggleButton = ufmPrintToggleButton;
            this.Map = map;
            _mapArea = mapArea;
            // We're not actually using templates at the mo' but using same framework
            OnApplyTemplate();
            _ufmPrintToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_ufmPrintToggleButton, "Print UFM Diagram");
            _ufmPrintControl = new UfmPrintControl(map, GetUfmPrintService());
            _ufmPrintControl.txtSendTo.Text = WebContext.Current.User.Name.Replace("PGE\\", "");
        }

        private string GetUfmPrintService()
        {
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            return xe.Element("UfmPrintSendService").Attribute("Url").Value;
        }

        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(UfmPrintTool), null);

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
            typeof(UfmPrintTool),
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

            _ufmPrintToggleButton.Click += new RoutedEventHandler(_exportToggleButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _ufmPrintToggleButton.IsEnabled = false;
                ToolTipService.SetToolTip(_ufmPrintToggleButton, "No Geometry Service Configured.");
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
                _ufmPrintToggleButton.IsChecked = false;
                _ufmPrintControl.ClearGraphics();
                _ufmPrintControl.StopListeningToMapExtentChanged();
            }
            else // Active
            {
//                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                _ufmPrintToggleButton.IsChecked = true;
                _ufmPrintControl.IsEnabled = true;
                _ufmPrintControl.PrintButton.IsEnabled = true;
                _ufmPrintControl.InitializeDrawGraphics();
                _fw.Visibility = System.Windows.Visibility.Visible;
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(UfmPrintTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var UfmPrintTool = (UfmPrintTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
            }
        }


        #endregion IActiveControl Members

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measure = d as UfmPrintTool;
            if (measure == null) return;

        }

        void _exportToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _fw.Width = 210;
                _fw.Height = 190;
                _fw.Title = "UFM Print";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.NoResize;
                _fw.Content = _ufmPrintControl;
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
                    _ufmPrintToggleButton.IsChecked = true;
                    setActive(true);
                }
                else
                {
                    _fw.Visibility = System.Windows.Visibility.Collapsed;
                    _ufmPrintToggleButton.IsChecked = false;
                    _ufmPrintControl.ClearGraphics();
                }
            }
        }


        void _fw_Closing(object sender, CancelEventArgs e)
        {
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            _ufmPrintControl.StopListeningToMapExtentChanged();
            _ufmPrintControl.RotationUpDown.Value = 0;
        }


    }
}
