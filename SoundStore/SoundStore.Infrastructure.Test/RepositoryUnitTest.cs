using Microsoft.EntityFrameworkCore;
using Moq;

namespace SoundStore.Infrastructure.Test
{
    public class RepositoryUnitTest
    {
        private readonly Mock<DbSet<DummyEntity>> _mockSet;
        private readonly Mock<SoundStoreDbContext> _mockContext;
        private readonly Repository<DummyEntity> _repository;

        public RepositoryUnitTest()
        {
            _mockSet = new Mock<DbSet<DummyEntity>>();
            _mockContext = new Mock<SoundStoreDbContext>();
            _mockContext.Setup(m => m.Set<DummyEntity>()).Returns(_mockSet.Object);
            _repository = new Repository<DummyEntity>(_mockContext.Object);
        }

        [Fact]
        public void Add_Calls_DbContext_Add()
        {
            var entity = new DummyEntity();
            _repository.Add(entity);
            _mockContext.Verify(m => m.Add(entity), Times.Once);
        }

        [Fact]
        public void AddRange_Calls_DbContext_AddRange()
        {
            var entities = new List<DummyEntity> { new(), new() };
            _repository.AddRange(entities);
            _mockContext.Verify(m => m.AddRange(entities), Times.Once);
        }

        [Fact]
        public void Delete_Calls_DbContext_Remove()
        {
            var entity = new DummyEntity();
            _repository.Delete(entity);
            _mockContext.Verify(m => m.Remove(entity), Times.Once);
        }

        [Fact]
        public void DeleteRange_Calls_DbContext_RemoveRange()
        {
            var entities = new List<DummyEntity> { new(), new() };
            _repository.DeleteRange(entities);
            _mockContext.Verify(m => m.RemoveRange(entities), Times.Once);
        }

        [Fact]
        public void Update_Calls_DbContext_Update()
        {
            var entity = new DummyEntity();
            _repository.Update(entity);
            _mockContext.Verify(m => m.Update(entity), Times.Once);
        }

        [Fact]
        public void UpdateRange_Calls_DbContext_UpdateRange()
        {
            var entities = new List<DummyEntity> { new(), new() };
            _repository.UpdateRange(entities);
            _mockContext.Verify(m => m.UpdateRange(entities), Times.Once);
        }

        [Fact]
        public async Task FindByIdAsync_Calls_DbSet_FindAsync()
        {
            var key = 1;
            var expectedEntity = new DummyEntity { Id = key };

            var mockDbSet = new Mock<DbSet<DummyEntity>>();
            mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                     .ReturnsAsync(expectedEntity);
            _mockContext.Setup(m => m.Set<DummyEntity>()).Returns(mockDbSet.Object);

            var repo = new Repository<DummyEntity>(_mockContext.Object);
            var result = await repo.FindByIdAsync(key);

            Assert.Equal(expectedEntity, result);
        }

        // Mocked DbSet<T> doesn't support LINQ without a full backing store or better abstraction.
        // TODO: Use EF Core's In-Memory Provider
        [Fact]
        public void GetAll_Returns_IQueryable()
        {
            var data = new List<DummyEntity>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<DummyEntity>>();
            mockSet.As<IQueryable<DummyEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<DummyEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<DummyEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<DummyEntity>>().Setup(m => m.GetEnumerator())
                .Returns(() => data.GetEnumerator());

            var mockContext = new Mock<SoundStoreDbContext>();
            mockContext.Setup(c => c.Set<DummyEntity>()).Returns(mockSet.Object);

            var repo = new Repository<DummyEntity>(mockContext.Object);

            var result = repo.GetAll();

            Assert.Equal(2, result.Count()); 
        }
    }

    public class DummyEntity
    {
        public int Id { get; set; }
    }
}
