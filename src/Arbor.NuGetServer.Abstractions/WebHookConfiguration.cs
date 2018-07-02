using System;

namespace Arbor.NuGetServer.Abstractions
{
    public class WebHookConfiguration
    {
        public WebHookConfiguration(Uri url, string signingKeyName)
        {
            Url = url;
            SigningKeyName = signingKeyName;
        }

        public Uri Url { get; }

        public string SigningKeyName { get; }
    }
}
