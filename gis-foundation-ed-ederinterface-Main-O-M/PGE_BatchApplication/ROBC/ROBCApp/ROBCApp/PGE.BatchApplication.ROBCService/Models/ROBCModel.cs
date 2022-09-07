using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBCService
{
    public class ROBCModel:CircuitSourceModel
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedUser { get; set; }
        public string DesiredROBC { get; set; }
        public string EstablishedROBC { get; set; }
        public string DesiredSubBlock { get; set; }
        public string EstablishedSubBlock { get; set; }
        public string DesiredROBCDesc { get; set; }
        public string EstablishedROBCDesc { get; set; }
        public string DesiredSubBlockDesc { get; set; }
        public string EstablishedSubBlockDesc { get; set; }
        public string CircuitName { get; set; }
        public string ParentCircuitID { get; set; }
        public string ChildCircuitIDs { get; set; }
        
        public string GlobalId { get; set; }

        public ROBCModel() {}

        public ROBCModel(int id, string circuitId, string feederName, string scada, string robc, string subBlock,
            string establishedRobc, string establishedSubBlock, string division,string circuitName)
        {
            this.Id = id;
            this.CircuitId = circuitId;
            this.FeederName = feederName;
            this.IsScada = scada;
            this.DesiredROBC = robc;
            this.EstablishedROBC = establishedRobc;
            this.DesiredSubBlock = subBlock;
            this.EstablishedSubBlock = establishedSubBlock;
            this.Division = division;
            this.CircuitName = circuitName;
        }
    }
}
