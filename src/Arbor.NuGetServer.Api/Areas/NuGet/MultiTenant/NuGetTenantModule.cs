using System;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.NuGetServer.Abstractions;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public class NuGetTenantModule : MetaModule
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public NuGetTenantModule([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        protected override void Load(ContainerBuilder builder)
        {

            if (_keyValueConfiguration.ValueOrDefault(TenantConstants.InMemorySourceEnabled, defaultValue: true))
            {
                builder.RegisterType<InMemoryNuGetTenantReadService>()
                    .IfNotRegistered(typeof(INuGetTenantReadService))
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
        }
    }
}
