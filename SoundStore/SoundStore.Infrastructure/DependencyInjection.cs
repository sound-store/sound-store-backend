using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoundStore.Core.Entities;

namespace SoundStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? throw new ArgumentException("Cannot fetch the current environment!");

            services.AddDbContext<SoundStoreDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionString(env), o =>
                {
                    o.CommandTimeout(120);
                });
            });

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<SoundStoreDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(o =>
            {
                o.Password.RequiredLength = 8;
                o.Lockout.MaxFailedAccessAttempts = 10;
                o.User.RequireUniqueEmail = true;
            });

            return services;
        }

        private static string GetConnectionString(string environment)
        {
            if ("Development" == environment)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .Build();

                return builder.GetConnectionString("Default") ?? string.Empty;

            }
            else if ("Production" == environment)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .Build();

                return builder.GetConnectionString("Default") ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
