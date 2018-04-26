using System.Diagnostics;
using System.Web;
using System.Web.Hosting;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.AspNet;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [UsedImplicitly]
    public class MvcApplication : HttpApplication
    {
        public static NuGetServerApp NuGetServerApp { get; private set; }

        protected void Application_Start()
        {
            Debug.WriteLine("Application start");
            NuGetServerApp = NuGetServerApp.Create(HostingEnvironment.QueueBackgroundWorkItem);

            var appRegistered = new AppRegisteredAdapter(NuGetServerApp);

            HostingEnvironment.RegisterObject(appRegistered);

            NuGetServerApp.Start();

            MvcStartup.Start(NuGetServerApp);
        }

        protected void Application_End()
        {
            NuGetServerApp.SafeDispose();
            NuGetServerApp = null;
            Debug.WriteLine("Shut down, application end");
        }
    }
}
