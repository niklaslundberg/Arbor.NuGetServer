namespace Arbor.NuGetServer.IisHost.Areas.Clean
{
    public class CleanViewOutputModel
    {
        public CleanViewOutputModel(string cleanPostRoute, int packagesToKeep)
        {
            CleanPostRoute = cleanPostRoute;
            PackagesToKeep = packagesToKeep;
        }

        public string CleanPostRoute { get; }

        public int PackagesToKeep { get; }
    }
}
