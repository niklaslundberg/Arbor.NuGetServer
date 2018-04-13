using System;
using System.Collections.Generic;
using System.Net.Http;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class HttpClientFactory : IHttpClientFactory
    {
        private Dictionary<string, HttpClient> _dictionary;

        public HttpClientFactory()
        {
            _dictionary = new Dictionary<string, HttpClient>(StringComparer.OrdinalIgnoreCase);
        }

        public HttpClient GetClient(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "";
            }

            if (_dictionary.TryGetValue(name, out HttpClient client))
            {
                return client;
            }

            HttpClient httpClient = System.Net.Http.HttpClientFactory.Create();

            _dictionary.Add(name, httpClient);

            return httpClient;
        }
    }
}