using System;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.KVConfiguration.Core.Extensions.StringExtensions;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public class KeyValueSettingsProvider : ISettingsProvider
    {
        private readonly IKeyValueConfiguration _appSettings;

        public KeyValueSettingsProvider([NotNull] IKeyValueConfiguration appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
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
