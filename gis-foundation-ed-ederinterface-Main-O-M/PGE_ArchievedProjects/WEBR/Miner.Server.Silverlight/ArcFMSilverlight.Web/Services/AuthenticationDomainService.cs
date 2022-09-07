using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace Miner.Server.Silverlight.Web
{
    [EnableClientAccess]
    public class AuthenticationDomainService : AuthenticationBase<User>
    {
        
    }

    public class User : UserBase
    {
        
    }
}