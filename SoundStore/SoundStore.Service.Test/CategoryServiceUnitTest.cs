using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using SoundStore.Core;
using SoundStore.Core.Entities;
using SoundStore.Core.Exceptions;

namespace SoundStore.Service.Test
{
    public class CategoryServiceUnitTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILogger<CategoryService>> _mockLogger = new();
        private readonly Mock<IRepository<Category>> _mockCategoryRepository = new();

        private CategoryService CreateService()
        {
            _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(_mockCategoryRepository.Object);
            return new CategoryService(_mockUnitOfWork.Object, _mockLogger.Object);
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
            var categories = new List<Category> { category }.AsQueryable();

            // Build the mock IQueryable<Category> and return its .Object for GetAll()
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll())
                .Returns(mockCategories);

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
            var categories = new List<Category>().AsQueryable();

            // Use MockQueryable to create a mock async queryable
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

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
                SubCategories = []
            };
            var categories = new List<Category> { category }.AsQueryable();
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

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
            var categories = new List<Category>().AsQueryable();

            // Use MockQueryable to create a mock async queryable
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

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

            var categories = new List<Category> { category }.AsQueryable();
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();
            // Act
            var result = await service.UpdateCategory(id, "New", "NewDesc");

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(x => x.Update(It.Is<Category>(c => c.Name == "New" && c.Description == "NewDesc")),
                Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }
        #endregion

        #region UpdateCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist
        [Fact]
        public async Task UpdateCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = 2;
            var categories = new List<Category>().AsQueryable();

            // Use MockQueryable to create a mock async queryable
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

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
                new() { Id = 1, Name = "A", SubCategories = [] },
                new() { Id = 2, Name = "B", SubCategories = [] }
            }.AsQueryable();

            // Use MockQueryable to create a mock async queryable
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

            var service = CreateService();

            // Act
            var result = await service.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "A");
            Assert.Contains(result, r => r.Name == "B");
        }
        #endregion


        #region GetCategoriesList_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist
        [Fact]
        public async Task GetCategoriesList_ShouldThrowNoDataRetrievalException_WhenNoCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>();
            var mockCategories = categories.BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategories);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NoDataRetrievalException>(() => service.GetCategories());
        }
        #endregion

        #region AddSubCategory Tests

        [Fact]
        public async Task AddSubCategory_ShouldReturnTrue_WhenSubCategoryIsAddedSuccessfully()
        {
            // Arrange
            var categoryId = 1;
            var subCategoryName = "Guitars";

            // Mock the category repository
            var categories = new List<Category> { new() { Id = categoryId } }
                .AsQueryable()
                .BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            // Mock the subcategory repository
            var subCategories = new List<SubCategory>().AsQueryable().BuildMock();
            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories);

            _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(_mockCategoryRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddSubCategory(categoryId, subCategoryName);

            // Assert
            Assert.True(result);
            mockSubCategoryRepository.Verify(x => x.Add(It.Is<SubCategory>(sc => sc.Name == subCategoryName 
                && sc.CategoryId == categoryId)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }
        
        [Fact]
        public async Task AddSubCategory_ShouldThrowDuplicatedException_WhenSubCategoryNameExists()
        {
            // Arrange
            var categoryId = 1;
            var subCategoryName = "Guitars";

            var existingSubCategories = new List<SubCategory>
            {
                new() { Name = "Guitars" }
            }.AsQueryable();

            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll())
                .Returns(existingSubCategories);
            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>())
                .Returns(mockSubCategoryRepository.Object);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<DuplicatedException>(() => service.AddSubCategory(categoryId, subCategoryName));
        }

        [Fact]
        public async Task AddSubCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 99;
            var subCategoryName = "Guitars";

            // Mock the category repository with an empty list
            var categories = new List<Category>().AsQueryable().BuildMock();
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            // Mock the subcategory repository with an empty list
            var subCategories = new List<SubCategory>().AsQueryable().BuildMock();
            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories);

            _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(_mockCategoryRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AddSubCategory(categoryId, subCategoryName));
        }


        #endregion

        #region DeleteSubCategory Tests

        [Fact]
        public async Task DeleteSubCategory_ShouldReturnTrue_WhenSubCategoryExists()
        {
            // Arrange
            var subCategoryId = 1;
            var subCategory = new SubCategory { Id = subCategoryId, Name = "Guitars" };

            // Mock the subcategory repository with a list containing the subcategory
            var subCategories = new List<SubCategory> { subCategory }.AsQueryable().BuildMock();
            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories);

            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);
            _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.DeleteSubCategory(subCategoryId);

            // Assert
            Assert.True(result);
            mockSubCategoryRepository.Verify(x => x.Delete(It.Is<SubCategory>(sc => sc.Id == subCategoryId)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSubCategory_ShouldThrowKeyNotFoundException_WhenSubCategoryDoesNotExist()
        {
            // Arrange
            var subCategoryId = 99;

            // Mock the subcategory repository with an empty list
            var subCategories = new List<SubCategory>().AsQueryable().BuildMock();
            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories);

            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteSubCategory(subCategoryId));
        }

        #endregion

        #region UpdateSubCategory Tests

        //[Fact]
        //public async Task UpdateSubCategory_ShouldReturnTrue_WhenSubCategoryIsUpdatedSuccessfully()
        //{
        //    // Arrange
        //    var subCategoryId = 1;
        //    var categoryId = 2;
        //    var newName = "Updated SubCategory";
        //    var subCategory = new SubCategory { Id = subCategoryId, Name = "Old SubCategory", CategoryId = 1 };

        //    // Mock the subcategory repository with the existing subcategory
        //    var subCategories = new List<SubCategory> { subCategory }
        //        .AsQueryable()
        //        .BuildMockDbSet();
        //    var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
        //    mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories.Object);

        //    // Mock the category repository with the valid category
        //    var categories = new List<Category> { new Category { Id = categoryId } }
        //        .AsQueryable()
        //        .BuildMockDbSet();
        //    var mockCategoryRepository = new Mock<IRepository<Category>>();
        //    mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories.Object);

        //    _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);
        //    _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(mockCategoryRepository.Object);
        //    _mockUnitOfWork.Setup(x => x.SaveAsync(default)).ReturnsAsync(1);

        //    var service = CreateService();

        //    // Act
        //    var result = await service.UpdateSubCategory(subCategoryId, newName, categoryId);

        //    // Assert
        //    Assert.True(result);
        //    mockSubCategoryRepository.Verify(x => x.Update(It.Is<SubCategory>(sc =>
        //        sc.Id == subCategoryId &&
        //        sc.Name == newName &&
        //        sc.CategoryId == categoryId)), Times.Once);
        //    _mockUnitOfWork.Verify(x => x.SaveAsync(default), Times.Once);
        //}


        [Fact]
        public async Task UpdateSubCategory_ShouldThrowKeyNotFoundException_WhenSubCategoryDoesNotExist()
        {
            // Arrange
            var subCategoryId = 99;
            var newName = "Updated SubCategory";
            var categoryId = 1;

            // Mock the subcategory repository with an empty list
            var subCategories = new List<SubCategory>().AsQueryable().BuildMock();
            var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
            mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories);

            _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateSubCategory(subCategoryId, 
                newName, 
                categoryId));
        }

        //[Fact]
        //public async Task UpdateSubCategory_ShouldThrowKeyNotFoundException_WhenCategoryDoesNotExist()
        //{
        //    // Arrange
        //    var subCategoryId = 1;
        //    var categoryId = 99;
        //    var newName = "Updated SubCategory";
        //    var subCategory = new SubCategory { Id = subCategoryId, Name = "Old SubCategory", CategoryId = 1 };

        //    // Mock the subcategory repository with the existing subcategory
        //    var subCategories = new List<SubCategory> { subCategory }
        //        .AsQueryable()
        //        .BuildMockDbSet();
        //    var mockSubCategoryRepository = new Mock<IRepository<SubCategory>>();
        //    mockSubCategoryRepository.Setup(x => x.GetAll()).Returns(subCategories.Object);

        //    // Mock the category repository with an empty list
        //    var categories = new List<Category>()
        //        .AsQueryable()
        //        .BuildMockDbSet();
        //    var mockCategoryRepository = new Mock<IRepository<Category>>();
        //    mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories.Object);

        //    _mockUnitOfWork.Setup(x => x.GetRepository<SubCategory>()).Returns(mockSubCategoryRepository.Object);
        //    _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(mockCategoryRepository.Object);

        //    var service = CreateService();

        //    // Act & Assert
        //    await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateSubCategory(subCategoryId, newName, categoryId));
        //}

        #endregion

    }
}
