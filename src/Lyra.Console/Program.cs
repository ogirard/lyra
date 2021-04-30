using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using LiteDB;
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

            var dbConnectionString = new ConnectionString(
                configuration
                    .GetValue<string>("Database:ConnectionString")
                    .Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));

            var runCleanMigration = configuration.GetValue<bool>("Migration:RunCleanMigration");
            if (runCleanMigration)
            {
                Migrator.Cleanup(dbConnectionString.Filename, Serilog.Log.Logger);
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
