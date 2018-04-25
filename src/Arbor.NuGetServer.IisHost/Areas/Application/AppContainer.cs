using System;
using Arbor.NuGetServer.Core.Extensions;
using Autofac;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public sealed class AppContainer : IDisposable
    {
        public AppContainer([NotNull] IContainer container, [NotNull] ILifetimeScope appScope)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            AppScope = appScope ?? throw new ArgumentNullException(nameof(appScope));
        }

        public IContainer Container { get; }

        public ILifetimeScope AppScope { get; }

        public void Dispose()
        {
            AppScope.SafeDispose();
            Container.SafeDispose();
        }
    }
}
