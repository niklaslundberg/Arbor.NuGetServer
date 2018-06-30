using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    [RoutePrefix("webhooks/ping")]
    public class WebHookPingController : ApiController
    {
        private readonly ILogger _logger;

        public WebHookPingController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Index()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            string message = await Request.Content.ReadAsStringAsync();

            _logger.Information("Received web hook message {Message}", message);

            return response;
        }
    }
}
