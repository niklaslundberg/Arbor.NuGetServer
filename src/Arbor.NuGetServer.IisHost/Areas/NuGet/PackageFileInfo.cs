using System;
using System.IO;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class PackageFileInfo
    {
        public PackageFileInfo(
            [NotNull] FileInfo fileInfo,
            [NotNull] string relativePath,
            [NotNull] PackageIdentifier packageIdentifier)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativePath));
            }

            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            RelativePath = relativePath;
            PackageIdentifier = packageIdentifier ?? throw new ArgumentNullException(nameof(packageIdentifier));
        }

        public FileInfo FileInfo { get; }

        public string RelativePath { get; }

        public PackageIdentifier PackageIdentifier { get; }
    }
}
