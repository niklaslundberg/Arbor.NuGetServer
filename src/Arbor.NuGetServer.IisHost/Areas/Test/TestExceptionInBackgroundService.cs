using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.IisHost.Areas.Clean;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Test
{
    [UsedImplicitly]
    public class TestExceptionInBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("Running exception test background service");

            Debug.WriteLine("Waiting in test exception background service");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            Debug.WriteLine("Waited in test exception background service");

            throw new InvalidOperationException("Exception from test background service");
        }
    }
}
