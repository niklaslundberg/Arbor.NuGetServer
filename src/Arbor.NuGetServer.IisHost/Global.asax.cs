using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.NuGetServer.Api.Clean;
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

            IReadOnlyCollection<Assembly> AssemblyResolver() => BuildManager.GetReferencedAssemblies()
                .OfType<Assembly>()
                .SafeToReadOnlyCollection();

            IContainer container = Bootstrapper.Start(AssemblyResolver);

            if (StaticKeyValueConfigurationManager.AppSettings.ValueOrDefault(CleanConstants.CleanOnStartEnabled))
            {
                ILifetimeScope beginLifetimeScope = container.BeginLifetimeScope();

                var cleanService = beginLifetimeScope.Resolve<CleanService>();

                cleanService.CleanBinFiles(whatIf: false);
            }

            string packagesPath = StaticKeyValueConfigurationManager.AppSettings[ConfigurationKeys.PackagesDirectoryPath];

            if (!string.IsNullOrWhiteSpace(packagesPath))
            {
                if (packagesPath.StartsWith("~"))
                {
                   packagesPath = Server.MapPath(packagesPath);
                }

                var packagesDirectory = new DirectoryInfo(packagesPath);

                if (!packagesDirectory.Exists)
                {
                    packagesDirectory.Create();
                }
            }
            
            GlobalConfiguration.Configure(configuration => WebApiConfig.Register(configuration, container));

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
