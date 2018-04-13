using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.SystemConfiguration;
using Arbor.KVConfiguration.UserConfiguration;

namespace Arbor.NuGetServer.IisHost.Areas.Configuration
{
    public static class ConfigurationStartup
    {
        public static IKeyValueConfiguration Start()
        {
            IKeyValueConfiguration keyValueConfiguration = KeyValueConfigurationManager
                .Add(new AppSettingsKeyValueConfiguration())
                .Add(new UserConfiguration())
                .Build();

            return keyValueConfiguration;
        }
    }
}
