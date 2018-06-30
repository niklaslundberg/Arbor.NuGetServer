using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Messaging
{
    [UsedImplicitly]
    public class PackagePushedNotificationHandler : INotificationHandler<PackagePushedNotification>
    {
        private readonly PackageNotificationQueueHandler _queueHandler;

        public PackagePushedNotificationHandler([NotNull] PackageNotificationQueueHandler queueHandler)
        {
            _queueHandler = queueHandler ?? throw new ArgumentNullException(nameof(queueHandler));
        }

        public Task Handle(PackagePushedNotification notification, CancellationToken cancellationToken)
        {
            _queueHandler.Enqueue(notification);

            return Task.CompletedTask;
        }
    }
}
