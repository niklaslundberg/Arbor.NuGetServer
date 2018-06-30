using System;
using System.Web.Hosting;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    [UsedImplicitly]
    public class ServerMapPath : IPathMapper
    {
        public string MapPath([NotNull] string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(relativePath));
            }

            return HostingEnvironment.MapPath(relativePath);
        }
    }
}
