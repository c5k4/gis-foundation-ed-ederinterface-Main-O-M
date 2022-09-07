using System;
using System.Linq;
using System.Collections.Generic;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{
    /// <summary>
    /// The search item for a single layer in identify.
    /// </summary>
    public class SingleLayerIdentify : VisibleLayerIdentify
    {
        private readonly LayerInformation _layerInfo;
        private Layer _layer;

        public SingleLayerIdentify(LayerInformation info)
        {
            if (info == null) throw new ArgumentNullException("info");

            _layerInfo = info;
        }

        /// <summary>
        /// The name of the identify item, which is also the name of the layer
        /// </summary>
        public override string Name
        {
            get { return _layerInfo.Name; }
        }

        /// <summary>
        /// Execute the identify for this single layer.
        /// </summary>
        /// <param name="map">the current map</param>
        /// <param name="identifiedArea">the area to identify</param>
        public override void IdentifyAsync(Map map, Geometry identifiedArea)
        {
            Results.Clear();
            _layer = map.LayerFromUrl(_layerInfo.MapService);
            ESRI.ArcGIS.Client.Tasks.IdentifyParameters idParams = this.GetParameters(map, identifiedArea, _layer.LayerDefinitions());

            ExecuteTask(idParams, _layerInfo.MapService, _layer);
        }

        /// <summary>
        /// Clone the current instance of SingleLayerIdentify.
        /// </summary>
        /// <returns>Cloned instance</returns>
        public SingleLayerIdentify Clone()
        {
            var newIdentify = new SingleLayerIdentify(this._layerInfo)
            {
                UseClientLayerVisibility = this.UseClientLayerVisibility,
            };

            return newIdentify;
        }

        internal protected override ESRI.ArcGIS.Client.Tasks.IdentifyParameters GetParameters(Map map, Geometry envelope, IEnumerable<LayerDefinition> layerDefs = null)
        {
            ESRI.ArcGIS.Client.Tasks.IdentifyParameters idParams = base.GetParameters(map, envelope, layerDefs);

            if ((base.UseClientLayerVisibility) && (_layer is ArcGISDynamicMapServiceLayer))
            {
                idParams.LayerIds.AddRange(VisibleLayerIds(_layer, _layerInfo.ID));
            }
            else
            {
                idParams.LayerIds.Add(_layerInfo.ID);
            }

            return idParams;
        }

        private List<int> VisibleLayerIds(Layer layer, int layerId)
        {
             List<int> visibleLayers = new List<int>();
            
            // only dynamic map services have sublayers that can change visibility
            if (!(layer is ArcGISDynamicMapServiceLayer)) return visibleLayers;

            ArcGISDynamicMapServiceLayer dynamicMapService = layer as ArcGISDynamicMapServiceLayer;

            IsSubLayerVisible(dynamicMapService, layerId, ref visibleLayers);

            return visibleLayers;
        }

        private bool IsGrouplayer(LayerInfo layerInfo)
        {
            if (layerInfo.SubLayerIds == null) return false;
            return (layerInfo.SubLayerIds.Count() > 0);
        }

        private void IsSubLayerVisible(ArcGISDynamicMapServiceLayer dynamicMapService, int layerId, ref List<int> listOfSublayers)
        {
            LayerInfo layerInfo = (from li in dynamicMapService.Layers where li.ID == layerId select li).FirstOrDefault();

            bool useDefaultVisibility = (dynamicMapService.VisibleLayers == null);
            bool isGrouplayer = IsGrouplayer(layerInfo);
            
            bool IsVisible = false;
            if (useDefaultVisibility)
            {
                // nothing has been toggled in the TOC yet, use default visibility
                IsVisible = (from li in dynamicMapService.Layers
                                where li.DefaultVisibility
                                && (li.ID == layerId)
                                select li.ID).Count() > 0;
            }
            else
            {
                // is this layer in my list of visible layers? 
                IsVisible = (from id in dynamicMapService.VisibleLayers
                                where id == layerId
                                select id).Count() > 0;
            }

            // craziness!! ESRI has totally different behavior around group layers for default and client-side visibility
            if ((useDefaultVisibility && IsVisible && isGrouplayer ) ||
               (!useDefaultVisibility && isGrouplayer))
            {
                foreach (int subLayerId in layerInfo.SubLayerIds)
                {
                    // recurse
                    IsSubLayerVisible(dynamicMapService, subLayerId, ref listOfSublayers);
                }
            }
            else if (IsVisible)
            {
                listOfSublayers.Add(layerId);
            }

        }
    }
}
