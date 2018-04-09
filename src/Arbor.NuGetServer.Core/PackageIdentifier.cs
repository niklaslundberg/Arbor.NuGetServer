﻿using System;

using NuGet.Versioning;

namespace Arbor.NuGetServer.Core
{
    public class PackageIdentifier
    {
        public PackageIdentifier(string packageId, SemanticVersion semanticVersion)
        {
            if (semanticVersion == null)
            {
                throw new ArgumentNullException(nameof(semanticVersion));
            }

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(packageId));
            }

            PackageId = packageId;
            SemanticVersion = semanticVersion;
        }

        public string PackageId { get; }

        public SemanticVersion SemanticVersion { get; }
    }
}