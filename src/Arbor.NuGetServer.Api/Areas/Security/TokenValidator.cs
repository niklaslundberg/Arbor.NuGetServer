using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenValidator
    {
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenValidator([NotNull] TokenConfiguration tokenConfiguration)
        {
            _tokenConfiguration = tokenConfiguration ?? throw new ArgumentNullException(nameof(tokenConfiguration));
        }

        public bool TryValidate(string jwt, out ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                ClaimsPrincipal principal = Validate(jwt);
                claimsPrincipal = principal;
                if (principal is null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                claimsPrincipal = null;
                return false;
            }
        }

        public ClaimsPrincipal Validate([NotNull] string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(jwt));
            }

            if (jwt.Count(c => c.Equals('.')) != 2)
            {
                return null;
            }

            var credentials =
                new SigningCredentials(_tokenConfiguration.RsaKey.ConvertToRsaSecurityKey(), RsaKey.Algorithm);

            var tokenHandler = new JwtSecurityTokenHandler();

            var parameters = new TokenValidationParameters
            {
                IssuerSigningKey = credentials.Key,
                ValidIssuer = _tokenConfiguration.TokenIssuer.Issuer,
                ValidAudience = _tokenConfiguration.TokenAudience.Audience
            };

            ClaimsPrincipal claimsPrincipal;

            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(jwt, parameters, out SecurityToken token);

            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                throw new SecurityTokenInvalidSignatureException($"Failed signing validation using key id {_tokenConfiguration.RsaKey.KeyId}",ex);
            }

            return claimsPrincipal;
        }
    }
}
