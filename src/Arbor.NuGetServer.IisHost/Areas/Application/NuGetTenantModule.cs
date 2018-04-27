using Arbor.NuGetServer.IisHost.Areas.NuGet;
using Autofac;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public class NuGetTenantModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryNuGetTenantReadService>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
