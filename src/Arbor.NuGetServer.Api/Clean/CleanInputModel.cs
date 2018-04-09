namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanInputModel
    {
        public bool Whatif { get; }

        public bool PreReleaseOnly { get; }

        public string PackageId { get; }

        public CleanInputModel(bool whatif = false, bool preReleaseOnly = true, string packageId = "")
        {
            Whatif = whatif;
            PreReleaseOnly = preReleaseOnly;
            PackageId = packageId;
        }
    }
}