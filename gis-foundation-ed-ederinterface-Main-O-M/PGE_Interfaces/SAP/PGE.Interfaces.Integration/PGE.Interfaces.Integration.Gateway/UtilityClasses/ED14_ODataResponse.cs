using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
// Sample: https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED14Set?$filter=RequestDate eq '20210420' and BatchId eq ''&$format=json 

namespace PGE.Interfaces.Integration.Gateway
{

    public class ED14Result
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string Qmnum { get; set; }
        public string Qmart { get; set; }
        public string Status { get; set; }
        public string Qmtxt { get; set; }
        public string Equnr { get; set; }
        public string Gisguid { get; set; }
        public string Eqart { get; set; }
        public string Strno { get; set; }
        public string Qmgrp { get; set; }
        public string Qmcod { get; set; }
        public string Erdat { get; set; }
        public string Qmdat { get; set; }
        public string Qmdab { get; set; }
        public string Ltrmn { get; set; }
        public string Priok { get; set; }
        public string Aufnr { get; set; }
        public string Ilart { get; set; }
        public string Itemtxt { get; set; }
        public string Probtxt { get; set; }
        public string Causetxt { get; set; }
        public string Activity { get; set; }
    }

    public class ED14d
    {
        [JsonProperty("results")]
        public List<ED14Result> ed14Results;
    }

    public class RootED14
    {
        [JsonProperty("d")]
        public ED14d D;
    }
}
