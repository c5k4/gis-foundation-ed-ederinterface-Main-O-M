using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{

    /// <summary>
    /// The identify item for a service. The children
    /// </summary>
    public class ServiceIdentify : VisibleLayerIdentify
    {
        internal Layer Layer { get; private set; }

        public ServiceIdentify(Layer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer is null.");

            Layer = layer;
        }

        public override string Name
        {
            get
            {
                return Layer.ID;
            }
        }

        public override void IdentifyAsync(Map map, Geometry identifiedArea)
        {
            Results.Clear();
            IdentifyParameters idParams = this.GetParameters(map, identifiedArea, Layer.LayerDefinitions());
            ExecuteTask(idParams, Layer.Url(), Layer);
        }

        internal protected override IdentifyParameters GetParameters(Map map, Geometry envelope, IEnumerable<LayerDefinition> layerDefs = null)
        {
            var idParams = base.GetParameters(map, envelope, layerDefs);
            if ((base.UseClientLayerVisibility) && (Layer is ArcGISDynamicMapServiceLayer))
            {
                idParams.LayerIds.AddRange(base.GetVisibleLayers(Layer, map));
            }

            return idParams;
        }

    }
}
