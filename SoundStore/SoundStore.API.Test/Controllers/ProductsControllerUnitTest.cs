using Microsoft.AspNetCore.Mvc;
using Moq;
using SoundStore.API.Controllers.v1;
using SoundStore.Core.Commons;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;

namespace SoundStore.API.Test.Controllers
{
    public class ProductsControllerUnitTest
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerUnitTest()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public void GetProducts_ReturnsOkResult_WithProductList()
        {
            // Arrange
            var expected = new PaginatedList<ProductResponse>(new List<ProductResponse>(), 0, 1, 10);
            _mockService.Setup(s => s.GetProducts(1, 10, It.IsAny<ProductFilterParameters>(), null))
                        .Returns(expected);

            // Act
            var result = _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PaginatedList<ProductResponse>>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public void GetProductByCategory_ReturnsOkResult()
        {
            // Arrange
            var expected = new PaginatedList<ProductResponse>(new List<ProductResponse>(), 0, 1, 10);
            _mockService.Setup(s => s.GetProductByCategory(1, 10, 1)).Returns(expected);

            // Act
            var result = _controller.GetProductByCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PaginatedList<ProductResponse>>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public void GetProductBySubCategory_ReturnsOkResult()
        {
            var expected = new PaginatedList<ProductResponse>(new List<ProductResponse>(), 0, 1, 10);
            _mockService.Setup(s => s.GetProductBySubCategory(1, 10, 1)).Returns(expected);

            var result = _controller.GetProductBySubCategory(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<PaginatedList<ProductResponse>>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult()
        {
            var product = new ProductResponse { Id = 1 };
            _mockService.Setup(s => s.GetProduct(1)).ReturnsAsync(product);

            var result = await _controller.GetProduct(1);

            var response = Assert.IsType<ApiResponse<ProductResponse>>(result.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal(1, response.Value.Id);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOkResult_WhenSuccess()
        {
            _mockService.Setup(s => s.UpdateProduct(1, It.IsAny<ProductUpdatedRequest>())).ReturnsAsync(true);

            var result = await _controller.UpdateProduct(1, new ProductUpdatedRequest());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task AddProduct_ReturnsOkResult_WhenSuccess()
        {
            _mockService.Setup(s => s.AddProduct(It.IsAny<ProductCreatedRequest>())).ReturnsAsync(true);

            var request = new ProductCreatedRequest();
            _controller.ModelState.Clear(); // Simulate valid model

            var result = await _controller.AddProduct(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsOkResult_WhenSuccess()
        {
            _mockService.Setup(s => s.DeleteProduct(1)).ReturnsAsync(true);

            var result = await _controller.DeleteProduct(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.IsSuccess);
        }
    }
}
