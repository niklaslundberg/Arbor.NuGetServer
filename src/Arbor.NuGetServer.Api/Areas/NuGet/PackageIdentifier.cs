﻿using System;
using JetBrains.Annotations;
using NuGet.Versioning;

namespace Arbor.NuGetServer.Api.Areas.NuGet
{
    public class PackageIdentifier
    {
        public PackageIdentifier(
            [NotNull] string packageId,
            [NotNull] SemanticVersion semanticVersion)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(packageId));
            }

            PackageId = packageId;
            SemanticVersion = semanticVersion ?? throw new ArgumentNullException(nameof(semanticVersion));
        }

        public string PackageId { get; }

        public SemanticVersion SemanticVersion { get; }

        public override string ToString()
        {
            return
                $"{nameof(PackageId)}: {PackageId}, {nameof(SemanticVersion)}: {SemanticVersion.ToNormalizedString()}";
        }
    }
}
