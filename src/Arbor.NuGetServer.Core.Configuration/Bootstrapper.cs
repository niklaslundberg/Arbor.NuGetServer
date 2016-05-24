using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Arbor.NuGetServer.Core.Configuration.Modules;
using Arbor.NuGetServer.Core.Extensions;

using Autofac;
using Autofac.Core;

namespace Arbor.NuGetServer.Core.Configuration
{
    public static class Bootstrapper
    {
        public static IContainer Start(Func<IReadOnlyCollection<Assembly>> assemblyResolver)
        {
            if (assemblyResolver == null)
            {
                throw new ArgumentNullException(nameof(assemblyResolver));
            }

            var builder = new ContainerBuilder();

            IReadOnlyCollection<Assembly> assemblies = assemblyResolver().Where(assembly => !assembly.IsDynamic && assembly.GetName().Name.StartsWith("Arbor.NuGetServer")).ToArray();

            builder.RegisterInstance(assemblies).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(type => type.GetTypeInfo().IsPublicConcreteClassImplementing<MetaModule>())
                .As<MetaModule>();

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(
                    type =>
                    !type.GetTypeInfo().IsPublicConcreteClassImplementing<MetaModule>()
                    && type.GetTypeInfo().IsPublicConcreteClassImplementing<IModule>())
                .As<IModule>();

            IContainer container = builder.Build();

            var metaModules = container.Resolve<IEnumerable<MetaModule>>().ToList();

            foreach (MetaModule metaModule in metaModules)
            {
                metaModule.Configure(container.ComponentRegistry);
            }

            var standardModules = container.Resolve<IEnumerable<IModule>>().ToList();

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