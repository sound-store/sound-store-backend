using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SoundStore.Service.Test
{
    public class ProductRatingServiceUnitTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILogger<ProductRatingService>> _mockLogger = new();
        private readonly Mock<IUserClaimsService> _mockUserClaimsService = new();
        private readonly Mock<IRepository<Product>> _mockProductRepository = new();
        private readonly Mock<IRepository<Order>> _mockOrderRepository = new();

        private ProductRatingService CreateService()
        {
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Order>()).Returns(_mockOrderRepository.Object);
            return new ProductRatingService(_mockUnitOfWork.Object, _mockLogger.Object, _mockUserClaimsService.Object);
        }

        #region AddRating Test Suites

        [Fact]
        public async Task AddRating_ShouldReturnTrue_WhenRatingIsAddedSuccessfully()
        {
            // Arrange
            var userId = "user123";
            var productId = 1L;

            var request = new ProductRatingRequest
            {
                ProductId = productId,
                Point = 5,
                Comment = "Great product!"
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Headphones",
                Ratings = new List<Rating>()
            };

            var order = new Order
            {
                Id = 1,
                UserId = userId,
                OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { ProductId = productId }
                    }
            };

            // Mock user claims service
            _mockUserClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            // Mock product repository
            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            // Mock order repository
            var orders = new List<Order> { order }.AsQueryable().BuildMock();
            _mockOrderRepository.Setup(x => x.GetAll())
                .Returns(orders);

            // Mock save changes
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddRating(request);

            // Assert
            Assert.True(result);
            Assert.Single(product.Ratings);
            //Assert.Equal(request.Point, product.Ratings[0].RatingPoint);
            //Assert.Equal(request.Comment, product.Ratings[0].Comment);
            //Assert.Equal(userId, product.Ratings[0].UserId);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddRating_ShouldThrowUnauthorizedAccessException_WhenUserTokenIsInvalid()
        {
            // Arrange
            _mockUserClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns((string)null);

            var request = new ProductRatingRequest
            {
                ProductId = 1,
                Point = 5,
                Comment = "Great product!"
            };

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.AddRating(request));
            Assert.Equal("Invalid user's token!", exception.Message);
        }

        [Fact]
        public async Task AddRating_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var userId = "user123";
            var productId = 1L;

            var request = new ProductRatingRequest
            {
                ProductId = productId,
                Point = 5,
                Comment = "Great product!"
            };

            // Mock user claims service
            _mockUserClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            // Mock empty product repository
            var products = new List<Product>().AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.AddRating(request));
            Assert.Equal("Product not found!", exception.Message);
        }

        [Fact]
        public async Task AddRating_ShouldThrowException_WhenUserHasNotPurchasedProduct()
        {
            // Arrange
            var userId = "user123";
            var productId = 1L;

            var request = new ProductRatingRequest
            {
                ProductId = productId,
                Point = 5,
                Comment = "Great product!"
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Headphones",
                Ratings = new List<Rating>()
            };

            var order = new Order
            {
                Id = 1,
                UserId = userId,
                OrderDetails = new List<OrderDetail>
                {
                    // Different product ID
                    new OrderDetail { ProductId = 999 }
                }
            };

            // Mock user claims service
            _mockUserClaimsService.Setup(x => x.GetClaim(JwtRegisteredClaimNames.Sid))
                .Returns(userId);

            // Mock product repository
            var products = new List<Product> { product }.AsQueryable().BuildMock();
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(products);

            // Mock order repository with order that has different product
            var orders = new List<Order> { order }.AsQueryable().BuildMock();
            _mockOrderRepository.Setup(x => x.GetAll())
                .Returns(orders);

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.AddRating(request));
            Assert.Contains("haven't bought the product yet", exception.Message);
        }

        #endregion


        #region GetRatingOfAProduct Test Suites

        [Fact]
        public async Task GetRatingOfAProduct_ShouldReturnProductRatingResponse_WhenProductHasRatings()
        {
            // Arrange
            var productId = 1L;

            var ratings = new List<Rating>
            {
                new Rating { Id = 1, ProductId = productId, RatingPoint = 5, Comment = "Excellent!" },
                new Rating { Id = 2, ProductId = productId, RatingPoint = 4, Comment = "Very good." },
                new Rating { Id = 3, ProductId = productId, RatingPoint = 3, Comment = "It's okay." }
            };

            var product = new Product
            {
                Id = productId,
                Name = "Test Headphones",
                Ratings = ratings
            };

            // Create a mock ProductRatingResponse that the repository will return
            var expectedResponse = new ProductRatingResponse
            {
                ProductId = productId,
                RatingPoint = 4.0m, // (5+4+3)/3
                Comment = new List<string> { "Excellent!", "Very good.", "It's okay." }
            };

            // Setup mock repository to return the product
            var mockProductDbSet = new List<ProductRatingResponse> { expectedResponse }.AsQueryable().BuildMock();

            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            // Use the underlying LINQ query to mimic the database operation
            var service = CreateService();

            // Act
            var result = await service.GetRatingOfAProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.ProductId, result.ProductId);
            Assert.Equal(expectedResponse.RatingPoint, result.RatingPoint);
            Assert.Equal(expectedResponse.Comment.Count, result.Comment.Count);
            foreach (var comment in expectedResponse.Comment)
            {
                Assert.Contains(comment, result.Comment);
            }
        }

        [Fact]
        public async Task GetRatingOfAProduct_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999L;

            // Setup mock empty repository
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(new List<Product>().AsQueryable().BuildMock());

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.GetRatingOfAProduct(productId));
            Assert.Equal("No rating data for this product!", exception.Message);
        }

        [Fact]
        public async Task GetRatingOfAProduct_ShouldCalculateAverageRatingCorrectly()
        {
            // Arrange
            var productId = 1L;

            // Create ratings with varying points for accurate average calculation
            var ratings = new List<Rating>
            {
                new Rating { Id = 1, ProductId = productId, RatingPoint = 5, Comment = "Perfect!" },
                new Rating { Id = 2, ProductId = productId, RatingPoint = 2, Comment = "Not so good." },
                new Rating { Id = 3, ProductId = productId, RatingPoint = 3, Comment = "Average." },
                new Rating { Id = 4, ProductId = productId, RatingPoint = 4, Comment = "Good product." }
            };

            var expectedAverage = 3.5m; // (5+2+3+4)/4 = 3.5

            var product = new Product
            {
                Id = productId,
                Name = "Test Speaker",
                Ratings = ratings
            };

            // Create the expected response
            var expectedResponse = new ProductRatingResponse
            {
                ProductId = productId,
                RatingPoint = expectedAverage,
                Comment = ratings.Select(r => r.Comment!).ToList()
            };

            // Setup mock repository
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            var service = CreateService();

            // Act
            var result = await service.GetRatingOfAProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAverage, result.RatingPoint);
            Assert.Equal(4, result.Comment.Count); // Should have 4 comments
        }

        [Fact]
        public async Task GetRatingOfAProduct_ShouldHandleProductWithNoRatings()
        {
            // Arrange
            var productId = 1L;

            // Product exists but has no ratings
            var product = new Product
            {
                Id = productId,
                Name = "New Product",
                Ratings = new List<Rating>() // Empty ratings list
            };

            // Setup mock repository
            _mockProductRepository.Setup(x => x.GetAll())
                .Returns(new List<Product> { product }.AsQueryable().BuildMock());

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.GetRatingOfAProduct(productId));
            Assert.Equal("No rating data for this product!", exception.Message);
        }

        #endregion
    }
}
