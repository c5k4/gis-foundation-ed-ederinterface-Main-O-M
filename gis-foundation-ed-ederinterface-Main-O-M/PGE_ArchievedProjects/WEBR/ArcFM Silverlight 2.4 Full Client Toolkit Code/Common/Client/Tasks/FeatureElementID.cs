using System;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Element ID of a feature from a feature class.
    /// </summary>
    public class FeatureElementID
    {
        #region public properties

        /// <summary>
        /// Element id of a feature.
        /// </summary>
        public int ElementId { get; set; }

        //Object Id of a feature.
        public int ObjectId { get; set; }

        //Object (Feature) class name.
        public string ObjectClassName { get; set; }

        #endregion public properties

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FeatureElementID()
        {
            ElementId = -1;
            ObjectId = -1;
            ObjectClassName = string.Empty;
        }

        #endregion ctor

        #region internal methods

        /// <summary>
        /// Convert Json string into FeatureElementId object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>FeatureElementId</returns>
        internal static FeatureElementID FromResults(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            FeatureElementID featureElementID = null;
            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            if ((jsonObject.ContainsKey(Constants.ElementID)) &&
                (jsonObject.ContainsKey(Constants.ObjectID)) &&
                (jsonObject.ContainsKey(Constants.ObjectClassName)))
            {
                featureElementID = new FeatureElementID();
                featureElementID.ElementId = Convert.ToInt32(jsonObject[Constants.ElementID].ToString());
                featureElementID.ObjectId = Convert.ToInt32(jsonObject[Constants.ObjectID].ToString());
                featureElementID.ObjectClassName = Utility.DecodeString(jsonObject[Constants.ObjectClassName].ToString());
            }

            return featureElementID;
        }

        #endregion internal methods
    }
}
