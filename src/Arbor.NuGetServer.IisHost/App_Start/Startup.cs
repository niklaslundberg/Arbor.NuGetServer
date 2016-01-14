using System;
using System.Collections.Generic;
using System.Linq;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.IisHost.Extensions;
using Arbor.NuGetServer.IisHost.Security;

using Microsoft.Owin.Extensions;

using Owin;

using Thinktecture.IdentityModel.Owin;

namespace Arbor.NuGetServer.IisHost
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            const string Key = "nuget:base-route";

            var nugetRoute =
                KVConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace(
                    $"AppSetting with key '{Key}' is not set");

            List<string> authorizedPaths = new List<string>(10) { nugetRoute, "packages" };

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
