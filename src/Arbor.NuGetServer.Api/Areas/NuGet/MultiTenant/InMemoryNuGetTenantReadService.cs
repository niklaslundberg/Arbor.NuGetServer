using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.WebHooks;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    [UsedImplicitly]
    public sealed class InMemoryNuGetTenantReadService : INuGetTenantReadService
    {
        private readonly TokenGenerator _tokenGenerator;

        public InMemoryNuGetTenantReadService(TokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        public ImmutableArray<NuGetTenantId> GetNuGetTenantIds()
        {
            return GetNuGetTenantConfigurations()
                .Select(config => config.TenantId)
                .ToImmutableArray();
        }

        public ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations()
        {
            var defaultClaims = new[] { new Claim(ClaimTypes.NameIdentifier, "test") };

            JwtSecurityToken testToken = _tokenGenerator.CreateJwt(new NuGetTenantId("test"),
                new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed },
                defaultClaims);

            JwtSecurityToken pushToken = _tokenGenerator.CreateJwt(new NuGetTenantId("test"),
                new List<NuGetClaimType> { NuGetClaimType.CanPushPackage },
                defaultClaims);

            var handler = new JwtSecurityTokenHandler();
            string testPassword = handler.WriteToken(testToken);
            string pushApiToken = handler.WriteToken(pushToken);

            NuGetTenantConfiguration[] tenantsId =
            {
                new NuGetTenantConfiguration("test",
                    pushApiToken,
                    "testuser",
                    testPassword,
                    GetPackageDirectory("test").FullName),
                new NuGetTenantConfiguration("test2", "test2key", "test2user", "test2password", null),
                new NuGetTenantConfiguration("test3", "test3key", "", "", null)
            };

            return tenantsId
                .OrderBy(_ => _.TenantId)
                .ToImmutableArray();
        }

        public Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(
            NuGetTenantId tenantId,
            CancellationToken cancellationToken)
        {
            var webhooks = new List<TenantWebHook>
            {
                new TenantWebHook(new WebHookConfiguration(new Uri("http://localhost:8042"), "test"),
                    new NuGetTenantId("test")),
                new TenantWebHook(new WebHookConfiguration(new Uri("http://localhost:8042"), "test2"),
                    new NuGetTenantId("test2"))
            };

            IReadOnlyList<TenantWebHook> result = webhooks
                .Where(hook => hook.TenantId == tenantId)
                .ToImmutableArray();

            return Task.FromResult(result);
        }

        public Task<NuGetTenantConfiguration> GetNuGetTenantConfigurationAsync(
            NuGetTenantId nugetTenantId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(GetNuGetTenantConfigurations().SingleOrDefault(tenant =>
                tenant.Id.Equals(nugetTenantId.TenantId, StringComparison.OrdinalIgnoreCase)));
        }

        private DirectoryInfo GetPackageDirectory(string tenantId)
        {
            return new DirectoryInfo(Path.Combine(Path.GetTempPath(),
                "ANS",
                tenantId,
                Guid.NewGuid().ToString())).EnsureExists();
        }
    }
}
