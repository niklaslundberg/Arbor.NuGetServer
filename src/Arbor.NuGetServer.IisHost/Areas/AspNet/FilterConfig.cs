using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
