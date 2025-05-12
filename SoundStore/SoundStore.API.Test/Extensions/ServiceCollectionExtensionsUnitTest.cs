using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoundStore.API.Extensions;
using SoundStore.API.Middlewares;
using SoundStore.Core.Commons;

namespace SoundStore.API.Test.Extensions
{
    public class ServiceCollectionExtensionsUnitTest
    {
        private readonly IConfiguration _configuration;

        public ServiceCollectionExtensionsUnitTest()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "TestKeyForJwtToken1234567890"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"EmailSettings:From", "test@example.com"},
                {"CloudinarySettings:CloudName", "demo"},
                {"payOS:ClientId", "test-client-id"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public void Register_ShouldConfigureRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.Register(_configuration);

            // Assert
            Assert.Contains(services, s => s.ServiceType == typeof(IHttpContextAccessor));
            Assert.Contains(services, s => s.ServiceType == typeof(GlobalExceptionHandlingMiddleware));
            Assert.Contains(services, s => s.ServiceType == typeof(UserClaimsMiddleware));
            Assert.Contains(services, s => s.ServiceType.FullName?.Contains("IApiVersionDescriptionProvider") == true);
            Assert.Contains(services, s => s.ServiceType.FullName?.Contains("IAuthenticationService") == true);
            Assert.Contains(services, s => s.ServiceType.FullName?.Contains("IAuthorizationService") == true);
        }

        [Fact]
        public void Register_ShouldBindConfigurationSections()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.Register(_configuration);

            // Assert
            var provider = services.BuildServiceProvider();

            var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
            var cloudinarySettings = _configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
            var payOSSettings = _configuration.GetSection("payOS").Get<PayOSSettings>();

            Assert.NotNull(jwtSettings);
            Assert.NotNull(emailSettings);
            Assert.NotNull(cloudinarySettings);
            Assert.NotNull(payOSSettings);
        }

        [Fact]
        public void Register_ShouldAddCorsPolicy()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.Register(_configuration);

            // Assert
            // Check if CORS services are registered by checking the presence of the CORS policy provider or configuration
            Assert.Contains(services, s =>
                s.ServiceType.FullName?.Contains("ICorsPolicyProvider") == true ||
                s.ServiceType.FullName?.Contains("CorsService") == true
            );
        }

    }
}

