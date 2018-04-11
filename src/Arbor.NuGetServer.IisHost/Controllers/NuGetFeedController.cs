using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Arbor.NuGetServer.IisHost.Configuration;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2.Controllers;

namespace Arbor.NuGetServer.IisHost.Controllers
{
    [Authorize]
    public class NuGetFeedController : NuGetODataController
    {
        public NuGetFeedController(NuGetFeedConfiguration nuGetFeedConfiguration)
            : base(nuGetFeedConfiguration.Repository, new ApiKeyPackageAuthenticationService(true, nuGetFeedConfiguration.ApiKey))
        {
        }

        [AllowAnonymous]
        [Route("~/api/v2/package")]
        [HttpPut]
        public virtual async Task<HttpResponseMessage> UploadPackageCompatibility(CancellationToken token)
        {
            string apiKeyFromHeader = GetApiKeyFromHeader();


            IPrincipal requestContextPrincipal = RequestContext.Principal;

            bool isAuthenticated = _authenticationService.IsAuthenticated(requestContextPrincipal, apiKeyFromHeader, null);

            HttpResponseMessage uploadPackageCompatibility = await UploadPackage(token);

            return uploadPackageCompatibility;
        }

        private string GetApiKeyFromHeader()
        {
            string str = (string)null;
            IEnumerable<string> values;
            if (this.Request.Headers.TryGetValues("X-NUGET-APIKEY", out values))
                str = values.FirstOrDefault<string>();
            return str;
        }

    }

    [Authorize]
    public class TestController : ApiController
    {
    }
}
