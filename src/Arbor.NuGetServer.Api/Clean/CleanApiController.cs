using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Arbor.NuGetServer.Core.Http;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Clean
{
    [Authorize]
    public class CleanApiController : ApiController
    {
        private readonly CleanService _cleanService;

        public CleanApiController(CleanService cleanService)
        {
            _cleanService = cleanService;
        }

        [Route(CleanConstants.PostRoute)]
        [HttpPost]
        public IHttpActionResult Clean(CleanInputModel cleanInputModel, string tenant)
        {
            CleanResult cleanResult = _cleanService.Clean(cleanInputModel.Whatif,
                cleanInputModel.PreReleaseOnly,
                cleanInputModel.PackageId,
                cleanInputModel.PackagesToKeep);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(
                        JsonConvert.SerializeObject(new { cleanResult, cleanInputModel }),
                        Encoding.UTF8,
                        ContentType.Json)
            };

            return ResponseMessage(httpResponseMessage);
        }
    }
}
