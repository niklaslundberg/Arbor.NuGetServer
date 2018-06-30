using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using JetBrains.Annotations;
using MediatR;

namespace Arbor.NuGetServer.Api.Areas.MediatR
{
    [UsedImplicitly]
    public class MediatRModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public MediatRModule([NotNull] IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            builder
                .Register<SingleInstanceFactory>(ctx =>
                {
                    var componentContext = ctx.Resolve<IComponentContext>();
                    return serviceType =>
                        componentContext.TryResolve(serviceType, out object instance) ? instance : null;
                })
                .InstancePerLifetimeScope();

            builder
                .Register<MultiInstanceFactory>(ctx =>
                {
                    var componentContext = ctx.Resolve<IComponentContext>();
                    return typeArguments =>
                        (IEnumerable<object>)componentContext.Resolve(
                            typeof(IEnumerable<>).MakeGenericType(typeArguments));
                })
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .Where(type => type.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        currentInterface => currentInterface.IsGenericType
                                            && currentInterface.GetGenericTypeDefinition()
                                            == typeof(INotificationHandler<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .Where(type => type.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        currentInterface => currentInterface.IsGenericType
                                            && currentInterface.GetGenericTypeDefinition()
                                            == typeof(IRequestHandler<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
