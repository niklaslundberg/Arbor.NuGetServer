using System.Web.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    [UsedImplicitly]
    public class CleanAreaRegistration : AreaRegistration
    {
        public const string CleanAreaName = nameof(Clean);

        public override string AreaName => CleanAreaName;

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
