using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Time;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenGenerator
    {
        private readonly ICustomClock _customClock;
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenGenerator(TokenConfiguration tokenConfiguration, ICustomClock customClock)
        {
            _tokenConfiguration = tokenConfiguration;
            _customClock = customClock;
        }

        public JwtSecurityToken CreateJwt([NotNull] NuGetTenantId nugetTenantId, IEnumerable<NuGetClaimType> claimTypes)
        {
            if (nugetTenantId == null)
            {
                throw new ArgumentNullException(nameof(nugetTenantId));
            }

            ImmutableArray<NuGetClaimType> usedClaimTypes =
                claimTypes?.ToImmutableArray() ?? ImmutableArray<NuGetClaimType>.Empty;

            if (usedClaimTypes.IsDefaultOrEmpty)
            {
                throw new ArgumentException("At least one claim must be provided", nameof(claimTypes));
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            DateTime utcNow = _customClock.UtcNow().UtcDateTime;

            var claims = new List<Claim>();

            foreach (NuGetClaimType nugetClaimType in usedClaimTypes)
            {
                claims.Add(new Claim(nugetClaimType.Key, nugetTenantId.TenantId));
            }

            var claimsIdentity = new ClaimsIdentity(claims);

            var credentials =
                new SigningCredentials(_tokenConfiguration.RsaKey.ConvertToRsaSecurityKey(), RsaKey.Algorithm);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _tokenConfiguration.TokenAudience.Audience,
                Expires = utcNow.Add(_tokenConfiguration.ExpirationTime),
                IssuedAt = utcNow,
                Issuer = _tokenConfiguration.TokenIssuer.Issuer,
                NotBefore = utcNow,
                Subject = claimsIdentity,
                SigningCredentials = credentials
            };

            JwtSecurityToken jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(securityTokenDescriptor);

            return jwtSecurityToken;
        }
    }
}
