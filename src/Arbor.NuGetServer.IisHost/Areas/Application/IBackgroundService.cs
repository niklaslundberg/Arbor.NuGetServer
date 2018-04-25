using System.Threading;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public interface IBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
