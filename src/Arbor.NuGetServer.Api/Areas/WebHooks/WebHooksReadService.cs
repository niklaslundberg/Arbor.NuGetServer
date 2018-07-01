using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public class WebHooksReadService : IWebHooksReadService
    {
        private readonly INuGetTenantReadService _tenantReadService;

        public WebHooksReadService(INuGetTenantReadService tenantReadService)
        {
            _tenantReadService = tenantReadService;
        }

        public async Task< IReadOnlyList<WebHookConfiguration>> GetWebHookConfigurationsAsync<T>(T notification, CancellationToken cancellationToken) where T : IPackageNotification
        {
            IReadOnlyList<TenantWebHook> tenantWebHooks = await _tenantReadService.GetPackageWebHooksAsync(notification.TenantId, cancellationToken);

            return tenantWebHooks.Select(webHook => webHook.Configuration).ToImmutableArray();
        }
    }
}