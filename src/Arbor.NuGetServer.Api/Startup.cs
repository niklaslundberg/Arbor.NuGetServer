using Arbor.NuGetServer.Api;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.NuGet.Conflicts;
using Arbor.NuGetServer.Api.Areas.Security;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using Thinktecture.IdentityModel.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Arbor.NuGetServer.Api
{
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            NuGetServerApp nuGetServerApp = ApplicationHolder.App;

            app.UseAutofacLifetimeScopeInjector(nuGetServerApp.LifetimeScope);

            app.UseMiddlewareFromContainer<TenantMiddleware>();

            ConfigureAuth(app, nuGetServerApp);

            bool conflictMiddlewareEnabled = bool.TryParse(nuGetServerApp.KeyValueConfiguration[ConfigurationKeys.ConflictMiddlewareEnabled],
                         out bool enabled) && enabled;

            if (conflictMiddlewareEnabled)
            {
                app.UseAutofacLifetimeScopeInjector(nuGetServerApp.LifetimeScope);
                app.UseMiddlewareFromContainer<NuGetPackageConflictMiddleware>();
            }
        }

        private void ConfigureAuth(IAppBuilder app, NuGetServerApp nuGetServerApp)
        {
            app.UseBasicAuthentication(
                new BasicAuthenticationOptions(
                    "Basic",
                    (username, password) =>
                    {
                        var simpleAuthenticator = nuGetServerApp.LifetimeScope.Resolve<SimpleAuthenticator>();

                        return simpleAuthenticator.IsAuthenticated(username, password);
                    }));

            app.UseStageMarker(PipelineStage.Authenticate);

            app.UseMiddlewareFromContainer<NuGetAuthorizationMiddleware>();
        }
    }
}
