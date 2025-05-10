using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Constants;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

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

        #region Login_ShouldThrowException_WhenUserDoesNotExist
        [Fact]
        public async Task Login_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((AppUser)null);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.Login(email, "anypassword"));

            Assert.Equal("User does not exist!", exception.Message);
        }
        #endregion

        #region RegisterUser_ShouldReturnTrue_WhenValidDataProvided
        [Fact]
        public async Task RegisterUser_ShouldReturnTrue_WhenValidDataProvided()
        {
            // Arrange
            var userRegistration = new UserRegistration
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(userRegistration.Email))
                .ReturnsAsync((AppUser)null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), userRegistration.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), UserRoles.Customer))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.RegisterUser(userRegistration);

            // Assert
            Assert.True(result);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), userRegistration.Password), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), UserRoles.Customer), Times.Once);
        }
        #endregion

        #region RegisterUser_ShouldThrowException_WhenPasswordsDontMatch
        [Fact]
        public async Task RegisterUser_ShouldThrowException_WhenPasswordsDontMatch()
        {
            // Arrange
            var userRegistration = new UserRegistration
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword123!",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => userService.RegisterUser(userRegistration));

            Assert.Equal("Password and confirm password do not match!", exception.Message);
        }
        #endregion

        #region RegisterUser_ShouldThrowException_WhenEmailAlreadyExists
        [Fact]
        public async Task RegisterUser_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var userRegistration = new UserRegistration
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var existingUser = new AppUser { Email = userRegistration.Email };
            _mockUserManager.Setup(x => x.FindByEmailAsync(userRegistration.Email))
                .ReturnsAsync(existingUser);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DuplicatedException>(
                () => userService.RegisterUser(userRegistration));

            Assert.Equal("User with this email has already existed!", exception.Message);
        }
        #endregion

        #region UpdateStatus_ShouldReturnTrue_WhenValidStatusProvided
        [Fact]
        public async Task UpdateStatus_ShouldReturnTrue_WhenValidStatusProvided()
        {
            // Arrange
            var userId = "1";
            var status = "Inactived";
            var user = new AppUser { Id = userId };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.UpdateStatus(userId, status);

            // Assert
            Assert.True(result);
            _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<AppUser>()), Times.Once);
        }
        #endregion

        #region UpdateStatus_ShouldThrowException_WhenUserNotFound
        [Fact]
        public async Task UpdateStatus_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = "nonexistent";
            var status = "Inactived";

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((AppUser)null);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.UpdateStatus(userId, status));

            Assert.Equal("User does not exist!", exception.Message);
        }
        #endregion

        #region UpdateStatus_ShouldThrowException_WhenInvalidStatusProvided
        [Fact]
        public async Task UpdateStatus_ShouldThrowException_WhenInvalidStatusProvided()
        {
            // Arrange
            var userId = "1";
            var status = "InvalidStatus";
            var user = new AppUser { Id = userId };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => userService.UpdateStatus(userId, status));

            Assert.Equal("Invalid user status!", exception.Message);
        }
        #endregion

        #region AddUser_ShouldReturnTrue_WhenValidDataProvided
        [Fact]
        public async Task AddUser_ShouldReturnTrue_WhenValidDataProvided()
        {
            // Arrange
            var addedUserRequest = new AddedUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Role = UserRoles.Customer
            };

            // Setup UserManager mocks
            _mockUserManager.Setup(x => x.FindByEmailAsync(addedUserRequest.Email))
                .ReturnsAsync((AppUser)null); // No existing user with this email

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), addedUserRequest.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), addedUserRequest.Role))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.AddUser(addedUserRequest);

            // Assert
            Assert.True(result);
            _mockUserManager.Verify(x => x.FindByEmailAsync(addedUserRequest.Email), Times.Once);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), addedUserRequest.Password), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), addedUserRequest.Role), Times.Once);
        }
        #endregion

        #region AddUser_ShouldThrowException_WhenInvalidRole
        [Fact]
        public async Task AddUser_ShouldThrowException_WhenInvalidRole()
        {
            // Arrange
            var addedUserRequest = new AddedUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                Role = "InvalidRole"
            };

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => userService.AddUser(addedUserRequest));

            Assert.Equal("Invalid user's role!", exception.Message);
        }
        #endregion

        #region GetCustomer_ShouldReturnCustomerInfo_WhenCustomerExists
        [Fact]
        public async Task GetCustomer_ShouldReturnCustomerInfo_WhenCustomerExists()
        {
            // Arrange
            var customerId = "1";
            var customer = new AppUser
            {
                Id = customerId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Status = UserState.Actived
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(customerId))
                .ReturnsAsync(customer);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.GetCustomer(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.FirstName, result.FirstName);
            Assert.Equal(customer.LastName, result.LastName);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.PhoneNumber, result.PhoneNumber);
            Assert.Equal(customer.Address, result.Address);
            Assert.Equal(customer.DateOfBirth, result.DateOfBirth);
            Assert.Equal(customer.Status.ToString(), result.Status);
        }
        #endregion

        #region GetCustomer_ShouldThrowException_WhenCustomerNotFound
        [Fact]
        public async Task GetCustomer_ShouldThrowException_WhenCustomerNotFound()
        {
            // Arrange
            var customerId = "nonexistent";

            _mockUserManager.Setup(x => x.FindByIdAsync(customerId))
                .ReturnsAsync((AppUser)null);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.GetCustomer(customerId));

            Assert.Equal("Customer not found!", exception.Message);
        }
        #endregion

        #region DeleteUser_ShouldBehaveAsExpected
        [Fact]
        public async Task DeleteUser_ShouldReturnTrue_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var userId = "1";
            var user = new AppUser
            {
                Id = userId,
                Orders = new List<Order>(),
                Transactions = new List<Transaction>()
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.DeleteUser(userId);

            // Assert
            Assert.True(result);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
            _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent";

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((AppUser)null);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.DeleteUser(userId));

            Assert.Equal("User does not exist!", exception.Message);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrowException_WhenUserHasOrdersOrTransactions()
        {
            // Arrange
            var userId = "1";
            var user = new AppUser
            {
                Id = userId,
                Orders = new List<Order> { new Order() },
                Transactions = new List<Transaction>()
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => userService.DeleteUser(userId));

            Assert.Equal("Cannot delete this user because of data conflict in other tables!", exception.Message);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
            _mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<AppUser>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrowException_WhenDeleteFails()
        {
            // Arrange
            var userId = "1";
            var user = new AppUser
            {
                Id = userId,
                Orders = new List<Order>(),
                Transactions = new List<Transaction>()
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => userService.DeleteUser(userId));

            Assert.Equal("An error occured while deleting the user!", exception.Message);
            _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
            _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);
        }
        #endregion

        #region GetUserInfoBasedOnToken_ShouldBehaveAsExpected
        [Fact]
        public async Task GetUserInfoBasedOnToken_ShouldReturnLoginResponse_WhenTokenIsValid()
        {
            // Arrange
            var userId = "1";
            var user = new AppUser
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Customer" });

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.GetUserInfoBasedOnToken();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
            Assert.Equal(user.Address, result.Address);
            Assert.Equal(user.DateOfBirth, result.DateOfBirth);
            Assert.Equal("Customer", result.Role);
        }

        [Fact]
        public async Task GetUserInfoBasedOnToken_ShouldThrowUnauthorizedAccessException_WhenTokenIsMissing()
        {
            // Arrange
            _mockClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(string.Empty);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => userService.GetUserInfoBasedOnToken());

            Assert.Equal("User is not authenticated!", exception.Message);
        }

        [Fact]
        public async Task GetUserInfoBasedOnToken_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent";

            _mockClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((AppUser)null);

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.GetUserInfoBasedOnToken());

            Assert.Equal("User does not exist!", exception.Message);
        }

        [Fact]
        public async Task GetUserInfoBasedOnToken_ShouldThrowKeyNotFoundException_WhenRoleDoesNotExist()
        {
            // Arrange
            var userId = "1";
            var user = new AppUser
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _mockClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>()); // No roles found

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => userService.GetUserInfoBasedOnToken());

            Assert.Equal("User's role does not exist!", exception.Message);
        }
        #endregion

        #region GetCustomers_ShouldBehaveAsExpected
        [Fact]
        public async Task GetCustomers_ShouldReturnPaginatedList_WhenCustomersExist()
        {
            // Arrange
            var customers = new List<AppUser>
            {
                new AppUser
                {
                    Id = "1",
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Status = UserState.Actived
                },
                new AppUser
                {
                    Id = "2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PhoneNumber = "0987654321",
                    Address = "456 Elm St",
                    DateOfBirth = new DateOnly(1985, 5, 15),
                    Status = UserState.Inactived
                }
            };

            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(UserRoles.Customer))
                .ReturnsAsync(customers);

            var pageNumber = 1;
            var pageSize = 10;

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.GetCustomers(null, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customers.Count, result.Items.Count);
            Assert.Equal(customers[0].Id, result.Items[0].Id);
            Assert.Equal(customers[0].FirstName + " " + customers[0].LastName, result.Items[0].FullName);
            Assert.Equal(customers[0].PhoneNumber, result.Items[0].PhoneNumber);
            Assert.Equal(customers[0].Address, result.Items[0].Address);
            Assert.Equal(customers[0].DateOfBirth, result.Items[0].DateOfBirth);
            Assert.Equal(customers[0].Status.ToString(), result.Items[0].Status);
        }

        [Fact]
        public async Task GetCustomers_ShouldThrowNoDataRetrievalException_WhenNoCustomersExist()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(UserRoles.Customer))
                .ReturnsAsync(new List<AppUser>());

            var pageNumber = 1;
            var pageSize = 10;

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NoDataRetrievalException>(
                () => userService.GetCustomers(null, pageNumber, pageSize));

            Assert.Equal("No customers found!", exception.Message);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnFilteredPaginatedList_WhenNameFilterIsApplied()
        {
            // Arrange
            var customers = new List<AppUser>
            {
                new AppUser
                {
                    Id = "1",
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "1234567890",
                    Address = "123 Main St",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Status = UserState.Actived
                },
                new AppUser
                {
                    Id = "2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PhoneNumber = "0987654321",
                    Address = "456 Elm St",
                    DateOfBirth = new DateOnly(1985, 5, 15),
                    Status = UserState.Inactived
                }
            };

            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(UserRoles.Customer))
                .ReturnsAsync(customers);

            var pageNumber = 1;
            var pageSize = 10;
            var nameFilter = "Jane";

            var userService = new UserService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockUserManager.Object,
                _mockTokenService.Object,
                _mockClaimsService.Object
            );

            // Act
            var result = await userService.GetCustomers(nameFilter, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("2", result.Items[0].Id);
            Assert.Equal("Jane Smith", result.Items[0].FullName);
            Assert.Equal("0987654321", result.Items[0].PhoneNumber);
            Assert.Equal("456 Elm St", result.Items[0].Address);
            Assert.Equal(new DateOnly(1985, 5, 15), result.Items[0].DateOfBirth);
            Assert.Equal(UserState.Inactived.ToString(), result.Items[0].Status);
        }
        #endregion

    }
}
