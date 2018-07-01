using Arbor.KVConfiguration.Urns;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Marten
{
    [Urn(MartenConstants.MartenConfigurationKey)]
    [UsedImplicitly]
    public class MartenConfiguration
    {
        public MartenConfiguration(string connectionString, bool enabled)
        {
            ConnectionString = connectionString;
            Enabled = enabled;
        }

        public string ConnectionString { get; }

        public bool Enabled { get; }
    }
}
