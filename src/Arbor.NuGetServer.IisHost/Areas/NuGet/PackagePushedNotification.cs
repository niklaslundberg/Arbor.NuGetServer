using Arbor.NuGetServer.Core;
using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class PackagePushedNotification : INotification
    {
        public PackageIdentifier PackageIdentifier { get; }

        public PackagePushedNotification(PackageIdentifier packageIdentifier)
        {
            PackageIdentifier = packageIdentifier;
        }
    }
}