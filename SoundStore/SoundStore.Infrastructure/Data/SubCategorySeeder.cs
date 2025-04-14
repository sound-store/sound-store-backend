using Microsoft.EntityFrameworkCore;
using SoundStore.Core.Entities;

namespace SoundStore.Infrastructure.Data
{
    public static class SubCategorySeeder
    {
        public static void SeedDataForSubCategory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubCategory>().HasData(
                new SubCategory
                {
                    Id = 1,
                    Name = "LOA DI ĐỘNG",
                    CategoryId = 1,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 2,
                    Name = "LOA NGHE TRONG NHÀ",
                    CategoryId = 1,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 3,
                    Name = "LIMITED EDITION",
                    CategoryId = 1,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 4,
                    Name = "TRUE WIRELESS",
                    CategoryId = 2,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 5,
                    Name = "ON-EAR",
                    CategoryId = 2,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 6,
                    Name = "OVER-EAR",
                    CategoryId = 2,
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new SubCategory
                {
                    Id = 7,
                    Name = "IN-EAR",
                    CategoryId = 2,
                    CreatedAt = new DateTime(2025, 4, 14)
                }
            );
        }
    }
}
