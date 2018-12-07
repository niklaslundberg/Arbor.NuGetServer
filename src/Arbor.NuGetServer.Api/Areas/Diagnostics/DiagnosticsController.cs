using System;
using System.Linq;
using System.Web.Http;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Application;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Arbor.NuGetServer.Api.Areas.Diagnostics
{
    [Authorize]
    public class DiagnosticsController : ApiController
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;
        private readonly Functions _functions;

        public DiagnosticsController([NotNull] IKeyValueConfiguration keyValueConfiguration, Functions functions)
        {
            _keyValueConfiguration =
                keyValueConfiguration ?? throw new ArgumentNullException(nameof(keyValueConfiguration));
            _functions = functions;
        }

        [HttpGet]
        [Route("~/diagnostics")]
        public object Index()
        {
            var multiSourceKeyValueConfiguration = (MultiSourceKeyValueConfiguration)_keyValueConfiguration;

            var data = new
            {
                BaseDirectory = _functions.MapPath("~/"),
                Sources = multiSourceKeyValueConfiguration.SourceChain,
                Values = multiSourceKeyValueConfiguration.AllKeys
                    .OrderBy(key => key)
                    .Select(key => new
                    {
                        Key = key,
                        Value = multiSourceKeyValueConfiguration[key],
                        Source = multiSourceKeyValueConfiguration.ConfiguratorFor(key).ToString()
                    })
            };

            return Json(data, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
