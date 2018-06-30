using System.Net.Http;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Abstraction
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient([CanBeNull] string name = null);
    }
}
