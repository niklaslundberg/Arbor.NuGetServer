using System;
using Arbor.NuGetServer.Api.Areas.Security;

namespace Arbor.NuGetServer.Tests.Integration.Helpers
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
