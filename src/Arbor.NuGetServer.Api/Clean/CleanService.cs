using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.Core.Logging;

namespace Arbor.NuGetServer.Api.Clean
{

    public class CleanTarget
    {
        public FileInfo FileInfo { get; }
        public PackageIdentifier PackageIdentifier { get; }

        public CleanTarget(FileInfo fileInfo, PackageIdentifier packageIdentifier)
        {
            FileInfo = fileInfo;
            PackageIdentifier = packageIdentifier;
        }
    }

    public class CleanService
    {
        private readonly IPathMapper _pathMapper;

        public CleanService(IPathMapper pathMapper)
        {
            _pathMapper = pathMapper;
        }

        public async Task<CleanResult> CleanAsync(bool whatif = false, bool preReleaseOnly = true, string packageId = "")
        {
            if (!StaticKeyValueConfigurationManager.AppSettings[CleanConstants.CleanEnabled].ParseAsBoolOrDefault())
            {
                return CleanResult.NotRun;
            }

            if (!int.TryParse(StaticKeyValueConfigurationManager.AppSettings[CleanConstants.PackagesToKeepKey], out int packagesToKeep) || packagesToKeep <= 0)
            {
                packagesToKeep = CleanConstants.DefaultValues.PackagesToKeep;
            }

            string packagesFullPath =
                _pathMapper.MapPath(StaticKeyValueConfigurationManager.AppSettings[PackageConfigurationConstants.PackagePath]);

            var packageDirectory = new DirectoryInfo(packagesFullPath);

            if (!packageDirectory.Exists)
            {
                return CleanResult.NotRun;
            }

            FileInfo[] allNuGetPackageFiles = packageDirectory.GetFiles("*.nupkg", SearchOption.AllDirectories);

            CleanTarget[] packagesInfo = allNuGetPackageFiles
                .Select(package => new CleanTarget(package,
                    PackageIdentifierHelper.GetPackageIdentifier(package, packageDirectory)))
                .Where(target => target.PackageIdentifier != null)
                .ToArray();

            CleanTarget[] filtered = packagesInfo;

            filtered = filtered
                .Where(package => package.PackageIdentifier.SemanticVersion.IsPrerelease == preReleaseOnly)
                .ToArray();

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                filtered = filtered.Where(package =>
                        package.PackageIdentifier.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            IEnumerable<IGrouping<string, CleanTarget>> grouped = filtered.GroupBy(_ => _.PackageIdentifier.PackageId);

            var packagesToDelete = new List<CleanTarget>();

            foreach (IGrouping<string, CleanTarget> group in grouped)
            {
                packagesToDelete.AddRange(group.OrderByDescending(_ => _.PackageIdentifier.SemanticVersion).Skip(packagesToKeep).ToArray());
            }

            foreach (CleanTarget cleanTarget in packagesToDelete)
            {
                Logger.Debug($"Deleting package {cleanTarget.FileInfo.FullName}");
            }

            IReadOnlyCollection<CleanedPackage> cleanedPackages = packagesToDelete.Select(cleanTarget => new CleanedPackage(cleanTarget.FileInfo.FullName)).SafeToReadOnlyCollection();

            foreach (CleanTarget cleanTarget in packagesToDelete)
            {
                DeletePackageAndRelatedFiles(cleanTarget, whatif);
            }

            foreach (FileInfo binFile in packageDirectory.GetFiles("*.bin", SearchOption.AllDirectories))
            {
                Logger.Debug($"Deleting bin file '{binFile.FullName}'");

                if (!whatif)
                {
                    binFile.Delete();
                }
            }

            return new CleanResult(cleanedPackages);
        }

        private void DeletePackageAndRelatedFiles(CleanTarget nugetPackageFile, bool whatif)
        {
            DirectoryInfo directoryInfo = nugetPackageFile.FileInfo.Directory;

            if (directoryInfo == null || !directoryInfo.Exists)
            {
                return;
            }

            string shaFilePath = $"{nugetPackageFile.FileInfo.FullName}.sha512";

            string nuspecFilePath = Path.Combine(
                directoryInfo.FullName,
                $"{Path.GetFileNameWithoutExtension(nugetPackageFile.FileInfo.Name)}.nuspec");

            var shaFile = new FileInfo(shaFilePath);
            var nuspecFile = new FileInfo(nuspecFilePath);

            if (nugetPackageFile.FileInfo.Exists)
            {
                Logger.Debug($"Deleting NuGet package file '{nugetPackageFile.FileInfo.FullName}'");
                if (!whatif)
                {
                    nugetPackageFile.FileInfo.Delete();
                }
            }

            if (shaFile.Exists)
            {
                Logger.Debug($"Deleting NuGet package SHA file '{shaFile.FullName}'");
                if (!whatif)
                {
                    shaFile.Delete();
                }
            }

            if (nuspecFile.Exists)
            {
                Logger.Debug($"Deleting NuGet specification file '{nuspecFile.FullName}'");
                if (!whatif)
                {
                    nuspecFile.Delete();
                }
            }
        }

    }
}
