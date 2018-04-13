using System.Net.Http;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient(string name = null);
    }
}