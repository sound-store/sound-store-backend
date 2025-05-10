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

            services.AddScoped(typeof(ITokenService), typeof(TokenService));

            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IProductRatingService, ProductRatingService>();

            services.AddScoped(typeof(IUserClaimsService), typeof(UserClaimsService));

            services.AddScoped<IFileStorageService, FileStorageService>();

            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            return services;
        }
    }
}
