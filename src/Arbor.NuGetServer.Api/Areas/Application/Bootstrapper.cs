using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Arbor.NuGetServer.Api.Areas.Logging;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Core.Extensions;
using Autofac;
using Autofac.Core;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    public static class Bootstrapper
    {
        public static AppContainer Start(
            Func<ImmutableArray<Assembly>> assemblyResolver,
            [NotNull] ILogger logger,
            [NotNull] MultiSourceKeyValueConfiguration keyValueConfiguration,
            [NotNull] IReadOnlyList<IModule> modulesToRegister)
        {
            if (assemblyResolver == null)
            {
                throw new ArgumentNullException(nameof(assemblyResolver));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (keyValueConfiguration == null)
            {
                throw new ArgumentNullException(nameof(keyValueConfiguration));
            }

            if (modulesToRegister == null)
            {
                throw new ArgumentNullException(nameof(modulesToRegister));
            }

            var builder = new ContainerBuilder();

            ImmutableArray<Assembly> appAssemblies = assemblyResolver();

            Assembly[] assemblies = appAssemblies
                .Where(assembly => !assembly.IsDynamic && assembly.GetName().Name
                                       .StartsWith("Arbor.NuGetServer", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var modules = new List<IModule>
            {
                new LoggingModule(logger),
                new ConfigurationModule(keyValueConfiguration),
                new KeyValueConfigurationModule(keyValueConfiguration, logger),
                new NuGetTenantModule(),
                new UrnConfigurationModule(keyValueConfiguration, logger, appAssemblies)
            };

            builder.RegisterInstance(assemblies).AsImplementedInterfaces();

            foreach (IModule module in modules)
            {
                builder.RegisterModule(module);
            }

            foreach (IModule module in modulesToRegister)
            {
                builder.RegisterModule(module);
            }

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

                scopeBuilder.RegisterAssemblyTypes(assemblies)
                    .Where(
                        type =>
                            type.IsPublicConcreteClassImplementing<AppModule>())
                    .As<AppModule>();
            });

            var appModules = appScope.Resolve<IReadOnlyCollection<AppModule>>();

            ILifetimeScope subScope = appScope.BeginLifetimeScope(scopeBuilder =>
            {
                foreach (AppModule appModule in appModules)
                {
                    scopeBuilder.RegisterModule(appModule);
                }
            });

            return new AppContainer(container, appScope, subScope);
        }
    }
}
