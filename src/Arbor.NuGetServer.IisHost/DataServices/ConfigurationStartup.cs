using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.SystemConfiguration;
using Arbor.KVConfiguration.UserConfiguration;

namespace Arbor.NuGetServer.IisHost.DataServices
{
    public static class ConfigurationStartup
    {
        public static void Start()
        {
            IKeyValueConfiguration keyValueConfiguration = new UserConfiguration(new AppSettingsKeyValueConfiguration());

            KVConfigurationManager.Initialize(keyValueConfiguration);
        }
    }
}
