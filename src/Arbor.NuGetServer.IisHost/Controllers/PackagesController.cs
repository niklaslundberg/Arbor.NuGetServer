using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Configuration;
using Arbor.NuGetServer.IisHost.Models;
using Arbor.NuGetServer.IisHost.Routes;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    [Authorize]
    public class PackagesController : Controller
    {
        private readonly IPathMapper _mapper;

        public PackagesController(IPathMapper mapper)
        {
            _mapper = mapper;
        }

        [Route(RouteConstants.PackageRoute)]
        [Authorize]
        public ActionResult Index()
        {

            string packagesPath =
                StaticKeyValueConfigurationManager.AppSettings[ConfigurationKeys.PackagesDirectoryPath]
                    .ThrowIfNullOrWhitespace($"AppSetting key '{ConfigurationKeys.PackagesDirectoryPath}' is not set");

            string fullPath = Server.MapPath(packagesPath);

            var nugetPackagesDirectory = new DirectoryInfo(fullPath);


            if (!nugetPackagesDirectory.Exists)
            {
                return View(new PackagesViewModel(Array.Empty<CustomFileInfo>(), Array.Empty<CustomFileInfo>()));
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

                List<CustomFileInfo> relativeNuGetPaths =
                    nuGetFiles.Select(file => new CustomFileInfo(file,
                            file.FullName.Substring(fullPath.Length + 1),
                            PackageIdentifierHelper.GetPackageIdentifier(file, nugetPackagesDirectory)))
                        .Where(file => file.PackageIdentifier != null)
                        .OrderBy(_ => _.PackageIdentifier.PackageId)
                        .ThenByDescending(_ => _.PackageIdentifier.SemanticVersion)
                        .ToList();

                List<CustomFileInfo> relativeOtherPaths =
                    otherFiles.Select(file => new CustomFileInfo(file,
                            file.FullName.Substring(fullPath.Length + 1),
                            PackageIdentifierHelper.GetPackageIdentifier(file, nugetPackagesDirectory)))
                        .Where(package => package.PackageIdentifier != null)
                        .OrderBy(_ => _.PackageIdentifier.PackageId)
                        .ThenByDescending(_ => _.PackageIdentifier.SemanticVersion)
                        .ToList();


            var viewModel = new PackagesViewModel(relativeNuGetPaths, relativeOtherPaths);

            return View(viewModel);
        }
    }
}
