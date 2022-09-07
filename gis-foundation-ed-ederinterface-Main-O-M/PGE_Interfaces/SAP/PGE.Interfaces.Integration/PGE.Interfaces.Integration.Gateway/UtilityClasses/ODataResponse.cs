using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGE.Interfaces.Integration.Gateway
{
    internal class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }

    public class OData
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public List<results> Value { get; set; }
    }
    public class results
    {
        [JsonProperty("odata.type")]
        //[JsonProperty("SapEquipId")]
        public string SapEquipId
        {
            get;
            set;
        }
        //[JsonProperty("EquipName")]
        public string EquipName
        {
            get;
            set;
        }
        //[JsonProperty("EquipType")]
        public string EquipType
        {
            get;
            set;
        }
        //[JsonProperty("GisGuid")]
        public string GisGuid
        {
            get;
            set;
        }
        //[JsonProperty("RecordId")]
        public string RecordId
        {
            get;
            set;
        }
        //[JsonProperty("BatchId")]
        public string BatchId
        {
            get;
            set;
        }
        public string RELATEDRECORDID
        {
            get;
            set;
        }

        //[JsonProperty("RequestDate")]
        public string RequestDate
        {
            get;
            set;
        }
        [JsonProperty("__metadata")]
        public metadata __metadata
        {
            get;
            set;
        }

    }
}
