using System.Threading.Tasks;

namespace Arbor.NuGetServer.Tools.Commands
{
    internal abstract class AppCommand
    {
        public abstract Task RunAsync();
    }
}