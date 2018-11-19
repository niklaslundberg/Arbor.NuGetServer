using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.Security;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class CustomAuthenticationService
    {
        public async Task<ClaimsIdentity> AuthenticateAsync(string tenantId, string username, string password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(CustomClaimTypes.CanIssueToken, tenantId)
            };

            if (tenantId.Equals("test", StringComparison.OrdinalIgnoreCase)
                && username.Equals("testuser", StringComparison.OrdinalIgnoreCase))
            {
                //claims.Add(new Claim(ClaimTypes.Role, username));
            }

            return new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}