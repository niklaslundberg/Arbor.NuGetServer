namespace Arbor.NuGetServer.IisHost.Extensions
{
    public static class LongExtensions
    {
        public static string AsMiB(this long value)
        {
            return (value / (1024D * 1024D)).ToString("F");
        }
    }
}
