﻿using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Abstractions
{
    public class NuGetTenantConfiguration
    {
        private string _id;

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
            _id = id;
            ApiKey = apiKey;
            Username = username;
            Password = password;
            PackageDirectory = packageDirectory;
        }

        [IgnoreDataMember]
        public NuGetTenantId TenantId { get; }

        public string Id => _id;

        public string ApiKey { get; }

        public string Username { get; }

        public string Password { get; }

        public string PackageDirectory { get; }

        public bool AllowAnonymous => string.IsNullOrWhiteSpace(Username)
                                      && string.IsNullOrWhiteSpace(Password);
    }
}