using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBCService
{
    public class PCPModel
    {
        public object PcpGlobalId { get; set; }
        public object ObjectId { get; set; }
        public object DeviceGuid { get; set; }
        public object SummerProjected { get; set; }
        public object WinterProjected { get; set; }
        public object DeviceType { get; set; }
        public object OperatingNo { get; set; }
        public object CreatedDate { get; set; }
        public object CreatedUser { get; set; }
        public object ModifiedDate { get; set; }
        public object ModifiedUser { get; set; }
        public object DesiredRobc { get; set; }
        public object DesiredSubBlock { get; set; }
        public object EstablishedRobc { get; set; }
        public object EstablishedSubBlock { get; set; }
        public object DesiredRobcDesc { get; set; }
        public object DesiredSubBlockDesc { get; set; }
        public object EstablishedRobcDesc { get; set; }
        public object EstablishedSubBlockDesc { get; set; }
        public object TotalCustomer { get; set; }
        public object DeviceObjectId { get; set; }
        public object CircuitId { get; set; }
        public object PcpRobcGuid { get; set; }
        public object ParentCircuitSourceGuid { get; set; }
        public object FeederName { get; set; }
        public object DeviceSubTypeCD { get; set; }
        public object LastCheckDate { get; set; }
    }
}
