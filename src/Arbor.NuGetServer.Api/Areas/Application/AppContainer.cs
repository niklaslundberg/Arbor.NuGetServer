using System;
using Arbor.NuGetServer.Core.Extensions;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    public sealed class AppContainer : IDisposable
    {
        public AppContainer(
            [NotNull] IContainer container,
            [NotNull] ILifetimeScope appScope,
            [NotNull] ILifetimeScope subScope)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            AppScope = appScope ?? throw new ArgumentNullException(nameof(appScope));
            SubScope = subScope ?? throw new ArgumentNullException(nameof(subScope));
        }

        public IContainer Container { get; }

        public ILifetimeScope AppScope { get; }

        public ILifetimeScope SubScope { get; }

        public void Dispose()
        {
            SubScope.SafeDispose();
            AppScope.SafeDispose();
            Container.SafeDispose();
        }
    }
}
