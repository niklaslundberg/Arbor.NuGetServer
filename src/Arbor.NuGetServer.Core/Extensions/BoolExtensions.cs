namespace Arbor.NuGetServer.Core.Extensions
{
    public static class BoolExtensions
    {
        public static bool ParseAsBoolOrDefault(this string value, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            if (!bool.TryParse(value, out bool result))
            {
                return defaultValue;
            }

            return result;
        }
    }
}
