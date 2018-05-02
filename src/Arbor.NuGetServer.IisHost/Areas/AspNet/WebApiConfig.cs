using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
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

            var nuGetFeedConfigurations = lifetimeScope.Resolve<IEnumerable<NuGetFeedConfiguration>>();

            foreach (NuGetFeedConfiguration nuGetFeedConfiguration in nuGetFeedConfigurations)
            {
                config.UseNuGetV2WebApiFeed(nuGetFeedConfiguration.RouteName,
                    nuGetFeedConfiguration.RouteUrl,
                    nameof(NuGetFeedController).Replace(nameof(Controller), ""));
            }
        }
    }
}
