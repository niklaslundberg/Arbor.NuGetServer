using System;
using Arbor.NuGetServer.Abstractions;

namespace Arbor.NuGetServer.Tests.Integration
{
    public static class TestKeys
    {
        public const string TestAudience = "TestAudience";

        public const string TestIssuer = "TestIssuer";

        public static readonly RsaKey TestKey = RsaKeyHelper.CreateKey();

        public static readonly TokenConfiguration TestConfiguration = new TokenConfiguration(TimeSpan.FromMinutes(5),
            new TokenAudience(TestAudience),
            new TokenIssuer(TestIssuer),
            TestKey);
    }
}
