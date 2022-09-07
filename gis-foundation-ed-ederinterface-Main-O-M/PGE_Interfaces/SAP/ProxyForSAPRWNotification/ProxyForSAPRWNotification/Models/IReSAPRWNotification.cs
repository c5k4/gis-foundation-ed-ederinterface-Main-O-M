using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
    internal interface IReSaprwNotification
    {
        bool Reprocess(string notificatioid);
    }
}
