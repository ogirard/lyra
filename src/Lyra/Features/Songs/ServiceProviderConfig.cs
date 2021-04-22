using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lyra.Features.Songs
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddSong(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISongRepository, SongRepository>();
            return services;
        }
    }
}