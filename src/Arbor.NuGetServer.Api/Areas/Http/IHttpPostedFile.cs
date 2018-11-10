using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Http
{
    public interface IHttpPostedFile
    {
        void SaveAs([NotNull] string filePath);
    }
}
