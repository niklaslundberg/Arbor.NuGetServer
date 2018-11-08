using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Arbor.Ginkgo;
using Arbor.NuGetServer.Marten;
using MysticMind.PostgresEmbed;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration
{
    public sealed class WhenMakingHttpGetRequestToDiagnosticsUrl : IDisposable
    {
        public WhenMakingHttpGetRequestToDiagnosticsUrl(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose()
        {
            _pgServer?.Dispose();
        }

        private const string PostgresqlUser = "postgres";

        private const string ConnectionStringFormat =
            "Server=localhost;Port={0};User Id={1};Password=test;Database=postgres;Pooling=false";

        private const bool AddLocalUserAccessPermission = true;

        private readonly ITestOutputHelper _outputHelper;
        private PgServer _pgServer;

        [NCrunch.Framework.Timeout(120_000)]
        [Fact]
        public async Task ThenItShouldReturnHttp200Ok()
        {
            string vcsRootPath = VcsTestPathHelper.FindVcsRootPath();

            Path webSitePath = Path.Combine(vcsRootPath, "src", "Arbor.NuGetServer.IisHost");

            Path templatePath = Path.Combine(vcsRootPath,
                "src",
                "Arbor.NuGetServer.Tests.Integration",
                "applicationhost.config");

            _pgServer = new PgServer(
                "10.5.1",
                PostgresqlUser,
                addLocalUserAccessPermission: AddLocalUserAccessPermission,
                clearInstanceDirOnStop: true);

            _pgServer.Start();

            string connStr = string.Format(ConnectionStringFormat, _pgServer.PgPort, PostgresqlUser);

            var environmentVariables = new Dictionary<string, string>
            {
                [MartenConstants.MartenConfigurationKey + ":default:enabled"] = "true",
                [MartenConstants.MartenConfigurationKey + ":default:connectionString"] = connStr,
            };
            using (IisExpress iis = await IisHelper.StartWebsiteAsync(webSitePath,
                templatePath,
                ignoreSiteRemovalErrors: true,
                onCopiedWebsite: TestHelper.BeforeStart,
                removeSiteOnExit: false, environmentVariables: environmentVariables))
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage httpResponseMessage =
                        await httpClient.GetAsync($"http://localhost:{iis.Port}/diagnostics");

                    string content = await httpResponseMessage.Content.ReadAsStringAsync();

                    _outputHelper.WriteLine(content);

                    Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
                }
            }
        }
    }
}
