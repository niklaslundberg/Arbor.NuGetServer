namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenAudience
    {
        public TokenAudience(string audience)
        {
            Audience = audience;
        }

        public string Audience { get; }
    }
}