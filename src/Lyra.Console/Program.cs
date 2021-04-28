using System;
using System.IO;
using Lyra.Console.Migration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Debugging;
using static System.Console;

namespace Lyra.Console
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Title = $"Lyra.Console {DateTime.Now:dd.MM.yyyy HH:mm:ss} [user {Environment.UserName} on {Environment.MachineName}]";
            SelfLog.Enable(msg =>
            {
                var currentColor = ForegroundColor;
                ForegroundColor = ConsoleColor.Magenta;
                WriteLine($"SERILOG> {msg}");
                ForegroundColor = currentColor;
            });
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.console.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("Lyra_")
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();

            var dbPath = configuration
                .GetValue<string>("Database:ConnectionString")
                .Replace("Filename=", string.Empty)
                .Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            var runCleanMigration = configuration.GetValue<bool>("Migration:RunCleanMigration");
            if (runCleanMigration)
            {
                Migrator.Cleanup(dbPath, Serilog.Log.Logger);
            }

            var app = new App(
                configuration,
                services =>
                {
                    services.AddSingleton<Migrator>();
                });

            // Migrate
            if (runCleanMigration)
            {
                var migrator = app.Host.Services.GetRequiredService<Migrator>();
                migrator.InitializeLegacyStore();
            }

            app.InitializeComponent();
            app.Run();
        }
    }
}
