using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Configuration
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
