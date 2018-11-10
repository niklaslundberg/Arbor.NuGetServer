using System.Threading;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Tools.Commands
{
    internal class ExitCommand : AppCommand
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ExitCommand(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public override Task RunAsync()
        {
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }
    }
}
