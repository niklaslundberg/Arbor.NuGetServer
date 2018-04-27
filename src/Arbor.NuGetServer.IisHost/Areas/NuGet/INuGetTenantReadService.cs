using System.Collections.Immutable;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public interface INuGetTenantReadService
    {
        ImmutableArray<NuGetTenant> GetNuGetTenants();
    }
}