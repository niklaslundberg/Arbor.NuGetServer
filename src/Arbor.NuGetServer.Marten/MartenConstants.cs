using Arbor.KVConfiguration.Core.Metadata;

namespace Arbor.NuGetServer.Marten
{
    public static class MartenConstants
    {
        public const string MartenConfigurationKey = "urn:arbor:nugetserver:marten";

        [Metadata(defaultValue: "false")]
        public const string DefaultEnabled = MartenConfigurationKey + ":default:enabled";
    }
}
