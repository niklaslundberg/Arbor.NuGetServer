using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.NuGet.Conflicts;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.Time;
using Autofac;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    [UsedImplicitly]
    public class NuGetFeedModule : AppModule
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly Serilog.ILogger _logger;
        private readonly ICache _cache;
        private readonly ICustomClock _customClock;
        private readonly Functions _functions;
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public NuGetFeedModule(
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] INuGetTenantReadService nuGetTenantReadService,
            [NotNull] ITenantRouteHelper tenantRouteHelper,
            [NotNull] ILogger logger,
            ICache cache,
            ICustomClock customClock,
            [NotNull] Functions functions)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _nuGetTenantReadService =
                nuGetTenantReadService ?? throw new ArgumentNullException(nameof(nuGetTenantReadService));
            _tenantRouteHelper = tenantRouteHelper ?? throw new ArgumentNullException(nameof(tenantRouteHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
            _customClock = customClock;
            _functions = functions ?? throw new ArgumentNullException(nameof(functions));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var repositories = new Dictionary<NuGetTenantId, IServerPackageRepository>();

            var multiTenantRepository = new MultiTenantRepository(repositories, _tenantRouteHelper);
            var repository = new ProxyRepository(multiTenantRepository, new NuGetCache(_cache, _customClock));

            var logger = new NuGetLoggerToSerilogAdapter(_logger);

            foreach (NuGetTenantConfiguration nuGetTenant in _nuGetTenantReadService.GetNuGetTenantConfigurations())
            {
                string packagePath =
                    _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPathKey];

                bool customTenantPackagePathsEnabled =
                    _keyValueConfiguration[ConfigurationKeys.CustomTenantPackagePathsEnabled]
                        .ParseAsBoolOrDefault(false);

                string fallbackPackageDirectoryPath = Path.Combine(packagePath, nuGetTenant.TenantId.TenantId);

                string tenantPackageDirectoryPath = customTenantPackagePathsEnabled
                    ? !string.IsNullOrWhiteSpace(nuGetTenant.PackageDirectory)
                        ? nuGetTenant.PackageDirectory
                        : fallbackPackageDirectoryPath
                    : fallbackPackageDirectoryPath;

                ISettingsProvider settingsProvider =
                    new KeyValueSettingsProvider(_keyValueConfiguration);

                DirectoryInfo packageDirectory = tenantPackageDirectoryPath.StartsWith("~")
                    ? new DirectoryInfo(_functions.MapPath(tenantPackageDirectoryPath))
                    : new DirectoryInfo(tenantPackageDirectoryPath);

                if (!packageDirectory.Exists)
                {
                    packageDirectory.Create();
                }

                IServerPackageRepository tenantRepository =
                    NuGetV2WebApiEnabler.CreatePackageRepository(packageDirectory.FullName, settingsProvider, logger);

                repositories.Add(nuGetTenant.TenantId, tenantRepository);

                var nuGetFeedConfiguration =
                    new NuGetFeedConfiguration(nuGetTenant.TenantId.TenantId,
                        nuGetTenant.TenantId.TenantId,
                        $"nuget/{nuGetTenant.TenantId.TenantId}",
                        repository,
                        packageDirectory.FullName);

                builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();
            }

            builder.RegisterInstance(multiTenantRepository).SingleInstance().AsSelf();
            builder.RegisterInstance(repository).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<NuGetPackageConflictMiddleware>().SingleInstance().AsSelf();
            builder.RegisterType<TenantMiddleware>().SingleInstance().AsSelf();
        }
    }
}
