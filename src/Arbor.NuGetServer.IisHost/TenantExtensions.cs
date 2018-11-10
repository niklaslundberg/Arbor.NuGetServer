using System;
using System.Web;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.IisHost
{
    public static class TenantExtensions
    {
        public static NuGetTenantId GetTenantId([NotNull] this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            return GetTenantId(httpContext.GetOwinContext());
        }

        public static NuGetTenantId GetTenantId([NotNull] this IOwinContext owinContext)
        {
            if (owinContext == null)
            {
                throw new ArgumentNullException(nameof(owinContext));
            }

            return owinContext.Get<NuGetTenantId>(TenantConstants.Tenant);
        }
    }
}
