using Microsoft.Extensions.DependencyInjection;

namespace SoundStore.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(TokenService));
            
            return services;
        }
    }
}
