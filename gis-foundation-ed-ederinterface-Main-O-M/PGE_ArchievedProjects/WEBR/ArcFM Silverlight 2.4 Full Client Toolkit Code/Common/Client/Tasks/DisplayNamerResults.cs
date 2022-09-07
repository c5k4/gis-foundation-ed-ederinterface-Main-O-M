using System;
using System.Collections.Generic;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    internal class DisplayNamerResults
    {
        public string LayerName { get; set; }

        public bool HasNamerObject { get; set; }

        public IDictionary<int, string> DisplayNames { get; set; }

        internal static DisplayNamerResults FromResults(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            if (jsonObject.ContainsKey("error")) return null;

            DisplayNamerResults results = new DisplayNamerResults();
            if (jsonObject.ContainsKey(Constants.LayerNameKey))
            {
                results.LayerName = (string)jsonObject[Constants.LayerNameKey];
            }
            if (jsonObject.ContainsKey("hasNamerObject"))
            {
                string boolAsStr = (string)jsonObject["hasNamerObject"];
                results.HasNamerObject = Boolean.Parse(boolAsStr);
            }
            if (jsonObject.ContainsKey("displayNames"))
            {
                IDictionary<int, string> coll = new Dictionary<int, string>();
                JsonArray array = jsonObject["displayNames"] as JsonArray;
                foreach (JsonObject jo in array)
                {
                    int objectID = -1;
                    string displayName = string.Empty;
                    if (jo.ContainsKey("OBJECTID"))
                    {
                        objectID = (int)jo["OBJECTID"];
                    }
                    if (jo.ContainsKey("displayName"))
                    {
                        displayName = (string)jo["displayName"];
                    }
                    if ((objectID != -1) && (displayName != string.Empty) && (!coll.ContainsKey(objectID)))
                    {
                        coll.Add(objectID, displayName);
                    }
                }
                results.DisplayNames = coll;
            }

            return results;
        }
    }
}
