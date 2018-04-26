using System;
using Arbor.NuGetServer.Core;
using JetBrains.Annotations;
using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class PackagePushedNotification : INotification
    {
        public PackagePushedNotification([NotNull] PackageIdentifier packageIdentifier)
        {
            PackageIdentifier = packageIdentifier ?? throw new ArgumentNullException(nameof(packageIdentifier));
        }

        public PackageIdentifier PackageIdentifier { get; }
    }
}
