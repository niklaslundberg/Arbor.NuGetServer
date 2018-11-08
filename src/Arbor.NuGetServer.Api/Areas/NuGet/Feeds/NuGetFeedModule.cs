using System;
using System.Collections.Generic;
using System.IO;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.NuGet.Conflicts;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Core;
using Autofac;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    [UsedImplicitly]
    public class NuGetFeedModule : AppModule
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly Serilog.ILogger _logger;
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly IPathMapper _pathMapper;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public NuGetFeedModule(
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] IPathMapper pathMapper,
            [NotNull] INuGetTenantReadService nuGetTenantReadService,
            [NotNull] ITenantRouteHelper tenantRouteHelper,
            [NotNull] Serilog.ILogger logger)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _pathMapper = pathMapper ?? throw new ArgumentNullException(nameof(pathMapper));
            _nuGetTenantReadService =
                nuGetTenantReadService ?? throw new ArgumentNullException(nameof(nuGetTenantReadService));
            _tenantRouteHelper = tenantRouteHelper ?? throw new ArgumentNullException(nameof(tenantRouteHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var repositories = new Dictionary<NuGetTenantId, IServerPackageRepository>();

            var repository = new MultiTenantRepository(repositories, _tenantRouteHelper);

            var logger = new SerilogAdapter(_logger);

            foreach (NuGetTenantConfiguration nuGetTenant in _nuGetTenantReadService.GetNuGetTenantConfigurations())
            {
                string packagePath =
                    _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

                string tenantPackageDirectoryPath = !string.IsNullOrWhiteSpace(nuGetTenant.PackageDirectory)
                    ? nuGetTenant.PackageDirectory
                    : Path.Combine(packagePath, nuGetTenant.TenantId.TenantId);

                ISettingsProvider settingsProvider =
                    new KeyValueSettingsProvider(_keyValueConfiguration);

                DirectoryInfo packageDirectory = tenantPackageDirectoryPath.StartsWith("~")
                    ? new DirectoryInfo(_pathMapper.MapPath(tenantPackageDirectoryPath))
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
                        _keyValueConfiguration[ConfigurationKeys.ApiKey],
                        packageDirectory.FullName);

                builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();
            }

            builder.RegisterInstance(repository).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<NuGetPackageConflictMiddleware>().SingleInstance().AsSelf();
            builder.RegisterType<TenantMiddleware>().SingleInstance().AsSelf();
        }
    }
}
