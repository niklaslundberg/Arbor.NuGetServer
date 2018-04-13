using System.Diagnostics;
using System.Web;
using System.Web.Hosting;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Arbor.NuGetServer.IisHost.Areas.AspNet;

namespace Arbor.NuGetServer.IisHost
{
    public class MvcApplication : HttpApplication
    {
        private static NuGetServerApp _nuGetServerApp;

        public static NuGetServerApp NuGetServerApp => _nuGetServerApp;

        protected void Application_Start()
        {
            Debug.WriteLine("Application start");
            _nuGetServerApp = NuGetServerApp.Create(HostingEnvironment.QueueBackgroundWorkItem);

            var appRegistered = new AppRegistered(_nuGetServerApp);

            HostingEnvironment.RegisterObject(appRegistered);

            _nuGetServerApp.Start();

            MvcStartup.Start(_nuGetServerApp);
        }

        protected void Application_End()
        {
            SafeDispose.Dispose(_nuGetServerApp);
            _nuGetServerApp = null;
            Debug.WriteLine("Shut down, application end");
        }
    }
}
