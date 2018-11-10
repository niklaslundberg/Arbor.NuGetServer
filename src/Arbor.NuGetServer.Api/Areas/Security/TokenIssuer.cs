namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenIssuer
    {
        public TokenIssuer(string issuer)
        {
            Issuer = issuer;
        }

        public string Issuer { get; }
    }
}