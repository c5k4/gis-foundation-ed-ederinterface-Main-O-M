using System;
using System.Collections.Generic;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Provides information about a single layer in the map.
    /// </summary>
    public class LayerInformation
    {

        public LayerInformation()
        {
        }

        internal LayerInformation(ESRI.ArcGIS.Client.LayerInfo layerInfo)
        {
            ID = layerInfo.ID;
            Name = layerInfo.Name;
            DefaultVisibility = layerInfo.DefaultVisibility;
            SubLayerIDs = layerInfo.SubLayerIds;
        }
        #region public properties

        /// <summary>
        /// Layer ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Layer name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Parent Layer ID
        /// </summary>
        public int ParentLayerID { get; private set; }

        /// <summary>
        /// Layer visibility
        /// </summary>
        public bool DefaultVisibility { get; private set; }

        /// <summary>
        /// Sub layers' IDs.
        /// </summary>
        public int[] SubLayerIDs { get; private set; }

        /// <summary>
        /// Layer type.
        /// </summary>
        public string LayerType { get; set; }

        /// <summary>
        /// Map service where the layer is published.
        /// </summary>
        public string MapService { get; set; }

        public string ProxyUrl { get; set; }

        public IEnumerable<Field> Fields { get; private set; }

        public string DisplayField { get; private set; }

        public string SubtypeField { get; private set; }

        public bool HasDisplayNamer { get; private set; }

        public int ObjectClassID { get; private set; }

        public Dictionary<object, SubType> SubTypes { get; private set; }

        #endregion public properties

        public override string ToString()
        {
            return this.Name;
        }

        internal static LayerInformation FromResults(string json, string url, string proxyUrl = null)
        {
            if (string.IsNullOrEmpty(json)) return null;

            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            if (jsonObject == null) return null;

            if (jsonObject.ContainsKey("error")) return null;

            LayerInformation info = new LayerInformation { MapService = url, ProxyUrl = proxyUrl };
            if (jsonObject.ContainsKey("id"))
            {
                info.ID = (int)jsonObject["id"];
            }
            if (jsonObject.ContainsKey("name"))
            {
                info.Name = (string)jsonObject["name"];
            }
            if (jsonObject.ContainsKey("type"))
            {
                info.LayerType = (string)jsonObject["type"];
            }
            if (jsonObject.ContainsKey("displayField"))
            {
                info.DisplayField = (string)jsonObject["displayField"];
            }
            if (jsonObject.ContainsKey("displayNamer"))
            {
                string boolAsStr = (string)jsonObject["displayNamer"];
                info.HasDisplayNamer = Boolean.Parse(boolAsStr);
            }
            if (jsonObject.ContainsKey("objectClassID"))
            {
                info.ObjectClassID = (int)jsonObject["objectClassID"];
            }
            if (jsonObject.ContainsKey("fields"))
            {
                List<Field> fields = new List<Field>();
                info.Fields = fields;
                JsonArray fieldArray = jsonObject["fields"] as JsonArray;
                foreach (JsonObject jsonField in fieldArray)
                {
                    fields.Add(Field.FromJsonObject(jsonField));
                }
            }
            if (jsonObject.ContainsKey("typeIdField"))
            {
                info.SubtypeField = (string)jsonObject["typeIdField"];
            }
            if (jsonObject.ContainsKey("types"))
            {
                JsonArray types = jsonObject["types"] as JsonArray;
                if (types != null)
                {
                    info.SubTypes = new Dictionary<object, SubType>();
                    foreach (var item in types)
                    {
                        SubType subtype = SubType.FromJson(item, (List<Field>)info.Fields);
                        if (subtype != null)
                        {
                            info.SubTypes.Add(subtype.ID, subtype);
                        }
                    }
                }
            }
            return info;
        }

        internal static List<LayerInformation> ListFromResults(string json, string url, string proxyUrl = null)
        {
            if (string.IsNullOrEmpty(json)) return new List<LayerInformation>();

            List<LayerInformation> layers = new List<LayerInformation>();
            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            if (jsonObject.ContainsKey(Constants.LayersKey))
            {
                // ArcGIS Format
                layers = GetLayers(jsonObject[Constants.LayersKey] as JsonArray, url, proxyUrl);
            }
            else if (jsonObject.ContainsKey(Constants.IDKey))
            {
                // ArcFM Format
                layers = GetLayers(jsonObject[Constants.IDKey] as JsonArray, url, proxyUrl);
            }
            return layers;
        }

        private static List<LayerInformation> GetLayers(JsonArray jsonArray, string url, string proxyUrl = null)
        {
            List<LayerInformation> layers = new List<LayerInformation>();
            foreach (JsonObject jsonLayer in jsonArray)
            {
                LayerInformation layer = new LayerInformation();
                layer.MapService = url;
                layer.ProxyUrl = proxyUrl;

                if (jsonLayer.ContainsKey(Constants.IDKey))
                {
                    layer.ID = (int)jsonLayer[Constants.IDKey];
                }
                if (jsonLayer.ContainsKey(Constants.TypeKey))
                {
                    layer.LayerType = (string)jsonLayer[Constants.TypeKey];
                    // We don't want tables
                    if (layer.LayerType == "Table") continue;
                }
                if (jsonLayer.ContainsKey(Constants.NameKey))
                {
                    layer.Name = (string)jsonLayer[Constants.NameKey];
                }
                if (jsonLayer.ContainsKey(Constants.ParentLayerIDKey))
                {
                    layer.ParentLayerID = (int)jsonLayer[Constants.ParentLayerIDKey];
                }
                if (jsonLayer.ContainsKey(Constants.SubLayerIDsKey))
                {
                    JsonArray subLayerIdsValueJsonArray = jsonLayer[Constants.SubLayerIDsKey] as JsonArray;
                    if (subLayerIdsValueJsonArray != null)
                    {
                        List<int> subLayerIds = new List<int>();

                        foreach (JsonValue subLayerId in subLayerIdsValueJsonArray)
                        {
                            subLayerIds.Add((int)subLayerId);
                        }
                        layer.SubLayerIDs = subLayerIds.ToArray();
                    }
                }
                layers.Add(layer);
            }
            return layers;
        }
    }
}