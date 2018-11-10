using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.Http
{
    public interface IRequestFileHelper
    {
        IReadOnlyList<IHttpPostedFile> GetFiles([NotNull] IOwinContext context);
    }
}
