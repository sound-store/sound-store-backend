using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Services;

namespace SoundStore.Service.Test
{
    public class UserServiceUnitTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILogger<UserService>> _mockLogger = new();
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<ITokenService> _mockTokenService = new();
        private readonly Mock<IUserClaimsService> _mockClaimsService = new();

        public UserServiceUnitTest()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                store.Object, Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<AppUser>>(),
                Array.Empty<IUserValidator<AppUser>>(),
                Array.Empty<IPasswordValidator<AppUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<AppUser>>>()
            );
        }

        #region Login_ShouldReturnLoginResponse_WhenCredentialsAreValid
        [Fact]
        public async Task Login_ShouldReturnLoginResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1),
                UserName = "test@example.com"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "password"))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(["Customer"]);

            _mockTokenService.Setup(x => x.GenerateToken(user, "Customer"))
                .Returns("mocked-token");

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );
            // Act
            var result = await userService.Login(user.Email, "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
            Assert.Equal(user.Address, result.Address);
            Assert.Equal(user.DateOfBirth, result.DateOfBirth);
            Assert.Equal("mocked-token", result.Token);
            Assert.Equal("Customer", result.Role);
        }
        #endregion

        #region Login_ShouldThrowException_WhenPasswordIsIncorrect
        [Fact]
        public async Task Login_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new AppUser { Email = "test@example.com" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "wrongpassword"))
                .ReturnsAsync(false);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.Login(user.Email, "wrongpassword"));

            Assert.Equal("Incorrect password!", exception.Message);
        }
        #endregion


    }
}
