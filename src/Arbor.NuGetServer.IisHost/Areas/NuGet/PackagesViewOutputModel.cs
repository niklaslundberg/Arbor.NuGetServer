using System.Collections.Generic;
using System.Linq;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class PackagesViewOutputModel
    {
        public PackagesViewOutputModel(
            IReadOnlyCollection<PackageFileInfo> relativeNuGetPaths,
            IReadOnlyCollection<OtherFileInfo> relativeOtherPaths)
        {
            RelativeNuGetPaths = relativeNuGetPaths;
            RelativeOtherPaths = relativeOtherPaths;

            TotalNuGetSize = relativeNuGetPaths.Sum(file => file.FileInfo.Length);
            TotalOtherFileSize = relativeOtherPaths.Sum(file => file.FileInfo.Length);

            TopPackagesBySize = RelativeNuGetPaths
                .OrderByDescending(file => file.FileInfo.Length)
                .Take(10)
                .OrderByDescending(_ => _.FileInfo.Length)
                .ToList();
        }

        public long TotalNuGetSize { get; }

        public long TotalOtherFileSize { get; }

        public IReadOnlyCollection<PackageFileInfo> RelativeNuGetPaths { get; }

        public IReadOnlyCollection<PackageFileInfo> TopPackagesBySize { get; }

        public IReadOnlyCollection<OtherFileInfo> RelativeOtherPaths { get; }
    }
}
