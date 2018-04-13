using Arbor.NuGetServer.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Clean
{
    [UsedImplicitly]
    public class CleanProtectedRoute : IProtectedRoute
    {
        public string Route => CleanConstants.PostRoute;
    }
}
