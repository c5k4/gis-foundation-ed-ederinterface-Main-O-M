using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
// Sample: https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED15_INDVSet?$filter=RequestDate eq '20210420' and BatchId eq ''&$format=json

namespace PGE.Interfaces.Integration.Gateway
{
    public class ED15INDVResult
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string Action { get; set; }
        public string Qmnum { get; set; }
        public string ProjName { get; set; }
        public string QueueNum { get; set; }
        public string EgiEqunr { get; set; }
        public string GenDesign { get; set; }
        public string GenTech { get; set; }
        public string FuelType { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string PtcRating { get; set; }
        public string InvEfficiency { get; set; }
        public string NpRatingKw { get; set; }
        public string Quantity { get; set; }
        public string NpCapcKw { get; set; }
        public string EgiPf { get; set; }
        public string EffRatingKw { get; set; }
        public string EffRatingKva { get; set; }
        public string KvoltsRating { get; set; }
        public string Certification { get; set; }
        public string Phase { get; set; }
        public string InvUsgMode { get; set; }
        public string InvEqunr { get; set; }
        public string AesMaxCap { get; set; }
        public string AesRtechrgDmnd { get; set; }
        public string AesGridCharge { get; set; }
        public string PosResist { get; set; }
        public string PosReact { get; set; }
        public string PosResistPrm { get; set; }
        public string PosReactPrm { get; set; }
        public string PosResistDblPrm { get; set; }
        public string PosReactDblPrm { get; set; }
        public string NegResist { get; set; }
        public string NegReact { get; set; }
        public string ZeroResist { get; set; }
        public string ZeroReact { get; set; }
        public string GrndResist { get; set; }
        public string GrndReact { get; set; }
        public string PtoDate { get; set; }
        public string ProjType { get; set; }
        public string EnosEqref { get; set; }
        public string EnosRef { get; set; }
        public string FsServPt { get; set; }
        public string FsSpatialGuid { get; set; }
        public string  BackupGen { get; set; }
        public string BkupgenAct { get; set; }
        public string TransType { get; set; }
        public string ParaPeriod { get; set; }
        public string  StandbyGen { get; set; }
        public string Derated { get; set; }
        public string StrOperMode { get; set; }
        public string GridExport { get; set; }
        public string NexpScheme { get; set; }
        public string DeratedVal { get; set; }
        public string NpDeratedKw { get; set; }
        public string AesEstannKwh { get; set; }
    }

    public class ED15indv
    {
        [JsonProperty("results")]
        public List<ED15INDVResult> ed15INDVResults;
    }

    public class RootED15INDV
    {
        [JsonProperty("d")]
        public ED15indv D;
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Sample: https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED15_SUMMSet?$filter=RequestDate eq '20210420' and BatchId eq ''&$format=json
    public class ED15SummaryResult
    {
        public Metadata __metadata { get; set; }
        public string RequestDate { get; set; }
        public string RecordId { get; set; }
        public string BatchId { get; set; }
        public string Action { get; set; }
        public string Qmnum { get; set; }
        public string ProjName { get; set; }
        public string GenTech { get; set; }
        public string TotEffMachrtKw { get; set; }
        public string TotEffInvrtKw { get; set; }
        public string TotEffMachrtKva { get; set; }
        public string TotEffInvrtKva { get; set; }
        public string AesMaxCap { get; set; }
        public string AesRtechrgDmnd { get; set; }
        public string ProjType { get; set; }
        public string FsServPt { get; set; }
        public string FsSpatialGuid { get; set; }
        public string LimitedExport { get; set; }
        public string TotNpDeratedKw { get; set; }
        public string LimitedBy { get; set; }
        public string OperatingMode { get; set; }
        public string Telemetry { get; set; }
        public string TelmCommTyp { get; set; }
        public string TelmCntrEnb { get; set; }
        public string TelmDerprg_typ { get; set; }
    }

    public class ED15Summary
    {
        [JsonProperty("results")]
        public List<ED15SummaryResult> ed15INDVResults;
    }

    public class RootED15Summary
    {
        [JsonProperty("d")]
        public ED15Summary D;
    }

}
