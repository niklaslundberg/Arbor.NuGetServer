using Arbor.NuGetServer.Core;
using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class PackagePushedNotification : INotification
    {
        public PackagePushedNotification(PackageIdentifier packageIdentifier)
        {
            PackageIdentifier = packageIdentifier;
        }

        public PackageIdentifier PackageIdentifier { get; }
    }
}
