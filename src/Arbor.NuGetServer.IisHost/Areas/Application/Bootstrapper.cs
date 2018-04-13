using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Configuration.Modules;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Logging;
using Autofac;
using Autofac.Core;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public static class Bootstrapper
    {
        public static IContainer Start(
            Func<IReadOnlyCollection<Assembly>> assemblyResolver,
            ILogger logger,
            IKeyValueConfiguration keyValueConfiguration)
        {
            if (assemblyResolver == null)
            {
                throw new ArgumentNullException(nameof(assemblyResolver));
            }

            var builder = new ContainerBuilder();

            IReadOnlyCollection<Assembly> assemblies = assemblyResolver()
                .Where(assembly => !assembly.IsDynamic && assembly.GetName().Name
                                       .StartsWith("Arbor.NuGetServer", StringComparison.OrdinalIgnoreCase)).ToArray();

            builder.RegisterInstance(assemblies).AsImplementedInterfaces();

            builder.RegisterModule(new LoggingModule(logger));
            builder.RegisterModule(new ConfigurationModule(keyValueConfiguration));

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(type => type.GetTypeInfo().IsPublicConcreteClassImplementing<MetaModule>())
                .As<MetaModule>();

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(
                    type =>
                        !type.GetTypeInfo().IsPublicConcreteClassImplementing<MetaModule>()
                        && type.GetTypeInfo().IsPublicConcreteClassImplementing<IModule>()
                        && type != typeof(LoggingModule)
                        && type != typeof(ConfigurationModule))
                .As<IModule>();

            IContainer container = builder.Build();

            List<MetaModule> metaModules = container.Resolve<IEnumerable<MetaModule>>().ToList();

            foreach (MetaModule metaModule in metaModules)
            {
                metaModule.Configure(container.ComponentRegistry);
            }

            List<IModule> standardModules = container.Resolve<IEnumerable<IModule>>().ToList();

            foreach (IModule module in standardModules)
            {
                module.Configure(container.ComponentRegistry);
            }

            var updateBuilder = new ContainerBuilder();

            updateBuilder.RegisterInstance(container);

            updateBuilder.Update(container.ComponentRegistry);

            return container;
        }
    }
}
