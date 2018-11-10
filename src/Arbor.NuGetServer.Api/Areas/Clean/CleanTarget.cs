using System;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Clean
{
    public class CleanTarget
    {
        public CleanTarget([NotNull] FileInfo fileInfo, [NotNull] PackageIdentifier packageIdentifier)
        {
            FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            PackageIdentifier = packageIdentifier ?? throw new ArgumentNullException(nameof(packageIdentifier));
        }

        public FileInfo FileInfo { get; }

        public PackageIdentifier PackageIdentifier { get; }
    }
}
