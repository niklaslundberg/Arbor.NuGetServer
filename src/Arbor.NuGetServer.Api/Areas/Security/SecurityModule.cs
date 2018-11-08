using Arbor.NuGetServer.Abstractions;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class SecurityModule : MetaModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SimpleAuthenticator>().AsSelf().SingleInstance();
            builder.RegisterType<NuGetAuthorizationMiddleware>().AsSelf().InstancePerRequest();
        }
    }
}
