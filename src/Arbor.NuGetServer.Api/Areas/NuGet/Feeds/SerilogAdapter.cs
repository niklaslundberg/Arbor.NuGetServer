using NuGet.Server.Core.Logging;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class SerilogAdapter : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogAdapter(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);

            switch (level)
            {
                case LogLevel.Error:
                    _logger.Error("{Message}", formattedMessage);
                    break;
                case LogLevel.Warning:
                    _logger.Warning("{Message}", formattedMessage);
                    break;
                case LogLevel.Info:
                    _logger.Information("{Message}", formattedMessage);
                    break;
                case LogLevel.Verbose:
                    _logger.Debug("{Message}", formattedMessage);
                    break;
            }
        }
    }
}