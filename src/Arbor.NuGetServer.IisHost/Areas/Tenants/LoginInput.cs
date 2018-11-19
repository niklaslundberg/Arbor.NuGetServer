namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class LoginInput
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string TenantId { get; set; }
    }
}
