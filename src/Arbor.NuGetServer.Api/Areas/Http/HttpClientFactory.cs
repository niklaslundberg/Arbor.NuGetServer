using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Arbor.NuGetServer.Core.Extensions;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.Http
{
    [UsedImplicitly]
    public sealed class HttpClientFactory : IHttpClientFactory, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, HttpClient> _dictionary;

        public HttpClientFactory([NotNull] ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dictionary = new ConcurrentDictionary<string, HttpClient>(StringComparer.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _dictionary.Values.ForEach(httpClient => httpClient.SafeDispose());
            _dictionary.Clear();
        }

        public HttpClient GetClient(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "";
            }

            if (_dictionary.TryGetValue(name, out HttpClient client))
            {
                _logger.Debug("Found cached http client with name '{Name}'", name);
                return client;
            }

            HttpClient httpClient = System.Net.Http.HttpClientFactory.Create();

            _logger.Debug("Created new http client with name '{Name}'", name);

            if (_dictionary.TryAdd(name, httpClient))
            {
                _logger.Warning("Could not add http client to dictionary");
            }

            return httpClient;
        }
    }
}
