using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    [UsedImplicitly]
    public class HostingModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AspNetHostingEnvironment>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
