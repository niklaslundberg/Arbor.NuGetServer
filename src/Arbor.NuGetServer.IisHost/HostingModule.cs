using Arbor.NuGetServer.Abstractions;
using Autofac;

namespace Arbor.NuGetServer.IisHost
{
    public class HostingModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AspNetHostingEnvironment>().AsImplementedInterfaces().SingleInstance();
        }
    }
}