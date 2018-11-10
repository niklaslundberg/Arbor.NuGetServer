using System;
using System.Web.Mvc;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.Clean;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Routing;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [Authorize]
    public class CleanController : Controller
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public CleanController([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
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

            string cleanPostRoute = $"/{CleanConstants.PostRoute.WithParameter(CleanConstants.TenantRouteParameterName, tenantId.TenantId)}";

            return View(new CleanViewOutputModel(cleanPostRoute, packagesToKeep, true, true));
        }

        private NuGetTenantId ValidateTenant(string tenant)
        {
            // TODO add tenant validation
            return new NuGetTenantId(tenant);
        }
    }
}
