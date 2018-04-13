using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arbor.NuGetServer.Core.Configuration.Modules;
using Autofac;
using Autofac.Integration.WebApi;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    [UsedImplicitly]
    public class WebApiModule : MetaModule
    {
        private readonly IReadOnlyCollection<Assembly> _assemblies;

        public WebApiModule(IReadOnlyCollection<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(_assemblies.ToArray());
        }
    }
}
