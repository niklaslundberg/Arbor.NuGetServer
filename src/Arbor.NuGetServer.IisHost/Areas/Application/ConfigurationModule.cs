using Arbor.KVConfiguration.Core;
using Autofac;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public class ConfigurationModule : Module
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public ConfigurationModule(IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_keyValueConfiguration).SingleInstance();
        }
    }
}