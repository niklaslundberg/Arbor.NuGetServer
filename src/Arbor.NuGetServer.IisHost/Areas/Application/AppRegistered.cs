using System;
using System.Web.Hosting;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public class AppRegistered : IRegisteredObject
    {
        private readonly NuGetServerApp _app;

        public AppRegistered([NotNull] NuGetServerApp app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        public void Stop(bool immediate)
        {
            _app.Stop();
        }
    }
}
