using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.WebHooks
{
    [UsedImplicitly]
    public class WebHooksModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackagePushedQueueHandler>().AsSelf().SingleInstance();
        }
    }
}
