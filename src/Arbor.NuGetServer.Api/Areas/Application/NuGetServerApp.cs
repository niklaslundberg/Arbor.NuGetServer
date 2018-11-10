using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.NuGetServer.Api.Areas.Clean;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.Configuration;
using Autofac;
using Autofac.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace Arbor.NuGetServer.Api.Areas.Application
{
    public sealed class NuGetServerApp : IDisposable
    {
        private readonly Action<Func<CancellationToken, Task>> _backgroundServiceHandler;
        private readonly List<IHostedService> _backgroundServices;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Logger _logger;
        private AppContainer _container;
        private bool _isRunning;
        private bool _isStopped;

        private NuGetServerApp(
            IKeyValueConfiguration keyValueConfiguration,
            AppContainer container,
            Logger logger,
            Action<Func<CancellationToken, Task>> backgroundServiceHandler,
            IReadOnlyCollection<IHostedService> backgroundServices)
        {
            KeyValueConfiguration = keyValueConfiguration;
            _container = container;
            _logger = logger;
            _backgroundServiceHandler = backgroundServiceHandler;
            _backgroundServices = backgroundServices.ToList();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool Disposed { get; private set; }

        public IKeyValueConfiguration KeyValueConfiguration { get; }

        public ILifetimeScope LifetimeScope
        {
            get
            {
                CheckState();
                return _container.SubScope;
            }
        }

        public static NuGetServerApp Create(
            Action<Func<CancellationToken, Task>> backgroundServiceHandler,
            IReadOnlyList<IModule> modules,
            Func<Assembly[]> assemblyResolver)
        {
            Logger logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.Seq("http://localhost:5341")
                .MinimumLevel.Debug()
                .CreateLogger();

            ImmutableArray<Assembly> AssemblyResolver() => assemblyResolver()
                .SafeToImmutableArray();

            MultiSourceKeyValueConfiguration keyValueConfiguration = ConfigurationInitialization.InitializeConfiguration(AssemblyResolver());

            AppContainer container = Bootstrapper.Start(AssemblyResolver, logger, keyValueConfiguration, modules);

            var backgroundServices = new List<IHostedService>();

            if (keyValueConfiguration.ValueOrDefault(CleanConstants.CleanOnStartEnabled))
            {
                var cleanService = container.Container.Resolve<CleanService>();

                cleanService.CleanBinFiles(false);
            }

            var collection = container.Container.Resolve<IEnumerable<IHostedService>>();

            backgroundServices.AddRange(collection);

            logger.Information("Added background services {BackgroundServices}",
                backgroundServices
                    .Select(s => s.GetType().FullName)
                    .ToArray());

            string packagesPath =
                keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

            if (!string.IsNullOrWhiteSpace(packagesPath))
            {
                if (packagesPath.StartsWith("~", StringComparison.OrdinalIgnoreCase))
                {
                    packagesPath = container.Container.Resolve<IPathMapper>().MapPath(packagesPath);
                }

                if (packagesPath != null)
                {
                    var packagesDirectory = new DirectoryInfo(packagesPath);

                    if (!packagesDirectory.Exists)
                    {
                        packagesDirectory.Create();
                    }
                }
            }

            var app = new NuGetServerApp(keyValueConfiguration,
                container,
                logger,
                backgroundServiceHandler,
                backgroundServices);

            return app;
        }

        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel(false);
            }

            foreach (IHostedService backgroundService in _backgroundServices)
            {
                if (backgroundService is IDisposable disposable)
                {
                    disposable.SafeDispose();
                }
            }

            _container.SafeDispose();
            _logger.SafeDispose();
            _container = null;
            Disposed = true;
        }

        public void Stop()
        {
            if (_isStopped)
            {
                return;
            }

            CheckState();

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _isRunning = false;
            _isStopped = true;
        }

        public void Start()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Could not start when already started");
            }

            if (_isStopped)
            {
                throw new InvalidOperationException("Could not start when already stopped");
            }

            CheckState();

            foreach (IHostedService backgroundService in _backgroundServices)
            {
                _backgroundServiceHandler.Invoke(token =>
                {
                    CancellationToken linkedToken = CancellationTokenSource
                        .CreateLinkedTokenSource(token, _cancellationTokenSource.Token).Token;

                    _logger.Information("Starting background service {BackgroundService}",
                        backgroundService.GetType().Name);

                    return ScheduleWork(cancellationToken => backgroundService.StartAsync(cancellationToken),
                        linkedToken);
                });
            }
        }

        private async Task ScheduleWork(
            [NotNull] Func<CancellationToken, Task> taskFunc,
            CancellationToken cancellationToken)
        {
            if (taskFunc == null)
            {
                throw new ArgumentNullException(nameof(taskFunc));
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await taskFunc(cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception in background service, restarting");
                }
            }
        }

        private void CheckState()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
