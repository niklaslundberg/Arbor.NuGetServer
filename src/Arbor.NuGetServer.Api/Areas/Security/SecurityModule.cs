using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class SecurityModule :MetaModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SimpleAuthenticator>().AsSelf().SingleInstance();
            builder.RegisterType<NuGetAuthorizationMiddleware>().AsSelf().InstancePerRequest();
        }
    }
}
