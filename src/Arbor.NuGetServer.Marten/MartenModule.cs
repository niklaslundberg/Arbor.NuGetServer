using System;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using JetBrains.Annotations;
using Marten;
using Serilog;

namespace Arbor.NuGetServer.Marten
{
    [UsedImplicitly]
    public class MartenModule : MetaModule
    {
        private readonly ILogger _logger;
        private readonly MartenConfiguration _martenConfiguration;

        public MartenModule(
            [NotNull] MartenConfiguration martenConfiguration,
            [NotNull] ILogger logger)
        {
            _martenConfiguration = martenConfiguration ?? throw new ArgumentNullException(nameof(martenConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Load(ContainerBuilder builder)
        {
            _logger.Debug("Marten enabled: {Status}", _martenConfiguration.Enabled);

            if (!_martenConfiguration.Enabled)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(_martenConfiguration.ConnectionString))
            {
                builder.RegisterType<MartenDb>().AsImplementedInterfaces().SingleInstance();

                builder.Register(context => DocumentStore.For(_martenConfiguration.ConnectionString))
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
            else
            {
                _logger.Warning("Marten is enabled but no connection string is set");
            }
        }
    }
}
