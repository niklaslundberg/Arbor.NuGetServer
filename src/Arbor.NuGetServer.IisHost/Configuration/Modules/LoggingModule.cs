using Arbor.NuGetServer.Core.Configuration.Modules;
using Arbor.NuGetServer.Core.Logging;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Configuration.Modules
{
    [UsedImplicitly]
    public class LoggingModule : MetaModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Logger.LoggerInstance).SingleInstance().AsImplementedInterfaces();
        }
    }
}