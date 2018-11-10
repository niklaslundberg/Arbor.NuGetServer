using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    [UsedImplicitly]
    public class RsaKey
    {
        public string KeyId { get; set; }

        public RSAParameters Parameters { get; set; }

        public const string Algorithm = "RS256";
    }
}
