using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Areas.Start
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return View();
        }
    }
}