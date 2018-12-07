using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Test
{
    [UsedImplicitly]
    public class TestBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Debug.WriteLine("Running test background service");

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    Debug.WriteLine("Waiting in test background service");
            //    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            //    Debug.WriteLine("Waited in test background service");
            //}

            //Debug.WriteLine("Done in test background service");
        }
    }
}
