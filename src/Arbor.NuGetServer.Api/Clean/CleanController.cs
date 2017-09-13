using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

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
        [HttpPost]
        public async Task<IHttpActionResult> CleanAsync(bool whatif = false, bool preReleaseOnly = true, string packageId = "")
        {
            CleanResult cleanResult = await _cleanService.CleanAsync(whatif, preReleaseOnly, packageId);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(
                        JsonConvert.SerializeObject(new { whatif, cleanResult, preReleaseOnly,packageId}),
                        Encoding.UTF8,
                        "application/json")
            };

            return ResponseMessage(httpResponseMessage);
        }
    }
}
