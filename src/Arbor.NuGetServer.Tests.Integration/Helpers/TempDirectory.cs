using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;

namespace Arbor.NuGetServer.Tests.Integration.Helpers
{
    public sealed class TempDirectory : IDisposable
    {
        private TempDirectory(DirectoryInfo directoryInfo)
        {
            Directory = directoryInfo;
        }

        public DirectoryInfo Directory { get; }

        static string Hash(string input)
        {
            using (MD5 sha1 = new MD5Cng())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static TempDirectory CreateTempDirectory(string contextName = null)
        {
            string uniqueName = Hash($"arns-{contextName}-{Guid.NewGuid()}");
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(),
                    uniqueName))
                .EnsureExists();

            return new TempDirectory(directoryInfo);
        }

        public void Dispose()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            if (Directory?.Exists == true)
            {
                Directory.Delete(true);
            }
        }
    }
}
