using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGE.Interfaces.Integration.Gateway
{
    public class ED13Result
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string Aufnr { get; set; }
        public string Auart { get; set; }
        public string Erdat { get; set; }
        public string Ktext { get; set; }
        public string Vaplz { get; set; }
        public string Parnr { get; set; }
        public string Strno { get; set; }
        public string Equnr { get; set; }
        public string Ilart { get; set; }
        public string Zzworktype { get; set; }
        public string EPlat { get; set; }
        public string Region { get; set; }
        public string Zzdivision { get; set; }
        public string Txt04 { get; set; }
        public string Zzestimator1 { get; set; }
        public string Zzestimator2 { get; set; }
    }

    public class ED13d
    {
        [JsonProperty("results")]
        public List<ED13Result> ed13Results;
    }

    public class RootED13
    {
        [JsonProperty("d")]
        public ED13d D;
    }
}
