using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lyra.Features.SessionTracking
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddSessionTracking(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISessionRepository, SessionRepository>();
            return services;
        }
    }
}