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
using Serilog;
using Serilog.Core;

namespace Arbor.NuGetServer.IisHost.Areas.Application
{
    public sealed class NuGetServerApp : IDisposable
    {
        private readonly Action<Func<CancellationToken, Task>> _backgroundServiceHandler;
        private readonly List<IBackgroundService> _backgroundServices;
        private CancellationTokenSource _cancellationTokenSource;
        private IContainer _container;
        private Logger _logger;

        private NuGetServerApp(
            IKeyValueConfiguration keyValueConfiguration,
            IContainer container,
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

        public IContainer Container
        {
            get
            {
                CheckState();
                return _container;
            }
        }

        public static NuGetServerApp Create(Action<Func<CancellationToken, Task>> backgroundServiceHandler)
        {
            Logger logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger();
            Log.Logger = logger; //TODO

            IKeyValueConfiguration keyValueConfiguration = ConfigurationStartup.Start();

            IReadOnlyCollection<Assembly> AssemblyResolver() => BuildManager.GetReferencedAssemblies()
                .OfType<Assembly>()
                .SafeToReadOnlyCollection();

            IContainer container = Bootstrapper.Start(AssemblyResolver, logger, keyValueConfiguration);

            var backgroundServices = new List<IBackgroundService>();

            using (ILifetimeScope beginLifetimeScope = container.BeginLifetimeScope())
            {
                if (keyValueConfiguration.ValueOrDefault(CleanConstants.CleanOnStartEnabled))
                {
                    var cleanService = beginLifetimeScope.Resolve<CleanService>();

                    cleanService.CleanBinFiles(false);
                }

                var collection = beginLifetimeScope.Resolve<IEnumerable<IBackgroundService>>();

                backgroundServices.AddRange(collection);

                logger.Information("Added background services {BackgroundServices}", backgroundServices);
            }

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

            Log.CloseAndFlush();

            SafeDispose.Dispose(_container);
            SafeDispose.Dispose(_logger);
            _container = null;
            Disposed = true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Start()
        {
            foreach (IBackgroundService backgroundService in _backgroundServices)
            {
                _backgroundServiceHandler.Invoke(token =>
                {
                    CancellationToken linkedToken = CancellationTokenSource
                        .CreateLinkedTokenSource(token, _cancellationTokenSource.Token).Token;

                    _logger.Information("Starting background service {BackgroundService}", backgroundService.GetType().Name);

                    return ScheduleWork(linkedToken, cancellationToken => backgroundService.StartAsync(cancellationToken));
                });
            }
        }

        private async Task ScheduleWork(CancellationToken cancellationToken, Func<CancellationToken, Task> taskFunc)
        {
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
