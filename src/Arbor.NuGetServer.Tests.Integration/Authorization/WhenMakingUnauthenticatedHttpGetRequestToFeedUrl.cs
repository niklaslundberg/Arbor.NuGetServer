using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration.Authorization
{
    public sealed class WhenMakingUnauthenticatedHttpGetRequestToFeedUrl
    {
        public WhenMakingUnauthenticatedHttpGetRequestToFeedUrl(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private readonly ITestOutputHelper _outputHelper;

        [NCrunch.Framework.Timeout(120_000)]
        [Fact]
        public async Task ThenItShouldReturnHttp401Unauthorized()
        {
            using (IntegrationTestSetup server = await IntegrationTestSetup.StartServerAsync(nameof(WhenMakingUnauthenticatedHttpGetRequestToFeedUrl)))
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request =
                        new HttpRequestMessage(HttpMethod.Get, $"http://{Environment.MachineName}:{server.IIS.Port}/nuget/test/"))
                    {
                        //request.AddToken("testuser", new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed });

                        using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                        {
                            string content = await httpResponseMessage.Content.ReadAsStringAsync();

                            _outputHelper.WriteLine(content);

                            Assert.Equal(HttpStatusCode.Unauthorized, httpResponseMessage.StatusCode);
                        }
                    }
                }
            }
        }
    }
}
