using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoundStore.Infrastructure.SqlServer;

namespace SoundStore.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<SoundStoreSqlServerDbContext>(o =>
            {
                o.UseSqlServer(configuration.GetConnectionString("Default"), a =>
                {
                    a.CommandTimeout(120);
                });
                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
