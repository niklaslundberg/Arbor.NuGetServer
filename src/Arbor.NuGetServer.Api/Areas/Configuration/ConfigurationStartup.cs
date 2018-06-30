using System;
using System.IO;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.JsonConfiguration;
using Arbor.KVConfiguration.SystemConfiguration;
using Arbor.KVConfiguration.UserConfiguration;

namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public static class ConfigurationStartup
    {
        public static IKeyValueConfiguration Start()
        {
            var siteRootDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            string settingsFile = Path.Combine(siteRootDirectory.FullName, "App_Data", "settings.json");

            IKeyValueConfiguration keyValueConfiguration = KeyValueConfigurationManager
                .Add(new AppSettingsKeyValueConfiguration())
                .Add(new JsonKeyValueConfiguration(settingsFile, throwWhenNotExists: false))
                .Add(new UserConfiguration())
                .Build();

            return keyValueConfiguration;
        }
    }
}
