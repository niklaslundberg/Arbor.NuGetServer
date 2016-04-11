using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;

using Microsoft.Owin;

using Newtonsoft.Json;

namespace Arbor.NuGetServer.IisHost
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
                    string[] webHooks = KVConfigurationManager.AppSettings.AllValues.Where(
                        pair =>
                        pair.Key.Equals("urn:arbor:nuget:web-hook:url", StringComparison.InvariantCultureIgnoreCase))
                        .Select(pair => pair.Value).ToArray();

                    List<PackageIdentifier> packageIdentifiers =
                        context.Get<List<PackageIdentifier>>("urn:arbor:nuget:packages");

                    if (packageIdentifiers == null || !packageIdentifiers.Any())
                    {
                        Logger.Info("No package identifiers were published");
                        return;
                    }

                    Logger.Info($"Found webhooks: {string.Join(Environment.NewLine, webHooks)}");

                    using (var httpClient = new HttpClient())
                    {
                        foreach (string url in webHooks)
                        {
                            try
                            {
                                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

                                string packages = JsonConvert.SerializeObject(
                                    packageIdentifiers);

                                request.Content = new StringContent(packages, Encoding.UTF8, "application/json");

                                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                                {
                                    Logger.Info(
                                        $"{url} status: {httpResponseMessage.StatusCode}, content {packages}");

                                    Logger.Info(
                                        await
                                        httpResponseMessage.Content.ReadAsStringAsync());
                                }
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
