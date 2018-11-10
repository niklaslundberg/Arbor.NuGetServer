using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    [UsedImplicitly]
    public class BackgroundServiceModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public BackgroundServiceModule([NotNull] IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .Where(type => type.IsPublicConcreteClassImplementing<IHostedService>()).As<IHostedService>()
                .SingleInstance();
        }
    }
}
