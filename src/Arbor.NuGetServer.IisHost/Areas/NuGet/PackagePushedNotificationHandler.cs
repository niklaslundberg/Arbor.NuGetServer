using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class PackagePushedNotificationHandler : INotificationHandler<PackagePushedNotification>
    {
        private readonly PackagePushedQueueHandler _queueHandler;

        public PackagePushedNotificationHandler([NotNull] PackagePushedQueueHandler queueHandler)
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
