namespace Arbor.NuGetServer.Abstractions
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