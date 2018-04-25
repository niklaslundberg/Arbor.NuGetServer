using System;
using Autofac;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public class AppContainer : IDisposable
    {
        public AppContainer(IContainer container, ILifetimeScope appScope)
        {
            Container = container;
            AppScope = appScope;
        }

        public IContainer Container { get; }

        public ILifetimeScope AppScope { get; }

        public void Dispose()
        {
            AppScope?.Dispose();
            Container?.Dispose();
        }
    }
}
