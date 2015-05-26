using System.Web.Mvc;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}