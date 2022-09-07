using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Used by the LayerInformation Service to convert json into client-side subtype information
    /// </summary>
    public class SubType
    {
        /// <summary>
        /// Name of the subtype
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The ID of the subtype
        /// </summary>
        public object ID { get; private set; }

        /// <summary>
        /// The list of domains associated with this subtype
        /// </summary>
        public Dictionary<string, Domain> Domains { get; private set; }

        internal static SubType FromJson(JsonValue item, List<Field> fields)
        {
            if (item == null) return null;

            SubType subType = new SubType();
            if (item.ContainsKey("id"))
            {
                if (item["id"] == null)
                {
                    throw new NullReferenceException("id is null.");
                }

                subType.ID = item["id"].ToType();
            }
            if (item.ContainsKey("name"))
            {
                subType.Name = (string)item["name"];
            }
            if (item.ContainsKey("domains"))
            {
                subType.Domains = new Dictionary<string, Domain>();
                JsonObject domains = item["domains"] as JsonObject;

                if (domains == null)
                {
                    throw new NullReferenceException("domains is null.");
                }

                foreach (string fieldName in domains.Keys)
                {
                    Field field = (from f in fields
                                  where f.Name == fieldName
                                  select f).FirstOrDefault();
                    if (field != null)
                    {
                        JsonObject domainObject = domains[fieldName] as JsonObject;
                        if ((domainObject != null) && (domainObject.ContainsKey("type")))
                        {
                            Domain domain = null;
                            string type = (string)domainObject["type"];
                            if (type == "inherited")
                            {
                                domain = field.Domain;
                            }
                            else
                            {
                                domain = Domain.FromJsonObject(domainObject);
                            }
                            if (domain != null)
                            {
                                subType.Domains.Add(fieldName, domain);
                            }
                        }
                    }
                }
            }
            return subType;
        }

    }
}
