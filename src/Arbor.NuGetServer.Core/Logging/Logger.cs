namespace Arbor.NuGetServer.Core.Logging
{
    public static class Logger
    {
        private static ILogger _logger;

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        public static void Info(string message)
        {
            _logger?.Info(message);
        }

        public static void Error(string message)
        {
            _logger?.Info(message);
        }

        public static void Debug(string message)
        {
            _logger?.Info(message);
        }
    }
}