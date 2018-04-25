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

            bool result;
            if (!bool.TryParse(value, out result))
            {
                return defaultValue;
            }

            return result;
        }
    }
}
