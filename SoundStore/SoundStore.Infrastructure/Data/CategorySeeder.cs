using Microsoft.EntityFrameworkCore;
using SoundStore.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public static class CategorySeeder
    {
        public static void SeedDataForCategory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "LOA MARSHALL",
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                 new Category
                 {
                     Id = 2,
                     Name = "TAI NGHE MARSHALL",
                     CreatedAt = new DateTime(2025, 4, 14)
                 },
                  new Category
                  {
                      Id = 3,
                      Name = "PHỤ KIỆN LIFESTYLE",
                      CreatedAt = new DateTime(2025, 4, 14)
                  }
            );
        }
    }
}
