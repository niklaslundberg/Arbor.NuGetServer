using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.KVConfiguration.Core.Extensions.StringExtensions;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.IisHost.Areas.Configuration
{
    public class KeyValueSettingsProvider : ISettingsProvider
    {
        private readonly IKeyValueConfiguration _appSettings;

        public KeyValueSettingsProvider(IKeyValueConfiguration appSettings)
        {
            _appSettings = appSettings;
        }

        public bool GetBoolSetting(string key, bool defaultValue)
        {
            return _appSettings.ValueOrDefault(key, defaultValue);
        }

        public string GetStringSetting(string key, string defaultValue)
        {
            return _appSettings.ValueOrDefault(key, defaultValue);
        }
    }
}
