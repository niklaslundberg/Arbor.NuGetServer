using Autofac;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.Logging
{
    [UsedImplicitly]
    public class LoggingModule : Module
    {
        private readonly ILogger _logger;

        public LoggingModule(ILogger logger)
        {
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_logger).SingleInstance().AsImplementedInterfaces();
        }
    }
}
