using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arbor.Ginkgo;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;
using MysticMind.PostgresEmbed;

namespace Arbor.NuGetServer.Tests.Integration
{
    public sealed class IntegrationTestSetup : IDisposable
    {
        private const string PostgresqlUser = "postgres";

        private const string ConnectionStringFormat =
            "Server=localhost;Port={0};User Id={1};Password=test;Database=postgres;Pooling=false";

        private const bool AddLocalUserAccessPermission = true;
        private EnvironmentVariableScope _environmentVariableScope;
        private PgServer _pgServer;
        private TempDirectory _tempDirectory;

        private IntegrationTestSetup(
            string contextName,
            IisExpress iis,
            PgServer pgServer,
            EnvironmentVariableScope environmentVariableScope,
            TempDirectory tempDirectory)
        {
            _pgServer = pgServer;
            _environmentVariableScope = environmentVariableScope;
            ContextName = contextName;
            IIS = iis;
            _tempDirectory = tempDirectory;
            TempDirectory = tempDirectory;
        }

        public string ContextName { get; }

        public IisExpress IIS { get; private set; }

        public TempDirectory TempDirectory { get; }

        public static async Task<IntegrationTestSetup> StartServerAsync([NotNull] string contextName)
        {
            if (string.IsNullOrWhiteSpace(contextName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(contextName));
            }

            TempDirectory tempDirectory = TempDirectory.CreateTempDirectory(contextName);

            EnvironmentVariableScope environmentVariableScope = EnvironmentVariableScope.Create(
                new Dictionary<string, string>
                {
                    ["TMP"] = tempDirectory.Directory.FullName,
                    ["TEMP"] = tempDirectory.Directory.FullName
                });

            try
            {
                string vcsRootPath = VcsTestPathHelper.FindVcsRootPath();

                Path webSitePath = Path.Combine(vcsRootPath, "src", "Arbor.NuGetServer.IisHost");

                Path templatePath = Path.Combine(vcsRootPath,
                    "src",
                    "Arbor.NuGetServer.Tests.Integration",
                    "applicationhost.config");

                //var pgServer = new PgServer(
                //    "10.5.1",
                //    PostgresqlUser,
                //    addLocalUserAccessPermission: AddLocalUserAccessPermission,
                //    clearInstanceDirOnStop: true);

                //pgServer.Start();

                //string connStr = string.Format(ConnectionStringFormat, pgServer.PgPort, PostgresqlUser);

                //var environmentVariables = new Dictionary<string, string>
                //{
                //    [MartenConstants.MartenConfigurationKey + ":default:enabled"] = "true",
                //    [MartenConstants.MartenConfigurationKey + ":default:connectionString"] = connStr,
                //};

                string tempKeyFile = System.IO.Path.GetTempFileName();

                RsaKeyHelper.WriteKey(TestKeys.TestKey, tempKeyFile);

                var environmentVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    [TokenConfiguration.AudienceKey] = TestKeys.TestAudience,
                    [TokenConfiguration.IssuerKey] = TestKeys.TestIssuer,
                    [TokenConfiguration.SecurityKeyFileKey] = tempKeyFile,
                    //["tmp"] = tempDirectory.Directory.FullName,
                    ["temp"] = tempDirectory.Directory.FullName
                };

                IisExpress iis = await IisHelper.StartWebsiteAsync(webSitePath,
                    templatePath,
                    ignoreSiteRemovalErrors: true,
                    onCopiedWebsite: TestHelper.BeforeStart,
                    removeSiteOnExit: false,
                    environmentVariables: environmentVariables,
                    customHostName: Environment.MachineName,
                    httpPort: 45000);

                var integrationTestSetup = new IntegrationTestSetup(
                    contextName,
                    iis,
                    null,
                    environmentVariableScope,
                    tempDirectory);

                return integrationTestSetup;
            }
            catch (Exception)
            {
                environmentVariableScope?.Dispose();
                tempDirectory.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            IIS?.Dispose();
            _pgServer?.Dispose();
            _environmentVariableScope?.Dispose();

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            _tempDirectory?.Dispose();
            IIS = null;
            _pgServer = null;
            _environmentVariableScope = null;
            _tempDirectory = null;
        }
    }
}
