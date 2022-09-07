using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace PGE.Interfaces.Integration.Gateway
{   

    //public class Metadata
    //{
    //    public string id { get; set; }
    //    public string uri { get; set; }
    //    public string type { get; set; }
    //}

    public class ED11Result
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string ProjectIdI { get; set; }
        public string ProjectIdD { get; set; }
        public string ProjectIdU { get; set; }
        public string SapEquipIdI { get; set; }
        public string SapEquipIdD { get; set; }
        public string SapEquipIdU { get; set; }
        public string GuidI { get; set; }
        public string GuidD { get; set; }
        public string GuidU { get; set; }
        public string TaskType { get; set; }
        public string PoleClass { get; set; }
        public int PoleHeight { get; set; }
        public string PoleType { get; set; }
        public string PoleCntyname { get; set; }
        public string PoleMapgpslat { get; set; }
        public string PoleMapgpslong { get; set; }
        public string PgeDivision { get; set; }
        public string PgeDistrict { get; set; }
        public string PoleAddress { get; set; }
        public string PoleCityname { get; set; }
        public string PoleZipcode { get; set; }
        public string PoleBarcode { get; set; }
        public string Mapname { get; set; }
        public string Mappingoffice { get; set; }
        public string StartupdateI { get; set; }
        public string StartupdateD { get; set; }
        public string JointPoleNbr { get; set; }
        public string AttributenameU { get; set; }
        public string OldvalueU { get; set; }
        public string NewvalueU { get; set; }
    }

    public class ED11d
    {
        [JsonProperty("results")]
        public List<ED11Result> ed11Results;
    }

    public class RootED11
    {
        [JsonProperty("d")]
        public ED11d D;
    }

}
