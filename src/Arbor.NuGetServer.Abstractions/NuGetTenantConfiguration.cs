using System;
using System.Runtime.Serialization;
using Arbor.KVConfiguration.Urns;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Abstractions
{
    [Urn(TenantConstants.Tenant)]
    public class NuGetTenantConfiguration
    {
        public NuGetTenantConfiguration(
            [NotNull] string id,
            [NotNull] string apiKey,
            [CanBeNull] string username,
            [CanBeNull] string password,
            [CanBeNull] string packageDirectory)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(apiKey));
            }

            TenantId = new NuGetTenantId(id);
            Id = id;
            ApiKey = apiKey;
            Username = username;
            Password = password;
            PackageDirectory = packageDirectory;
        }

        [IgnoreDataMember]
        public NuGetTenantId TenantId { get; }

        [PublicAPI]
        public string Id { get; }

        public string ApiKey { get; }

        public string Username { get; }

        public string Password { get; }

        public string PackageDirectory { get; }

        public bool AllowAnonymous => string.IsNullOrWhiteSpace(Username)
                                      && string.IsNullOrWhiteSpace(Password);
    }
}
