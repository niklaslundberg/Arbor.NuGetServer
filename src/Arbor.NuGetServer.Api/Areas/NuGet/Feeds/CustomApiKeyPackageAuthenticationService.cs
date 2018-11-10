using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using NuGet.Server.Core.Infrastructure;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class CustomApiKeyPackageAuthenticationService : IPackageAuthenticationService
    {
        private readonly ILogger _logger;
        private readonly INuGetTenantReadService _tenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;
        private readonly TokenValidator _validator;

        public CustomApiKeyPackageAuthenticationService(
            ILogger logger,
            ITenantRouteHelper tenantRouteHelper,
            INuGetTenantReadService tenantReadService,
            TokenValidator validator)
        {
            _logger = logger;
            _tenantRouteHelper = tenantRouteHelper;
            _tenantReadService = tenantReadService;
            _validator = validator;
        }

        public bool IsAuthenticated(IPrincipal user, string apiKey, string packageId)
        {
            if (!user.Identity.IsAuthenticated)
            {
                _logger.Information("Could not push package {PackageId}, user is not authenticated", packageId);
                return false;
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.Information("Could not push package {PackageId}, api key is missing", packageId);

                return false;
            }

            NuGetTenantId nuGetTenantId = _tenantRouteHelper.GetTenantId();

            if (nuGetTenantId is null)
            {
                _logger.Information("Could not authenticate package push of {PackageId}, tenant is null", packageId);
                return false;
            }

            NuGetTenantConfiguration tenantConfiguration = _tenantReadService.GetNuGetTenantConfigurations().SingleOrDefault(nuGetTenantConfiguration =>
                nuGetTenantId.TenantId.Equals(nuGetTenantConfiguration.Id, StringComparison.OrdinalIgnoreCase));

            if (tenantConfiguration is null)
            {
                _logger.Information("Could not authenticate package push of {PackageId}, tenant configuration is null",
                    packageId);

                return false;
            }

            if (!_validator.TryValidate(apiKey, out ClaimsPrincipal claimsPrincipal))
            {
                _logger.Information("Could not validate api key when user {User} tried to push packaged {PackageId}", user.Identity.Name, packageId);

                return false;
            }

            if (!claimsPrincipal.Claims.Any(claim =>
                claim.Type.Equals(NuGetClaimType.CanPushPackage.Key, StringComparison.Ordinal)
                && claim.Value.Equals(nuGetTenantId.TenantId)))
            {
                string claims = string.Join(", ", claimsPrincipal.Claims.Select(c => $"['{c.Type}': '{c.Value}']"));

                _logger.Information("Api key for user {User} does not contain push claim when tried to push package {PackageId}, claims: {Claims}", user.Identity.Name, packageId, claims);
                return false;
            }

            _logger.Information("Successfully authenticated package push of {PackageId} for user {User}", packageId, user.Identity.Name);

            return true;
        }
    }
}
