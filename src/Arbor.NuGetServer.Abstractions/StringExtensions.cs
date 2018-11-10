using System;

namespace Arbor.NuGetServer.Abstractions
{
    public static class StringExtensions
    {
        public static string ThrowIfNullOrWhitespace(this string value, string message = "")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentNullException(nameof(value), message);
                }

                throw new ArgumentNullException(nameof(value));
            }

            return value;
        }

        public static string WithDefault(this string text, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return defaultValue;
            }

            return text;
        }
    }
}
