using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Time
{
    [UsedImplicitly]
    public class ClockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomSystemClock>().AsImplementedInterfaces();
        }
    }
}
