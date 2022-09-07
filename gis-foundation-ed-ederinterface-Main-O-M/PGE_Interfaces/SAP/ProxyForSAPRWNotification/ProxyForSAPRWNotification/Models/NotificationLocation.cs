namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
   
    public class NotificationLocation
    {
        // Properties for Map location
        #region properties

        public string NotificationId { get; set; }
        public int MapLocationNum { get; set; }
        public string MapLocation { get; set; }
        public string Comments { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string MapJobId { get; set; }
        public string[] FileName { get; set; }
        public string MapCorrectionType { get; set; }

        #endregion


    }
}