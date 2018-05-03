using System.Net.Http;

namespace Arbor.NuGetServer.IisHost.Areas.Abstraction
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient(string name = null);
    }
}
