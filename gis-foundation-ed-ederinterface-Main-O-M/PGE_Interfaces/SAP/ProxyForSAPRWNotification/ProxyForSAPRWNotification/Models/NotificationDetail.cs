using System;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{

    public class NotificationDetail
    {
        // properties for Header
        #region Properties
        public string NotificationId { get; set; }
        public string MapNumber { get; set; }
        public string SubStation { get; set; }
        public string CircuitId { get; set; }
        public DateTime SubmitDate { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Lanid { get; set; }
        public string ConstructionType { get; set; }
        public string CorrectionType { get; set; }
        public int Status { get; set; }
        public int NumberOfCorrection { get; set; }
        public string FunctionalLocation { get; set; }
        public string MainWorkCenter { get; set; }
        public string DataCenter { get; set; }

        #endregion
    }



}
