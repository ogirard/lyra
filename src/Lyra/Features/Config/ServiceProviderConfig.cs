using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lyra.Features.Config
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPresenterConfigService, PresenterConfigService>();
            return services;
        }
    }
}