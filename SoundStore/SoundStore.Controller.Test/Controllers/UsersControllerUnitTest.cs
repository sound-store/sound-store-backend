using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SoundStore.API.Controllers.v1;
using SoundStore.Core.Commons;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Test.Controllers
{
    public class UsersControllerUnitTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerUnitTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult_WithPaginatedList()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            string name = "Test";
            var expectedCustomers = new PaginatedList<CustomerInfoResponse>(
                new List<CustomerInfoResponse>
                {
                new CustomerInfoResponse { /* Initialize properties */ }
                },
                1, 1, 10);

            _mockUserService.Setup(service => service.GetCustomers(name, pageNumber, pageSize))
                .ReturnsAsync(expectedCustomers);

            // Act
            var result = await _controller.GetCustomers(pageNumber, pageSize, name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PaginatedList<CustomerInfoResponse>>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Fetch customer successfully", response.Message);
            Assert.Equal(expectedCustomers, response.Value);
        }

        [Fact]
        public async Task GetCustomer_ReturnsOkResult_WithCustomerInfo()
        {
            // Arrange
            string customerId = "user123";
            var expectedCustomer = new CustomerDetailedInfoResponse { /* Initialize properties */ };

            _mockUserService.Setup(service => service.GetCustomer(customerId))
                .ReturnsAsync(expectedCustomer);

            // Act
            var result = await _controller.GetCustomer(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CustomerDetailedInfoResponse>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Fetch customer successfully!", response.Message);
            Assert.Equal(expectedCustomer, response.Value);
        }

        [Fact]
        public async Task AddUser_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var addUserRequest = new AddedUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Customer"
            };

            _mockUserService.Setup(service => service.AddUser(addUserRequest))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddUser(addUserRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Add successfully!", response.Message);
        }

        [Fact]
        public async Task UpdateCustomerState_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            string customerId = "user123";
            string status = "Active";

            _mockUserService.Setup(service => service.UpdateStatus(customerId, status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCustomerState(customerId, status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Update successfully!", response.Message);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            string customerId = "user123";

            _mockUserService.Setup(service => service.DeleteUser(customerId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCustomer(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("Delete customer successfully", response.Message);
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenUserServiceThrowsException()
        {
            // Arrange
            string customerId = "nonexistent";
            _mockUserService.Setup(service => service.GetCustomer(customerId))
                .ThrowsAsync(new KeyNotFoundException("Customer not found"));

            // Act
            Func<Task> act = async () => await _controller.GetCustomer(customerId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Customer not found");
        }

        [Fact]
        public async Task AddUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required");
            var addUserRequest = new AddedUserRequest();

            // Act
            var result = await _controller.AddUser(addUserRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomers_WithNullName_CallsServiceWithEmptyString()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            string? name = null;
            var expectedCustomers = new PaginatedList<CustomerInfoResponse>(
                new List<CustomerInfoResponse>(), 0, pageNumber, pageSize);

            _mockUserService.Setup(service => service.GetCustomers(It.IsAny<string>(), pageNumber, pageSize))
                .ReturnsAsync(expectedCustomers);

            // Act
            await _controller.GetCustomers(pageNumber, pageSize, name);

            // Assert
            _mockUserService.Verify(service => service.GetCustomers(string.Empty, pageNumber, pageSize), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerState_ThrowsException_WhenInvalidStatus()
        {
            // Arrange
            string customerId = "user123";
            string invalidStatus = "InvalidStatus";

            _mockUserService.Setup(service => service.UpdateStatus(customerId, invalidStatus))
                .ThrowsAsync(new ArgumentException("Invalid status value"));

            // Act
            Func<Task> act = async () => await _controller.UpdateCustomerState(customerId, invalidStatus);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid status value");
        }
    }
}
