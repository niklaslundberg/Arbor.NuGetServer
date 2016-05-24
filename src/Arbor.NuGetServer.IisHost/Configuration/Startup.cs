using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Configuration;
using Arbor.NuGetServer.IisHost.Middleware;
using Arbor.NuGetServer.IisHost.Routes;
using Arbor.NuGetServer.IisHost.Security;

using Microsoft.Owin;
using Microsoft.Owin.Extensions;

using Owin;

using Thinktecture.IdentityModel.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Arbor.NuGetServer.IisHost.Configuration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);

            if (KVConfigurationManager.AppSettings[ConfigurationKeys.ConflictMiddlewareEnabled].AsBool(false))
            {
                app.Map(
                    "/api/v2",
                    config =>
                        {
                            config.Use<NuGetWebHookMiddleware>();
                            config.Use<NuGetInterceptMiddleware>();
                        });
            }
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            IEnumerable<IProtectedRoute> protectedRoutes = DependencyResolver.Current.GetServices<IProtectedRoute>().ToArray();

            const string Key = "nuget:base-route";

            var nugetRoute =
                KVConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace(
                    $"AppSetting with key '{Key}' is not set");

            List<string> authorizedPaths = new List<string>(10)
                                               {
                                                   nugetRoute,
                                                   RouteConstants.PackageRoute
                                               };

            authorizedPaths.AddRange(protectedRoutes.Select(_ => _.Route));

            app.MapWhen(
                context =>
                authorizedPaths.Any(
                    path =>
                    context.Request.Uri.PathAndQuery.StartsWith($"/{path}", StringComparison.InvariantCultureIgnoreCase)),
                configuration =>
                    {
                        configuration.UseBasicAuthentication(
                            new BasicAuthenticationOptions(
                                "Basic",
                                (username, password) => new SimpleAuthenticator().IsAuthenticated(username, password)));

                        configuration.UseStageMarker(PipelineStage.Authenticate);
                        configuration.Use<NuGetAuthorizationMiddleware>();
                    });
        }
    }
}