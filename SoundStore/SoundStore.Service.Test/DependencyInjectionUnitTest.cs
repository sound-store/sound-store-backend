using Microsoft.Extensions.DependencyInjection;
using SoundStore.Core;
using SoundStore.Core.Services;
using SoundStore.Infrastructure;

namespace SoundStore.Service.Test
{
    public class DependencyInjectionUnitTest
    {
        [Fact]
        public void RegisterServices_ShouldRegisterAllServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = DependencyInjection.RegisterServices(services);

            // Assert
            Assert.Same(services, result); // Verify the same instance is returned

            // Test generic repository registration
            Assert.Contains(services,
                sd => sd.ServiceType.IsGenericType &&
                     sd.ServiceType.GetGenericTypeDefinition() == typeof(IRepository<>) &&
                     sd.ImplementationType?.IsGenericType == true &&
                     sd.ImplementationType.GetGenericTypeDefinition() == typeof(Repository<>) &&
                     sd.Lifetime == ServiceLifetime.Scoped);

            // Verify non-generic service registrations
            VerifyServiceRegistration<IUnitOfWork, UnitOfWork>(services);
            VerifyServiceRegistration<ITokenService, TokenService>(services);
            VerifyServiceRegistration<ICategoryService, CategoryService>(services);
            VerifyServiceRegistration<IProductService, ProductService>(services);
            VerifyServiceRegistration<IUserService, UserService>(services);
            VerifyServiceRegistration<IProductRatingService, ProductRatingService>(services);
            VerifyServiceRegistration<IUserClaimsService, UserClaimsService>(services);
            VerifyServiceRegistration<IImageService, ImageService>(services);
            VerifyServiceRegistration<IPaymentService, PaymentService>(services);
        }

        private static void VerifyServiceRegistration<TService, TImplementation>(IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            Assert.Contains(services,
                sd => sd.ServiceType == typeof(TService) &&
                     sd.ImplementationType == typeof(TImplementation) &&
                     sd.Lifetime == ServiceLifetime.Scoped);
        }
    }
}
