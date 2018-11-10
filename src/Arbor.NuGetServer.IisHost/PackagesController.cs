using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api;
using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using JetBrains.Annotations;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace Arbor.NuGetServer.IisHost
{
    public class PackagesController : Controller
    {
        private readonly IReadOnlyCollection<NuGetFeedConfiguration> _feedConfigurations;
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly INuGetTenantReadService _tenantReadService;

        public PackagesController(
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            INuGetTenantReadService tenantReadService,
            IReadOnlyCollection<NuGetFeedConfiguration> feedConfigurations)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _tenantReadService = tenantReadService;
            _feedConfigurations = feedConfigurations;
        }

        [Route(RouteConstants.PackageRoute, Name = RouteConstants.PackageRouteName)]
        public ActionResult Index(string tenant)
        {
            var foundTenant = _feedConfigurations
                .SingleOrDefault(t => t.Id.Equals(tenant, StringComparison.OrdinalIgnoreCase));

            if (foundTenant is null)
            {
                return new HttpStatusCodeResult(400);
            }

            string packagesPath = foundTenant.PackageDirectory;

            var nugetPackagesDirectory = new DirectoryInfo(packagesPath);

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
                        file.FullName.Substring(packagesPath.Length + 1),
                        PackageIdentifierHelper.GetPackageIdentifier(file, nugetPackagesDirectory)))
                    .Where(file => file.PackageIdentifier != null)
                    .OrderBy(_ => _.PackageIdentifier.PackageId)
                    .ThenByDescending(_ => _.PackageIdentifier.SemanticVersion)
                    .ToList();

            List<OtherFileInfo> relativeOtherPaths =
                otherFiles.Select(file => new OtherFileInfo(file,
                        file.FullName.Substring(packagesPath.Length + 1)))
                    .ToList();

            var viewModel = new PackagesViewOutputModel(relativeNuGetPaths, relativeOtherPaths);

            return View(viewModel);
        }
    }
}
