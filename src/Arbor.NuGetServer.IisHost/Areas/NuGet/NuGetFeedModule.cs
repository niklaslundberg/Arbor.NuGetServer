using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Autofac;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.Core.Logging;
using NuGet.Server.V2;
using ILogger = NuGet.Server.Core.Logging.ILogger;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class NuGetFeedModule : MetaModule
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly IPathMapper _pathMapper;
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly RouteHelper _routeHelper;

        public NuGetFeedModule([NotNull] IKeyValueConfiguration keyValueConfiguration, [NotNull] IPathMapper pathMapper, INuGetTenantReadService nuGetTenantReadService, RouteHelper routeHelper)
        {
            _keyValueConfiguration = keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _pathMapper = pathMapper ?? throw new ArgumentNullException(nameof(pathMapper));
            _nuGetTenantReadService = nuGetTenantReadService;
            _routeHelper = routeHelper;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var repositories = new Dictionary<NuGetTenant,IServerPackageRepository>();

                var repository = new MultiTenantRepository(repositories, _routeHelper);

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
                    new NuGetFeedConfiguration($"NuGetPrivate-{nuGetTenant.TenantId}",
                        "nuget/{tenant}",
                        nameof(NuGetFeedController).Replace(nameof(Controller), ""),
                        repository,
                        _keyValueConfiguration[ConfigurationKeys.ApiKey]);

                builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();
            }


            builder.RegisterInstance(repository).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<NuGetPackageConflictMiddleware>().SingleInstance().AsSelf();
        }
    }
}
