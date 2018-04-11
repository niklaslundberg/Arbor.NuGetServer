using System.Web.Http;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.KVConfiguration.Core.Extensions.StringExtensions;
using Arbor.WebApi.Formatting.HtmlForms;
using Autofac;
using Autofac.Integration.WebApi;
using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2;

namespace Arbor.NuGetServer.IisHost.Configuration
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IContainer container)
        {
            config.MapHttpAttributeRoutes();

            config.Formatters.Insert(0, new XWwwFormUrlEncodedFormatter());

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            using (ILifetimeScope rootScope = container.BeginLifetimeScope())
            {
                var nuGetFeedConfiguration = rootScope.Resolve<NuGetFeedConfiguration>();

                config.UseNuGetV2WebApiFeed(nuGetFeedConfiguration.RouteName,
                    nuGetFeedConfiguration.RouteUrl,
                    nuGetFeedConfiguration.ControllerName);
            }
        }
    }

    public class KeyValueSettingsProvider : ISettingsProvider
    {
        private readonly IKeyValueConfiguration _appSettings;

        public KeyValueSettingsProvider(IKeyValueConfiguration appSettings)
        {
            _appSettings = appSettings;
        }

        public bool GetBoolSetting(string key, bool defaultValue)
        {
            return _appSettings.ValueOrDefault(key, defaultValue);
        }

        public string GetStringSetting(string key, string defaultValue)
        {
            return _appSettings.ValueOrDefault(key, defaultValue);
        }
    }

    public class NuGetFeedConfiguration
    {
        public NuGetFeedConfiguration(
            string routeName,
            string routeUrl,
            string controllerName,
            IServerPackageRepository repository,
            string apiKey)
        {
            RouteName = routeName;
            RouteUrl = routeUrl;
            ControllerName = controllerName;
            Repository = repository;
            ApiKey = apiKey;
        }

        public string RouteName { get; }
        public string RouteUrl { get; }
        public string ControllerName { get; }
        public IServerPackageRepository Repository { get; }
        public string ApiKey { get; }
    }
}
