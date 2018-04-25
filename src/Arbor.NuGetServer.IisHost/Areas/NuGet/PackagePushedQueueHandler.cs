﻿using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class PackagePushedQueueHandler
    {
        private BlockingCollection<PackagePushedNotification> _queue;

        public PackagePushedQueueHandler()
        {
            _queue = new BlockingCollection<PackagePushedNotification>();
        }

        public void Enqueue(PackagePushedNotification packagePushedNotification)
        {
            _queue.Add(packagePushedNotification);
        }

        public PackagePushedNotification Take(CancellationToken cancellationToken = default)
        {
            return _queue.Take(cancellationToken);
        }
    }
}
