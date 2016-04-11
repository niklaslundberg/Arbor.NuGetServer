using System;
using System.IO;
using System.Text;

namespace Arbor.NuGetServer.IisHost
{
    static class Logger
    {
        public static void Info(string message)
        {
            Write($"INFO {message}");
        }

        public static void Error(string message)
        {
            Write($"ERROR {message}");
        }

        public static void Debug(string message)
        {
            Write($"DEBUG {message}");
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
