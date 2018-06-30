using Arbor.NuGetServer.Api.Areas.NuGet.Messaging;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    [UsedImplicitly]
    public class WebHooksModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackageNotificationQueueHandler>().AsSelf().SingleInstance();
            builder.RegisterType<InMemoryKeyStore>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryKeyStore>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<TokenHelper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<WebHooksReadService>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
