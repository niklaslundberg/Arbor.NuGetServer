using System.Web;
using Arbor.NuGetServer.IisHost.Areas.Abstraction;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class CustomHttpPostedFile : IHttpPostedFile
    {
        private readonly HttpPostedFileBase _httpPostedFileBase;

        public CustomHttpPostedFile(HttpPostedFileBase httpPostedFileBase)
        {
            _httpPostedFileBase = httpPostedFileBase;
        }

        public void SaveAs(string filePath)
        {
            _httpPostedFileBase.SaveAs(filePath);
        }
    }
}
