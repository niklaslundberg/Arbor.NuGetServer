namespace Arbor.NuGetServer.Api.Clean
{
    public class CleanedPackage
    {
        private readonly string _fullName;

        public CleanedPackage(string fullName)
        {
            _fullName = fullName;
        }
    }
}