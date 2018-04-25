using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Hosting;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Core.Extensions.BoolExtensions;
using Arbor.NuGetServer.Api.Clean;
using Arbor.NuGetServer.Core.Extensions;
using Arbor.NuGetServer.IisHost.Areas.Configuration;
using Autofac;
using JetBrains.Annotations;
using Serilog;
using Serilog.Core;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public sealed class NuGetServerApp : IDisposable
    {
        private readonly Action<Func<CancellationToken, Task>> _backgroundServiceHandler;
        private readonly List<IBackgroundService> _backgroundServices;
        private CancellationTokenSource _cancellationTokenSource;
        private AppContainer _container;
        private Logger _logger;
        private bool _isStopped;
        private bool _isRunning;

        private NuGetServerApp(
            IKeyValueConfiguration keyValueConfiguration,
            AppContainer container,
            Logger logger,
            Action<Func<CancellationToken, Task>> backgroundServiceHandler,
            IReadOnlyCollection<IBackgroundService> backgroundServices)
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

        public ILifetimeScope LifeTimeScope
        {
            get
            {
                CheckState();
                return _container.AppScope;
            }
        }

        public static NuGetServerApp Create(Action<Func<CancellationToken, Task>> backgroundServiceHandler)
        {
            Logger logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger();

            IKeyValueConfiguration keyValueConfiguration = ConfigurationStartup.Start();

            IReadOnlyCollection<Assembly> AssemblyResolver() => BuildManager.GetReferencedAssemblies()
                .OfType<Assembly>()
                .SafeToReadOnlyCollection();

            AppContainer container = Bootstrapper.Start(AssemblyResolver, logger, keyValueConfiguration);

            var backgroundServices = new List<IBackgroundService>();

            if (keyValueConfiguration.ValueOrDefault(CleanConstants.CleanOnStartEnabled))
            {
                var cleanService = container.Container.Resolve<CleanService>();

                cleanService.CleanBinFiles(false);
            }

            var collection = container.Container.Resolve<IEnumerable<IBackgroundService>>();

            backgroundServices.AddRange(collection);

            logger.Information("Added background services {BackgroundServices}", backgroundServices);

            string packagesPath =
                keyValueConfiguration[ConfigurationKeys.PackagesDirectoryPath];

            if (!string.IsNullOrWhiteSpace(packagesPath))
            {
                if (packagesPath.StartsWith("~", StringComparison.OrdinalIgnoreCase))
                {
                    packagesPath = HostingEnvironment.MapPath(packagesPath);
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

            foreach (IBackgroundService backgroundService in _backgroundServices)
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

            foreach (IBackgroundService backgroundService in _backgroundServices)
            {
                _backgroundServiceHandler.Invoke(token =>
                {
                    CancellationToken linkedToken = CancellationTokenSource
                        .CreateLinkedTokenSource(token, _cancellationTokenSource.Token).Token;

                    _logger.Information("Starting background service {BackgroundService}",
                        backgroundService.GetType().Name);

                    return ScheduleWork(linkedToken,
                        cancellationToken => backgroundService.StartAsync(cancellationToken));
                });
            }
        }

        private async Task ScheduleWork(CancellationToken cancellationToken, [NotNull] Func<CancellationToken, Task> taskFunc)
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
