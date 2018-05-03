using System;
using System.Collections.Generic;
using System.IO;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Autofac;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.Core.Logging;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class NuGetFeedModule : MetaModule
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly IPathMapper _pathMapper;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public NuGetFeedModule(
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] IPathMapper pathMapper,
            INuGetTenantReadService nuGetTenantReadService,
            ITenantRouteHelper tenantRouteHelper)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _pathMapper = pathMapper ?? throw new ArgumentNullException(nameof(pathMapper));
            _nuGetTenantReadService = nuGetTenantReadService;
            _tenantRouteHelper = tenantRouteHelper;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var repositories = new Dictionary<NuGetTenant, IServerPackageRepository>();

            var repository = new MultiTenantRepository(repositories, _tenantRouteHelper);

            foreach (NuGetTenant nuGetTenant in _nuGetTenantReadService.GetNuGetTenants())
            {
                string packagePath =
                    _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

                string tenantPackageDirectoryPath = Path.Combine(packagePath, nuGetTenant.TenantId);

                ISettingsProvider settingsProvider =
                    new KeyValueSettingsProvider(_keyValueConfiguration);

                ILogger logger = new ConsoleLogger();

                DirectoryInfo packageDirectory = tenantPackageDirectoryPath.StartsWith("~")
                    ? new DirectoryInfo(_pathMapper.MapPath(tenantPackageDirectoryPath))
                    : new DirectoryInfo(tenantPackageDirectoryPath);

                IServerPackageRepository tenantRepository =
                    NuGetV2WebApiEnabler.CreatePackageRepository(packageDirectory.FullName, settingsProvider, logger);

                repositories.Add(nuGetTenant, tenantRepository);

                var nuGetFeedConfiguration =
                    new NuGetFeedConfiguration(nuGetTenant.TenantId,
                        $"nuget/{nuGetTenant.TenantId}",
                        repository,
                        _keyValueConfiguration[ConfigurationKeys.ApiKey]);

                builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();
            }

            builder.RegisterInstance(repository).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<NuGetPackageConflictMiddleware>().SingleInstance().AsSelf();
        }
    }
}
