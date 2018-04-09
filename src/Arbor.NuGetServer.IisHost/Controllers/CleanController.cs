using System.Web.Mvc;
using Arbor.NuGetServer.Api.Clean;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    [Authorize]
    public class CleanController : Controller
    {
        [HttpGet]
        [Route("~/" + CleanConstants.Route)]
        public ActionResult Index()
        {
            return View(new CleanViewOutputModel($"/{CleanConstants.Route}"));
        }
    }
}
