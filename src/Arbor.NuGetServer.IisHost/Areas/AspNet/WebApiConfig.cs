using System;
using System.Web.Http;
using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Arbor.WebApi.Formatting.HtmlForms;
using Autofac;
using Autofac.Integration.WebApi;
using JetBrains.Annotations;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public static class WebApiConfig
    {
        public static void Register([NotNull] HttpConfiguration config, [NotNull] ILifetimeScope lifetimeScope)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (lifetimeScope == null)
            {
                throw new ArgumentNullException(nameof(lifetimeScope));
            }

            config.MapHttpAttributeRoutes();

            config.Formatters.Insert(0, new XWwwFormUrlEncodedFormatter());

            config.DependencyResolver = new AutofacWebApiDependencyResolver(lifetimeScope);

            var nuGetFeedConfiguration = lifetimeScope.Resolve<NuGetFeedConfiguration>();

            config.UseNuGetV2WebApiFeed(nuGetFeedConfiguration.RouteName,
                nuGetFeedConfiguration.RouteUrl,
                nuGetFeedConfiguration.ControllerName);
        }
    }
}
