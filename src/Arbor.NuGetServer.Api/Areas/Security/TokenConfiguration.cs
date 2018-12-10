using System;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenConfiguration
    {
        public const string DefaultExpirationMinutes = "urn:arbor-nuget-server:security:tokens:expiration-time-in-minutes";
        public const string DefaultAudience= "Arbor NuGet Server Client";
        public const string DefaultIssuer = "Arbor NuGet Server";
        public const string AudienceKey = "urn:arbor-nuget-server:security:tokens:audience";
        public const string IssuerKey = "urn:arbor-nuget-server:security:tokens:issuer";
        public const string SecurityKeyFileKey = "urn:arbor-nuget-server:security:tokens:security-key";

        public TokenConfiguration(
            TimeSpan expirationTime,
            TokenAudience tokenAudience,
            TokenIssuer tokenIssuer,
            RsaKey rsaKey)
        {
            ExpirationTime = expirationTime;
            TokenAudience = tokenAudience;
            TokenIssuer = tokenIssuer;
            RsaKey = rsaKey;
        }

        public TimeSpan ExpirationTime { get; }
        public TokenAudience TokenAudience { get; }
        public TokenIssuer TokenIssuer { get; }
        public RsaKey RsaKey { get; }
    }
}
