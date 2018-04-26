using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Clean;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [Authorize]
    public class PackagesController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public PackagesController([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        [Route(RouteConstants.PackageRoute)]
        [Authorize]
        public ActionResult Index()
        {
            string packagesPath =
                _keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath]
                    .ThrowIfNullOrWhitespace($"AppSetting key '{ConfigurationKeys.PackagesDirectoryPath}' is not set");

            string fullPath = Server.MapPath(packagesPath);

            var nugetPackagesDirectory = new DirectoryInfo(fullPath);

            if (!nugetPackagesDirectory.Exists)
            {
                return View(new PackagesViewOutputModel(Array.Empty<PackageFileInfo>(), Array.Empty<OtherFileInfo>()));
            }

            var otherFiles = new List<FileInfo>(1000);

            var nuGetFiles = new List<FileInfo>(1000);

            IEnumerable<FileInfo> enumerateFiles = nugetPackagesDirectory.EnumerateFiles(
                "*.*",
                SearchOption.AllDirectories);

            foreach (FileInfo currentFile in enumerateFiles)
            {
                if (currentFile.Extension.Equals(".nupkg", StringComparison.InvariantCultureIgnoreCase))
                {
                    nuGetFiles.Add(currentFile);
                }
                else
                {
                    otherFiles.Add(currentFile);
                }
            }

            List<PackageFileInfo> relativeNuGetPaths =
                nuGetFiles.Select(file => new PackageFileInfo(file,
                        file.FullName.Substring(fullPath.Length + 1),
                        PackageIdentifierHelper.GetPackageIdentifier(file, nugetPackagesDirectory)))
                    .Where(file => file.PackageIdentifier != null)
                    .OrderBy(_ => _.PackageIdentifier.PackageId)
                    .ThenByDescending(_ => _.PackageIdentifier.SemanticVersion)
                    .ToList();

            List<OtherFileInfo> relativeOtherPaths =
                otherFiles.Select(file => new OtherFileInfo(file,
                        file.FullName.Substring(fullPath.Length + 1)))
                    .ToList();

            var viewModel = new PackagesViewOutputModel(relativeNuGetPaths, relativeOtherPaths);

            return View(viewModel);
        }
    }
}
