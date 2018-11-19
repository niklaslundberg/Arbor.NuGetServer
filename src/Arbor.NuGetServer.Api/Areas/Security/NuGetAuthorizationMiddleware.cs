using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.Http;
using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.OwinExtensions;
using JetBrains.Annotations;
using Microsoft.Owin;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class NuGetAuthorizationMiddleware : OwinMiddleware
    {
        private readonly PathString _nuGetPath = new PathString(TenantConstants.NuGetBaseRoute);
        private readonly INuGetTenantReadService _tenantReadService;
        private readonly ILogger _logger;

        public NuGetAuthorizationMiddleware(
            [NotNull] OwinMiddleware next,
            [NotNull] INuGetTenantReadService tenantReadService,
            [NotNull] ILogger logger)
            : base(next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Unauthorized, reason);
                return;
            }

            bool isGetRequest = context.Request.IsGetRequest();

            if (context.Request.IsLoginRequest())
            {
                await Next.Invoke(context);
                return;
            }

            if (isGetRequest && nuGetTenantConfiguration.AllowAnonymous)
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Authentication.User is null)
            {
                Challenge(context);
                await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Unauthorized, "Missing user");
                return;
            }

            if (context.Authentication.User.Identity is null)
            {
                Challenge(context);
                await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Unauthorized, "Missing identity");
                return;
            }

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                Challenge(context);
                await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Unauthorized, "Unauthenticated");
                return;
            }

            if (context.Authentication.User.Identity.IsAuthenticated)
            {
                if (!(context.Authentication.User.Identity is ClaimsIdentity claimsIdentity)
                    || !claimsIdentity.Claims.Any(
                        claim => claim.Type == ClaimTypes.NameIdentifier && !string.IsNullOrWhiteSpace(claim.Value)))
                {
                    Challenge(context);
                    await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Forbidden, "Missing name identifier");
                    return;
                }

                string tenantClaimValue = claimsIdentity.Claims.SingleOrDefault(
                    claim => claim.Type == CustomClaimTypes.TenantId)?.Value;

                if (string.IsNullOrWhiteSpace(tenantClaimValue))
                {
                    Challenge(context);
                    await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Forbidden, "Missing tenant");
                    return;
                }

                if (!tenantClaimValue.Equals(nuGetTenantId.TenantId, StringComparison.OrdinalIgnoreCase))
                {
                    Challenge(context);
                    await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Forbidden, "Tenant mismatch");
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
                        await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Forbidden, $"Missing claim type {canReadTenantFeedClaimType}");
                        return;
                    }

                    if (!nuGetTenantId.TenantId.Equals(canReedFeedValue))
                    {
                        Challenge(context);
                        await WriteAuthorizationFailedBodyAsync(context, HttpStatusCode.Forbidden, $"Claim value mismatch for claim type {canReadTenantFeedClaimType}");
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

            if (immutableArray.Length >= 3 && immutableArray[2].Equals("Search()", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private async Task WriteAuthorizationFailedBodyAsync(IOwinContext context, HttpStatusCode statusCode, string reason = null)
        {
            _logger.Warning("Authorization failed {Reason}, status code {StatusCode}", reason, statusCode);
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = ContentType.PlainText;

            using (var streamWriter = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
            {
                await streamWriter.WriteAsync(statusCode + " " + reason);
            }
        }

        private static async Task BadRequestAsync(IOwinContext context, string reason)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = ContentType.PlainText;

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
