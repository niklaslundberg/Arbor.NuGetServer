using System.Net.Http;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Http
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient([CanBeNull] string name = null);
    }
}
