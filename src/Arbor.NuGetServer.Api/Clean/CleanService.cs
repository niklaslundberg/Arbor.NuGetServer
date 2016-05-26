using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.Core.Logging;

using NuGet.Versioning;

namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanService
    {
        private readonly IPathMapper _pathMapper;

        public CleanService(IPathMapper pathMapper)
        {
            _pathMapper = pathMapper;
        }

        public async Task<CleanResult> CleanAsync(bool whatif = false)
        {
            if (!KVConfigurationManager.AppSettings[CleanConstants.CleanEnabled].ParseAsBoolOrDefault())
            {
                return CleanResult.NotRun;
            }

            string packagesFullPath =
                _pathMapper.MapPath(KVConfigurationManager.AppSettings[PackageConfigurationConstants.PackagePath]);

            var packageDirectory = new DirectoryInfo(packagesFullPath);

            if (!packageDirectory.Exists)
            {
                return CleanResult.NotRun;
            }

            FileInfo[] allNuGetPackageFiles = packageDirectory.GetFiles("*.nupkg", SearchOption.AllDirectories);

            var packagesInfo = allNuGetPackageFiles.Select(
                package => new
                               {
                                   File = package,
                                   PackageIdentifier = GetPackageIdentifier(package, packageDirectory)
                               }).ToArray();

            var preReleaseVersions =
                packagesInfo.Where(package => package.PackageIdentifier.SemanticVersion.IsPrerelease)
                    .SafeToReadOnlyCollection();

            var grouped = preReleaseVersions.GroupBy(_ => _.PackageIdentifier.PackageId);

            var packagesToDelete = new List<FileInfo>();

            foreach (var group in grouped)
            {
                packagesToDelete.AddRange(group.OrderByDescending(_ => _.PackageIdentifier.SemanticVersion).Skip(3).Select(x => x.File).ToArray());
            }

            foreach (FileInfo fileInfo in packagesToDelete)
            {
                Logger.Debug($"Deleting package {fileInfo.FullName}");
            }

            IReadOnlyCollection<CleanedPackage> cleanedPackages = packagesToDelete.Select(file => new CleanedPackage(file.FullName)).SafeToReadOnlyCollection();

            foreach (FileInfo fileInfo in packagesToDelete)
            {
                DeletePackageAndRelatedFiles(fileInfo, whatif);
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

        private void DeletePackageAndRelatedFiles(FileInfo nugetPackageFile, bool whatif)
        {
            DirectoryInfo directoryInfo = nugetPackageFile.Directory;

            if (directoryInfo == null || !directoryInfo.Exists)
            {
                return;
            }

            string shaFilePath = $"{nugetPackageFile.FullName}.sha512";

            string nuspecFilePath = Path.Combine(
                directoryInfo.FullName,
                $"{Path.GetFileNameWithoutExtension(nugetPackageFile.Name)}.nuspec");

            var shaFile = new FileInfo(shaFilePath);
            var nuspecFile = new FileInfo(nuspecFilePath);

            if (nugetPackageFile.Exists)
            {
                Logger.Debug($"Deleting NuGet package file '{nugetPackageFile.FullName}'");
                if (!whatif)
                {
                    nugetPackageFile.Delete();
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

        private PackageIdentifier GetPackageIdentifier(FileInfo fileInfo, DirectoryInfo packageDirectory)
        {
            string relativePath = fileInfo.FullName.Replace(packageDirectory.FullName, "").TrimStart(Path.DirectorySeparatorChar);

            int firstSeparatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);

            string versionAndFile = relativePath.Substring(firstSeparatorIndex + 1);

            int secondSeparatorIndex = versionAndFile.IndexOf(Path.DirectorySeparatorChar);

            string identifier = relativePath.Substring(0, firstSeparatorIndex);

            string version = versionAndFile.Substring(0, secondSeparatorIndex);

            SemanticVersion semanticVersion = SemanticVersion.Parse(version);

            return new PackageIdentifier(identifier, semanticVersion);
        }
    }
}