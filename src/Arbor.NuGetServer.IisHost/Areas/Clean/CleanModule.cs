using Arbor.NuGetServer.Api.Clean;
using Arbor.NuGetServer.Core.Configuration.Modules;
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

            //builder.RegisterType<CleanBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();

            //builder.RegisterType<TestBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();

            //builder.RegisterType<TestExceptionInBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();

            //builder.RegisterType<TestOnceBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();
        }
    }
}
