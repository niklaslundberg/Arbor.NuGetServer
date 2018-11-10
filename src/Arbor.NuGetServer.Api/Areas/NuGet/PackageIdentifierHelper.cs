using System;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using NuGet.Versioning;

namespace Arbor.NuGetServer.Api.Areas.NuGet
{
    public static class PackageIdentifierHelper
    {
        public static PackageIdentifier GetPackageIdentifier(
            [NotNull] FileInfo fileInfo,
            [NotNull] DirectoryInfo packageDirectory)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (packageDirectory == null)
            {
                throw new ArgumentNullException(nameof(packageDirectory));
            }

            string relativePath = fileInfo.FullName
                .Replace(packageDirectory.FullName, "")
                .TrimStart(Path.DirectorySeparatorChar);

            int firstSeparatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);

            if (firstSeparatorIndex < 0)
            {
                return null;
            }

            string versionAndFile = relativePath.Substring(firstSeparatorIndex + 1);

            int secondSeparatorIndex = versionAndFile.IndexOf(Path.DirectorySeparatorChar);

            if (secondSeparatorIndex < 0)
            {
                return null;
            }

            string identifier = relativePath.Substring(0, firstSeparatorIndex);

            string version = versionAndFile.Substring(0, secondSeparatorIndex);

            SemanticVersion semanticVersion = SemanticVersion.Parse(version);

            return new PackageIdentifier(identifier, semanticVersion);
        }
    }
}
