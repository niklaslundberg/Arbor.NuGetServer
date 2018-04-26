using System;
using System.Web.Hosting;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    [UsedImplicitly]
    public class ServerMapPath : IPathMapper
    {
        public string MapPath([NotNull] string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            return HostingEnvironment.MapPath(relativePath);
        }
    }
}
