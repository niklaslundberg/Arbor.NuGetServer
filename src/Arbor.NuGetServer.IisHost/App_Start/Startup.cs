using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Thinktecture.IdentityModel.Owin;

namespace Arbor.NuGetServer.IisHost
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.Map("/nuget", configuration =>
            {

                configuration.UseBasicAuthentication(new BasicAuthenticationOptions("Basic",
                    (username, password) => new SimpleAuthenticator().IsAuthenticated(username, password)));


                configuration.UseStageMarker(PipelineStage.Authenticate);
                configuration.Use<NuGetAuthorizationMiddleware>();

            });
        }
    }
}