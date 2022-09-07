using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
{
#elif WPF
namespace Miner.Mobile.Client
{
#endif
    /// <summary>
    /// Layer Messaging Framework
    /// </summary>
    public static class LayerVisibilityTree
    {
        private static Dictionary<string, LayerItem> _itemIDs;
        private static Dictionary<Map, Dictionary<string, LayerItem>> ItemIDList { get; set; }
        private static Map _map;

        static LayerVisibilityTree()
        {
            RootList = new Dictionary<Map, List<LayerItem>>();
            ItemIDList = new Dictionary<Map, Dictionary<string, LayerItem>>();
        }

        public static List<LayerItem> Root { get; private set; }
        public static Dictionary<Map, List<LayerItem>> RootList { get; private set; }

        /// <summary>
        /// Fired when some layer is refreshed.
        /// </summary>
        public static event EventHandler LayerRefreshed;

        /// <summary>
        /// Fires when the visibility of some layer changes.
        /// </summary>
        public static event EventHandler<IsVisibleEventArgs> LayerVisibilityChanged;

        /// <summary>
        /// Invoked by TOC Control to send out the message that some layer has been changed.
        /// </summary>
        /// <param name="sender"></param>
        public static void OnLayerRefreshed(object sender)
        {
            EventHandler handler = LayerVisibilityTree.LayerRefreshed;
            if (handler != null)
            {
                handler(sender, null);
            }
        }
        /// <summary>
        /// Invoked by TOC Control to send out the message that the visibility of some layer has been changed.
        /// </summary>
        /// <param name="sender">Layer item</param>
        /// <param name="isVisible">Whether the layer is visible</param>
        /// <param name="isEnabledChanged">Whether the enabled property of the layer is changed.</param>
        public static void OnVisibilityChanged(object sender, bool isVisible, bool isEnabledChanged)
        {
            EventHandler<IsVisibleEventArgs> handler = LayerVisibilityTree.LayerVisibilityChanged;
            if (handler != null)
            {
                handler(sender, new IsVisibleEventArgs(isVisible, isEnabledChanged));
            }
        }

        /// <summary>
        /// Initialize the tree structure.
        /// </summary>
        public static void InitializeTree(Map map)
        {
            if (_map == null || map == _map)
            {
                Root = new List<LayerItem>();
                _itemIDs = new Dictionary<string, LayerItem>();
                _map = map;
            }
            else
            {
                RootList[map] = new List<LayerItem>();
                ItemIDList[map] = new Dictionary<string, LayerItem>();
            }
        }

        /// <summary>
        /// Add a layer item to the root.
        /// </summary>
        /// <param name="item">The layer item of a map service</param>
        /// <param name="map">Optional map</param>
        public static void AddItemToRoot(LayerItem item, Map map = null)
        {
            if (map == null || map == _map)
            {
                if (Root == null)
                {
                    Root = new List<LayerItem>();
                    _itemIDs = new Dictionary<string, LayerItem>();
                }

                Root.Add(item);
            }
            else if (RootList.ContainsKey(map))
            {
                RootList[map].Add(item);
            }
        }

        /// <summary>
        /// Find an item with specific layer, label and sublayer ID.
        /// </summary>
        /// <param name="layer">Layer instance</param>
        /// <param name="label">Label</param>
        /// <param name="subLayerID">Sublayer ID</param>
        /// <param name="map">Optional map</param>
        /// <returns></returns>
        public static LayerItem FindItem(Layer layer, string label, int subLayerID, Map map = null)
        {
            if (layer != null)
            {
                var id = layer.Url() + "/" + subLayerID + "/" + label;
                if (map == null || map == _map)
                {
                    if (_itemIDs.ContainsKey(id)) return _itemIDs[id];
                }
                else if (ItemIDList.ContainsKey(map))
                {
                    var ids = ItemIDList[map];
                    if (ids.ContainsKey(id)) return ids[id];
                }
            }

            return null;
        }

        public static bool IsMainMap(Map map)
        {
            return map == _map;
        }

        internal static void AddItem(LayerItem item, Map map = null)
        {
            if (item.Layer != null)
            {
                var id = item.Layer.Url() + "/" + item.SubLayerID + "/" + item.Label;
                if (map == null || map == _map)
                {
                    if (!_itemIDs.ContainsKey(id))
                    {
                        _itemIDs.Add(id, item);
                    }
                }
                else if (ItemIDList.ContainsKey(map))
                {
                    if (!ItemIDList[map].ContainsKey(id))
                    {
                        ItemIDList[map].Add(id, item);
                    }
                }
            }
        }
    }
}