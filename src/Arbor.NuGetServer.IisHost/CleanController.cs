using System;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Clean;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [RoutePrefix(CleanConstants.GetRoute)]
    [Authorize]
    public class CleanController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public CleanController([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        [Route]
        [HttpGet]
        public ActionResult Index()
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

            return View(new CleanViewOutputModel($"/{CleanConstants.PostRoute}", packagesToKeep, true, true));
        }
    }
}
