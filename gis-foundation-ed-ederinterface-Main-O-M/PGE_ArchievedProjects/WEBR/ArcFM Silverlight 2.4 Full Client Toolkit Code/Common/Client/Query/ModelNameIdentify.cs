using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Identify on using a common ArcFM model name
    /// </summary>
    public class ModelNameIdentify  : VisibleLayerIdentify
    {
        private readonly ModelName _modelName;
        private Layer _layer;

        public ModelNameIdentify(ModelName modelName)
        {
            if (modelName == null) throw new ArgumentNullException("modelName");

            _modelName = modelName;
        }

        /// <summary>
        /// The Name of the identify item that appears in the combo box
        /// </summary>
        public override string Name
        {
            get { return _modelName.Text; }
        }

        /// <summary>
        /// Execute the identify for this item
        /// </summary>
        /// <param name="map">current map</param>
        /// <param name="identifiedArea">area to identify</param>
        public override void IdentifyAsync(Map map, Geometry identifiedArea)
        {
            Results.Clear();

            _layer = map.LayerFromUrl(_modelName.Url);
            ESRI.ArcGIS.Client.Tasks.IdentifyParameters idParams = this.GetParameters(map, identifiedArea, _layer.LayerDefinitions());

            ExecuteTask(idParams, _modelName.Url, _layer);
        }

        internal protected override ESRI.ArcGIS.Client.Tasks.IdentifyParameters GetParameters(Map map, Geometry envelope, IEnumerable<LayerDefinition> layerDefs = null)
        {
            ESRI.ArcGIS.Client.Tasks.IdentifyParameters idParams = base.GetParameters(map, envelope, layerDefs);

            if ((base.UseClientLayerVisibility) && (_layer is ArcGISDynamicMapServiceLayer))
            {
                // get the layers which are both visible and belong to the model name 
                IEnumerable<int> visibleLayers = base.GetVisibleLayers(_layer, map) as IEnumerable<int>;
                IEnumerable<int> visibleModelNameLayers = (from id in _modelName.LayerIDs
                                                           where visibleLayers.Contains(id)
                                                           select id);
                idParams.LayerIds.AddRange(visibleModelNameLayers);
            }
            else
            {
                idParams.LayerIds.AddRange(_modelName.LayerIDs);
            }

            return idParams;
        }
    }
}
