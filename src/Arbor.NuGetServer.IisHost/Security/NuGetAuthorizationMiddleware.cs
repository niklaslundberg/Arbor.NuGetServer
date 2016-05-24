using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Owin;

namespace Arbor.NuGetServer.IisHost.Security
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
                await WriteBodyAsync(context);
                return;
            }

            if (context.Authentication.User.Identity == null)
            {
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (!context.Authentication.User.Identity.IsAuthenticated)
            {
                Challenge(context);
                await WriteBodyAsync(context);
                return;
            }

            if (context.Authentication.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = context.Authentication.User.Identity as ClaimsIdentity;

                if (claimsIdentity == null
                    || !claimsIdentity.Claims.Any(
                        claim => claim.Type == ClaimTypes.NameIdentifier && !string.IsNullOrWhiteSpace(claim.Value)))
                {
                    await WriteBodyAsync(context);

                    return;
                }
            }

            if (HttpContext.Current != null)
            {
                Thread.CurrentPrincipal = context.Authentication.User;
                HttpContext.Current.User = context.Authentication.User;
            }

            await Next.Invoke(context);
        }

        private static async Task WriteBodyAsync(IOwinContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "text/plain";

            using (var streamWriter = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, leaveOpen: true))
            {
                await streamWriter.WriteAsync("Unauthorized");
            }
        }

        private void Challenge(IOwinContext context)
        {
            context.Response.Headers.AppendValues("WWW-Authenticate", "basic");
        }
    }
}
