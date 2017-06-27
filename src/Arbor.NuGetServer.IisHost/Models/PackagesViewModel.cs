using System.Collections.Generic;
using System.Linq;

namespace Arbor.NuGetServer.IisHost.Models
{
    public class PackagesViewModel
    {
        public PackagesViewModel(
            IReadOnlyCollection<CustomFileInfo> relativeNuGetPaths,
            IReadOnlyCollection<CustomFileInfo> relativeOtherPaths)
        {
            RelativeNuGetPaths = relativeNuGetPaths;
            RelativeOtherPaths = relativeOtherPaths;

            TotalNuGetSize = relativeNuGetPaths.Sum(file => file.FileInfo.Length);
            TotalOtherFileSize = relativeOtherPaths.Sum(file => file.FileInfo.Length);

            TopPackagesBySize = RelativeNuGetPaths.OrderByDescending(file => file.FileInfo.Length).Take(10)
                .OrderByDescending(_ => _.FileInfo.Length)
                .ToList();
        }

        public long TotalNuGetSize { get; }

        public long TotalOtherFileSize { get; }

        public IReadOnlyCollection<CustomFileInfo> RelativeNuGetPaths { get; }

        public IReadOnlyCollection<CustomFileInfo> TopPackagesBySize { get; }

        public IReadOnlyCollection<CustomFileInfo> RelativeOtherPaths { get; }
    }
}
