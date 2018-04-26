using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.IisHost.Areas.Clean;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Arbor.NuGetServer.IisHost.Areas.Security;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class WebHookBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly PackagePushedQueueHandler _queueHandler;
        private IHttpClientFactory _httpClientFactory;
        private TimeSpan _httpRequestTimeout;
        private IKeyValueConfiguration _keyValueConfiguration;

        public WebHookBackgroundService(
            PackagePushedQueueHandler queueHandler,
            ILogger logger,
            IKeyValueConfiguration keyValueConfiguration,
            IHttpClientFactory httpClientFactory)
        {
            _queueHandler = queueHandler;
            _logger = logger;
            _keyValueConfiguration = keyValueConfiguration;
            _httpClientFactory = httpClientFactory;

            string timeOutAppSettingsValue =
                _keyValueConfiguration[ConfigurationKeys.NuGetWebHookTimeout];

            if (!int.TryParse(timeOutAppSettingsValue, out int timeoutInSeconds)
                || timeoutInSeconds <= 0)
            {
                timeoutInSeconds = 10;
            }

            _httpRequestTimeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Uri[] webHooks = _keyValueConfiguration.AllValues.Where(
                    pair =>
                        pair.Key.Equals("urn:arbor:nuget:web-hook:url",
                            StringComparison.InvariantCultureIgnoreCase))
                .Select(pair => pair.Value)
                .Select(
                    url =>
                    {
                        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            return null;
                        }

                        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri parsed))
                        {
                            return null;
                        }

                        return parsed;
                    })
                .Where(uri => uri != null)
                .ToArray();

            while (!stoppingToken.IsCancellationRequested)
            {
                PackagePushedNotification packagePushedNotification = _queueHandler.Take(stoppingToken);

                _logger.Information("Package {Package} was pushed, triggering web hooks",
                    packagePushedNotification.PackageIdentifier);

                if (!webHooks.Any())
                {
                    _logger.Information("Found no webhooks: {Message}",
                        string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString())));
                }
                else
                {
                    _logger.Information("Found webhooks: {Hooks}",
                        string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString())));

                    foreach (Uri url in webHooks)
                    {
                        await SendMessage(url, packagePushedNotification);
                    }
                }
            }
        }

        private static string CreatePackageIdToken(string packageIdentifier)
        {
            var handler = new JwtSecurityTokenHandler();

            IEnumerable<Claim> claims =
                new List<Claim>
                {
                    new Claim("sub", "sub TODO"),
                    new Claim("custom", packageIdentifier)
                };

            RsaSecurityKey key;
            using (RSA rsa = RSA.Create())
            {
                if (rsa is RSACryptoServiceProvider)
                {
                    rsa.Dispose();
                    var cng = new RSACng(2048);

                    RSAParameters parameters = cng.ExportParameters(true);
                    key = new RsaSecurityKey(parameters);
                }
                else
                {
                    rsa.KeySize = 2048;
                    key = new RsaSecurityKey(rsa);
                }
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = "TODO",
                Issuer = "TODO",
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(key, "RS256")
            };

            SecurityToken securityToken = handler.CreateToken(descriptor);

            string tokenAsString = handler.WriteToken(securityToken);

            return tokenAsString;
        }

        private async Task SendMessage(Uri url, PackagePushedNotification packagePushedNotification)
        {
            HttpClient httpClient = _httpClientFactory.GetClient(url.Host);

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    string packageIdentifier = packagePushedNotification.PackageIdentifier.ToString();

                    string tokenAsString = CreatePackageIdToken(packageIdentifier);

                    var jsonObject = new
                    {
                        NuGetPackage = packagePushedNotification.PackageIdentifier,
                        Jwt = tokenAsString
                    };

                    string packageAsJson = JsonConvert.SerializeObject(jsonObject);

                    request.Content = new StringContent(tokenAsString, Encoding.UTF8, ContentTypes.Json);

                    var cancellationTokenSource = new CancellationTokenSource(_httpRequestTimeout);

                    using (
                        HttpResponseMessage httpResponseMessage =
                            await httpClient.SendAsync(request, cancellationTokenSource.Token))
                    {
                        _logger.Information("{Url} status: {StatusCode}, content {Packages}",
                            url,
                            httpResponseMessage.StatusCode,
                            packageAsJson);

                        string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                        _logger.Information("Response message {ResponseMessage}", responseMessage);
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.Error(ex,
                    "Task cancelled when invoking web hook url {Url} for package {Package}",
                    url,
                    packagePushedNotification.PackageIdentifier);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception when invoking web hook url {Url} for package {Package}",
                    url,
                    packagePushedNotification.PackageIdentifier);
            }
        }
    }
}
