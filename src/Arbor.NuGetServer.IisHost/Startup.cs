using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Arbor.NuGetServer.IisHost.Startup))]
namespace Arbor.NuGetServer.IisHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
