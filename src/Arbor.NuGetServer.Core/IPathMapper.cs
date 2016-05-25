namespace Arbor.NuGetServer.Core
{
    public interface IPathMapper
    {
        string MapPath(string relativePath);
    }
}