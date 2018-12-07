using System;
using System.Diagnostics;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public class NuGetTenantModule : Module
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public NuGetTenantModule([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            bool defaultEnabled = Debugger.IsAttached;

            if (_keyValueConfiguration.ValueOrDefault(TenantConstants.InMemorySourceEnabled, defaultValue: defaultEnabled))
            {
                builder.RegisterType<InMemoryNuGetTenantReadService>()
                    .IfNotRegistered(typeof(INuGetTenantReadService))
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
        }
    }
}
