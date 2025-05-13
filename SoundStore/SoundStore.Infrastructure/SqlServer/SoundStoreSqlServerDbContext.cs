using Microsoft.EntityFrameworkCore;
using SoundStore.Domain.Entities;

namespace SoundStore.Infrastructure.SqlServer
{
    public class SoundStoreSqlServerDbContext : DbContext
    {
        public SoundStoreSqlServerDbContext()
        {
        }

        public SoundStoreSqlServerDbContext(DbContextOptions<SoundStoreSqlServerDbContext> options) : base(options)
        {   
        }

        public virtual DbSet<User> Users { get; set; }
        
        public virtual DbSet<Category> Categories { get; set; }
        
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
