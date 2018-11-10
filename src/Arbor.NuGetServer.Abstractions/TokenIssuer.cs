namespace Arbor.NuGetServer.Abstractions
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