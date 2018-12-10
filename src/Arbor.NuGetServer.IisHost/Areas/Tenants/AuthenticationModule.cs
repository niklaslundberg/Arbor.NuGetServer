using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class AuthenticationModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomAuthenticationService>().SingleInstance();
        }
    }
}