using System.Web.WebPages;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.IisHost;

using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Arbor.NuGetServer.IisHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            if (KVConfigurationManager.AppSettings["nuget:custom-conflict-middleware:enabled"].AsBool(false))
            {
                app.Map(
                    "/api/v2",
                    config => { config.Use<NuGetInterceptMiddleware>(); });
            }
        }
    }
}