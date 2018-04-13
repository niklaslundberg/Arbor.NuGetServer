using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Autofac.Integration.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public static class MvcStartup
    {
        public static void Start([NotNull] NuGetServerApp app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            DependencyResolver.SetResolver(new AutofacDependencyResolver(app.Container));

            GlobalConfiguration.Configure(configuration =>
                WebApiConfig.Register(configuration, app.Container));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
