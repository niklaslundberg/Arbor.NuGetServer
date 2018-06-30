using System;
using System.Collections.Immutable;
using System.Linq;
using System.Web.Mvc;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [RoutePrefix(TenantConstants.HttpGetRoute)]
    public class TenantsController : Controller
    {
        private readonly INuGetTenantReadService _tenantReadService;

        public TenantsController([NotNull] INuGetTenantReadService tenantReadService)
        {
            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
        }

        [Route("")]
        public ActionResult Index()
        {
            ImmutableArray<NuGetTenantStatistics> nuGetTenants = _tenantReadService
                .GetNuGetTenantIds()
                .Select(tenant => new NuGetTenantStatistics(tenant))
                .ToImmutableArray();

            return View(new TenantsOutputViewModel(nuGetTenants));
        }
    }
}
