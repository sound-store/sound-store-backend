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
    public class CategoriesControllerUnitTest
    {
        private readonly Mock<ICategoryService> _mockService;
        private readonly CategoriesController _controller;

        public CategoriesControllerUnitTest()
        {
            _mockService = new Mock<ICategoryService>();
            _controller = new CategoriesController(_mockService.Object);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnList()
        {
            // Arrange
            var categories = new List<CategoryResponse> { new CategoryResponse { Id = 1, Name = "Test" } };
            _mockService.Setup(s => s.GetCategories()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var response = ok!.Value as ApiResponse<List<CategoryResponse>>;
            response!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void GetCategories_WithPagination_ReturnsPaginatedList()
        {
            // Arrange
            var paginated = new PaginatedList<CategoryResponse>([], 1, 1, 10);
            _mockService
                .Setup(s => s.GetCategories(null, 1, 10))
                .Returns(paginated);

            // Act
            var result = _controller.GetCategories(1, 10, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Extract OkObjectResult
            var apiResponse = Assert.IsType<ApiResponse<PaginatedList<CategoryResponse>>>(okResult.Value); // Extract ApiResponse<T>

            Assert.True(apiResponse.IsSuccess);
            Assert.Equal("Fetch data successfully", apiResponse.Message);
            Assert.NotNull(apiResponse.Value);
            Assert.IsType<PaginatedList<CategoryResponse>>(apiResponse.Value);
        }


        [Fact]
        public async Task GetCategory_ById_ReturnsCategory()
        {
            var category = new CategoryResponse { Id = 1, Name = "Sample" };
            _mockService.Setup(s => s.GetCategory(1)).ReturnsAsync(category);

            var result = await _controller.GetCategory(1);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            var response = ok!.Value as ApiResponse<CategoryResponse>;
            response!.IsSuccess.Should().BeTrue();
            response.Value.Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.UpdateCategory(1, "Name", "Desc")).ReturnsAsync(true);

            var request = new CategoryUpdatedRequest { Name = "Name", Description = "Desc" };
            var result = await _controller.UpdateCategory(1, request);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.AddCategory("Test", null)).ReturnsAsync(true);

            var request = new CategoryCreatedRequest { Name = "Test" };
            var result = await _controller.CreateCategory(request);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteCategory(1)).ReturnsAsync(true);

            var result = await _controller.DeleteCategory(1);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CreateSubCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.AddSubCategory(1, "SubCat")).ReturnsAsync(true);

            var result = await _controller.CreateSubCategory(1, new SubCategoryCreatedRequest { Name = "SubCat" });

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteSubCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteSubCategory(1)).ReturnsAsync(true);

            var result = await _controller.DeleteSubCategory(1);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateSubCategory_Success_ReturnsOk()
        {
            _mockService.Setup(s => s.UpdateSubCategory(1, "Updated", 2)).ReturnsAsync(true);

            var result = await _controller.UpdateSubCategory(1, new SubCategoryUpdatedRequest { Name = "Updated", CategoryId = 2 });

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            (ok.Value as ApiResponse<string>)!.IsSuccess.Should().BeTrue();
        }
    }
}
