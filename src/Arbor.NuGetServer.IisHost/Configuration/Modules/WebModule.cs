using Arbor.NuGetServer.Core.Configuration.Modules;

using Autofac;

using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Configuration.Modules
{
    [UsedImplicitly]
    public class WebModule : MetaModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServerMapPath>().AsImplementedInterfaces();
        }
    }
}