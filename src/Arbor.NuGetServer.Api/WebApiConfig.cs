using System;
using System.Collections.Generic;
using System.Web.Http;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using Arbor.WebApi.Formatting.HtmlForms;
using Autofac;
using Autofac.Integration.WebApi;
using JetBrains.Annotations;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.Api
{
    public static class WebApiConfig
    {
        public const string Controller = nameof(Controller);

        public static void Register(
            [NotNull] HttpConfiguration config,
            [NotNull] ILifetimeScope lifetimeScope)
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
                    nameof(NuGetFeedController).Replace(Controller, ""));
            }
        }
    }
}
