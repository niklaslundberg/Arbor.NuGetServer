namespace Arbor.NuGetServer.IisHost.Areas.Abstraction
{
    public interface IHttpPostedFile
    {
        void SaveAs(string filePath);
    }
}