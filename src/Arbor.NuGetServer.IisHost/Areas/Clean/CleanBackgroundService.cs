using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.KVConfiguration.Core.Extensions.IntExtensions;
using Arbor.NuGetServer.Api.Clean;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public class CleanBackgroundService : BackgroundService
    {
        private readonly CleanService _cleanService;
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly ILogger _logger;

        public CleanBackgroundService(
            IKeyValueConfiguration keyValueConfiguration,
            ILogger logger,
            CleanService cleanService)
        {
            _keyValueConfiguration = keyValueConfiguration;
            _logger = logger;
            _cleanService = cleanService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_keyValueConfiguration.ValueOrDefault(CleanConstants.AutomaticCleanEnabled, false))
            {
                _logger.Information("Clean background service is disabled");
                return;
            }

            int defaultDelayInSeconds = 120;
            TimeSpan delay = TimeSpan.FromSeconds(
                _keyValueConfiguration.ValueOrDefault(CleanConstants.AutomaticCleanIntervalInSeconds,
                    defaultDelayInSeconds));

            _logger.Information("Clean background service is enabled");

            TimeSpan initialDelay = TimeSpan.FromSeconds(20);

            await Task.Delay(initialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                bool whatIf = false;

                bool preReleaseOnly = true;

                _logger.Information("Starting automatic clean");

                CleanResult cleanResult = _cleanService.Clean(whatIf, preReleaseOnly);

                _logger.Information("Ended automatic clean with result {Result}", cleanResult);

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
