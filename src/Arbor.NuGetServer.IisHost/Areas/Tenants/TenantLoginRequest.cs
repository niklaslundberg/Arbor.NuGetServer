using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class TenantLoginRequest : IRequest<LoginResult>
    {
        public LoginInput LoginInput { get; }

        public TenantLoginRequest(LoginInput loginInput)
        {
            LoginInput = loginInput;
        }
    }
}