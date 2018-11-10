using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class HostingModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AspNetHostingEnvironment>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
