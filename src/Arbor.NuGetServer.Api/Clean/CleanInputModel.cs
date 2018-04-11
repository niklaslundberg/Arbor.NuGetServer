namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanInputModel
    {
        public CleanInputModel(
            bool whatif = false,
            bool preReleaseOnly = true,
            string packageId = "",
            int packagesToKeep = -1)
        {
            PackagesToKeep = packagesToKeep;
            Whatif = whatif;
            PreReleaseOnly = preReleaseOnly;
            PackageId = packageId;
        }

        public bool Whatif { get; }

        public bool PreReleaseOnly { get; }

        public string PackageId { get; }

        public int PackagesToKeep { get; }
    }
}
