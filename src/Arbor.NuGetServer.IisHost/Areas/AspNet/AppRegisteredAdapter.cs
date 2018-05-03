using System;
using System.Web.Hosting;
using Arbor.NuGetServer.IisHost.Areas.Application;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class AppRegisteredAdapter : IRegisteredObject
    {
        private readonly NuGetServerApp _app;

        public AppRegisteredAdapter([NotNull] NuGetServerApp app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        public void Stop(bool immediate)
        {
            _app.Stop();
        }
    }
}
