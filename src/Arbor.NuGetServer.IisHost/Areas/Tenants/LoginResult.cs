using System.Security.Claims;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class LoginResult
    {
        public LoginResult(bool isSuccessful, ClaimsIdentity claimsIdentity)
        {
            Identity = claimsIdentity;
            IsSuccessful = isSuccessful;
        }

        public static LoginResult Failed(string reason)
        {
            return new LoginResult(false, null);
        }

        public bool IsSuccessful { get; }

        public ClaimsIdentity Identity { get; }
    }
}