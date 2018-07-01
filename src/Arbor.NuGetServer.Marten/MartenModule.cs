using System;
using Arbor.NuGetServer.Abstractions;
using Autofac;
using JetBrains.Annotations;
using Marten;

namespace Arbor.NuGetServer.Marten
{
    [UsedImplicitly]
    public class MartenModule : MetaModule
    {
        private readonly MartenConfiguration _martenConfiguration;

        public MartenModule([NotNull] MartenConfiguration martenConfiguration)
        {
            _martenConfiguration = martenConfiguration ?? throw new ArgumentNullException(nameof(martenConfiguration));
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (!_martenConfiguration.Enabled)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(_martenConfiguration.ConnectionString))
            {
                builder.RegisterType<MartenDb>().AsImplementedInterfaces().SingleInstance();

                builder.Register(context => DocumentStore.For(_martenConfiguration.ConnectionString))
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }
        }
    }
}
