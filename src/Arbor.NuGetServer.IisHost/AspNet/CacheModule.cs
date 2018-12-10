using Autofac;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CacheWrapper>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
