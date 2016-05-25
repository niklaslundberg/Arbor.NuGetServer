using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;

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

        public async Task<CleanResult> CleanAsync()
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

            var allNuGetPackageFiles = packageDirectory.GetFiles("*.nupkg", SearchOption.AllDirectories);

            var packagesInfo = allNuGetPackageFiles.Select(
                package => new
                               {
                                   File = package,
                                   PackageIdentifier = GetPackageIdentifier(package, packageDirectory)
                               }).ToArray();

            var preReleaseVersions =
                packagesInfo.Where(package => package.PackageIdentifier.SemanticVersion.IsPrerelease)
                    .SafeToReadOnlyCollection();



            return new CleanResult();
        }

        private PackageIdentifier GetPackageIdentifier(FileInfo fileInfo, DirectoryInfo packageDirectory)
        {
            var relativePath = fileInfo.FullName.Replace(packageDirectory.FullName, "");

            var firstSeparatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);
            var secondSeparatorIndex = relativePath.IndexOf(Path.DirectorySeparatorChar);

            var identifier = relativePath.Substring(0, firstSeparatorIndex);

            var version = relativePath.Substring(firstSeparatorIndex + 1, secondSeparatorIndex);

            SemanticVersion semanticVersion = SemanticVersion.Parse(version);

            return new PackageIdentifier(identifier, semanticVersion);
        }
    }
}