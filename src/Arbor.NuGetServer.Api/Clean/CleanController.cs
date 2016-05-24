using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Arbor.NuGetServer.Api.Clean
{
    [RoutePrefix(CleanConstants.Route)]
    [Authorize]
    public class CleanController : ApiController
    {
        private readonly CleanService _cleanService;

        public CleanController(CleanService cleanService)
        {
            _cleanService = cleanService;
        }

        [Route]
        [HttpGet]
        public async Task<HttpResponseMessage> CleanAsync()
        {
            await _cleanService.CleanAsync();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}