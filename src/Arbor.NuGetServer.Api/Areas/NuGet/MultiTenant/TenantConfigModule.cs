﻿using System;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.NuGetServer.Api.Areas.Application;
using Autofac;
using JetBrains.Annotations;
using Serilog;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    [UsedImplicitly]
    public class TenantConfigModule : MetaModule
    {
        private readonly bool _enabled;

        public TenantConfigModule([NotNull] IKeyValueConfiguration keyValueConfiguration, [NotNull] ILogger logger)
        {
            if (keyValueConfiguration == null)
            {
                throw new ArgumentNullException(nameof(keyValueConfiguration));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _enabled = keyValueConfiguration.ValueOrDefault(TenantConstants.ConfigurationSourceEnabled);
            logger.Debug("Tenant configuration source enabled: {Status}", _enabled);
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_enabled)
            {
                builder.RegisterType<ConfigurationNuGetTenantReadService>()
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
        }
    }
}
