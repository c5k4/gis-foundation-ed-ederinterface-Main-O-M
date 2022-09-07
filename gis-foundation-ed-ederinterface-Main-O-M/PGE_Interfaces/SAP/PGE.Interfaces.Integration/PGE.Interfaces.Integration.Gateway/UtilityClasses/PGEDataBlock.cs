using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGE.Interfaces.Integration.Gateway
{
    #region Not used
    //  [Serializable]
    public class ED07Data
    {
        [JsonProperty("__metadata")]
        public metadata __metadata { get; set; }
        //public ED07Record eD07Record { get; set; }

        [JsonProperty("SapEquipId")]
        //[JsonProperty("SapEquipId")]
        public string SapEquipId
        {
            get;
            set;
        }
        [JsonProperty("EquipName")]
        public string EquipName
        {
            get;
            set;
        }
        [JsonProperty("EquipType")]
        public string EquipType
        {
            get;
            set;
        }
        [JsonProperty("GisGuid")]
        public string GisGuid
        {
            get;
            set;
        }
        [JsonProperty("RecordId")]
        public string RecordId
        {
            get;
            set;
        }
        [JsonProperty("BatchId")]
        public string BatchId
        {
            get;
            set;
        }
        [JsonProperty("RelatedRecordID")]
        public string RELATEDRECORDID
        {
            get;
            set;
        }

        [JsonProperty("RequestDate")]
        public string RequestDate
        {
            get;
            set;
        }

    }
    //[Serializable]

    public class ED07Record
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
        //[JsonProperty("__metadata")]
        public metadata __metadata
        {
            get;
            set;
        }

    }

    public class metadata
    {
        [JsonProperty("id")]
        public string id
        {
            get;
            set;
        }
        [JsonProperty("uri")]
        public string uri
        {
            get;
            set;
        }
        [JsonProperty("type")]
        public string type
        {
            get;
            set;
        }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    #endregion

    #region ED07 json class
    public class Metadata
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("uri")]
        public string Uri;

        [JsonProperty("type")]
        public string Type;
    }

    public class Result
    {
        [JsonProperty("__metadata")]
        public Metadata Metadata;

        [JsonProperty("RequestDate")]
        public string RequestDate;

        [JsonProperty("RecordId")]
        public string RecordId;

        [JsonProperty("BatchId")]
        public string BatchId;

        //V3sf
        //[JsonProperty("Status")]
        //public string Status;

        //[JsonProperty("Guid")]
        //public string Guid;

        //[JsonProperty("Equnr")]
        //public string Equnr;

        //[JsonProperty("Class")]
        //public string Cls;

        //[JsonProperty("Atnam")]
        //public string Atnam;

        //[JsonProperty("Atwrt")]
        //public string Atwrt;

        //[JsonProperty("Clear")]
        //public string Clear;



        [JsonProperty("RelatedRecordID")]
        public string RelatedRecordID;

        [JsonProperty("SapEquipId")]
        public string SapEquipId;

        [JsonProperty("EquipName")]
        public string EquipName;

        [JsonProperty("EquipType")]
        public string EquipType;

        [JsonProperty("GisGuid")]
        public string GisGuid;

    }

    public class D
    {
        [JsonProperty("results")]
        public List<Result> Results;
        // public List<ED11Result> ED11Results;
    }

    public class Root
    {
        [JsonProperty("d")]
        public D D;
    }

    #endregion

}
