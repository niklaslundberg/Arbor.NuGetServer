using Autofac;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class PathModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServerMapPath>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
