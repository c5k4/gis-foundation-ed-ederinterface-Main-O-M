using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;

namespace ArcFMSilverlight
{
    public partial class LegendControl : UserControl
    {
        private Map _map;

        public Legend Legend
        {
            get { return EsriLegend; }
            set { EsriLegend = value; }
        }

        public LegendControl(Map map, string[] layerIds)
        {
            InitializeComponent();
            _map = map;
            this.EsriLegend.Map = map;
            EsriLegend.LayerIDs = layerIds;
            EsriLegend.ShowOnlyVisibleLayers = true;
            EsriLegend.Refreshed += new EventHandler<ESRI.ArcGIS.Client.Toolkit.Legend.RefreshedEventArgs>(EsriLegend_Refreshed);
        }

        void CloseLayerItems(LayerItemViewModel layerItem)
        {
            layerItem.IsExpanded = false;

            if (layerItem.LayerItems != null)
            {
                foreach (var sublayerItem in layerItem.LayerItems)
                {
                    CloseLayerItems(sublayerItem);
                }                
            }
        }

        void EsriLegend_Refreshed(object sender, Legend.RefreshedEventArgs e)
        {
            CloseLayerItems(e.LayerItem);
        }
    }
}
