using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using System.Text;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using NLog;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;

namespace ArcFMSilverlight
{
    public partial class NewPoleFromLATLONG : ChildWindow
    {
        public NewPoleFromLATLONG()
        {
            InitializeComponent();
           
        }

        private object _enteredLat = null;
        private object _enteredLong = null;
        private Map _MapProperty = null;
        private string _geometryServiceUrl = string.Empty;
        private PLCWidget _plcWidgetObj = null;
        private string _graphicLayerName = string.Empty;
        private bool _isNPLatLongActive = false;
        public Map MapProperty
        {
            get { return _MapProperty; }
            set { _MapProperty = value; }
        }

        public string GeometryServiceUrl
        {
            get { return _geometryServiceUrl; }
            set { _geometryServiceUrl = value; }
        }

        public PLCWidget PLCWidegetObj
        {
            get { return _plcWidgetObj; }
            set { _plcWidgetObj = value; }
        }

        public string PoleGraphicLayerName
        {
            get { return _graphicLayerName; }
            set { _graphicLayerName = value; }
        }

        public bool IsNPLatLongActive
        {
            get { return _isNPLatLongActive; }
            set { _isNPLatLongActive = value; }
        }

        private void CreatePole_Click(object sender, RoutedEventArgs e)
        {
            if (this.latitideTxt.Text != "" && this.longitudeTxt.Text != "")
            {
                if (ValidateLatLong(latitideTxt.Text, longitudeTxt.Text))
                {
                    _enteredLat = latitideTxt.Text;
                    _enteredLong = longitudeTxt.Text;
                    convertLatLongToXY();
                }
                else
                {
                    MessageBox.Show("Please Enter Valid Lat and Long.");
                }
            }
            else
            {
                MessageBox.Show("Please Enter Lat and Long.");
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
            _isNPLatLongActive = false;
            latitideTxt.Text = "";
            longitudeTxt.Text = "";
            GraphicsLayer layer = MapProperty.Layers[_graphicLayerName] as GraphicsLayer;
            if (layer != null)
            {
                if (layer.Graphics.Count > 0)
                {
                    layer.Graphics.Clear();
                }
            }
        }

        private bool ValidateLatLong(string Lat, string Long)
        {
            try
            {
                // Is it a double?
               double y= Convert.ToDouble(Lat);
               double x= Convert.ToDouble(Long);

                if ((x >= -180.0) && (x <= 0))
                {
                    // Y can be +-90
                    return ((y >= 0.0) && (y <= 90.0));
                }

                return false;

               // return true;
            }
            catch
            {
                return false;
            }
        }

        private void convertLatLongToXY()
        {
            try
            {
                var point = new MapPoint(Convert.ToDouble(_enteredLong), Convert.ToDouble(_enteredLat), new SpatialReference(4326));

                var graphic = new Graphic
                {
                    Geometry = point
                };

                var graphicList = new List<Graphic> { graphic };

                var geometryService = new GeometryService(_geometryServiceUrl);
                geometryService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_sapEquipIDqueryTask_Failed);
                geometryService.ProjectCompleted += GeometryServiceNavigateProjectCompleted;
                geometryService.ProjectAsync(graphicList, _MapProperty.SpatialReference, point);
            }
            catch (Exception ex)
            {

            }

        }

        private void GeometryServiceNavigateProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var point = (MapPoint)e.Results[0].Geometry;
                ConfigUtility.StatusBar.Text = "";
                Envelope envelope = new Envelope();
                if (point.Extent != null)
                {
                    envelope.XMax = point.Extent.XMax + 200;
                    envelope.XMin = point.Extent.XMin - 200;
                    envelope.YMax = point.Extent.YMax + 200;
                    envelope.YMin = point.Extent.YMin - 200;

                    _MapProperty.ZoomTo(envelope);

                    Graphic poleGraphic = new Graphic()
                    {
                        Geometry = point,

                    };

                    _plcWidgetObj.getPoleGraphicFromNewPoleLatLong(poleGraphic);
                }
                else
                {
                    MessageBox.Show("Please enter valid latitude and longitude.");
                }
                //MapControl.Layers.Add(graphicLayer);

            }
            catch (Exception ex)
            {
                throw new Exception("CoordinatesControl: An error occurred. " + ex.Message);
            }
        }
        void _sapEquipIDqueryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Geometry Service Task Failed";

        }

        private void NPoleLatLongClose_click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
            latitideTxt.Text = "";
            longitudeTxt.Text = "";
            _isNPLatLongActive = false;
            GraphicsLayer layer = MapProperty.Layers[_graphicLayerName] as GraphicsLayer;
            if (layer != null)
            {
                if (layer.Graphics.Count > 0)
                {
                    layer.Graphics.Clear();
                }
            }
        }



    }
}

