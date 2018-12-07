using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;
        private NuGetServerApp _nugetServerApp;

        public NuGetFeedModule(
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] INuGetTenantReadService nuGetTenantReadService,
            [NotNull] ITenantRouteHelper tenantRouteHelper,
            [NotNull] ILogger logger,
            ICache cache,
            ICustomClock customClock,
            NuGetServerApp nugetServerApp)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _nugetServerApp = nugetServerApp;
            _nuGetTenantReadService =
                nuGetTenantReadService ?? throw new ArgumentNullException(nameof(nuGetTenantReadService));
            _tenantRouteHelper = tenantRouteHelper ?? throw new ArgumentNullException(nameof(tenantRouteHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
            _customClock = customClock;
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
                    _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

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
                    ? new DirectoryInfo(_nugetServerApp.Functions.MapPath(tenantPackageDirectoryPath))
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

            builder.RegisterInstance(multiTenantRepository).SingleInstance().AsSelf();
            builder.RegisterInstance(repository).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<NuGetPackageConflictMiddleware>().SingleInstance().AsSelf();
            builder.RegisterType<TenantMiddleware>().SingleInstance().AsSelf();
        }
    }

    public interface ICache
    {
        bool TryGetItem(string cacheKey, out object o);

        void TryRemove(string key);

        void TryAdd<T>(string cacheKey, T item, DateTime expiresUtc) where T : class;
    }

    public class NuGetCache
    {
        private readonly ICache _cache;
        private readonly ICustomClock _customClock;

        private static readonly ConcurrentDictionary<string, DateTime?> _CacheKeys = new ConcurrentDictionary<string, DateTime?>(StringComparer.OrdinalIgnoreCase);

        public NuGetCache([NotNull] ICache cache, ICustomClock customClock)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _customClock = customClock;
        }

        public void Invalidate(string prefix)
        {
            string[] strings = _CacheKeys.Keys.ToArray().Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string key in strings)
            {
                _CacheKeys.TryRemove(key, out DateTime? _);

                _cache.TryRemove(key);
            }
        }

        public T TryGet<T>(string key, params string[] keyArgs) where T: class
        {
            string cacheKey = CacheKey(key, keyArgs);

            if (!_CacheKeys.TryGetValue(cacheKey, out DateTime? expiresUtc))
            {
                return default;
            }

            if (!expiresUtc.HasValue)
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
                _cache.TryRemove(cacheKey);
                return default;
            }

            if (_customClock.UtcNow().UtcDateTime > expiresUtc.Value)
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
                _cache.TryRemove(cacheKey);
            }

            if (_cache.TryGetItem(cacheKey, out object cached) && cached is T cachedT)
            {
                return cachedT;
            };

            return default;
        }

        private static string CacheKey(string key, string[] keyArgs)
        {
            string cacheKey = (key + (keyArgs.Length == 0 ? "" : "+" + string.Join("+", keyArgs))).ToUpperInvariant();
            return cacheKey;
        }

        public void TryAdd<T>(T item, string key, params string[] keyArgs) where T: class
        {
            string cacheKey = CacheKey(key, keyArgs);

            DateTime expiresUtc = _customClock.UtcNow().UtcDateTime.AddMinutes(10);

            if (_CacheKeys.ContainsKey(cacheKey))
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
            }

            _CacheKeys.TryAdd(cacheKey, expiresUtc);

            _cache.TryAdd(cacheKey, item, expiresUtc);
        }
    }
}
