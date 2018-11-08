using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Areas.Test
{
    [Authorize]
    [RoutePrefix("shutdown")]
    public class TestApiController : ApiController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public TestApiController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


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

            _hostingEnvironment.InitiateShutdown();

            return ResponseMessage(httpResponseMessage);
        }
    }
}
