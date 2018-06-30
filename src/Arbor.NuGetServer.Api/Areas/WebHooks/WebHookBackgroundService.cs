using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Abstraction;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.Messaging;
using Arbor.NuGetServer.Api.Areas.Security;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    [UsedImplicitly]
    public class WebHookBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly PackageNotificationQueueHandler _queueHandler;
        private readonly IHttpClientFactory _httpClientFactory;
        private TimeSpan _httpRequestTimeout;
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly ITokenHelper _tokenHelper;
        private readonly IWebHooksReadService _webHooksReadService;

        public WebHookBackgroundService(
            PackageNotificationQueueHandler queueHandler,
            ILogger logger,
            IKeyValueConfiguration keyValueConfiguration,
            IHttpClientFactory httpClientFactory,
            ITokenHelper tokenHelper,
            IWebHooksReadService webHooksReadService)
        {
            _queueHandler = queueHandler;
            _logger = logger;
            _keyValueConfiguration = keyValueConfiguration;
            _httpClientFactory = httpClientFactory;
            _tokenHelper = tokenHelper;
            _webHooksReadService = webHooksReadService;

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
            while (!stoppingToken.IsCancellationRequested)
            {
                IPackageNotification packageNotification = _queueHandler.Take(stoppingToken);

                IReadOnlyList<WebHookConfiguration> webHooks = await _webHooksReadService.GetWebHookConfigurationsAsync(packageNotification,stoppingToken);

                _logger.Information("Package {Package} was pushed, triggering web hooks",
                    packageNotification.PackageIdentifier);

                if (webHooks.Count == 0)
                {
                    _logger.Information("Found no webhooks: {Message}",
                        string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString())));
                }
                else
                {
                    _logger.Information("Found webhooks: {Hooks}",
                        string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString())));

                    foreach (WebHookConfiguration webHookConfiguration in webHooks)
                    {
                        await SendMessage(webHookConfiguration, packageNotification);
                    }
                }
            }
        }

        private async Task SendMessage(WebHookConfiguration webHookConfiguration, IPackageNotification packagePushedNotification)
        {
            HttpClient httpClient = _httpClientFactory.GetClient(webHookConfiguration.Url.ToString());

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, webHookConfiguration.Url))
                {
                    string packageIdentifier = packagePushedNotification.PackageIdentifier.ToString();

                    string tokenAsString = await _tokenHelper.CreatePackageIdTokenAsync(new[]{new Claim(CustomClaimTypes.PackageIdentifier, packageIdentifier)}, webHookConfiguration.SigningKeyName);

                    string packageAsJson = JsonConvert.SerializeObject(tokenAsString);

                    request.Content = new StringContent(tokenAsString, Encoding.UTF8, ContentTypes.PlainText);

                    var cancellationTokenSource = new CancellationTokenSource(_httpRequestTimeout);

                    using (
                        HttpResponseMessage httpResponseMessage =
                            await httpClient.SendAsync(request, cancellationTokenSource.Token))
                    {
                        _logger.Information("{Url} status: {StatusCode}, content {Packages}",
                            webHookConfiguration.Url,
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
                    webHookConfiguration.Url,
                    packagePushedNotification.PackageIdentifier);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Exception when invoking web hook url {Url} for package {Package}",
                    webHookConfiguration.Url,
                    packagePushedNotification.PackageIdentifier);
            }
        }
    }
}
