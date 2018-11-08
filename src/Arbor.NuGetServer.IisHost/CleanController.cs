using System;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Clean;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [Authorize]
    public class CleanController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public CleanController([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        [Route(CleanConstants.CleanGetRoute, Name = CleanConstants.CleanGetRouteName)]
        [HttpGet]
        public ActionResult Index(string tenant)
        {
            int packagesToKeep;

            if (!int.TryParse(_keyValueConfiguration[CleanConstants.PackagesToKeepKey],
                    out int packagesToKeepFromConfig) || packagesToKeepFromConfig <= 0)
            {
                packagesToKeep = CleanConstants.DefaultValues.PackagesToKeep;
            }
            else
            {
                packagesToKeep = packagesToKeepFromConfig;
            }

            string cleanPostRoute = $"/{CleanConstants.PostRoute}";

            return View(new CleanViewOutputModel(cleanPostRoute, packagesToKeep, true, true));
        }
    }
}
