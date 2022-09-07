using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROBCApp
{
    public class MultiplePCP
    {
         public int Id { get; set; }
        public string DeviceType { get; set; }
        public string OperatingNo { get; set; }
        public long CircuitId { get; set; }
        public string FeederName { get; set; }
        public string DesiredROBC { get; set; }
        public string DesiredSubBlock { get; set; }
        public string EstablishedROBC { get; set; }
        public string EstablishedSubBlock { get; set; }
        
        public MultiplePCP(int id,long circuitId,string deviceType, string operatingNo, string feederName,  string robc, string subBlock,
            string establishedRobc, string establishedSubBlock)
        {
            this.Id = id;
            this.CircuitId = circuitId;
            this.DeviceType = deviceType;
            this.OperatingNo = operatingNo;
            this.FeederName = feederName;
            this.DesiredROBC = robc;
            this.EstablishedROBC = establishedRobc;
            this.DesiredSubBlock = subBlock;
            this.EstablishedSubBlock = establishedSubBlock;
        }
    }
}
