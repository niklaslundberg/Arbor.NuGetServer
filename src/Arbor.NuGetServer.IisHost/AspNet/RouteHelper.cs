using System.Linq;
using System.Web;
using System.Web.Http.OData.Routing;
using System.Web.Routing;
using Arbor.NuGetServer.Api;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    [UsedImplicitly]
    public class RouteHelper : IRouteHelper
    {
        public string GetCurrentRouteName()
        {
            HttpContextBase httpContextWrapper = HttpContextHelper.GetCurrentContext();

            string currentRouteName = httpContextWrapper.Request.Url.TenantId();

            if (!string.IsNullOrWhiteSpace(currentRouteName))
            {
                return currentRouteName;
            }

            return currentRouteName;
        }
    }
}
