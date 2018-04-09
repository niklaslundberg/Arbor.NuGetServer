using System.Web.Http;
using Arbor.WebApi.Formatting.HtmlForms;
using Autofac;
using Autofac.Integration.WebApi;

namespace Arbor.NuGetServer.IisHost.Configuration
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IContainer container)
        {
            config.MapHttpAttributeRoutes();

            config.Formatters.Insert(0, new XWwwFormUrlEncodedFormatter());

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
