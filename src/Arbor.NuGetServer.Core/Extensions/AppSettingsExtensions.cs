using System;
using Arbor.KVConfiguration.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class AppSettingsExtensions
    {
        public static string ThrowIfNullOrWhitespace([NotNull] this IKeyValueConfiguration keyValueConfiguration,
            [NotNull] string key)
        {
            if (keyValueConfiguration == null)
            {
                throw new ArgumentNullException(nameof(keyValueConfiguration));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            }

            string value = keyValueConfiguration[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Could not get app required setting with key '{key}'");
            }

            return value;
        }

        public static string ThrowIfNullOrWhitespace([NotNull] this string currentValue)
        {
            if (string.IsNullOrWhiteSpace(currentValue))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(currentValue));
            }

            return currentValue;
        }
    }
}
