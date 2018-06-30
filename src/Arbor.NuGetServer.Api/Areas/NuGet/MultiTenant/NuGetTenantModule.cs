using Autofac;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public class NuGetTenantModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TenantRouteHelper>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InMemoryNuGetTenantReadService>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
