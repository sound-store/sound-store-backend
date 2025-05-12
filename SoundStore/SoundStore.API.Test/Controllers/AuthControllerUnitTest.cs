using Microsoft.AspNetCore.Mvc;
using Moq;
using SoundStore.API.Controllers.v1;
using SoundStore.Core.Commons;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Test.Controllers
{
    public class AuthControllerUnitTest
    {
        private readonly Mock<IUserService> _mockService;
        private readonly AuthController _controller;

        public AuthControllerUnitTest()
        {
            _mockService = new Mock<IUserService>();
            _controller = new AuthController(_mockService.Object);
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsLoginResponse()
        {
            // Arrange
            var loginRequest = new Microsoft.AspNetCore.Identity.Data.LoginRequest 
            { 
                Email = "user@example.com", 
                Password = "pass" 
            };
            var expected = new LoginResponse { Token = "abc123" };

            _mockService.Setup(s => s.Login(loginRequest.Email, loginRequest.Password))
                        .ReturnsAsync(expected);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(okResult.Value);

            Assert.True(response.IsSuccess);
            Assert.Equal("Success", response.Message);
            Assert.Equal("abc123", response.Value?.Token);
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsSuccess()
        {
            var registration = new UserRegistration { Email = "newuser@example.com", Password = "pass" };

            _mockService.Setup(s => s.RegisterUser(registration)).ReturnsAsync(true);

            var result = await _controller.Register(registration);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);

            Assert.True(response.IsSuccess);
            Assert.Equal("User registered successfully!", response.Message);
        }

        [Fact]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var result = await _controller.Register(new UserRegistration());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Register_WhenServiceFails_ReturnsBadRequest()
        {
            var registration = new UserRegistration { Email = "fail@example.com", Password = "pass" };

            _mockService.Setup(s => s.RegisterUser(registration)).ReturnsAsync(false);

            var result = await _controller.Register(registration);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task GetUserInfo_ReturnsLoginResponse()
        {
            var expected = new LoginResponse { Token = "xyz456" };

            _mockService.Setup(s => s.GetUserInfoBasedOnToken()).ReturnsAsync(expected);

            var result = await _controller.GetUserInformationBasedOnToken();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<LoginResponse>>(okResult.Value);

            Assert.True(response.IsSuccess);
            Assert.Equal("Success", response.Message);
            Assert.Equal("xyz456", response.Value?.Token);
        }
    }
}
