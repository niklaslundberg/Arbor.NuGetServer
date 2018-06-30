using System;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public class NuGetTenantConfiguration
    {
        public NuGetTenantConfiguration(
            [NotNull] NuGetTenantId tenantId,
            string apiKey,
            string username,
            string password)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            ApiKey = apiKey;
            Username = username;
            Password = password;
        }

        public NuGetTenantId TenantId { get; }

        public string ApiKey { get; }

        public string Username { get; }

        public string Password { get; }

        public bool AllowAnonymous => string.IsNullOrWhiteSpace(Username)
                                  && string.IsNullOrWhiteSpace(Password);
    }
}
