using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using JetBrains.Annotations;
using Serilog;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Arbor.NuGetServer.Api.Clean
{
    [UsedImplicitly]
    public class CleanService
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly ILogger _logger;
        private readonly IPathMapper _pathMapper;
        private readonly INuGetTenantReadService _tenantReadService;

        public CleanService(
            [NotNull] IPathMapper pathMapper,
            [NotNull] ILogger logger,
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            INuGetTenantReadService tenantReadService)
        {
            _pathMapper = pathMapper ?? throw new ArgumentNullException(nameof(pathMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _tenantReadService = tenantReadService;
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
                _logger.Debug("Deleting bin file '{BinFile}'", binFile.FullName);

                if (!whatIf)
                {
                    _logger.Debug("Skipped deleting bin file '{BinFile}' due to what if flag set to true",
                        binFile.FullName);
                    binFile.Delete();
                }
            }
        }

        public async Task<CleanResult> CleanAsync(
            NuGetTenantId nugetTenantId,
            bool whatIf = false,
            bool preReleaseOnly = true,
            string packageId = "",
            int packagesToKeep = -1,
            CancellationToken cancellationToken = default)
        {
            if (!_keyValueConfiguration[CleanConstants.CleanEnabled].ParseAsBoolOrDefault())
            {
                return CleanResult.NotRun;
            }

            if (packagesToKeep <= 0)
            {
                if (!int.TryParse(_keyValueConfiguration[CleanConstants.PackagesToKeepKey],
                        out int packagesToKeepFromConfig) || packagesToKeepFromConfig <= 0)
                {
                    packagesToKeep = CleanConstants.DefaultValues.PackagesToKeep;
                }
            }

            NuGetTenantConfiguration configuration =
                await _tenantReadService.GetNuGetTenantConfigurationAsync(nugetTenantId, cancellationToken);

            string packagesFullPath =
                _pathMapper.MapPath(configuration.PackageDirectory);

            var packageDirectory = new DirectoryInfo(packagesFullPath);

            if (!packageDirectory.Exists)
            {
                return CleanResult.NotRun;
            }

            FileInfo[] allNuGetPackageFiles = packageDirectory
                .GetFiles("*.nupkg", SearchOption.AllDirectories);

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
                _logger.Debug("Deleting package {PackageFile}", cleanTarget.FileInfo.FullName);
            }

            IReadOnlyCollection<CleanedPackage> cleanedPackages = packagesToDelete
                .Select(cleanTarget => new CleanedPackage(cleanTarget.FileInfo.FullName)).SafeToImmutableArray();

            foreach (CleanTarget cleanTarget in packagesToDelete)
            {
                DeletePackageAndRelatedFiles(cleanTarget, whatIf);
            }

            foreach (FileInfo binFile in packageDirectory.GetFiles("*.bin", SearchOption.AllDirectories))
            {
                _logger.Debug("Deleting bin file '{BinFile}'", binFile.FullName);

                if (!whatIf)
                {
                    binFile.Delete();
                }
            }

            return new CleanResult(cleanedPackages, packagesToKeep);
        }

        private void DeletePackageAndRelatedFiles(CleanTarget nugetPackageFile, bool whatIf)
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
                _logger.Debug("Deleting NuGet package file '{PackageFile}'", nugetPackageFile.FileInfo.FullName);
                if (!whatIf)
                {
                    nugetPackageFile.FileInfo.Delete();
                }
            }

            if (shaFile.Exists)
            {
                _logger.Debug("Deleting NuGet package SHA file '{ShaFile}'", shaFile.FullName);
                if (!whatIf)
                {
                    shaFile.Delete();
                }
            }

            if (nuspecFile.Exists)
            {
                _logger.Debug("Deleting NuGet specification file '{NuSpecFile}'", nuspecFile.FullName);

                if (!whatIf)
                {
                    nuspecFile.Delete();
                }
            }

            if (nuspec2File.Exists)
            {
                _logger.Debug("Deleting NuGet specification file '{NuSpecFile}'", nuspec2File.FullName);

                if (!whatIf)
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

                    if (!whatIf)
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
                                if (!whatIf)
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
