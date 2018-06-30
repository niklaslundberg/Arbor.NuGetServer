using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class NuGetAuthorizationMiddleware : OwinMiddleware
    {
        private readonly INuGetTenantReadService _tenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public NuGetAuthorizationMiddleware(
            [NotNull] OwinMiddleware next,
            [NotNull] ITenantRouteHelper tenantRouteHelper,
            [NotNull] INuGetTenantReadService tenantReadService)
            : base(next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _tenantRouteHelper = tenantRouteHelper ?? throw new ArgumentNullException(nameof(tenantRouteHelper));
            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
        }

        public override async Task Invoke(IOwinContext context)
        {
            var allowedAnonymous = new List<string> { "Scripts", "Content", "diagnostics" };

            if (context.Request.Path.Value == "/" || context.Request.Path.Value == "" || allowedAnonymous.Any(allowed =>
            {
                IReadOnlyCollection<string> pathSegments = context.Request.Uri.PathSegments();

                return pathSegments.Count > 0 &&
                       allowed.Equals(pathSegments.First(), StringComparison.OrdinalIgnoreCase);
            }))
            {
                await Next.Invoke(context);
                return;
            }

            NuGetTenantId nuGetTenantId = _tenantRouteHelper.GetTenant();

            if (nuGetTenantId is null)
            {
                // TODO unauthorized?
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            NuGetTenantConfiguration nuGetTenantConfiguration = _tenantReadService.GetNuGetTenantConfigurations()
                .SingleOrDefault(tenant => tenant.TenantId == nuGetTenantId);

            if (nuGetTenantConfiguration is null)
            {
                // TODO unauthorized?
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (nuGetTenantConfiguration.AllowAnonymous)
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Authentication.User == null)
            {
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (context.Authentication.User.Identity == null)
            {
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (context.Authentication.User.Identity.IsAuthenticated)
            {
                if (!(context.Authentication.User.Identity is ClaimsIdentity claimsIdentity)
                    || !claimsIdentity.Claims.Any(
                        claim => claim.Type == ClaimTypes.NameIdentifier && !string.IsNullOrWhiteSpace(claim.Value)))
                {
                    Challenge(context);
                    await WriteBodyAsync(context);
                    return;
                }

                string tenantClaimValue = claimsIdentity.Claims.SingleOrDefault(
                    c => c.Type == CustomClaimTypes.TenantId)?.Value;

                if (string.IsNullOrWhiteSpace(tenantClaimValue))
                {
                    Challenge(context);
                    await WriteBodyAsync(context);
                    return;
                }

                if (!tenantClaimValue.Equals(nuGetTenantId.TenantId, StringComparison.OrdinalIgnoreCase))
                {
                    Challenge(context);
                    await WriteBodyAsync(context);
                    return;
                }
            }

            await Next.Invoke(context);
        }

        private static async Task WriteBodyAsync(IOwinContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = ContentTypes.PlainText;

            using (var streamWriter = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteAsync("Unauthorized");
            }
        }

        private void Challenge(IOwinContext context)
        {
            context.Response.Headers.AppendValues(BasicAuthenticationConstants.Challenge,
                BasicAuthenticationConstants.Realm);
        }
    }
}
