using Arbor.NuGetServer.Core.Configuration.Modules;

using Autofac;

using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Clean
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