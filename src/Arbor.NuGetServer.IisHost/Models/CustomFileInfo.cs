using System;
using System.IO;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Models
{
    public class CustomFileInfo
    {
        public CustomFileInfo(FileInfo fileInfo, string relativePath, PackageIdentifier packageIdentifier)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Argument is null or whitespace", nameof(relativePath));
            }

            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            RelativePath = relativePath;
            PackageIdentifier = packageIdentifier;
        }

        public FileInfo FileInfo { get; }

        public string RelativePath { get; }
        public PackageIdentifier PackageIdentifier { get; }
    }
}
