using System;
using System.Collections.Immutable;
using System.Linq;
using System.Web.Mvc;
using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    [Authorize]
    [RouteArea(TenantsAreaRegistration.TenantsAreaName)]
    public class TenantsController : Controller
    {
        private readonly INuGetTenantReadService _tenantReadService;

        public TenantsController([NotNull] INuGetTenantReadService tenantReadService)
        {
            _tenantReadService = tenantReadService ?? throw new ArgumentNullException(nameof(tenantReadService));
        }

        [HttpGet]
        [Route(TenantRouteConstants.TenantsHttpGetRoute, Name = TenantRouteConstants.TenantsHttpGetRouteName)]
        public ActionResult Index()
        {
            ImmutableArray<NuGetTenantStatistics> nuGetTenants = _tenantReadService
                .GetNuGetTenantIds()
                .Select(tenant => new NuGetTenantStatistics(tenant))
                .ToImmutableArray();

            return View(new TenantsOutputViewModel(nuGetTenants));
        }

        [HttpGet]
        [Route(TenantRouteConstants.TenantHttpGetRoute, Name = TenantRouteConstants.TenantHttpGetRouteName)]
        public ActionResult Tenant(string tenant)
        {
            return View();
        }
    }
}
