using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.IisHost.Areas.Test
{
    [RoutePrefix("shutdown")]
    public class TestApiController : ApiController
    {
        [Route]
        [HttpGet]
        public IHttpActionResult Shutdown()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(
                        JsonConvert.SerializeObject(new { Message = "Shutting down" }),
                        Encoding.UTF8,
                        "application/json")
            };

            HostingEnvironment.InitiateShutdown();

            return ResponseMessage(httpResponseMessage);
        }
    }
}
