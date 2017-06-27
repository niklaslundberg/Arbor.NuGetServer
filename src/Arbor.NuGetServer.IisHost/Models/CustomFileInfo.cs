using System;
using System.IO;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Models
{
    public class CustomFileInfo
    {
        public CustomFileInfo(FileInfo fileInfo, string relativePath, [NotNull] PackageIdentifier packageIdentifier)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (packageIdentifier == null)
            {
                throw new ArgumentNullException(nameof(packageIdentifier));
            }

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(relativePath));
            }

            FileInfo = fileInfo;
            RelativePath = relativePath;
            PackageIdentifier = packageIdentifier;
        }

        public FileInfo FileInfo { get; }

        public string RelativePath { get; }
        public PackageIdentifier PackageIdentifier { get; }
    }
}
