using System;
using Autofac;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.Logging
{
    [UsedImplicitly]
    public class LoggingModule : Module
    {
        private readonly ILogger _logger;

        public LoggingModule([NotNull] ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_logger).SingleInstance().AsImplementedInterfaces();
        }
    }
}
