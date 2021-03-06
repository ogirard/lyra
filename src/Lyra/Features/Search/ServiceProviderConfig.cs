using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lyra.Features.Search
{
    public static class ServiceProviderConfig
    {
        public static IServiceCollection AddSearch(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISearchService, IndexSearchService>();
            services.AddSingleton<SearchIndex>();
            services.AddHostedService(sp => sp.GetRequiredService<SearchIndex>());
            return services;
        }
    }
}