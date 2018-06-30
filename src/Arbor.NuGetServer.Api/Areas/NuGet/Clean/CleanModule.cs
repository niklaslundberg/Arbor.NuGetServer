using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Clean;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Clean
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
