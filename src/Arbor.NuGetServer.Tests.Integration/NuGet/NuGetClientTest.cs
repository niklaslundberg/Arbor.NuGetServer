using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.Time;
using Arbor.NuGetServer.Tests.Integration.Helpers;
using Arbor.Tooler;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration.NuGet
{
    public class NuGetClientTest
    {
        public NuGetClientTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        private string CreateNuGetConfigFile(string sourceName, string sourceUrl, string username, string password)
        {
            string template = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""{sourceName}"" value=""{sourceUrl}"" />
  </packageSources>
  <packageRestore>
    <add key=""enabled"" value=""True"" />
    <add key=""automatic"" value=""True"" />
  </packageRestore>
  <bindingRedirects>
    <add key=""skip"" value=""False"" />
  </bindingRedirects>
  <packageManagement>
    <add key=""format"" value=""1"" />
    <add key=""disabled"" value=""True"" />
  </packageManagement>
  <disabledPackageSources>
  </disabledPackageSources>
  <packageSourceCredentials>
    <{sourceName}>
      <add key=""Username"" value=""{username}"" />
      <add key=""ClearTextPassword"" value=""{password}"" />
    </{sourceName}>
  </packageSourceCredentials>
</configuration>";

            var tempFile = new FileInfo(Path.Combine(Path.GetTempPath(),
                $"ans-{nameof(NuGetListShouldReturnStatusCode0)}",
                "nuget.config"));

            if (tempFile.Directory?.Exists == false)
            {
                tempFile.Directory?.Create();
            }

            File.WriteAllText(tempFile.FullName, template, Encoding.UTF8);

            return tempFile.FullName;
        }

        [Fact]
        public async Task NuGetListShouldReturnStatusCode0()
        {
            using (IntegrationTestSetup server = await IntegrationTestSetup.StartServerAsync(nameof(NuGetClientTest)))
            {
                using (var httpClient = new HttpClient())
                {
                    NuGetDownloadResult nuGetDownloadResult =
                        await new NuGetDownloadClient().DownloadNuGetAsync(NuGetDownloadSettings.Default,
                            Logger.None,
                            httpClient);

                    if (nuGetDownloadResult.Succeeded)
                    {
                        string source = "testSource";

                        var nugetFile = new FileInfo(Path.Combine(
                            VcsTestPathHelper.FindVcsRootPath(),
                            "src",
                            "Arbor.NuGetServer.Tests.Integration",
                            "Arbor.Ginkgo.2.1.1.nupkg"));


                        string hostname = Environment.MachineName; // "ipv4.fiddler";

                        string sourceUrl = $"http://{hostname}:{server.IIS.Port}/nuget/test";

                        string username = "testuser";
                        var tokenGenerator = new TokenGenerator(TestKeys.TestConfiguration, new CustomSystemClock());
                        JwtSecurityToken jwtSecurityToken = tokenGenerator.CreateJwt(new NuGetTenantId("test"),
                            new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed });

                        var tenantService = new InMemoryNuGetTenantReadService(tokenGenerator);

                        NuGetTenantConfiguration tenantConfig = tenantService.GetNuGetTenantConfigurations()
                            .Single(nuGetTenantConfiguration =>
                                nuGetTenantConfiguration.Id.Equals("test", StringComparison.Ordinal));

                        //string destFileName = Path.Combine(tenantConfig.PackageDirectory, nugetFile.Name);

                        //if (!File.Exists(destFileName))
                        //{
                        //    nugetFile.CopyTo(destFileName, overwrite: false);
                        //}

                        var handler = new JwtSecurityTokenHandler();

                        string password = handler.WriteToken(jwtSecurityToken);
                        string configFile = CreateNuGetConfigFile(source, sourceUrl, username, password);

                        JwtSecurityToken apiKeyToken = tokenGenerator.CreateJwt(new NuGetTenantId("test"),
                            new List<NuGetClaimType> { NuGetClaimType.CanPushPackage });

                        var apiKey = handler.WriteToken(apiKeyToken);

                        PushPackage(nugetFile, source, configFile, apiKey, nuGetDownloadResult.NuGetExePath);

                        string testPackageName = "Arbor.Ginkgo";
                        var arguments = new List<string>
                        {
                            "list",
                            testPackageName,
                            "-Source",
                            source,
                            "-ConfigFile",
                            configFile,
                            "-NonInteractive",
                            "-Verbosity",
                            "detailed"
                        };

                        string args = string.Join(" ", arguments.Select(arg => $"\"{arg}\""));
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = nuGetDownloadResult.NuGetExePath,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            Arguments = args,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                        };

                        //startInfo.EnvironmentVariables.Add("http_proxy", "http://127.0.0.1:8888");

                        using (var process = new Process())
                        {
                            process.ErrorDataReceived += (sender, eventArgs) =>
                            {
                                if (eventArgs.Data != null)
                                {
                                    _output.WriteLine(eventArgs.Data);
                                }
                            };
                            process.OutputDataReceived += (sender, eventArgs) =>
                            {
                                if (eventArgs.Data != null)
                                {
                                    _output.WriteLine(eventArgs.Data);
                                }
                            };

                            process.StartInfo = startInfo;

                            process.Start();
                            process.BeginErrorReadLine();
                            process.BeginOutputReadLine();

                            process.WaitForExit();

                            _output.WriteLine($"Exit code {process.ExitCode}");

                            Assert.Equal(0, process.ExitCode);
                        }
                    }
                }
            }
        }

        private void PushPackage(
            FileInfo nugetFile,
            string source,
            string configFile,
            string apiKey,
            string nugetExePath)
        {
            //nuget push foo.nupkg 4003d786-cc37-4004-bfdf-c4f3e8ef9b3a -s https://customsource/
            var arguments = new List<string>
            {
                "push",
                nugetFile.FullName,
                "-Source",
                source,
                "-ConfigFile",
                configFile,
                "-NonInteractive",
                "-Verbosity",
                "detailed",
                "-ApiKey",
                apiKey
            };

            string args = string.Join(" ", arguments.Select(arg => $"\"{arg}\""));
            var startInfo = new ProcessStartInfo
            {
                FileName = nugetExePath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            //startInfo.EnvironmentVariables.Add("http_proxy", "http://127.0.0.1:8888");

            using (var process = new Process())
            {
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    if (eventArgs.Data != null)
                    {
                        _output.WriteLine(eventArgs.Data);
                    }
                };
                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    if (eventArgs.Data != null)
                    {
                        _output.WriteLine(eventArgs.Data);
                    }
                };

                process.StartInfo = startInfo;

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();

                _output.WriteLine($"Exit code {process.ExitCode}");

            }
        }
    }
}
