using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Areas.Start
{
    [AllowAnonymous]
    [RouteArea(StartAreaRegistration.StartAreaName)]
    public class HomeController : Controller
    {
        [Route("~/")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
