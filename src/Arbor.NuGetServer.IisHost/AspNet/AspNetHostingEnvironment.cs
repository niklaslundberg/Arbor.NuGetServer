using System.Web.Hosting;
using Arbor.NuGetServer.Api.Areas.Application;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class AspNetHostingEnvironment : IHostingEnvironment
    {
        public void InitiateShutdown()
        {
            HostingEnvironment.InitiateShutdown();
        }
    }
}
