using System.IO;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
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

        public NuGetFeedModule(IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            string packagePath =
                _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

            ISettingsProvider settingsProvider =
                new KeyValueSettingsProvider(_keyValueConfiguration);
            ILogger logger = new ConsoleLogger();
            DirectoryInfo packageDirectory = packagePath.StartsWith("~")
                ? new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath(packagePath))
                : new DirectoryInfo(packagePath);
            IServerPackageRepository repository =
                NuGetV2WebApiEnabler.CreatePackageRepository(packageDirectory.FullName, settingsProvider, logger);
            var nuGetFeedConfiguration =
                new NuGetFeedConfiguration("NuGetPrivate",
                    "nuget",
                    nameof(NuGetFeedController).Replace(nameof(Controller), ""),
                    repository,
                    _keyValueConfiguration[ConfigurationKeys.ApiKey]);

            builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();

            builder.RegisterType<NuGetInterceptMiddleware>().SingleInstance().AsSelf();
        }
    }
}
