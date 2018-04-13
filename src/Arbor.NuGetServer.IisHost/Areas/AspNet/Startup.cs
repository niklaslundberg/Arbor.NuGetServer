using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.AspNet;
using Arbor.NuGetServer.IisHost.Areas.Clean;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Arbor.NuGetServer.IisHost.Areas.Security;
using Arbor.NuGetServer.IisHost.Areas.WebHooks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using Thinktecture.IdentityModel.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            NuGetServerApp nuGetServerApp = MvcApplication.NuGetServerApp;

            ConfigureAuth(app, nuGetServerApp);

            if (nuGetServerApp.KeyValueConfiguration[ConfigurationKeys.ConflictMiddlewareEnabled].AsBool(false))
            {
                app.UseAutofacLifetimeScopeInjector(nuGetServerApp.Container);
                //app.UseMiddlewareFromContainer<NuGetWebHookMiddleware>();
                app.UseMiddlewareFromContainer<NuGetInterceptMiddleware>();
            }
        }

        public void ConfigureAuth(IAppBuilder app, NuGetServerApp nuGetServerApp)
        {
            IEnumerable<IProtectedRoute> protectedRoutes = DependencyResolver.Current.GetServices<IProtectedRoute>().ToArray();

            const string Key = "nuget:base-route";

            var nugetRoute =
                nuGetServerApp.KeyValueConfiguration[Key].ThrowIfNullOrWhitespace(
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
                                (username, password) => new SimpleAuthenticator(nuGetServerApp.KeyValueConfiguration).IsAuthenticated(username, password)));

                        configuration.UseStageMarker(PipelineStage.Authenticate);
                        configuration.Use<NuGetAuthorizationMiddleware>();
                    });
        }
    }
}
