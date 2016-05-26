using System;
using System.IO;
using System.Text;

namespace Arbor.NuGetServer.Core.Logging
{
    public class FileLogger : ILogger
    {
        public void Info(string message)
        {
            Write(message);
        }

        public void Error(string message)
        {
            Write(message);
        }

        public void Debug(string message)
        {
            Write(message);
        }

        private static void Write(string message)
        {
            var file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "log.txt"));

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            var messages = new[]
                               {
                                   message
                               };

            File.AppendAllLines(
                file.FullName,
                messages,
                Encoding.UTF8);
        }
    }
}