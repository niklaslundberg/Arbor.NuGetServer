using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Core.Extensions;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    [UsedImplicitly]
    public class BackgroundServiceModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public BackgroundServiceModule(IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .Where(type => type.IsPublicConcreteClassImplementing<IHostedService>()).As<IHostedService>()
                .SingleInstance();
        }
    }
}
