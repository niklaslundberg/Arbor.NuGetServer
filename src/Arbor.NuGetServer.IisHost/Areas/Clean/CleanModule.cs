using Arbor.NuGetServer.Api.Clean;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    public class CleanModule : MetaModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CleanService>().AsSelf();
        }
    }
}
