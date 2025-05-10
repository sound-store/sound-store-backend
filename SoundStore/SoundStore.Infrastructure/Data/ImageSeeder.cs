using Microsoft.EntityFrameworkCore;
using SoundStore.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    public static class ImageSeeder
    {
        public static void SeedDataForImage(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().HasData(
                new Image
                {
                    Id = 1,
                    ProductId = 1,
                    Url = "https://soundway.vn/wp-content/uploads/2024/10/2.png",
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new Image
                {
                    Id = 2,
                    ProductId = 1,
                    Url = "https://soundway.vn/wp-content/uploads/2024/10/major-v-two-asset-hybrid-09.png",
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new Image
                {
                    Id = 3,
                    ProductId = 1,
                    Url = "https://soundway.vn/wp-content/uploads/2024/10/major-v-two-asset-hybrid-03-lon.png",
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new Image
                {
                    Id = 4,
                    ProductId = 1,
                    Url = "https://soundway.vn/wp-content/uploads/2024/10/5-major-v-brown-lifestyle-desktop-trung-binh.png",
                    CreatedAt = new DateTime(2025, 4, 14)
                },
                new Image
                {
                    Id = 5,
                    ProductId = 2,
                    Url = "https://soundway.vn/wp-content/uploads/2021/09/Tang-ngay-01-goi-Marshall-11.png",
                    CreatedAt = new DateTime(2025, 4, 14)
                }
            );
        }
    }
}
