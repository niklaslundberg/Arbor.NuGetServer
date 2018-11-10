using System;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.Security;

namespace Arbor.NuGetServer.Tools
{
    internal class ShowKeyCommand : AppCommand
    {
        public override Task RunAsync()
        {
            Console.WriteLine("Enter path to key");
            string path = Console.ReadLine();

            RsaKey rsaKey = RsaKeyHelper.ReadKey(path);
            Console.WriteLine(RsaKeyHelper.ConvertToJson(rsaKey));

            return Task.CompletedTask;
        }
    }
}
