using System;

using JetBrains.Annotations;

namespace Arbor.NuGetServer.Core.Logging
{
    public class CategoryLogger : ILogger
    {
        private readonly ILogger _logger;

        public CategoryLogger([NotNull] ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        public void Info(string message)
        {
            _logger.Info($"INFO {message}");
        }

        public void Error(string message)
        {
            _logger.Info($"Error {message}");
        }

        public void Debug(string message)
        {
            _logger.Info($"Debug {message}");
        }
    }
}