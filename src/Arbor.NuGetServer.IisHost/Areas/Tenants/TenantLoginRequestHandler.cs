using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class AuthenticationModule : AppModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomAuthenticationService>().SingleInstance();
        }
    }

    public class TenantLoginRequestHandler : IRequestHandler<TenantLoginRequest, LoginResult>
    {
        private readonly CustomAuthenticationService _customAuthenticationService;

        public TenantLoginRequestHandler(CustomAuthenticationService customAuthenticationService)
        {
            _customAuthenticationService = customAuthenticationService;
        }

        public async Task<LoginResult> Handle(TenantLoginRequest request, CancellationToken cancellationToken)
        {
            if (request is null
                || string.IsNullOrWhiteSpace(request.LoginInput.Username)
                || string.IsNullOrWhiteSpace(request.LoginInput.Password))
            {
                return LoginResult.Failed("Missing input");
            }

            ClaimsIdentity claimsIdentity = await _customAuthenticationService.AuthenticateAsync(request.LoginInput.TenantId, request.LoginInput.Username, request.LoginInput.Password);

            if (claimsIdentity is null)
            {
                return LoginResult.Failed("Invalid username or password");
            }

            return new LoginResult(true, claimsIdentity);
        }
    }
}
