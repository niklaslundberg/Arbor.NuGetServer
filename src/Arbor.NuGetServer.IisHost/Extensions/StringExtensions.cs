using System;

namespace Arbor.NuGetServer.IisHost.Extensions
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
    }
}
