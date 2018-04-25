using Arbor.NuGetServer.IisHost.Areas.Application;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
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
