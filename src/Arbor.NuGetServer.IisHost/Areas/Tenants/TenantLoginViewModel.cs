namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class TenantLoginViewModel
    {
        public LoginResult LoginResult { get; }

        public TenantLoginViewModel(LoginResult loginResult = null)
        {
            LoginResult = loginResult;
        }
    }
}