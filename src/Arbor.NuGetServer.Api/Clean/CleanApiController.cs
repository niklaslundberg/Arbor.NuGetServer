using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Core.Http;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Clean
{
    [Authorize]
    public class CleanApiController : ApiController
    {
        private readonly CleanService _cleanService;

        public CleanApiController([NotNull] CleanService cleanService)
        {
            _cleanService = cleanService ?? throw new ArgumentNullException(nameof(cleanService));
        }

        [Route(CleanConstants.PostRoute)]
        [HttpPost]
        public async Task<IHttpActionResult> Clean(CleanInputModel cleanInputModel, string tenant)
        {
            NuGetTenantId nugetTenantId = ValidateTenant(tenant);

            CleanResult cleanResult = await _cleanService.CleanAsync(
                nugetTenantId,
                cleanInputModel.Whatif,
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

        private NuGetTenantId ValidateTenant(string tenant)
        {
            //TODO add validation
            return new NuGetTenantId(tenant);
        }
    }
}
