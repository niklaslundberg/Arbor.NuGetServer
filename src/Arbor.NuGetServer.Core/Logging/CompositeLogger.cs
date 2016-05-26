using System.Collections.Generic;

using Arbor.NuGetServer.Core.Extensions;

namespace Arbor.NuGetServer.Core.Logging
{
    public class CompositeLogger : ILogger
    {
        private readonly IReadOnlyCollection<ILogger> _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            _loggers = loggers.SafeToReadOnlyCollection();
        }

        public void Info(string message)
        {
            foreach (ILogger logger in _loggers)
            {
                logger?.Info(message);
            }
        }

        public void Error(string message)
        {
            foreach (ILogger logger in _loggers)
            {
                logger?.Error(message);
            }
        }

        public void Debug(string message)
        {
            foreach (ILogger logger in _loggers)
            {
                logger?.Debug(message);
            }
        }
    }
}