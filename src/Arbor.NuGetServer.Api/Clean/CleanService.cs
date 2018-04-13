using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.Api.Clean
{
    [UsedImplicitly]
    public class CleanService
    {
        private readonly ILogger _logger;
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly IPathMapper _pathMapper;

        public CleanService(IPathMapper pathMapper, ILogger logger, IKeyValueConfiguration keyValueConfiguration)
        {
            _pathMapper = pathMapper;
            _logger = logger;
            _keyValueConfiguration = keyValueConfiguration;
        }

        public void CleanBinFiles(bool whatIf)
        {
            string packagesFullPath =
                _pathMapper.MapPath(
                    _keyValueConfiguration[PackageConfigurationConstants.PackagePath]);

            var packageDirectory = new DirectoryInfo(packagesFullPath);

            if (!packageDirectory.Exists)
            {
                return;
            }

            foreach (FileInfo binFile in packageDirectory.GetFiles("*.bin", SearchOption.AllDirectories))
            {
                _logger.Debug($"Deleting bin file '{binFile.FullName}'");

                if (!whatIf)
                {
                    _logger.Debug($"Skipped deleting bin file '{binFile.FullName}' due to what if flag set to true");
                    binFile.Delete();
                }
            }
        }

        public async Task<CleanResult> CleanAsync(
            bool whatif = false,
            bool preReleaseOnly = true,
            string packageId = "",
            int packagesToKeep = -1)
        {
            if (!_keyValueConfiguration[CleanConstants.CleanEnabled].ParseAsBoolOrDefault())
            {
                return CleanResult.NotRun;
            }

            if (packagesToKeep < 0)
            {
                if (!int.TryParse(_keyValueConfiguration[CleanConstants.PackagesToKeepKey],
                        out int packagesToKeepFromConfig) || packagesToKeepFromConfig <= 0)
                {
                    packagesToKeep = CleanConstants.DefaultValues.PackagesToKeep;
                }
            }

            string packagesFullPath =
                _pathMapper.MapPath(
                    _keyValueConfiguration[PackageConfigurationConstants.PackagePath]);

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

            if (preReleaseOnly)
            {
                filtered = filtered
                    .Where(package => package.PackageIdentifier.SemanticVersion.IsPrerelease)
                    .ToArray();
            }

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
                packagesToDelete.AddRange(group.OrderByDescending(_ => _.PackageIdentifier.SemanticVersion)
                    .Skip(packagesToKeep).ToArray());
            }

            foreach (CleanTarget cleanTarget in packagesToDelete)
            {
                _logger.Debug($"Deleting package {cleanTarget.FileInfo.FullName}");
            }

            IReadOnlyCollection<CleanedPackage> cleanedPackages = packagesToDelete
                .Select(cleanTarget => new CleanedPackage(cleanTarget.FileInfo.FullName)).SafeToReadOnlyCollection();

            foreach (CleanTarget cleanTarget in packagesToDelete)
            {
                DeletePackageAndRelatedFiles(cleanTarget, whatif);
            }

            foreach (FileInfo binFile in packageDirectory.GetFiles("*.bin", SearchOption.AllDirectories))
            {
                _logger.Debug($"Deleting bin file '{binFile.FullName}'");

                if (!whatif)
                {
                    binFile.Delete();
                }
            }

            return new CleanResult(cleanedPackages, packagesToKeep);
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

            string nuspec2FilePath = Path.Combine(
                directoryInfo.FullName,
                $"{nugetPackageFile.PackageIdentifier.PackageId}.nuspec");

            var shaFile = new FileInfo(shaFilePath);
            var nuspecFile = new FileInfo(nuspecFilePath);
            var nuspec2File = new FileInfo(nuspec2FilePath);

            if (nugetPackageFile.FileInfo.Exists)
            {
                _logger.Debug($"Deleting NuGet package file '{nugetPackageFile.FileInfo.FullName}'");
                if (!whatif)
                {
                    nugetPackageFile.FileInfo.Delete();
                }
            }

            if (shaFile.Exists)
            {
                _logger.Debug($"Deleting NuGet package SHA file '{shaFile.FullName}'");
                if (!whatif)
                {
                    shaFile.Delete();
                }
            }

            if (nuspecFile.Exists)
            {
                _logger.Debug($"Deleting NuGet specification file '{nuspecFile.FullName}'");

                if (!whatif)
                {
                    nuspecFile.Delete();
                }
            }

            if (nuspec2File.Exists)
            {
                _logger.Debug($"Deleting NuGet specification file '{nuspec2File.FullName}'");

                if (!whatif)
                {
                    nuspec2File.Delete();
                }
            }

            if (nuspec2File.Directory != null &&
                nuspec2File.Directory.Name.Equals(
                    nugetPackageFile.PackageIdentifier.SemanticVersion.ToNormalizedString()))
            {
                nuspec2File.Directory.Refresh();
                FileInfo[] fileInfos = nuspec2File.Directory.GetFiles();

                if (!fileInfos.Any())
                {
                    DirectoryInfo parent = nuspec2File.Directory.Parent;

                    if (!whatif)
                    {
                        nuspec2File.Directory.Delete();
                    }

                    if (parent != null)
                    {
                        parent.Refresh();

                        if (parent.Name.Equals(nugetPackageFile.PackageIdentifier.PackageId,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (parent.GetDirectories().Length == 0 && parent.GetFiles().Length == 0)
                            {
                                if (!whatif)
                                {
                                    parent.Refresh();

                                    if (parent.Exists)
                                    {
                                        parent.Delete();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
