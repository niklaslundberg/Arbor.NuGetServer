using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.IisHost.AspNet;
using Autofac.Core;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
{
    [UsedImplicitly]
    public class MvcApplication : HttpApplication
    {
        public static NuGetServerApp NuGetServerApp { get; private set; }

        protected void Application_Start()
        {
            IReadOnlyList<IModule> aspNetModules = new List<IModule> { new CustomAspNetModule(), new CacheModule() };
            NuGetServerApp = NuGetServerApp.Create(HostingEnvironment.QueueBackgroundWorkItem,
                aspNetModules,
                () => BuildManager.GetReferencedAssemblies().OfType<Assembly>().ToArray(), new Functions {MapPath = HostingEnvironment.MapPath});

            ApplicationHolder.App = NuGetServerApp;

            var appRegistered = new AppRegisteredAdapter(NuGetServerApp);

            HostingEnvironment.RegisterObject(appRegistered);

            NuGetServerApp.Start();

            MvcStartup.Start(NuGetServerApp);
        }

        protected void Application_End()
        {
            NuGetServerApp.SafeDispose();
            NuGetServerApp = null;
        }
    }
}
