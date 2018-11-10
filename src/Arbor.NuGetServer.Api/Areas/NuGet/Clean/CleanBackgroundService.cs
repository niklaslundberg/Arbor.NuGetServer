using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.KVConfiguration.Core.Extensions.IntExtensions;
using Arbor.NuGetServer.Api.Areas.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Clean
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public class CleanBackgroundService : BackgroundService
    {
        private readonly CleanService _cleanService;
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly ILogger _logger;
        private readonly INuGetTenantReadService _tenantReadService;

        public CleanBackgroundService(
            IKeyValueConfiguration keyValueConfiguration,
            ILogger logger,
            CleanService cleanService,
            INuGetTenantReadService tenantReadService)
        {
            _keyValueConfiguration = keyValueConfiguration;
            _logger = logger;
            _cleanService = cleanService;
            _tenantReadService = tenantReadService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_keyValueConfiguration.ValueOrDefault(CleanConstants.AutomaticCleanEnabled, false))
            {
                _logger.Information("Clean background service is disabled");
                return;
            }

            const int defaultDelayInSeconds = 120;

            TimeSpan delay = TimeSpan.FromSeconds(
                _keyValueConfiguration.ValueOrDefault(CleanConstants.AutomaticCleanIntervalInSeconds,
                    defaultDelayInSeconds));

            _logger.Information("Clean background service is enabled");

            TimeSpan initialDelay = TimeSpan.FromSeconds(20);

            await Task.Delay(initialDelay, stoppingToken);

            const bool whatIf = false;

            const bool preReleaseOnly = true;
            ImmutableArray<NuGetTenantId> nuGetTenantIds = _tenantReadService.GetNuGetTenantIds();

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (NuGetTenantId nuGetTenantId in nuGetTenantIds)
                {
                    _logger.Information(
                        "Starting automatic clean for tenant {Tenant}",
                        nuGetTenantId);

                    CleanResult cleanResult = await _cleanService.CleanAsync(
                        nuGetTenantId,
                        whatIf,
                        preReleaseOnly,
                        cancellationToken: stoppingToken);

                    _logger.Information(
                        "Ended automatic clean with result {Result} for tenant {Tenant}",
                        cleanResult,
                        nuGetTenantId);
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
