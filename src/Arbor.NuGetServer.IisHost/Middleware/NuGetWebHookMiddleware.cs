using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.IisHost.Configuration;

using Microsoft.Owin;

using Newtonsoft.Json;

namespace Arbor.NuGetServer.IisHost.Middleware
{
    public class NuGetWebHookMiddleware : OwinMiddleware
    {
        public NuGetWebHookMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                var anyStatusCode = new[]
                                        {
                                            (int)HttpStatusCode.Created,
                                            (int)HttpStatusCode.OK
                                        };

                if (anyStatusCode.Any(code => code == context.Response.StatusCode))
                {
                    Uri[] webHooks = KVConfigurationManager.AppSettings.AllValues.Where(
                        pair =>
                        pair.Key.Equals("urn:arbor:nuget:web-hook:url", StringComparison.InvariantCultureIgnoreCase))
                        .Select(pair => pair.Value)
                        .Select(
                            url =>
                                {
                                    Uri parsed;

                                    if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                                    {
                                        return null;
                                    }

                                    if (!Uri.TryCreate(url, UriKind.Absolute, out parsed))
                                    {
                                        return null;
                                    }

                                    return parsed;
                                })
                        .Where(uri => uri != null)
                        .ToArray();

                    List<PackageIdentifier> packageIdentifiers =
                        context.Get<List<PackageIdentifier>>("urn:arbor:nuget:packages");

                    if (packageIdentifiers == null || !packageIdentifiers.Any())
                    {
                        Logger.Info("No package identifiers were published");
                        return;
                    }

                    if (!webHooks.Any())
                    {
                        Logger.Info(
                            $"Found no webhooks: {string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString()))}");
                    }
                    else
                    {
                        Logger.Info(
                            $"Found webhooks: {string.Join(Environment.NewLine, webHooks.Select(hook => hook.ToString()))}");

                        using (var httpClient = new HttpClient())
                        {
                            foreach (Uri url in webHooks)
                            {
                                try
                                {
                                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                                    string packages = JsonConvert.SerializeObject(
                                        packageIdentifiers);

                                    request.Content = new StringContent(packages, Encoding.UTF8, "application/json");

                                    string timeOutAppSettingsValue =
                                        KVConfigurationManager.AppSettings[ConfigurationKeys.NuGetWebHookTimeout];

                                    int timeoutInSeconds;

                                    if (!int.TryParse(timeOutAppSettingsValue, out timeoutInSeconds)
                                        || timeoutInSeconds <= 0)
                                    {
                                        timeoutInSeconds = 10;
                                    }

                                    CancellationTokenSource cancellationTokenSource =
                                        new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds));

                                    using (
                                        HttpResponseMessage httpResponseMessage =
                                            await httpClient.SendAsync(request, cancellationTokenSource.Token))
                                    {
                                        Logger.Info(
                                            $"{url} status: {httpResponseMessage.StatusCode}, content {packages}");

                                        Logger.Info(
                                            await
                                            httpResponseMessage.Content.ReadAsStringAsync());
                                    }
                                }
                                catch (TaskCanceledException ex)
                                {
                                    Logger.Error($"Task cancelled when invoking web hook url {url} {ex}");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Exception when invoking web hook url {url} {ex}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}