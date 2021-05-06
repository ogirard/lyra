using System;
using System.IO;
using LiteDB;
using Lyra.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lyra
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddUI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton<SongPresenterViewModel>();
            services.AddTransient<SongPresenter>();
            services.AddTransient<JumpmarkActivatedHandler>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LyraOptions>(configuration.GetSection("Lyra"));

            // Register db and repo
            services.AddSingleton<ILiteDatabase>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<LyraOptions>>();
                var connectionString = new ConnectionString(options.Value.DatabaseConnectionString
                    .Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
                Directory.CreateDirectory(Path.GetDirectoryName(connectionString.Filename));
                return new LiteDatabase(connectionString);
            });

            services.AddSingleton<ILiteRepository>(sp =>
            {
                var db = sp.GetRequiredService<ILiteDatabase>();
                return new LiteRepository(db);
            });

            return services;
        }
    }
}