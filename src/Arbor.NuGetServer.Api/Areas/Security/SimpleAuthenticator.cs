using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class SimpleAuthenticator
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly INuGetTenantReadService _nuGetTenantReadService;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public SimpleAuthenticator(IKeyValueConfiguration keyValueConfiguration, INuGetTenantReadService nuGetTenantReadService, ITenantRouteHelper tenantRouteHelper)
        {
            _keyValueConfiguration = keyValueConfiguration;
            _nuGetTenantReadService = nuGetTenantReadService;
            _tenantRouteHelper = tenantRouteHelper;
        }

        public Task<IEnumerable<Claim>> IsAuthenticated(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
            }

            //string usernameKey = "nuget:authentication:basicauthentication:username";
            //string passwordKey = "nuget:authentication:basicauthentication:password";

            //string storedUsername =
            //    _keyValueConfiguration[usernameKey].ThrowIfNullOrWhitespace(
            //        $"AppSetting key '{usernameKey}' is not set");
            //string storedPassword =
            //    _keyValueConfiguration[passwordKey].ThrowIfNullOrWhitespace(
            //        $"AppSetting key '{passwordKey}' is not set");

            NuGetTenantId tenant = _tenantRouteHelper.GetTenant();

            if (tenant is null)
            {
                return InvalidUser;
            }

            NuGetTenantConfiguration nuGetTenantConfiguration = _nuGetTenantReadService.GetNuGetTenantConfigurations().SingleOrDefault(tenantId => tenantId.TenantId == tenant);

            if (nuGetTenantConfiguration is null)
            {
                return InvalidUser;
            }

            string storedUsername = nuGetTenantConfiguration.Username;
            string storedPassword = nuGetTenantConfiguration.Password;

            bool correctUsername = username.Equals(storedUsername, StringComparison.InvariantCultureIgnoreCase);
            bool correctPassword = password.Equals(storedPassword, StringComparison.InvariantCulture);

            if (!(correctUsername && correctPassword))
            {
                return InvalidUser;
            }

            return
                Task.FromResult<IEnumerable<Claim>>(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, storedUsername),
                        new Claim(ClaimTypes.NameIdentifier, storedUsername),
                        new Claim(CustomClaimTypes.TenantId, tenant.TenantId),
                    });
        }

        public Task<IEnumerable<Claim>> InvalidUser => Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
    }
}
