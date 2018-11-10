using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Abstractions;
using Autofac;
using Autofac.Integration.WebApi;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    [UsedImplicitly]
    public class WebApiModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public WebApiModule([NotNull] IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(_assemblies.ToArray());
        }
    }
}
