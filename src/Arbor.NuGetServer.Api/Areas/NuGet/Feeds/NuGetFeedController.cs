﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2.Controllers;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class NuGetFeedController : NuGetODataController
    {
        public NuGetFeedController(CustomApiKeyPackageAuthenticationService authenticationService, IServerPackageRepository packageRepository)
            : base(packageRepository,
                authenticationService)
        {
        }

        [Route("~/api/v2/package")]
        [HttpPut]
        public async Task<HttpResponseMessage> UploadPackageCompatibility(CancellationToken token)
        {
            string apiKeyFromHeader = GetApiKeyFromHeader();

            IPrincipal requestContextPrincipal = RequestContext.Principal;

            bool isAuthenticated =
                _authenticationService.IsAuthenticated(requestContextPrincipal, apiKeyFromHeader, null);

            if (!isAuthenticated)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            HttpResponseMessage uploadPackageCompatibility = await UploadPackage(token);

            return uploadPackageCompatibility;
        }

        private string GetApiKeyFromHeader()
        {
            string str = null;
            if (Request.Headers.TryGetValues("X-NUGET-APIKEY", out IEnumerable<string> values))
            {
                str = values.FirstOrDefault();
            }

            return str;
        }
    }
}
