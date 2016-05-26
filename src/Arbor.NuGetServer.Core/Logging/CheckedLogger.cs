using System;

using JetBrains.Annotations;

namespace Arbor.NuGetServer.Core.Logging
{
    public class CheckedLogger : ILogger
    {
        private readonly ILogger _logger;

        public CheckedLogger([NotNull] ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        public void Info(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _logger.Info(message);
        }

        public void Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _logger.Info(message);
        }

        public void Debug(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _logger.Info(message);
        }
    }
}