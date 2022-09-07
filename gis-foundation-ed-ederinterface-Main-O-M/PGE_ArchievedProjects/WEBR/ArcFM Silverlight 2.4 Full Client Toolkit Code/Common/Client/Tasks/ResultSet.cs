using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Json;
using System.Linq;
using System.ComponentModel;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Collection of feature results.
    /// </summary>
    public class ResultSet : FeatureSet, IResultSet, INotifyPropertyChanged
    {
        ObservableCollection<Graphic> _graphics;

        public ResultSet()
        {
            ID = -1;
            Features = new ObservableCollection<Graphic>();
            RelationshipService = new RelationshipService();
        }

        public ResultSet(FeatureSet set)
            : this()
        {
            if (set == null) throw new ArgumentNullException("set");

            if (set.FieldAliases != null)
            {
                foreach (var field in set.FieldAliases)
                {
                    FieldAliases.Add(field.Key, field.Value);
                }
            }
            int index = 1;
            foreach (var feature in set.Features)
            {
                GraphicPlus graphic = new GraphicPlus(feature) { FieldAliases = this.FieldAliases, DisplayFieldName = this.DisplayFieldName };
                graphic.Attributes["RowIndex"] = index++;
                this.Features.Add(graphic);
            }

            this.GeometryType = set.GeometryType;
            this.SpatialReference = set.SpatialReference;
            this.DisplayFieldName = set.DisplayFieldName;
        }

        #region IResultSet Members

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the features.
        /// </summary>
        public new ObservableCollection<Graphic> Features
        {
            get
            {
                return _graphics as ObservableCollection<Graphic>;
            }

            private set
            {
                if (_graphics != value)
                {
                    _graphics = value;
                    OnNotifyPropertyChanged("Features");
                }
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the maximum number of records was exceded.
        /// </summary>
        public bool ExceededThreshold { get; set; }

        /// <summary>
        /// Gets or sets the type of the geometry.
        /// </summary>
        public new GeometryType GeometryType { get; internal set; }

        /// <summary>
        /// Gets or sets the service URL the results are from.
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the service to retrieve relationships.
        /// </summary>
        public IRelationshipService RelationshipService { get; set; }

        #endregion IResultSet Members

        #region Internal Methods

        internal static List<IResultSet> FromResults(string json, string service = null, string proxyUrl = null, string searchTitle = null)
        {
            List<IResultSet> resultSets = new List<IResultSet>();

            try
            {
                JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
                if (jsonObject.ContainsKey(Constants.ResultsKey))
                {
                    JsonArray jsonArray = jsonObject[Constants.ResultsKey] as JsonArray;
                    resultSets = ConvertJsonToResults(jsonArray, service, proxyUrl, searchTitle);
                }
            }
            catch
            {
                //resultSets = null;
            }

            return resultSets;
        }

        internal static IDictionary<int, IEnumerable<IResultSet>> FromResults(IDictionary<int, RelationshipResult> relationshipResults, string service = null, bool correlate = false)
        {
            if (relationshipResults == null) return null;

            IDictionary<int, IEnumerable<IResultSet>> resultSetsByObjectId = new Dictionary<int, IEnumerable<IResultSet>>();
            if (relationshipResults.Count <= 0) return resultSetsByObjectId;

            try
            {
                foreach (var relationshipResultByLayerId in relationshipResults)
                {
                    RelationshipResult relationshipResult = relationshipResultByLayerId.Value;
                    if (relationshipResult.RelatedRecordsGroup.Count == 0) continue;

                    int layerId = relationshipResultByLayerId.Key;

                    foreach (var graphicsByObjectId in relationshipResult.RelatedRecordsGroup)
                    {
                        int objectId = graphicsByObjectId.Key;

                        List<IResultSet> results;
                        if (resultSetsByObjectId.ContainsKey(objectId))
                        {
                            results = resultSetsByObjectId[objectId] as List<IResultSet>;
                        }
                        else
                        {
                            results = new List<IResultSet>();
                            resultSetsByObjectId[graphicsByObjectId.Key] = results;
                        }

                        ResultSet resultSet = new ResultSet();
                        resultSet.Service = service;
                        resultSet.ID = layerId;
                        results.Add(resultSet);

                        foreach (var field in relationshipResult.Fields)
                        {
                            resultSet.FieldAliases.Add(field.Name, field.Alias);
                        }

                        foreach (Graphic graphic in graphicsByObjectId.Value)
                        {
                            GraphicPlus graphicPlus = new GraphicPlus(graphic);
                            resultSet.Features.Add(graphicPlus);
                        }

                    }
                }
            }
            catch
            {
                //resultSets = null;
            }

            if (correlate) resultSetsByObjectId = CorrelateResults(resultSetsByObjectId);

            return resultSetsByObjectId;
        }

        /// <summary>
        /// Remove intermidiate relationship results and add final related items to initial object ID result set.
        /// </summary>
        /// <param name="resultSetsByObjectId">A dictionary of results keyed by the object id to which they are related.</param>
        /// <returns>A dictionary of results keyed by the object id to which they are related that doesn't contain intermidiate relationships.</returns>
        private static Dictionary<int, IEnumerable<IResultSet>> CorrelateResults(IDictionary<int, IEnumerable<IResultSet>> resultSetsByObjectId)
        {
            if (resultSetsByObjectId == null) return null;

            var addList = new Dictionary<int, IEnumerable<IResultSet>>();
            var ignoreList = new List<int>();

            foreach (var kvp in resultSetsByObjectId)
            {
                foreach (var resultSet in kvp.Value)
                {
                    foreach (var feature in resultSet.Features)
                    {
                        if ((feature.Attributes == null) || !feature.Attributes.ContainsKey("OBJECTID")) continue;

                        var objectId = Convert.ToInt32(feature.Attributes["OBJECTID"]);

                        if ((!resultSetsByObjectId.ContainsKey(objectId)) || (objectId == kvp.Key)) continue;

                        ignoreList.Add(objectId);

                        if (addList.ContainsKey(kvp.Key))
                        {
                            var list = (List<IResultSet>)addList[kvp.Key];

                            list.AddRange(resultSetsByObjectId[objectId]);

                            addList[kvp.Key] = list;
                        }
                        else
                        {
                            addList.Add(kvp.Key, resultSetsByObjectId[objectId]);
                        }
                    }
                }
            }

            // Here is what is happening below in an easier to read format.
            //var compressedResults = new Dictionary<int, IEnumerable<IResultSet>>();

            //foreach (var kvp in resultSetsByObjectId)
            //{
            //    if (ignoreList.Contains(kvp.Key)) continue;
            //    if (addList.ContainsKey(kvp.Key))
            //    {
            //        compressedResults.Add(kvp.Key, addList[kvp.Key]);
            //    } else
            //    {
            //        compressedResults.Add(kvp.Key, kvp.Value);
            //    }
            //}

            var compressedResults = resultSetsByObjectId.Where(kvp => !ignoreList.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => addList.ContainsKey(kvp.Key) ? addList[kvp.Key] : kvp.Value);

            return compressedResults;
        }

        #endregion Internal Methods

        #region private methods

        private static List<IResultSet> ConvertJsonToResults(JsonArray jsonArray, string service = null, string proxyUrl = null, string searchTitle = null)
        {
            string url = WebRequest.ProxyEncodeUrlAsString(service, proxyUrl);

            List<IResultSet> resultSets = new List<IResultSet>();
            if (jsonArray != null)
            {
                foreach (JsonObject jsonLayer in jsonArray)
                {
                    FeatureSet featureSet = FeatureSet.FromJson(jsonLayer.ToString());
                    ResultSet resultSet = new ResultSet
                    {
                        Service = url,
                        GeometryType = featureSet.GeometryType,
                        ObjectIdFieldName = featureSet.ObjectIdFieldName,
                        GlobalIdFieldName = featureSet.GlobalIdFieldName,
                        DisplayFieldName = featureSet.DisplayFieldName,
                        SpatialReference = featureSet.SpatialReference,
                    };
                    foreach (var field in featureSet.FieldAliases)
                    {
                        resultSet.FieldAliases.Add(field.Key, field.Value);
                    }

                    int featureIndex = 1;
                    foreach (Graphic graphic in featureSet.Features)
                    {
                        //INC000004150619
                        if (searchTitle == "SAP Equipment ID Search" && (string)jsonLayer[Constants.LayerNameKey] == "Enclosure" && graphic.Attributes["SUBTYPECD"].ToString() != "5")
                            continue;
                        GraphicPlus graphicPlus = new GraphicPlus(graphic) { FieldAliases = resultSet.FieldAliases, DisplayFieldName = resultSet.DisplayFieldName };
                        graphicPlus.Attributes["RowIndex"] = featureIndex++;
                        resultSet.Features.Add(graphicPlus);
                    }

                    if (jsonLayer.ContainsKey(Constants.LayerNameKey))
                    {
                        resultSet.Name = (string)jsonLayer[Constants.LayerNameKey];
                    }
                    if (jsonLayer.ContainsKey(Constants.LayerIdKey))
                    {
                        resultSet.ID = Convert.ToInt32(jsonLayer[Constants.LayerIdKey].ToString());
                    }
                    if (jsonLayer.ContainsKey(Constants.ExceededThresholdKey))
                    {
                        resultSet.ExceededThreshold = (bool)jsonLayer[Constants.ExceededThresholdKey];
                    }

                    resultSets.Add(resultSet);
                }
            }
            return resultSets;
        }

        #endregion private methods

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Notifies the listeners when a property changed
        /// </summary>
        /// <param name="p"></param>
        protected void OnNotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        /// <summary>
        /// Fires when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}
