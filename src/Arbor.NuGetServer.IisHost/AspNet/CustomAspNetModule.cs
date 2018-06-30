using Autofac;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class CustomAspNetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RouteHelper>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<OwinSystemWebRequestFileHelper>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
