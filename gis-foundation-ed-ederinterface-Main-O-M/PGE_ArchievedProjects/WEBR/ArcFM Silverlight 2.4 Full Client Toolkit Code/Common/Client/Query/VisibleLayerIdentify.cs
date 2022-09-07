using System;
using System.Collections.Generic;
using System.Linq;

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
    /// The search item for visible layers in identify.
    /// </summary>
    public class VisibleLayerIdentify : IdentifyItem
    {
        public VisibleLayerIdentify()
        {
            ExecutingTasks = new List<IdentifyTask>();
        }

        #region Public Properties

        /// <summary>
        /// The name of the identify item "Visible Layers"
        /// </summary>
        public override string Name
        {
            get
            {
#if SILVERLIGHT
                return LocalizationManager.GetString("VisibleLayers");
#elif WPF
                return "Visible Layers";
#endif
            }
        }

        /// <summary>
        /// Gets and Sets the ability to ignore client-side layer visibility for identify
        /// Applies to ArcGISDynamicMapServiceLayers when used with a TOC control
        /// where the user can toggle individual layers off, in addition to the map services
        /// </summary>
        public bool UseClientLayerVisibility { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Execute the identify on all visible layers
        /// </summary>
        /// <param name="map">the current map</param>
        /// <param name="identifiedArea">the area to identify</param>
        public override void IdentifyAsync(Map map, Geometry identifiedArea)
        {
            if (map == null) throw new ArgumentNullException("map");

            Results.Clear();

            IList<Layer> layers = (from layer in map.Layers 
                                        // No image layers
                                        where (layer is ArcGISImageServiceLayer) == false &&
                                        // Must have a Url
                                        string.IsNullOrEmpty(layer.Url()) == false
                                        && layer.Visible
                                        select layer).ToList();

            if (layers.Count == 0)
            {
                base.OnIdentifyComplete(new ResultEventArgs(Results));
            }
            else
            {
                int emptyLayerNum = 0;
                foreach (Layer layer in layers)
                {
                    IdentifyParameters idParams = GetParameters(map, identifiedArea, layer);
                    string url = layer.Url();
                    if (layer is FeatureLayer)
                    {
                        url = GetFeatureLayerUrl(url, ref idParams);
                    }

                    emptyLayerNum = ExecuteTask(idParams, url, layer, layers.Count(), emptyLayerNum);
                }
            }
        }

        /// <summary>
        /// Cancel the visible layer identify
        /// </summary>
        public override void CancelAsync()
        {
            foreach (IdentifyTask task in ExecutingTasks)
            {
                task.CancelAsync();
            }
            ExecutingTasks.Clear();
        }

        #endregion Public Methods

        #region Protected Methods

        protected int ExecuteTask(IdentifyParameters idParams, string url, Layer layer, int layerNum = -1, int emptyLayerNum = -1)
        {
            // special case: passing in zero layer ids is the same as passing all of them
            if ((UseClientLayerVisibility) && (layer is ArcGISDynamicMapServiceLayer) && (idParams.LayerIds.Count == 0))
            {
                emptyLayerNum++;

                // just finish the identify
                if ((layerNum != -1 && emptyLayerNum == layerNum) || (layerNum == -1 && idParams.LayerIds.Count <= 0))
                {
                    base.OnIdentifyComplete(new ResultEventArgs(Results));
                }
                return emptyLayerNum;
            }

            var task = new IdentifyTask(url) { ProxyURL = layer.ProxyUrl() };
            ExecutingTasks.Add(task);
            task.Failed += (s, e) => RemoveTask((IdentifyTask)s);
            task.ExecuteCompleted += Task_ExecuteCompleted;
            task.ExecuteAsync(idParams);

            return emptyLayerNum;
        }

        protected List<int> GetVisibleLayers(Layer layer, Map map)
        {
            List<int> visibleLayers = new List<int>();

            // if the parent mapservice is not visible, also treat the children as not visible
            if (layer.Visible == false) return visibleLayers;

            ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
            if ((dynamicLayer == null) || (dynamicLayer.Layers == null) || (dynamicLayer.Layers.Count() < 1)) return visibleLayers;

            var root = LayerVisibilityTree.IsMainMap(map) ? LayerVisibilityTree.Root : LayerVisibilityTree.RootList[map];
            var layerItem = root.FirstOrDefault(l => l.Layer.Url() == layer.Url());
            if (layerItem != null)
            {
                GetVisibleSublayers(layerItem, visibleLayers);
            }

            return visibleLayers;
        }

        #endregion Protected Methods

        #region Internal Methods

        internal protected virtual IdentifyParameters GetParameters(Map map, Geometry geometry, IEnumerable<LayerDefinition> layerDefs = null)
        {
            IdentifyParameters idParams = new IdentifyParameters()
            {
                Geometry = geometry,
                Tolerance = 5,
                Height = (int)map.ActualHeight,
                Width = (int)map.ActualWidth,
                MapExtent = map.Extent,
                LayerOption = LayerOption.all,
                SpatialReference = map.SpatialReference,
                LayerDefinitions = layerDefs
            };
            return idParams;
        }

        #endregion Internal Methods

        #region Private Properties

        private List<IdentifyTask> ExecutingTasks { get; set; }

        #endregion Private Properties

        #region Private Methods

        private string GetFeatureLayerUrl(string url, ref IdentifyParameters idParams)
        {
            string featureLayerUrl = string.Empty;

            featureLayerUrl = url.Replace("FeatureServer", "MapServer");
            string layerID;
            int lastIndexofSlash = featureLayerUrl.LastIndexOf("/");
            layerID = featureLayerUrl.Substring(lastIndexofSlash + 1, featureLayerUrl.Length - lastIndexofSlash - 1);
            idParams.LayerIds.Add(Convert.ToInt32(layerID));
            featureLayerUrl = featureLayerUrl.Substring(0, lastIndexofSlash);

            return featureLayerUrl;
        }

        private void RemoveTask(IdentifyTask task)
        {
            ExecutingTasks.Remove(task);
            if (ExecutingTasks.Count == 0)
            {
                OnIdentifyComplete(new ResultEventArgs(Results));
            }
        }

        private void AddToResults(List<IdentifyResult> results, string service = null)
        {
            foreach (var result in results)
            {
                Tasks.IResultSet resultSet = Results.FirstOrDefault(set => (set.ID == result.LayerId) && (set.Service == service));
                if (resultSet == null)
                {
                    resultSet = new Tasks.ResultSet()
                    {
                        DisplayFieldName = result.DisplayFieldName,
                        ID = result.LayerId,
                        Name = result.LayerName,
                        GeometryType = Utility.GetGeometryType(result.Feature.Geometry),
                        Service = service
                    };

                    foreach (KeyValuePair<string, object> attribute in result.Feature.Attributes)
                    {
                        string key = Utility.RemoveSpecialCharactersFromAlias(attribute.Key);
                        resultSet.FieldAliases.Add(key, key);
                    }
                    Results.Add(resultSet);
                }

                GraphicPlus newGraphic = CreateNewGraphic(result.Feature);
                newGraphic.FieldAliases = resultSet.FieldAliases;
                newGraphic.DisplayFieldName = resultSet.DisplayFieldName;
                newGraphic.Attributes.Add("RowIndex", resultSet.Features.Count + 1);
                resultSet.Features.Add(newGraphic);
            }
        }

        private GraphicPlus CreateNewGraphic(Graphic oldGraphic)
        {
            var newGraphic = new GraphicPlus();
            newGraphic.Geometry = oldGraphic.Geometry;
            newGraphic.MapTip = oldGraphic.MapTip;
            newGraphic.Selected = oldGraphic.Selected;
            newGraphic.Symbol = oldGraphic.Symbol;
            newGraphic.TimeExtent = oldGraphic.TimeExtent;

            foreach (KeyValuePair<string, object> kvp in oldGraphic.Attributes)
            {
                string key = Utility.RemoveSpecialCharactersFromAlias(kvp.Key);
                newGraphic.Attributes.Add(new KeyValuePair<string,object>(key, kvp.Value));
            }

            return newGraphic;
        }

        private void Task_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            if (e == null) return;
            if (e.IdentifyResults == null) return;

            IdentifyTask identifyTask = sender as IdentifyTask;
            string service = null;
            if (identifyTask != null)
            {
                service = WebRequest.ProxyEncodeUrlAsString(identifyTask.Url, identifyTask.ProxyURL);
            }
            AddToResults(e.IdentifyResults, service);
            RemoveTask((IdentifyTask)sender);
        }

        private IdentifyParameters GetParameters(Map map, Geometry envelope, Layer layer)
        {
            IdentifyParameters idParams = GetParameters(map, envelope, layer.LayerDefinitions());

            if ((UseClientLayerVisibility) && (layer is ArcGISDynamicMapServiceLayer))
            {
                List<int> layerIDs = GetVisibleLayers(layer, map);
                idParams.LayerIds.AddRange(layerIDs);
            }

            return idParams;
        }

        private void GetVisibleSublayers(LayerItem item, List<int> layers)
        {
            foreach (var childItem in item.LayerItems)
            {
                if (childItem.IsEnabled && childItem.IsVisible)
                {
                    if (!childItem.LayerItems.Any())
                    {
                        layers.Add(childItem.SubLayerID);
                    }
                    else
                    {
                        GetVisibleSublayers(childItem, layers);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}
