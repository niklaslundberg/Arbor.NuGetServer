using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Application;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Security
{
    [UsedImplicitly]
    public class ProtectedRoutesModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public ProtectedRoutesModule(IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies.ToArray())
                .Where(type => type.GetTypeInfo().IsPublicConcreteClassImplementing<IProtectedRoute>()).As<IProtectedRoute>();
        }
    }
}
