using System.Collections.Generic;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// The item for layer visibility.
    /// </summary>
    public class LayerItem
    {
        public Layer Layer { get; private set; }
        public List<LayerItem> LayerItems { get; private set; }
        public string Label { get; private set; }
        public int SubLayerID { get; private set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }

        public LayerItem(Layer layer, string label, int subLayerID, bool isVisible, bool isEnabled, Map map = null)
        {
            Layer = layer;
            Label = label;
            SubLayerID = subLayerID;
            IsVisible = isVisible;
            IsEnabled = isEnabled;
            LayerItems = new List<LayerItem>();

            LayerVisibilityTree.AddItem(this, map);
        }
    }
}