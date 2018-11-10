using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Abstractions
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
