using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using Miner.Server.Client.Toolkit;

namespace ArcFMSilverlight.Controls.Legend
{
    public class LegendTool : Control, IActiveControl
    {
        private const int SIZE_WIDTH = 300;
        private const int SIZE_HEIGHT = 450;

        private Map _map;
        private Grid _mapArea;
        private ToggleButton _legendToggleButton;
        private FloatableWindow _fw = null;
        private LegendControl _legendControl = null;
        private string[] _legendLayers;

        public LegendTool(Map map, Grid mapArea, ToggleButton legendToggleButton, XElement legendElement)
        {
            _map = map;
            _mapArea = mapArea;
            _legendToggleButton = legendToggleButton;
            var attribute = legendElement.Attribute("displayLayers");
            _legendLayers = attribute.Value.Split(',');
        }

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
            if (getFloatableWindow() == null) return;

            if (!isActive)
            {
                // Clear everything
                _fw.Visibility = System.Windows.Visibility.Collapsed;
            }
            else // Active
            {
                // Reset in case the widget gets lost. Not ideal.
                _fw.Height = _mapArea.ActualHeight < SIZE_HEIGHT ? _mapArea.ActualHeight - 100 : SIZE_HEIGHT;
                _fw.Width = SIZE_WIDTH;
                _fw.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void SetVerticalAlignment(double height)
        {
            if (height >= SIZE_HEIGHT)
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
            else
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.Height = height - 100;
            }            
        }

        private FloatableWindow getFloatableWindow()
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _mapArea.SizeChanged += new SizeChangedEventHandler(_mapArea_SizeChanged);
                _fw.Width = SIZE_WIDTH;
                _fw.Height = SIZE_HEIGHT;
                _fw.Title = "Legend";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                SetVerticalAlignment(_mapArea.ActualHeight);
                _fw.ResizeMode = ResizeMode.CanResize;
                _legendControl = new LegendControl(_map, _legendLayers);
                _fw.Content = _legendControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.SizeChanged += new SizeChangedEventHandler(_fw_SizeChanged);
                _fw.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                //BUG: I cannot seem to modify the position of this window, come hell or high water. 
                // Presumably this is because we are setting ourselves to the MapArea, not a Canvas
                setActive(true);
            }
            return _fw;
        }

        void _mapArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetVerticalAlignment(e.NewSize.Height);
        }

        void _fw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const double MIN_HEIGHT = 50;
            const double MIN_WIDTH = 30;

            if (_fw.ActualHeight <= MIN_HEIGHT || _fw.Width <= MIN_WIDTH)
            {
                _legendControl.Legend.MinWidth = _fw.ActualWidth;
                _legendControl.Legend.MinHeight = _fw.ActualHeight;                
            }
            else
            {
                _legendControl.Legend.MinWidth = _fw.ActualWidth - 30;
                _legendControl.Legend.MinHeight = _fw.ActualHeight - 50;
            }
            _legendControl.Legend.UpdateLayout();
        }

        void _fw_Closing(object sender, CancelEventArgs e)
        {
            _legendToggleButton.IsChecked = false;
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            //if (this.PreviousControl != null) this.PreviousControl();
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }
        #endregion

    }
}
