using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.Time;
using Xunit;

namespace Arbor.NuGetServer.Tests.Integration.Authorization
{
    public class TokenValidatorTests
    {
        [Fact]
        public void CreateAndValidateToken()
        {
            RsaKey rsaKey = RsaKeyHelper.CreateKey();

            var tokenConfiguration = new TokenConfiguration(TimeSpan.FromMinutes(5),
                new TokenAudience("TestAudience"),
                new TokenIssuer("TestIssuer"),
                rsaKey);

            var tokenGenerator = new TokenGenerator(tokenConfiguration, new CustomSystemClock());

            const string tenantId = "abc";
            var nuGetTenantId = new NuGetTenantId(tenantId);
            NuGetClaimType[] nuGetClaimTypes = { NuGetClaimType.CanReadTenantFeed };

            JwtSecurityToken token =
                tokenGenerator.CreateJwt(nuGetTenantId, nuGetClaimTypes);

            var jwtHandler = new JwtSecurityTokenHandler();
            string jwt = jwtHandler.WriteToken(token);

            var tokenValidator = new TokenValidator(tokenConfiguration);

            ClaimsPrincipal claimsPrincipal = tokenValidator.Validate(jwt);

            Assert.Contains(claimsPrincipal.Claims,
                claim => claim.Type.Equals(NuGetClaimType.CanReadTenantFeed.Key, StringComparison.Ordinal)
                         && claim.Value.Equals(tenantId, StringComparison.Ordinal));
        }
    }
}
