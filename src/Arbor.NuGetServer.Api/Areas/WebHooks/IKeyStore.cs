using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public interface IKeyStore
    {
        Task<RSAParameters> GetKeyAsync(string keyName);
    }
}