using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Logging;
using Autofac;
using Autofac.Core;
using Serilog;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public static class Bootstrapper
    {
        public static AppContainer Start(
            Func<ImmutableArray<Assembly>> assemblyResolver,
            ILogger logger,
            IKeyValueConfiguration keyValueConfiguration)
        {
            if (assemblyResolver == null)
            {
                throw new ArgumentNullException(nameof(assemblyResolver));
            }

            var builder = new ContainerBuilder();

            Assembly[] assemblies = assemblyResolver()
                .Where(assembly => !assembly.IsDynamic && assembly.GetName().Name
                                       .StartsWith("Arbor.NuGetServer", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            builder.RegisterInstance(assemblies).AsImplementedInterfaces();
            builder.RegisterModule(new LoggingModule(logger));
            builder.RegisterModule(new ConfigurationModule(keyValueConfiguration));

            builder.RegisterAssemblyTypes(assemblies)
                .Where(type => type.IsPublicConcreteClassImplementing<MetaModule>())
                .As<MetaModule>();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(
                    type =>
                        !type.IsPublicConcreteClassImplementing<MetaModule>()
                        && type.IsPublicConcreteClassImplementing<IModule>()
                        && type != typeof(LoggingModule)
                        && type != typeof(ConfigurationModule))
                .As<IModule>();

            IContainer container = builder.Build();

            List<MetaModule> metaModules = container.Resolve<IEnumerable<MetaModule>>().ToList();

            ILifetimeScope appScope = container.BeginLifetimeScope(scopeBuilder =>
            {
                foreach (MetaModule metaModule in metaModules)
                {
                    scopeBuilder.RegisterModule(metaModule);
                }
            });

            return new AppContainer(container, appScope);
        }
    }
}
