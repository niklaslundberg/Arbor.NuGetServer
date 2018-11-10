using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using Arbor.NuGetServer.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration
{
    public static class RequestAuthenticationExtensions
    {
        public static HttpRequestMessage AddToken(this HttpRequestMessage request, string username, List<NuGetClaimType> claimTypes)
        {

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken token =
                new TokenGenerator(TestKeys.TestConfiguration, new CustomSystemClock()).CreateJwt(
                    new NuGetTenantId("test"),
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