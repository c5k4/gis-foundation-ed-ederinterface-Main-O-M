using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROBCApp
{
    public class UnassignedROBC
    {
        public int Id { get; set; }
        public decimal CircuitId { get; set; }
        public string FeederName { get; set; }
        public string SCADA { get; set; }
        public string DesiredROBC { get; set; }
        public string DesiredSubBlock { get; set; }
        public string EstablishedROBC { get; set; }
        public string EstablishedSubBlock { get; set; }
        public string Division { get; set; }

        public UnassignedROBC(int id, decimal circuitId, string feederName, string scada, string robc, string subBlock,
            string establishedRobc, string establishedSubBlock,  string division)
        {
            this.Id = id;
            this.CircuitId = circuitId;
            this.FeederName = feederName;
            this.SCADA = scada;
            this.DesiredROBC = robc;
            this.EstablishedROBC = establishedRobc;
            this.DesiredSubBlock = subBlock;
            this.EstablishedSubBlock = establishedSubBlock;
            this.Division = division;
        }
    }
}
