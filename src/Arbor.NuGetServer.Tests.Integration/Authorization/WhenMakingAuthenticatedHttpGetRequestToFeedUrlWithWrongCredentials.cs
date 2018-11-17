using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Tests.Integration.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration.Authorization
{
    public sealed class WhenMakingAuthenticatedHttpGetRequestToFeedUrlWithWrongCredentials
    {
        public WhenMakingAuthenticatedHttpGetRequestToFeedUrlWithWrongCredentials(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private readonly ITestOutputHelper _outputHelper;

        [NCrunch.Framework.Timeout(120_000)]
        [Fact]
        public async Task ThenItShouldReturnHttp403Forbidden()
        {
            using (IntegrationTestSetup server = await IntegrationTestSetup.StartServerAsync(nameof(WhenMakingAuthenticatedHttpGetRequestToFeedUrl)))
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request =
                        new HttpRequestMessage(HttpMethod.Get, $"http://{Environment.MachineName}:{server.IIS.Port}/nuget/test/"))
                    {
                        request.AddToken("testuser2", "test2", TestKeys.TestKey, new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed });

                        using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                        {
                            string content = await httpResponseMessage.Content.ReadAsStringAsync();

                            _outputHelper.WriteLine(content);

                            Assert.Equal(HttpStatusCode.Forbidden, httpResponseMessage.StatusCode);
                        }
                    }
                }
            }
        }
    }
}