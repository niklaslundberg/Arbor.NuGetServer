using System;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public class WebHookConfiguration
    {
        public WebHookConfiguration(Uri url, string signingKeyName)
        {
            Url = url;
            SigningKeyName = signingKeyName;
        }

        public Uri Url { get; private set; }

        public string SigningKeyName { get; private set; }
    }
}