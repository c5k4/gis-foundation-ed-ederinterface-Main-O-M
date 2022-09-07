using System.Web;
using System.Web.Mvc;

namespace TLM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new TLM.Filters.GlobalExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}