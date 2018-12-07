using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.Diagnostics;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using Arbor.NuGetServer.IisHost.Areas.Start;
using Xunit;
using AllowAnonymousAttribute = System.Web.Mvc.AllowAnonymousAttribute;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace Arbor.NuGetServer.Tests.Integration.Authorization
{
    public class ExplicitAuthorize
    {
        public static IEnumerable<object[]> MvcControllers()
        {
            Console.WriteLine(typeof(HomeController).FullName);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.FullName.StartsWith("Arbor.NuGet")))
            {
                foreach (Type type in assembly.GetTypes()
                    .Where(type => type.IsPublicConcreteClassImplementing<Controller>()))
                {
                    yield return new object[] { type.Name, type.AssemblyQualifiedName };
                }
            }
        }

        public static IEnumerable<object[]> ApiControllers()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.FullName.StartsWith("Arbor.NuGet")))
            {
                foreach (Type type in assembly.GetTypes()
                    .Except(new[] { typeof(NuGetFeedController) })
                    .Where(type => type.IsPublicConcreteClassImplementing<ApiController>()))
                {
                    yield return new object[] { type.Name, type.AssemblyQualifiedName };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MvcControllers))]
        public void MvcControllersMustHaveExplicitAuthorizeOrAllowAnonymous(
            string controllerType,
            string assemblyQualifiedName)
        {
            Console.WriteLine(typeof(HomeController).FullName);
            Type type = Type.GetType(assemblyQualifiedName);

            Assert.NotNull(type);

            var authorizeAttribute = type.GetCustomAttribute<AuthorizeAttribute>();
            var allowAnonymousAttribute = type.GetCustomAttribute<AllowAnonymousAttribute>();
            Assert.True((authorizeAttribute != null) ^ (allowAnonymousAttribute != null));
        }

        [Theory]
        [MemberData(nameof(ApiControllers))]
        public void WebApiControllersMustHaveExplicitAuthorizeOrAllowAnonymous(
            string controllerType,
            string assemblyQualifiedName)
        {
            Console.WriteLine(typeof(DiagnosticsController).FullName);

            Type type = Type.GetType(assemblyQualifiedName);

            Assert.NotNull(type);

            var authorizeAttribute = type.GetCustomAttribute<System.Web.Http.AuthorizeAttribute>();
            var allowAnonymousAttribute = type.GetCustomAttribute<System.Web.Http.AllowAnonymousAttribute>();
            Assert.True((authorizeAttribute != null) ^ (allowAnonymousAttribute != null));
        }
    }
}
