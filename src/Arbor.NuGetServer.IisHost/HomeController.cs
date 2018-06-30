using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost
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
