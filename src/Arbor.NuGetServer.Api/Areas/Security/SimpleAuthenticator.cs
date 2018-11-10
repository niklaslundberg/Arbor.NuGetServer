using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class SimpleAuthenticator
    {
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;
        private readonly TokenValidator _tokenValidator;

        public SimpleAuthenticator(
            INuGetTenantReadService nuGetTenantReadService,
            ITenantRouteHelper tenantRouteHelper,
            TokenValidator tokenValidator)
        {
            _nuGetTenantReadService = nuGetTenantReadService;
            _tenantRouteHelper = tenantRouteHelper;
            _tokenValidator = tokenValidator;
        }

        public Task<IEnumerable<Claim>> InvalidUser => Task.FromResult<IEnumerable<Claim>>(Array.Empty<Claim>());

        public Task<IEnumerable<Claim>> IsAuthenticated(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return InvalidUser;
            }

            NuGetTenantId tenant = _tenantRouteHelper.GetTenantId();

            if (tenant is null)
            {
                return InvalidUser;
            }

            NuGetTenantConfiguration nuGetTenantConfiguration = _nuGetTenantReadService.GetNuGetTenantConfigurations()
                .SingleOrDefault(tenantId => tenantId.TenantId == tenant);

            if (nuGetTenantConfiguration is null)
            {
                return InvalidUser;
            }

            ClaimsPrincipal claimsPrincipal = _tokenValidator.Validate(password);

            if (claimsPrincipal is null)
            {
                return InvalidUser;
            }

            //string storedUsername = nuGetTenantConfiguration.Username;
            //string storedPassword = nuGetTenantConfiguration.Password;

            //bool correctUsername = username.Equals(storedUsername, StringComparison.InvariantCultureIgnoreCase);
            //bool correctPassword = password.Equals(storedPassword, StringComparison.InvariantCulture);

            //if (!(correctUsername && correctPassword))
            //{
            //    return InvalidUser;
            //}

            var finalClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(CustomClaimTypes.TenantId, tenant.TenantId)
            };

            foreach (Claim claim in claimsPrincipal.Claims.Where(NuGetClaimType.IsNuGetClaimType))
            {
                finalClaims.Add(claim);
            }

            return Task.FromResult<IEnumerable<Claim>>(finalClaims);
        }
    }
}
