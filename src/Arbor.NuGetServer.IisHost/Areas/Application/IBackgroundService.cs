using System.Threading;
using System.Threading.Tasks;
using System.Web.DynamicData;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public interface IBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
