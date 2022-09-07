using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{

    /// <summary>
    /// Allows the conversion of json into client-side domain objects
    /// </summary>
    public class Domain
    {
        internal Domain() { }

        /// <summary>
        /// The name of the domain from json
        /// </summary>
        public string Name { get; internal set; }

        internal static Domain FromJsonObject(JsonObject json)
        {
            if (json == null) return null;
            if (json.ContainsKey(Constants.DomainTypeKey) == false) return null;

            switch((string)json[Constants.DomainTypeKey])
            {
                case Constants.DomainTypeValueCV:
                    return CodedValueDomain.FromJsonObject(json);
                default:
                    return null;
            }
        }
    }
}
