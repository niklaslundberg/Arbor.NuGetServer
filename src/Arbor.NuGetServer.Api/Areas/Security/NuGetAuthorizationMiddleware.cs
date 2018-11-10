using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class NuGetAuthorizationMiddleware : OwinMiddleware
    {
        private readonly PathString _nuGetPath = new PathString(TenantConstants.NuGetBaseRoute);
        private readonly INuGetTenantReadService _tenantReadService;

        public NuGetAuthorizationMiddleware(
            [NotNull] OwinMiddleware next,
            [NotNull] INuGetTenantReadService tenantReadService)
            : base(next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (!context.Request.Path.StartsWithSegments(_nuGetPath))
            {
                await Next.Invoke(context);
                return;
            }

            var nuGetTenantId = context.Get<NuGetTenantId>(TenantConstants.Tenant);

            if (nuGetTenantId is null)
            {
                await BadRequestAsync(context, "Missing tenant");
                return;
            }

            NuGetTenantConfiguration nuGetTenantConfiguration = _tenantReadService.GetNuGetTenantConfigurations()
                .SingleOrDefault(tenant => tenant.TenantId == nuGetTenantId);

            if (nuGetTenantConfiguration is null)
            {
                string reason = "Missing tenant configuration";
                await BadRequestAsync(context, reason);
                Challenge(context);
                await WriteUnauthorizedBodyAsync(context, reason);
                return;
            }

            bool isGetRequest = context.Request.IsGetRequest();

            if (isGetRequest && nuGetTenantConfiguration.AllowAnonymous)
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Authentication.User is null)
            {
                Challenge(context);
                await WriteUnauthorizedBodyAsync(context, "Missing user");
                return;
            }

            if (context.Authentication.User.Identity is null)
            {
                Challenge(context);
                await WriteUnauthorizedBodyAsync(context, "Missing identity");
                return;
            }

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                Challenge(context);
                await WriteUnauthorizedBodyAsync(context, "Unauthenticated");
                return;
            }

            if (context.Authentication.User.Identity.IsAuthenticated)
            {
                if (!(context.Authentication.User.Identity is ClaimsIdentity claimsIdentity)
                    || !claimsIdentity.Claims.Any(
                        claim => claim.Type == ClaimTypes.NameIdentifier && !string.IsNullOrWhiteSpace(claim.Value)))
                {
                    Challenge(context);
                    await WriteUnauthorizedBodyAsync(context, "Missing name identifier");
                    return;
                }

                string tenantClaimValue = claimsIdentity.Claims.SingleOrDefault(
                    claim => claim.Type == CustomClaimTypes.TenantId)?.Value;

                if (string.IsNullOrWhiteSpace(tenantClaimValue))
                {
                    Challenge(context);
                    await WriteUnauthorizedBodyAsync(context, "Missing tenant");
                    return;
                }

                if (!tenantClaimValue.Equals(nuGetTenantId.TenantId, StringComparison.OrdinalIgnoreCase))
                {
                    Challenge(context);
                    await WriteUnauthorizedBodyAsync(context, "Tenant mismatch");
                    return;
                }

                bool isFeedRequest = IsFeedRequest(context, isGetRequest);

                if (isFeedRequest)
                {
                    string canReadTenantFeedClaimType = NuGetClaimType.CanReadTenantFeed.Key;

                    string canReedFeedValue = claimsIdentity.Claims.SingleOrDefault(
                        claim => string.Equals(claim.Type,
                            canReadTenantFeedClaimType,
                            StringComparison.Ordinal))?.Value;

                    if (string.IsNullOrWhiteSpace(canReedFeedValue))
                    {
                        Challenge(context);
                        await WriteUnauthorizedBodyAsync(context, $"Missing claim type {canReadTenantFeedClaimType}");
                        return;
                    }

                    if (!nuGetTenantId.TenantId.Equals(canReedFeedValue))
                    {
                        Challenge(context);
                        await WriteUnauthorizedBodyAsync(context, $"Claim value mismatch for claim type {canReadTenantFeedClaimType}");
                        return;
                    }
                }
            }

            await Next.Invoke(context);
        }

        private static bool IsFeedRequest(IOwinContext context, bool isGetRequest)
        {
            if (!isGetRequest)
            {
                return false;
            }

            ImmutableArray<string> immutableArray = context.Request.GetPathSegments();

            if (immutableArray.Length == 2)
            {
                return true;
            }

            //if (immutableArray.Length >= 3 && immutableArray[2].Equals("$metadata", StringComparison.OrdinalIgnoreCase))
            //{
            //    return true;
            //}

            if (immutableArray.Length >= 3 && immutableArray[2].Equals("Search()", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static async Task WriteUnauthorizedBodyAsync(IOwinContext context, string reason = null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = ContentTypes.PlainText;

            using (var streamWriter = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteAsync("Unauthorized2 " + reason);
            }
        }

        private static async Task BadRequestAsync(IOwinContext context, string reason)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = ContentTypes.PlainText;

            using (var streamWriter = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteAsync("Bad request - " + reason);
            }
        }

        private void Challenge(IOwinContext context)
        {
            context.Response.Headers.AppendValues(BasicAuthenticationConstants.Challenge,
                BasicAuthenticationConstants.Realm);
        }
    }
}
