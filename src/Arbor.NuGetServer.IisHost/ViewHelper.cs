using System;
using System.Linq;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api;

namespace Arbor.NuGetServer.IisHost
{
    public static class ViewHelper
    {
        public static string Tenant(this INuGetTenantReadService readService, Uri uri)
        {
            if (uri is null)
            {
                return null;
            }

            string tenant = uri.TenantId();

            return readService.GetNuGetTenantIds()
                .SingleOrDefault(t => t.TenantId.Equals(tenant, StringComparison.OrdinalIgnoreCase))?.TenantId;
        }
    }
}
