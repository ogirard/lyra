using System.IO;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lyra
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddShell(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<MainWindowViewModel>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetSection("Database"));

            // Register db and repo
            services.AddSingleton<ILiteDatabase>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<DatabaseOptions>>();
                var connectionString = new ConnectionString(options.Value.ConnectionString);
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