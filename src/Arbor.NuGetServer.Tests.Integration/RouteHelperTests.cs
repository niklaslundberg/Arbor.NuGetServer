using Arbor.NuGetServer.Core;
using Xunit;

namespace Arbor.NuGetServer.Tests.Integration
{
    public class RouteHelperTests
    {
        [Fact]
        public void WithParameterShouldDoNothingForLiteralRoute()
        {
            const string routePattern = "/nuget";

            string concreteRoute = routePattern.WithParameter("tenant", "abc");

            Assert.Equal("/nuget", concreteRoute);
        }

        [Fact]
        public void WithParameterShouldInsertValueToPlaceHolder()
        {
            const string routePattern = "/nuget/{tenant}/";

            string concreteRoute = routePattern.WithParameter("tenant", "abc");

            Assert.Equal("/nuget/abc/", concreteRoute);
        }
    }
}
