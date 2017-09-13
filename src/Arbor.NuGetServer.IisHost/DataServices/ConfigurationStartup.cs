using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.SystemConfiguration;
using Arbor.KVConfiguration.UserConfiguration;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Logging;

namespace Arbor.NuGetServer.IisHost.DataServices
{
    public static class ConfigurationStartup
    {
        public static void Start()
        {
            IKeyValueConfiguration keyValueConfiguration = KeyValueConfigurationManager
                .Add(new AppSettingsKeyValueConfiguration())
                .Add(new UserConfiguration())
                .Build();

            StaticKeyValueConfigurationManager.Initialize(keyValueConfiguration);

            Logger.Initialize(
                new CategoryLogger(new CheckedLogger(new CompositeLogger(new DebugLogger(), new ConsoleLogger(), new FileLogger()))));
        }
    }
}
