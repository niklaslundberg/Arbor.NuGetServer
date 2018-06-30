using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public class InMemoryKeyStore : IKeyStore
    {
        private static readonly
            Lazy<Dictionary<string, RSAParameters>> _dictionary = new Lazy<Dictionary<string, RSAParameters>>(Initialize);

        public Task<RSAParameters> GetKeyAsync(string keyName)
        {
            if (!_dictionary.Value.TryGetValue(keyName, out RSAParameters parameters))
            {
                return Task.FromResult<RSAParameters>(default);
            }

            return Task.FromResult(parameters);
        }

        private static Dictionary<string, RSAParameters> Initialize()
        {
            var rsaParameterses = new Dictionary<string, RSAParameters>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "test",
                    JsonConvert.DeserializeObject<RSAParameters>(
                        @"{""Exponent"":""AQAB"",""Modulus"":""uPEnrmo0u9yVK1EnJm+rIWXBpsK7Lv42XQnnCqTIFO6m11FpEBJR42Ztu7LQ5E50lNTJrR3oZ2QcIP+gyIFd5iV1K/9R3MvHEyVDa99VbMdRUFIBOkJ8pv3bJbkd28wcksanHf6kTTuHq6rr4N9B4Zi76n1010lh0lSfSpX9DZU=""}")
                },
                {
                    "test2",
                    JsonConvert.DeserializeObject<RSAParameters>(
                        @"{""Exponent"":""AQAB"",""Modulus"":""2Lli7kR+x72lojdxH9yh9/qZ65JjAG/himFjGLu4/bXGaf450CAoH7RdywsMrxAE1tEsFYex/gNZd/vLqaaoa3He+aArqnIDEazBPiV6ygm8ATRxxjsgMkget9aQRcRvFFGqcMaqZ+yL7lNfsX3JWkJeAk+9CjqCg6hfyT2MzYU=""}")
                }
            };

            return rsaParameterses;
        }
    }
}