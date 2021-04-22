using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Lyra.Features.SessionTracking;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Lyra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider ServiceProvider { get; }

        public IConfiguration Configuration { get; }

        public App(Action<IServiceCollection> configureServices = null)
        {
            Configuration = BuildConfiguration();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddLogging(configure =>
            {
                configure.AddSerilog(dispose: true);
            });

            ConfigureServices(services);
            configureServices?.Invoke(services);
            ServiceProvider = services.BuildServiceProvider();

            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private static IConfiguration BuildConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.console.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("Lyra_")
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddShell(Configuration);
            services.AddDatabase(Configuration);

            services.AddSong(Configuration);
            services.AddPresentationStyle(Configuration);
            services.AddSessionTracking(Configuration);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.ViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            => Log.Logger.Fatal(e.Exception, "Unhandled exception.");

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Log.CloseAndFlush();
        }
    }
}
