using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public interface ITokenHelper
    {
        Task<string> CreatePackageIdTokenAsync(IReadOnlyCollection<Claim> claims, string keyName);
    }
}