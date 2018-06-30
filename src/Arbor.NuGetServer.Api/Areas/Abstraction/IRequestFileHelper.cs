using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.Abstraction
{
    public interface IRequestFileHelper
    {
        IReadOnlyList<IHttpPostedFile> GetFiles([NotNull] IOwinContext context);
    }
}
