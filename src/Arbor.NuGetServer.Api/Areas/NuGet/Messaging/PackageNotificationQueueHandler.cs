using System;
using System.Collections.Concurrent;
using System.Threading;
using Arbor.NuGetServer.Api.Areas.WebHooks;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Messaging
{
    [UsedImplicitly]
    public class PackageNotificationQueueHandler
    {
        private readonly BlockingCollection<IPackageNotification> _queue;

        public PackageNotificationQueueHandler()
        {
            _queue = new BlockingCollection<IPackageNotification>();
        }

        public void Enqueue([NotNull] IPackageNotification packagePushedNotification)
        {
            if (packagePushedNotification == null)
            {
                throw new ArgumentNullException(nameof(packagePushedNotification));
            }

            _queue.Add(packagePushedNotification);
        }

        public IPackageNotification Take(CancellationToken cancellationToken = default)
        {
            return _queue.Take(cancellationToken);
        }
    }
}
