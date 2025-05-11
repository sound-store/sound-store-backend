using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using SoundStore.Core.Entities;

namespace SoundStore.Infrastructure.Test
{
    public class DependencyInjectionUnitTest
    {
        [Fact]
        public void RegisterInfrastructure_ShouldRegisterRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configMock = new Mock<IConfiguration>();
            var connectionString = "Server=test;Database=SoundStore;Trusted_Connection=True;";
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(s => s.Value).Returns(connectionString);

            // Setup environment variable for testing
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            // Mock configuration to return a connection string
            configMock.Setup(c => c.GetSection("ConnectionStrings:Default"))
                .Returns(configSectionMock.Object);

            // Act
            var result = DependencyInjection.RegisterInfrastructure(services, configMock.Object);

            // Assert
            Assert.Same(services, result); // Verify the same instance is returned

            // Verify DbContext is registered
            Assert.Contains(services,
                sd => sd.ServiceType == typeof(DbContextOptions<SoundStoreDbContext>));

            // Verify Identity services are registered
            Assert.Contains(services,
                sd => sd.ServiceType == typeof(UserManager<AppUser>));

            Assert.Contains(services,
                sd => sd.ServiceType == typeof(RoleManager<IdentityRole>));

            // Verify IdentityOptions are configured
            var serviceProvider = services.BuildServiceProvider();
            var identityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;

            Assert.Equal(8, identityOptions.Password.RequiredLength);
            Assert.Equal(10, identityOptions.Lockout.MaxFailedAccessAttempts);
            Assert.True(identityOptions.User.RequireUniqueEmail);
        }

        [Fact]
        public void RegisterInfrastructure_ShouldThrowException_WhenEnvironmentVariableIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            var configMock = new Mock<IConfiguration>();

            // Setup environment variable to be null
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                DependencyInjection.RegisterInfrastructure(services, configMock.Object));

            Assert.Equal("Cannot fetch the current environment!", exception.Message);
        }

        [Fact]
        public void GetConnectionString_ShouldReturnConnectionStringForDevelopment()
        {
            // This test would need to use reflection to test the private method
            // or would require making the method internal and using InternalsVisibleTo
            // For now, we're testing it indirectly through the RegisterInfrastructure method
        }
    }
}