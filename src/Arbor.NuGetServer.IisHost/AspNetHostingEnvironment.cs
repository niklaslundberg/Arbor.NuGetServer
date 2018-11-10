using System.Web.Hosting;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.Test;

namespace Arbor.NuGetServer.IisHost
{
    public class AspNetHostingEnvironment : IHostingEnvironment
    {
        public void InitiateShutdown()
        {
            HostingEnvironment.InitiateShutdown();
        }
    }
}
