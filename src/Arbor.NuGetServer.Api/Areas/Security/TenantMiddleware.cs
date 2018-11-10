using System;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class TenantMiddleware : OwinMiddleware
    {
        [NotNull]
        private readonly TenantIdCache _cache;

        private readonly INuGetTenantReadService _tenantReadService;

        public TenantMiddleware(
            OwinMiddleware next,
            [NotNull] INuGetTenantReadService tenantReadService) : base(next)
        {
            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
            _cache = new TenantIdCache();
        }

        public override Task Invoke(IOwinContext context)
        {
            if (!(context.Get<NuGetTenantId>(TenantConstants.Tenant) is null))
            {
                return Next.Invoke(context);
            }

            NuGetTenantId cached = _cache.Get(context.Request.Uri);

            NuGetTenantId nugetTenantId = cached ?? _tenantReadService.Tenant(context.Request.Uri);

            if (!(nugetTenantId is null))
            {
                context.Environment.Add(TenantConstants.Tenant, nugetTenantId);

                if (cached is null)
                {
                    _cache.Set(nugetTenantId, context.Request.Uri);
                }
            }

            return Next.Invoke(context);
        }
    }
}
