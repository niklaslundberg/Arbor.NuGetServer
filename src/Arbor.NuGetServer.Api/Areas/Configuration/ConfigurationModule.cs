using System;
using Arbor.KVConfiguration.Core;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public class ConfigurationModule : Module
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public ConfigurationModule([NotNull] IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_keyValueConfiguration).SingleInstance();
        }
    }
}
