using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ArcFMSilverlight.Butterfly;
using ESRI.ArcGIS.Client;
using Miner.Server.Client.Toolkit;

namespace ArcFMSilverlight.Controls.Butterfly
{
    public class ButterflyTool : Control, IActiveControl
    {
        private const int SIZE_WIDTH = 350;
        private const int SIZE_HEIGHT = 500;

        private Map _map;
        private Grid _mapArea;
        //private ToggleButton _legendToggleButton;
        private FloatableWindow _fw = null;
        private ButterflyControl _butterflyControl = null;

        //TODO: dry
        private string _basemapUrl;
        private string _commonLandbaseUrl;
        private string _ufmUrl;
        private int[] _ufmLayerIds;
        public string UfmFloorMapServiceUrl { get; private set; }
        private string _printTemplate;
        static public IList<string> ButterflyRolloverLayerNames { get; private set; }

        public ButterflyTool(Map map, Grid mapArea, XElement butterflyElement)
        {
            _map = map;
            _mapArea = mapArea;
            _ufmUrl = butterflyElement.Attribute("url").Value;
            _printTemplate = butterflyElement.Attribute("printTemplate").Value;
            string layersCSV = butterflyElement.Attribute("ids").Value;
            IList<string> layersArray = layersCSV.Split(',').ToList();
            _ufmLayerIds = layersArray.Select(x => Int32.Parse(x)).ToArray();
            UfmFloorMapServiceUrl = butterflyElement.Element("UfmFloor").Attribute("url").Value + "/" +
                                    butterflyElement.Element("UfmFloor").Attribute("LayerId").Value;

            // Get basemap (streets) and commonlandbase
            XElement rootElement = butterflyElement.Ancestors().First();
            XElement streetsElement = rootElement.Element("Layers")
                .Elements("Layer")
                .Where(l => l.Attribute("MapServiceName").Value == "Streets")
                .SingleOrDefault();
            XElement commonLandbaseElement = rootElement.Element("Layers")
                .Elements("Layer")
                .Where(l => l.Attribute("MapServiceName").Value == "Commonlandbase")
                .SingleOrDefault();
            _basemapUrl= streetsElement.Attribute("Url").Value;
            _commonLandbaseUrl = commonLandbaseElement.Attribute("Url").Value;

            ButterflyRolloverLayerNames =
                butterflyElement.Element("ButterflyRolloverLayerNames")
                    .Attribute("layerNamesCsv")
                    .Value.Split(',')
                    .ToList();
        }

        #region IActiveControl Members

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { setActive(value); }
        }

        public void Show(Graphic graphic, string structureNumber)
        {
            this.IsActive = true;
            _butterflyControl.CenterAt(graphic);
            _fw.Title = "Butterfly Diagram [ " + structureNumber + " ]";
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
                const int SIZE_BUFFER = 100;
                _fw.Width = (_mapArea.ActualWidth / 2) > SIZE_WIDTH ? (_mapArea.ActualWidth / 2) - 50 : SIZE_WIDTH;
                _fw.Height = _mapArea.ActualHeight > SIZE_HEIGHT ? _mapArea.ActualHeight - SIZE_BUFFER : SIZE_HEIGHT;
                _fw.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private FloatableWindow getFloatableWindow()
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                const int SIZE_BUFFER = 100;
                _fw.Width = (_mapArea.ActualWidth / 2) > SIZE_WIDTH ? (_mapArea.ActualWidth / 2) - 50 : SIZE_WIDTH;
                _fw.Height = _mapArea.ActualHeight > SIZE_HEIGHT ? _mapArea.ActualHeight - SIZE_BUFFER : SIZE_HEIGHT;
                _fw.Title = "Butterfly Diagram";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.ResizeMode = ResizeMode.CanResize;
                _butterflyControl = new ButterflyControl(_basemapUrl, _commonLandbaseUrl, _ufmUrl, _ufmLayerIds, _printTemplate);
                _butterflyControl.populateExportLayoutDropdown();
                _fw.Content = _butterflyControl;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.SizeChanged += new SizeChangedEventHandler(_fw_SizeChanged);
                _fw.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setActive(true);
            }
            return _fw;
        }

        void _fw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _butterflyControl.ButterflyMap.UpdateLayout();
        }

        void _fw_Closing(object sender, CancelEventArgs e)
        {
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }
        #endregion

    }
}
