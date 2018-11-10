using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public interface IWebHooksReadService
    {
        Task< IReadOnlyList<WebHookConfiguration>> GetWebHookConfigurationsAsync<T>(T notification,
            CancellationToken cancellationToken) where T: IPackageNotification;
    }
}