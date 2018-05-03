using System.Collections.Generic;
using Microsoft.Owin;

namespace Arbor.NuGetServer.IisHost.Areas.Abstraction
{
    public interface IRequestFileHelper
    {
        IReadOnlyList<IHttpPostedFile> GetFiles(IOwinContext context);
    }
}
