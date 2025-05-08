using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Filters;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Services;

namespace SoundStore.Service.Test
{
    public class ProductServiceUnitTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILogger<ProductService>> _mockLogger = new();
        private readonly Mock<IImageService> _mockImageService = new();
        private readonly Mock<IRepository<Product>> _mockProductRepository = new();
        private readonly Mock<IRepository<Image>> _mockImageRepository = new();

        private ProductService CreateService()
        {
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Image>()).Returns(_mockImageRepository.Object);
            return new ProductService(_mockUnitOfWork.Object, _mockLogger.Object, _mockImageService.Object);
        }

        #region AddProduct Tests

        [Fact]
        public async Task AddProduct_ShouldReturnTrue_WhenProductIsAddedSuccessfully()
        {
            // Arrange
            var request = new ProductCreatedRequest
            {
                Name = "Test Headphones",
                Description = "High-quality headphones",
                StockQuantity = 10,
                Price = 99.99m,
                SubCategoryId = 1,
                Images = new List<IFormFile> { CreateMockFile("image.jpg") }
            };

            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            _mockImageService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("https://example.com/images/test.jpg");

            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddProduct(request);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
            _mockImageService.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ShouldThrowDuplicatedException_WhenProductNameExists()
        {
            // Arrange
            var request = new ProductCreatedRequest
            {
                Name = "Existing Product",
                Description = "Test Description",
                StockQuantity = 10,
                Price = 99.99m,
                SubCategoryId = 1
            };

            var products = new List<Product>
                {
                    new Product { Name = "Existing Product" }
                }.AsQueryable().BuildMock();

            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<DuplicatedException>(() => service.AddProduct(request));
        }

        [Fact]
        public async Task AddProduct_ShouldThrowException_WhenImageUploadFails()
        {
            // Arrange
            var request = new ProductCreatedRequest
            {
                Name = "New Headphones",
                Description = "Description",
                StockQuantity = 5,
                Price = 50m,
                SubCategoryId = 1,
                Images = new List<IFormFile> { CreateMockFile("image.jpg") }
            };

            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            _mockImageService.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((string)null);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.AddProduct(request));
        }

        #endregion

        #region GetProducts Tests

        [Fact]
        public void GetProducts_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = service.GetProducts(1, 10, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(1, result.PageNumber);
        }

        [Fact]
        public void GetProducts_ShouldFilterByName_WhenNameParameterIsProvided()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var parameters = new ProductFilterParameters
            {
                Name = "Blue"
            };

            var service = CreateService();

            // Act
            var result = service.GetProducts(1, 10, parameters, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Blue Headphones", result.Items[0].Name);
        }

        [Fact]
        public void GetProducts_ShouldFilterByStatus_WhenStatusParameterIsProvided()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var parameters = new ProductFilterParameters
            {
                Status = "OutOfStock"
            };

            var service = CreateService();

            // Act
            var result = service.GetProducts(1, 10, parameters, null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Red Headphones", result.Items[0].Name);
        }

        [Fact]
        public void GetProducts_ShouldSortByPriceAscending_WhenSortByPriceIsAsc()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = service.GetProducts(1, 10, null, "asc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
            Assert.Equal(49.99m, result.Items[0].Price);
            Assert.Equal(99.99m, result.Items[1].Price);
            Assert.Equal(149.99m, result.Items[2].Price);
        }

        [Fact]
        public void GetProducts_ShouldSortByPriceDescending_WhenSortByPriceIsDesc()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = service.GetProducts(1, 10, null, "desc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(149.99m, result.Items[0].Price);
            Assert.Equal(99.99m, result.Items[1].Price);
            Assert.Equal(49.99m, result.Items[2].Price);
        }

        [Fact]
        public void GetProducts_ShouldThrowNoDataRetrievalException_WhenNoProductsFound()
        {
            // Arrange
            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            Assert.Throws<NoDataRetrievalException>(() => service.GetProducts(1, 10, null, null));
        }

        #endregion

        #region GetProductByCategory Tests

        [Fact]
        public void GetProductByCategory_ShouldReturnProducts_WhenCategoryExists()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = service.GetProductByCategory(1, 10, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public void GetProductByCategory_ShouldThrowNoDataRetrievalException_WhenNoCategoryFound()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            Assert.Throws<NoDataRetrievalException>(() => service.GetProductByCategory(999, 10, 1));
        }

        #endregion

        #region GetProductBySubCategory Tests

        [Fact]
        public void GetProductBySubCategory_ShouldReturnProducts_WhenSubCategoryExists()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = service.GetProductBySubCategory(1, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public void GetProductBySubCategory_ShouldThrowNoDataRetrievalException_WhenNoSubCategoryFound()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            Assert.Throws<NoDataRetrievalException>(() => service.GetProductBySubCategory(999, 1, 10));
        }

        #endregion

        #region GetProduct Tests

        [Fact]
        public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act
            var result = await service.GetProduct(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Blue Headphones", result.Name);
            Assert.Equal(99.99m, result.Price);
        }

        [Fact]
        public async Task GetProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var products = CreateMockProducts().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetProduct(999));
        }

        #endregion

        #region DeleteProduct Tests

        [Fact]
        public async Task DeleteProduct_ShouldReturnTrue_WhenProductIsDeletedSuccessfully()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", OrderDetails = new List<OrderDetail>() };
            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.DeleteProduct(1);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(x => x.Delete(It.Is<Product>(p => p.Id == 1)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteProduct(1));
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowException_WhenProductHasOrderDetails()
        {
            // Arrange
            var orderDetails = new List<OrderDetail> { new OrderDetail() };
            var product = new Product { Id = 1, Name = "Test Product", OrderDetails = orderDetails };
            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteProduct(1));
        }

        #endregion

        #region UpdateProduct Tests

        [Fact]
        public async Task UpdateProduct_ShouldReturnTrue_WhenProductIsUpdatedSuccessfully()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Old Name" };
            var request = new ProductUpdatedRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                StockQuantity = 20,
                Price = 199.99m,
                SubCategoryId = 2,
                Status = "InStock"
            };

            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.UpdateProduct(1, request);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(x => x.Update(It.Is<Product>(p => p.Id == 1 && p.Name == "Updated Name")), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var request = new ProductUpdatedRequest
            {
                Name = "Updated Name",
                Description = "Description",
                Status = "InStock",
                StockQuantity = 10,
                Price = 99.99m,
                SubCategoryId = 1
            };

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateProduct(1, request));
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowArgumentException_WhenStatusIsInvalid()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Old Name" };
            var request = new ProductUpdatedRequest
            {
                Name = "Updated Name",
                Description = "Description",
                Status = "InvalidStatus",
                StockQuantity = 10,
                Price = 99.99m,
                SubCategoryId = 1
            };

            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateProduct(1, request));
        }

        #endregion

        #region Helper Methods

        private IFormFile CreateMockFile(string fileName)
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
            fileMock.Setup(x => x.FileName).Returns(fileName);
            fileMock.Setup(x => x.Length).Returns(ms.Length);

            return fileMock.Object;
        }

        private IQueryable<Product> CreateMockProducts()
        {
            var category = new Category { Id = 1, Name = "Audio" };
            var subCategory1 = new SubCategory { Id = 1, Name = "Headphones", Category = category, CategoryId = 1 };
            var subCategory2 = new SubCategory { Id = 2, Name = "Speakers", Category = category, CategoryId = 1 };

            var products = new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Blue Headphones",
                        Description = "High-quality headphones",
                        Price = 99.99m,
                        StockQuantity = 10,
                        Status = ProductState.InStock,
                        SubCategoryId = 1,
                        SubCategory = subCategory1,
                        Images = new List<Image>
                        {
                            new Image { Id = 1, Url = "https://example.com/blue.jpg", ProductId = 1 }
                        },
                        Ratings = new List<Rating>
                        {
                            new Rating
                            {
                                Id = 1,
                                RatingPoint = 5,
                                Comment = "Great product",
                                User = new AppUser { FirstName = "John", LastName = "Doe" }
                            }
                        }
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Red Headphones",
                        Description = "Wireless headphones",
                        Price = 149.99m,
                        StockQuantity = 0,
                        Status = ProductState.OutOfStock,
                        SubCategoryId = 1,
                        SubCategory = subCategory1,
                        Images = new List<Image>
                        {
                            new Image { Id = 2, Url = "https://example.com/red.jpg", ProductId = 2 }
                        }
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Portable Speaker",
                        Description = "Bluetooth speaker",
                        Price = 49.99m,
                        StockQuantity = 20,
                        Status = ProductState.InStock,
                        SubCategoryId = 2,
                        SubCategory = subCategory2,
                        Images = new List<Image>
                        {
                            new Image { Id = 3, Url = "https://example.com/speaker.jpg", ProductId = 3 }
                        }
                    }
                };

            return products.AsQueryable();
        }

        #endregion
    }
}
