using Microsoft.Extensions.Logging;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Exceptions;

namespace SoundStore.Service.Test
{
    public class CategoryServiceUnitTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILogger<CategorySerivce>> _mockLogger = new();
        private readonly Mock<IRepository<Category>> _mockCategoryRepository = new();

        private CategorySerivce CreateService()
        {
            _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(_mockCategoryRepository.Object);
            return new CategorySerivce(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        #region AddCategory_ShouldReturnTrue_WhenCategoryIsAddedSuccessfully
        [Fact]
        public async Task AddCategory_ShouldReturnTrue_WhenCategoryIsAddedSuccessfully()
        {
            // Arrange
            var name = "Electronics";
            var description = "Electronic items";
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category>().AsQueryable());
            _mockCategoryRepository.Setup(x => x.Add(It.IsAny<Category>()));
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddCategory(name, description);

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(x => x.Add(It.Is<Category>(c => c.Name == name && c.Description == description)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }
        #endregion

        #region AddCategory_ShouldThrowDuplicatedException_WhenCategoryNameExists
        [Fact]
        public async Task AddCategory_ShouldThrowDuplicatedException_WhenCategoryNameExists()
        {
            // Arrange
            var name = "Books";
            var description = "All books";
            var existing = new List<Category> { new Category { Name = "Books" } }.AsQueryable();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(existing);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<DuplicatedException>(() => service.AddCategory(name, description));
        }
        #endregion

        #region DeleteCategory_ShouldReturnTrue_WhenCategoryExists
        [Fact]
        public async Task DeleteCategory_ShouldReturnTrue_WhenCategoryExists()
        {
            // Arrange
            var id = 1;
            var category = new Category { Id = id, Name = "Toys" };
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category> { category }.AsQueryable());
            _mockCategoryRepository.Setup(x => x.Delete(category));
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.DeleteCategory(id);

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(x => x.Delete(category), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }
        #endregion

        #region DeleteCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist
        [Fact]
        public async Task DeleteCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 99;
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category>().AsQueryable());

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteCategory(id));
        }
        #endregion

        #region GetCategories_ShouldReturnPaginatedList_WhenCategoriesExist
        [Fact]
        public void GetCategories_ShouldReturnPaginatedList_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "A", Description = "DescA", CreatedAt = System.DateTime.Now, SubCategories = new List<SubCategory>() },
                    new Category { Id = 2, Name = "B", Description = "DescB", CreatedAt = System.DateTime.Now, SubCategories = new List<SubCategory>() }
                }.AsQueryable();

            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            var service = CreateService();

            // Act
            var result = service.GetCategories(null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Items.Count > 0);
        }
        #endregion

        #region GetCategories_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist
        [Fact]
        public void GetCategories_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(new List<Category>().AsQueryable());

            var service = CreateService();

            // Act & Assert
            Assert.Throws<NoDataRetrievalException>(() => service.GetCategories(null, 1, 10));
        }
        #endregion

        #region GetCategory_ShouldReturnCategoryResponse_WhenCategoryExists
        [Fact]
        public async Task GetCategory_ShouldReturnCategoryResponse_WhenCategoryExists()
        {
            // Arrange
            var id = 1;
            var category = new Category
            {
                Id = id,
                Name = "Clothing",
                Description = "Clothes",
                CreatedAt = System.DateTime.Now,
                SubCategories = new List<SubCategory>()
            };
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category> { category }.AsQueryable());

            var service = CreateService();

            // Act
            var result = await service.GetCategory(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Clothing", result.Name);
        }
        #endregion

        #region GetCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist
        [Fact]
        public async Task GetCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 100;
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category>().AsQueryable());

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetCategory(id));
        }
        #endregion

        #region UpdateCategory_ShouldReturnTrue_WhenCategoryIsUpdated
        [Fact]
        public async Task UpdateCategory_ShouldReturnTrue_WhenCategoryIsUpdated()
        {
            // Arrange
            var id = 1;
            var category = new Category { Id = id, Name = "Old", Description = "OldDesc" };
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category> { category }.AsQueryable());
            _mockCategoryRepository.Setup(x => x.Update(It.IsAny<Category>()));
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.UpdateCategory(id, "New", "NewDesc");

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(x => x.Update(It.Is<Category>(c => c.Name == "New" && c.Description == "NewDesc")), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }
        #endregion

        #region UpdateCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist
        [Fact]
        public async Task UpdateCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 2;
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(new List<Category>().AsQueryable());

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateCategory(id, "Name", "Desc"));
        }
        #endregion

        #region GetCategoriesList_ShouldReturnList_WhenCategoriesExist
        [Fact]
        public async Task GetCategoriesList_ShouldReturnList_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "A", SubCategories = new List<SubCategory>() },
                    new Category { Id = 2, Name = "B", SubCategories = new List<SubCategory>() }
                }.AsQueryable();

            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            var service = CreateService();

            // Act
            var result = await service.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        #endregion

        #region GetCategoriesList_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist
        [Fact]
        public async Task GetCategoriesList_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(new List<Category>().AsQueryable());

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NoDataRetrievalException>(() => service.GetCategories());
        }
        #endregion
    }
}
