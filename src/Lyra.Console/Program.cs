using System;
using System.IO;
using Lyra.Console.Migration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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

            var app = new App(s =>
            {
                s.AddSingleton<Migrator>();
            });

            app.InitializeComponent();

            var runCleanMigration = app.Configuration.GetValue<bool>("Migration:RunCleanMigration");
            if (runCleanMigration)
            {
                var dbPath = new FileInfo(app.Configuration.GetValue<string>("Database:ConnectionString").Replace("Filename=", string.Empty).Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
                if (dbPath.Exists)
                {
                    dbPath.Delete();
                    Log.Logger.Information($"Deleted {dbPath.FullName}");

                    var dbLogPath = new FileInfo(dbPath.FullName.Replace(".db", "-log.db"));
                    if (dbLogPath.Exists)
                    {
                        dbLogPath.Delete();
                        Log.Logger.Information($"Deleted {dbLogPath.FullName}");
                    }
                }

                var migrator = app.ServiceProvider.GetRequiredService<Migrator>();
                var legacyStore = migrator.InitializeLegacyStore();
            }

            app.Run();
        }
    }
}
