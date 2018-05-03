using Arbor.NuGetServer.IisHost.Areas.AspNet;
using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Autofac;

namespace Arbor.NuGetServer.IisHost
{
    public class CustomAspNetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RouteHelper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<OwinSystemWebRequestFileHelper>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
