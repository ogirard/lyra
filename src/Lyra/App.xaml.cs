using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Lyra.Features.SessionTracking;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.FluentLight.WPF;

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
            SyncfusionLicenseProvider.RegisterLicense("NDM1MDY1QDMxMzkyZTMxMmUzMGRmSWo1MkdMczNsdStibjd2RlphdHNqV002S0ttb0RITU8ra1pRU3FXR3c9");
            InitializeTheme();
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

        private static void InitializeTheme()
        {
            SfSkinManager.ApplyStylesOnApplication = true;

            var themeSettings = new FluentLightThemeSettings
            {
                BodyFontSize = 15,
                HeaderFontSize = 18,
                SubHeaderFontSize = 17,
                TitleFontSize = 17,
                SubTitleFontSize = 16,
                BodyAltFontSize = 15,
                FontFamily = new FontFamily("Roboto"),
            };

            var locale = new CultureInfo("de");
            Thread.CurrentThread.CurrentCulture = locale;
            Thread.CurrentThread.CurrentUICulture = locale;
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(locale.IetfLanguageTag)));

            SfSkinManager.RegisterThemeSettings("FluentLight", themeSettings);
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
