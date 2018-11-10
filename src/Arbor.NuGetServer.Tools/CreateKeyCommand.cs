using System;
using System.Threading.Tasks;
using Arbor.NuGetServer.Abstractions;

namespace Arbor.NuGetServer.Tools
{
    internal class CreateKeyCommand : AppCommand
    {
        public override Task RunAsync()
        {
            Console.WriteLine("Enter path to save key");
            string path = Console.ReadLine();

            RsaKey rsaKey = RsaKeyHelper.CreateKey();

            RsaKeyHelper.WriteKey(rsaKey, path);

            return Task.CompletedTask;
        }
    }
}
