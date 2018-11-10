using System;
using Arbor.NuGetServer.Api;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Xunit;

namespace Arbor.NuGetServer.Tests.Integration
{
    public class TenantRouteHelperTests
    {
        [Fact]
        public void ItShouldFindTenantInUrl()
        {
            var uri = new Uri("http://localhost/nuget/abc");

            string tenantIdFromUrl = uri.GetTenantIdFromUrl();

            Assert.Equal("abc", tenantIdFromUrl);
        }

        [Fact]
        public void ItShouldFindTenantInUrlWithMultiplePathSegments()
        {
            var uri = new Uri("http://localhost/nuget/abc/123");

            string tenantIdFromUrl = uri.GetTenantIdFromUrl();

            Assert.Equal("abc", tenantIdFromUrl);
        }

        [Fact]
        public void ItShouldFindTenantInUrlWithQuery()
        {
            var uri = new Uri("http://localhost/nuget/abc?id=123");

            string tenantIdFromUrl = uri.GetTenantIdFromUrl();

            Assert.Equal("abc", tenantIdFromUrl);
        }

        [Fact]
        public void ItShouldNotFindTenantInNonNuGetUrl()
        {
            var uri = new Uri("http://localhost/abc");

            string tenantIdFromUrl = uri.GetTenantIdFromUrl();

            Assert.Null(tenantIdFromUrl);
        }

        [Fact]
        public void ItShouldNotFindTenantInUrlWithoutTenant()
        {
            var uri = new Uri("http://localhost/nuget");

            string tenantIdFromUrl = uri.GetTenantIdFromUrl();

            Assert.Null(tenantIdFromUrl);
        }
    }
}