using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public  class TokenHelper : ITokenHelper
    {
        private readonly IKeyStore _keyStore;

        public TokenHelper(IKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public async Task<string> CreatePackageIdTokenAsync([NotNull] IReadOnlyCollection<Claim> claims, string keyName)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var handler = new JwtSecurityTokenHandler();

            var issuedClaims = new List<Claim>(claims);


            RSAParameters parameters = await _keyStore.GetKeyAsync(keyName);

            var key = new RsaSecurityKey(parameters);

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = "TODO",
                Issuer = "TODO",
                Subject = new ClaimsIdentity(issuedClaims),
                SigningCredentials = new SigningCredentials(key, "RS256")
            };

            SecurityToken securityToken = handler.CreateToken(descriptor);

            string tokenAsString = handler.WriteToken(securityToken);

            return tokenAsString;
        }
    }
}