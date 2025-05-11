using Moq;
using SoundStore.Core.Entities;

namespace SoundStore.Infrastructure.Test
{
    public class UnitOfWorkUnitTest
    {
        private readonly Mock<SoundStoreDbContext> _mockDbContext;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkUnitTest()
        {
            _mockDbContext = new Mock<SoundStoreDbContext>();
            _unitOfWork = new UnitOfWork(_mockDbContext.Object);
        }

        [Fact]
        public void GetRepository_ShouldReturnSameInstance_ForSameType()
        {
            // Act
            var repo1 = _unitOfWork.GetRepository<Product>();
            var repo2 = _unitOfWork.GetRepository<Product>();

            // Assert
            Assert.NotNull(repo1);
            Assert.Same(repo1, repo2);
        }

        [Fact]
        public void GetRepository_ShouldReturnNewRepository_ForNewType()
        {
            // Act
            var repo1 = _unitOfWork.GetRepository<Product>();
            var repo2 = _unitOfWork.GetRepository<Order>();

            // Assert
            Assert.NotNull(repo1);
            Assert.NotNull(repo2);
            Assert.IsType<Repository<Product>>(repo1);
            Assert.IsType<Repository<Order>>(repo2);
            Assert.NotSame(repo1, repo2);
        }

        [Fact]
        public async Task SaveAsync_ShouldCallDbContextSaveChangesAsync()
        {
            // Arrange
            _mockDbContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(1);

            // Act
            var result = await _unitOfWork.SaveAsync();

            // Assert
            Assert.Equal(1, result);
            _mockDbContext.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldCallDbContextDispose()
        {
            // Act
            _unitOfWork.Dispose();

            // Assert
            _mockDbContext.Verify(d => d.Dispose(), Times.Once);
        }
    }
}
