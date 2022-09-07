using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ArcFMSilverlight
{
    public class NotificationDetail
    {
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

    }
}
