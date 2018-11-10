using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Reflection;
using Alphaleonis.Win32.Filesystem;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.JsonConfiguration;
using Arbor.KVConfiguration.SystemConfiguration;
using Arbor.KVConfiguration.UserConfiguration;

namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public static class ConfigurationStartup
    {
        public static MultiSourceKeyValueConfiguration Start(ImmutableArray<Assembly> assemblies)
        {
            var siteRootDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            string settingsFile = Path.Combine(siteRootDirectory.FullName, "App_Data", "settings.json");

            AppSettingsBuilder appSettingsBuilder = KeyValueConfigurationManager.Add(new InMemoryKeyValueConfiguration(new NameValueCollection()));

            foreach (Assembly assembly in assemblies)
            {
                appSettingsBuilder = appSettingsBuilder.Add(new ReflectionKeyValueConfiguration(assembly));
            }

            MultiSourceKeyValueConfiguration keyValueConfiguration = appSettingsBuilder
                .Add(new AppSettingsKeyValueConfiguration())
                .Add(new JsonKeyValueConfiguration(settingsFile, throwWhenNotExists: false))
                .Add(new EnvironmentVariableKeyValueConfigurationSource())
                .Add(new UserConfiguration())
                .Build();

            return keyValueConfiguration;
        }
    }
}
