using System.Web.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class NuGetAreaRegistration : AreaRegistration
    {
        public const string NuGetAreaName = nameof(NuGet);

        public override string AreaName => NuGetAreaName;

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
