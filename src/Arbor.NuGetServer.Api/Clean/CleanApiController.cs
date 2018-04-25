using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using ContentType = Arbor.NuGetServer.Core.Http.ContentType;

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
        public IHttpActionResult Clean(CleanInputModel cleanInputModel)
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
