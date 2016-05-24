using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

namespace Arbor.NuGetServer.IisHost.Configuration
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IContainer container)
        {
            config.MapHttpAttributeRoutes();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}