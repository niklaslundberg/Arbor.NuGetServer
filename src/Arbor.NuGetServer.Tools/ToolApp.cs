using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Tools
{
    internal sealed class ToolApp : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ImmutableArray<AppCommand> _commands;

        private ToolApp(
            IReadOnlyCollection<AppCommand> commands,
            CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _commands = commands.ToImmutableArray();
        }

        public static async Task<ToolApp> CreateAsync(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            AppCommand[] commands =
            {
                new ExitCommand(cancellationTokenSource),
                new CreateTokenCommand(),
                new ShowKeyCommand(),
                new CreateKeyCommand()
            };
            return new ToolApp(commands, cancellationTokenSource);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        public async Task<int> RunAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await RunCommandAsync();
            }

            return 0;
        }

        private async Task RunCommandAsync()
        {
            for (int i = 0; i < _commands.Length; i++)
            {
                AppCommand command = _commands[i];
                Console.WriteLine($"{i}.\t{command.GetType().Name}");
            }

            string readLine = Console.ReadLine();

            if (int.TryParse(readLine, out int index))
            {
                if (index >= 0 && index < _commands.Length)
                {
                    AppCommand command = _commands[index];

                    await command.RunAsync();
                }
            }
        }
    }
}
