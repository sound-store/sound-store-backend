using SoundStore.Core;

namespace SoundStore.Infrastructure
{
    public class UnitOfWork(SoundStoreDbContext dbContext) : IUnitOfWork
    {
        private readonly SoundStoreDbContext _dbContext = dbContext;
        private Dictionary<Type, object> _repositories = [];

        public void Dispose() => _dbContext.Dispose();

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repository))
            {
                return (IRepository<T>)repository;
            }

            var newRepository = new Repository<T>(_dbContext);
            _repositories[typeof(T)] = newRepository;
            return newRepository;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
            => await _dbContext.SaveChangesAsync(cancellationToken);

    }
}
