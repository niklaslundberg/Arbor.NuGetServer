using System.Threading.Tasks;
using Microsoft.Owin;
using Thinktecture.IdentityModel.Owin;

namespace Arbor.NuGetServer.IisHost
{
    public class NuGetAuthorizationMiddleware : OwinMiddleware
    {
        public NuGetAuthorizationMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Authentication.User == null)
            {
                Challenge(context);
                return;
            }

            if (context.Authentication.User.Identity == null)
            {
                Challenge(context);
                return;
            }

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                Challenge(context);
                return;
            }

            await Next.Invoke(context);
        }

        void Challenge(IOwinContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.AppendValues("WWW-Authenticate", new string[1] {"basic"});
            //context.Authentication.Challenge("basic");
        }
    }
}