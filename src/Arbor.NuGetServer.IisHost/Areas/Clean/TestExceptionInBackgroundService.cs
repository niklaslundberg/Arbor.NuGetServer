using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.IisHost.Areas.Application;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    public class TestExceptionInBackgroundService : IBackgroundService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("Running exception test background service");

            Debug.WriteLine("Waiting in test exception background service");
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            Debug.WriteLine("Waited in test exception background service");

            throw new InvalidOperationException("Exception from test background service");
        }
    }
}
