using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class SecurityModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SimpleAuthenticator>().AsSelf().SingleInstance();
            builder.RegisterType<CustomApiKeyPackageAuthenticationService>().AsSelf().SingleInstance();
            builder.RegisterType<NuGetAuthorizationMiddleware>().AsSelf().InstancePerRequest();
        }
    }
}
