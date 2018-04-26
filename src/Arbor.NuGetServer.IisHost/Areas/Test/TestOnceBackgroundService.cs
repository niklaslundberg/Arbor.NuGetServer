using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.IisHost.Areas.Clean;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Test
{
    [UsedImplicitly]
    public class TestOnceBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("Running once test background service");

            Debug.WriteLine("Waiting in test once background service");
            await Task.Delay(TimeSpan.FromSeconds(7), stoppingToken);
            Debug.WriteLine("Waited in test once background service");

            Debug.WriteLine("Done once background service");
        }
    }
}
