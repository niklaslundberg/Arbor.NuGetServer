using System.IO;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.IisHost.Controllers;
using Autofac;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.Core.Logging;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.IisHost.Configuration
{
    [UsedImplicitly]
    public class NuGetFeedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            string packagePath =
                StaticKeyValueConfigurationManager.AppSettings[ConfigurationKeys.PackagesDirectoryPath];

            ISettingsProvider settingsProvider =
                new KeyValueSettingsProvider(StaticKeyValueConfigurationManager.AppSettings);
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
                    StaticKeyValueConfigurationManager.AppSettings[ConfigurationKeys.ApiKey]);

            builder.RegisterInstance(nuGetFeedConfiguration).AsSelf();
        }
    }
}