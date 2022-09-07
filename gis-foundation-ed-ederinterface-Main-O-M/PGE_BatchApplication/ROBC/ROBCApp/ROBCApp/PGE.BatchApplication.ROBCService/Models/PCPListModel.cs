using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBCService
{
    public class PCPListModel
    {
        public string  ObjectId { get; set; }
        public string GlobalId { get; set; }
        public string CircuitId { get; set; }
        public string OperatingNo { get; set; }
        //public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        //public string FeederName { get; set; }
    }
}
