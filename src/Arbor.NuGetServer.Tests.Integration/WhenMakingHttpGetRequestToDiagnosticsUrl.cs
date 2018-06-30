using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Arbor.Ginkgo;
using Xunit;
using Xunit.Abstractions;
using Path = Arbor.Ginkgo.Path;

namespace Arbor.NuGetServer.Tests.Integration
{
    public class WhenMakingHttpGetRequestToDiagnosticsUrl
    {
        public WhenMakingHttpGetRequestToDiagnosticsUrl(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private readonly ITestOutputHelper _outputHelper;

        [Fact]
        public async Task ThenItShouldReturnHttp200Ok()
        {
            string vcsRootPath = VcsTestPathHelper.FindVcsRootPath();

            Path webSitePath = Path.Combine(vcsRootPath, "src", "Arbor.NuGetServer.IisHost");

            Path templatePath = Path.Combine(vcsRootPath,
                "src",
                "Arbor.NuGetServer.Tests.Integration",
                "applicationhost.config");

            using (IisExpress iis = await IisHelper.StartWebsiteAsync(webSitePath,
                templatePath,
                ignoreSiteRemovalErrors: true,
                onCopiedWebsite: TestHelper.BeforeStart,
                removeSiteOnExit: false))
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage =
                        await httpClient.GetAsync($"http://localhost:{iis.Port}/diagnostics");

                    Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);

                    string content = await httpResponseMessage.Content.ReadAsStringAsync();

                    _outputHelper.WriteLine(content);
                }
            }
        }
    }
}
