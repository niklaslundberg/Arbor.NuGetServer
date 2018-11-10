using System.Threading.Tasks;

namespace Arbor.NuGetServer.Tools
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using (ToolApp toolApp = await ToolApp.CreateAsync(args))
            {
                return await toolApp.RunAsync();
            }
        }
    }
}
