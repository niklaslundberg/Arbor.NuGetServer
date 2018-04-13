using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.IisHost.Areas.Application;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    public class TestBackgroundService : IBackgroundService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Running test background service");

            while (!cancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine("Waiting in test background service");
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                Debug.WriteLine("Waited in test background service");
            }

            Debug.WriteLine("Done in test background service");
        }
    }
}
