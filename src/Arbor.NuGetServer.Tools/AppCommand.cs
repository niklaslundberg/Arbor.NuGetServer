using System.Threading.Tasks;

namespace Arbor.NuGetServer.Tools
{
    internal abstract class AppCommand
    {
        public abstract Task RunAsync();
    }
}