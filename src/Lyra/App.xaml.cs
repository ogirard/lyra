using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Lyra.Features.Config;
using Lyra.Features.Search;
using Lyra.Features.SessionTracking;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Lyra.UI;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.FluentLight.WPF;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Windows.Media.FontFamily;

namespace Lyra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IHost Host { get; }

        public App()
            : this(null, null)
        {
        }

        public App(IConfiguration configuration, Action<IServiceCollection> configureServices)
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            Startup += OnStartupAsync;
            Exit += OnExitAsync;

            SyncfusionLicenseProvider.RegisterLicense(
                "NDM1MDY1QDMxMzkyZTMxMmUzMGRmSWo1MkdMczNsdStibjd2RlphdHNqV002S0ttb0RITU8ra1pRU3FXR3c9");

            var splashScreen = new SplashScreen(GetType().Assembly, "Resources/splash.png");
            splashScreen.Show(true, true);

            Host = new HostBuilder()
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    if (configuration != null)
                    {
                        configurationBuilder.AddConfiguration(configuration);
                        return;
                    }

                    configurationBuilder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("appsettings.logger.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables("Lyra_")
                        .AddCommandLine(Environment.GetCommandLineArgs());
                }).ConfigureLogging((context, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .CreateLogger();

                    logging.AddSerilog(dispose: true);
                })
                .ConfigureServices((context, services) =>
                {
                    InitializeTheme(context.Configuration);

                    ConfigureServices(services, context.Configuration);
                    configureServices?.Invoke(services);
                })
                .Build();
        }

        private void InitializeTheme(IConfiguration configuration)
        {
            var lyraOptions = configuration.GetSection("Lyra").Get<LyraOptions>();
            var themeOptions = configuration.GetSection("Theme").Get<ThemeOptions>();
            SfSkinManager.ApplyStylesOnApplication = true;
            var primaryBrush =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(themeOptions.PrimaryBackground));
            var themeSettings = new FluentLightThemeSettings
            {
                PrimaryBackground = primaryBrush,
                PrimaryForeground =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(themeOptions.PrimaryForeground)),
                HeaderFontSize = themeOptions.HeaderFontSize,
                SubHeaderFontSize = themeOptions.SubHeaderFontSize,
                TitleFontSize = themeOptions.TitleFontSize,
                SubTitleFontSize = themeOptions.SubTitleFontSize,
                BodyFontSize = themeOptions.BodyFontSize,
                BodyAltFontSize = themeOptions.BodyAltFontSize,
                FontFamily = new FontFamily("Roboto"),
                ListAccentMedium = primaryBrush,
                SystemAccentColor = primaryBrush,
                ListAccentLow = primaryBrush,
                ListAccentHigh = primaryBrush,
            };

            var locale = new CultureInfo(lyraOptions.Locale);
            Thread.CurrentThread.CurrentCulture = locale;
            Thread.CurrentThread.CurrentUICulture = locale;
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(locale.IetfLanguageTag)));

            SfSkinManager.RegisterThemeSettings("FluentLight", themeSettings);
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(App).GetTypeInfo().Assembly);
            services.AddConfig(configuration);
            services.AddDatabase(configuration);

            services.AddSong(configuration);
            services.AddPresentationStyle(configuration);
            services.AddSessionTracking(configuration);
            services.AddSearch(configuration);

            services.AddUI(configuration);
        }

        private async void OnStartupAsync(object sender, StartupEventArgs e)
        {
            await Host.StartAsync();

            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            mainWindow.ViewModel = Host.Services.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
        }

        private async void OnExitAsync(object sender, ExitEventArgs e)
        {
            using (Host)
            {
                Log.CloseAndFlush();
                await Host.StopAsync();
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            => Log.Logger.Fatal(e.Exception, "Unhandled exception.");
    }
}
