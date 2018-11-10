using System.Web.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Start
{
    [UsedImplicitly]
    public class StartAreaRegistration : AreaRegistration
    {
        public const string StartAreaName = nameof(Start);

        public override string AreaName => StartAreaName;

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
