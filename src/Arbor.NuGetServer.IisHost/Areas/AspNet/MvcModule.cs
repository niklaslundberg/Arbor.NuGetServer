using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Core.Configuration.Modules;
using Autofac;
using Autofac.Integration.Mvc;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    [UsedImplicitly]
    public class MvcModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public MvcModule(IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(_assemblies.ToArray());
            builder.RegisterModule<AutofacWebTypesModule>();
        }
    }
}