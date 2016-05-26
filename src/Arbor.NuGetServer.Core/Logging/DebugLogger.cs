namespace Arbor.NuGetServer.Core.Logging
{
    public class DebugLogger : ILogger
    {
        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}