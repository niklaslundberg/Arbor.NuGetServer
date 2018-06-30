using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Abstraction
{
    public interface IHttpPostedFile
    {
        void SaveAs([NotNull] string filePath);
    }
}
