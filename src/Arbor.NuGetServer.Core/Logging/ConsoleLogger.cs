using System;

namespace Arbor.NuGetServer.Core.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }
    }
}