using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Clean
{
    [RoutePrefix(CleanConstants.PostRoute)]
    [Authorize]
    public class CleanApiController : ApiController
    {
        private readonly CleanService _cleanService;

        public CleanApiController(CleanService cleanService)
        {
            _cleanService = cleanService;
        }

        [Route]
        [HttpPost]
        public async Task<IHttpActionResult> CleanAsync(CleanInputModel cleanInputModel)
        {
            CleanResult cleanResult = await _cleanService.CleanAsync(cleanInputModel.Whatif, cleanInputModel.PreReleaseOnly, cleanInputModel.PackageId, cleanInputModel.PackagesToKeep);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(
                        JsonConvert.SerializeObject(new {cleanResult, cleanInputModel}),
                        Encoding.UTF8,
                        "application/json")
            };

            return ResponseMessage(httpResponseMessage);
        }
    }
}
