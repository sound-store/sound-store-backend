using Microsoft.EntityFrameworkCore;
using SoundStore.Core;

namespace SoundStore.Infrastructure
{
    public class Repository<T>(SoundStoreDbContext dbContext) : IRepository<T> where T : class
    {
        private readonly SoundStoreDbContext _dbContext = dbContext;
        private DbSet<T> _dbSet = dbContext.Set<T>();

        public void Add(T entity) => _dbContext.Add(entity);

        public void AddRange(IEnumerable<T> entities) => _dbContext.AddRange(entities);

        public void Delete(T entity) => _dbContext.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _dbContext.RemoveRange(entities);

        public async Task<T?> FindByIdAsync(params object[] keyValues)
            => await _dbSet.FindAsync(keyValues);

        public IQueryable<T> GetAll() => _dbSet.AsQueryable();

        public void Update(T entity) => _dbContext.Update(entity);

        public void UpdateRange(IEnumerable<T> entities) => _dbContext.UpdateRange(entities);
    }
}
