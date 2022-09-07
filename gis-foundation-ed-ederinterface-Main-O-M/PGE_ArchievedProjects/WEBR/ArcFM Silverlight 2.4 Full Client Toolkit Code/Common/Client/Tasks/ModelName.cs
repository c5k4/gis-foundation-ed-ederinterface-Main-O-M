using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Text/Value pair for model names used to
    /// identify common features (e.g. CommonElectricFeatures)
    /// </summary>
    public class ModelName
    {
        #region public properties

        /// <summary>
        /// Gets or sets the description of the model name.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the value of the model name.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets the LayerIDs the model name is associated with.
        /// </summary>
        public IEnumerable<int> LayerIDs { get; internal set; }

        /// <summary>
        /// Gets or sets the source service url that contained the model name.
        /// </summary>
        public string Url { get; set; }

        #endregion public properties

        /// <summary>
        /// Convert ModelName text to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        internal static List<ModelName> FromResults(string json, string url)
        {
            List<ModelName> modelNames = new List<ModelName>();
            if (string.IsNullOrEmpty(json) == false)
            {
                JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;

                if (jsonObject == null)
                {
                    throw new NullReferenceException("jsonObject is null.");
                }

                if (jsonObject.ContainsKey(Constants.IdentifyModelNamesKey))
                {
                    var jsonArray = jsonObject[Constants.IdentifyModelNamesKey] as JsonArray;
                    if (jsonArray != null)
                    {
                        foreach (JsonObject jsonModelName in jsonArray)
                        {
                            if (jsonModelName.Count == 0) continue;

                            var modelName = new ModelName
                                                {
                                                    Url = url,
                                                    Text = jsonModelName[Constants.TextKey],
                                                    Value = jsonModelName[Constants.ValueKey]
                                                };

                            if (jsonModelName.ContainsKey(Constants.LayerIDsKey))
                            {
                                modelName.LayerIDs =
                                    (jsonModelName[Constants.LayerIDsKey] as JsonArray).Select(id => (int) id);
                            }
                            modelNames.Add(modelName);
                        }
                    }
                }
            }

            return modelNames;
        }
    }
}
