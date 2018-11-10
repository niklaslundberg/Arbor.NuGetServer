using System.Web.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    [UsedImplicitly]
    public class TenantsAreaRegistration : AreaRegistration
    {
        public const string TenantsAreaName = nameof(Tenants);

        public override string AreaName => TenantsAreaName;

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
