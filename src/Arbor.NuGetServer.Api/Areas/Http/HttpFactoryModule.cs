using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Http
{
    [UsedImplicitly]
    public class HttpFactoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpClientFactory>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
