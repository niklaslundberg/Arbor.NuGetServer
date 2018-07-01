using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using Autofac.Integration.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    [UsedImplicitly]
    public class MvcModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public MvcModule([NotNull] IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(_assemblies.ToArray());
            builder.RegisterModule<AutofacWebTypesModule>();
        }
    }
}
