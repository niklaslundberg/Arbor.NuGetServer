using System;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Api.Areas.Security;

namespace Arbor.NuGetServer.Tools.Commands
{
    internal class ShowKeyCommand : AppCommand
    {
        public override Task RunAsync()
        {
            Console.WriteLine("Enter path to key");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("No path entered");
                return Task.CompletedTask;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"Key path '{path}' does not exist");
                return Task.CompletedTask;
            }

            RsaKey rsaKey = RsaKeyHelper.ReadKey(path);
            Console.WriteLine(RsaKeyHelper.ConvertToJson(rsaKey));

            return Task.CompletedTask;
        }
    }
}
