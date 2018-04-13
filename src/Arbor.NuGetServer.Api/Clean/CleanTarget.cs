using System.IO;
using Arbor.NuGetServer.Core;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanTarget
    {
        public CleanTarget(FileInfo fileInfo, PackageIdentifier packageIdentifier)
        {
            FileInfo = fileInfo;
            PackageIdentifier = packageIdentifier;
        }

        public FileInfo FileInfo { get; }

        public PackageIdentifier PackageIdentifier { get; }
    }
}
