using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using Arbor.NuGetServer.IisHost.Extensions;
using Arbor.NuGetServer.IisHost.Models;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    [Authorize]
    [RoutePrefix("packages")]
    public class PackagesController : Controller
    {
        [Authorize]
        [Route]
        public ActionResult Index()
        {
            const string Key = "packagesPath";

            string packagesPath =
                ConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace($"AppSetting key '{Key}' is not set");

            string fullPath = Server.MapPath(packagesPath);

            var nugetPackagesDirectory = new DirectoryInfo(fullPath);

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
                nuGetFiles.Select(file => new CustomFileInfo(file, file.FullName.Substring(fullPath.Length + 1)))
                    .OrderBy(_ => _.RelativePath)
                    .ToList();
            List<CustomFileInfo> relativeOtherPaths =
                otherFiles.Select(file => new CustomFileInfo(file, file.FullName.Substring(fullPath.Length + 1)))
                    .OrderBy(_ => _.RelativePath)
                    .ToList();

            var viewModel = new PackagesViewModel(relativeNuGetPaths, relativeOtherPaths);

            return View(viewModel);
        }
    }
}
