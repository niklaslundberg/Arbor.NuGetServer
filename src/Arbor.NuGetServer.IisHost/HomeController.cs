using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost
{
    public class HomeController : Controller
    {
        [Route("~/")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
