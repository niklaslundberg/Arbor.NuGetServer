using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Arbor.NuGetServer.Core.Configuration;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Configuration;
using Arbor.NuGetServer.IisHost.DataServices;

using Autofac;
using Autofac.Integration.Mvc;

namespace Arbor.NuGetServer.IisHost
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ConfigurationStartup.Start();

            Func<IReadOnlyCollection<Assembly>> assemblyResolver = () => BuildManager.GetReferencedAssemblies()
                                                                             .OfType<Assembly>()
                                                                             .SafeToReadOnlyCollection();

            IContainer container = Bootstrapper.Start(assemblyResolver);

            NuGetRoutes.Configure();

            GlobalConfiguration.Configure(configuration => WebApiConfig.Register(configuration, container));

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}