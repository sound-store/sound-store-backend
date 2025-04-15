using Microsoft.Extensions.DependencyInjection;
using SoundStore.Core;
using SoundStore.Core.Services;
using SoundStore.Infrastructure;

namespace SoundStore.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(TokenService));

            services.AddScoped<ICategoryService, CategorySerivce>();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped(typeof(UserClaimsService));

            return services;
        }
    }
}
