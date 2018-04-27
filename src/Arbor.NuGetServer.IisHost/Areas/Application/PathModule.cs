using Arbor.NuGetServer.IisHost.Areas.AspNet;
using Autofac;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public class PathModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServerMapPath>().SingleInstance().AsImplementedInterfaces();
        }
    }
}