using System;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Routing;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [RouteArea(CleanAreaRegistration.CleanAreaName)]
    [Authorize]
    public class CleanController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly Functions _functions;

        public CleanController([NotNull] IKeyValueConfiguration keyValueConfiguration, Functions functions)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _functions = functions;
        }

        [Route(CleanConstants.CleanGetRoute, Name = CleanConstants.CleanGetRouteName)]
        [HttpGet]
        public ActionResult Index(string tenant)
        {
            int packagesToKeep;

            NuGetTenantId tenantId = ValidateTenant(tenant);

            if (!int.TryParse(_keyValueConfiguration[CleanConstants.PackagesToKeepKey],
                    out int packagesToKeepFromConfig) || packagesToKeepFromConfig <= 0)
            {
                packagesToKeep = CleanConstants.DefaultValues.PackagesToKeep;
            }
            else
            {
                packagesToKeep = packagesToKeepFromConfig;
            }

            string cleanPostRoute = _functions.MapPath($"{CleanConstants.PostRoute.WithParameter(CleanConstants.TenantRouteParameterName, tenantId.TenantId)}");

            return View(new CleanViewOutputModel(cleanPostRoute, packagesToKeep, true, true));
        }

        private NuGetTenantId ValidateTenant(string tenant)
        {
            // TODO add tenant validation
            return new NuGetTenantId(tenant);
        }
    }
}
