using System;
using System.Linq;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public static class ViewHelper
    {
        public static NuGetTenantId Tenant(this INuGetTenantReadService readService, Uri uri)
        {
            if (uri is null)
            {
                return null;
            }

            string tenant = uri.GetTenantIdFromUrl();

            if (string.IsNullOrWhiteSpace(tenant))
            {
                return null;
            }

            return readService.GetNuGetTenantIds()
                .SingleOrDefault(nugetTenantId => nugetTenantId.TenantId.Equals(tenant, StringComparison.OrdinalIgnoreCase));
        }
    }
}
