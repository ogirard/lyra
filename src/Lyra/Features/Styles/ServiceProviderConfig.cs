using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lyra.Features.Styles
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddPresentationStyle(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IStyleRepository, StyleRepository>();
            return services;
        }
    }
}