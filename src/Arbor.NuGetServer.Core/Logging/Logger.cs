using System;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Core.Logging
{
    public static class Logger
    {
        private static ILogger _logger;

        public static ILogger LoggerInstance => _logger;

        public static void Initialize([NotNull] ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
