using System;
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

            var app = new App(
                (services, config) =>
                {
                    services.AddSingleton<Migrator>();
                },
                new[] { "appsettings.console.json" });

            // Migrate
            var migrator = app.Host.Services.GetRequiredService<Migrator>();
            var configuration = app.Host.Services.GetRequiredService<IConfiguration>();
            var runCleanMigration = configuration.GetValue<bool>("Migration:RunCleanMigration");
            if (runCleanMigration)
            {
                migrator.Cleanup();
                var legacyStore = migrator.InitializeLegacyStore();
            }

            app.InitializeComponent();
            app.Run();
        }
    }
}
