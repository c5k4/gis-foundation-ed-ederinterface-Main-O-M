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
    public class NotificationLocation
    {
        public string NotificationId { get; set; }
        public int MapLocationNum { get; set; }
        public string MapLocation { get; set; }
        public string Comments { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MapJobId { get; set; }
        public string[] FileName { get; set; }
        public string MapCorrectionType { get; set; }

    }
}
