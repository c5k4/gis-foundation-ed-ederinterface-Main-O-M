using System.Net.Http;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
    internal interface ISaprwNotificationDetail
    {
        bool ProcessSaprwNotification(SaprwNotification item);
    }
}