using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGE.Interfaces.Integration.Gateway
{
    public class ED13AResult
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string Notification { get; set; }
        public string Notificationtype { get; set; }
        public string Notifdescription { get; set; }
        public string Worktype { get; set; }
        public string Pmordernumber { get; set; }
        public string Ordertype { get; set; }
        public string Ordercreatedate { get; set; }
        public string Objectclass { get; set; }
        public string Sapequipmentid { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Pin { get; set; }
        public string Ois { get; set; }
        public string Ssd { get; set; }
        public string Circuit { get; set; }
        public string Platmap { get; set; }
        public string Street { get; set; }
        public string Jobowner { get; set; }
        public string Filecreationdt { get; set; }
    }

    public class ED13Ad
    {
        [JsonProperty("results")]
        public List<ED13AResult> ed13AResults;
    }

    public class RootED13A
    {
        [JsonProperty("d")]
        public ED13Ad D;
    }
}
