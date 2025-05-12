using Microsoft.AspNetCore.Mvc;
using Moq;
using SoundStore.API.Controllers.v1;
using SoundStore.Core.Commons;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Test.Controllers
{
    public class RatingsControllerUnitTest
    {
        private readonly Mock<IProductRatingService> _mockService;
        private readonly RatingsController _controller;

        public RatingsControllerUnitTest()
        {
            _mockService = new Mock<IProductRatingService>();
            _controller = new RatingsController(_mockService.Object);
        }

        [Fact]
        public async Task GetRatingOfAProduct_ReturnsCorrectRating()
        {
            // Arrange
            var expected = new ProductRatingResponse
            {
                ProductId = 1,
                RatingPoint = 4.2m,
                Comment = new List<string> { "Great!", "Loved it!" }
            };

            _mockService.Setup(s => s.GetRatingOfAProduct(1)).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetRatingOfAProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductRatingResponse>>(okResult.Value);

            Assert.True(response.IsSuccess);
            Assert.Equal("Fetch product's rating successfully", response.Message);
            Assert.Equal(1, response.Value?.ProductId);
            Assert.Equal(4.2m, response.Value?.RatingPoint);
            Assert.Contains("Great!", response.Value!.Comment);
            Assert.Contains("Loved it!", response.Value.Comment);
        }

        [Fact]
        public async Task AddRating_WhenSuccessful_ReturnsOk()
        {
            var ratingRequest = new ProductRatingRequest
            {
                ProductId = 1,
                Point = 5,
                Comment = "Excellent"
            };

            _mockService.Setup(s => s.AddRating(ratingRequest)).ReturnsAsync(true);

            var result = await _controller.AddRating(ratingRequest);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);

            Assert.True(response.IsSuccess);
            Assert.Equal("Add rating successfully!", response.Message);
        }

        [Fact]
        public async Task AddRating_WhenFails_ReturnsBadRequest()
        {
            var ratingRequest = new ProductRatingRequest
            {
                ProductId = 1,
                Point = 2,
                Comment = "Not great"
            };

            _mockService.Setup(s => s.AddRating(ratingRequest)).ReturnsAsync(false);

            var result = await _controller.AddRating(ratingRequest);

            Assert.IsType<BadRequestResult>(result.Result);
        }
    }
}
