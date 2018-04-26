using System.Web.WebPages;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.AspNet;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Arbor.NuGetServer.IisHost.Areas.Security;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using Thinktecture.IdentityModel.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            NuGetServerApp nuGetServerApp = MvcApplication.NuGetServerApp;

            ConfigureAuth(app, nuGetServerApp);

            if (nuGetServerApp.KeyValueConfiguration[ConfigurationKeys.ConflictMiddlewareEnabled].AsBool(false))
            {
                app.UseAutofacLifetimeScopeInjector(nuGetServerApp.LifetimeScope);
                app.UseMiddlewareFromContainer<NuGetPackageConflictMiddleware>();
            }
        }

        public void ConfigureAuth(IAppBuilder app, NuGetServerApp nuGetServerApp)
        {
            app.UseBasicAuthentication(
                new BasicAuthenticationOptions(
                    "Basic",
                    (username, password) =>
                    {
                        var simpleAuthenticator = new SimpleAuthenticator(nuGetServerApp.KeyValueConfiguration);

                        return simpleAuthenticator.IsAuthenticated(username, password);
                    }));

            app.UseStageMarker(PipelineStage.Authenticate);

            app.Use<NuGetAuthorizationMiddleware>();
        }
    }
}
