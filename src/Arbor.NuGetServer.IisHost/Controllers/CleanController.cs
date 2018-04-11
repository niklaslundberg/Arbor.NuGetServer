using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Clean;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    [RoutePrefix(CleanConstants.GetRoute)]
    [Authorize]
    public class CleanController : Controller
    {
        [Route]
        [HttpGet]
        public ActionResult Index()
        {
            int packagesToKeep;

            if (!int.TryParse(StaticKeyValueConfigurationManager.AppSettings[CleanConstants.PackagesToKeepKey],
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
