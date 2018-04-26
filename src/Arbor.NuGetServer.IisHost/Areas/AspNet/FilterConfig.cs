using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
