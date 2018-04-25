using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.IisHost.Areas.Application;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    public class TestOnceBackgroundService : IBackgroundService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Running once test background service");

            Debug.WriteLine("Waiting in test once background service");
            await Task.Delay(TimeSpan.FromSeconds(7), cancellationToken);
            Debug.WriteLine("Waited in test once background service");

            Debug.WriteLine("Done once background service");
        }
    }
}
