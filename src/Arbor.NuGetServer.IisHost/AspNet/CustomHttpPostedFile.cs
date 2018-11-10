using System;
using System.Web;
using Arbor.NuGetServer.Api.Areas.Http;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class CustomHttpPostedFile : IHttpPostedFile
    {
        private readonly HttpPostedFileBase _httpPostedFileBase;

        public CustomHttpPostedFile([NotNull] HttpPostedFileBase httpPostedFileBase)
        {
            _httpPostedFileBase = httpPostedFileBase ?? throw new ArgumentNullException(nameof(httpPostedFileBase));
        }

        public void SaveAs([NotNull] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            }

            _httpPostedFileBase.SaveAs(filePath);
        }
    }
}
