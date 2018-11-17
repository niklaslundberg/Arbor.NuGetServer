using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.Time;

namespace Arbor.NuGetServer.Tests.Integration.Helpers
{
    public static class RequestAuthenticationExtensions
    {
        public static HttpRequestMessage AddToken(this HttpRequestMessage request, string username, string tenantId, RsaKey rsaKey, List<NuGetClaimType> claimTypes)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken token =
                new TokenGenerator(TestKeys.GetTestConfiguration(rsaKey), new CustomSystemClock()).CreateJwt(
                    new NuGetTenantId(tenantId),
                    claimTypes);

            string jwt = handler.WriteToken(token);

            byte[] byteArray = Encoding.UTF8.GetBytes($"{username}:{jwt}");

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(byteArray));

            return request;

        }
    }
}
