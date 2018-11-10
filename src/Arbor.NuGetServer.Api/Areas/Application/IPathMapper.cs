namespace Arbor.NuGetServer.Api.Areas.Application
{
    public interface IPathMapper
    {
        string MapPath(string relativePath);
    }
}
