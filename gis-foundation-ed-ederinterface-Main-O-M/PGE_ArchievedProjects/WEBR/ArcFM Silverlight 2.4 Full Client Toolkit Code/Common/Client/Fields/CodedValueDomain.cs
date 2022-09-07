using System.Collections.Generic;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Allows the conversion of json into codedvalue domains on the client
    /// </summary>
    public class CodedValueDomain : Domain
    {
        internal CodedValueDomain() { }

        /// <summary>
        /// A dictionary representing the coded values created from json
        /// </summary>
        public IDictionary<object, string> CodedValues { get; private set; }

        internal static new CodedValueDomain FromJsonObject(JsonObject json)
        {
            if (json == null) return null;

            CodedValueDomain cvDomain = new CodedValueDomain();
            if (json.ContainsKey(Constants.DomainNameKey))
            {
                cvDomain.Name = (string)json[Constants.DomainNameKey];
            }

            if (json.ContainsKey(Constants.DomainCodedValues) == false) return cvDomain;

            JsonArray domainArray = json[Constants.DomainCodedValues] as JsonArray;
            if (domainArray == null) return cvDomain;

            cvDomain.CodedValues = new Dictionary<object, string>();
            foreach (JsonObject codedValue in domainArray)
            {
                if ((codedValue.ContainsKey(Constants.DomainNameKey)) && (codedValue.ContainsKey(Constants.DomainCode)))
                {
                    string name = codedValue[Constants.DomainNameKey];
                    object value = codedValue[Constants.DomainCode].ToType();
                    cvDomain.CodedValues.Add(value, name);
                }
            }

            return cvDomain;
        }
    }
}


