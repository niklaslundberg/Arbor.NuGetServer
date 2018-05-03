using System.Linq;
using System.Web;
using System.Web.Http.OData.Routing;
using System.Web.Routing;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    [UsedImplicitly]
    public class RouteHelper : IRouteHelper
    {
        public string GetCurrentRouteName()
        {
            HttpContextBase httpContextWrapper = HttpContextHelper.GetCurrentContext();
            RouteData routeData = RouteTable.Routes.GetRouteData(httpContextWrapper);

            if (routeData?.Route is Route route)
            {
                ODataPathRouteConstraint oDataPathRouteConstraint =
                    route.Constraints.Values.OfType<ODataPathRouteConstraint>().SingleOrDefault();

                if (oDataPathRouteConstraint != null)
                {
                    if (!string.IsNullOrWhiteSpace(oDataPathRouteConstraint.RouteName))
                    {
                        return oDataPathRouteConstraint.RouteName;
                    }
                }
            }

            if (routeData is null)
            {
                return null;
            }

            RouteValueDictionary routeValueDictionary = routeData.DataTokens;

            if (!routeValueDictionary.TryGetValue("RouteName", out object routeName))
            {
                return null;
            }

            if (!(routeName is string routeNameValue))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(routeNameValue))
            {
                return null;
            }

            return routeNameValue;
        }
    }
}
