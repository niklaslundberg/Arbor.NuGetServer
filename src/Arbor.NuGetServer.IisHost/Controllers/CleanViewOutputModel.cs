namespace Arbor.NuGetServer.IisHost.Controllers
{
    public class CleanViewOutputModel
    {
        public string CleanPostRoute { get; }

        public CleanViewOutputModel(string cleanPostRoute)
        {
            CleanPostRoute = cleanPostRoute;
        }
    }
}