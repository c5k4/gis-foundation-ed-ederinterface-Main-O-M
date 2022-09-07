using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
    public class SaprwNotification
    {
        public NotificationDetail Notificationdetails { get; set; }
        public List<NotificationLocation> NotificationLocations { get; set; }

    }
}