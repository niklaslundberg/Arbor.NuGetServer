using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Clean;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [RoutePrefix(CleanConstants.GetRoute)]
    [Authorize]
    public class CleanController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public CleanController(IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration;
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

            return View(new CleanViewOutputModel($"/{CleanConstants.PostRoute}", packagesToKeep));
        }
    }
}
